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
	private Level level;
	public void SetSerialReader(SerialReader sr)
	{
		serialReader = sr;
		if (serialReader != null)
		{
			CallDeferred(nameof(DeferredConnectSerialReader));
		}
	}

	private void DeferredConnectSerialReader()
	{
		if (serialReader != null)
		{
			serialReader.Connect(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated)));
			GD.Print("SerialReader conectado correctamente.");
		}
	}

	private bool haQuitadoVida = false;

	    public void DisableInput()
    {
        SetProcessInput(false); // Deshabilitar la entrada
    }

	public override void _Process(double delta)
	{
		Position += new Vector2(0, (float)(Gravedad * delta));  // mueve hacia abajo las notas	
		if (estaDentro)
		{
			//GD.Print("debug => Tecla asignada:", keySelected);
			if (Input.IsKeyPressed(keySelected)) // Verifica si la tecla correcta fue presionada
			{
				GD.Print("debug => ¡Qué bieeeeeeen!");
				level.GainLife(1);
				QueueFree();  // Elimina la nota si la tecla correcta fue presionada
			}
		}
		if (Visible)
		{  // solo procesar si el objeto está visible
			if (Position.Y > 950 && !haQuitadoVida)  // Cuando sale de la pantalla y no ha quitado vida aún
			{
				GD.Print("debug => ¡NO HAS APRETADO LA TECLA!");
				level.TakeDamage(1);  // Llama a TakeDamage en Level
				haQuitadoVida = true;  // Asegura que solo quita vida una vez
				QueueFree();  // Elimina la nota
				GD.Print("Nota eliminada, fuera de la pantalla");
			}
		}
	}

	// método que se llama cuando el sensor se activa
	public void OnSensorActivated(int sensorNumber)
	{
		//GD.Print($"Nota posición Y: {Position.Y}, estaDentro: {estaDentro}, sensorNumber: {this.sensorNumber}");

		// es para asegurarse de que el sensor activado es el correcto y que la nota está dentro del área
		if (estaDentro && sensorNumber == this.sensorNumber)
		{
			GD.Print("Sensor " + sensorNumber + " activado correctamente en el área. Eliminando nota.");
			level.GainLife(1);
			QueueFree();  // elimina la nota si el sensor correcto se activa y la nota está en el área
		}
	}
	public override void _Ready()
	{
		// conectar las señales para las colisiones de área
		Connect("area_entered", new Callable(this, nameof(_on_area_entered)));
		Connect("area_exited", new Callable(this, nameof(_on_area_exited)));

		level = GetParent<Level>();  // Asignar la instancia de Level
	}

	// inicializa la nota en un carril específico
	public void Spawn(int key, Vector2 pos)
	{
		Position = pos;  // ajustar posición
		sensorNumber = key + 1;  // asignar el número del sensor basado en el carril (1-4)
		Visible = true;  // hacer visible la nota

		// cambiar color según el carril de la nota
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
		//GD.Print($"Nota ha entrado en el área de colisión, posición Y: {Position.Y}, área: {area.Name}");
	}

	public void _on_area_exited(Area2D area)
	{
		estaDentro = false;
		//GD.Print($"Nota ha salido del área de colisión, posición Y: {Position.Y}, área: {area.Name}");
	}

}
