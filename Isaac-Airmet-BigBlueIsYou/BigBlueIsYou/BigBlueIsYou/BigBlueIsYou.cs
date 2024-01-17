using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace BigBlueIsYou
{
  public class BigBlueIsYou : Game
  {
    private GraphicsDeviceManager graphics;
    private IGameState currentState;
    private GameStateEnum nextStateEnum = GameStateEnum.MainMenu;
    private Dictionary<GameStateEnum, IGameState> m_states;
    KeyBindingStorage bindings = new KeyBindingStorage();
    public List<LevelDataContainer> levels = new List<LevelDataContainer>();
    FileManager fileManager = new FileManager();

    public BigBlueIsYou()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
      fileManager.startLoadingLevels(Constants.LEVEL_FILE);
      fileManager.startLoadingKeyBindings(Constants.KEY_BINDING_FILE, bindings);
    }

    protected override void Initialize()
    {
      graphics.PreferredBackBufferWidth = Constants.WINDOW_WIDTH;
      graphics.PreferredBackBufferHeight = Constants.WINDOW_HEIGHT;

      graphics.ApplyChanges();

      // Create all the game states here
      m_states = new Dictionary<GameStateEnum, IGameState>
            {
                { GameStateEnum.MainMenu, new MainMenuView() },
                { GameStateEnum.LevelSelector, new LevelSelectorView() },
                { GameStateEnum.GamePlay, new GamePlayView() },
                { GameStateEnum.Controls, new ControlsView() },
                { GameStateEnum.Credits, new CreditsView() }
            };

      foreach (var item in m_states)
      {
        item.Value.initialize(this.GraphicsDevice, graphics);
      }

      // We are starting with the main menu
      currentState = m_states[nextStateEnum];

      base.Initialize();
    }

    protected override void LoadContent()
    {
      // Give all game states a chance to load their content
      foreach (var item in m_states)
      {
        item.Value.loadContent(this.Content);
      }

    }

    protected override void Update(GameTime gameTime)
    {
      nextStateEnum = currentState.processInput(gameTime);
      checkSavedBindings();
      // Special case for exiting the game
      if (nextStateEnum == GameStateEnum.Exit)
      {
        Exit();
      }

      currentState = m_states[nextStateEnum];

      // Update the current game state
      currentState.update(gameTime);

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.Black);

      // Render current game state, then swap if needed
      currentState.render(gameTime);

      base.Draw(gameTime);
    }

    private void checkSavedBindings()
    {
      if (bindings.missingOnPC)
      {
        List<Keys> keyBindings = new List<Keys>();
        bindings = new KeyBindingStorage(keyBindings);
        bindings.keyBindings.Add(Keys.W);
        bindings.keyBindings.Add(Keys.D);
        bindings.keyBindings.Add(Keys.S);
        bindings.keyBindings.Add(Keys.A);
        bindings.keyBindings.Add(Keys.R);
        bindings.keyBindings.Add(Keys.Z);
        fileManager.startSavingKeyBindings(Constants.KEY_BINDING_FILE, bindings);
      }
    }
  }
}