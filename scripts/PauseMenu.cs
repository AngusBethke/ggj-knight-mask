using Godot;
using System;

public partial class PauseMenu : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		ProcessMode = ProcessModeEnum.WhenPaused;
		
		// CONTINUE BUTTON
		Button continueButton = GetNode<Control>("ButtonManager").GetNode<Button>("Continue");
		continueButton.Pressed += _Continue;

		// MAIN MENYU BUTTON
		Button mainMenuButton = GetNode<Control>("ButtonManager").GetNode<Button>("MainMenu");
		mainMenuButton.Pressed += _mainMenu;

		// QUIT BUTTON
		Button quitButton = GetNode<Control>("ButtonManager").GetNode<Button>("Quit");
		quitButton.Pressed += () => GetTree().Quit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
	}

	private void _Continue()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		Visible = false;
	}
	
	private void _mainMenu()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://ui/menus/Main_Menu.tscn");
	}
}
