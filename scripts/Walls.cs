using Godot;
using System;

public partial class Walls : Node3D
{
	#region Nodes

	// First solid wall
	private Node3D _solidWall1 => GetNode<Node3D>("CastleWall");
	private CollisionShape3D _solidWall1Collision => GetNode<Node3D>("CastleWall").GetNode<MeshInstance3D>("Base").GetNode<StaticBody3D>("StaticBody3D").GetNode<CollisionShape3D>("CollisionShape3D");

	// First hidden wall
	private Node3D _hiddenWall1 => GetNode<Node3D>("CastleWallBottomA");
	private CollisionShape3D _hiddenWall1Collision => GetNode<Node3D>("CastleWallBottomA").GetNode<MeshInstance3D>("CastleWallSecret").GetNode<StaticBody3D>("StaticBody3D").GetNode<CollisionShape3D>("CollisionShape3D");

	// End zone solid wall
	private Node3D _endZoneWallNode => GetNode<Node3D>("CastleWallDoorClosed");
	private CollisionShape3D _endZoneWallCollision => GetNode<Node3D>("CastleWallDoorClosed").GetNode<MeshInstance3D>("CastleWallDoor").GetNode<StaticBody3D>("StaticBody3D").GetNode<CollisionShape3D>("CollisionShape3D");
	private CollisionShape3D _endZoneWallDoorCollision => GetNode<Node3D>("CastleWallDoorClosed").GetNode<MeshInstance3D>("CastleDoor").GetNode<StaticBody3D>("StaticBody3D").GetNode<CollisionShape3D>("CollisionShape3D");

	// End zone hidden wall
	private Node3D _endZoneHiddenWallNode => GetNode<Node3D>("CastleWallSecret");
	private CollisionShape3D _endZoneHiddenWallCollision => GetNode<Node3D>("CastleWallSecret").GetNode<MeshInstance3D>("Base").GetNode<StaticBody3D>("StaticBody3D").GetNode<CollisionShape3D>("CollisionShape3D");

	#endregion
	public override void _Ready()
	{
		_hiddenWall1Collision.SetDeferred("disabled", false);
		_endZoneHiddenWallCollision.SetDeferred("disabled", false);

		_hiddenWall1.Hide();
		_endZoneHiddenWallNode.Hide();

		_solidWall1.Show();
	}


	#region Level Wall Functions
	public void EnableObjectiveWalls()
	{
		// toggle first wall
		_solidWall1Collision.SetDeferred("disabled", true);
		_hiddenWall1Collision.SetDeferred("disabled", true);
		_solidWall1.Hide();
		_hiddenWall1.Show();

		// toggle end zone wall
		_endZoneWallCollision.SetDeferred("disabled", true);
		_endZoneWallDoorCollision.SetDeferred("disabled", true);
		_endZoneWallNode.Hide();
		_endZoneHiddenWallNode.Show();
	}

	public void DisableObjectiveWalls()
	{
		// toggle first wall
		_solidWall1Collision.SetDeferred("disabled", false);
		_hiddenWall1Collision.SetDeferred("disabled", true);
		_solidWall1.Show();
		_hiddenWall1.Hide();


		// toggle end zone wall
		_endZoneWallCollision.SetDeferred("disabled", false);
		_endZoneWallDoorCollision.SetDeferred("disabled", false);
		_endZoneHiddenWallNode.Hide();
		_endZoneWallNode.Show();
	}


	#endregion
}
