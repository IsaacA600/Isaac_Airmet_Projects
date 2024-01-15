using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace CS5410
{
    // Displays the high scores page
    public class HighScoresView : GameStateView
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        private SpriteFont font;
        private Texture2D menuBackgroundTexture;
        private KeyboardInput inputKeyboard;
        private bool didExit;
        private bool shouldReset;
        private bool loadingError;
        private bool savingError;
        private bool resetError;
        private bool loading;
        private bool saving;
        private bool reseting;
        private bool justEntered;
        private HighScoreStorage highScores;

        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphicsDevice);
            inputKeyboard = new KeyboardInput();
            inputKeyboard.registerCommand(Keys.Escape, true, new InputDeviceHelper.CommandDelegate(onEscape));
            inputKeyboard.registerCommand(Keys.R, true, new InputDeviceHelper.CommandDelegate(onReset));
            didExit = false;
            shouldReset = false;
            resetError = false;
            loadingError = false;
            savingError = false;
            loading = false;
            saving = false;
            reseting = false;
            highScores = null;
            justEntered = false;
        }

        public override void loadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/regFont");
            menuBackgroundTexture = contentManager.Load<Texture2D>("Images/background");
            startLoadingScores();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            inputKeyboard.Update(gameTime);
            if (didExit)
            {
                return GameStateEnum.MainMenu;
            }
            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(menuBackgroundTexture, new Rectangle(0, 0, 1920, 1080), Color.White);

            spriteBatch.DrawString(
                font,
                "High Scores",
                new Vector2(960 - (font.MeasureString("High Scores") / 2).X, 50),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                1f,
                SpriteEffects.None,
                0
            );
            // Render the scores
            renderScores();

            spriteBatch.DrawString(
                font,
                "[Press R to reset -- Escape to go back]",
                new Vector2(960 - (font.MeasureString("[Press R to reset -- Escape to go back]") / 2).X * .7f, 900),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.7f,
                SpriteEffects.None,
                0
            );

            // Display error message if we can't save the reset scores
            if (savingError)
            {
                spriteBatch.DrawString(font, "Reset Failed! Contact Support", new Vector2(860, 1000), Color.White);
            }

            spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            // If we need to exit, don't update anything besides resetting class vars
            if (didExit)
            {
                resetVars();
                return;
            }

            // On new entry, load scores as they might have changed from last time
            if (!justEntered)
            {
                justEntered = true;
                startLoadingScores();
            }

            // If asked to reset, reset scores
            if (shouldReset)
            {
                shouldReset = false;
                highScores = new HighScoreStorage(new List<int>());
                for (int i = 0; i < 5; i++)
                {
                    highScores.highScores.Add(0);
                }
                startSavingScores();
            }
        }

        // Keyboard function when escape key is pressed
        private void onEscape(GameTime gameTime)
        {
            didExit = true;
        }

        // Keyboard function when R is pressed
        private void onReset(GameTime gameTime)
        {
            savingError = false;
            shouldReset = true;
        }

        // Render the current high scores
        private void renderScores() 
        {
            // Make sure there wasn't an error loading tem
            if (loadingError)
            {
                spriteBatch.DrawString(font, "ERROR. Contact Support", new Vector2(860, 200), Color.White);
                return;
            }
            
            string score1 = "1. ";
            string score2 = "2. ";
            string score3 = "3. ";
            string score4 = "4. ";
            string score5 = "5. ";

            if (highScores != null)
            {
                score1 += highScores.highScores[0];
                score2 += highScores.highScores[1];
                score3 += highScores.highScores[2];
                score4 += highScores.highScores[3];
                score5 += highScores.highScores[4];
            } else {
                score1 += "No Score Available";
                score2 += "No Score Available";
                score3 += "No Score Available";
                score4 += "No Score Available";
                score5 += "No Score Available";
            }

            string scores = score1 + "\n\n" + score2 + "\n\n" + score3 + "\n\n" + score4 + "\n\n" + score5;
            spriteBatch.DrawString(font, scores, new Vector2(860, 200), Color.White);
        }

        // Start saving the reset scores
        private void startSavingScores()
        {
            lock (this)
            {
                if (!saving)
                {
                    saving = true;
                    saveHighScores();
                }
            }
        }

        // Save new scores of 0
        private async void saveHighScores()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores5.xml", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(HighScoreStorage));
                                mySerializer.Serialize(fs, highScores);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        savingError = true;
                    }
                }
                this.saving = false;
            });
        }

        // Start loading current high scores
        private void startLoadingScores()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    getHighScores();
                }
            }
        }

        // Get the current high scores
        public async void getHighScores()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("HighScores5.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores5.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(HighScoreStorage));
                                    highScores = (HighScoreStorage)mySerializer.Deserialize(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        loadingError = true;
                    }
                }
                this.loading = false;
            });
        }

        // Reset class vars when leaving
        private void resetVars()
        {
            didExit = false;
            shouldReset = false;
            resetError = false;
            loadingError = false;
            savingError = false;
            loading = false;
            saving = false;
            reseting = false;
            highScores = null;
            justEntered = false;
        }
    }
}
