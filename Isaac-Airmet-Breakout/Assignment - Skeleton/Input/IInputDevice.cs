using Microsoft.Xna.Framework;

// Taken From Professor Dean
namespace CS5410
{
    /// <summary>
    /// Abstract base class that defines how input is presented to game code.
    /// </summary>
    public interface IInputDevice
    {
        void Update(GameTime gameTime);
    }

    public class InputDeviceHelper
    {
        public delegate void CommandDelegate(GameTime gameTime);
    }
}
