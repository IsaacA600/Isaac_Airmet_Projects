using BigBlueIsYou.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBlueIsYou.Components;
using System.ComponentModel.DataAnnotations;

namespace BigBlueIsYou.Systems
{
  public class RenderSpritesSystem : ISystem
  {
    SpriteBatch spriteBatch;
    public RenderSpritesSystem(SpriteBatch spriteBatch)
    {
      Entities = new Dictionary<Guid, IEntity>();
      this.spriteBatch = spriteBatch;
    }

    public Dictionary<Guid, IEntity> Entities { get; set; }

    public bool IsInterested(IEntity entity)
    {
      if (!entity.HasComponent<SpriteComponent>())
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
        List<IEntity> highLevelEntities = new List<IEntity>();
        List<IEntity> lowLevelEntities = new List<IEntity>();
        foreach (IEntity entity in Entities.Values)
        {
          if (entity is IText || entity.HasComponent<InputComponent>())
          {
            highLevelEntities.Add(entity);
          }
          else
          {
            lowLevelEntities.Add(entity);
          }
        }

        spriteBatch.Begin();

        foreach (IEntity entity in lowLevelEntities)
        {
          SpriteComponent comp = entity.GetComponent<SpriteComponent>();
          comp.sprite.draw(spriteBatch);
        }
        foreach (IEntity entity in highLevelEntities)
        {
          SpriteComponent comp = entity.GetComponent<SpriteComponent>();
          comp.sprite.draw(spriteBatch);
        }

        spriteBatch.End();

        return true;
    }
  }
}
