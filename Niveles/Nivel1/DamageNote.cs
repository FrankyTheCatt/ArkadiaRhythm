using Godot;

public partial class DamageNote : Area2D
{
	public override void _Ready()
	{
		Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
	}

	private void OnAreaEntered(Area2D area)
	{
		// Verificar si el área con la que colisionó es la esperada (por ejemplo, un carril específico)
		if (area.IsInGroup("Player"))  // Cambia "Player" si necesitas un grupo distinto
		{
			// Llama a la función TakeDamage en el nodo de nivel
			Node2D levelNode = GetTree().Root.GetNode<Node2D>("res://BASE JUEGO/scripts/Level.cs"); // Ajusta la ruta si es necesario
			if (levelNode != null && levelNode is Level levelScript)
			{
				levelScript.TakeDamage(10); // Aplica 10 de daño o el valor que desees
			}
				QueueFree(); // Destruye la nota después de hacer daño
		}
	}
}
