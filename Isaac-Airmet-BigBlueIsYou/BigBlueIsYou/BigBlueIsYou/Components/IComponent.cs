using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public interface IComponent
  {
    public abstract IComponent Clone();
  }
}
