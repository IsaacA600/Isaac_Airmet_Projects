using Microsoft.Xna.Framework;

namespace CS5410
{
    // Ball Info Class
    public class Ball
    {
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public double radius;
        public HitBox hitBox;
        public HitBox prevHitBox;
        public bool isExtraBall; // Extra ball from bonuses. 

        public Ball(Vector2 position, Vector2 direction, float speed, double radius = 20, bool isExtraBall = false) 
        { 
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.radius = radius;
            hitBox = new HitBox(position, radius, radius);
            prevHitBox = new HitBox(position, radius, radius);
            this.isExtraBall = isExtraBall;
        }

        // Updates Location of ball
        public void updateLocation(Vector2 position)
        {
            prevHitBox.updateLocation(this.position);
            this.position = position;
            hitBox.updateLocation(position);
        }
    }
}
