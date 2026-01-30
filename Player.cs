using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float Sensitivity = 0.01f;

	protected Node3D Head => GetNode<Node3D>("Head");
	protected Camera3D Camera => GetNode<Node3D>("Head")
            .GetNode<Camera3D>("Camera3D");
	
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
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
