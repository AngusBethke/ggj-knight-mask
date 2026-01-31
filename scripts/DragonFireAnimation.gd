extends GPUParticles3D

var move_start: Vector3
@export var move_end: Vector3 = Vector3(endingX, endingY, endingZ)
var endingX: float
var endingY: float
var endingZ: float
#@export_range(0.000, 1000.000, 0.001) var EndingX: float
#@export_range(0.000, 1000.000, 0.001) var EndingY: float
#@export_range(0.000, 1000.000, 0.001) var EndingZ: float 

# Debug for Dragon fire attack


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	move_start = global_position
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if Input.is_action_pressed("dragonfire"):
		_attackmove()
	pass

func _attackmove() -> void:
	pass
