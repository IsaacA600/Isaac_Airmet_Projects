using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CS5410
{
    // Main menu displayer
    public class MainMenuView : GameStateView
    {
        // Which direction are we moving in the menu?
        private enum MoveDirection {
            moveUp,
            moveDown,
            none
        }
        // Which item is currently selected?
        private enum CurrentSelectedItem {
            newGame,
            highScores,
            credits
        }
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private KeyboardInput inputKeyboard;
        private SpriteFont font;
        private Texture2D menuBackgroundTexture;
        private Texture2D selectorStar;
        private bool didExit;
        private bool didSelect;
        private MoveDirection moveDirection;
        private CurrentSelectedItem currentSelectedItem;

        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphicsDevice);
            inputKeyboard = new KeyboardInput();
            inputKeyboard.registerCommand(Keys.Up, true, new InputDeviceHelper.CommandDelegate(onMoveUp));
            inputKeyboard.registerCommand(Keys.Down, true, new InputDeviceHelper.CommandDelegate(onMoveDown));
            inputKeyboard.registerCommand(Keys.Escape, true, new InputDeviceHelper.CommandDelegate(onEscape));
            inputKeyboard.registerCommand(Keys.Enter, true, new InputDeviceHelper.CommandDelegate(onSelect));
            didExit = false;
            didSelect = false;
            moveDirection = MoveDirection.none;
            currentSelectedItem = CurrentSelectedItem.newGame;
        }

        public override void loadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/regFont");
            selectorStar = contentManager.Load<Texture2D>("Images/star");
            menuBackgroundTexture = contentManager.Load<Texture2D>("Images/background");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            inputKeyboard.Update(gameTime);
            if (didExit) return GameStateEnum.Exit;
            // See what we selected from the menu
            if (didSelect) 
            {
                didSelect = false;
                moveDirection = MoveDirection.none;
                if (currentSelectedItem == CurrentSelectedItem.newGame) return GameStateEnum.GamePlay;
                else if (currentSelectedItem == CurrentSelectedItem.highScores) return GameStateEnum.HighScores;
                else if (currentSelectedItem == CurrentSelectedItem.credits) return GameStateEnum.Credits;
            }
            return GameStateEnum.MainMenu;
        }

        public override void render(GameTime gameTime)
        {
            // Simple text render
            spriteBatch.Begin();

            spriteBatch.Draw(menuBackgroundTexture, new Rectangle(0, 0, 1920, 1080), Color.White);

            spriteBatch.DrawString(
                font,
                "Breakout!",
                new Vector2(960 - (font.MeasureString("Breakout!") / 2).X, 50),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                1f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "New Game",
                new Vector2(960 - (font.MeasureString("New Game") / 2).X * .8f, 200),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "High Scores",
                new Vector2(960 - (font.MeasureString("High Scores") / 2).X * .8f, 400),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "Credits",
                new Vector2(960 - (font.MeasureString("Credits") / 2).X * .8f, 600),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "[Press Escape to Exit]",
                new Vector2(960 - (font.MeasureString("[Press Escape to Exit]") / 2).X * .7f, 900),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.7f,
                SpriteEffects.None,
                0
            );

            // Star highlighter
            highLightSelected();

            spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            // What are we currently selecting?
            if (moveDirection == MoveDirection.moveDown) 
            {
                if (currentSelectedItem != CurrentSelectedItem.credits) {
                    currentSelectedItem = currentSelectedItem == CurrentSelectedItem.newGame ? CurrentSelectedItem.highScores : CurrentSelectedItem.credits;
                }
            } else if (moveDirection == MoveDirection.moveUp) 
            {
                if (currentSelectedItem != CurrentSelectedItem.newGame) {
                    currentSelectedItem = currentSelectedItem == CurrentSelectedItem.highScores ? CurrentSelectedItem.newGame : CurrentSelectedItem.highScores;
                }
            }
            moveDirection = MoveDirection.none;
        }

        // Highlight the current item with a star
        private void highLightSelected() 
        {
            Dictionary<CurrentSelectedItem, int> starLocationy = new Dictionary<CurrentSelectedItem, int>
            {
                { CurrentSelectedItem.newGame, 200},
                { CurrentSelectedItem.highScores, 400},
                { CurrentSelectedItem.credits, 600 },
            };
            Dictionary<CurrentSelectedItem, int> starLocationx = new Dictionary<CurrentSelectedItem, int>
            {
                { CurrentSelectedItem.newGame, (int)(960 - (font.MeasureString("New Game") / 2).X - 100)},
                { CurrentSelectedItem.highScores, (int)(960 - (font.MeasureString("High Scores") / 2).X - 100)},
                { CurrentSelectedItem.credits, (int)(960 - (font.MeasureString("Credits") / 2).X - 100) },
            };

            Rectangle starBox = new Rectangle(starLocationx[currentSelectedItem], starLocationy[currentSelectedItem] - 20, 80, 80);
            spriteBatch.Draw(selectorStar, starBox, Color.White);
        }

        // Keyboard function when moving down the menu
        private void onMoveDown(GameTime gameTime) 
        {
            moveDirection = MoveDirection.moveDown;
        }

        // Keyboard function when escaping the menu (close game)
        private void onEscape(GameTime gameTime) 
        {
            didExit = true;
        }

        // Keyboard function when moving up the menu
        private void onMoveUp(GameTime gameTime) 
        {
            moveDirection = MoveDirection.moveUp;
        }

        // Keyboard function when selecting an item
        private void onSelect(GameTime gameTime)
        {
            didSelect = true;
        }
    }
}
