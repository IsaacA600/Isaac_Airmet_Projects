﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBlueIsYou.Components;

namespace BigBlueIsYou.Entities
{
  public class TextBabaEntity : IEntity, IText
  {
    public List<IComponent> Components { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }

    public TextBabaEntity(int productionNumber)
    {
      Id = Guid.NewGuid();
      Components = new List<IComponent>();
      Name = "TextBaba" + productionNumber;
    }

    public TextBabaEntity(List<IComponent> Components, string Name)
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
      return new TextBabaEntity(ClonedComponents, Name);
    }
  }
}