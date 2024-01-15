using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410
{
    // Manages all the particle emitters
    public class ParticleManager
    {
        private List<ParticleEmitter> particleEmitters = new List<ParticleEmitter>();
        public ParticleManager() { }

        // Adds a new particle emitter
        public void addParticleEmitter(int sourceX, int sourceY, int width, int height, int size, int speed, TimeSpan lifetime, Texture2D smokeTexture)
        {
            particleEmitters.Add(new ParticleEmitter(sourceX, sourceY, width, height, size, speed, lifetime, smokeTexture));
        }

        // Updates all emitters and removes old ones
        public void updateParticleEmitters(GameTime gameTime)
        {
            foreach(ParticleEmitter emitter in particleEmitters)
            {
                emitter.update(gameTime);
            }
            for (int i = 0; i < particleEmitters.Count; i++)
            {
                if (particleEmitters[i].isDone())
                {
                    particleEmitters.RemoveAt(i);
                    i--;
                }
            }
        }

        // Returns all emitters currently displaying particles
        public List<ParticleEmitter> GetParticleEmitters()
        {
            return particleEmitters;
        }
    }
}
