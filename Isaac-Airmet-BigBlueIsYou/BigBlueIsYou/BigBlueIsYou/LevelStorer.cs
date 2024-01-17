using System.Collections.Generic;

namespace BigBlueIsYou
{
  public class LevelStorer
  {
    private List<LevelDataContainer> levels = new List<LevelDataContainer>();
    private LevelDataContainer activeLevel = null;
    private LevelStorer()
    {

    }

    private static readonly LevelStorer levelStorer = new LevelStorer();

    public static LevelStorer getLevelStorer()
    {
      return levelStorer;
    }

    public void addLevel(LevelDataContainer level)
    {
      levels.Add(level);
    }

    public List<LevelDataContainer> getAllLevels()
    {
      return levels;
    }

    public void setActiveLevel(LevelDataContainer level)
    {
      activeLevel = level;
    }

    public LevelDataContainer getActiveLevel()
    {
      return activeLevel;
    }
  }
}
