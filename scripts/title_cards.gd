extends Node2D

@export var voicecard1: Sprite2D
@export var voicecard2: Sprite2D
@export var voicecard3: Sprite2D
@export var voiceact1: AudioStreamPlayer2D
@export var voiceact2: AudioStreamPlayer2D
@export var voiceact3: AudioStreamPlayer2D
@export var blackfadeout: ColorRect
@export_range(1, 10, 0.2) var fade_duration = 5.0


var finished: bool
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	voicecard1.visible = false
	voicecard2.visible = false
	voicecard3.visible = false
	voicecard1.top_level = false
	voicecard2.top_level = false
	voicecard3.top_level = false
	blackfadeout.visible = false
	blackfadeout.color.a = 0.0
	await _titlecards()
	blackfadeout.visible = true
	blackfadeout.top_level = true
	#await fade(255).finished
	get_tree().change_scene_to_file("res://scenes/Main_World.tscn")
	

func fade(target_alpha: float):
	var tween = create_tween() 
	tween.tween_property(blackfadeout, "colour.a", target_alpha, fade_duration)
	return tween
	
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
	await voiceact3.finished

func _titlecards():
	await _card1()
	await _card2()
	await _card3()
	#voicecard1.visible = true
	#voicecard1.top_level = true
	#voiceact1.playing = true
	#if voiceact1.finished:
	#	voiceact1.playing = false
	#	voicecard2.visible = true
	#	voicecard2.top_level = true
	#	voiceact2.playing = true
	#if voiceact2.finished:
	#	voiceact2.playing = false
	#	voicecard3.visible = true
	#	voicecard3.top_level = true
	#	voiceact3.playing = true
	#if voiceact3.finished:
	#	voiceact3.playing = false

		
