using Godot;
using System.Collections.Generic;
using System.IO;
using Godot.Collections; // referencia al espacio de nombres correcto para usar Dictionary


public partial class Level : Node2D
{
	private List<float> positions = new List<float>(); // posiciones X de cada carril
	private List<Timer> timers = new List<Timer>(); // temporizadores para cada carril
	private PackedScene keyObjectScene;  // referencia a la plantilla del KeyObject
	private SerialReader serialReader;
	
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
