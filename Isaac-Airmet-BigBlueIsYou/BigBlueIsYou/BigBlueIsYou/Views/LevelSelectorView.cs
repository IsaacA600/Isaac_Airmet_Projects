using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BigBlueIsYou
{
  internal class LevelSelectorView : GameStateView
  {
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    private const int MENU_WIDTH = Constants.WINDOW_WIDTH / 3;
    private const int MENU_HEIGHT = Constants.WINDOW_HEIGHT - 200;
    private const int MENU_BUTTON_WIDTH = (int)(MENU_WIDTH / 1.5);
    private int MENU_BUTTON_HEIGHT;
    private const string TITLE = "Level Selector";
    private const string BINDINGS_ERROR = "Key Loading Error! Close the program and try again";
    private Dictionary<Guid, MenuButtonObject> menuButtons;
    private Dictionary<Guid, Action<GameTime, bool>> buttonActionsMap;
    private SpriteFont titleFont;
    private SpriteFont buttonLabelFont;
    private SpriteFont errorFont;
    private Texture2D menuBackground;
    private Texture2D buttonTexture;
    private Rectangle menuBackgroundRect = new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);
    private GameStateEnum nextState;
    private MouseDeviceInput mouseDevice;
    private Point currentMousePosition;
    private Guid? selectedButton;
    private bool isLeaving;
    private LevelStorer levelStorer;
    private Dictionary<Guid, LevelDataContainer> buttonLevelMap;
    private MenuButtonObject backButton;
    private KeysStorer keysStorer;
    private bool isLoadingError;

    public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
      this.graphics = graphics;
      spriteBatch = new SpriteBatch(graphicsDevice);
      menuButtons = new Dictionary<Guid, MenuButtonObject>();
      buttonActionsMap = new Dictionary<Guid, Action<GameTime, bool>>();
      buttonLevelMap = new Dictionary<Guid, LevelDataContainer>();
      nextState = GameStateEnum.LevelSelector;
      isLeaving = false;
      levelStorer = LevelStorer.getLevelStorer();
      keysStorer = KeysStorer.getKeysStorer();
      isLoadingError = false;

      foreach (LevelDataContainer level in levelStorer.getAllLevels())
      {
        MenuButtonObject button = new MenuButtonObject(level.Name, Color.White, Color.Yellow, Color.Red);
        menuButtons.Add(button.ButtonId, button);
        buttonActionsMap.Add(button.ButtonId, levelButtonAction);
        buttonLevelMap.Add(button.ButtonId, level);
      }
      backButton = new MenuButtonObject("Back", Color.White, Color.Yellow, Color.Red);

      menuButtons.Add(backButton.ButtonId, backButton);

      buttonActionsMap.Add(backButton.ButtonId, backButtonAction);

      mouseDevice = new MouseDeviceInput();
      mouseDevice.setLeftMouseButtonAction(mouseLeftButtonAction);
      mouseDevice.setMouseCursorAction(mouseCursorAction);

      MENU_BUTTON_HEIGHT = (MENU_HEIGHT / menuButtons.Count) - 20;
    }

    public override void loadContent(ContentManager contentManager)
    {
      titleFont = contentManager.Load<SpriteFont>("Fonts/regFont");
      buttonLabelFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      errorFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      menuBackground = contentManager.Load<Texture2D>("Images/background");
      buttonTexture = contentManager.Load<Texture2D>("Images/stone-pattern");
      List<MenuButtonObject> buttons = menuButtons.Values.ToList();
      for (int i = 0; i < buttons.Count; ++i)
      {
        buttons[i].loadContent(buttonLabelFont, new Rectangle(menuBackgroundRect.Center.X - MENU_BUTTON_WIDTH / 2, (menuBackgroundRect.Y + 150) + (i * (MENU_BUTTON_HEIGHT + 10)), MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT), buttonTexture);
      }

    }

    public override GameStateEnum processInput(GameTime gameTime)
    {
      mouseDevice.update(gameTime);

      if (nextState != GameStateEnum.LevelSelector) isLeaving = true;

      return nextState;
    }

    public override void render(GameTime gameTime)
    {
      spriteBatch.Begin();

      spriteBatch.Draw(menuBackground, menuBackgroundRect, Color.White);
      spriteBatch.DrawString(
          titleFont,
          TITLE,
          new Vector2(Constants.WINDOW_WIDTH / 2 - (titleFont.MeasureString(TITLE) / 2).X, 50),
          Color.White,
          0.0f,
          new Vector2(0f, 0f),
          1f,
          SpriteEffects.None,
          0
      );

      if (isLoadingError)
      {
        renderErrorMessage();
        return;
      }

      spriteBatch.End();

      foreach (MenuButtonObject button in menuButtons.Values)
      {
        button.RenderObject(spriteBatch);
      }
    }

    private void renderErrorMessage()
    {
      spriteBatch.DrawString(
          errorFont,
          BINDINGS_ERROR,
          new Vector2(Constants.WINDOW_WIDTH / 2 - (titleFont.MeasureString(BINDINGS_ERROR) / 2).X, 200),
          Color.Red,
          0.0f,
          new Vector2(0f, 0f),
          1f,
          SpriteEffects.None,
          0
      );
    }

    public override void update(GameTime gameTime)
    {
      selectedButton = null;
      if (isLeaving)
      {
        nextState = GameStateEnum.LevelSelector;
        isLeaving = false;
        isLoadingError = false;
        return;
      }

      foreach (MenuButtonObject button in menuButtons.Values)
      {
        button.IsHighlighted = button.ObjectRectangle.Contains(currentMousePosition);
        if (button.IsHighlighted)
        {
          selectedButton = button.ButtonId;
          if (button.Label != "Back") levelStorer.setActiveLevel(buttonLevelMap[button.ButtonId]);
        }
      }
    }

    private void backButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.MainMenu;
      }
    }

    private void levelButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      nextState = GameStateEnum.GamePlay;
    }

    private void mouseLeftButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed && selectedButton != null)
      {
        if (menuButtons[selectedButton.Value].ObjectRectangle.Contains(currentMousePosition))
        {
          buttonActionsMap[selectedButton.Value](gameTime, alreadyPressed);
        }
      }
    }

    private void mouseCursorAction(GameTime gameTime, Point cursorPosition)
    {
      currentMousePosition = cursorPosition;
    }
  }
}
