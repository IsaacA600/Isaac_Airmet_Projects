using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBlueIsYou.Components
{
  public class PropertyComponent : IComponent
  {
    public Properties AppliedProperties;

    public PropertyComponent(params Properties[] props)
    {
      foreach (Properties prop in props)
      {
        AddProperty(prop);
      }
    }

    public PropertyComponent(Properties props)
    {
      AppliedProperties = props;
    }

    public void AddProperty(Properties prop)
    {
      if (!AppliedProperties.HasFlag(prop))
      {
        AppliedProperties |= prop;
      }
    }

    public void RemoveProperty(Properties prop)
    {
      if (AppliedProperties.HasFlag(prop))
      {
        AppliedProperties ^= prop;
      }
    }

    public bool HasProperty(Properties prop)
    {
      return AppliedProperties.HasFlag(prop);
    }

    public IComponent Clone()
    {
      return new PropertyComponent(AppliedProperties);
    }
  }
}
