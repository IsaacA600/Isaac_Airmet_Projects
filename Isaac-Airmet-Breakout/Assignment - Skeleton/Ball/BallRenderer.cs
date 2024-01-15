using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CS5410
{
    // Renders all the balls
    public class BallRenderer
    {
        private BallManager ballManager;
        public BallRenderer(BallManager ballManager)
        {
            this.ballManager = ballManager;
        }

        // Iterates over balls and renders them
        public void renderBalls(SpriteBatch spriteBatch, Texture2D ballTexture)
        {
            foreach (Ball ball in ballManager.getBalls())
            {
                Rectangle ballBox = new Rectangle((int)(ball.position.X - ball.radius), (int)(ball.position.Y - ball.radius), (int)(ball.radius * 2), (int)(ball.radius * 2));
                spriteBatch.Draw(ballTexture, ballBox, Color.White);
            }
        }
    }
}
