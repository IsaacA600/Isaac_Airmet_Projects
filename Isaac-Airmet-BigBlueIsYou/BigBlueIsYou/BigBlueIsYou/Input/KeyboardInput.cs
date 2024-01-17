using BigBlueIsYou.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

// Adapted from Professor Dean
namespace BigBlueIsYou
{
  public class KeyboardInput : IDeviceInput
  {
    private KeyboardState m_statePrevious;

    public void registerCommand(Keys key, bool keyPressOnly, InputDeviceHelper.CommandDelegate callback)
    {
      if (m_commandEntries.ContainsKey(key))
      {
        m_commandEntries.Remove(key);
      }
      m_commandEntries.Add(key, new CommandEntry(key, keyPressOnly, callback));
    }

    public void registerEntityBasedCommand(Keys key, bool keyPressOnly, InputDeviceHelper.EntityUpdateDelegate callback)
    {
      if (m_commandEntityEntries.ContainsKey(key))
      {
        m_commandEntityEntries.Remove(key);
      }
      m_commandEntityEntries.Add(key, new CommandEntityEntry(key, keyPressOnly, callback));
    }

    public void listen(GameTime gameTime, InputDeviceHelper.ListenerDelegate callback)
    {
      KeyboardState state = Keyboard.GetState();
      foreach (Keys key in state.GetPressedKeys())
      {
        callback(gameTime, key);
      }
    }

    private Dictionary<Keys, CommandEntry> m_commandEntries = new Dictionary<Keys, CommandEntry>();
    private Dictionary<Keys, CommandEntityEntry> m_commandEntityEntries = new Dictionary<Keys, CommandEntityEntry>();

    private struct CommandEntry
    {
      public CommandEntry(Keys key, bool keyPressOnly, InputDeviceHelper.CommandDelegate callback)
      {
        this.key = key;
        this.keyPressOnly = keyPressOnly;
        this.callback = callback;
      }

      public Keys key;
      public bool keyPressOnly;
      public InputDeviceHelper.CommandDelegate callback;
    }

    private struct CommandEntityEntry
    {
      public CommandEntityEntry(Keys key, bool keyPressOnly, InputDeviceHelper.EntityUpdateDelegate callback)
      {
        this.key = key;
        this.keyPressOnly = keyPressOnly;
        this.callback = callback;
      }

      public Keys key;
      public bool keyPressOnly;
      public InputDeviceHelper.EntityUpdateDelegate callback;
    }

    public void update(GameTime gameTime)
    {
      KeyboardState state = Keyboard.GetState();
      foreach (CommandEntry entry in this.m_commandEntries.Values)
      {
        if (entry.keyPressOnly && keyPressed(entry.key))
        {
          entry.callback(gameTime);
        }
        else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
        {
          entry.callback(gameTime);
        }
      }

      m_statePrevious = state;
    }

    public void update(GameTime gameTime, List<IEntity> entites)
    {
      KeyboardState state = Keyboard.GetState();
      foreach (CommandEntityEntry entry in this.m_commandEntityEntries.Values)
      {
        if (entry.keyPressOnly && keyPressed(entry.key))
        {
          entry.callback(gameTime, entites);

          return;
        }
        else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
        {
          entry.callback(gameTime, entites);

          return;
        }
      }

      m_statePrevious = state;
    }

    private bool keyPressed(Keys key)
    {
      return (Keyboard.GetState().IsKeyUp(key) && m_statePrevious.IsKeyDown(key));
    }
  }
}
