extends Control



func _on_play_pressed():
	get_tree().change_scene_to_file("res://MENU/Scenes/scena_juego.tscn")


func _on_options_pressed():
	get_tree().change_scene_to_file("res://MENU/Scenes/Opciones.tscn")


func _on_quit_pressed():
	get_tree().quit()
	
