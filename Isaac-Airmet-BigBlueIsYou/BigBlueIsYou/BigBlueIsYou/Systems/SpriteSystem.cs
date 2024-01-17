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
  public class SpriteSystem : ISystem
  {
    public SpriteSystem()
    {
      Entities = new Dictionary<Guid, IEntity>();
    }

    public Dictionary<Guid, IEntity> Entities { get; set; }

    public bool IsInterested(IEntity entity)
    {
      if (!entity.HasComponent<SpriteComponent>())
      {
        return false;
      }

      if (!entity.HasComponent<PositionComponent>())
      {
        return false;
      }

      return true;
    }

    public void LoadContent(params object[] content)
    {
      // Empty
    }

    public bool Update(GameTime gameTime)
    {
        foreach (IEntity entity in Entities.Values)
        {
          SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();
          PositionComponent positionComponent = entity.GetComponent<PositionComponent>();
          spriteComponent.sprite.update(gameTime, positionComponent.Position);
        }

        return true;
    }
  }
}
