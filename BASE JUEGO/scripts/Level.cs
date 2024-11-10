using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections; // Importa el namespace de Godot para usar Godot.Collections.Dictionary

public partial class Level : Node2D
{
	private List<float> amarilloTimes = new List<float>();
	private List<float> azulTimes = new List<float>();
	private List<float> rojoTimes = new List<float>();
	private List<float> verdeTimes = new List<float>();

	private List<float> positions = new List<float>(); // posiciones X de cada carril
	private List<Timer> timers = new List<Timer>(); // temporizadores para cada carril
	private PackedScene keyObjectScene;  // referencia a la plantilla del KeyObject
	private SerialReader serialReader;

	public override void _Ready()
	{
		GD.Print("Estructura del árbol de nodos: ");
		GetTree().Root.PrintTreePretty();

		keyObjectScene = (PackedScene)ResourceLoader.Load("res://BASE JUEGO/KeyObject.tscn");

		serialReader = GetNodeOrNull<SerialReader>("Objects/SerialReader");
		if (serialReader == null)
		{
			GD.PrintErr("Error: SerialReader no encontrado en Level.");
			return;
		}
		GD.Print("SerialReader encontrado correctamente en Level.");

		// Agregar posiciones de cada carril
		positions.Add(GetNode<Node2D>("Objects/ColorObject4").GlobalPosition.X); // Rojo
		positions.Add(GetNode<Node2D>("Objects/ColorObject3").GlobalPosition.X); // Amarillo
		positions.Add(GetNode<Node2D>("Objects/ColorObject2").GlobalPosition.X); // Azul
		positions.Add(GetNode<Node2D>("Objects/ColorObject").GlobalPosition.X);  // Verde

		// Configura cada carril con el SerialReader
		GetNodeOrNull<Rojo>("Objects/ColorObject4/Rojo")?.SetSerialReader(serialReader);
		GetNodeOrNull<Verde>("Objects/ColorObject/Verde")?.SetSerialReader(serialReader);
		GetNodeOrNull<Azul>("Objects/ColorObject2/Azul")?.SetSerialReader(serialReader);
		GetNodeOrNull<Amarillo>("Objects/ColorObject3/Amarillo")?.SetSerialReader(serialReader);

		// Cargar y parsear el archivo JSON
		LoadJsonData("res://path/to/yourfile.json");

		// Crear y configurar temporizadores usando las listas
		SetupTimers();
	}

	private void LoadJsonData(string filePath)
	{
		var file = new File();
		if (file.FileExists(filePath))
		{
			file.Open(filePath, File.ModeFlags.Read);
			string jsonData = file.GetAsText();
			file.Close();

			// Parsear el JSON usando el sistema de Godot
			var parsedData = JSON.Parse(jsonData);
			if (parsedData.Error == Error.Ok && parsedData.Result is Godot.Collections.Dictionary dataDict)
			{
				FillTimeList(dataDict, "amarillo_times", amarilloTimes);
				FillTimeList(dataDict, "azul_times", azulTimes);
				FillTimeList(dataDict, "rojo_times", rojoTimes);
				FillTimeList(dataDict, "verde_times", verdeTimes);
			}
			else
			{
				GD.PrintErr("Error al parsear el archivo JSON.");
			}
		}
		else
		{
			GD.PrintErr("Error: El archivo JSON no existe en la ruta especificada.");
		}
	}

	private void FillTimeList(Godot.Collections.Dictionary dataDict, string key, List<float> timeList)
	{
		if (dataDict.ContainsKey(key) && dataDict[key] is Godot.Collections.Array timesArray)
		{
			foreach (var time in timesArray)
			{
				if (time is float timeValue)
				{
					timeList.Add(timeValue);
				}
			}
		}
	}

	private void SetupTimers()
	{
		List<List<float>> allTimes = new List<List<float>> { rojoTimes, amarilloTimes, azulTimes, verdeTimes };

		for (int i = 0; i < 4; i++)
		{
			Timer timer = new Timer();
			if (allTimes[i].Count > 0)
			{
				timer.WaitTime = allTimes[i][0]; // Usa el primer valor como tiempo del temporizador
				allTimes[i].RemoveAt(0);         // Elimina el valor ya usado
			}
			else
			{
				timer.WaitTime = 2.0f; // Tiempo predeterminado si la lista está vacía
			}
			timer.OneShot = false;
			timer.Autostart = true;
			timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
			timers.Add(timer);
			AddChild(timer);
		}
	}

	private void OnTimerTimeout()
	{
		int key = (int)(GD.Randi() % 4);
		Vector2 pos = new Vector2(positions[key], 0);

		if (keyObjectScene != null)
		{
			var newKeyObject = (KeyObject)keyObjectScene.Instantiate();
			AddChild(newKeyObject);

			newKeyObject.Spawn(key, pos);
			newKeyObject.SetSerialReader(serialReader);
			newKeyObject.Visible = true;
			GD.Print("Nota generada en el carril: " + key);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de KeyObject.");
		}
	}
}
