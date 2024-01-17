using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  [Flags]
  public enum Properties
  {
    None = 0,
    Stop = 1,
    Push = 2,
    You = 4,
    Win = 8,
    Sink = 16,
    Kill = 32
  }
}
