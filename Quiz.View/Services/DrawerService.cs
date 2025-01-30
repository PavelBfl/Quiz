using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Slip.View.Services;

internal sealed class DrawerService
{
	private const string DEFAULT_FONT_KEY = "DefaultFont";

	private static Color ToMono(System.Drawing.Color color) => new(color.R, color.G, color.B, color.A);

	private static Vector2 ToMono(System.Drawing.PointF point) => new(point.X, point.Y);

	private static Rectangle ToMono(System.Drawing.Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

	public DrawerService(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager content)
	{
		ArgumentNullException.ThrowIfNull(spriteBatch);
		ArgumentNullException.ThrowIfNull(graphicsDevice);
		ArgumentNullException.ThrowIfNull(content);

		SpriteBatch = spriteBatch;
		GraphicsDevice = graphicsDevice;
		Content = content;

		DefaultFont = content.Load<SpriteFont>(DEFAULT_FONT_KEY);

		Pixel = new(graphicsDevice, 1, 1);
		Pixel.SetData([Color.White]);
	}

	private SpriteFont DefaultFont { get; }

	private Texture2D Pixel { get; }

	private Dictionary<string, Texture2D> Textures { get; } = [];

	private Texture2D GetTexture(string name)
	{
		if (!Textures.TryGetValue(name, out var result))
		{
			result = Content.Load<Texture2D>(name);
			Textures.Add(name, result);
		}

		return result;
	}

	public SpriteBatch SpriteBatch { get; }

	public GraphicsDevice GraphicsDevice { get; }

	public ContentManager Content { get; }

	public void Line(System.Numerics.Vector2 start, System.Numerics.Vector2 end, System.Drawing.Color color, float thickness = 2f)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(thickness);

		var delta = end - start;
		SpriteBatch.Draw(
			Pixel,
			new(start.X, start.Y),
			null,
			ToMono(color),
			Utils.ToAngle(delta),
			new Vector2(0, 0.5f),
			new Vector2(delta.Length(), thickness),
			SpriteEffects.None,
			0f
		);
	}

	public void Polygon(IReadOnlyList<System.Numerics.Vector2> vertices, System.Drawing.Color color, float thickness = 2f)
	{
		ArgumentNullException.ThrowIfNull(vertices);
		ArgumentOutOfRangeException.ThrowIfNegative(thickness);

		var prevIndex = vertices.Count - 1;
		for (var i = 0; i < vertices.Count; i++)
		{
			Line(vertices[prevIndex], vertices[i], color, thickness);
			prevIndex = i;
		}
	}

	public void DrawString(string text, System.Numerics.Vector2 position, System.Drawing.Color color)
		=> SpriteBatch.DrawString(
			DefaultFont,
			text,
			position,
			ToMono(color)
		);

	public Vector2 MeasureString(string text) => DefaultFont.MeasureString(text);

	public void Rectangle(System.Drawing.RectangleF rectangle, System.Drawing.Color color)
	{
		SpriteBatch.Draw(
			Pixel,
			new Vector2(rectangle.X, rectangle.Y),
			null,
			ToMono(color),
			0,
			Vector2.Zero,
			new Vector2(rectangle.Width, rectangle.Height),
			SpriteEffects.None,
			0f
		);
	}
}
