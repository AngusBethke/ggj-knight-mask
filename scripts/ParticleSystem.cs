using Godot;
using System;
using System.Collections.Generic;
using Utils;

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

	#region Dragon Sound Nodes
	private AudioStreamPlayer3D _dragonWingSound1 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonWings").GetNode<AudioStreamPlayer3D>("Wings1");
	private AudioStreamPlayer3D _dragonWingSound2 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonWings").GetNode<AudioStreamPlayer3D>("Wings2");
	private AudioStreamPlayer3D _dragonWingSound3 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonWings").GetNode<AudioStreamPlayer3D>("Wings3");

	private AudioStreamPlayer3D _dragonWingSound4 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonWings").GetNode<AudioStreamPlayer3D>("Wings4");
	private AudioStreamPlayer3D _dragonWingSound5 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonWings").GetNode<AudioStreamPlayer3D>("Wings5");
	private AudioStreamPlayer3D _dragonWingSound6 => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonWings").GetNode<AudioStreamPlayer3D>("Wings6");

	private bool _isFadingIn = false;

	private List<AudioStreamPlayer3D> _wingSounds => new List<AudioStreamPlayer3D>{
		_dragonWingSound1,
		_dragonWingSound2,
		_dragonWingSound3,
		_dragonWingSound4,
		_dragonWingSound5,
		_dragonWingSound6
	};

	private ulong _lastWingSoundPlayedTime = 0;
	private int _currentSoundIndex = 0;


	private AudioStreamPlayer3D _dragonRoarSound => GetNode<Node3D>("SoundEffects").GetNode<Node3D>("DragonSounds").GetNode<AudioStreamPlayer3D>("Roar");
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
		
		// roar sound logic
		if(_dragonFirePorted.ShouldDragonRoar()){
			// play roar sound
			PlayDragonRoarSound();
		}
		// wing sound logic
		if(_dragonFirePorted.IsDragonAboutToAttack()){
			PlayDragonWingSound();
		}
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

	private void PlayDragonRoarSound()
	{
		_dragonFirePorted.SetDragonRoared();
		_dragonRoarSound.Play();
	}

	private void PlayDragonWingSound()
	{

		// Bail out if the sound should not be played
		
		var speedDelta = 350f;

		

		if (!((_lastWingSoundPlayedTime + (500 + speedDelta)) < Time.GetTicksMsec())) { return; }
		_lastWingSoundPlayedTime = Time.GetTicksMsec();

		var soundIncrease = _dragonFirePorted.isDragonIncoming();
		var fadedSound = FadeDragonWingSound(soundIncrease, _wingSounds[5]);


		// incase I want to randomize sounds later
		if (_currentSoundIndex > 5)
		{
			_currentSoundIndex = 0;
		}
	
		fadedSound.Play();
		_currentSoundIndex++;
	}

	private AudioStreamPlayer3D FadeDragonWingSound(bool increasingVolume, AudioStreamPlayer3D sound)
	{
		
		// Fade out the dragon wing sound over time
		if(increasingVolume)
		{
			// fade in
			if( _isFadingIn ) {
				return sound;
			}
			var tween = CreateTween();
			_isFadingIn = true;
			tween.TweenProperty(sound, "volume_db", 80f, 2f);
			
		} else {
			// fade out
			if( !_isFadingIn ) {
				return sound;
			}
			var tween = CreateTween();
			_isFadingIn = false;
			tween.TweenProperty(sound, "volume_db", -30f, 2f);
			
		}
		return sound;
	}
}
