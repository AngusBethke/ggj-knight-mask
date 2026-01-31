using Godot;
using System;

public partial class MainWorld : Node3D
{
	#region Nodes
	private PauseMenu _pauseMenu => GetNode<PauseMenu>("PauseMenu");

	private Node3D _playerScene => GetNode<Node3D>("Player");
	private Player _player => _playerScene.GetNode<Player>("Player");


	private Building _building => GetNode<Building>("Building");
	private QuestObjects _questObjects => _building.GetNode<QuestObjects>("QuestObjects");
	private Level1 _level1 => _questObjects.GetNode<Level1>("Level1");

	private Walls _walls => _level1.GetNode<Walls>("Walls");
	#endregion

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

		#region Objective Walls Logic
		if (_player.IsWearingMask())
		{
			_walls.EnableObjectiveWalls();
		}
		else
		{
			_walls.DisableObjectiveWalls();
		}
		#endregion

		#region End Zone Logic
		Areas areas = GetNode<Areas>("Areas");
		if (areas.isPlayerInEndZone(_player))
		{
		
			Input.MouseMode = Input.MouseModeEnum.Visible;
			GetTree().ChangeSceneToFile("res://ui/menus/End_Screen.tscn");

		}
		#endregion
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

	private void Pause(bool pause)
	{
		GetTree().Paused = pause;
	}
}
