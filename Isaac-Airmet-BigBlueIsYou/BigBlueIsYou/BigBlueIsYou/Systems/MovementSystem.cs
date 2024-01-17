using BigBlueIsYou.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BigBlueIsYou.Components;
using Microsoft.Xna.Framework.Audio;
using System;
using BigBlueIsYou.Utils;
using BigBlueIsYou.Particles;

namespace BigBlueIsYou.Systems
{
  public class MovementSystem : ISystem
  {
    private SoundEffect walkSound;
    private ParticleSystem particleSystem;

    public MovementSystem(ParticleSystem pSystem)
    {
      Entities = new Dictionary<Guid, IEntity>();
      particleSystem = pSystem;
    }

    public Dictionary<Guid, IEntity> Entities { get; set; }

    public bool IsInterested(IEntity entity)
    {
      if (!entity.HasComponent<MoveComponent>())
      {
        return false;
      }

      if (!entity.HasComponent<PositionComponent>())
      {
        return false;
      }

      return true;
    }

    public void LoadContent(params object[] content)
    {
      if (content?.Length > 0)
      {
        walkSound = (SoundEffect)content[0];
      }
    }

    public Point GetNextPosition(Point curPoint, Direction dir)
    {
      switch (dir)
      {
        case Direction.Up:
          return new Point(curPoint.X - 1, curPoint.Y);

        case Direction.Down:
          return new Point(curPoint.X + 1, curPoint.Y);

        case Direction.Left:
          return new Point(curPoint.X, curPoint.Y - 1);

        case Direction.Right:
          return new Point(curPoint.X, curPoint.Y + 1);

        default:
          return curPoint;
      }
    }

    public bool CheckOnGrid(Point point)
    {
      GridManager gm = GridManager.GetInstance();
      return point.X >= 0 && point.X <= gm.GetGridWidth() - 1 && point.Y >= 0 && point.Y <= gm.GetGridHeight() - 1;
    }

    public bool CheckCanMove(IEntity movingEntity, Direction dir, out List<IEntity> movingEntities)
    {
      movingEntities = new List<IEntity>();
      PositionComponent pos = movingEntity.GetComponent<PositionComponent>();
      Point nextPos = GetNextPosition(pos.Position, dir);

      if (CheckOnGrid(nextPos))
      {
        List<IEntity>[,] curGrid = GridManager.GetInstance().GetCurrentGrid();
        List<IEntity> entities = curGrid[nextPos.X, nextPos.Y];
        foreach (IEntity entity in entities)
        {
          if (entity.HasComponent<PropertyComponent>())
          {
            PropertyComponent props = entity.GetComponent<PropertyComponent>();
            if (props.HasProperty(Properties.Stop))
            {
              return false;
            }

            if (props.HasProperty(Properties.Push))
            {
              bool canMove = CheckCanMove(entity, dir, out List<IEntity> chainMoveEntities);
              movingEntities.AddRange(chainMoveEntities);
              movingEntities.Add(movingEntity);
              return canMove;
            }
          }
        }

        movingEntities.Add(movingEntity);
        return true;
      }

      return false;
    }

    public bool UpdateGrid(List<IEntity> movingEntities, Direction dir, List<IEntity>[,] updatedGrid)
    {
      bool didWin = false;
      foreach (IEntity entity in movingEntities)
      {
        PositionComponent pos = entity.GetComponent<PositionComponent>();
        PropertyComponent props = entity.GetComponent<PropertyComponent>();
        NounComponent noun = entity.GetComponent<NounComponent>();

        IEntity gridEntity = updatedGrid[pos.Position.X, pos.Position.Y].Find(e => e.Name == entity.Name);
        gridEntity.RemoveComponent<MoveComponent>();
        entity.RemoveComponent<MoveComponent>();
        pos = gridEntity.GetComponent<PositionComponent>();

        Point nextPos = GetNextPosition(pos.Position, dir);

        List<IEntity> nextSquareEntities = updatedGrid[nextPos.X, nextPos.Y];
        List<(string, Point)> entitiesToRemove = new List<(string, Point)>();
        bool move = true;
        foreach (IEntity nextEntity in nextSquareEntities)
        {
          NounComponent nextNoun = nextEntity.GetComponent<NounComponent>();
          if (nextEntity.HasComponent<PropertyComponent>())
          {
            PropertyComponent nextProps = nextEntity.GetComponent<PropertyComponent>();
            if (nextProps.HasProperty(Properties.Kill))
            {
              if (props.HasProperty(Properties.You))
              {
                entitiesToRemove.Add((entity.Name, pos.Position));
                particleSystem.OnDestroy(new Point(nextPos.Y, nextPos.X), noun.NType);
                move = false;
                break;
              }
            }
            else if (nextProps.HasProperty(Properties.Sink))
            {
              // do sink stuff
              if (!entity.HasComponent<TextComponent>())
              {
                entitiesToRemove.Add((entity.Name, pos.Position));
                entitiesToRemove.Add((nextEntity.Name, nextPos));
                particleSystem.OnDestroy(new Point(nextPos.Y, nextPos.X), noun.NType);
                particleSystem.OnDestroy(new Point(nextPos.Y, nextPos.X), nextNoun.NType);
                move = false;
                break;
              }
            }
            else if (nextProps.HasProperty(Properties.Win))
            {
              if (props.HasProperty(Properties.You))
              {
                didWin = true;
                particleSystem.OnWin(new Point(nextPos.Y, nextPos.X));
                break;
              }
            }
          }
        }

        if (move)
        {
          entitiesToRemove.Add((entity.Name, pos.Position));
          pos.Position = nextPos;
          updatedGrid[nextPos.X, nextPos.Y].Add(gridEntity);
        }

        foreach ((string, Point) toRemove in entitiesToRemove)
        {
          updatedGrid[toRemove.Item2.X, toRemove.Item2.Y].RemoveAll(e => e.Name == toRemove.Item1);
        }
      }

      return didWin;
    }

    public bool Update(GameTime gameTime)
    {
      GridManager gridManager = GridManager.GetInstance();
      List<IEntity>[,] updatedGrid = gridManager.GetCurrentGridClone();
      bool didStep = false;
      bool didWin = false;
      foreach (IEntity entity in Entities.Values)
      {
        MoveComponent moveComp = entity.GetComponent<MoveComponent>();
        PositionComponent posComp = entity.GetComponent<PositionComponent>();

        didStep |= CheckCanMove(entity, moveComp.direction, out List<IEntity> movingEntities);
        didWin |= UpdateGrid(movingEntities, moveComp.direction, updatedGrid);

        entity.RemoveComponent<MoveComponent>();
      }

      Entities.Clear();

      if (didStep){
        walkSound.Play();
        gridManager.AddGrid(updatedGrid);
      } 

      return didWin;
    }
  }
}
