using Godot;

public partial class MusicVolumeSlider : HSlider
{
	private const string MusicBusName = "Music"; // Cambia esto al nombre de tu bus

	public override void _Ready()
	{
		// Configura el valor inicial del slider según el volumen del bus
		int busIndex = AudioServer.GetBusIndex(MusicBusName);
		Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
		
		// Conecta el evento del slider
		ValueChanged += OnValueChanged;
	}

	private void OnValueChanged(double value)
	{
		// Ajusta el volumen del bus según el valor del slider
		int busIndex = AudioServer.GetBusIndex(MusicBusName);
		AudioServer.SetBusVolumeDb(busIndex, Mathf.LinearToDb((float)value));
	}
}
