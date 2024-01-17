using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public class NounComponent : IComponent
  {
    public NounType NType;

    public NounComponent(NounType nt)
    {
      NType = nt;
    }

    public IComponent Clone()
    {
      return new NounComponent(NType);
    }
  }
}
