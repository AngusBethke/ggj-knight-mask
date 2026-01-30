using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float WalkSpeed = 1.5f;
	public const float SprintSpeed = 2.2f;
	public const float JumpVelocity = 2.0f;
	public const float Sensitivity = 0.003f;

	// Bob variables
	public const float BobFrequency = 8.0f;
	public const float BobAmplitude = 0.05f;

	private float _bobTime = 0.0f;	
	private float _speed = WalkSpeed;

	protected Node3D Head => GetNode<Node3D>("Head");
	protected Camera3D Camera => GetNode<Node3D>("Head")
            .GetNode<Camera3D>("Camera3D");

	protected Vector3 Gravity => GetGravity() * 0.66f;
	
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		 base._UnhandledInput(@event);
		if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				Head.RotateY(-mouseMotionEvent.Relative.X * Sensitivity);
				Camera.RotateX(-mouseMotionEvent.Relative.Y * Sensitivity);
				float cameraRotationX = Mathf.Clamp(Camera.Rotation.X,Mathf.DegToRad(-40),Mathf.DegToRad(60));
				Camera.Rotation = new Vector3(cameraRotationX, Camera.Rotation.Y, Camera.Rotation.Z);
			}
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += Gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if( Input.IsActionPressed("sprint"))
		{
			_speed = SprintSpeed;
		}
		else
		{
			_speed = WalkSpeed;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * _speed;
			velocity.Z = direction.Z * _speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
		}

		Velocity = velocity;

		float isOnFloor = IsOnFloor() ? 1 : 0;

		var velocityLength = velocity.Length();
        if(velocityLength > 0 && isOnFloor > 0)
		{
			_bobTime += (float)delta * velocityLength * isOnFloor;
		}
		
	

		Transform3D lookTransform = Camera.Transform;
		lookTransform.Origin = HeadBob(_bobTime);
		Camera.Transform = lookTransform;

		MoveAndSlide();

	}

	private static Vector3 HeadBob(float time)
	{
		var pos = Vector3.Zero;
		pos.Y = Mathf.Sin(time * BobFrequency) * BobAmplitude;
		pos.X = Mathf.Cos(time * BobFrequency / 2) * BobAmplitude;
		return pos;
	}
}
