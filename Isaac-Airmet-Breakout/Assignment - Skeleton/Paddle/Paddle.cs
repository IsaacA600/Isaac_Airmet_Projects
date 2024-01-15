using Microsoft.Xna.Framework;

namespace CS5410
{
    // Paddle info class
    public class Paddle
    {
        public Vector2 center;
        public double width;
        public double height;
        public HitBox hitBox;
        public Vector2 direction;
        public float speed;
        public int size;
        public bool isShrinking;
        
        public Paddle(Vector2 center, double width, double height, Vector2 direction, float speed)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.direction = direction;
            this.speed = speed;
            hitBox = new HitBox(center, width, height);
            size = 100;
            isShrinking = false;
        }

        // Updates center of paddle as moved around
        public void updateCenter(Vector2 center)
        {
            this.center = center;
            hitBox.updateLocation(center);
        }

        // Updates size as paddle shrinks
        public void updateSize(double height, double width)
        {
            this.height = height;
            this.width = width;
            hitBox.updateSize(width, height);
        }
    }
}
