using Godot;
using System;

public partial class Amarillo : Sprite2D
{
	private SerialReader serialReader;

	
	public void SetSerialReader(SerialReader sr)
	{
		serialReader = sr;

		if (serialReader != null)
		{
			GD.Print("SerialReader asignado correctamente en Amarillo.");
			CallDeferred(nameof(DeferredConnectSerialReader));
		}
		else
		{
			GD.PrintErr("Error: SerialReader es null en Amarillo.");
		}
	}

	private void DeferredConnectSerialReader()
	{
		if (serialReader != null && !serialReader.IsConnected(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated))))
		{
			serialReader.Connect(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated)));
			GD.Print("SerialReader conectado correctamente a Amarillo.");
		}
		else
		{
			GD.PrintErr("Error: No se pudo conectar el SerialReader a Amarillo.");
		}
	}

	// método que se llama cuando el sensor correspondiente es activado
	public void OnSensorActivated(int sensorNumber)
	{
		if (sensorNumber == 2)  // verifica el sensor 2 (Amarillo)
		{
			var animPlayer = GetNode<AnimationPlayer>("animAmarillo");
			if (animPlayer != null)
			{
				animPlayer.Play("ApretarAmarillo");
				GD.Print("Sensor Amarillo activado y animación ejecutada.");
			}
			else
			{
				GD.PrintErr("Error: AnimPlayer para Amarillo no encontrado.");
			}
		}
	}
}
