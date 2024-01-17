using BigBlueIsYou.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Entities
{
  public class GrassEntity : IEntity
  {
    public List<IComponent> Components { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }

    public GrassEntity(int productionNumber)
    {
      Id = Guid.NewGuid();
      Components = new List<IComponent>();
      Name = "Grass" + productionNumber;
    }
    public GrassEntity(List<IComponent> Components, string Name)
    {
      Id = Guid.NewGuid();
      this.Components = Components;
      this.Name = Name;
    }

    public IEntity Clone()
    {
      List<IComponent> ClonedComponents = new List<IComponent>();
      foreach (IComponent component in Components)
      {
        ClonedComponents.Add(component.Clone());
      }
      return new GrassEntity(ClonedComponents, Name);
    }
  }
}
