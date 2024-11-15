using Godot;

public class TabBarScript : TabBar
{
	// Método llamado cuando se activa o desactiva el modo de pantalla completa
	private void OnFullScreenToggled(bool toggledOn)
	{
		if (toggledOn)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
	}

	// Método llamado cuando se activa o desactiva el modo sin bordes
	private void OnBorderlessToggled(bool toggledOn)
	{
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, toggledOn);
	}

	// Método llamado cuando se selecciona una opción de sincronización vertical (VSync)
	private void OnVsyncItemSelected(int index)
	{
		DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)index);
	}
}
