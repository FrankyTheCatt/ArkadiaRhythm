extends Area2D

const gravedad = 120
	

#Hace que la letra caiga
func _process(delta):
	position.y += gravedad * delta

#Detecta un área si hay una cosa dentro
func _on

func _on_area_entered(area: Area2D) -> void:
	pass # Replace with function body.


func _on_area_exited(area: Area2D) -> void:
	pass # Replace with function body.
