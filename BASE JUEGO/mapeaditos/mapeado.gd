extends Node

# Variables para almacenar las listas de tiempos de teclas
var verde_times: Array = []
var azul_times: Array = []
var amarillo_times: Array = []
var rojo_times: Array = []

# Variables de estado para controlar si las teclas están presionadas
var is_a_pressed = false
var is_s_pressed = false
var is_d_pressed = false
var is_f_pressed = false
var recording_enabled = true  # Controla si se siguen registrando las teclas

# Ruta del archivo donde se guardarán los datos
# var save_file_path = "res://BASE JUEGO/mapeaditos/tiempos_teclas2.json"
var save_file_path = "res://BASE JUEGO/mapeaditos/tiempos_damages_notes2.json"

func _ready():
	print("Listo para registrar tiempos de teclas")

func _process(delta):
	# Verifica si se presionan Ctrl+S para guardar o Ctrl+L para cargar
	if Input.is_physical_key_pressed(KEY_CTRL) and Input.is_physical_key_pressed(KEY_S):
		print("Guardando datos...")
		save_data()
		recording_enabled = false  # Detiene el registro de teclas

	var delay = 0.0  # Define el retraso en segundos

	if recording_enabled:
	# Tecla A (Rojo)
		if Input.is_physical_key_pressed(KEY_A):
			if not is_a_pressed:
				var time = (Time.get_ticks_msec() / 1000.0) + delay
				rojo_times.append(time)
				print("Tiempo registrado para tecla A (Rojo):", time)
				is_a_pressed = true
		else:
			is_a_pressed = false  # Se reinicia cuando se suelta la tecla

		# Tecla S (Amarillo)
		if Input.is_physical_key_pressed(KEY_F):
			if not is_s_pressed:
				var time = (Time.get_ticks_msec() / 1000.0) + delay
				amarillo_times.append(time)
				print("Tiempo registrado para tecla S (Amarillo):", time)
				is_s_pressed = true
		else:
			is_s_pressed = false  # Se reinicia cuando se suelta la tecla

		# Tecla D (Azul)
		if Input.is_physical_key_pressed(KEY_S):
			if not is_d_pressed:
				var time = (Time.get_ticks_msec() / 1000.0) + delay
				azul_times.append(time)
				print("Tiempo registrado para tecla D (Azul):", time)
				is_d_pressed = true
		else:
			is_d_pressed = false  # Se reinicia cuando se suelta la tecla

		# Tecla F (Verde)
		if Input.is_physical_key_pressed(KEY_D):
			if not is_f_pressed:
				var time = (Time.get_ticks_msec() / 1000.0) + delay
				verde_times.append(time)
				print("Tiempo registrado para tecla F (Verde):", time)
				is_f_pressed = true
		else:
			is_f_pressed = false  # Se reinicia cuando se suelta la tecla


# Función para guardar los datos en un archivo
func save_data():
	var data = {
		"rojo_times": rojo_times,
		"amarillo_times": amarillo_times,
		"azul_times": azul_times,
		"verde_times": verde_times
	}
	
	var file = FileAccess.open(save_file_path, FileAccess.WRITE)
	if file:
		file.store_string(JSON.stringify(data))  # Convierte los datos a JSON y guárdalos
		file.close()
		print("Datos guardados en", save_file_path)
	else:
		print("Error al guardar los datos")
