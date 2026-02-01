using Godot;
using System;
using System.Collections.Generic;

public partial class DragonFireAnimationPorted : GpuParticles3D
{
	// Non-exported internal start position
	private Vector3 _moveStart = Vector3.Zero;

	// Exported target position (editable in the editor)
	[Export]
	public Vector3 MoveEnd { get; set; } = Vector3.Zero;

	// Exported move duration with a range hint
	[Export(PropertyHint.Range, "0.0,5.0,7")]
	public float MoveDuration { get; set; } = 1.0f;

	// Individual components which can be edited in the editor
	[Export]
	public float EndingX { get; set; } = 0f;

	[Export]
	public float EndingY { get; set; } = 5.0f;

	[Export]
	public float EndingZ { get; set; } = 7f;

	private float _actionCooldown = 15;
	private float _initalCooldown = 15;


	

	public override void _Ready()
	{
		Emitting = false;
		_moveStart = Position;

		// If MoveEnd was not set in the editor, build it from the individual components
		if (MoveEnd == Vector3.Zero)
		{
			MoveEnd = new Vector3(EndingX, EndingY, EndingZ);
		}
	}

	public override void _Process(double delta)
	{
	   
	}

	public void HandleAttack(double delta)
	{
		 _actionCooldown = MathF.Max(0, _actionCooldown - (float)delta);
		
		
		if (_actionCooldown <= 0)
		{
			AttackMove();
			_actionCooldown = _initalCooldown;
		}

		// Use an approximate comparison to avoid floating point equality issues
		if (Position.IsEqualApprox(MoveEnd))
		{
			Position = _moveStart;
			Emitting = false;
		}
	}

	private void AttackMove()
	{
		Emitting = true;

		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.In);

		// Tween the node's position to MoveEnd over MoveDuration seconds
		tween.TweenProperty(this, "position", MoveEnd, MoveDuration);
	}

	public bool IsDragonAttacking(){
		
		var timeDifference =  _initalCooldown - _actionCooldown;

		return timeDifference > 2.2f &&  timeDifference < 8f;
		
	}

	public bool ShouldDragonRoar(){
		// is dragon at _moveStart
		return _moveStart.Equals(Position);
	}

	public bool IsDragonAboutToAttack(){
		
		return true;
		
	}

	public bool isDragonIncoming(){
		var timeDifference =  _initalCooldown - _actionCooldown;
		var inWindow =  timeDifference > 11f ||  timeDifference < 4f;
		return inWindow;
	}
}
