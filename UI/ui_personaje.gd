extends Node2D

@export var max_health: int = 100
var current_health: int = max_health

@export var regen_amount: int = 5  # Cantidad de vida que se regenera
@export var regen_interval: float = 1.0  # Intervalo entre regeneraciones (en segundos)
@export var regen_delay: float = 5.0  # Tiempo de espera antes de iniciar la regeneración

func _ready() -> void:
	# Configura el temporizador para regenerar vida
	$Timer.wait_time = regen_interval

# Método para recibir daño. llamalo y asignale un valor al daño para este caso 25!!
func take_damage(amount: int) -> void:
	current_health -= amount
	if current_health <= 0:
		current_health = 0
		die()
	update_health_bar()
	# Reinicia el temporizador de regeneración
	$Timer.stop()
	$Timer.start(regen_delay)

# Método para ganar vida
func heal(amount: int) -> void:
	current_health += amount
	if current_health > max_health:
		current_health = max_health
	update_health_bar()

# Método para manejar la muerte del personaje
func die() -> void:
	print("El personaje ha muerto.")
	# Cambia a la escena de Game Over
	var new_scene_path = "res://MENU/menu_inicio.tscn"  # Ruta de la nueva escena
	get_tree().change_scene_to(load(new_scene_path))

# Método para actualizar la barra de vida
func update_health_bar() -> void:
	if $PlayerInfo/HealthProgressBar:  # Comprueba si el nodo existe
		$PlayerInfo/HealthProgressBar.value = current_health

# Método de regeneración
func _on_Timer_timeout() -> void:
	if current_health < max_health:
		heal(regen_amount)
		# Vuelve a iniciar el temporizador si aún no está al máximo
		if current_health < max_health:
			$Timer.start()


func _on_timer_timeout():
	pass # Replace with function body.
