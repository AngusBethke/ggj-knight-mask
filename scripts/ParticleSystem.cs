using Godot;
using System;

public partial class ParticleSystem : Node3D
{
	#region Door Particle Nodes
	private Node3D _firstDoor => GetNode<Node3D>("Particles").GetNode<Node3D>("MagicDoor");
	private GpuParticles3D _firstDoorParticles => _firstDoor.GetNode<GpuParticles3D>("MagicDoorParticleEngine");

	private Node3D _endDoor => GetNode<Node3D>("Particles").GetNode<Node3D>("MagicDoorEnd");
	private GpuParticles3D _endDoorParticles => _endDoor.GetNode<GpuParticles3D>("MagicDoorParticleEngine");

	#endregion

	#region Dragon Particle Nodes
	private Node3D _dragonFireNode => GetNode<Node3D>("Particles").GetNode<Node3D>("DragonFire");
	private GpuParticles3D _dragonFireParticles => _dragonFireNode.GetNode<GpuParticles3D>("DragonBreathParticleEngine");

	private GpuParticles3D _dragonFloorParticles => _dragonFireParticles.GetNode<GpuParticles3D>("DragonBurning-on-floorParticleEngine");

	// dragon fire ported
	DragonFireAnimationPorted _dragonFirePorted => _dragonFireNode.GetNode<DragonFireAnimationPorted>("DragonBreathParticleEngine");
	#endregion
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
		
		_dragonFirePorted.HandleAttack(delta);
	}

	public void ToggleDoorParticles(bool wearingMask)
	{
		_firstDoorParticles.Emitting = wearingMask;
		_firstDoorParticles.Visible = wearingMask;

		_endDoorParticles.Emitting = !wearingMask;
		_endDoorParticles.Visible = !wearingMask;
	}

	public bool IsPlayerInDragonFire(Player player)
	{
		Area3D dragonArea = _dragonFireNode.GetNode<Area3D>("DragonArea");

		
		Godot.Collections.Array<Node3D> bodies = dragonArea.GetOverlappingBodies();

		if (bodies.Contains(player))
		{
			// check if particles are intersecting with the area
			var IsDragonAttacking = _dragonFirePorted.IsDragonAttacking();
			if (IsDragonAttacking)
			{
				return true;
			}
		}
		return false;
	}
}
