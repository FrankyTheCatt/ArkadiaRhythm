extends CanvasLayer


# Called when the node enters the scene tree for the first time.
func _ready():
	$HealthProgressBar.value = get_parent().get_node("Player").health


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
