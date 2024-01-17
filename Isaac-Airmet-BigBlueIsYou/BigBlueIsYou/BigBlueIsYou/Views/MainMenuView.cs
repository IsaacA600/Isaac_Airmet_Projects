using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BigBlueIsYou
{
  public class MainMenuView : GameStateView
  {
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    public const int MENU_WIDTH = Constants.WINDOW_WIDTH / 3;
    public const int MENU_HEIGHT = Constants.WINDOW_HEIGHT - 200;
    public const int MENU_BUTTON_WIDTH = (int)(MENU_WIDTH / 1.5);
    public const int MENU_BUTTON_HEIGHT = (MENU_HEIGHT / 4) - 20;
    public const string TITLE = "Big Blue Is You";
    private Dictionary<Guid, MenuButtonObject> menuButtons;
    private Dictionary<Guid, Action<GameTime, bool>> buttonActionsMap;
    private SpriteFont titleFont;
    private SpriteFont buttonLabelFont;
    private Texture2D menuBackground;
    private Texture2D buttonTexture;
    private Rectangle menuBackgroundRect = new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);
    private GameStateEnum nextState;
    private MouseDeviceInput mouseDevice;
    private Point currentMousePosition;
    private Guid? selectedButton;
    private bool isLeaving;
    MenuButtonObject newGameButton;
    MenuButtonObject controlsButton;
    MenuButtonObject creditsButton;
    MenuButtonObject quitButton;

    public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
      this.graphics = graphics;
      spriteBatch = new SpriteBatch(graphicsDevice);
      menuButtons = new Dictionary<Guid, MenuButtonObject>();
      buttonActionsMap = new Dictionary<Guid, Action<GameTime, bool>>();
      nextState = GameStateEnum.MainMenu;
      isLeaving = false;
      newGameButton = new MenuButtonObject("New Game", Color.White, Color.Yellow, Color.Red);
      controlsButton = new MenuButtonObject("Control Settings", Color.White, Color.Yellow, Color.Red);
      creditsButton = new MenuButtonObject("Credits", Color.White, Color.Yellow, Color.Red);
      quitButton = new MenuButtonObject("Exit", Color.White, Color.Yellow, Color.Red);

      menuButtons.Add(newGameButton.ButtonId, newGameButton);
      menuButtons.Add(controlsButton.ButtonId, controlsButton);
      menuButtons.Add(creditsButton.ButtonId, creditsButton);
      menuButtons.Add(quitButton.ButtonId, quitButton);

      buttonActionsMap.Add(newGameButton.ButtonId, newGameButtonAction);
      buttonActionsMap.Add(controlsButton.ButtonId, controlsButtonAction);
      buttonActionsMap.Add(creditsButton.ButtonId, creditsButtonAction);
      buttonActionsMap.Add(quitButton.ButtonId, quitButtonAction);

      mouseDevice = new MouseDeviceInput();
      mouseDevice.setLeftMouseButtonAction(mouseLeftButtonAction);
      mouseDevice.setMouseCursorAction(mouseCursorAction);
    }

    public override void loadContent(ContentManager contentManager)
    {
      titleFont = contentManager.Load<SpriteFont>("Fonts/regFont");
      buttonLabelFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      menuBackground = contentManager.Load<Texture2D>("Images/background");
      buttonTexture = contentManager.Load<Texture2D>("Images/stone-pattern");

      List<MenuButtonObject> buttons = menuButtons.Values.ToList();
      for (int i = 0; i < buttons.Count; ++i)
      {
        buttons[i].loadContent(buttonLabelFont, new Rectangle(menuBackgroundRect.Center.X - MENU_BUTTON_WIDTH / 2, (menuBackgroundRect.Y + 150) + (i * (MENU_BUTTON_HEIGHT + 10)), MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT), buttonTexture);
      }
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

      spriteBatch.End();

      foreach (MenuButtonObject button in menuButtons.Values)
      {
        button.RenderObject(spriteBatch);
      }
    }

    public override GameStateEnum processInput(GameTime gameTime)
    {
      mouseDevice.update(gameTime);

      if (nextState != GameStateEnum.MainMenu) isLeaving = true;

      return nextState;
    }

    public override void update(GameTime gameTime)
    {
      selectedButton = null;
      if (isLeaving)
      {
        nextState = GameStateEnum.MainMenu;
        isLeaving = false;
        return;
      }

      foreach (MenuButtonObject button in menuButtons.Values)
      {
        button.IsHighlighted = button.ObjectRectangle.Contains(currentMousePosition);
        if (button.IsHighlighted)
        {
          selectedButton = button.ButtonId;
        }
      }
    }

    private void newGameButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.LevelSelector;
      }
    }

    private void controlsButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.Controls;
      }
    }

    private void creditsButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.Credits;
      }
    }

    private void quitButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.Exit;
      }
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
