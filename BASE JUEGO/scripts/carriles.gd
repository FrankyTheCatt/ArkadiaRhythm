extends Area2D

const gravedad = 120
var esta_dentro = false
var selected_Key = 0

# Hace que la nota caiga
func _process(delta):
	position.y += gravedad * delta
	if esta_dentro:
		if Input.is_key_pressed(selected_Key):
			print("debug=> Qué bien!")
			queue_free()

# Asignar un color y tecla fija por carril
func spawn(key:int, pos:Vector2)->void:
	position = pos
	match key:
		0:  # Carril 1
			selected_Key = KEY_A
			self.modulate = Color(1, 0, 0)  # Rojo
		1:  # Carril 2
			selected_Key = KEY_S
			self.modulate = Color(1, 1, 0)  # Amarillo
		2:  # Carril 3
			selected_Key = KEY_D
			self.modulate = Color(0, 0, 1)  # Azul
		3:  # Carril 4
			selected_Key = KEY_F
			self.modulate = Color(0, 1, 0)  # Verde

# Detecta si una nota entra en el área
func _on_area_entered(_area: Area2D) -> void:
	esta_dentro = true

# Detecta si una nota sale del área
func _on_area_exited(_area: Area2D) -> void:
	esta_dentro = false
