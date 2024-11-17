using Godot;
using System;

public partial class MainMenu : Control
{
	
	public override void _Ready() { }

	public override void _Process(double delta) { }
	
	public void _on_play_button_up() { }
	
	public void _on_options_button_up() {
		GetTree().ChangeSceneToFile("res://MENU/Opciones.tscn");
	}
	
	public void _on_quit_button_up() { 
		GetTree().Quit();
	}
	
}
