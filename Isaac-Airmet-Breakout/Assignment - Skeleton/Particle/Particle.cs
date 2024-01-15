using System;
using Microsoft.Xna.Framework;

namespace CS5410
{
    // Particle info class
    public class Particle
    {
        public Vector2 position;
        public float rotation;
        public Vector2 direction;
        public float speed;
        public TimeSpan lifetime;
        public Particle(Vector2 position, Vector2 direction, float speed, TimeSpan lifetime)
        {
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.lifetime = lifetime;
            this.rotation = 0;
        }
    }
}
