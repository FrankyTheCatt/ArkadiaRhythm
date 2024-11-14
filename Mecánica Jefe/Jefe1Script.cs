using Godot;
using System;

public partial class Jefe1 : Node2D
{
	[Export] public PackedScene CircleObjectScene; // Referencia a CircleObject.tscn
	private Timer noteSpawnTimer;
	private ProgressBar lifeBar;
	private int maxLife = 100;
	private int currentLife;

	public override void _Ready()
	{
		// Configurar la barra de vida
		currentLife = maxLife;
		lifeBar = new ProgressBar();
		lifeBar.MaxValue = maxLife;
		lifeBar.Value = currentLife;
		lifeBar.Size = new Vector2(200, 20);
		lifeBar.Position = new Vector2(10, 10);
		AddChild(lifeBar);

		// Configurar el temporizador para generar notas
		noteSpawnTimer = new Timer();
		noteSpawnTimer.WaitTime = 2.0f; // Intervalo de aparición de notas
		noteSpawnTimer.Autostart = true;
		noteSpawnTimer.OneShot = false;
		noteSpawnTimer.Connect("timeout", new Callable(this, nameof(OnNoteSpawnTimerTimeout)));
		AddChild(noteSpawnTimer);
	}

	private void OnNoteSpawnTimerTimeout()
	{
		if (CircleObjectScene != null)
		{
			// Instanciar una nota
			Area2D circle = (Area2D)CircleObjectScene.Instantiate();
			AddChild(circle);

			// Configurar la posición aleatoria
			float randomX = GD.Randf() * 400 + 100; // Ajustar según el tamaño de la escena
			float startY = -50; // Fuera de la pantalla en la parte superior
			circle.Position = new Vector2(randomX, startY);

			// Agregar lógica para mover la nota hacia abajo
			Tween tween = CreateTween();
			tween.TweenProperty(circle, "position:y", 600, 3.0f); // Ajusta la velocidad
		}
	}

	public void ReduceLife(int damage)
	{
		currentLife -= damage;
		lifeBar.Value = currentLife;

		if (currentLife <= 0)
		{
			GD.Print("Game Over");
			// Agregar lógica para manejar la derrota (por ejemplo, reiniciar el nivel)
		}
	}

	private void _OnNoteTouched()
	{
		// Este método se llama cuando una nota colisiona con un área específica
		ReduceLife(10); // Quita 10 puntos de vida al jugador
	}
}
