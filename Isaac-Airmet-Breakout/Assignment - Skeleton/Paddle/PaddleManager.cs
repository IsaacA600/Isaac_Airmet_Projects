using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CS5410
{
    // Manages the paddles
    public class PaddleManager
    {
        private List<Paddle> paddles = new List<Paddle>();
        private int paddleShrinkRate = 100;
        private int initalWidth = 271;
        private int initalHeight = 25;
        public bool paddleRemoved = false;
        public PaddleManager()
        {
            // Create all 3 paddles initally and store them
            for (int i = 0; i < 3; i++)
            {
                paddles.Add(new Paddle(new Vector2(960, 950), initalWidth, initalHeight, new Vector2(0, 0), 0));
            }
        }

        // Updates paddle location and size
        public void updatePaddles(GameTime gameTime)
        {
            if (paddles.Count == 0) return;
            Paddle currentPaddle = paddles[0];
            double timeElapsed = gameTime.ElapsedGameTime.TotalSeconds;
            if (currentPaddle.isShrinking) 
            {
                // Shrink based on the size perameter on the paddle
                double newWidth = currentPaddle.width - (paddleShrinkRate * timeElapsed);
                if (newWidth <= (currentPaddle.size / 100f) * initalWidth){
                    newWidth = (currentPaddle.size / 100f) * initalWidth;
                    currentPaddle.isShrinking = false;
                } 
                // Check if it should be removed
                if (newWidth == 0)
                {
                    paddles.Remove(currentPaddle);
                    paddleRemoved = true;
                } 
                if (currentPaddle == null) return;
                currentPaddle.updateSize(currentPaddle.height, newWidth);
            }
            Vector2 currentLocation = currentPaddle.center;
            Vector2 newLocation = (currentLocation += (currentPaddle.direction * currentPaddle.speed * (float)timeElapsed));
            // Update location if within screen
            if (newLocation.X >= currentPaddle.width / 2 && newLocation.X <= 1920 - (currentPaddle.width / 2)) currentPaddle.updateCenter(newLocation);

        }

        // Gets the currently used paddle
        public Paddle getCurrentPaddle()
        {
            return paddles[0];
        }

        // Try and move to the left
        public void movePaddleLeft()
        {
            if (paddles.Count == 0) return;
            Paddle currentPaddle = paddles[0];
            currentPaddle.direction = new Vector2(-1, 0);
            currentPaddle.speed = 1800;
        }

        // Try and move to the right
        public void movePaddleRight()
        {
            if (paddles.Count == 0) return;
            Paddle currentPaddle = paddles[0];
            currentPaddle.direction = new Vector2(1, 0);
            currentPaddle.speed = 1800;
        }

        // Stop paddles motion after releasing arrows
        public void verifyPaddleStill()
        {
            if (paddles.Count == 0) return;
            Paddle currentPaddle = paddles[0];
            currentPaddle.direction = new Vector2(0, 0);
            currentPaddle.speed = 0;
        }

        // See how many paddles are in reserve
        public int getRemainingPaddles()
        {
            return paddles.Count - 1;
        }

        // Shrink a paddle to a given amount relative to the orgional size
        public void shrinkPaddle(int amount, Paddle paddle)
        {
            paddle.size = amount;
            paddle.isShrinking = true;
        }

        // See if a paddle can be halfed from inital value
        public bool canHalfPaddle(Paddle paddle)
        {
            return paddle.size == 100;
        }

        // Get all the paddles
        public List<Paddle> getAllPaddles()
        {
            return paddles;
        }

        // Update the managers status on that it just removed a paddle
        public void updatePaddleRemovedStatus(bool status)
        {
            paddleRemoved = status;
        }
    }
}
