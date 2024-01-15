using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410
{
    // Creates particles for a given location
    public class ParticleEmitter
    {
        private List<Particle> particles = new List<Particle>();
        private Texture2D smokeTexture;
        private int sourceX;
        private int sourceY;
        private int width;
        private int height;
        private int particleSize;
        private int speed;
        private TimeSpan lifetime;
        private bool isOver;
        private bool initalUpdate;

        public ParticleEmitter(int sourceX, int sourceY, int width, int height, int size, int speed, TimeSpan lifetime, Texture2D smokeTexture)
        {
            this.sourceX = sourceX;
            this.sourceY = sourceY;
            this.width = width;
            this.height = height;
            particleSize = size;
            this.speed = speed;
            this.lifetime = lifetime;
            this.smokeTexture = smokeTexture;
            initalUpdate = true;
        }

        /// <summary>
        /// Generates new particles/updates them
        /// </summary>
        public void update(GameTime gameTime)
        {
            // Create particles to cover given area
            if (initalUpdate)
            {
                int particlesAcross = width / particleSize;
                int particlesDown = height / particleSize;
                for (int i = 0; i < particlesDown; i++)
                {
                    for (int j = 0; j < particlesAcross; j++)
                    {
                        Vector2 position = new Vector2(sourceX + (j * particleSize), sourceY + (i * particleSize));
                        Vector2 direction = new Vector2(0, 1);
                        float pSpeed = speed + (speed * i * .38f);
                        Particle p = new Particle(position, direction, pSpeed, lifetime);
                        particles.Add(p);
                    }
                }
                initalUpdate = false;
            }

            foreach (Particle p in particles)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    // All particles have same lifetime. One is up, they all are.
                    isOver = true;
                    return;
                }
                p.position += (p.direction * p.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle particleBox = new Rectangle(0, 0, particleSize, particleSize);
            foreach (Particle p in particles)
            {
                particleBox.X = (int)p.position.X;
                particleBox.Y = (int)p.position.Y;
                spriteBatch.Draw(smokeTexture, particleBox, Color.White);
            }
        }

        // Check if we can remove the emitter from the manager
        public bool isDone()
        {
            return isOver;
        }
    }
}
