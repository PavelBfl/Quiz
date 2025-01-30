using System;

namespace Slip.View.Services;

internal class GameHandler(DrawerService drawer, ControlService control)
{
	public DrawerService Drawer { get; } = drawer ?? throw new ArgumentNullException(nameof(drawer));

	public ControlService Control { get; } = control ?? throw new ArgumentNullException(nameof(control));

	public void Update()
	{

	}

	public void Draw()
	{

	}
}
