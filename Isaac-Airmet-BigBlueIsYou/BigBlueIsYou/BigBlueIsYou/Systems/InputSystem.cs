using BigBlueIsYou.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBlueIsYou.Components;

namespace BigBlueIsYou.Systems
{
  public class InputSystem : ISystem
  {
    public KeyboardInput keyboardInput;

    public Dictionary<Guid, IEntity> Entities { get; set; }

    public InputSystem(KeyboardInput keyboardInput)
    { 
      Entities = new Dictionary<Guid, IEntity>();
      this.keyboardInput = keyboardInput;
    }

    public bool Update(GameTime gameTime)
    {
      List<IEntity> eList = new List<IEntity>();
      foreach (IEntity entity in Entities.Values)
      {
        eList.Add(entity);
      }

      keyboardInput.update(gameTime);
      keyboardInput.update(gameTime, eList);

      return true;
    }

    public bool IsInterested(IEntity entity)
    {
      if (!entity.HasComponent<InputComponent>())
      {
        return false;
      }

      return true;
    }

    public void LoadContent(params object[] content)
    {
      // Empty
    }
  }
}
