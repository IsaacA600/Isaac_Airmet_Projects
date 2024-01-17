using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBlueIsYou.Components;

namespace BigBlueIsYou.Entities
{
  public class TextStopEntity : IEntity, IText
  {
    public List<IComponent> Components { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }

    public TextStopEntity(int productionNumber)
    {
      Id = Guid.NewGuid();
      Components = new List<IComponent>();
      Name = "TextStop" + productionNumber;
    }

    public TextStopEntity(List<IComponent> Components, string Name)
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
      return new TextStopEntity(ClonedComponents, Name);
    }
  }
}
