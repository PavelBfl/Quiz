using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slip.View.Services;

namespace Slip.View;

public class Game1 : Game
{
	private GraphicsDeviceManager Graphics { get; }

	private GameRunner? Runner { get; set; }

	public Game1()
	{
		Graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void LoadContent()
	{
		var spriteBatch = new SpriteBatch(GraphicsDevice);
		var control = new ControlService();
		var drawer = new DrawerService(spriteBatch, GraphicsDevice, Content);
		Runner = new(
			spriteBatch,
			control,
			new(drawer, control)
		);

		base.LoadContent();
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
		{
			Exit();
		}

		Runner?.Update();
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);

		Runner?.Draw();
	}

	private sealed class GameRunner(SpriteBatch spriteBatch, ControlService controlService, GameHandler gameHandler)
	{
		public SpriteBatch SpriteBatch { get; } = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));

		public ControlService ControlService { get; } = controlService ?? throw new ArgumentNullException(nameof(controlService));

		public GameHandler GameHandler { get; } = gameHandler ?? throw new ArgumentNullException(nameof(gameHandler));

		public void Update()
		{
			GameHandler.Update();
			ControlService.Update();
		}

		public void Draw()
		{
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			GameHandler.Draw();

			SpriteBatch.End();
		}
	}
}