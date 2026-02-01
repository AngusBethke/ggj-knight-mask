using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Player : CharacterBody3D
{
	#region Constants
	// Movement consts
	public const float WalkSpeed = 1.8f;
	public const float SprintSpeed = 2.4f;
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

	// mask nodes
	private MaskOverlay _maskOverlay => GetNode<MaskOverlay>("MaskOverlay");


	// interaction hints
	private Node2D _interactionHint => GetNode<Node2D>("InteractionHint");
	#endregion

	#region SoundNodes
	private ulong _lastWalkingSoundPlayedTime = 0;
	private int _currentSoundIndex = 0;

	// -- Footstep Sounds -- //
	// Reverb
	private AudioStreamPlayer3D _reverbFootstep1 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsReverb").GetNode<AudioStreamPlayer3D>("Footstep1");
    private AudioStreamPlayer3D _reverbFootstep2 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsReverb").GetNode<AudioStreamPlayer3D>("Footstep2");
    private AudioStreamPlayer3D _reverbFootstep3 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsReverb").GetNode<AudioStreamPlayer3D>("Footstep3");
    private AudioStreamPlayer3D _reverbFootstep4 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsReverb").GetNode<AudioStreamPlayer3D>("Footstep4");
    private AudioStreamPlayer3D _reverbFootstep5 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsReverb").GetNode<AudioStreamPlayer3D>("Footstep5");
    private AudioStreamPlayer3D _reverbFootstep6 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsReverb").GetNode<AudioStreamPlayer3D>("Footstep6");

	// Normal
    private AudioStreamPlayer3D _normalFootstep1 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsNormal").GetNode<AudioStreamPlayer3D>("Footstep1");
    private AudioStreamPlayer3D _normalFootstep2 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsNormal").GetNode<AudioStreamPlayer3D>("Footstep2");
    private AudioStreamPlayer3D _normalFootstep3 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsNormal").GetNode<AudioStreamPlayer3D>("Footstep3");
    private AudioStreamPlayer3D _normalFootstep4 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsNormal").GetNode<AudioStreamPlayer3D>("Footstep4");
    private AudioStreamPlayer3D _normalFootstep5 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsNormal").GetNode<AudioStreamPlayer3D>("Footstep5");
    private AudioStreamPlayer3D _normalFootstep6 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("FootstepsNormal").GetNode<AudioStreamPlayer3D>("Footstep6");

	// -- Visor Sounds -- //
    // Normal
    private AudioStreamPlayer3D _visorSound1 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("Visor").GetNode<AudioStreamPlayer3D>("Lift1");
    private AudioStreamPlayer3D _visorSound2 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("Visor").GetNode<AudioStreamPlayer3D>("Lift2");
    private AudioStreamPlayer3D _visorSound3 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("Visor").GetNode<AudioStreamPlayer3D>("Lift3");
    #endregion


    public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		_maskOverlay.Visible = false;
		_interactionHint.Visible = false;
		
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
				float cameraRotationX = Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(-70), Mathf.DegToRad(70));
				float cameraRotationY = Mathf.Clamp(Camera.Rotation.Y, Mathf.DegToRad(-60), Mathf.DegToRad(60));
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
			_speed = _isHoldingObject ? SprintSpeed * 0.75f : SprintSpeed;
		}
		else
		{
			_speed = _isHoldingObject ? WalkSpeed * 0.75f : WalkSpeed;
		}
		

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (IsOnFloor())
		{
			if (direction != Vector3.Zero)
			{
				PlayWalkingSound();
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
			// show interaction hint
			if(!_isHoldingObject)
			{
				_interactionHint.Visible = true;
			}
			// if interaction key pressed
			if (Input.IsActionJustPressed("interaction"))
			{
				HandleInteraction(selectedObj);
			}

		} else
		{
			_interactionHint.Visible = false;
		}

		// drop action
		if (Input.IsActionJustPressed("drop") && _isHoldingObject)
		{
			HandleInteraction(_heldObject);
		}
		#endregion
		

	}

    #region Movement Sounds
	private void PlayWalkingSound()
	{
		// Bail out if the sound should not be played
		var speedDelta = 500 * (1 - _speed / WalkSpeed);

        if (!((_lastWalkingSoundPlayedTime + (500 + speedDelta)) < Time.GetTicksMsec())) { return; }
        _lastWalkingSoundPlayedTime = Time.GetTicksMsec();

        var speed = _speed;

        List<AudioStreamPlayer3D> reverbWalkingSounds = [
            _reverbFootstep1,
            _reverbFootstep2,
            _reverbFootstep3,
            _reverbFootstep4,
            _reverbFootstep5,
            _reverbFootstep6
        ];

        List<AudioStreamPlayer3D> normalWalkingSounds = [
            _normalFootstep1,
            _normalFootstep2,
            _normalFootstep3,
            _normalFootstep4,
            _normalFootstep5,
            _normalFootstep6
        ];

		if (_currentSoundIndex > 5)
		{
			_currentSoundIndex = 0;
		}

        bool isInside = false;

		if (isInside)
		{
			var sound = reverbWalkingSounds[_currentSoundIndex];
			sound.Play();
        }
		else
		{
            var sound = normalWalkingSounds[_currentSoundIndex];
            sound.Play();
        }

		_currentSoundIndex++;
    }
    #endregion

    #region Sound Helpers
    private AudioStreamPlayer3D GetRandomSoundFromList(List<AudioStreamPlayer3D> sounds)
    {
		var random = new Random();
		var index = random.Next(0, sounds.Count - 1);

        return sounds[index];
    }
    #endregion

    #region Helper Methods
    private static Vector3 HeadBob(float time)
	{
		var pos = Vector3.Zero;
		pos.Y = Mathf.Sin(time * BobFrequency) * BobAmplitude;
		pos.X = Mathf.Cos(time * BobFrequency / 2) * BobAmplitude;
		return pos;
	}

	private void HandleInteraction(Node selectedObj)
	{
        List<AudioStreamPlayer3D> visorSounds = [
            _visorSound1,
            _visorSound2,
            _visorSound3
        ];

        // remove from scene
        if (!_isHoldingObject)
		{
			_heldObject = selectedObj;
			selectedObj.GetParent().RemoveChild(selectedObj);
			_isHoldingObject = true;

			_maskOverlay.Visible = true;
			_interactionHint.Visible = false;

			var sound = GetRandomSoundFromList(visorSounds);
			sound.Play();

			// shake player
			Shake(magnitude: 0.05f, period: 1f);
		}
		else
		{
			// drop mask in front of player
			Vector3 dropPosition = Head.GlobalTransform.Origin + -Head.GlobalTransform.Basis.Z;
			Node3D maskObject = (Node3D)_heldObject;
			maskObject.GlobalTransform = new Transform3D(maskObject.GlobalTransform.Basis, dropPosition);
			GetTree().CurrentScene.AddChild(maskObject);
			_isHoldingObject = false;
			_heldObject = null;

            var sound = GetRandomSoundFromList(visorSounds);
            sound.Play();

            _maskOverlay.Visible = false;
		}
	}
	#endregion

	#region Getters
	public bool IsWearingMask()
	{
		return _isHoldingObject;
	}

	#endregion

	#region Shake Methods
	
	public void Shake(float magnitude = 0.1f, float period = 2f)
	{
		_ =CameraShakeAsync(magnitude, period);
	}

	 private async Task CameraShakeAsync(float magnitude = 0.1f, float period = 2f)
	{
		Transform3D initial_transform = this.Transform;
		float elapsed_time = 0.0f;
		var tracker = 2;
		while(elapsed_time < period)
		{
			if(tracker % 2 != 0)
			{
				tracker++;	
				continue;
			}
			var offset = new Vector3(
				(float)GD.RandRange(-magnitude, magnitude),
				(float)GD.RandRange(-magnitude, magnitude),
				0.0f
			);

			Transform3D new_transform = initial_transform;
			new_transform.Origin += offset;
			Transform = new_transform;

			elapsed_time += (float)GetProcessDeltaTime();


			await ToSignal(GetTree(), "process_frame");
			tracker++;	
		}

		Transform = initial_transform;
	}
	#endregion
}
