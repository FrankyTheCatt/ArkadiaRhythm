using Godot;
using System.Collections.Generic;
using System.IO;
using Godot.Collections; // referencia al espacio de nombres correcto para usar Dictionary


public partial class Level : Node2D
{
	protected List<float> positions = new List<float>(); // posiciones X de cada carril
	protected List<Timer> timers = new List<Timer>(); // temporizadores para cada carril
	protected PackedScene keyObjectScene;  // referencia a la plantilla del KeyObject
	protected SerialReader serialReader;
	private int playerHealth = 5; // Vida del jugador, puede ser modificada según sea necesario.
	private int maxLife = 5; // Define el máximo de vida del jugador
	private PackedScene damageNoteScene;
	private Timer bossLifeTimer;
	private int bossLifeIndex = 1;
	private int maxBossLife = 5; // Número máximo de vidas del boss
	private float songDuration = 185f; // Duración de la canción en segundos

	public void _Process()
	{
		Timer time = new Timer();
		GD.Print(time);
	}
	public override void _Ready()
	{
		GD.Print("Estructura del árbol de nodos: ");
		GetTree().Root.PrintTreePretty();  // verifica la estructura del árbol de nodos para saber si toma los nodos bn

		// cargaa la escena KeyObject para instanciar nuevas notas y que no se bugeee!!!!
		keyObjectScene = (PackedScene)ResourceLoader.Load("res://BASE JUEGO/KeyObject.tscn");

		// cargar la escena DamageNote para instanciar nuevas notas de daño
		damageNoteScene = (PackedScene)ResourceLoader.Load("res://Niveles/Nivel1/DamageNote.tscn");
		if (damageNoteScene == null)
		{
			GD.PrintErr("Error: No se pudo cargar la escena de DamageNote.");
			return;
		}

		// obtiene el nodo SerialReader
		serialReader = GetNodeOrNull<SerialReader>("Objects/SerialReader");
		if (serialReader == null)
		{
			GD.PrintErr("Error: SerialReader no encontrado en Level.");
			return;
		}
		GD.Print("SerialReader encontrado correctamente en Level.");

		// Obtiene los carriles y sus posiciones
		positions.Add(GetNode<Node2D>("Objects/ColorObject4").GlobalPosition.X); // Rojo
		positions.Add(GetNode<Node2D>("Objects/ColorObject3").GlobalPosition.X); // Amarillo
		positions.Add(GetNode<Node2D>("Objects/ColorObject2").GlobalPosition.X); // Azul
		positions.Add(GetNode<Node2D>("Objects/ColorObject").GlobalPosition.X);  // Verde

		GetNodeOrNull<Rojo>("Objects/ColorObject4/Rojo")?.SetSerialReader(serialReader);
		GetNodeOrNull<Verde>("Objects/ColorObject/Verde")?.SetSerialReader(serialReader);
		GetNodeOrNull<Azul>("Objects/ColorObject2/Azul")?.SetSerialReader(serialReader);
		GetNodeOrNull<Amarillo>("Objects/ColorObject3/Amarillo")?.SetSerialReader(serialReader);

		//timers para cada color
		Timer timer_rojo = new Timer();
		Timer timer_azul = new Timer();
		Timer timer_verde = new Timer();
		Timer timer_amarillo = new Timer();

		//DELAY
		Timer audioTimer = new Timer();
		audioTimer.WaitTime = 2.4f; // segundos de espera para que inicie la cancion
		audioTimer.OneShot = true;
		audioTimer.Timeout += () => GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play(0.0f);
		AddChild(audioTimer);
		audioTimer.Start();

		// Load JSON DAMAGENOTE
		Godot.Collections.Dictionary data2 = new Godot.Collections.Dictionary();
		string json2 = Json.Stringify(data2);
		string path2 = ProjectSettings.GlobalizePath("res://BASE JUEGO/mapeaditos/");
		GD.Print("JSON DAMAGENOTE");
		GD.Print(LoadTextFromFile(path2, "tiempos_damages_notes.json"));
		string loadedData2 = LoadTextFromFile(path2, "tiempos_damages_notes.json");

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

		for (int i = 0; i < damage_azul_times_list.Count; i++)
		{
			//GD.Print(damage_azul_times_list[i]);
		}

		var damage_verde_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict2["verde_times"])
		{
			damage_verde_times_list.Add((float)time);
		}

		for (int i = 0; i < damage_verde_times_list.Count; i++)
		{
			//GD.Print(damage_verde_times_list[i]);
		}

		var damage_amarillo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict2["amarillo_times"])
		{
			damage_amarillo_times_list.Add((float)time);
		}

		for (int i = 0; i < damage_amarillo_times_list.Count; i++)
		{
			//GD.Print(damage_amarillo_times_list[i]);
		}

		SetupDamageNoteTimersForLane(damage_rojo_times_list, 0);     // Carril rojo
		SetupDamageNoteTimersForLane(damage_azul_times_list, 1);     // Carril azul
		SetupDamageNoteTimersForLane(damage_verde_times_list, 2);    // Carril verde
		SetupDamageNoteTimersForLane(damage_amarillo_times_list, 3); // Carril amarillo

		// Load JSON
		Godot.Collections.Dictionary data = new Godot.Collections.Dictionary();
		string json = Json.Stringify(data);
		string path = ProjectSettings.GlobalizePath("res://BASE JUEGO/mapeaditos/");
		GD.Print("JSON KEYOBJECT");
		GD.Print(LoadTextFromFile(path, "tiempos_teclas.json"));
		string loadedData = LoadTextFromFile(path, "tiempos_teclas.json");

		Json jsonLoader = new Json();
		Error error = jsonLoader.Parse(loadedData);

		if (error != Error.Ok)
		{
			GD.Print(error);
			return;
		}
		Godot.Collections.Dictionary loadedDataDict = (Godot.Collections.Dictionary)jsonLoader.Data;

		var amarillo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["amarillo_times"])
		{
			amarillo_times_list.Add((float)time);

		}

		for (int i = 0; i < amarillo_times_list.Count; i++)
		{
			//GD.Print(amarillo_times_list[i]);
		}
		var verde_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["verde_times"])
		{
			verde_times_list.Add((float)time);
		}

		for (int i = 0; i < verde_times_list.Count; i++)
		{
			//GD.Print(verde_times_list[i]);
		}

		var azul_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["azul_times"])
		{
			azul_times_list.Add((float)time);
		}

		for (int i = 0; i < azul_times_list.Count; i++)
		{
			//GD.Print(azul_times_list[i]);
		}

		var rojo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["rojo_times"])
		{
			rojo_times_list.Add((float)time);
		}

		for (int i = 0; i < rojo_times_list.Count; i++)
		{
			//GD.Print(rojo_times_list[i]);
		}

		SetupTimersForLane(rojo_times_list, 0);     // Carril rojo
		SetupTimersForLane(azul_times_list, 1);     // Carril azul
		SetupTimersForLane(verde_times_list, 2);    // Carril verde
		SetupTimersForLane(amarillo_times_list, 3); // Carril amarillo
		RotateHeart(playerHealth);

			// Inicializa el Timer
	bossLifeTimer = new Timer();
	AddChild(bossLifeTimer);
	bossLifeTimer.OneShot = false;
	bossLifeTimer.Timeout += OnBossLifeTimerTimeout;

	// Inicia el proceso de eliminación de vidas del boss
	StartBossLifeCountdown();

	}
	//FUNCIONES

	// Método para girar todos los corazones
	private void RotateHeart(int heartIndex)
	{
		var animationPlayerPath = $"PlayerLife{heartIndex}/AnimationPlayer{heartIndex}";
		var animationPlayer = GetNodeOrNull<AnimationPlayer>(animationPlayerPath);
		if (animationPlayer != null)
		{
			animationPlayer.Play("girarPlayer"); // Reemplaza "girarPlayer" con el nombre de tu animación de rotación
			GD.Print($"Girando corazón {heartIndex}");
		}
		else
		{
			GD.PrintErr($"No se encontró el nodo: {animationPlayerPath}");
		}
	}

	private void SetupDamageNoteTimersForLane(List<float> timesList, int key)
	{
		foreach (var time in timesList)
		{
			Timer timer = new Timer();
			AddChild(timer);
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
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout"); // Espera 2.9 segundos antes de continuar

		Vector2 pos = new Vector2(positions[key], 0);  // posición inicial (X) del carril

		if (damageNoteScene != null)
		{
			var newDamageNote = (DamageNote)damageNoteScene.Instantiate();
			AddChild(newDamageNote);

			// Inicializa el DamageNote en el carril correcto
			newDamageNote.Spawn(key, pos);  // Posición en el carril especificado
			newDamageNote.SetSerialReader(serialReader);  // Asigna el SerialReader al nuevo objeto
			newDamageNote.Visible = true;  // Asegura que la nota sea visible
										   //GD.Print("Nota de daño generada en el carril: " + key);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de DamageNote.");
		}
	}
	public void TakeDamage(int damageAmount)
	{
		// Reducir la vida del jugador
		playerHealth -= damageAmount;
		GD.Print("Daño recibido: " + damageAmount + ". Vida restante: " + playerHealth);

		// Actualizar la vida del jugador
		UpdatePlayerLife(playerHealth);

		if (playerHealth <= 0)
		{
			GD.PrintErr("¡Game Over! Vida agotada.");
			// Lógica adicional si el jugador pierde toda la vida, como reiniciar el nivel
		}
	}

	public void GainLife(int lifeAmount)
	{
		// Incrementar la vida del jugador
		playerHealth += lifeAmount;
		if (playerHealth > maxLife)
		{
			playerHealth = maxLife; // Asegurarse de que la vida no exceda el máximo
		}
		GD.Print("Vida ganada: " + lifeAmount + ". Vida restante: " + playerHealth);

		// Actualizar la vida del jugador
		UpdatePlayerLife(playerHealth);
	}

	public void UpdatePlayerLife(int newLife)
	{
		if(newLife <= 0) {
			Die();
		}
		
		for (int i = 1; i <= maxLife; i++)
		{
			var spritePath = $"PlayerLife{i}";
			var sprite = GetNodeOrNull<Sprite2D>(spritePath);
			if (sprite != null)
			{
				bool wasVisible = sprite.Visible;
				sprite.Visible = i <= newLife;

				var animationPlayerPath = $"{spritePath}/AnimationPlayer{i}";
				var animationPlayer = GetNodeOrNull<AnimationPlayer>(animationPlayerPath);
				if (animationPlayer != null)
				{
					if (sprite.Visible && !wasVisible)
					{
						RotateHeart(i); // Llama a RotateHeart para rotar el corazón cuando se vuelve visible
					}
					else if (!sprite.Visible)
					{
						animationPlayer.Stop();
					}
				}
			}
		}
	}
	
	private void Die() {
		GetTree().ChangeSceneToFile("res://MENU/DefeatScene.tscn");
	}
	
	private void SetupTimersForLane(List<float> timesList, int key)
	{
		foreach (var time in timesList)
		{
			Timer timer = new Timer();
			AddChild(timer);
			timer.WaitTime = time;
			timer.OneShot = true;
			// Conecta la señal Timeout para llamar a una función que spawnea el objeto en el carril especificado
			timer.Timeout += () => SpawnKeyObjectInLane(key);
			timer.Start();
		}
	}

	private void SpawnKeyObjectInLane(int key)
	{
		Vector2 pos = new Vector2(positions[key], 0);  // posición inicial (X) del carril

		if (keyObjectScene != null)
		{
			var newKeyObject = (KeyObject)keyObjectScene.Instantiate();
			AddChild(newKeyObject);

			// Inicializa el KeyObject en el carril correcto
			newKeyObject.Spawn(key, pos);  // Posición en el carril especificado
			newKeyObject.SetSerialReader(serialReader);  // Asigna el SerialReader al nuevo objeto
			newKeyObject.Visible = true;  // Asegura que la nota sea visible
										  //GD.Print("Nota generada en el carril: " + key);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de KeyObject.");
		}
	}

	private string LoadTextFromFile(string path, string fileName)
	{
		string data = null;
		path = Path.Join(path, fileName);

		if (!File.Exists(path)) return null;

		try
		{
			data = File.ReadAllText(path);
		}
		catch (System.Exception e)
		{
			GD.Print(e);
		}
		return data;
	}

	//BOSS FUNCION HEALTH
public void StartBossLifeCountdown()
{
	if (bossLifeTimer == null)
	{
		GD.PrintErr("Timer no está inicializado.");
		return;
	}

	float interval = songDuration / maxBossLife; // Dividir la duración de la canción entre el número de vidas del boss

	bossLifeTimer.WaitTime = interval;
	bossLifeTimer.Start();
}

private void OnBossLifeTimerTimeout()
{
	if (bossLifeIndex <= maxBossLife)
	{
		var bossLifePath = $"BossLife{bossLifeIndex}";
		var bossLifeNode = GetNodeOrNull<CanvasItem>(bossLifePath);
		if (bossLifeNode != null)
		{
			bossLifeNode.Visible = false;
			GD.Print($"BossLife{bossLifeIndex} se ha hecho invisible.");
		}
		bossLifeIndex++;
		if(bossLifeIndex == maxBossLife) {
			bossLifeTimer.Stop();
			GetTree().ChangeSceneToFile("res://MENU/VictoryScene.tscn");
		}
	} else {
		// pone aca lo que quieras uwu oh no FATAL ERRROR
		GD.PrintErr("FATAL ERROR, ERROR MATEMÁTICO");
	}
}
}
