using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Slip.View.Services;

public sealed class ControlService
{
	private KeyboardState PrevKeyboardState { get; set; }

	private MouseState PrevMouseState { get; set; }

	public bool IsKeyDown(Keys key) => Keyboard.GetState().IsKeyDown(key);

	public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !PrevKeyboardState.IsKeyDown(key);

	private bool LeftButtonOnPress => Mouse.GetState().LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton != ButtonState.Pressed;

	private bool RightButtonOnPress => Mouse.GetState().RightButton == ButtonState.Pressed && PrevMouseState.RightButton != ButtonState.Pressed;

	private Point GetMouseOffset() => Mouse.GetState().Position - PrevMouseState.Position;

	public void Update()
	{
		PrevKeyboardState = Keyboard.GetState();
		PrevMouseState = Mouse.GetState();
	}
}
