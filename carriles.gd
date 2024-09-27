extends Area2D

const gravedad = 120
	
func _process(delta):
	position.y += gravedad * delta
