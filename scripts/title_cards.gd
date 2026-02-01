extends Node2D

@export var voicecard1: Sprite2D
@export var voicecard2: Sprite2D
@export var voicecard3: Sprite2D
@export var voiceact1: AudioStreamPlayer2D
@export var voiceact2: AudioStreamPlayer2D
@export var voiceact3: AudioStreamPlayer2D
@export var voiceactChoirScream: AudioStreamPlayer2D
@export var blackfadeout: ColorRect
#@export_range(0, 5, 0.2) var fade_duration = 1.0


var finished: bool

func _process(delta: float) -> void:
	#check if "skip" input is pressed
	if Input.is_action_just_pressed("skip"):
		get_tree().change_scene_to_file("res://scenes/Main_World.tscn")
		
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	voicecard1.visible = false
	voicecard2.visible = false
	voicecard3.visible = false
	voicecard1.top_level = false
	voicecard2.top_level = false
	voicecard3.top_level = false
	blackfadeout.visible = false
	blackfadeout.color.a = 0
	await _titlecards()
	blackfadeout.visible = true
	blackfadeout.top_level = true
	
	await _fade(254, 5.5)
	
	get_tree().change_scene_to_file("res://scenes/Main_World.tscn")
	

func _fade(target_alpha: float, fade_duration: float):
	var tween = create_tween()
	tween.set_trans(Tween.TRANS_SINE);
	tween.set_ease(Tween.EASE_IN);
	tween.tween_property(blackfadeout, "color:a", target_alpha, fade_duration)
	await tween.finished
	
func _fadeaudio(target_volume: float, fade_duration: float):
	var tweenaudio = create_tween()
	tweenaudio.set_trans(Tween.TRANS_SINE);
	tweenaudio.set_ease(Tween.EASE_IN);
	tweenaudio.tween_property(voiceactChoirScream, "volume_db", target_volume, fade_duration)
	
	
func _card1() -> void:
	voicecard1.visible = true
	voicecard1.top_level = true
	voiceact1.playing = true
	await voiceact1.finished
	
func _card2() -> void:
	voicecard2.visible = true
	voicecard2.top_level = true
	voiceact2.playing = true
	await voiceact2.finished
	
func _card3() -> void:
	voicecard3.visible = true
	voicecard3.top_level = true
	voiceact3.playing = true
	voiceactChoirScream.volume_db = -10.0
	voiceactChoirScream.playing = true
	_fadeaudio(15.0, 8.0)
	await voiceact3.finished

func _titlecards():
	await _card1()
	await _card2()
	await _card3()
	
