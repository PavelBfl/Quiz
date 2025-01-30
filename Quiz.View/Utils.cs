using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Slip.View
{
	public static class Utils
	{
		public static float ToAngle(Vector2 vector) => MathF.Atan2(vector.Y, vector.X);

		public static IEnumerable<Vector2> GetRegularPolygon(float radius, int verticesCount, float offset)
		{
			if (verticesCount < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(verticesCount));
			}

			for (var i = 0; i < verticesCount; i++)
			{
				var angleStep = MathF.Tau / verticesCount * i + offset;

				var x = MathF.Cos(angleStep) * radius;
				var y = MathF.Sin(angleStep) * radius;
				yield return new Vector2(x, y);
			}
		}

		public static bool Contains(IReadOnlyList<Vector2> vertices, Vector2 point)
		{
			var result = false;
			var prevIndex = vertices.Count - 1;
			for (var i = 0; i < vertices.Count; i++)
			{
				var prevPoint = vertices[prevIndex];
				var currentPoint = vertices[i];
				if (currentPoint.Y < point.Y && prevPoint.Y >= point.Y || prevPoint.Y < point.Y && currentPoint.Y >= point.Y)
				{
					if (currentPoint.X + (point.Y - currentPoint.Y) / (prevPoint.Y - currentPoint.Y) * (prevPoint.X - currentPoint.X) < point.X)
					{
						result = !result;
					}
				}
				prevIndex = i;
			}
			return result;
		}
	}
}
