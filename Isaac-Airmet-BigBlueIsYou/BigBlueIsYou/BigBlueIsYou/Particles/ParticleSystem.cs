using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using BigBlueIsYou.Components;
using Microsoft.Xna.Framework.Graphics;
using BigBlueIsYou.Utils;

namespace BigBlueIsYou.Particles
{
  public class ParticleSystem
  {
    public readonly Dictionary<NounType, Color> NounTypeToColorMap = new Dictionary<NounType, Color>
    {
      { NounType.BigBlue, Color.White },
      { NounType.Wall, Color.Gray },
      { NounType.Flag, Color.Yellow },
      { NounType.Rock, Color.Brown },
      { NounType.Lava, Color.Red },
      { NounType.Water, Color.Blue },
      { NounType.Grass, Color.Green },
      { NounType.Hedge, Color.Green },
    };

    public Dictionary<Guid, ParticleEmitter> Emitters;
    public Texture2D ParticleTexture;

    private const int tileOffset = 12;
    private const int particleSize = 15;

    public ParticleSystem()
    {
      Emitters = new Dictionary<Guid, ParticleEmitter>();
    }

    public void LoadContent(Texture2D texture)
    {
      ParticleTexture = texture;
    }

    public void Update(GameTime gameTime)
    {
      List<Guid> toRemove = new List<Guid>();
      foreach (ParticleEmitter emitter in Emitters.Values)
      {
        emitter.Update(gameTime);
        if (emitter.EmitterLifetime <= TimeSpan.Zero)
        {
          toRemove.Add(emitter.Id);
        }
      }

      foreach (Guid id in toRemove)
      {
        Emitters.Remove(id);
      }
    }

    public void Render(SpriteBatch spriteBatch)
    {
      foreach (ParticleEmitter emitter in Emitters.Values)
      {
        emitter.Render(spriteBatch);
      }
    }

    public void OnWin(Point position)
    {
      GridManager gm = GridManager.GetInstance();
      Rectangle tile = new Rectangle(gm.StartX + position.X * gm.CellSize - tileOffset, gm.StartY + position.Y * gm.CellSize - tileOffset, gm.CellSize, gm.CellSize);
      ParticleEmitter emitter = new ParticleEmitter(ParticleTexture, TimeSpan.FromMilliseconds(10), tile, particleSize, 5.0f, 1.0f, Color.Yellow, TimeSpan.FromSeconds(3.0), ParticleEmitterSourceConfiguration.TileEdge);
      Emitters.TryAdd(emitter.Id, emitter);
    }
    public void OnDestroy(Point position, NounType nType)
    {
      GridManager gm = GridManager.GetInstance();
      Rectangle tile = new Rectangle(gm.StartX + position.X * gm.CellSize - tileOffset, gm.StartY + position.Y * gm.CellSize - tileOffset, gm.CellSize, gm.CellSize);
      ParticleEmitter emitter = new ParticleEmitter(ParticleTexture, TimeSpan.FromMilliseconds(1), tile, particleSize, 0.05f, 1.0f, NounTypeToColorMap[nType], TimeSpan.FromSeconds(0.5), ParticleEmitterSourceConfiguration.TileArea);
      Emitters.TryAdd(emitter.Id, emitter);
    }
    public void OnChangeYou(Point position)
    {
      GridManager gm = GridManager.GetInstance();
      Rectangle tile = new Rectangle(gm.StartX + position.X * gm.CellSize - tileOffset, gm.StartY + position.Y * gm.CellSize - tileOffset, gm.CellSize, gm.CellSize);
      ParticleEmitter emitter = new ParticleEmitter(ParticleTexture, TimeSpan.FromMilliseconds(5), tile, particleSize, 0.0f, 1.0f, Color.White, TimeSpan.FromSeconds(1.0), ParticleEmitterSourceConfiguration.TileEdge);
      Emitters.TryAdd(emitter.Id, emitter);
    }

    public void OnChangeWin(Point position)
    {
      GridManager gm = GridManager.GetInstance();
      Rectangle tile = new Rectangle(gm.StartX + position.X * gm.CellSize - tileOffset, gm.StartY + position.Y * gm.CellSize - tileOffset, gm.CellSize, gm.CellSize);
      ParticleEmitter emitter = new ParticleEmitter(ParticleTexture, TimeSpan.FromMilliseconds(5), tile, particleSize, 0.0f, 1.0f, Color.Yellow, TimeSpan.FromSeconds(1.0), ParticleEmitterSourceConfiguration.TileEdge);
      Emitters.TryAdd(emitter.Id, emitter);
    }

    public void ClearEmitters()
    {
      Emitters.Clear();
    }
  }
}
