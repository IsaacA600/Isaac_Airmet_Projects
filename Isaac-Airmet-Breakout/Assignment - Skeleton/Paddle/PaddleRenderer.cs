using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CS5410
{
    // Render the paddle in use
    public class PaddleRenderer
    {
        private PaddleManager paddleManager;
        public PaddleRenderer(PaddleManager paddleManager)
        {
            this.paddleManager = paddleManager;
        }

        // Renders the current paddle if there are any
        public void renderPaddle(SpriteBatch spriteBatch, Texture2D paddleTexture)
        {
            Paddle currentPaddle = paddleManager.getAllPaddles()[0];
            if (currentPaddle == null) return;
            Rectangle paddleBox = new Rectangle((int)(currentPaddle.center.X - (currentPaddle.width / 2)), (int)(currentPaddle.center.Y - (currentPaddle.height / 2)), (int)currentPaddle.width, (int)currentPaddle.height);
            spriteBatch.Draw(paddleTexture, paddleBox, Color.White);
        }
    }
}
