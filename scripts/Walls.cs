using Godot;
using System;

public partial class Walls : Node3D
{
	#region Nodes

	private CsgBox3D _level1CollisionWalls2 => GetNode<CsgBox3D>("1m-BlockerWall2");

	private CsgBox3D _level1ObjectiveWalls2 => GetNode<CsgBox3D>("1m-ObjectiveWall2");

	#endregion
	public override void _Ready()
	{
		_level1ObjectiveWalls2.UseCollision = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	#region Level Wall Functions
	public void EnableObjectiveWalls()
	{
		// transform the y of the collision wall to be under the map
		_level1CollisionWalls2.UseCollision = false;
		_level1CollisionWalls2.Hide();
		_level1ObjectiveWalls2.Show();
		
	}

	public void DisableObjectiveWalls()
	{
		_level1CollisionWalls2.UseCollision = true;
		_level1CollisionWalls2.Show();
		_level1ObjectiveWalls2.Hide();
	}


	#endregion
}
