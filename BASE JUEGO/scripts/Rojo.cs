using Godot;
using System;

public partial class Rojo : Sprite2D
{
	private SerialReader serialReader;


	public void SetSerialReader(SerialReader sr)
	{
		serialReader = sr;

		if (serialReader != null)
		{
			GD.Print("SerialReader asignado correctamente en Rojo.");
			CallDeferred(nameof(DeferredConnectSerialReader));
		}
		else
		{
			GD.PrintErr("Error: SerialReader es null en Rojo.");
		}
	}

	private void DeferredConnectSerialReader()
	{
		if (serialReader != null && !serialReader.IsConnected(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated))))
		{
			serialReader.Connect(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated)));
			GD.Print("SerialReader conectado correctamente a Rojo.");
		}
		else
		{
			GD.PrintErr("Error: No se pudo conectar el SerialReader a Rojo.");
		}
	}

	// método que se llama cuando el sensor correspondiente es activado
	public void OnSensorActivated(int sensorNumber)
	{
		if (sensorNumber == 1)  // verifica al sensor 1 (Rojo)
		{
			var animPlayer = GetNode<AnimationPlayer>("animRojo");
			if (animPlayer != null)
			{
				animPlayer.Play("ApretarRojo");
				GD.Print("Sensor Rojo activado y animación ejecutada.");
			}
			else
			{
				GD.PrintErr("Error: AnimPlayer para Rojo no encontrado.");
			}
		}
	}
}
