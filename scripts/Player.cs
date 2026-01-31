using Godot;
using System;

public partial class Player : CharacterBody3D
{
	#region Constants
	// Movement consts
	public const float WalkSpeed = 1.5f;
	public const float SprintSpeed = 2.2f;
	public const float JumpVelocity = 2.0f;
	public const float Sensitivity = 0.003f;

	// Bob consts
	public const float BobFrequency = 8.0f;
	public const float BobAmplitude = 0.05f;

	// fov consts
	public const float BaseFOV = 75.0f;
	public const float FOVChange = 1.5f;
	#endregion

    #region Variables
	private float _bobTime = 0.0f;
	private float _speed = WalkSpeed;

	// interaction variables
	private RayCast3D _interactionRayCast => GetNode<Node3D>("Head").GetNode<Camera3D>("Camera3D").GetNode<RayCast3D>("RayCast3D");
	private Node3D _hand => GetNode<Node3D>("Head").GetNode<Node3D>("Hand");

	private bool _isHoldingObject = false;

	private Node _heldObject = null;
	#endregion


	#region Nodes
	protected Node3D Head => GetNode<Node3D>("Head");
	protected Camera3D Camera => GetNode<Node3D>("Head")
            .GetNode<Camera3D>("Camera3D");

	protected Vector3 Gravity => GetGravity() * 0.66f;
	#endregion
	
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
				float cameraRotationX = Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(-40), Mathf.DegToRad(60));
				float cameraRotationY = Mathf.Clamp(Camera.Rotation.Y, Mathf.DegToRad(-40), Mathf.DegToRad(40));
				Camera.Rotation = new Vector3(cameraRotationX, cameraRotationY, Camera.Rotation.Z);
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

		#region Movement

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionPressed("sprint"))
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
		if (IsOnFloor())
		{
			if (direction != Vector3.Zero)
			{
				velocity.X = direction.X * _speed;
				velocity.Z = direction.Z * _speed;
			}
			else
			{
				velocity.X = (float)Mathf.Lerp(Velocity.X, direction.X * _speed, delta * 10.0f);
				velocity.Z = (float)Mathf.Lerp(Velocity.Z, direction.Z * _speed, delta * 10.0f);
			}
		}
		else
		{
			velocity.X = (float)Mathf.Lerp(Velocity.X, direction.X * _speed, delta * 2.0f);
			velocity.Z = (float)Mathf.Lerp(Velocity.Z, direction.Z * _speed, delta * 2.0f);
		}

		Velocity = velocity;
		#endregion
		


		#region Head Bobbing
		float isOnFloor = IsOnFloor() ? 1 : 0;
		var velocityLength = velocity.Length();
		_bobTime += (float)delta * velocityLength * isOnFloor;
		Transform3D lookTransform = Camera.Transform;
		lookTransform.Origin = HeadBob(_bobTime);
		Camera.Transform = lookTransform;
		#endregion

		#region FOV Change
		var velocityClamped = Mathf.Clamp(velocityLength, 0.5, SprintSpeed * 2);
		var targetFOV = BaseFOV + FOVChange * velocityClamped;
		Camera.Fov = (float)Mathf.Lerp(Camera.Fov, targetFOV, (float)delta * 8.0f);
		#endregion

		// Move the character.
		MoveAndSlide();

		#region Interaction
		// object interaction
		var interactionObject = _interactionRayCast.GetCollider();
		if (_interactionRayCast.IsColliding() && interactionObject is Node selectedObj && selectedObj.IsInGroup("pickable"))
		{
			// if interaction key pressed
			if (Input.IsActionJustPressed("interaction"))
			{
				HandleInteraction(selectedObj);
			}

		}

		// drop action
		if(Input.IsActionJustPressed("drop") && _isHoldingObject)
		{
			HandleInteraction(_heldObject);
		}
		#endregion

	}

	private static Vector3 HeadBob(float time)
	{
		var pos = Vector3.Zero;
		pos.Y = Mathf.Sin(time * BobFrequency) * BobAmplitude;
		pos.X = Mathf.Cos(time * BobFrequency / 2) * BobAmplitude;
		return pos;
	}

	private  void HandleInteraction(Node selectedObj)
	{
		// remove from scene
		if (!_isHoldingObject)
		{
			_heldObject = selectedObj;
			selectedObj.GetParent().RemoveChild(selectedObj);
			_isHoldingObject = true;
		}
		else
		{
			// drop mask in front of player
			Vector3 dropPosition = Head.GlobalTransform.Origin + -Head.GlobalTransform.Basis.Z ;
			Node3D maskObject = (Node3D)_heldObject;
			maskObject.GlobalTransform = new Transform3D(maskObject.GlobalTransform.Basis, dropPosition);
			GetTree().CurrentScene.AddChild(maskObject);
			_isHoldingObject = false;
			_heldObject = null;
		}
	}
}
