using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigBlueIsYou;

namespace BigBlueIsYou.Components
{
  public class InputComponent : IComponent
  {
    public InputComponent() { }

    public IComponent Clone()
    {
      return new InputComponent();
    }
  }
}
