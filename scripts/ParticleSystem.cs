using Godot;
using System;

public partial class ParticleSystem : Node3D
{
	// Called when the node enters the scene tree for the first time.
	private Node3D _firstDoor => GetNode<Node3D>("Particles").GetNode<Node3D>("MagicDoor");
	private GpuParticles3D _firstDoorParticles => _firstDoor.GetNode<GpuParticles3D>("MagicDoorParticleEngine");

	private Node3D _endDoor => GetNode<Node3D>("Particles").GetNode<Node3D>("MagicDoorEnd");
	private GpuParticles3D _endDoorParticles => _endDoor.GetNode<GpuParticles3D>("MagicDoorParticleEngine");
	public override void _Ready()
	{
		// initially set the end door particles to on
		_firstDoorParticles.Emitting = true;
		_firstDoorParticles.Visible = true;

		// initially set the first door particles to off
		_endDoorParticles.Emitting = false;
		_endDoorParticles.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ToggleDoorParticles(bool wearingMask)
	{
		_firstDoorParticles.Emitting = wearingMask;
		_firstDoorParticles.Visible = wearingMask;

		_endDoorParticles.Emitting = !wearingMask;
		_endDoorParticles.Visible = !wearingMask;
	}
}
