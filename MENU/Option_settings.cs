using Godot;
using System;

public partial class Option_settings : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_full_screen_toggled(bool toggled) {
		if(toggled){
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}
	
	public void _on_borderless_toggled(bool toggled) {
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, toggled);
	}
	
	public void _on_back_button_up() {
		GetTree().ChangeSceneToFile("res://MENU/menu_inicio.tscn");
	}
	
}
