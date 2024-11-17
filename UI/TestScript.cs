using Godot;
using System;

public partial class TestScript : Node
{
	[Export] public NodePath PlayerLife; // Ruta al nodo del jugador
	[Export] public int DamageAmount = 25; // Cantidad de daño infligido
	[Export] public int HealAmount = 25; // Cantidad de vida recuperada

	private PlayerLife _player;

	public override void _Ready()
	{
		// Obtiene una referencia al personaje usando la ruta proporcionada
		_player = GetNode<PlayerLife>(PlayerLife);

		if (_player == null)
		{
			GD.PrintErr("No se pudo encontrar al jugador. Asegúrate de configurar PlayerPath.");
		}
	}

	public override void _Process(double delta)
	{
		// Infligir daño al presionar la tecla "D"
		if (Input.IsActionJustPressed("ui_accept")) // Cambiar por "ui_accept" u otra acción
		{
			GD.Print("Infligiendo daño al jugador.");
			_player.TakeDamage(DamageAmount);
		}

		// Curar al presionar la tecla "H"
		if (Input.IsActionJustPressed("ui_cancel")) // Cambiar por "ui_cancel" u otra acción
		{
			GD.Print("Curando al jugador.");
			_player.Heal(HealAmount);
		}
	}
}
