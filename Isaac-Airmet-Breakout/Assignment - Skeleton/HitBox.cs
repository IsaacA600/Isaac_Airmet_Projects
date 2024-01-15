using Microsoft.Xna.Framework;

namespace CS5410
{
    // Hitbox class for paddle, ball, bricks
    public class HitBox
    {
        public Vector2 center;
        public double width;
        public double height;
        public double right;
        public double left;
        public double top;
        public double bottom;
        public HitBox(Vector2 center, double width, double height)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.right = center.X + (width / 2.0);
            this.left = center.X - (width / 2.0);
            this.top = center.Y - (height / 2.0);
            this.bottom = center.Y + (height / 2.0);
        }

        // Did we collide with another hitbox?
        public bool didCollideWithHitBox(HitBox otherHitBox)
        {
            return !(
                left > otherHitBox.right ||
                right < otherHitBox.left ||
                top > otherHitBox.bottom ||
                bottom < otherHitBox.top
            );
        }

        // Update hitbox location
        public void updateLocation(Vector2 center)
        {
            this.center = center;
            this.right = center.X + (width / 2);
            this.left = center.X - (width / 2);
            this.top = center.Y - (height / 2);
            this.bottom = center.Y + (height / 2);
        }

        // Update hitbox size
        public void updateSize(double width, double height)
        {
            this.width = width;
            this.height = height;
            this.right = center.X + (height / 2);
            this.left = center.X - (height / 2);
            this.top = center.Y - (height / 2);
            this.bottom = center.Y + (height / 2);
        }
    }
}
