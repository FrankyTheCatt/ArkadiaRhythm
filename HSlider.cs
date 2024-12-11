using Godot;

public partial class VolumeSlider : HSlider
{
	[Export] public string BusName { get; set; }

	private int _busIndex;

	public override void _Ready()
	{
		_busIndex = AudioServer.GetBusIndex(BusName);
		ValueChanged += OnValueChanged;

		// Si estás usando Godot 3, reemplaza DbToLinear() con Db2Linear().
		Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_busIndex));
	}

	private void OnValueChanged(double value)
	{
		// Si estás usando Godot 3, reemplaza LinearToDb() con Linear2Db().
		AudioServer.SetBusVolumeDb(_busIndex, Mathf.LinearToDb((float)value));
	}
}
