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
	
	private AudioStreamPlayer3D _vl5 => GetNode<AudioStreamPlayer3D>("VL5");

	private AudioStreamPlayer3D _vl6 => GetNode<AudioStreamPlayer3D>("VL6");
	private AudioStreamPlayer3D _vl7 => GetNode<AudioStreamPlayer3D>("VL7");
	private AudioStreamPlayer3D _vl8 => GetNode<AudioStreamPlayer3D>("VL8");
	
	private AudioStreamPlayer3D _vl9 => GetNode<AudioStreamPlayer3D>("VL9");
	private List<AudioStreamPlayer3D> _voiceLines => new List<AudioStreamPlayer3D>{
		_vl1,
		_vl2,
		_vl3,
		_vl4,
		_vl5,
		_vl6,
		_vl7,
		_vl8,
		_vl9
	};

	private int _playedIndex = 0;
    #endregion

    public override void _Ready()
    {
        GD.Randomize();
		var sessionRandom = GD.Randi() % 9;
		_playedIndex = (int)sessionRandom;
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
		if(_playedIndex > _voiceLines.Count)
		{
			_playedIndex = 0;
		}
		if (!_voiceLinePlayed)
		{
			_voiceLines[_playedIndex].Play();
			_voiceLinePlayed = true;
		}
		_playedIndex++;
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
