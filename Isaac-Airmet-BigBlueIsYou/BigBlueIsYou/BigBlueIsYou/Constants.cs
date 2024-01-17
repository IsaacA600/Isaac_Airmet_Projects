using BigBlueIsYou.Components;

namespace BigBlueIsYou
{
  public static class Constants
  {
    public const int WINDOW_HEIGHT = 1080;
    public const int WINDOW_WIDTH = 1920;
    public const string KEY_BINDING_FILE = "KeyBindings100.xml";
    public const string LEVEL_FILE = "levels-all.bbiy";
    public static readonly NounType[] NounNounTypes = { NounType.BigBlue, NounType.Wall, NounType.Flag, NounType.Rock, NounType.Lava, NounType.Water, NounType.Grass, NounType.Hedge };
    public static readonly NounType[] VerbNounTypes = { NounType.Stop, NounType.Push, NounType.You, NounType.Win, NounType.Sink, NounType.Kill };
  }

  public class UtilSingleton
  {
    public int gridOffsetX;
    public int gridOffsetY;
    public int entitiesCreated;

    private static UtilSingleton Instance = null;

    private UtilSingleton()
    {
      gridOffsetX = 0;
      gridOffsetY = 0;
      entitiesCreated = 0;
    }

    public static UtilSingleton getUtilSingleton()
    {
      if (Instance == null)
      {
        Instance = new UtilSingleton();
      }

      return Instance;
    }
  }
}
