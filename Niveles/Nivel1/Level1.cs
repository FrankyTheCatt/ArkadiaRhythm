using Godot;
using System.Collections.Generic;
using System.IO;
using Godot.Collections; // referencia al espacio de nombres correcto para usar Dictionary

public partial class Level1 : Level
{
	private PackedScene damageNoteScene;

	public override void InitializeLevel()
	{
		// cargar la escena DamageNote para instanciar nuevas notas de daño
		damageNoteScene = (PackedScene)ResourceLoader.Load("res://Niveles/Nivel1/DamageNote.tscn");
		if (damageNoteScene == null)
		{
			GD.PrintErr("Error: No se pudo cargar la escena de DamageNote.");
			return;
		}
		else{
			GD.Print("Escena de DamageNote cargada correctamente.");
		}

		// Load JSON DAMAGENOTE
		Godot.Collections.Dictionary data2 = new Godot.Collections.Dictionary();
		string json2 = Json.Stringify(data2);
		string path2 = ProjectSettings.GlobalizePath("res://BASE JUEGO/mapeaditos/");
		GD.Print("JSON DAMAGENOTE");
		GD.Print(base.LoadTextFromFile(path2, "tiempos_damages_notes.json"));
		string loadedData2 = base.LoadTextFromFile(path2, "tiempos_damages_notes.json");

		Json jsonLoader2 = new Json();
		Error error2 = jsonLoader2.Parse(loadedData2);

		if (error2 != Error.Ok)
		{
			GD.Print(error2);
			return;
		}
		Godot.Collections.Dictionary loadedDataDict2 = (Godot.Collections.Dictionary)jsonLoader2.Data;

		// Procesar tiempos de damage notes
		var damage_rojo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict2["rojo_times"])
		{
			damage_rojo_times_list.Add((float)time);
		}

		for (int i = 0; i < damage_rojo_times_list.Count; i++)
		{
			//GD.Print(damage_rojo_times_list[i]);
		}

		var damage_azul_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict2["azul_times"])
		{
			damage_azul_times_list.Add((float)time);
		}

		var damage_verde_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict2["verde_times"])
		{
			damage_verde_times_list.Add((float)time);
		}

		var damage_amarillo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict2["amarillo_times"])
		{
			damage_amarillo_times_list.Add((float)time);
		}

		SetupDamageNoteTimersForLane(damage_rojo_times_list, 0);     // Carril rojo
		SetupDamageNoteTimersForLane(damage_azul_times_list, 1);     // Carril azul
		SetupDamageNoteTimersForLane(damage_verde_times_list, 2);    // Carril verde
		SetupDamageNoteTimersForLane(damage_amarillo_times_list, 3); // Carril amarillo
		
		
		//GLOBALES
		base.songDuration = 185f;
		base.pathDefeat = "res://MENU/DefeatScene.tscn";
		base.pathVictory = "res://MENU/VictoryScene.tscn";
		base.maxBossLife = 5; 
		base.playerHealth=5; // Vida del jugador, puede ser modificada según sea necesario.
		base.maxLife = 5;// Define el máximo de vida del jugador

	}

		private void SetupDamageNoteTimersForLane(List<float> timesList, int key)
	{
		foreach (var time in timesList)
		{
			Timer timer = new Timer();
			base.AddChild(timer);
			timer.WaitTime = time;
			timer.OneShot = true;

			// Conecta la señal Timeout para llamar a una función que spawnea el objeto en el carril especificado
			timer.Timeout += () => SpawnDamageNoteInLane(key);
			timer.Start();
		}
	}
	// método que se llama cuando un temporizador se activa
	private async void SpawnDamageNoteInLane(int key)
	{
		await base.ToSignal(base.GetTree().CreateTimer(1.5f), "timeout"); // Espera 2.9 segundos antes de continuar

		Vector2 pos = new Vector2(base.positions[key], 0);  // posición inicial (X) del carril

		if (damageNoteScene != null)
		{
			var newDamageNote = (DamageNote)damageNoteScene.Instantiate();
			base.AddChild(newDamageNote);

			// Inicializa el DamageNote en el carril correcto
			newDamageNote.Spawn(key, pos);  // Posición en el carril especificado
			newDamageNote.SetSerialReader(base.serialReader);  // Asigna el SerialReader al nuevo objeto
			newDamageNote.Visible = true;  // Asegura que la nota sea visible
										   //GD.Print("Nota de daño generada en el carril: " + key);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de DamageNote.");
		}
	}
}
