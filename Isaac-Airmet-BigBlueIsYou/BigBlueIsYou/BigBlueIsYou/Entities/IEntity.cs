using BigBlueIsYou.Components;
using System;
using System.Collections.Generic;

namespace BigBlueIsYou.Entities
{
  public interface IEntity
  {
    public List<IComponent> Components { get; }
    public Guid Id { get; set; }
    public string Name { get; }

    public void AddComponent(IComponent component)
    {
      Components.Add(component);
    }

    public bool RemoveComponent<TComponent>() where TComponent : IComponent
    {
      return Components.Remove(Components.Find(c => c.GetType() == typeof(TComponent)));
    }

    public bool HasComponent<TComponent>() where TComponent : IComponent
    {
      return Components.Exists(c => c.GetType() == typeof(TComponent));
    }

    //public bool HasComponent(Type type)
    //{
    //  return Components.Exists(c => c.GetType() == type);
    //}

    public TComponent GetComponent<TComponent>() where TComponent : IComponent
    {
      return (TComponent)Components.Find(c => c.GetType() == typeof(TComponent));
    }

    public void ClearComponents()
    {
      Components.Clear();
    }

    public IEntity Clone();
  }
}
