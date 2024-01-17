using BigBlueIsYou.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BigBlueIsYou.Systems
{
  public interface ISystem
  {
    Dictionary<Guid, IEntity> Entities { get; set; }

    bool IsInterested(IEntity entity);

    bool Update(GameTime gameTime);

    void LoadContent(params object[] content);

    bool AddEntity(IEntity entity)
    {
      return IsInterested(entity) && Entities.TryAdd(entity.Id, entity);
    }

    bool RemoveEntity(Guid id)
    {
      return Entities.Remove(id);
    }

    void RemoveAllEntities()
    {
      Entities = new Dictionary<Guid, IEntity>();
    }
  }
}
