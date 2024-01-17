using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BigBlueIsYou.Entities;
using System.Collections.Generic;

namespace BigBlueIsYou
{
  public interface IDeviceInput
  {
    public void update(GameTime gameTime);
  }

  public class InputDeviceHelper
  {
    public delegate void CommandDelegate(GameTime gameTime);
    public delegate void EntityUpdateDelegate(GameTime gameTime, List<IEntity> entity);
    public delegate void ListenerDelegate(GameTime gameTime, Keys key);
  }
}
