using Godot;
using System.Collections.Generic;

public partial class Nivel1 : Level
{
	private float playerHealth = 100f; // Vida del jugador, puede ser modificada según sea necesario.
	private PackedScene damageNoteScene;

	public override void _Ready()
{
	GD.Print("Estructura del árbol de nodos: ");
	GetTree().Root.PrintTreePretty();  // Verifica la estructura del árbol de nodos

	// Acceder a los nodos dentro de "Level"
	var levelNode = GetNode<Node>("Level");
	
	// Verificar si el nodo 'Level' se encuentra correctamente
	if (levelNode == null)
	{
		GD.PrintErr("Error: No se encuentra el nodo 'Level' en la escena.");
		return;
	}
	
	// Ahora, accedemos a los nodos de "Objects" dentro de "Level"
	positions.Add(levelNode.GetNode<Node2D>("Objects/ColorObject4").GlobalPosition.X); // Rojo
	positions.Add(levelNode.GetNode<Node2D>("Objects/ColorObject3").GlobalPosition.X); // Amarillo
	positions.Add(levelNode.GetNode<Node2D>("Objects/ColorObject2").GlobalPosition.X); // Azul
	positions.Add(levelNode.GetNode<Node2D>("Objects/ColorObject").GlobalPosition.X);  // Verde

	GetNodeOrNull<Rojo>("Objects/ColorObject4/Rojo")?.SetSerialReader(serialReader);
	GetNodeOrNull<Verde>("Objects/ColorObject/Verde")?.SetSerialReader(serialReader);
	GetNodeOrNull<Azul>("Objects/ColorObject2/Azul")?.SetSerialReader(serialReader);
	GetNodeOrNull<Amarillo>("Objects/ColorObject3/Amarillo")?.SetSerialReader(serialReader);

// Crear temporizadores para los carriles
	for (int i = 0; i < 4; i++)
	{
		Timer timer = new Timer();
		timer.WaitTime = (float)GD.RandRange(1.0, 3.0);  // Da ritmo aleatorio para cada carril
		timer.OneShot = false;
		timer.Autostart = true;
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		timers.Add(timer);
		AddChild(timer);
	}

	// Si la lista tiene menos de 4 elementos, lanza un error o haz un manejo adecuado.
	if (positions.Count < 4)
	{
		GD.PrintErr("Error: La lista 'positions' no tiene suficientes elementos.");
		return;
	}

		base._Ready();

		// Carga la escena de la nota de daño.
		damageNoteScene = (PackedScene)ResourceLoader.Load("res://Niveles/Nivel1/DamageNote.tscn");

		// Empieza un temporizador para generar notas de daño.
		Timer damageNoteTimer = new Timer();
		damageNoteTimer.WaitTime = (float)GD.RandRange(2.0, 5.0); // Intervalo aleatorio para las notas de daño.
		damageNoteTimer.OneShot = false;
		damageNoteTimer.Autostart = true;
		damageNoteTimer.Connect("timeout", new Callable(this, nameof(SpawnDamageNote)));
		AddChild(damageNoteTimer);
	}

	private void SpawnDamageNote()
	{
		// Genera un índice aleatorio entre 0 y 3
		int randomIndex = (int)GD.Randi() % 4;
		// Verifica si el índice es válido
		if (randomIndex < 0 || randomIndex >= positions.Count)
	{
		GD.PrintErr("Error: Índice fuera de rango.");
		return;
	}
		// Usa el índice aleatorio para acceder a la posición
		Vector2 spawnPosition = new Vector2(positions[randomIndex], 0);
		// Instancia una nueva nota de daño.
		if (damageNoteScene != null)
		{
			var damageNote = (DamageNote)damageNoteScene.Instantiate();
			damageNote.GlobalPosition = spawnPosition;
			damageNote.Connect("DamageDealt", new Callable(this, nameof(TakeDamage)));
			AddChild(damageNote);
			GD.Print("Nota de daño generada en el carril: " + randomIndex);
		}
		else
		{
			GD.PrintErr("Error: No se pudo cargar la escena de DamageNote.");
		}
	}

	public void TakeDamage(float damageAmount)
	{
		// Reduce la vida del jugador.
		playerHealth -= damageAmount;
		GD.Print("Daño recibido: " + damageAmount + ". Vida restante: " + playerHealth);

		if (playerHealth <= 0)
		{
			GD.Print("¡Game Over! Vida agotada.");
			// Lógica adicional si el jugador pierde toda la vida, como reiniciar el nivel.
		}
	}
}
