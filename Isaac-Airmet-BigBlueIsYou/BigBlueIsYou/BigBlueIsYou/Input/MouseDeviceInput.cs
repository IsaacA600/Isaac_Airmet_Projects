using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BigBlueIsYou
{
  public class MouseDeviceInput : IDeviceInput
  {
    private Action<GameTime, bool> MouseLeftButtonAction;
    private Action<GameTime, Point> MouseCursorAction;
    private MouseState PrevState;

    public MouseDeviceInput()
    {
      MouseLeftButtonAction = null;
      MouseCursorAction = null;
      PrevState = Mouse.GetState();
    }

    public void update(GameTime gameTime)
    {
      MouseState curState = Mouse.GetState();

      if (curState.LeftButton == ButtonState.Pressed)
      {
        MouseLeftButtonAction?.Invoke(gameTime, PrevState.LeftButton == ButtonState.Pressed);
      }

      MouseCursorAction?.Invoke(gameTime, curState.Position);

      PrevState = curState;
    }

    public void setLeftMouseButtonAction(Action<GameTime, bool> action)
    {
      MouseLeftButtonAction = action;
    }

    public void setMouseCursorAction(Action<GameTime, Point> action)
    {
      MouseCursorAction = action;
    }
  }
}
