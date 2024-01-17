using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BigBlueIsYou.Particles
{
  public class Particle
  {
    public Texture2D Texture;
    public int Id;
    public Vector2 Position;
    public int Size;
    public Vector2 Direction;
    public float Speed;
    public float Rotation;
    public float RotationRate;
    public TimeSpan Lifetime;
    public Color ParticleColor;

    public Rectangle ParticleRect
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, Size, Size);
      }
    }

    public Particle()
    {
      Rotation = 0;
    }

    public Particle(Texture2D texture, int name, Vector2 position, int size, Vector2 direction, float speed, float rotRate, TimeSpan lifetime, Color color)
      : this()
    {
      Texture = texture;
      Id = name;
      Position = position;
      Size = size;
      Direction = direction;
      Speed = speed;
      Lifetime = lifetime;
      ParticleColor = color;
      RotationRate = rotRate;
    }

    public void Update(GameTime gameTime)
    {
      Lifetime -= gameTime.ElapsedGameTime;
      Position += Direction * Speed;
      Rotation += RotationRate;
    }

    public void Render(SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();
      spriteBatch.Draw(Texture, ParticleRect, null, ParticleColor, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), SpriteEffects.None, 0);
      spriteBatch.End();
    }
  }
}
