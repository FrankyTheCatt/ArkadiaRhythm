using Godot;
using System;

public partial class Options_Menu : Node
{
	public override void _Ready() {}

	public override void _Process(double delta) {}
	
	public void _on_volumen_button_up() {}
	
	public void _on_resolution_button_up() {}
	
	public void _on_back_button_up() {
		GetTree().ChangeSceneToFile("res://MENU/menu_inicio.tscn");
	}
	
	
}
