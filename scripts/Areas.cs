using Godot;
using System;

public partial class Areas : Node3D
{
	#region Nodes

	private Area3D _endZone => GetNode<Area3D>("EndZone");
	private CollisionShape3D _endZoneCollisionShape => _endZone.GetNode<CollisionShape3D>("CollisionShape3D");
	#endregion
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	#region Area Functions
	public bool isPlayerInEndZone(Player player)
	{
		var ContainsPlayer = _endZone.GetOverlappingBodies().Contains(player);
		return ContainsPlayer;
	}
	#endregion
}
