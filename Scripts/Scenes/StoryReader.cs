using DocumentFormat.OpenXml.Drawing;
using Godot;
using Storyder;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public partial class StoryReader : Control
{
	private RichTextLabel textDisplay;
	private AudioStreamPlayer audioPlayer;
	private PictureDisplay pictureDisplay;
	private Control pictureLayout;

	private StoryParagraph _currentStoryChunk;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		textDisplay = this.GetNode<RichTextLabel>("VBoxContainer/TextLayout/TextDisplay");
		pictureDisplay = this.GetNode<PictureDisplay>("VBoxContainer/PictureLayout/PictureDisplay");
		pictureLayout = this.GetNode<Control>("VBoxContainer/PictureLayout");
		audioPlayer = this.GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		// FileData.CreateDefaultExcel("Template.xlsx");
		GetStory("Ressources/LaSorciereDesNeiges/Story.xlsx");
		// GetStory("Ressources/Test/Story.xlsx");
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

	public void SetStoryChunk(StoryParagraph storyChunk)
	{
		_currentStoryChunk = storyChunk;

		// Actuate Post-Effects
		foreach (var effect in storyChunk.PostEffects)
		{
			effect.Actuate(this);
		}

		// Display Text
		textDisplay.Text = storyChunk.Text + "\n\n";
		int index = 0;
		foreach(var choice in storyChunk.Choices)
		{
			textDisplay.Text += string.Format("\t[url={0}]{1}[/url]\n", index++, choice.Text);
		}
		textDisplay.ScrollToLine(0);
	}

	public void _on_text_meta_clicked(string meta)
	{
		int index = int.Parse(meta);
		SetStoryChunk(_currentStoryChunk.Choices[index].Next);
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
}
