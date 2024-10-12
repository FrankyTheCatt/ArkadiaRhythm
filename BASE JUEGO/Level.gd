extends Node2D
@export var KeyObject: PackedScene

var positions: Array = []
var timers: Array = []  # Array para los temporizadores

func _ready():
	positions.append($Objects/ColorObject4.global_position.x) # Carril 1 (tecla A)
	positions.append($Objects/ColorObject3.global_position.x)# Carril 2 (tecla S)
	positions.append($Objects/ColorObject2.global_position.x)# Carril 3 (tecla D)
	positions.append($Objects/ColorObject.global_position.x)# Carril 4 (tecla F)

	# Crear y configurar un temporizador para cada carril
	for i in range(4):
		var timer = Timer.new()
		timer.wait_time = randf_range(1.0, 3.0)  # Ritmo diferente para cada carril
		timer.one_shot = false
		# Usamos un closure para pasar el índice del carril correctamente
		timer.connect("timeout", Callable(self, "_spawn").bind(i))  
		add_child(timer)
		timers.append(timer)
		timer.start()

# Spawnea una nota en el carril específico
func _spawn(carril:int):
	var KeyInstance = KeyObject.instantiate()  
	var pos = $Marker2D.position
	pos.x = positions[carril]  # Asignar la posición según el carril
	KeyInstance.spawn(carril, pos)  # Pasar el carril y el color correspondiente
	add_child(KeyInstance)
