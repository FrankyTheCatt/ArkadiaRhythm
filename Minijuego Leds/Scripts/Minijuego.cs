using Godot;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

public partial class Minijuego : Node
{
	[Export] public NodePath PlayerLife;
	private PlayerLife _player;
	private SerialPort _arduino;
	private AudioStreamPlayer _audioPlayer;
	private double _elapsedTime = 0.0; // Tiempo transcurrido en segundos
	private List<LedEvent> _sequence = new List<LedEvent>
	{
		new LedEvent(1, 10.0, 3.0),
		new LedEvent(2, 11.0, 3.0), // LED 2, se enciende a los 11.0s y dura 3.0s
		new LedEvent(3, 12.0, 2.0), // LED 3, se enciende a los 12.0s y dura 2.0s
		new LedEvent(4, 13.0, 1.3), // LED 4, se enciende a los 13.0s y dura 1.3s
		new LedEvent(3, 15.0, 1.3),
		new LedEvent(4, 15.0, 1.3),
		new LedEvent(1, 16.0, 1.3),
		new LedEvent(2, 16.0, 1.3),
		new LedEvent(3, 17.5, 1.3),
		new LedEvent(2, 18.0, 1.3),
		new LedEvent(3, 19.0, 1.3),
		new LedEvent(1, 19.0, 1.3),
		new LedEvent(4, 20.0, 1.3),
		new LedEvent(2, 20.0, 1.3),
		new LedEvent(4, 21.5, 1.3),
		new LedEvent(2, 21.5, 1.3),
		new LedEvent(1, 23.5, 1.3),
		new LedEvent(4, 24.0, 1.3),
		new LedEvent(3, 24.0, 1.3),
		new LedEvent(2, 25.0, 1.3),
		new LedEvent(1, 26.0, 1.3),
		new LedEvent(3, 26.5, 1.3),
		new LedEvent(4, 26.5, 1.3),
		new LedEvent(2, 27.0, 1.3),
		new LedEvent(1, 29.0, 1.3),
		new LedEvent(4, 29.0, 1.3),
		new LedEvent(3, 30.0, 1.3),
		new LedEvent(2, 30.0, 1.3),
		new LedEvent(1, 30.5, 1.3),
		new LedEvent(4, 32.0, 1.3),
		new LedEvent(2, 32.5, 1.3),
		new LedEvent(3, 34.0, 1.3),
		new LedEvent(1, 35.0, 1.3),
		new LedEvent(2, 35.0, 1.3),
		new LedEvent(3, 36.0, 1.3),
		new LedEvent(4, 37.0, 1.3),
		new LedEvent(2, 38.0, 1.3),
		new LedEvent(3, 39.0, 1.3),
		new LedEvent(2, 40.0, 1.3),
		new LedEvent(1, 40.0, 1.3),
		new LedEvent(3, 41.0, 1.3),
		new LedEvent(4, 41.0, 1.3),
		new LedEvent(2, 42.0, 1.3),
		new LedEvent(1, 42.0, 1.3),
		new LedEvent(3, 43.0, 1.3),
		new LedEvent(4, 43.5, 1.3),
		new LedEvent(1, 45.0, 1.3),
		new LedEvent(3, 45.0, 1.3),
		new LedEvent(2, 46.0, 1.3),
		new LedEvent(4, 46.0, 1.3),
		new LedEvent(1, 47.0, 1.3),
		new LedEvent(3, 47.5, 1.3),
		new LedEvent(4, 48.0, 1.3),
		new LedEvent(2, 49.0, 1.3),
		new LedEvent(4, 50.0, 1.3),
		new LedEvent(1, 50.5, 1.3),
		new LedEvent(3, 50.5, 1.3),
		new LedEvent(2, 52.0, 1.3),
		new LedEvent(4, 53.5, 1.3),
		new LedEvent(3, 53.5, 1.3),
		new LedEvent(1, 55.0, 1.3),
		new LedEvent(2, 55.0, 1.3),
		new LedEvent(3, 56.0, 1.3),
		new LedEvent(4, 56.5, 1.3),
		new LedEvent(1, 58.0, 1.3),
		new LedEvent(3, 59.0, 1.3),
		new LedEvent(2, 59.0, 1.3),
		new LedEvent(4, 60.0, 1.3),
		new LedEvent(1, 60.0, 1.3)
		
	};

	private Dictionary<int, int> _ledMapping = new Dictionary<int, int>
	{
		{1, 1}, 
		{2, 2}, 
		{3, 3}, 
		{4, 4}  
	};
	
	private Dictionary<int, double> _lastSensorActivation = new Dictionary<int, double>(); // Última activación por sensor
	private double _debounceTime = 0.5; // Tiempo muerto para señales repetidas (en segundos)
	
	private bool _patternCompleted = false;
	private int _ledsCompleted = 0;
	
	public override void _Ready()
	{
		_audioPlayer = GetNode<AudioStreamPlayer>("sonido");
		_player = GetNode<PlayerLife>("UiPersonaje/Player2");
		res://UI/PlayerLife.cs
		if (_player == null)
		{
			GD.PrintErr("No se pudo encontrar al jugador. Asegúrate de configurar PlayerPath.");
		}
		
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
			
			// Apagar todos los LEDs
			foreach (var led in _ledMapping.Values)
			{
				_arduino.WriteLine($"OFF:{led}");
			}
			GD.Print("Todos los LEDs apagados.");
			
		}
		if (_player.CurrentHealth == 0)
		{
			// Apagar todos los LEDs
			foreach (var led in _ledMapping.Values)
			{
				_arduino.WriteLine($"OFF:{led}");
			}
			GD.Print("Todos los LEDs apagados.");
			GetTree().ChangeSceneToFile("res://MENU/Defeat_leds.tscn");
		}
		
		//CAMBIO A ESCENA DE VICTORIA
		if( _elapsedTime == 70.0)
		{
			GetTree().ChangeSceneToFile("res://MENU/Victory_leds.tscn");
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
		_audioPlayer.Play();
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
		_player.TakeDamage(25);
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
		// Si es válido, apaga el LED y actualiza el tiempo de activación
		GD.Print($"Jugador apagó LED {ledEvent.Led}.");
		DeactivateLed(ledEvent, false); // No quita vida
		_player.Heal(15);
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
