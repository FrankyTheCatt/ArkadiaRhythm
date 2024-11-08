using Godot;

public class HealthBar : Control
{
	private ProgressBar healthBar;

	public override void _Ready()
	{
		healthBar = GetNode<ProgressBar>("ProgressBar"); // Asegúrate de que el nombre coincide
	}

	// Método para actualizar el valor de la barra de vida
	public void UpdateHealthBar(int currentHealth, int maxHealth)
	{
		if (healthBar != null)
		{
			healthBar.Value = (float)currentHealth / maxHealth * 100; // Convertimos a porcentaje
		}
	}
}
