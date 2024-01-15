using Microsoft.Xna.Framework;

namespace CS5410
{
    // Manages all the bricks
    public class BrickManager
    {
        private Brick[,] bricks = new Brick[8, 14];
        public BrickManager() { }

        // Creates all the inital bricks in the right places
        public void createBricks()
        {
            for (int row = 0; row < 2; row++)
            {
                for (int i = 0; i < 14; i++) 
                {
                    float x = (float)(((i + 1) * 2) + (i * 135) + 67.5);
                    float y = (float)(200 + (row * 2) + (row * 33) + 16.5);
                    Vector2 brickCenter = new Vector2(x, y);
                    bricks[row, i] = new Brick(brickCenter, 135, 33, BrickColorEnum.Green, new Vector2(row, i));
                }
            }

            for (int row = 2; row < 4; row++)
            {
                for (int i = 0; i < 14; i++) 
                {
                    float x = (float)(((i + 1) * 2) + (i * 135) + 67.5);
                    float y = (float)(200 + (row * 2) + (row * 33) + 16.5);
                    Vector2 brickCenter = new Vector2(x, y);
                    bricks[row, i] = new Brick(brickCenter, 135, 33, BrickColorEnum.Blue, new Vector2(row, i));
                }
            }

            for (int row = 4; row < 6; row++)
            {
                for (int i = 0; i < 14; i++) 
                {
                    float x = (float)(((i + 1) * 2) + (i * 135) + 67.5);
                    float y = (float)(200 + (row * 2) + (row * 33) + 16.5);
                    Vector2 brickCenter = new Vector2(x, y);
                    bricks[row, i] = new Brick(brickCenter, 135, 33, BrickColorEnum.Orange, new Vector2(row, i));
                }
            }

            for (int row = 6; row < 8; row++)
            {
                for (int i = 0; i < 14; i++) 
                {
                    float x = (float)(((i + 1) * 2) + (i * 135) + 67.5);
                    float y = (float)(200 + (row * 2) + (row * 33) + 16.5);
                    Vector2 brickCenter = new Vector2(x, y);
                    bricks[row, i] = new Brick(brickCenter, 135, 33, BrickColorEnum.Yellow, new Vector2(row, i));
                }
            }
        }

        // Removes a brick (that has been hit)
        public void removeBrick(Brick brick)
        {
            bricks[(int)brick.index.X, (int)brick.index.Y] = null;
        }

        // Checks if any bricks are left
        public bool noBricksLeft()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if (bricks[i, j] != null) return false;
                }
            }
            return true;
        }

        // Returns the brick grid
        public Brick[,] getBricks()
        {
            return bricks;
        }

        // Checks if a new line will be cleared when given brick removed
        public bool newLineCleared(Brick brick)
        {
            for (int i = 0; i < 14; i++)
            {
                if (bricks[(int)brick.index.X, i] != null && bricks[(int)brick.index.X, i] != brick) return false;
            }
            return true;
        }
    }
}
