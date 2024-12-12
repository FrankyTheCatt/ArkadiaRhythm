using Godot;
using System;

public partial class PlayerLife : Node2D // O Node2D dependiendo de tu configuración
{
	[Export] public int MaxHealth = 100; // Vida máxima
	public int CurrentHealth; // Vida actual

	[Export] public int RegenAmount = 5; // Cantidad de vida regenerada
	[Export] public float RegenInterval = 1.0f; // Intervalo entre regeneraciones (en segundos)
	[Export] public float RegenDelay = 5.0f; // Tiempo de espera antes de iniciar la regeneración

	private Timer _regenTimer; // Temporizador para regeneración

	public override void _Ready()
	{
		// Inicializa la vida actual y configura el temporizador
		CurrentHealth = MaxHealth;

		_regenTimer = GetNode<Timer>("Timer"); // Asegúrate de tener un nodo Timer como hijo
		_regenTimer.WaitTime = RegenInterval;
	}

	// Método para recibir daño
	public void TakeDamage(int amount)
	{
		CurrentHealth -= amount;
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			Die();
		}

		UpdateHealthBar();

		// Reinicia el temporizador de regeneración
		_regenTimer.Stop();
		_regenTimer.Start(RegenDelay);
	}

	// Método para ganar vida
	public void Heal(int amount)
	{
		CurrentHealth += amount;
		if (CurrentHealth > MaxHealth)
			CurrentHealth = MaxHealth;

		UpdateHealthBar();
	}

	// Método para manejar la muerte del personaje
	public void Die()
	{
		GD.Print("El personaje ha muerto.");

		// Cargar la nueva escena
		string newScenePath = "res://MENU/Defeat_leds.tscn";
		PackedScene newScene = ResourceLoader.Load<PackedScene>(newScenePath);

		if (newScene != null)
		{
			// Cambiar la escena actual
			GetTree().Root.GetChild(0).QueueFree(); // Libera la escena actual
			GetTree().Root.AddChild(newScene.Instantiate()); // Añade la nueva escena
		}
		else
		{
			GD.PrintErr("No se pudo cargar la escena: " + newScenePath);
		}
	}

	// Método para actualizar la barra de vida
	public void UpdateHealthBar()
	{
		var progressBar = GetNode<ProgressBar>("ProgressBar");
		if (progressBar != null)
		{
			progressBar.Value = CurrentHealth;
		}
	}

	// Método llamado por el temporizador para regenerar vida
	private void OnTimerTimeout()
	{
		if (CurrentHealth < MaxHealth)
		{
			Heal(RegenAmount);

			// Vuelve a iniciar el temporizador si aún no está al máximo
			if (CurrentHealth < MaxHealth)
				_regenTimer.Start();
		}
	}
}
