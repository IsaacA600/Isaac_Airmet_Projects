using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Adapted from Professor Dean
namespace CS5410
{
    public abstract class GameStateView : IGameState
    {
        public abstract void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics);
        public abstract void loadContent(ContentManager contentManager);
        public abstract GameStateEnum processInput(GameTime gameTime);
        public abstract void render(GameTime gameTime);
        public abstract void update(GameTime gameTime);
    }
}
