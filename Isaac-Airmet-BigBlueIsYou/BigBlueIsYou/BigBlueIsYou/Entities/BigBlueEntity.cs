using BigBlueIsYou.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Entities
{
  public class BigBlueEntity : IEntity
  {
    public List<IComponent> Components { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }

    public BigBlueEntity(int productionNumber)
    {
      Id = Guid.NewGuid();
      Components = new List<IComponent>();
      Name = "BigBlue" + productionNumber;
    }
    public BigBlueEntity(List<IComponent> Components, string Name)
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
      return new BigBlueEntity(ClonedComponents, Name);
    }
  }
}
