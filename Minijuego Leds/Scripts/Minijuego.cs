using Godot;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

public partial class Minijuego : Node
{
	private SerialPort _arduino;
	private double _elapsedTime = 0.0; // Tiempo transcurrido en segundos
	private List<LedEvent> _sequence = new List<LedEvent>
	{
		new LedEvent(1, 0.0, 1.0),
		new LedEvent(2, 1.0, 1.0), // LED 2, se enciende a los 0.5s y dura 1.0s
		new LedEvent(3, 2.0, 1.0), // LED 3, se enciende a los 1.0s y dura 1.5s
		new LedEvent(4, 3.0, 1.0),  // LED 4, se enciende a los 1.5s y dura 1.0s
		new LedEvent(3, 5.0, 2.0)
		
		
	};

	private Dictionary<int, int> _ledMapping = new Dictionary<int, int>
	{
		{1, 1}, // LED 1 mapeado al pin digital 5
		{2, 2}, // LED 2 mapeado al pin digital 4
		{3, 3}, // LED 3 mapeado al pin digital 3
		{4, 4}  // LED 4 mapeado al pin digital 2
	};
	
	private Dictionary<int, double> _lastSensorActivation = new Dictionary<int, double>(); // Última activación por sensor
	private double _debounceTime = 0.5; // Tiempo muerto para señales repetidas (en segundos)
	
	private bool _patternCompleted = false;
	private int _ledsCompleted = 0;
	
	public override void _Ready()
	{
		try
		{
			_arduino = new SerialPort("COM3", 9600); // Cambia "COM3" según el puerto en tu sistema
			_arduino.Open();
			GD.Print("Conexión con Arduino establecida.");
		}
		catch (Exception e)
		{
			GD.PrintErr("Error al conectar con Arduino: ", e.Message);
		}
	}

	public override void _Process(double delta)
	{
		if (_patternCompleted){
			return;
		}
		
		_elapsedTime += delta;

		foreach (var ledEvent in _sequence)
		{
			// Omitir eventos ya completados
			if (ledEvent.IsCompleted) continue;
			
			if (!ledEvent.IsActive && (_elapsedTime >= ledEvent.StartTime && _elapsedTime <= ledEvent.StartTime + ledEvent.Duration))
			{
				ActivateLed(ledEvent);
			}
			
			// Leer entrada analógica mientras el LED está activo
			if (ledEvent.IsActive && _arduino.BytesToRead > 0)
			{
				string input = _arduino.ReadLine().Trim();
				if (int.TryParse(input, out int sensorPressed))
				{
					ProcessSensorInput(sensorPressed, ledEvent);
				}
			}
				
			if (ledEvent.IsActive && (_elapsedTime >= ledEvent.StartTime + ledEvent.Duration && _elapsedTime <= ledEvent.StartTime + ledEvent.Duration + 0.5))
			{
				GD.Print($"Tiempo agotado para LED {ledEvent.Led}.");
				DeactivateLed(ledEvent, true); // Quita vida
			}
		}
		
		// Verificar si todos los LEDs se han completado
		if (_ledsCompleted >= _sequence.Count)
		{
			_patternCompleted = true; // Marca que el patrón se ha completado
			GD.Print("Patrón de LEDs completado.");
		}
		
	}

	public override void _ExitTree()
	{
		if (_arduino != null && _arduino.IsOpen)
		{
			_arduino.Close();
		}
	}

	private void ActivateLed(LedEvent ledEvent)
	{
		GD.Print($"Activando LED {ledEvent.Led}");
		_arduino.WriteLine($"ON:{_ledMapping[ledEvent.Led]}"); // Envía comando para encender el LED
		ledEvent.IsActive = true;
	}

	private void DeactivateLed(LedEvent ledEvent, bool penalize)
	{
		GD.Print($"Desactivando LED {ledEvent.Led}");
		_arduino.WriteLine($"OFF:{_ledMapping[ledEvent.Led]}"); // Envía comando para apagar el LED
		ledEvent.IsActive = false;
		
		// Aumentar el contador de LEDs completados
		_ledsCompleted++;
		
		if (penalize)
		{
			QuitarVida();
			}
	}
	
	private void QuitarVida()
	{
		GD.Print("¡Vida reducida!");
		// Aquí puedes implementar la lógica para reducir la vida del jugador.
	}
	
	private void ProcessSensorInput(int sensorPressed, LedEvent ledEvent)
	{
		// Mapea las entradas analógicas (A0, A1, ...) a LEDs (1, 2, ...)
		int expectedSensor = ledEvent.Led;
		
		// Verifica si el sensor activado corresponde al LED actual
		if (sensorPressed != expectedSensor)
		{
			GD.Print($"Sensor {sensorPressed} no corresponde al LED {expectedSensor}. Ignorando.");
			return;
		}
		// Verifica el tiempo muerto (debounce) para evitar activaciones repetidas
		if (!_lastSensorActivation.ContainsKey(sensorPressed))
		{
			_lastSensorActivation[sensorPressed] = -_debounceTime; // Inicializa si no existe
		}
		if ((_elapsedTime - _lastSensorActivation[sensorPressed]) < _debounceTime)
		{
			GD.Print($"Input ignorado por debounce: Sensor {sensorPressed}");
			return;
		}
		// Si es válido, apaga el LED y actualiza el tiempo de activación
		GD.Print($"Jugador apagó LED {ledEvent.Led}.");
		DeactivateLed(ledEvent, false); // No quita vida
		_lastSensorActivation[sensorPressed] = _elapsedTime;
		ledEvent.IsCompleted = true;
		}
		
	private class LedEvent
	{
		public int Led { get; }             // Número del LED (1 a 4)
		public double StartTime { get; }    // Momento en que el LED debe encenderse
		public double Duration { get; }     // Tiempo que el LED debe permanecer encendido
		public bool IsActive { get; set; }  // Si el evento está activo o no
		public bool IsCompleted { get; set; } // Marca si el evento ya fue completado
		
		public LedEvent(int led, double startTime, double duration)
		{
			Led = led;
			StartTime = startTime;
			Duration = duration;
			IsActive = false;
			IsCompleted = false;
		}
	}
}
