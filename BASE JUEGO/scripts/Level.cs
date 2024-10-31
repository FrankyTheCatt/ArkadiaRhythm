using Godot;
using System.Collections.Generic;

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

		// crear temporizadores para spawnear las motas en cada carril
		for (int i = 0; i < 4; i++)
		{
			Timer timer = new Timer();
			timer.WaitTime = (float)GD.RandRange(1.0, 3.0);  // da diferente ritmo para cada carril
			timer.OneShot = false;
			timer.Autostart = true;
			timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
			timers.Add(timer);
			AddChild(timer);
			
		}
	}

	// método que se llama cuando un temporizador se activa
	private void OnTimerTimeout()
	{
		int key = (int)(GD.Randi() % 4);  // escoge un carril al azar de los cuatro
		Vector2 pos = new Vector2(positions[key], 0);  // posición inicial (X) del carril

		// instanciar un nuevo KeyObject a partir de la plantilla
		if (keyObjectScene != null)
		{
			var newKeyObject = (KeyObject)keyObjectScene.Instantiate();
			AddChild(newKeyObject);

			// logica para inicializar el KeyObject en el carril correcto
			newKeyObject.Spawn(key, pos);  // Spawn de la nota en la posición del carril
			newKeyObject.SetSerialReader(serialReader);  // asignar el SerialReader al nuevo objeto para crear la nota
			newKeyObject.Visible = true;   // asegurarse de que sea visible la nota
			GD.Print("Nota generada en el carril: " + key);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de KeyObject.");
		}
	}
}
