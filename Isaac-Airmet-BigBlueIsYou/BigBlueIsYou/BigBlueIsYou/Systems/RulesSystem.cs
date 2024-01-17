using BigBlueIsYou.Components;
using BigBlueIsYou.Entities;
using BigBlueIsYou.Particles;
using BigBlueIsYou.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigBlueIsYou.Systems
{
  public class RulesSystem : ISystem
  {
    private GridManager gridManager;
    private ContentManager cm;
    private SoundEffect OnChangeWinSound;
    private ParticleSystem particleSystem;
    private bool WinChanged;
    private bool YouIsWin;

    public Dictionary<Guid, IEntity> Entities { get; set; }

    public RulesSystem(ParticleSystem pSystem)
    {
      Entities = new Dictionary<Guid, IEntity>();
      gridManager = GridManager.GetInstance();
      particleSystem = pSystem;
      WinChanged = false;
      YouIsWin = false;
    }

    public bool IsInterested(IEntity entity)
    {
      if (!entity.HasComponent<PositionComponent>())
      {
        return false;
      }

      if (!entity.HasComponent<TextComponent>())
      {
        return false;
      }

      return true;
    }

    public bool Update(GameTime gameTime)
    {
      if (YouIsWin)
      {
        YouIsWin = false;
      }

      List<IEntity>[,] grid = gridManager.GetCurrentGrid();
      List<Rule> rules = FindRules(grid);
      List<Rule> nounRules = rules.Where(r => Constants.NounNounTypes.Contains(r.Ending)).ToList();
      List<Rule> verbRules = rules.Where(r => Constants.VerbNounTypes.Contains(r.Ending)).ToList();

      EnactNounRules(nounRules, grid);
      EnactVerbRules(verbRules, grid);

      if (WinChanged)
      {
        OnChangeWinSound.Play();
        WinChanged = false;
      }

      return YouIsWin;
    }

    public List<Rule> FindRules(List<IEntity>[,] grid)
    {
      List<Rule> rules = new List<Rule>();

      for (int i = 0; i < grid.GetLength(0); ++i)
      {
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
          IEntity textEnt = grid[i, j].Find(ent => ent.HasComponent<TextComponent>());
          if (textEnt != null)
          {
            TextComponent text = textEnt.GetComponent<TextComponent>();

            // only nouns can be the start of a rule
            if (text.TType == TextType.Noun)
            {
              // only check for right rule if entity is not on the right side of the grid
              if (i < gridManager.GetGridWidth() - 2)
              {
                // check to right for text entity
                IEntity isEnt = grid[i + 1, j].Find(ent => ent.HasComponent<TextComponent>());
                if (isEnt != null && isEnt.GetComponent<TextComponent>().TType == TextType.Adjective)
                {
                  IEntity rightEnt = grid[i + 2, j].Find(ent => ent.HasComponent<TextComponent>());
                  if (rightEnt != null)
                  {
                    textEnt.GetComponent<TextComponent>().IsPartOfHorizontalRule = true;
                    rightEnt.GetComponent<TextComponent>().IsPartOfHorizontalRule = true;
                    rules.Add(new Rule(textEnt.GetComponent<NounComponent>().NType, rightEnt.GetComponent<NounComponent>().NType));
                  }
                }
              }

              // only check for down rule if entity is not on the bottom of the grid
              if (j < gridManager.GetGridHeight() - 2)
              {
                // check down for text entity
                TextIsEntity isEnt = grid[i, j + 1].Find(ent => ent.HasComponent<TextComponent>()) as TextIsEntity;
                if (isEnt != null)
                {
                  IEntity bottomEnt = grid[i, j + 2].Find(ent => ent.HasComponent<TextComponent>());
                  if (bottomEnt != null)
                  {
                    textEnt.GetComponent<TextComponent>().IsPartOfVerticalRule = true;
                    bottomEnt.GetComponent<TextComponent>().IsPartOfVerticalRule = true;
                    rules.Add(new Rule(textEnt.GetComponent<NounComponent>().NType, bottomEnt.GetComponent<NounComponent>().NType));
                  }
                }
              }
            }
          }
        }
      }

      return rules;
    }

    private void EnactVerbRules(List<Rule> verbRules, List<IEntity>[,] grid)
    {
      for (int i = 0; i < grid.GetLength(0); ++i)
      {
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
          bool hasWin = false;
          bool hasYou = false;
          for (int k = 0; k < grid[i, j].Count; ++k)
          {
            if (!grid[i, j][k].HasComponent<TextComponent>())
            {
              PropertyComponent prevProperties = grid[i, j][k].GetComponent<PropertyComponent>();
              grid[i, j][k].RemoveComponent<PropertyComponent>();
              grid[i, j][k].RemoveComponent<InputComponent>();
              PropertyComponent newProps = new PropertyComponent();
              NounComponent nComp = grid[i, j][k].GetComponent<NounComponent>();
              foreach (Rule rule in verbRules)
              {
                if (rule.Beginning == nComp.NType)
                {
                  Properties newProp = GetProperty(rule.Ending);
                  newProps.AddProperty(newProp);
                  if (newProp == Properties.You)
                  {
                    hasYou = true;
                    grid[i, j][k].AddComponent(new InputComponent());
                    if (prevProperties != null && !prevProperties.HasProperty(Properties.You))
                    {
                      particleSystem.OnChangeYou(new Point(j, i));
                    }
                  }

                  if (newProp == Properties.Win)
                  {
                    hasWin = true;
                    if (prevProperties != null && !prevProperties.HasProperty(Properties.Win))
                    {
                      particleSystem.OnChangeWin(new Point(j, i));
                      WinChanged = true;
                    }
                  }
                }
              }

              grid[i, j][k].AddComponent(newProps);
            }

            if (hasWin && hasYou)
            {
              YouIsWin = true;
              particleSystem.OnWin(new Point(j, i));
            }
          }
        }
      }
    }

    private void EnactNounRules(List<Rule> nounRules, List<IEntity>[,] grid)
    {
      for (int i = 0; i < grid.GetLength(0); ++i)
      {
        for (int j = 0; j < grid.GetLength(1); ++j)
        {
          for (int k = 0; k < grid[i, j].Count; ++k)
          {
            if (!grid[i, j][k].HasComponent<TextComponent>())
            {
              NounComponent nComp = grid[i, j][k].GetComponent<NounComponent>();
              foreach (Rule rule in nounRules)
              {
                if (rule.Beginning == nComp.NType)
                {
                  grid[i, j][k] = TransformEntity(grid[i, j][k], rule.Ending);
                }
              }
            }
          }
        }
      }
    }

    private Properties GetProperty(NounType noun)
    {
      switch (noun)
      {
        case NounType.Stop:
          return Properties.Stop;

        case NounType.Push:
          return Properties.Push;

        case NounType.You:
          return Properties.You;

        case NounType.Win:
          return Properties.Win;

        case NounType.Sink:
          return Properties.Sink;

        case NounType.Kill:
          return Properties.Kill;

        default:
          return Properties.None;
      }
    }

    private IEntity TransformEntity(IEntity entity, NounType noun)
    {
      SpriteComponent spriteComponent = entity.GetComponent<SpriteComponent>();
      PositionComponent positionComponent = entity.GetComponent<PositionComponent>();
      switch (noun)
      {
        case NounType.BigBlue:
          return EntityFactory.CreateEntity(typeof(BigBlueEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Wall:
          return EntityFactory.CreateEntity(typeof(WallEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Flag:
          return EntityFactory.CreateEntity(typeof(FlagEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Rock:
          return EntityFactory.CreateEntity(typeof(RockEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Lava:
          return EntityFactory.CreateEntity(typeof(LavaEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Water:
          return EntityFactory.CreateEntity(typeof(WaterEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Grass:
          return EntityFactory.CreateEntity(typeof(GrassEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        case NounType.Hedge:
          return EntityFactory.CreateEntity(typeof(HedgeEntity), cm, spriteComponent.sprite.spriteRect, positionComponent.Position);

        default:
          return entity;
      }
    }

    public void LoadContent(params object[] content)
    {
      if (content.Length > 1)
      {
        cm = (ContentManager)content[0];
        OnChangeWinSound = (SoundEffect)content[1];
      }
    }
  }
}
