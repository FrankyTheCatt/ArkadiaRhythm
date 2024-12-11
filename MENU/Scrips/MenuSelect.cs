using Godot;
using System;

public partial class MenuSelect : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
	
	public void _on_exit_pressed() {
		GetTree().ChangeSceneToFile("res://MENU/menu_inicio.tscn");
	}
	
	public void _on_nivel_1_pressed() {
		GetTree().ChangeSceneToFile("res://BASE JUEGO/Level.tscn");
	}
	
	public void _on_level_2_pressed() {
		
	}
	
}
