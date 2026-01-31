using Godot;
using System;

public partial class MainWorld : Node3D
{
	private PauseMenu _pauseMenu => GetNode<PauseMenu>("PauseMenu");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_pauseMenu.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// PAUSE MENU ON ESCAPE
		if (Input.IsActionPressed("pause_menu"))
		{
			LoadPauseMenu();
		}
	}

	private void LoadPauseMenu()
	{
		// CHANGE MOUSE POINTER MODE TO VISIBLE
		Input.MouseMode = Input.MouseModeEnum.Visible;

		// SHOW THE PAUSE MENU
		_pauseMenu.Visible = true;

		// PAUSE THE GAME
		Pause(true);
	}

	private void Pause( bool pause)
	{
		GetTree().Paused = pause;	
	}
}
