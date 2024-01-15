using Microsoft.Xna.Framework;

namespace CS5410
{
    // Info class for a brick
    public class Brick
    {
        public Vector2 center;
        public double width;
        public double height;
        public BrickColorEnum color;
        public HitBox hitBox;
        public Vector2 index; // Index in the grid

        public Brick(Vector2 center, double width, double height, BrickColorEnum color, Vector2 index)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            this.color = color;
            this.hitBox = new HitBox(center, width, height);
            this.index = index;
        }
    }
}
