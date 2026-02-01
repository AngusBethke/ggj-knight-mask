extends Node


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function bod

func _fade(audio: AudioStreamPlayer2D, target_db: float, fade_duration: float):
	var tween = create_tween() 
	tween.tween_property(audio, "volume_db", target_db, fade_duration)
	return tween
