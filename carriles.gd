extends Area2D

const gravedad = 120
var esta_dentro = false;
var selected_Key = 0
	
#Hace que la letra caiga
func _process(delta):
	position.y += gravedad * delta
	if esta_dentro:
		if Input.is_key_pressed(selected_Key):
			print("debug=> Qué bien!")
			queue_free()

func spawn(key:int, pos:Vector2)->void:
	position = pos
	match key:
		0:
			selected_Key = KEY_LEFT
			rotation_degrees = 0
		1:
			selected_Key = KEY_RIGHT
			rotation_degrees = 180
		2:
			selected_Key = KEY_UP
			rotation_degrees = 90
		3:
			selected_Key = KEY_DOWN
			rotation_degrees = -90

#Detecta un objeto si entra al área
func _on_area_entered(area: Area2D) -> void:
	esta_dentro = true

#Detecta un objeto si sale del área
func _on_area_exited(area: Area2D) -> void:
	esta_dentro = false
