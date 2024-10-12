extends Sprite2D

var selected_key = 0
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	selected_key = KEY_D
	
	if Input.is_key_pressed(selected_key):
		get_node("animAzul").play("ApretarAzul")
