using Godot;
using System;

public partial class DamageNote : Area2D
{
	private const int Gravedad = 500;  // Velocidad de caída
	private bool estaDentro = false;

	public override void _Ready()
	{
		// Conectar las señales para las colisiones de área
		Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
		Connect("area_exited", new Callable(this, nameof(OnAreaExited)));
	}

	public override void _Process(double delta)
	{
		// Mueve la nota hacia abajo
		Position += new Vector2(0, (float)(Gravedad * delta));

		// Elimina la nota si sale de la pantalla
		if (Position.Y > GetViewportRect().Size.Y)
		{
			QueueFree();  // Elimina la nota
		}
	}

	public void Spawn(Vector2 pos)
	{
		Position = pos;  // Ajusta la posición inicial de la nota
		Visible = true;  // Haz visible la nota
	}

	private void OnAreaEntered(Area2D area)
	{
		// Verifica si el área con la que colisionó es del grupo "Player"
		if (area.IsInGroup("Player"))  // Cambia "Player" si necesitas un grupo distinto
		{
			GD.Print("Nota de daño colisionó con el área del jugador.");
			// Lógica adicional como aplicar daño al jugador
			Node2D levelNode = GetTree().Root.GetNode<Node2D>("Niveles/Nivel1/Nivel1"); // Ajusta la ruta si es necesario
			if (levelNode != null && levelNode is Nivel1 nivel1Script)
			{
				nivel1Script.TakeDamage(10);  // Aplica 10 de daño o el valor que desees
				QueueFree();  // Destruye la nota después de hacer daño
			}
		}
	}

	private void OnAreaExited(Area2D area)
	{
		estaDentro = false;
		GD.Print("Nota de daño salió del área de colisión.");
	}
}
