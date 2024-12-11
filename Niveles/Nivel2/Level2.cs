using Godot;
using System.Collections.Generic;
using System.IO;
using Godot.Collections; // referencia al espacio de nombres correcto para usar Dictionary

public partial class Level2 : Level
{
	private PackedScene damageNoteScene;

	public override void InitializeLevel()
	{	
		//GLOBALES
		base.songDuration = 150f;
		base.pathDefeat = "res://MENU/DefeatScene.tscn";
		base.pathVictory = "res://MENU/VictoryScene.tscn";
		base.maxBossLife = 5; // Vida del boss, puede ser modificada según sea necesario.
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
