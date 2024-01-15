using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CS5410
{
    // View for the credits page
    public class CreditsView : GameStateView
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private KeyboardInput inputKeyboard;
        private bool didExit;
        private SpriteFont font;
        private Texture2D menuBackgroundTexture;

        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            didExit = false;
            spriteBatch = new SpriteBatch(graphicsDevice);
            inputKeyboard = new KeyboardInput();
            inputKeyboard.registerCommand(Keys.Escape, true, new InputDeviceHelper.CommandDelegate(onEscape));
            inputKeyboard.registerCommand(Keys.Enter, true, new InputDeviceHelper.CommandDelegate((GameTime gameTime) => { throw new NoSuitableGraphicsDeviceException(); }));
        }

        public override void loadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/regFont");
            menuBackgroundTexture = contentManager.Load<Texture2D>("Images/background");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            inputKeyboard.Update(gameTime);
            if (didExit) 
            {
                didExit = false;
                return GameStateEnum.MainMenu;
            }
            return GameStateEnum.Credits;
        }

        public override void render(GameTime gameTime)
        {
            spriteBatch.Begin();
            // Draw all text for credits. Simple code
            spriteBatch.Draw(menuBackgroundTexture, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.DrawString(
                font,
                "Credits",
                new Vector2(960 - (font.MeasureString("Credits") / 2).X, 50),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                1f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "Created by Isaac Airmet",
                new Vector2(960 - (font.MeasureString("Created by Isaac Airmet") / 2).X * .8f, 250),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "Art taken from opengameart.org and sounds from pixabay",
                new Vector2(960 - (font.MeasureString("Art taken from opengameart.org and sounds from pixabay") / 2).X * .8f, 400),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "HitBox Helper taken from form. Linked in code.",
                new Vector2(960 - (font.MeasureString("HitBox Helper taken from form. Linked in code.") / 2).X * .8f, 550),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "Some code adapted from Professor Dean Mathias",
                new Vector2(960 - (font.MeasureString("Some code adapted from Professor Dean Mathias") / 2).X * .8f, 700),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.8f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "[Escape to go back]",
                new Vector2(960 - (font.MeasureString("[Escape to go back]") / 2).X * .7f, 900),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.7f,
                SpriteEffects.None,
                0
            );

            spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            // Nothing reactive to update
        }

        private void onEscape(GameTime gameTime)
        {
            didExit = true;
        }
    }
}
