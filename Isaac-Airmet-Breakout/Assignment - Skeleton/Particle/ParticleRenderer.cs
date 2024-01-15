using Microsoft.Xna.Framework.Graphics;

namespace CS5410
{
    // Renders all the particles via the manager via emitters
    public class ParticleRenderer
    {
        private ParticleManager particleManager;
        public ParticleRenderer(ParticleManager particleManager)
        {
            this.particleManager = particleManager;
        }

        // Iterates over emitters in the manager
        public void renderParticles(SpriteBatch spriteBatch)
        {
            foreach (ParticleEmitter emitter in particleManager.GetParticleEmitters())
            {
                emitter.draw(spriteBatch);
            }
        }
    }
}
