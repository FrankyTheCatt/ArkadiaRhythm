extends Node2D


# Called when the node enters the scene tree for the first time.
func _ready():
	var character = $Player
	character.TakeDamage(25)
	await get_tree().create_timer(2).timeout
	character.Heal(25)  # La barra debería aumentar

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
