using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BigBlueIsYou
{
  internal class CreditsView : GameStateView
  {
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    public const int MENU_WIDTH = Constants.WINDOW_WIDTH / 3;
    public const int MENU_HEIGHT = Constants.WINDOW_HEIGHT / 10;
    public const int MENU_BUTTON_WIDTH = (int)(MENU_WIDTH / 1.5);
    public const int MENU_BUTTON_HEIGHT = (MENU_HEIGHT / 1) - 20;
    public const string TITLE = "Credits";
    public const string CREATORS = "Created by Ian and Isaac";
    public const string ASSISTANTED_BY = "Assisted by CHATGPT and Dean Mathias";
    public const string SOUND_SOURCE = "Sounds taken from FreeSound.org";
    public const string IMAGES_SOURCE = "Images taken from opengameart.org, spriters-resource.com and class files";
    private Dictionary<Guid, MenuButtonObject> menuButtons;
    private Dictionary<Guid, Action<GameTime, bool>> buttonActionsMap;
    private SpriteFont titleFont;
    private SpriteFont buttonLabelFont;
    private SpriteFont regularTextFont;
    private Texture2D menuBackground;
    private Texture2D buttonTexture;
    private Rectangle menuBackgroundRect = new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);
    private GameStateEnum nextState;
    private MouseDeviceInput mouseDevice;
    private Point currentMousePosition;
    private Guid? selectedButton;
    private bool isLeaving;
    private MenuButtonObject backButton;
    public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
      this.graphics = graphics;
      spriteBatch = new SpriteBatch(graphicsDevice);
      menuButtons = new Dictionary<Guid, MenuButtonObject>();
      buttonActionsMap = new Dictionary<Guid, Action<GameTime, bool>>();
      nextState = GameStateEnum.Credits;
      isLeaving = false;
      backButton = new MenuButtonObject("Back", Color.White, Color.Yellow, Color.Red);

      menuButtons.Add(backButton.ButtonId, backButton);

      buttonActionsMap.Add(backButton.ButtonId, backButtonAction);

      mouseDevice = new MouseDeviceInput();
      mouseDevice.setLeftMouseButtonAction(mouseLeftButtonAction);
      mouseDevice.setMouseCursorAction(mouseCursorAction);
    }

    public override void loadContent(ContentManager contentManager)
    {
      titleFont = contentManager.Load<SpriteFont>("Fonts/regFont");
      buttonLabelFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      regularTextFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      menuBackground = contentManager.Load<Texture2D>("Images/background");
      buttonTexture = contentManager.Load<Texture2D>("Images/stone-pattern");

      // Will always be a single button, but permits for expanding in the future when also updating y axis positioning as desired.
      List<MenuButtonObject> buttons = menuButtons.Values.ToList();
      for (int i = 0; i < buttons.Count; ++i)
      {
        buttons[i].loadContent(buttonLabelFont, new Rectangle(menuBackgroundRect.Center.X - MENU_BUTTON_WIDTH / 2, (Constants.WINDOW_HEIGHT - MENU_BUTTON_HEIGHT - 100), MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT), buttonTexture);
      }
    }

    public override GameStateEnum processInput(GameTime gameTime)
    {
      mouseDevice.update(gameTime);

      if (nextState != GameStateEnum.Credits) isLeaving = true;

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

      spriteBatch.DrawString(
          regularTextFont,
          CREATORS,
          new Vector2(Constants.WINDOW_WIDTH / 2 - (regularTextFont.MeasureString(CREATORS) / 2).X, MENU_HEIGHT * 2),
          Color.White,
          0.0f,
          new Vector2(0f, 0f),
          1f,
          SpriteEffects.None,
          0
      );

      spriteBatch.DrawString(
          regularTextFont,
          ASSISTANTED_BY,
          new Vector2(Constants.WINDOW_WIDTH / 2 - (regularTextFont.MeasureString(ASSISTANTED_BY) / 2).X, MENU_HEIGHT * 3),
          Color.White,
          0.0f,
          new Vector2(0f, 0f),
          1f,
          SpriteEffects.None,
          0
      );

      spriteBatch.DrawString(
          regularTextFont,
          IMAGES_SOURCE,
          new Vector2(Constants.WINDOW_WIDTH / 2 - (regularTextFont.MeasureString(IMAGES_SOURCE) / 2).X, MENU_HEIGHT * 4),
          Color.White,
          0.0f,
          new Vector2(0f, 0f),
          1f,
          SpriteEffects.None,
          0
      );

      spriteBatch.DrawString(
          regularTextFont,
          SOUND_SOURCE,
          new Vector2(Constants.WINDOW_WIDTH / 2 - (regularTextFont.MeasureString(SOUND_SOURCE) / 2).X, MENU_HEIGHT * 5),
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

    public override void update(GameTime gameTime)
    {
      selectedButton = null;
      if (isLeaving)
      {
        nextState = GameStateEnum.Credits;
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

    private void backButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.MainMenu;
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
