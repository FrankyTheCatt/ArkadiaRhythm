using Godot;
using System;

public partial class Azul : Sprite2D
{
	private SerialReader serialReader;

	// método para asignar el SerialReader desde fuera
	public void SetSerialReader(SerialReader sr)
	{
		serialReader = sr;

		if (serialReader != null)
		{
			GD.Print("SerialReader asignado correctamente en Azul.");
			CallDeferred(nameof(DeferredConnectSerialReader));
		}
		else
		{
			GD.PrintErr("Error: SerialReader es null en Azul.");
		}
	}

	private void DeferredConnectSerialReader()
	{
		if (serialReader != null && !serialReader.IsConnected(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated))))
		{
			serialReader.Connect(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated)));
			GD.Print("SerialReader conectado correctamente a Azul.");
		}
		else
		{
			GD.PrintErr("Error: No se pudo conectar el SerialReader a Azul.");
		}
	}

	// este método se llama cuando el sensor correspondiente es activado
	public void OnSensorActivated(int sensorNumber)
	{
		if (sensorNumber == 3)  // verifica el sensor 3 (Azul)
		{
			var animPlayer = GetNode<AnimationPlayer>("animAzul");
			if (animPlayer != null)
			{
				animPlayer.Play("ApretarAzul");
				GD.Print("Sensor Azul activado y animación ejecutada.");
			}
			else
			{
				GD.PrintErr("Error: AnimPlayer para Azul no encontrado.");
			}
		}
	}
}
