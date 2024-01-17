using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public class PositionComponent : IComponent
  {
    public Point Position;

    public PositionComponent()
    {
      Position = Point.Zero;
    }

    public PositionComponent(Point location)
    {
      Position = location;
    }

    public IComponent Clone()
    {
      return new PositionComponent(new Point(Position.X, Position.Y));
    }
  }
}
