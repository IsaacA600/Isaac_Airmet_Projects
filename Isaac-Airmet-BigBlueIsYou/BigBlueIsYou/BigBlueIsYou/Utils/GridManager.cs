using BigBlueIsYou.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigBlueIsYou.Utils
{
  public class GridManager
  {
    private static GridManager Instance;
    private Stack<List<IEntity>[,]> GridStack = new Stack<List<IEntity>[,]>();
    private List<IEntity>[,] CurrentGrid;
    private int GridHeight;
    private int GridWidth;
    private int SCREEN_HEIGHT = Constants.WINDOW_HEIGHT;
    private int SCREEN_WIDTH = Constants.WINDOW_WIDTH;
    public int CellSize;
    public int StartX;
    public int StartY;

    private GridManager()
    {
      // Empty
    }

    public static GridManager GetInstance()
    {
      if (Instance == null)
      {
        Instance = new GridManager();
      }

      return Instance;
    }

    public void ParseLevelGrid(LevelDataContainer initialData, ContentManager contentManager)
    {
      GridHeight = initialData.height;
      GridWidth = initialData.width;
      CellSize = Math.Min(SCREEN_HEIGHT / GridHeight, SCREEN_WIDTH / GridWidth) - 1;
      StartX = (SCREEN_WIDTH - (CellSize * GridWidth)) / 2;
      StartY = (SCREEN_HEIGHT - (CellSize * GridHeight)) / 2;
      UtilSingleton.getUtilSingleton().gridOffsetX = StartX;
      UtilSingleton.getUtilSingleton().gridOffsetY = StartY;
      List<char>[,] data = initialData.data;
      List<IEntity>[,] initialGrid = new List<IEntity>[GridHeight, GridWidth];
      for (int i = 0; i < GridHeight; i++)
      {
        for (int j = 0; j < GridWidth; j++)
        {
          List<IEntity> positionList = new List<IEntity>();
          Rectangle positionRect = new Rectangle((StartX + j * CellSize), (StartY + i * CellSize), CellSize, CellSize);
          Point initalVector = new Point(i, j);
          foreach (char c in data[i, j])
          {
            if (c == 'w')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(WallEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'r')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(RockEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'f')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(FlagEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'b')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(BigBlueEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'l')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(FloorEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'g')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(GrassEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'a')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(WaterEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'v')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(LavaEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'h')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(HedgeEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'W')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextWallEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'R')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextRockEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'F')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextFlagEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'B')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextBabaEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'I')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextIsEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'S')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextStopEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'P')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextPushEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'V')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextLavaEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'A')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextWaterEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'Y')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextYouEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'X')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextWinEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'N')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextSinkEntity), contentManager, positionRect, initalVector));
            }
            else if (c == 'K')
            {
              positionList.Add(EntityFactory.CreateEntity(typeof(TextKillEntity), contentManager, positionRect, initalVector));
            }
          }
          initialGrid[i, j] = positionList;
        }
      }

      CurrentGrid = initialGrid;
    }

    public List<IEntity>[,] GetCurrentGrid()
    {
      return CurrentGrid;
    }

    public List<IEntity>[,] GetCurrentGridClone()
    {
      return CloneGrid(CurrentGrid);
    }

    public void AddGrid(List<IEntity>[,] grid)
    {
      GridStack.Push(CurrentGrid);
      CurrentGrid = grid;
    }

    public int GetGridHeight()
    {
      return GridHeight;
    }

    public int GetGridWidth()
    {
      return GridWidth;
    }

    public void Reset()
    {
      if (GridStack.Count > 0)
      {
        CurrentGrid = GridStack.Last();
        GridStack.Clear();
      }
    }

    public void Undo()
    {
      if (GridStack.TryPop(out List<IEntity>[,] grid))
      {
        CurrentGrid = grid;
      }
    }

    public static List<IEntity>[,] CloneGrid(List<IEntity>[,] grid)
    {
      List<IEntity>[,] newGrid = new List<IEntity>[grid.GetLength(0), grid.GetLength(1)];
      for (int i = 0; i < grid.GetLength(0); i++)
      {
        for (int j = 0; j < grid.GetLength(1); j++)
        {
          List<IEntity> positionList = new List<IEntity>();
          foreach (IEntity entity in grid[i, j])
          {
            positionList.Add(entity.Clone());
          }

          newGrid[i, j] = positionList;
        }
      }

      return newGrid;
    }
  }
}
