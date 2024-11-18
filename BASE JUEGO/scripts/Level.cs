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
	private float playerHealth = 100f; // Vida del jugador, puede ser modificada según sea necesario.
	private PackedScene damageNoteScene;
	
	public void _Process(){
		Timer time = new Timer();
		GD.Print(time);
	}
	public override void _Ready()
	{
		GD.Print("Estructura del árbol de nodos: ");
		GetTree().Root.PrintTreePretty();  // verifica la estructura del árbol de nodos para saber si toma los nodos bn

		// cargaa la escena KeyObject para instanciar nuevas notas y que no se bugeee!!!!
		keyObjectScene = (PackedScene)ResourceLoader.Load("res://BASE JUEGO/KeyObject.tscn");

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

		// Load JSON
		Godot.Collections.Dictionary data = new Godot.Collections.Dictionary();
		string json = Json.Stringify(data);
		string path = ProjectSettings.GlobalizePath("res://BASE JUEGO/mapeaditos/");
		GD.Print(LoadTextFromFile(path, "tiempos_teclas.json"));	
		string loadedData = LoadTextFromFile(path, "tiempos_teclas.json");

		Json jsonLoader = new Json();
		Error error = jsonLoader.Parse(loadedData);

		if(error != Error.Ok){
			GD.Print(error);
			return;
		}
		Godot.Collections.Dictionary loadedDataDict = (Godot.Collections.Dictionary)jsonLoader.Data;

		//GD.Print(loadedDataDict["amarillo_times"]);

		var amarillo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["amarillo_times"])
		{
			amarillo_times_list.Add((float)time);

		}

		for (int i = 0; i < amarillo_times_list.Count; i++)
		{
			GD.Print(amarillo_times_list[i]);
		}
		var verde_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["verde_times"])
		{
			verde_times_list.Add((float)time);
		}

		for (int i = 0; i < verde_times_list.Count; i++)
		{
			GD.Print(verde_times_list[i]);
		}

		var azul_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["azul_times"])
		{
			azul_times_list.Add((float)time);
		}

		for (int i = 0; i < azul_times_list.Count; i++)
		{
			GD.Print(azul_times_list[i]);
		}

		var rojo_times_list = new List<float>();
		foreach (var time in (Array)loadedDataDict["rojo_times"])
		{
			rojo_times_list.Add((float)time);
		}

		 for (int i = 0; i < rojo_times_list.Count; i++)
		 {
		 	GD.Print(rojo_times_list[i]);
		 }

	SetupTimersForLane(rojo_times_list, 0);     // Carril rojo
	SetupTimersForLane(azul_times_list, 1);     // Carril azul
	SetupTimersForLane(verde_times_list, 2);    // Carril verde
	SetupTimersForLane(amarillo_times_list, 3); // Carril amarillo

		// Crear temporizadores para los carriles
		for (int i = 0; i < 4; i++)
		{
			Timer timer = new Timer();
			timer.WaitTime = (float)GD.RandRange(1.0, 3.0);  // Ritmo aleatorio para cada carril
			timer.OneShot = false;
			timer.Autostart = true;
			timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
			timers.Add(timer);
			AddChild(timer);
		}
		
		damageNoteScene = (PackedScene)ResourceLoader.Load("res://Niveles/Nivel1/DamageNote.tscn");
		// Configurar un temporizador para generar notas de daño
		Timer damageNoteTimer = new Timer();
		damageNoteTimer.WaitTime = (float)GD.RandRange(2.0, 5.0); // Intervalo aleatorio para las notas de daño
		damageNoteTimer.OneShot = false;
		damageNoteTimer.Autostart = true;
		damageNoteTimer.Connect("timeout", new Callable(this, nameof(SpawnDamageNote)));
		AddChild(damageNoteTimer);
}

// método que se llama cuando un temporizador se activa
protected void OnTimerTimeout()
{
	int key = (int)(GD.Randi() % 3);  // escoge un carril al azar de los cuatro
	Vector2 pos = new Vector2(positions[key], 0);  // posición inicial (X) del carril

	// instanciar un nuevo DamageNote a partir de la plantilla
	if (damageNoteScene != null)
	{
		var newDamageNote = (DamageNote)damageNoteScene.Instantiate();
		AddChild(newDamageNote);

		// logica para inicializar el DamageNote en el carril correcto
		newDamageNote.GlobalPosition = pos;  // Spawn de la nota en la posición del carril
		newDamageNote.SetSerialReader(serialReader);  // asignar el SerialReader al nuevo objeto para crear la nota
		newDamageNote.Visible = true;   // asegurarse de que sea visible la nota
		GD.Print("Nota de daño generada en el carril: " + key);
	}
	else
	{
		GD.PrintErr("Error: No se pudo cargar la escena de DamageNote.");
	}
}

  private void SpawnDamageNote()
	{
		GD.Print("Intentando generar una nota de daño...");

		// Generar un índice aleatorio entre 0 y 3
		int randomIndex = (int)GD.Randi() % 4;

		// Verificar si el índice es válido
		if (randomIndex < 0 || randomIndex >= positions.Count)
		{
			GD.PrintErr("Error: Índice fuera de rango.");
			return;
		}

		// Usar el índice aleatorio para definir la posición de la nota de daño
		Vector2 spawnPosition = new Vector2(positions[randomIndex], 0);

		// Instanciar y añadir la nota de daño
		if (damageNoteScene != null)
		{
			DamageNote damageNote = (DamageNote)damageNoteScene.Instantiate();
			damageNote.GlobalPosition = spawnPosition;
			AddChild(damageNote);
			GD.Print("Nota de daño generada en el carril: " + randomIndex + " en la posición: " + spawnPosition);
		}
		else
		{
			GD.PrintErr("Error: La escena de DamageNote no está cargada.");
		}
	}
	public void TakeDamage(float damageAmount)
{
	// Reducir la vida del jugador
	playerHealth -= damageAmount;
	GD.Print("Daño recibido: " + damageAmount + ". Vida restante: " + playerHealth);
	if (playerHealth <= 0)
	{
		GD.Print("¡Game Over! Vida agotada.");
		// Lógica adicional si el jugador pierde toda la vida, como reiniciar el nivel
	}
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
		GD.Print("Nota generada en el carril: " + key);
	}
	else
	{
		GD.PrintErr("Error: No se pudo cargar la escena de KeyObject.");
	}
}

	private string LoadTextFromFile(string path, string fileName){
		string data = null;
		path = Path.Join(path,fileName);

		if(!File.Exists(path)) return null;

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


}
