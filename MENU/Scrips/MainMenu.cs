using Godot;
using System;

public partial class MainMenu : Control
{
	
	public override void _Ready() { }

	public override void _Process(double delta) { }
	
	public void _on_play_button_up() { 
		GetTree().ChangeSceneToFile("res://MENU/menu_Select.tscn");
	}
	
	public void _on_options_button_up() {
		GetTree().ChangeSceneToFile("res://MENU/Opciones_deplegable.tscn");
	}
	
	public void _on_quit_button_up() { 
		GetTree().Quit();
	}
	
}
