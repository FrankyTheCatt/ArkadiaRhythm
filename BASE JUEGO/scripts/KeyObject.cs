using Godot;
using System;

public partial class KeyObject : Area2D
{
	private const int Gravedad = 500;  // velocidad de caída
	private bool estaDentro = false;
	private int sensorNumber;  // número del sensor (1-4)
	private SerialReader serialReader;
	private Key keySelected;
	// método para asignar SerialReader desde Level
	public void SetSerialReader(SerialReader sr)
	{
@@ -28,12 +28,17 @@
	}

	// Hace que la nota caiga
	public override void _Process(double delta){
		Position += new Vector2(0, (float)(Gravedad * delta));  // mueve hacia abajo las notas	
		if (estaDentro){
			GD.Print("debug => Tecla asignada:", keySelected);
			if (Input.IsKeyPressed(keySelected)) // Verifica si la tecla correcta fue presionada
			{
				GD.Print("debug => ¡Qué bieeeeeeen!");
				QueueFree();  // Elimina la nota si la tecla correcta fue presionada
			}
		}
		if (Visible){  // solo procesar si el objeto está visible
			if (Position.Y > 950)  // Cuando sale de la pantalla
			{
				GD.Print("Nota eliminada, fuera de la pantalla");
@@ -61,10 +66,7 @@
		Connect("area_exited", new Callable(this, nameof(_on_area_exited)));
	}

	// inicializa la nota en un carril específico
	public void Spawn(int key, Vector2 pos)
	{
		Position = pos;  // ajustar posición
@@ -75,30 +77,34 @@
		switch (key)
		{
			case 0:
				keySelected = Key.A;
				Modulate = new Color(1, 0, 0);  // rojo (sensor 1)
				break;
			case 1:
				keySelected = Key.S;
				Modulate = new Color(1, 1, 0);  // amarillo (sensor 2)
				break;
			case 2:
				keySelected = Key.D;
				Modulate = new Color(0, 0, 1);  // azul (sensor 3)
				break;
			case 3:
				keySelected = Key.F;
				Modulate = new Color(0, 1, 0);  // verde (sensor 4)
				break;
		}
	}
	
	public void _on_area_entered(Area2D area)
	{
		estaDentro = true;
		GD.Print($"Nota ha entrado en el área de colisión, posición Y: {Position.Y}, área: {area.Name}");
	}

	public void _on_area_exited(Area2D area)
	{
		estaDentro = false;
		GD.Print($"Nota ha salido del área de colisión, posición Y: {Position.Y}, área: {area.Name}");
	}

}
