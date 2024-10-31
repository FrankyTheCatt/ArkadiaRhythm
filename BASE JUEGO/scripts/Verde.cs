using Godot;
using System;

public partial class Verde : Sprite2D
{
	private SerialReader serialReader;


	public void SetSerialReader(SerialReader sr)
	{
		serialReader = sr;

		if (serialReader != null)
		{
			GD.Print("SerialReader asignado correctamente en Verde.");
			CallDeferred(nameof(DeferredConnectSerialReader));
		}
		else
		{
			GD.PrintErr("Error: SerialReader es null en Verde.");
		}
	}

	private void DeferredConnectSerialReader()
	{
		if (serialReader != null && !serialReader.IsConnected(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated))))
		{
			serialReader.Connect(nameof(SerialReader.SensorActivatedEventHandler), new Callable(this, nameof(OnSensorActivated)));
			GD.Print("SerialReader conectado correctamente a Verde.");
		}
		else
		{
			GD.PrintErr("Error: No se pudo conectar el SerialReader a Verde.");
		}
	}

	// método que se llama cuando el sensor correspondiente es activado
	public void OnSensorActivated(int sensorNumber)
	{
		if (sensorNumber == 4)  // verifica al sensor 4 (Verde)
		{
			var animPlayer = GetNode<AnimationPlayer>("animVerde");
			if (animPlayer != null)
			{
				animPlayer.Play("ApretarVerde");
				GD.Print("Sensor Verde activado y animación ejecutada.");
			}
			else
			{
				GD.PrintErr("Error: AnimPlayer para Verde no encontrado.");
			}
		}
	}
}
