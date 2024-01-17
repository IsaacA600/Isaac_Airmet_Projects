using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BigBlueIsYou
{
  class ControlsView : GameStateView
  {
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    public const int MENU_WIDTH = Constants.WINDOW_WIDTH / 3;
    public const int MENU_BUTTON_WIDTH = (int)(MENU_WIDTH / 1.5);
    public const int MENU_HEIGHT = (int)(Constants.WINDOW_HEIGHT * 0.8);
    public const int MENU_BUTTON_HEIGHT = (MENU_HEIGHT / 7) - 20;
    public const string TITLE = "Custom Controls";
    public const string BINDINGS_ERROR = "Critical error loading/saving key bindings. Force close the application and try again";
    private Dictionary<Guid, MenuButtonObject> menuButtons;
    private Dictionary<Guid, Action<GameTime, bool>> buttonActionsMap;
    private Dictionary<MenuButtonObject, GamePlayActionEnum> buttonKeyMap;
    private SpriteFont titleFont;
    private SpriteFont buttonLabelFont;
    private SpriteFont regularTextFont;
    private Texture2D menuBackground;
    private Texture2D buttonTexture;
    private Rectangle menuBackgroundRect = new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);
    private GameStateEnum nextState;
    private MouseDeviceInput mouseDevice;
    private KeyboardInput inputKeyboard;
    private Point currentMousePosition;
    private Guid? selectedButton;
    private bool isLeaving;
    private bool shouldReload;
    private FileManager fileManager;
    private KeyBindingStorage keyBindings;
    private GamePlayActionEnum changingKey;
    private MenuButtonObject backButton;
    private MenuButtonObject changeMoveUpButton;
    private MenuButtonObject changeMoveDownButton;
    private MenuButtonObject changeMoveRightButton;
    private MenuButtonObject changeMoveLeftButton;
    private MenuButtonObject changeResetButton;
    private MenuButtonObject changeUndoButton;

    private Dictionary<GamePlayActionEnum, string> buttonActionStringMap;

    public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
      this.graphics = graphics;
      spriteBatch = new SpriteBatch(graphicsDevice);
      menuButtons = new Dictionary<Guid, MenuButtonObject>();
      buttonActionsMap = new Dictionary<Guid, Action<GameTime, bool>>();
      buttonKeyMap = new Dictionary<MenuButtonObject, GamePlayActionEnum>();
      nextState = GameStateEnum.Controls;
      isLeaving = false;
      shouldReload = true;
      fileManager = new FileManager();
      keyBindings = new KeyBindingStorage();
      changingKey = GamePlayActionEnum.None;
      buttonActionStringMap = new Dictionary<GamePlayActionEnum, string>();

      changeMoveUpButton = new MenuButtonObject("Change Move Up Key", Color.White, Color.Yellow, Color.Red);
      changeMoveDownButton = new MenuButtonObject("Change Move Down Key", Color.White, Color.Yellow, Color.Red);
      changeMoveRightButton = new MenuButtonObject("Change Move Right Key", Color.White, Color.Yellow, Color.Red);
      changeMoveLeftButton = new MenuButtonObject("Change Move Left Key", Color.White, Color.Yellow, Color.Red);
      changeResetButton = new MenuButtonObject("Change Reset Key", Color.White, Color.Yellow, Color.Red);
      changeUndoButton = new MenuButtonObject("Change Undo Key", Color.White, Color.Yellow, Color.Red);
      backButton = new MenuButtonObject("Back", Color.White, Color.Yellow, Color.Red);

      buttonActionStringMap.Add(GamePlayActionEnum.MoveUp, "Move Up");
      buttonActionStringMap.Add(GamePlayActionEnum.MoveDown, "Move Down");
      buttonActionStringMap.Add(GamePlayActionEnum.MoveRight, "Move Right");
      buttonActionStringMap.Add(GamePlayActionEnum.MoveLeft, "Move Left");
      buttonActionStringMap.Add(GamePlayActionEnum.Reset, "Reset");
      buttonActionStringMap.Add(GamePlayActionEnum.Undo, "Undo");

      backButton.setContainer(new Rectangle(menuBackgroundRect.Center.X - MENU_BUTTON_WIDTH / 2, (Constants.WINDOW_HEIGHT - MENU_BUTTON_HEIGHT - 100), MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT));

      menuButtons.Add(changeMoveUpButton.ButtonId, changeMoveUpButton);
      menuButtons.Add(changeMoveDownButton.ButtonId, changeMoveDownButton);
      menuButtons.Add(changeMoveLeftButton.ButtonId, changeMoveLeftButton);
      menuButtons.Add(changeMoveRightButton.ButtonId, changeMoveRightButton);
      menuButtons.Add(changeResetButton.ButtonId, changeResetButton);
      menuButtons.Add(changeUndoButton.ButtonId, changeUndoButton);
      menuButtons.Add(backButton.ButtonId, backButton);

      buttonKeyMap.Add(changeMoveUpButton, GamePlayActionEnum.MoveUp);
      buttonKeyMap.Add(changeMoveDownButton, GamePlayActionEnum.MoveDown);
      buttonKeyMap.Add(changeMoveLeftButton, GamePlayActionEnum.MoveLeft);
      buttonKeyMap.Add(changeMoveRightButton, GamePlayActionEnum.MoveRight);
      buttonKeyMap.Add(changeResetButton, GamePlayActionEnum.Reset);
      buttonKeyMap.Add(changeUndoButton, GamePlayActionEnum.Undo);

      buttonActionsMap.Add(changeMoveUpButton.ButtonId, changeMoveUpButtonAction);
      buttonActionsMap.Add(changeMoveDownButton.ButtonId, changeMoveDownButtonAction);
      buttonActionsMap.Add(changeMoveLeftButton.ButtonId, changeMoveLeftButtonAction);
      buttonActionsMap.Add(changeMoveRightButton.ButtonId, changeMoveRightButtonAction);
      buttonActionsMap.Add(changeResetButton.ButtonId, changeResetButtonAction);
      buttonActionsMap.Add(changeUndoButton.ButtonId, changeUndoButtonAction);
      buttonActionsMap.Add(backButton.ButtonId, backButtonAction);

      mouseDevice = new MouseDeviceInput();
      mouseDevice.setLeftMouseButtonAction(mouseLeftButtonAction);
      mouseDevice.setMouseCursorAction(mouseCursorAction);

      inputKeyboard = new KeyboardInput();
    }

    public override void loadContent(ContentManager contentManager)
    {
      titleFont = contentManager.Load<SpriteFont>("Fonts/regFont");
      buttonLabelFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      regularTextFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      menuBackground = contentManager.Load<Texture2D>("Images/background");
      buttonTexture = contentManager.Load<Texture2D>("Images/stone-pattern");

      foreach (KeyValuePair<MenuButtonObject, GamePlayActionEnum> entry in buttonKeyMap)
      {
        entry.Key.loadContent(buttonLabelFont, buttonTexture);
      }
      backButton.loadContent(buttonLabelFont, buttonTexture);
    }

    public override GameStateEnum processInput(GameTime gameTime)
    {
      mouseDevice.update(gameTime);

      if (changingKey != GamePlayActionEnum.None)
      {
        inputKeyboard.listen(gameTime, getNewKey);
      }

      if (nextState != GameStateEnum.Controls) isLeaving = true;

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

      if (keyBindings.isLoadingError || keyBindings.isSavingError)
      {
        renderErrorMessage();
        return;
      }
      spriteBatch.End();
      if (keyBindings.keyBindings != null)
      {
        renderKeys();
      }
    }

    private void renderKeys()
    {
      int i = 0;
      foreach (KeyValuePair<MenuButtonObject, GamePlayActionEnum> entry in buttonKeyMap)
      {
        entry.Key.setContainer(new Rectangle(menuBackgroundRect.Center.X - MENU_BUTTON_WIDTH / 2, (200) + (i * (MENU_BUTTON_HEIGHT + 10)), MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT));
        if (changingKey == entry.Value) entry.Key.updateActiveLabel($"Select New {buttonActionStringMap[changingKey]} Key:", Color.Yellow);
        else entry.Key.updateActiveLabel(entry.Key.Label, Color.White);
        entry.Key.RenderObject(spriteBatch);
        string keyValue = "";
        if (entry.Value == GamePlayActionEnum.MoveUp) keyValue = keyBindings.keyBindings[0].ToString();
        if (entry.Value == GamePlayActionEnum.MoveRight) keyValue = keyBindings.keyBindings[1].ToString();
        if (entry.Value == GamePlayActionEnum.MoveDown) keyValue = keyBindings.keyBindings[2].ToString();
        if (entry.Value == GamePlayActionEnum.MoveLeft) keyValue = keyBindings.keyBindings[3].ToString();
        if (entry.Value == GamePlayActionEnum.Reset) keyValue = keyBindings.keyBindings[4].ToString();
        if (entry.Value == GamePlayActionEnum.Undo) keyValue = keyBindings.keyBindings[5].ToString();
        spriteBatch.Begin();
        spriteBatch.DrawString(
            regularTextFont,
            keyValue,
            new Vector2(Constants.WINDOW_WIDTH / 2 + MENU_BUTTON_WIDTH / 2 + 25, (200) + (i * (MENU_BUTTON_HEIGHT + 10)) + (MENU_BUTTON_HEIGHT / 4)),
            Color.Red,
            0.0f,
            new Vector2(0f, 0f),
            1f,
            SpriteEffects.None,
            0
        );
        spriteBatch.End();
        i++;
      }
      backButton.RenderObject(spriteBatch);
    }

    private void renderErrorMessage()
    {
      spriteBatch.DrawString(
          regularTextFont,
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
      if (shouldReload)
      {
        fileManager.startLoadingKeyBindings(Constants.KEY_BINDING_FILE, keyBindings);
        shouldReload = false;
      }
      keyBindings = fileManager.getCurrentBindings();

      if (isLeaving)
      {
        fileManager.startSavingKeyBindings(Constants.KEY_BINDING_FILE, keyBindings);
        nextState = GameStateEnum.Controls;
        changingKey = GamePlayActionEnum.None;
        isLeaving = false;
        shouldReload = true;
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

    private void changeMoveDownButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        changingKey = GamePlayActionEnum.MoveDown;
      }
    }

    private void changeMoveUpButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        changingKey = GamePlayActionEnum.MoveUp;
      }
    }

    private void changeMoveLeftButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        changingKey = GamePlayActionEnum.MoveLeft;
      }
    }

    private void changeMoveRightButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        changingKey = GamePlayActionEnum.MoveRight;
      }
    }

    private void changeUndoButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        changingKey = GamePlayActionEnum.Undo;
      }
    }

    private void changeResetButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        changingKey = GamePlayActionEnum.Reset;
      }
    }

    private void getNewKey(GameTime gameTime, Keys key)
    {
      if (changingKey == GamePlayActionEnum.MoveUp) keyBindings.keyBindings[0] = key;
      if (changingKey == GamePlayActionEnum.MoveRight) keyBindings.keyBindings[1] = key;
      if (changingKey == GamePlayActionEnum.MoveDown) keyBindings.keyBindings[2] = key;
      if (changingKey == GamePlayActionEnum.MoveLeft) keyBindings.keyBindings[3] = key;
      if (changingKey == GamePlayActionEnum.Reset) keyBindings.keyBindings[4] = key;
      if (changingKey == GamePlayActionEnum.Undo) keyBindings.keyBindings[5] = key;

      changingKey = GamePlayActionEnum.None;
    }

    private void mouseLeftButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        if (selectedButton != null)
        {
          if (menuButtons[selectedButton.Value].ObjectRectangle.Contains(currentMousePosition))
          {
            buttonActionsMap[selectedButton.Value](gameTime, alreadyPressed);
          }
        }
        else
        {
          changingKey = GamePlayActionEnum.None;
        }
      }
    }

    private void mouseCursorAction(GameTime gameTime, Point cursorPosition)
    {
      currentMousePosition = cursorPosition;
    }
  }
}
