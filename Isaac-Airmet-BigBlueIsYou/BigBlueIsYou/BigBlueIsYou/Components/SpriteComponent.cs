using BigBlueIsYou;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public class SpriteComponent : IComponent
  {
    public AnimatedSprite sprite;

    public SpriteComponent(AnimatedSprite sprite)
    {
      this.sprite = sprite;
    }

    public IComponent Clone()
    {
      return new SpriteComponent(sprite);
    }
  }
}
