using Godot;
using System;

public partial class Audio : TabBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_master_value_changed(float value) {
		set_bus_volume(0,value);
	}
	
	public void _on_music_value_changed(float value) {
		set_bus_volume(1,value);
	}
	
	public void _on_sfx_value_changed(float value) {
		set_bus_volume(2,value);
	}
	
	public void set_bus_volume(int idex,float value) {
		//AudioServer.SetBusVolumeDb(idex,LinearToDb(value));
	}
}
