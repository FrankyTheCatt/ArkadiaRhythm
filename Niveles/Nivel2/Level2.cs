using Godot;
using System.Collections.Generic;
using GodotDictionary = Godot.Collections.Dictionary; // Alias para evitar ambigüedades

public partial class Level2 : Level
{
	private PackedScene frozenNoteScene;
	private bool[] lanesFrozen = new bool[4]; // Estado de congelación de los carriles
	private int[] freezeCounters = new int[4]; // Contadores de "descongelar" para cada carril

	public override void InitializeLevel()
	{
		// cargar la escena FrozenNote para instanciar nuevas notas congeladas
		frozenNoteScene = (PackedScene)ResourceLoader.Load("res://Niveles/Nivel2/FrozenNote.tscn");
		if (frozenNoteScene == null)
		{
			GD.PrintErr("Error: No se pudo cargar la escena de FrozenNote.");
			return;
		}
		else
		{
			GD.Print("Escena de FrozenNote cargada correctamente.");
		}

		// Load JSON FrozenNote
		GodotDictionary data = new GodotDictionary();
		string json = Json.Stringify(data);
		string path = ProjectSettings.GlobalizePath("res://BASE JUEGO/mapeaditos/");
		GD.Print("JSON FrozenNote");
		GD.Print(base.LoadTextFromFile(path, "tiempos_frozen_notes.json"));
		string loadedData = base.LoadTextFromFile(path, "tiempos_frozen_notes.json");

		Json jsonLoader = new Json();
		Error error = jsonLoader.Parse(loadedData);

		if (error != Error.Ok)
		{
			GD.Print(error);
			return;
		}
		GodotDictionary loadedDataDict = (GodotDictionary)jsonLoader.Data;

		// Procesar tiempos de frozen notes
		var frozen_rojo_times_list = new List<float>();
		foreach (var time in (Godot.Collections.Array)loadedDataDict["rojo_times"])
		{
			frozen_rojo_times_list.Add((float)time);
		}

		var frozen_azul_times_list = new List<float>();
		foreach (var time in (Godot.Collections.Array)loadedDataDict["azul_times"])
		{
			frozen_azul_times_list.Add((float)time);
		}

		var frozen_verde_times_list = new List<float>();
		foreach (var time in (Godot.Collections.Array)loadedDataDict["verde_times"])
		{
			frozen_verde_times_list.Add((float)time);
		}

		var frozen_amarillo_times_list = new List<float>();
		foreach (var time in (Godot.Collections.Array)loadedDataDict["amarillo_times"])
		{
			frozen_amarillo_times_list.Add((float)time);
		}

		SetupFrozenNoteTimersForLane(frozen_rojo_times_list, 0);     // Carril rojo
		SetupFrozenNoteTimersForLane(frozen_azul_times_list, 1);     // Carril azul
		SetupFrozenNoteTimersForLane(frozen_verde_times_list, 2);    // Carril verde
		SetupFrozenNoteTimersForLane(frozen_amarillo_times_list, 3); // Carril amarillo

		// GLOBALES
		base.songDuration = 150f;
		base.pathDefeat = "res://MENU/DefeatScene.tscn";
		base.pathVictory = "res://MENU/VictoryScene.tscn";
		base.maxBossLife = 5; // Vida del boss, puede ser modificada según sea necesario.
		base.playerHealth = 5; // Vida del jugador, puede ser modificada según sea necesario.
		base.maxLife = 5; // Define el máximo de vida del jugador

		// Inicializar estados de los carriles
		for (int i = 0; i < lanesFrozen.Length; i++)
		{
			lanesFrozen[i] = false;
			freezeCounters[i] = 0;
		}
	}

	private void SetupFrozenNoteTimersForLane(List<float> timesList, int key)
	{
		foreach (var time in timesList)
		{
			Timer timer = new Timer();
			base.AddChild(timer);
			timer.WaitTime = time;
			timer.OneShot = true;

			// Conecta la señal Timeout para llamar a una función que spawnea el objeto en el carril especificado
			timer.Timeout += () => SpawnFrozenNoteInLane(key);
			timer.Start();
		}
	}

	private async void SpawnFrozenNoteInLane(int key)
	{
		await base.ToSignal(base.GetTree().CreateTimer(1.5f), "timeout"); // Espera 1.5 segundos antes de continuar

		Vector2 pos = new Vector2(base.positions[key], 0); // posición inicial (X) del carril

		if (frozenNoteScene != null)
		{
			var newFrozenNote = (FrozenNote)frozenNoteScene.Instantiate();
			base.AddChild(newFrozenNote);

			// Inicializa la FrozenNote en el carril correcto
			newFrozenNote.Spawn(key, pos); // Posición en el carril especificado
			newFrozenNote.SetSerialReader(base.serialReader); // Asigna el SerialReader al nuevo objeto
			newFrozenNote.Visible = true; // Asegura que la nota sea visible

			// Congela el carril al colisionar
			// Suscribe el método FreezeLane al evento OnNoteHit
			newFrozenNote.OnNoteHit += () => FreezeLane(key);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de FrozenNote.");
		}
	}

	public override void FreezeLane(int lane)
	{
		lanesFrozen[lane] = true;
		freezeCounters[lane] = 3; // Establece el número de presiones necesarias para descongelar
		GD.Print($"Carril {lane} congelado.");
	}

	
}
