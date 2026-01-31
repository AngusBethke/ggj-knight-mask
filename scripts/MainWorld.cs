using Godot;
using System;

public partial class MainWorld : Node3D
{
	private PauseMenu PauseMenu => GetNode<PauseMenu>("PauseMenu");

	private Node3D _jonoWorld => GetNode<Node3D>("Jono_World");
	private Node3D _tillyWorld => GetNode<Node3D>("TillyWorld");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PauseMenu.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// PAUSE MENU ON ESCAPE
		if (Input.IsActionPressed("pause_menu"))
		{
			_loadPauseMenu();
		}
	}

	private void _loadPauseMenu()
	{
		// CHANGE MOUSE POINTER MODE TO VISIBLE
		Input.MouseMode = Input.MouseModeEnum.Visible;
		_pause(_jonoWorld, true);
		_pause(_tillyWorld, true);
		PauseMenu.Visible = true;
	}

	private void _pause(Node3D world, bool pause)
	{
		
		world.GetTree().Paused = pause;
		
	}
}
