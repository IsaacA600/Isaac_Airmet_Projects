using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public class TextComponent : IComponent
  {
    public TextType TType;

    public bool IsPartOfHorizontalRule;

    public bool IsPartOfVerticalRule;

    public TextComponent(TextType type, bool hEnd = false, bool vEnd = false)
    {
      TType = type;
      IsPartOfHorizontalRule = hEnd;
      IsPartOfVerticalRule = vEnd;
    }

    public IComponent Clone()
    {
      return new TextComponent(TType, IsPartOfHorizontalRule, IsPartOfVerticalRule);
    }
  }
}
