using BigBlueIsYou.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BigBlueIsYou.Particles
{
  public class ParticleEmitter
  {
    public Guid Id;
    public Dictionary<int, Particle> ActiveParticles;
    public Texture2D ParticleTexture;
    public TimeSpan EmitterRate;
    public Rectangle SourceRect;
    public int ParticleSize;
    public float ParticleSpeed;
    public float ParticleRotationRate;
    public Color ParticleColor;
    public TimeSpan EmitterLifetime;
    public MyRandom Rand;
    public ParticleEmitterSourceConfiguration ParticleConfig;
    public TimeSpan AccumulatedTime;

    public ParticleEmitter()
    {
      Id = Guid.NewGuid();
      ActiveParticles = new Dictionary<int, Particle>();
      Rand = new MyRandom();
      AccumulatedTime = TimeSpan.Zero;
    }

    public ParticleEmitter(Texture2D texture, TimeSpan rate, Rectangle source, int size, float speed, float rotationRate, Color color, TimeSpan lifetime, ParticleEmitterSourceConfiguration particleSourceConfig)
      : this()
    {
      ParticleTexture = texture;
      EmitterRate = rate;
      SourceRect = source;
      ParticleSize = size;
      ParticleSpeed = speed;
      ParticleRotationRate = rotationRate;
      ParticleColor = color;
      EmitterLifetime = lifetime;
      ParticleConfig = particleSourceConfig;
    }

    public void Update(GameTime gameTime)
    {
      AccumulatedTime += gameTime.ElapsedGameTime;
      EmitterLifetime -= gameTime.ElapsedGameTime;
      while (AccumulatedTime > EmitterRate)
      {
        AccumulatedTime -= EmitterRate;
        Particle particle = GenerateParticle();
        ActiveParticles.TryAdd(particle.Id, particle);
      }

      List<int> toRemove = new List<int>();
      foreach (Particle p in ActiveParticles.Values)
      {
        p.Update(gameTime);
        if (p.Lifetime <= TimeSpan.Zero)
        {
          toRemove.Add(p.Id);
        }
      }

      foreach (int id in toRemove)
      {
        ActiveParticles.Remove(id);
      }
    }

    public void Render(SpriteBatch spriteBatch)
    {
      // Renders particles
      foreach (Particle p in ActiveParticles.Values)
      {
        p.Render(spriteBatch);
      }
    }

    public Particle GenerateParticle()
    {
      Particle particle = null;
      switch (ParticleConfig)
      {
        case ParticleEmitterSourceConfiguration.TileEdge:
          particle = GenerateTileEdgeParticle();
          break;

        case ParticleEmitterSourceConfiguration.TileCenter:
          particle = GenerateTileCenterParticle();
          break;

        case ParticleEmitterSourceConfiguration.TileArea:
          particle = GenerateTileAreaParticle();
          break;

        case ParticleEmitterSourceConfiguration.Default:
        default:
          particle = GenerateTileCenterParticle();
          break;
      }

      return particle;
    }

    public Particle GenerateTileEdgeParticle()
    {
      Vector2 source = GetNextTileOutlinePoint();
      Vector2 direction = source - SourceRect.Center.ToVector2();
      direction.Normalize();

      return new Particle(ParticleTexture, Rand.Next(), source, ParticleSize, direction, ParticleSpeed, ParticleRotationRate, EmitterLifetime, ParticleColor);
    }

    public Particle GenerateTileCenterParticle()
    {
      Vector2 source = SourceRect.Center.ToVector2();
      Vector2 direction = Rand.nextCircleVector();
      direction.Normalize();

      return new Particle(ParticleTexture, Rand.Next(), source, ParticleSize, direction, ParticleSpeed, ParticleRotationRate, EmitterLifetime, ParticleColor);
    }

    public Particle GenerateTileAreaParticle()
    {
      float x = Rand.nextRange(SourceRect.Left, SourceRect.Right);
      float y = Rand.nextRange(SourceRect.Top, SourceRect.Bottom);
      Vector2 source = new Vector2(x, y);
      Vector2 direction = source - SourceRect.Center.ToVector2();

      return new Particle(ParticleTexture, Rand.Next(), source, ParticleSize, direction, ParticleSpeed, ParticleRotationRate, EmitterLifetime, ParticleColor);
    }

    private Vector2 GetNextTileOutlinePoint()
    {
      float x = Rand.nextRange(SourceRect.Left, SourceRect.Right);
      float y = Rand.nextRange(SourceRect.Top, SourceRect.Bottom);
      float leftDistance = x - SourceRect.Left;
      float rightDistance = SourceRect.Right - x;
      float topDistance = y - SourceRect.Top;
      float bottomDistance = SourceRect.Bottom - y;

      bool left = leftDistance < rightDistance;
      bool top = topDistance < bottomDistance;

      if (left)
      {
        if (top)
        {
          if (leftDistance < topDistance)
          {
            x = SourceRect.Left;
          }
          else
          {
            y = SourceRect.Top;
          }
        }
        else
        {
          if (leftDistance < bottomDistance)
          {
            x = SourceRect.Left;
          }
          else
          {
            y = SourceRect.Bottom;
          }
        }
      }
      else
      {
        if (top)
        {
          if (rightDistance < topDistance)
          {
            x = SourceRect.Right;
          }
          else
          {
            y = SourceRect.Top;
          }
        }
        else
        {
          if (rightDistance < bottomDistance)
          {
            x = SourceRect.Right;
          }
          else
          {
            y = SourceRect.Bottom;
          }
        }
      }

      return new Vector2(x, y);
    }
  }
}
