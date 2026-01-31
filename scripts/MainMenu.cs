using Godot;
using System;

public partial class MainMenu : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// PLAY BUTTON
		Button playButton = GetNode<Control>("ButtonManager").GetNode<Button>("Play");
		playButton.Pressed += Play;

		// SETTINGS BUTTON @TODO what will be our settings?

		// QUIT BUTTON
		Button quitButton = GetNode<Control>("ButtonManager").GetNode<Button>("Quit");
		quitButton.Pressed += () => GetTree().Quit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void Play()
	{
		GetTree().ChangeSceneToFile("res://scenes/Main_World.tscn");
	}
	
}
