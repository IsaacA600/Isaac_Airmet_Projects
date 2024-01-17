using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BigBlueIsYou.Entities;
using BigBlueIsYou.Systems;
using BigBlueIsYou.Components;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using BigBlueIsYou.Utils;
using BigBlueIsYou.Particles;

namespace BigBlueIsYou
{
  public class GamePlayView : GameStateView
  {
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;
    public const int MENU_WIDTH = Constants.WINDOW_WIDTH / 4;
    public const int MENU_BUTTON_WIDTH = (int)(MENU_WIDTH / 1.5);
    public const int MENU_HEIGHT = (int)(Constants.WINDOW_HEIGHT / 4);
    public const int MENU_BUTTON_HEIGHT = (MENU_HEIGHT / 2) - 20;
    private Dictionary<Guid, MenuButtonObject> menuButtons;
    private Dictionary<Guid, Action<GameTime, bool>> buttonActionsMap;
    private GridManager gridManager;
    private SpriteFont buttonLabelFont;
    private Texture2D buttonTexture;
    private Texture2D menuTexture;
    private Song backgroundSong;
    private SoundEffect onWinSound;
    private SoundEffect onMoveSound;
    private SoundEffect onWinConditionChangeSound;
    private Rectangle screenRectangle = new Rectangle(0, 0, Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);
    private Rectangle pauseBackgroundRectangle;
    private GameStateEnum nextState;
    private MouseDeviceInput mouseDevice;
    private KeyboardInput inputKeyboard;
    private Point currentMousePosition;
    private Guid? selectedButton;
    private bool isLeaving;
    private bool isPaused;
    private bool Won;
    private bool isInitialLaunch;
    private bool isMoving;
    private KeyBindingStorage keyBindings;
    private MenuButtonObject backButton;
    private MenuButtonObject resumeButton;
    private KeysStorer keysStorer;
    private LevelDataContainer levelData;
    private ISystem inputSystem;
    private ISystem renderSpritesSystem;
    private ISystem rulesSystem;
    private ISystem spriteSystem;
    private ISystem movementSystem;
    private ContentManager contentManager;
    private Direction currentMovingDirection;
    private double secondsMovingInDirection;
    private TimeSpan winDelay;
    private ParticleSystem particleSystem;

    public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
    {
      this.graphics = graphics;
      spriteBatch = new SpriteBatch(graphicsDevice);
      menuButtons = new Dictionary<Guid, MenuButtonObject>();
      buttonActionsMap = new Dictionary<Guid, Action<GameTime, bool>>();
      nextState = GameStateEnum.GamePlay;
      keyBindings = new KeyBindingStorage();
      isLeaving = false;
      isPaused = false;
      Won = false;
      winDelay = TimeSpan.Zero;
      isInitialLaunch = true;
      isMoving = false;
      currentMovingDirection = Direction.Still;
      secondsMovingInDirection = 0;
      gridManager = GridManager.GetInstance();
      resumeButton = new MenuButtonObject("Resume", Color.White, Color.Yellow, Color.Red);
      backButton = new MenuButtonObject("Exit", Color.White, Color.Yellow, Color.Red);
      particleSystem = new ParticleSystem();

      menuButtons.Add(resumeButton.ButtonId, resumeButton);
      menuButtons.Add(backButton.ButtonId, backButton);

      buttonActionsMap.Add(resumeButton.ButtonId, resumeButtonAction);
      buttonActionsMap.Add(backButton.ButtonId, backButtonAction);

      mouseDevice = new MouseDeviceInput();
      mouseDevice.setLeftMouseButtonAction(mouseLeftButtonAction);
      mouseDevice.setMouseCursorAction(mouseCursorAction);

      inputKeyboard = new KeyboardInput();

      inputSystem = new InputSystem(inputKeyboard);
      renderSpritesSystem = new RenderSpritesSystem(spriteBatch);
      rulesSystem = new RulesSystem(particleSystem);
      spriteSystem = new SpriteSystem();
      movementSystem = new MovementSystem(particleSystem);
    }

    public override void loadContent(ContentManager contentManager)
    {
      this.contentManager = contentManager;
      buttonLabelFont = contentManager.Load<SpriteFont>("Fonts/buttonFont");
      menuTexture = contentManager.Load<Texture2D>("Images/background");
      buttonTexture = contentManager.Load<Texture2D>("Images/stone-pattern");
      backgroundSong = contentManager.Load<Song>("Sounds/music");
      onWinConditionChangeSound = contentManager.Load<SoundEffect>("Sounds/zapping");
      onMoveSound = contentManager.Load<SoundEffect>("Sounds/quick-step");
      onWinSound = contentManager.Load<SoundEffect>("Sounds/trumpet");
      onWinConditionChangeSound = contentManager.Load<SoundEffect>("Sounds/changeWin");

      particleSystem.LoadContent(contentManager.Load<Texture2D>("Images/sparkle"));
      movementSystem.LoadContent(onMoveSound);
      rulesSystem.LoadContent(this.contentManager, onWinConditionChangeSound);

      List<MenuButtonObject> buttons = menuButtons.Values.ToList();
      for (int i = 0; i < buttons.Count; ++i)
      {
        buttons[i].loadContent(buttonLabelFont, new Rectangle(screenRectangle.Center.X - MENU_BUTTON_WIDTH / 2, (screenRectangle.Y + 150) + (i * (MENU_BUTTON_HEIGHT + 10)), MENU_BUTTON_WIDTH, MENU_BUTTON_HEIGHT), buttonTexture);
      }
    }

    private void loadGrid(ContentManager contentManager)
    {
      gridManager.ParseLevelGrid(levelData, contentManager);
      rulesSystem.Update(null);
      updateSystems();
    }

    private void updateSystems()
    {
      inputSystem.RemoveAllEntities();
      renderSpritesSystem.RemoveAllEntities();
      rulesSystem.RemoveAllEntities();
      spriteSystem.RemoveAllEntities();

      foreach (List<IEntity> entities in gridManager.GetCurrentGrid())
      {
        foreach (IEntity entity in entities)
        {
          inputSystem.AddEntity(entity);
          renderSpritesSystem.AddEntity(entity);
          rulesSystem.AddEntity(entity);
          spriteSystem.AddEntity(entity);
        }
      }
    }

    public override GameStateEnum processInput(GameTime gameTime)
    {
      if (isInitialLaunch)
      {
        return nextState;
      }

      if (isPaused)
      {
        mouseDevice.update(gameTime);
      }
      else if (Won)
      {
        if (winDelay >= onWinSound.Duration)
        {
          nextState = GameStateEnum.LevelSelector;
        }
      }
      else
      {
        isMoving = false;
        inputSystem.Update(gameTime);
      }

      if (nextState != GameStateEnum.GamePlay)
      {
        isLeaving = true;
      }

      return nextState;
    }

    public override void render(GameTime gameTime)
    {
      if (isLeaving)
      {
        return;
      }

      renderSpritesSystem.Update(gameTime);
      particleSystem.Render(spriteBatch);

      if (isPaused)
      {
        pauseBackgroundRectangle = new Rectangle(screenRectangle.Center.X - MENU_BUTTON_WIDTH / 2 - 50, screenRectangle.Y + 100, MENU_BUTTON_WIDTH + 100, menuButtons.Values.ToList().Count * (MENU_BUTTON_HEIGHT + 20) + 50);
        spriteBatch.Begin();
        spriteBatch.Draw(menuTexture, pauseBackgroundRectangle, Color.White);
        spriteBatch.End();
        foreach (MenuButtonObject button in menuButtons.Values)
        {
          button.RenderObject(spriteBatch);
        }
      }
    }

    public override void update(GameTime gameTime)
    {
      if (isLeaving)
      {
        performEndingActions();
        return;
      }

      if (isInitialLaunch)
      {
        performInitialActions();
      }

      if (isPaused)
      {
        handlePause();
        return;
      }

      if (Won)
      {
        winDelay += gameTime.ElapsedGameTime;
      }

      bool didWin = movementSystem.Update(gameTime);
      bool specialWin = rulesSystem.Update(gameTime);
      spriteSystem.Update(gameTime);
      particleSystem.Update(gameTime);
      updateSystems();

      if (!Won && (didWin || specialWin))
      {
        Won = true;
        onWinSound.Play();
      }

      if (!isMoving)
      {
        currentMovingDirection = Direction.Still;
      }
    }

    private void performInitialActions()
    {
      isInitialLaunch = false;
      keysStorer = KeysStorer.getKeysStorer();
      keyBindings = keysStorer.getKeys();
      levelData = LevelStorer.getLevelStorer().getActiveLevel();
      inputKeyboard.registerCommand(Keys.Escape, false, new InputDeviceHelper.CommandDelegate(onEscape));
      inputKeyboard.registerEntityBasedCommand(keyBindings.keyBindings[0], false, new InputDeviceHelper.EntityUpdateDelegate(onUp));
      inputKeyboard.registerEntityBasedCommand(keyBindings.keyBindings[1], false, new InputDeviceHelper.EntityUpdateDelegate(onRight));
      inputKeyboard.registerEntityBasedCommand(keyBindings.keyBindings[2], false, new InputDeviceHelper.EntityUpdateDelegate(onDown));
      inputKeyboard.registerEntityBasedCommand(keyBindings.keyBindings[3], false, new InputDeviceHelper.EntityUpdateDelegate(onLeft));
      inputKeyboard.registerCommand(keyBindings.keyBindings[4], true, new InputDeviceHelper.CommandDelegate(onReset));
      inputKeyboard.registerCommand(keyBindings.keyBindings[5], true, new InputDeviceHelper.CommandDelegate(onUndo));
      loadGrid(contentManager);
      particleSystem.ClearEmitters();

      MediaPlayer.Play(backgroundSong);
      MediaPlayer.IsRepeating = true;
    }

    private void handlePause()
    {
      MediaPlayer.Pause();
      selectedButton = null;
      foreach (MenuButtonObject button in menuButtons.Values)
      {
        button.IsHighlighted = button.ObjectRectangle.Contains(currentMousePosition);
        if (button.IsHighlighted)
        {
          selectedButton = button.ButtonId;
        }
      }
    }

    private void performEndingActions()
    {
      nextState = GameStateEnum.GamePlay;

      isLeaving = false;
      isPaused = false;
      Won = false;
      winDelay = TimeSpan.Zero;
      isInitialLaunch = true;
      isMoving = false;

      currentMovingDirection = Direction.Still;
      secondsMovingInDirection = 0;
      inputKeyboard = new KeyboardInput();

      particleSystem.ClearEmitters();
      inputSystem.RemoveAllEntities();
      renderSpritesSystem.RemoveAllEntities();
      spriteSystem.RemoveAllEntities();
      movementSystem.RemoveAllEntities();
      rulesSystem.RemoveAllEntities();

      MediaPlayer.Stop();
    }

    private void backButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        nextState = GameStateEnum.LevelSelector;
        isLeaving = true;
      }
    }

    private void resumeButtonAction(GameTime gameTime, bool alreadyPressed)
    {
      if (!alreadyPressed)
      {
        isPaused = false;
        MediaPlayer.Resume();
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

    private void onEscape(GameTime gameTime)
    {
      isPaused = true;
    }

    private void onReset(GameTime gameTime)
    {
      gridManager.Reset();
      rulesSystem.Update(null);
      updateSystems();
    }

    private void onUndo(GameTime gameTime)
    {
      gridManager.Undo();
      rulesSystem.Update(null);
      updateSystems();
    }

    private void onUp(GameTime gameTime, List<IEntity> entites)
    {
      isMoving = true;
      if (currentMovingDirection == Direction.Up)
      {
        secondsMovingInDirection += gameTime.ElapsedGameTime.TotalSeconds;
        if (secondsMovingInDirection > 0.25)
        {
          secondsMovingInDirection -= 0.25;
          foreach (IEntity entity in entites)
          {
            entity.AddComponent(new MoveComponent(Direction.Up));
            movementSystem.AddEntity(entity);
          }
        }
      }
      else
      {
        currentMovingDirection = Direction.Up;
        secondsMovingInDirection = 0.0;
        foreach (IEntity entity in entites)
        {
          entity.AddComponent(new MoveComponent(Direction.Up));
          movementSystem.AddEntity(entity);
        }
      }
    }

    private void onDown(GameTime gameTime, List<IEntity> entites)
    {
      isMoving = true;
      if (currentMovingDirection == Direction.Down)
      {
        secondsMovingInDirection += gameTime.ElapsedGameTime.TotalSeconds;
        if (secondsMovingInDirection > 0.25)
        {
          secondsMovingInDirection -= 0.25;
          foreach (IEntity entity in entites)
          {
            entity.AddComponent(new MoveComponent(Direction.Down));
            movementSystem.AddEntity(entity);
          }
        }
      }
      else
      {
        currentMovingDirection = Direction.Down;
        secondsMovingInDirection = 0.0;
        foreach (IEntity entity in entites)
        {
          entity.AddComponent(new MoveComponent(Direction.Down));
          movementSystem.AddEntity(entity);
        }
      }
    }

    private void onRight(GameTime gameTime, List<IEntity> entites)
    {
      isMoving = true;
      if (currentMovingDirection == Direction.Right)
      {
        secondsMovingInDirection += gameTime.ElapsedGameTime.TotalSeconds;
        if (secondsMovingInDirection > 0.25)
        {
          secondsMovingInDirection -= 0.25;
          foreach (IEntity entity in entites)
          {
            entity.AddComponent(new MoveComponent(Direction.Right));
            movementSystem.AddEntity(entity);
          }
        }
      }
      else
      {
        currentMovingDirection = Direction.Right;
        secondsMovingInDirection = 0.0;
        foreach (IEntity entity in entites)
        {
          entity.AddComponent(new MoveComponent(Direction.Right));
          movementSystem.AddEntity(entity);
        }
      }
    }

    private void onLeft(GameTime gameTime, List<IEntity> entites)
    {
      isMoving = true;
      if (currentMovingDirection == Direction.Left)
      {
        secondsMovingInDirection += gameTime.ElapsedGameTime.TotalSeconds;
        if (secondsMovingInDirection > 0.25)
        {
          secondsMovingInDirection -= 0.25;
          foreach (IEntity entity in entites)
          {
            entity.AddComponent(new MoveComponent(Direction.Left));
            movementSystem.AddEntity(entity);
          }
        }
      }
      else
      {
        currentMovingDirection = Direction.Left;
        secondsMovingInDirection = 0.0;
        foreach (IEntity entity in entites)
        {
          entity.AddComponent(new MoveComponent(Direction.Left));
          movementSystem.AddEntity(entity);
        }
      }
    }
  }
}
