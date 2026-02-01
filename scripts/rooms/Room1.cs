using Godot;
using System;
using System.Collections.Generic;

public partial class Room1 : Node3D
{
	// Called when the node enters the scene tree for the first time.

	private Area3D _roomArea => GetNode<Area3D>("Area");
	private CollisionShape3D _roomCollision => _roomArea.GetNode<CollisionShape3D>("Collision");

	private bool _voiceLinePlayed = false;

	#region sounds
	private AudioStreamPlayer3D _vl1 => GetNode<AudioStreamPlayer3D>("VL1");
	private AudioStreamPlayer3D _vl2 => GetNode<AudioStreamPlayer3D>("VL2");
	private AudioStreamPlayer3D _vl3 => GetNode<AudioStreamPlayer3D>("VL3");
	private AudioStreamPlayer3D _vl4 => GetNode<AudioStreamPlayer3D>("VL4");

	private List<AudioStreamPlayer3D> _voiceLines => new List<AudioStreamPlayer3D>{
		_vl1,
		_vl2,
		_vl3,
		_vl4
	};
	#endregion
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool IsPlayerInRoomArea(Player player)
	{
		Godot.Collections.Array<Node3D> bodies = _roomArea.GetOverlappingBodies();

		if (bodies.Contains(player))
		{
			return true;
		}
		return false;
	}

	public void PlayVoiceLine()
	{
		
		GD.Randomize();
		var randomVoiceLine = (int)(GD.Randi() % 4);
		if (!_voiceLinePlayed)
		{
			_voiceLines[randomVoiceLine].Play();
			_voiceLinePlayed = true;
		}
	}

	public void ResetVoiceLineWhenPlayerLeavesArea(Player player)
	{
		Godot.Collections.Array<Node3D> bodies = _roomArea.GetOverlappingBodies();

		if (!bodies.Contains(player))
		{
			_voiceLinePlayed = false;
		}
	}
}
