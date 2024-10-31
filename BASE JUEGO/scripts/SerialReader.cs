using Godot;
using System;
using System.IO.Ports;

public partial class SerialReader : Node
{
	[Signal]  // registro correcto para que tome bn la señal
	public delegate void SensorActivatedEventHandler(int sensorNumber);

	private SerialPort serialPort;
	public string portName = "COM3"; // puerto del pc para usar
	public int baudRate = 9600;

	public override void _Ready()
	{
		// verificar que la señal esté bn registrada
		if (!HasUserSignal("SensorActivatedEventHandler"))
		{
			AddUserSignal("SensorActivatedEventHandler");
			GD.Print("Señal SensorActivatedEventHandler registrada correctamente.");
		}

		// abre el puerto serial
		try
		{
			serialPort = new SerialPort(portName, baudRate);
			serialPort.Open();
			GD.Print("Puerto serial abierto correctamente.");
		}
		catch (Exception e)
		{
			GD.PrintErr("Error al abrir el puerto serial: " + e.Message);
		}
	}

	public override void _Process(double delta)
	{
		if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
		{
			try
			{
				string data = serialPort.ReadLine().Trim();
				GD.Print("Datos recibidos del serial: " + data);
				EmitSignal(nameof(SensorActivatedEventHandler), int.Parse(data));
			}
			catch (Exception e)
			{
				GD.PrintErr("Error al leer del puerto serial: " + e.Message);
			}
		}
	}

	public override void _ExitTree()
	{
		if (serialPort != null && serialPort.IsOpen)
		{
			serialPort.Close();
			GD.Print("Puerto serial cerrado.");
		}
	}
}
