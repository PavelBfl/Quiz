namespace Slip.View.Services
{
	internal static class CoordinatesExtensions
	{
		public static System.Drawing.Point ToDrawing(this Microsoft.Xna.Framework.Point value) => new(value.X, value.Y);
	}
}
