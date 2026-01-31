extends GPUParticles3D

@export var light: SpotLight3D

var move_start: Vector3
@export var move_end: Vector3 = Vector3(endingX, endingY, endingZ)
@export_range (0.0, 5.0, 0.1) var move_duration: float
var endingX: float
var endingY: float
var endingZ: float

#@export_range(0.000, 1000.000, 0.001) var EndingX: float
#@export_range(0.000, 1000.000, 0.001) var EndingY: float
#@export_range(0.000, 1000.000, 0.001) var EndingZ: float 

# Debug for Dragon fire attack


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	emitting = false
	move_start = position
	light.spot_range = 0

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if Input.is_action_pressed("dragonfire"):
		_attackmove()
	
	if self.position == move_end:
		position = move_start
		emitting = false
		light.spot_range = 0

func _attackmove() -> void:
	light.spot_range = 7
	emitting = true
	var tween := create_tween()
	tween.set_trans(Tween.TRANS_SINE)
	tween.set_ease(Tween.EASE_IN)
	
	tween.tween_property(self, "position", move_end, move_duration)
