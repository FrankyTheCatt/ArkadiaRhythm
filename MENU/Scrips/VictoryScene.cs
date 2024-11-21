using Godot;
using System;

public partial class VictoryScene : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_return_button_up() {
		GetTree().ChangeSceneToFile("res://MENU/menu_inicio.tscn");
	}
	
}
