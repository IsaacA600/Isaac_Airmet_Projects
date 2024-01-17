using BigBlueIsYou.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Utils
{
  public class Rule
  {
    public NounType Beginning;
    public NounType Ending;

    public Rule(NounType beginning, NounType ending)
    {
      Beginning = beginning;
      Ending = ending;
    }
  }
}
