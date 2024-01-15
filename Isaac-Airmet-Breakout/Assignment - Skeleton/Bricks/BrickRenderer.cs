using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CS5410
{
    // Renders all the bricks
    public class BrickRenderer
    {
        private BrickManager brickManager;
        public BrickRenderer(BrickManager brickManager)
        {
            this.brickManager = brickManager;
        }

        // Renders all bricks with the correct texture
        public void renderBricks(SpriteBatch spriteBatch, Texture2D yellowBrickTexture, Texture2D blueBrickTexture, Texture2D greenBrickTexture, Texture2D orangeBrickTexture)
        {
            Texture2D currentTexture;
            for (int i = 0; i < 8; i++)
            {
                if (i >= 6)
                {
                    currentTexture = yellowBrickTexture;
                } else if (i >= 4)
                {
                    currentTexture = orangeBrickTexture;
                } else if (i >= 2)
                {
                    currentTexture = blueBrickTexture;
                } else 
                {
                    currentTexture = greenBrickTexture;
                }
                for (int j = 0; j < 14; j++)
                {
                    if (brickManager.getBricks()[i, j] != null)
                    {
                        Brick brick = brickManager.getBricks()[i, j];
                        Rectangle brickBox = new Rectangle((int)(brick.center.X - (brick.width / 2)), (int)(brick.center.Y - (brick.height / 2)), (int)brick.width, (int)brick.height);
                        spriteBatch.Draw(currentTexture, brickBox, Color.White);
                    }
                }
            }
        }
    }
}
