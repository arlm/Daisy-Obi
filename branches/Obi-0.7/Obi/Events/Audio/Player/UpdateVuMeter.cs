using System;

namespace Obi.Events.Audio.Player
{
	public delegate void UpdateVuMeterHandler(object sender, UpdateVuMeterEventArgs e);

	public class UpdateVuMeterEventArgs : EventArgs
	{
	}
}
