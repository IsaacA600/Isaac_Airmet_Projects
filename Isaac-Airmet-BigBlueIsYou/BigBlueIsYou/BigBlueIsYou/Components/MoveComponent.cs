using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public enum Direction
  {
    Up,
    Down,
    Left,
    Right,
    Still,
  }
  public class MoveComponent : IComponent
  {
    public Direction direction;

    public MoveComponent(Direction direction)
    {
      this.direction = direction;
    }

    public IComponent Clone()
    {
      return new MoveComponent(direction);
    }
  }
}
