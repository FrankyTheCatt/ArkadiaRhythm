using Godot;
using System;

public partial class DefeatLeds : Node2D
{

public void _on_retry_button_up() {
	GetTree().ChangeSceneToFile("res://Minijuego Leds/Minijuego_Leds.tscn");
}

public void _on_menu_button_up() {
	GetTree().ChangeSceneToFile("res://MENU/menu_inicio.tscn");
}

}
