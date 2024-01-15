using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CS5410
{
    // Manages the balls
    public class BallManager
    {
        private List<Ball> balls = new List<Ball>();
        public BallManager() { }

        // Adds a ball to the list of balls
        public void addBall(Ball ball) 
        {
            balls.Add(ball);
        }

        // Removes a ball that goes off screen
        public void removeBall(Ball ball)
        {
            balls.Remove(ball);
        }

        // Updates all balls locations
        public void updateBallsLocations(GameTime gameTime)
        {
            double timeElapsed = gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Ball ball in balls)
            {
                Vector2 currentLocation = ball.position;
                double smallXLeg = ball.direction.X;
                double smallYLeg = ball.direction.Y;
                double smallHy = Math.Sqrt((smallXLeg * smallXLeg) + (smallYLeg * smallYLeg));
                double ratio = (ball.speed * timeElapsed) / smallHy;
                Vector2 newLocation = new Vector2((float)(ball.position.X + (smallXLeg * ratio)), (float)(ball.position.Y + (smallYLeg * ratio)));
                ball.updateLocation(newLocation);
            }
        }

        // Gets all the balls on screen
        public List<Ball> getBalls()
        {
            return balls;
        }

        // Updates direction when a ball htis a wall
        public void updateBallDirectionWallShot(bool verticalWall, Ball ball)
        {
            if (verticalWall) ball.direction = new Vector2(-1 * ball.direction.X, ball.direction.Y);
            else ball.direction = new Vector2(ball.direction.X, -1 * ball.direction.Y);
        }

        // Updates direction when a ball hits a corner
        public void updateBallDirectionCornerShot(Ball ball)
        {
            ball.direction = new Vector2(-1 * ball.direction.X, -1 * ball.direction.Y);
        }

        // Updates speed of the ball as more bricks get hit
        public void increaseBallSpeed(Ball ball)
        {
            ball.speed = ball.speed * 1.25f;
        }

        // Updates direction when ball hits paddle.
        public void updateBallDirectionPaddleShot(float height, float newX, Ball ball)
        {
            float leg1 = newX;
            float leg2 = ball.direction.Y;
            ball.direction = new Vector2(leg1, -1 * leg2);
            ball.updateLocation(new Vector2(ball.position.X, height));
        }
    }
}
