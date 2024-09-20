using DocumentFormat.OpenXml.Drawing;
using Godot;
using Storyder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Weaver.Heroes.Body;
using Weaver.Heroes.Body.Value;
using Weaver.Tales;

public partial class StoryReader : Control
{
	private RichTextLabel textDisplay;
	private RichTextLabel characterDisplay;
	private AudioStreamPlayer audioPlayer;
	private PictureDisplay pictureDisplay;
	private Control pictureLayout;

	private StoryParagraph _currentStoryParagraph;
	private List<IStoryChoice> _currentChoices = new();
	private string _appendedText = "";


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		textDisplay = this.GetNode<RichTextLabel>("VBoxContainer/TextLayout/TextDisplay");
		pictureDisplay = this.GetNode<PictureDisplay>("VBoxContainer/PictureLayout/PictureDisplay");
		pictureLayout = this.GetNode<Control>("VBoxContainer/PictureLayout");
		audioPlayer = this.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		characterDisplay = this.GetNode<RichTextLabel>("CharacterPanel/CharacterSheet");
		

		// FileData.CreateDefaultExcel("Template.xlsx");
		Game.Static.Init();
		InitDisplayCharacter(Game.Static.HeroModule);
		DisplayCharacter(Game.Static.HeroModule);
		// GetStory("Ressources/LaSorciereDesNeiges/Story.xlsx");
		GetStory("Ressources/Test/Story.xlsx");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if(keyEvent.PhysicalKeycode >= Key.Key0 
				&& keyEvent.PhysicalKeycode <= Key.Key9)
			{
				int index = (int)(keyEvent.PhysicalKeycode - Key.Key0);
				index = index==0 ? 9 : index-1;
				if(index < _currentChoices.Count)
				{
					SetChoice(index);
				}
			}
		}
	}

    public void InitDisplayCharacter(Module module)
	{
		characterDisplay.Text = module.ModuleName + "\n";
		foreach(Module m in module.GetChildren<Module>())
		{
			if(m is IValue<int> mi) {
				mi.OnValueChanged += ActualiseValues;
			}
			else if(m is IValue<string> ms) {
				ms.OnValueChanged += ActualiseValues;
			}
		}
		module.OnRegisteredModule += RegisterToNewModule;
		module.OnRegisteredModule += UnregisterToNewModule;
	}

    private void RegisterToNewModule(Module module)
    {
        if(module is ValueModule<int> vi) {
			GD.Print(" * Register INT : ", module.ModuleName);
			vi.OnValueChanged += ActualiseValues;
		}
        else if(module is ValueModule<string> vs) {
			GD.Print(" * Register STRING : ", module.ModuleName);
			vs.OnValueChanged += ActualiseValues;
		}
        else if(module is ValueModule<List<int>> vli) {
			GD.Print(" * Register LINT : ", module.ModuleName);
			vli.OnValueChanged += ActualiseValues;
		}
        else if(module is ValueModule<List<string>> vls) {
			GD.Print(" * Register LSTRING : ", module.ModuleName);
			vls.OnValueChanged += ActualiseValues;
		}
    }

    private void UnregisterToNewModule(Module module)
    {
        if(module is ValueModule<object> vi) {
			vi.OnValueChanged -= ActualiseValues;
		}
    }

    public void ActualiseValues<T>(IValue<T> sender)
    {
		DisplayCharacter(Game.Static.BaseModule.GetRegistered("Hero"));
    }

    public void DisplayCharacter(Module module)
	{
		characterDisplay.Text = module.ModuleName + "\n";
		foreach(Module m in module.GetChildren<Module>())
		{
			if(m is IValue<int> mi) {
				characterDisplay.Text += string.Format("   {0} : {1}\n" , m.ModuleName, mi.Value);
			}
			else if(m is IValue<string> ms) {
				characterDisplay.Text += string.Format("   {0} : {1}\n" , m.ModuleName, ms.Value);
			}
			else if(m is IValue<List<int>> mli) {
				characterDisplay.Text += string.Format("   {0} : \n" , m.ModuleName);
				foreach(int i in mli.Value) {
					characterDisplay.Text += string.Format("   - {0}\n" , i);
				}
			}
			else if(m is IValue<List<string>> mls) {
				characterDisplay.Text += string.Format("   {0} : \n" , m.ModuleName);
				foreach(string i in mls.Value) {
					characterDisplay.Text += string.Format("   - {0}\n" , i);
				}
			}
			else {
				characterDisplay.Text += string.Format(" {0} \n" , m.ModuleName);
			}
		}
	}

    public void GetStory(string filepath)
	{
		FileInfo fileInfo = new FileInfo(filepath);
		if(!fileInfo.Exists)
			Log.LogErr("Cant find file at '{0}'.", fileInfo.FullName);
		else if (fileInfo.Extension != ".xlsx")
			Log.LogErr("Given file is not a excel file : '{0}'.", fileInfo.FullName);
		else
		{
			StoryderProperties.RessourcePath = fileInfo.DirectoryName;
			var story = FileData.ReadFromExcel(filepath);
			SetStoryChunk(story.Paragraphs.ElementAt(0));
		}
	}

	public void SetStoryChunk(StoryParagraph storyParagraph)
	{
		_currentStoryParagraph = storyParagraph;
		_currentChoices.Clear();
		_currentChoices.AddRange(storyParagraph.Choices);
		_appendedText = "";

		// Actuate Pre-Effects
		foreach (var effect in storyParagraph.Effects)
		{
			effect.Actuate(this);
		}

		// Check for end game
		Game.Static.System.DoEffectChecks(this);

		// Display Text
		textDisplay.Text = storyParagraph.Text + "\n" + _appendedText + "\n\n";
		int index = 0;
		foreach(var choice in _currentChoices)
		{
			textDisplay.Text += string.Format("\t[url={0}]{2}. {1}[/url]\n", index++, choice.Text, index);
		}
		textDisplay.ScrollToLine(0);
	}

	public void _on_text_meta_clicked(string meta)
	{
		int index = int.Parse(meta);
		SetChoice(index);
	}

	private void SetChoice(int choiceIndex)
	{
		// Actuate Post-Effects
		foreach (var effect in _currentStoryParagraph.PostEffects)
		{
			effect.Actuate(this);
		}

		// Go to next paragraph
		SetStoryChunk(_currentChoices[choiceIndex].Next);
	}

	public void SetPicture(FileInfo filepath)
	{
		Texture2D text = GD.Load<Texture2D>(filepath.ToString());
		pictureDisplay.SetNewTexture(text);
	}

	public void SetMusic(FileInfo filepath)
	{
		AudioStream stream = GD.Load<AudioStream>(filepath.ToString());
		((AudioStreamOggVorbis) stream).Loop = true;
		// ((AudioStreamWav) stream).LoopMode = AudioStreamWav.LoopModeEnum.Forward;
		audioPlayer.Stream = stream;
		audioPlayer.Play();
	}

    public void HidePicturePanel(bool hide)
    {
        pictureLayout.Visible = !hide;
    }

	public void AppendText(string text)
	{
		_appendedText += text;
	}

	public void AppendText(string text, params object[] args)
	{
		_appendedText += string.Format(text + "\n", args);
	}

	public void AppendChoice(IStoryChoice newChoice)
	{
		_currentChoices.Add(newChoice);
	}

	public void SetEndGame(bool victory) {
		_currentChoices.Clear();
		_currentChoices.Add(new StoryChoice() {
			Text= victory? "Victory" : "Game Over",
			Next = null // TODO maybe instructions on game over (retry, blahblahblah) and credits on victory ?
		});
		// TODO And then, return to main menu when there is one
	}
}
