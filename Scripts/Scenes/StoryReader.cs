using Godot;
using Storyder.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class StoryReader : Control
{

	public Story CurrentStory { get; private set; } = new Story() {
		Title = "My First Story",
		Steps = new List<Step>() {
			new Step() { Id = 0, NextStepIds = new List<int>() { 1, 2 }, Content = "The story begins."},
			new Step() { Id = 1, NextStepIds = new List<int>() { 2 }, Content = "The second page."},
			new Step() { Id = 2, NextStepIds = new List<int>() { 1, 0 }, Content = "The third page."},
		}
	};

	private int _currentStepId = 0;

	public RichTextLabel TextField;

	public ItemList ChoicesField;

	public StoryReader()
	{
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TextField = GetNode("TextPanel/Text") as RichTextLabel;
		ChoicesField = GetNode("Choices") as ItemList;
		GoToStep(0);
	}

	private void NextStep()
	{
		GoToStep(_currentStepId + 1);
	}

	private void GoToStep(int i)
	{
		if (i >= CurrentStory.Steps.Count || i < 0)
		{
			_currentStepId = 0;
		}
		else
		{
			_currentStepId = i;
		}
		Step s = CurrentStory.Steps[_currentStepId];
		TextField.BbcodeText = s.Content;
		ChoicesField.Clear();
		foreach (var next in s.NextStepIds)
		{
			ChoicesField.AddItem($"Go to : {next}");
		}

	}

	private void _on_Text_gui_input(object @event)
	{
		// Replace with function body.
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsPressed())
			{
				NextStep();
			}
		}
	}
	
	private void _on_Choises_item_selected(int index)
	{
		Step s = CurrentStory.Steps[_currentStepId];
		GoToStep(s.NextStepIds[index]);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}




