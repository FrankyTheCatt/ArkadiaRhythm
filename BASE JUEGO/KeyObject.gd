extends Area2D

const gravedad = 500
var esta_dentro = false
var selected_Key = 0

# Hace que la nota caiga
func _process(delta):
	position.y += gravedad * delta

	# Si la nota está dentro del área de interacción
	if esta_dentro:
		print("debug => Tecla asignada:", selected_Key)
		if Input.is_key_pressed(selected_Key):
			print("debug => ¡Qué bieeeeeeen!")
			queue_free()  # Elimina la nota si la tecla correcta fue presionada

	if position.y > 1000:  
		print("debug => Nota no apretada, eliminada")
		queue_free()  # Elimina la nota si no fue presionada y salió de la pantalla no funciona 

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
func _on_area_entered(area: Area2D) -> void:
	esta_dentro = true

# Detecta si una nota sale del área
func _on_area_exited(area: Area2D) -> void:
	esta_dentro = false
