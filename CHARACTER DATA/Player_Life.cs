using Godot;
using System;

public class RhythmGame : Node
{
	private int maxHealth = 100;
	private int currentHealth;
	private HealthBar healthBar;

	public override void _Ready()
	{
		currentHealth = maxHealth;
		
		// Obtén la referencia al nodo de la barra de vida
		healthBar = GetNode<HealthBar>("Path/To/HealthBar");
		UpdateHealthUI();

		// Inicializamos el temporizador para la recuperación
		recoveryTimer = new Timer();
		recoveryTimer.WaitTime = 2.0f;  // Tiempo de espera para regenerar vida (ajusta según lo necesario)
		recoveryTimer.OneShot = false;
		recoveryTimer.Connect("timeout", this, nameof(OnRecoveryTimerTimeout));
		AddChild(recoveryTimer);
		recoveryTimer.Start();
	}

	// Método para manejar la pérdida de vida cuando el jugador falla
	public void OnNoteMissed()
	{
		failStreak += 1;
		currentHealth -= 25; // Reduce la vida (ajusta la cantidad según necesites)
		if (currentHealth <= 0)
		{
			GameOver();
		}

		// Llama al método para actualizar la interfaz de vida
		UpdateHealthUI();
		
	   // Reiniciamos el temporizador de recuperación cada vez que falle una nota
		recoveryTimer.Stop();
		recoveryTimer.Start();
	}

	// Método de recuperación de vida cuando el jugador no falla por un tiempo
	private void OnRecoveryTimerTimeout()
	{
		failStreak = 0; // Reinicia el contador de fallos al recuperar vida
		if (currentHealth < maxHealth)
		{
			currentHealth += 25;
			currentHealth = Mathf.Min(currentHealth, maxHealth);
			
			// Llama al método para actualizar la interfaz de vida
			UpdateHealthUI();
		}
	}

	private void UpdateHealthUI()
	{
		// Actualiza la barra de vida con los valores actuales de vida
		healthBar.UpdateHealthBar(currentHealth, maxHealth);
	}

	private void GameOver()
	{
		GD.Print("Game Over");
		GetTree().ReloadCurrentScene();
	}
}
