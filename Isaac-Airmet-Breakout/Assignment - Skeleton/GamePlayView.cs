using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        private SpriteFont font;
        private BallManager ballManager;
        private BrickManager brickManager;
        private PaddleManager paddleManager;
        private ParticleManager particleManager;
        private BallRenderer ballRenderer;
        private BrickRenderer brickRenderer;
        private PaddleRenderer paddleRenderer;
        private ParticleRenderer particleRenderer;
        private SoundEffect brickHitSound;
        private Song backgroundSong;
        private Texture2D ballTexture;
        private Texture2D blueBrickTexture;
        private Texture2D greenBrickTexture;
        private Texture2D orangeBrickTexture;
        private Texture2D yellowBrickTexture;
        private Texture2D paddleTexture;
        private Texture2D miniPaddleTexture;
        private Texture2D smokeTexture;
        private Texture2D starTexture;
        private Texture2D gameBackgroundTexture;
        private Texture2D menuBackgroundTexture;
        private KeyboardInput gameplayKeyboard;
        private KeyboardInput pauseMenuKeyboard;
        private KeyboardInput endGameKeyboard;
        private bool moveLeft;
        private bool moveRight;
        private bool pause;
        private bool initalUpdate;
        private bool initalRender;
        private bool hoverExitMenu;
        private bool hoverContinueMenu;
        private bool selectMenuOption;
        private int currentScore;
        private bool isGameOver;
        private bool didWinGame;
        private bool loadingScores;
        private bool savingScores;
        private bool loadingError;
        private bool savingError;
        private bool shouldExitNow;
        private HighScoreStorage highScores;
        private double secondsWaiting;
        private bool preGame;
        private IDictionary<Ball, int> bricksHitCnt;
        private int ballsAdded;
        private bool showEndScreen;
        private bool isPaddleDisapearing;
        private bool isPaddleShrinking;

        public override void initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            spriteBatch = new SpriteBatch(graphicsDevice);
            ballManager = new BallManager();
            brickManager = new BrickManager();
            paddleManager = new PaddleManager();
            particleManager = new ParticleManager();
            brickRenderer = new BrickRenderer(brickManager);
            ballRenderer = new BallRenderer(ballManager);
            paddleRenderer = new PaddleRenderer(paddleManager);
            particleRenderer = new ParticleRenderer(particleManager);
            gameplayKeyboard = new KeyboardInput();
            pauseMenuKeyboard = new KeyboardInput();
            endGameKeyboard = new KeyboardInput();
            gameplayKeyboard.registerCommand(Keys.Left, false, new InputDeviceHelper.CommandDelegate(onMoveLeft));
            gameplayKeyboard.registerCommand(Keys.Right, false, new InputDeviceHelper.CommandDelegate(onMoveRight));
            gameplayKeyboard.registerCommand(Keys.Escape, true, new InputDeviceHelper.CommandDelegate(onEscape));
            pauseMenuKeyboard.registerCommand(Keys.Left, true, new InputDeviceHelper.CommandDelegate(onSelectLeftMenu));
            pauseMenuKeyboard.registerCommand(Keys.Right, true, new InputDeviceHelper.CommandDelegate(onSelectRightMenu));
            pauseMenuKeyboard.registerCommand(Keys.Enter, true, new InputDeviceHelper.CommandDelegate(onSelectMenuOption));
            endGameKeyboard.registerCommand(Keys.Enter, true, new InputDeviceHelper.CommandDelegate(onEnterEndScreen));
            moveLeft = false;
            moveRight = false;
            pause = false;
            initalUpdate = true;
            initalRender = true;
            hoverContinueMenu = true;
            selectMenuOption = false;
            hoverExitMenu = false;
            currentScore = 0;
            isGameOver = false;
            didWinGame = false;
            highScores = null;
            savingScores = false;
            loadingScores = false;
            loadingError = false;
            savingError = false;
            shouldExitNow = false;
            secondsWaiting = 0;
            preGame = true;
            bricksHitCnt = new Dictionary<Ball, int>();
            ballsAdded = 0;
            showEndScreen = false;
        }

        public override void loadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("Fonts/regFont");
            brickHitSound = contentManager.Load<SoundEffect>("Audio/crash-glass-sound-effect-24-11503");
            backgroundSong = contentManager.Load<Song>("Audio/risk-136788");
            ballTexture = contentManager.Load<Texture2D>("Images/ball");
            blueBrickTexture = contentManager.Load<Texture2D>("Images/blue-tile");
            greenBrickTexture = contentManager.Load<Texture2D>("Images/green-tile");
            orangeBrickTexture = contentManager.Load<Texture2D>("Images/orange-tile");
            yellowBrickTexture = contentManager.Load<Texture2D>("Images/yellow-tile");
            paddleTexture = contentManager.Load<Texture2D>("Images/paddle");
            miniPaddleTexture = contentManager.Load<Texture2D>("Images/mini-paddle");
            smokeTexture = contentManager.Load<Texture2D>("Images/smoke");
            starTexture = contentManager.Load<Texture2D>("Images/star");
            gameBackgroundTexture = contentManager.Load<Texture2D>("Images/backgroundPurple");
            menuBackgroundTexture = contentManager.Load<Texture2D>("Images/background");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            // Choose keyboard based on what we are showing on screen
            if (pause) pauseMenuKeyboard.Update(gameTime);
            else if (showEndScreen) endGameKeyboard.Update(gameTime);
            else gameplayKeyboard.Update(gameTime);
            if (shouldExitNow)
            {
                return GameStateEnum.MainMenu;
            }
            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(gameBackgroundTexture, new Rectangle(0, 0, 1920, 1080), Color.White);

            // Central countdown timer
            if (preGame) renderCountDown();

            brickRenderer.renderBricks(spriteBatch, yellowBrickTexture, blueBrickTexture, greenBrickTexture, orangeBrickTexture);
            paddleRenderer.renderPaddle(spriteBatch, paddleTexture);
            particleRenderer.renderParticles(spriteBatch);
            ballRenderer.renderBalls(spriteBatch, ballTexture);

            // Render score and remaining paddle graphics
            renderScore();
            renderRemainingPaddles();

            // Other optional renders
            if (pause) renderPauseMenu();
            if (showEndScreen) renderEndScreen();
            if (loadingError) renderLoadingError();
            if (savingError) renderSavingError();

            spriteBatch.End();
        }

        // Displays if there was an error loading score data
        private void renderLoadingError()
        {
            spriteBatch.DrawString(font, "Loading Scores Error!!", new Vector2(720, 1000), Color.White);
        }

        // Displays if there was an error saving score data
        private void renderSavingError()
        {
            spriteBatch.DrawString(font, "Saving Scores Error!!", new Vector2(720, 1000), Color.White);
        }

        // Renders the end screen when done
        private void renderEndScreen()
        {
            Rectangle backgroundBox = new Rectangle(710, 390, 500, 300);
            spriteBatch.Draw(menuBackgroundTexture, backgroundBox, Color.White);
            string winLose = "You " + (didWinGame ? "Won!" : "Lost :(");
            spriteBatch.DrawString(font, winLose, new Vector2(720, 400), Color.White);
            spriteBatch.DrawString(
                font,
                "Press Enter to Exit",
                new Vector2(750, 490),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.6f,
                SpriteEffects.None,
                0
            );
        }

        // Renders the pause menu
        private void renderPauseMenu()
        {
            Rectangle backgroundBox = new Rectangle(710, 390, 500, 300);
            spriteBatch.Draw(menuBackgroundTexture, backgroundBox, Color.White);
            spriteBatch.DrawString(font, "Pause Menu", new Vector2(960 - (font.MeasureString("Pause Menu") / 2).X * .9f, 400), Color.White);
            spriteBatch.DrawString(
                font,
                "Exit",
                new Vector2(720, 490),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.9f,
                SpriteEffects.None,
                0
            );
            spriteBatch.DrawString(
                font,
                "Continue",
                new Vector2(970, 490),
                Color.White,
                0.0f,
                new Vector2(0f, 0f),
                0.9f,
                SpriteEffects.None,
                0
            );
            Rectangle starBox = new Rectangle(hoverContinueMenu ? 1020 : 720, 550, 100, 100);
            spriteBatch.Draw(starTexture, starBox, Color.White);
        }

        // Renders the remaining paddle graphics
        private void renderRemainingPaddles()
        {
            int remaining = paddleManager.getRemainingPaddles();
            if (remaining <= 0) return;
            for (int i = 0; i < remaining; i++)
            {
                Rectangle miniPaddleBox = new Rectangle(30 + (i * 60), 1000, 50, 20);
                spriteBatch.Draw(miniPaddleTexture, miniPaddleBox, Color.White);
            }
        }

        // Renders score in lower right
        private void renderScore()
        {
            string valString = "Score: " + currentScore;
            spriteBatch.DrawString(
                font,
                valString,
                new Vector2(1650 - (font.MeasureString(valString) / 2).X, 1000),
                Color.Red,
                0.0f,
                new Vector2(0f, 0f),
                1f,
                SpriteEffects.None,
                0
            );
        }

        // Renders the central countdown timer
        private void renderCountDown()
        {
            string valString = "" + Math.Floor(4 - secondsWaiting);
            spriteBatch.DrawString(
                font,
                valString,
                new Vector2(960 - (font.MeasureString(valString) / 2).X, 540 - (font.MeasureString(valString) / 2).Y),
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
            // Don't update anything if we need to exit
            if (shouldExitNow)
            {
                resetGameplayVariables();
                return;
            }

            // Get high scores, music going, and create bricks on inital entry
            if (initalUpdate)
            {
                initalUpdate = false;
                MediaPlayer.Play(backgroundSong);
                MediaPlayer.IsRepeating = true;
                brickManager.createBricks();
                loadExistingHighScores();
            }

            // Don't update if we are on the end screen
            if (showEndScreen) return;

            // Don't update if we are paused
            if (pause)
            {
                MediaPlayer.Pause();
                updatePauseMenu();
                return;
            }

            // Attempt to update paddle velocity if requested
            if (moveLeft)
            {
                paddleManager.movePaddleLeft();
                moveLeft = false;
            } else if (moveRight)
            {
                paddleManager.movePaddleRight();
                moveRight = false;
            } else paddleManager.verifyPaddleStill();

            // Update state of particles, paddle location/size, and ball locations
            particleManager.updateParticleEmitters(gameTime);
            paddleManager.updatePaddles(gameTime);
            ballManager.updateBallsLocations(gameTime);

            // Now inbetween rounds
            if (paddleManager.paddleRemoved && paddleManager.getCurrentPaddle != null)
            {
                preGame = true;
                paddleManager.updatePaddleRemovedStatus(false);
            }

            // Keep track of time while waiting
            if (preGame) secondsWaiting += gameTime.ElapsedGameTime.TotalSeconds;

            // Wait to chuck in new ball
            if (secondsWaiting <= 3 && preGame)
            {
                // Don't add ball for full 3 seconds
            } else if (secondsWaiting > 3 && preGame) {
                preGame = false;
                secondsWaiting = 0;
                addNewBall(false);
            }

            // See if the ball hit something
            manageBallHits(); 

            // Update high scores when the game ends
            if (isGameOver) 
            {
                updateHighScores();
            }
        }

        // Add a new ball on screen
        private void addNewBall(bool isExtraBall)
        {
            Paddle curP = paddleManager.getCurrentPaddle();
            float startingX = curP.center.X;
            float startingY = (float)(curP.center.Y - (curP.height / 2) - 40);
            Ball b = new Ball(new Vector2(startingX, startingY), new Vector2(-.3f, -.4f), 500f, 20, isExtraBall);
            ballManager.addBall(b);
            bricksHitCnt.Add(b, 0);
        }

        // Sets a few misc things if continuing is selected on pause menu
        private void updatePauseMenu()
        {
            if (selectMenuOption)
            {
                if (hoverContinueMenu)
                {
                    selectMenuOption = false;
                    pause = false;
                    MediaPlayer.Resume();
                }
            }
        }

        // Keyboard function when left arrow pressed to move paddle
        private void onMoveLeft(GameTime gameTime)
        {
            moveLeft = true;
        }

        // Keyboard function when right arrow pressed to move paddle
        private void onMoveRight(GameTime gameTime)
        {
            moveRight = true;
        }

        // Keyboard function when pause pressed while in game
        private void onEscape(GameTime gameTime)
        {
            pause = true;
        }

        // Keyboard function when moving left in pause menu
        private void onSelectLeftMenu(GameTime gameTime)
        {
            hoverExitMenu = true;
            hoverContinueMenu = false;
        }
        
        // Keyboard function when moving right in pause menu
        private void onSelectRightMenu(GameTime gameTime)
        {
            hoverContinueMenu = true;
            hoverExitMenu = false;
        }

        // Keyboard function when selecting in pause menu
        private void onSelectMenuOption(GameTime gameTime)
        {
            selectMenuOption = true;
            shouldExitNow = hoverExitMenu;
        }

        // Keyboard function when pressing enter on end screen
        private void onEnterEndScreen(GameTime gameTime)
        {
            shouldExitNow = true;
        }

        // Adds particles for a brick
        private void addBrickParticles(Brick brick)
        {
            particleManager.addParticleEmitter((int)(brick.center.X - (brick.width / 2)), (int)(brick.center.Y - (brick.height / 2)), (int)brick.width, (int)brick.height, 5, 15, new TimeSpan(0, 0, 2), smokeTexture);
        }

        // Manages the balls hiting things
        private void manageBallHits()
        {
            List<Ball> ballsToRemove = new List<Ball>();
            foreach (Ball ball in ballManager.getBalls())
            {
                // Only check for brick hit when high enough
                if (ball.position.Y <= 500) checkForBrickHit(ball);
                // Check for wall hits
                if (ball.position.X - ball.radius <= 0 && ball.position.Y - ball.radius <= 0) ballManager.updateBallDirectionCornerShot(ball);
                else if (ball.position.X + ball.radius >= 1920 && ball.position.Y - ball.radius <= 0) ballManager.updateBallDirectionCornerShot(ball);
                else if (ball.position.X - ball.radius <= 0 || ball.position.X + ball.radius >= 1920) ballManager.updateBallDirectionWallShot(true, ball);
                else if (ball.position.Y - ball.radius <= 0) ballManager.updateBallDirectionWallShot(false, ball);
                // When low enough, check for missed ball / paddle hit
                if (ball.position.Y > 1080) manageMissedBall(ballsToRemove, ball);
                
                else if (ball.position.Y >= 930) checkForPaddleHit(ball);
            }
            // Removes balls that are out of screen
            foreach (Ball ball in ballsToRemove)
            {
                ballManager.removeBall(ball);
            }
            // Add balls as score increases
            if (currentScore / 100 >= ballsAdded + 1)
            {
                while (currentScore / 100 != ballsAdded)
                {
                    addNewBall(true);
                    ballsAdded++;
                }
            }
            // Are you winning son?
            if (brickManager.noBricksLeft()) 
            {
                didWinGame = true;
                isGameOver = true;
                showEndScreen = true;
            }
        }

        // Deals with missing a ball
        private void manageMissedBall(List<Ball> list, Ball ball)
        {
            list.Add(ball);
            if (ball.isExtraBall) return;
            // Lost paddle only if ball isn't added due to high score
            Paddle curPaddle = paddleManager.getCurrentPaddle();
            paddleManager.shrinkPaddle(0, curPaddle);
            if (paddleManager.getRemainingPaddles() <= 0)
            {
                isGameOver = true;
                showEndScreen = true;
                return;
            }
        }

        // See if the paddle was hit with the ball
        private void checkForPaddleHit(Ball ball)
        {
            Paddle curPaddle = paddleManager.getCurrentPaddle();
            if (curPaddle == null) return;
            
            if (curPaddle.hitBox.didCollideWithHitBox(ball.hitBox))
            {
                float newX = (float)((ball.position.X - curPaddle.center.X) / (curPaddle.width / 2));
                ballManager.updateBallDirectionPaddleShot((float)(curPaddle.center.Y - curPaddle.height), newX, ball);
            }
        }

        // See if bricks were hit
        private void checkForBrickHit(Ball ball)
        {
            List<Brick> bricksHit = new List<Brick>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    Brick b = brickManager.getBricks()[i, j];
                    if (b != null) {
                        if (b.hitBox.didCollideWithHitBox(ball.hitBox)) {
                            bricksHit.Add(b);
                        }
                    }
                }
            }
            if (bricksHit.Count == 1)
            {
                // See which side of brick was hit
                CollisionSide s = CollisionHelperAABB.GetCollisionSide(ball.prevHitBox, bricksHit[0].hitBox, ball.direction);
                if (s == CollisionSide.Top || s == CollisionSide.Bottom) ballManager.updateBallDirectionWallShot(false, ball);
                else if (s == CollisionSide.Left || s == CollisionSide.Right) ballManager.updateBallDirectionWallShot(true, ball);
            }
            if (bricksHit.Count > 1)
            {
                // Special code to deal with a multi brick hit. Either bricks side by side or hit a corner of two exactly
                float indexX = bricksHit[0].index.X;
                float indexY = bricksHit[0].index.Y;
                float indexXBaseline = bricksHit[0].index.X;
                float indexYBaseline = bricksHit[0].index.Y;
                foreach (Brick b in bricksHit)
                {
                    indexX = b.index.X;
                    indexY = b.index.Y;
                }
                if (indexX == indexXBaseline)
                {
                    ballManager.updateBallDirectionWallShot(false, ball);
                } else if (indexY == indexYBaseline)
                {
                    ballManager.updateBallDirectionWallShot(true, ball);
                } else ballManager.updateBallDirectionCornerShot(ball);
            }
            foreach (Brick brick in bricksHit)
            {
                // For each brick hit, add particles, add score, play sound, increase ball speed if needed, see if we need to shrink paddle
                addBrickParticles(brick);
                updateScore(brick);
                brickManager.removeBrick(brick);
                bricksHitCnt[ball] = bricksHitCnt[ball] + 1;
                brickHitSound.Play();
                updateBallSpeed(ball);
                checkForShrinkingPaddle(paddleManager.getCurrentPaddle(), brick);
            }
            
        }

        // See if you are good enough to shrink the paddle
        private void checkForShrinkingPaddle(Paddle paddle, Brick brick)
        {
            if (brick.index.X == 0 && paddleManager.canHalfPaddle(paddle))
            {
                paddleManager.shrinkPaddle(50, paddle);
            }
        }

        // Update speed on intervals of bricks hit by a ball
        private void updateBallSpeed(Ball ball)
        {
            int count = bricksHitCnt[ball];
            if (count == 4 || count == 12 || count == 36 || count == 62) ballManager.increaseBallSpeed(ball);
        }

        // Update score based on brick type
        private void updateScore(Brick brick)
        {
            if (brick.color == BrickColorEnum.Blue) currentScore += 3;
            else if (brick.color == BrickColorEnum.Yellow) currentScore += 1;
            else if (brick.color == BrickColorEnum.Orange) currentScore += 2;
            else if (brick.color == BrickColorEnum.Green) currentScore += 5;
            if (brickManager.newLineCleared(brick)) currentScore += 25;
        }

        // Update high scores after game
        private void updateHighScores()
        {
            if (highScores == null && !loadingError)
            {
                List<int> scoreList = new List<int> {currentScore, 0, 0, 0, 0};
                highScores = new HighScoreStorage(scoreList);
            } else if (!loadingError) 
            {
                highScores.highScores.Add(currentScore);
                highScores.highScores.Sort();
                highScores.highScores.RemoveAt(0);
                highScores.highScores.Reverse();
            }
            saveHighScores();
        }

        // Sets up to save score
        private void saveHighScores()
        {
            lock (this)
            {
                if (!savingScores)
                {
                    savingScores = true;
                    finishSavingScores();
                }
            }
        }

        // Save scores to computer
        private async void finishSavingScores()
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
                        this.savingScores = false;
                    }
                }
                this.savingScores = false;
            });
        }

        // Setup to load existing scores
        private void loadExistingHighScores()
        {
            lock (this)
            {
                if (!loadingScores)
                {
                    loadingScores = true;
                    getHighScores();
                }
            }
        }

        // Get the scores
        private async void getHighScores()
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
                        loadingScores = false;
                    }
                }
                loadingScores = false;
            });
        }

        // Reset class vars when leaving
        private void resetGameplayVariables()
        {
            ballManager = new BallManager();
            brickManager = new BrickManager();
            paddleManager = new PaddleManager();
            particleManager = new ParticleManager();
            brickRenderer = new BrickRenderer(brickManager);
            ballRenderer = new BallRenderer(ballManager);
            paddleRenderer = new PaddleRenderer(paddleManager);
            particleRenderer = new ParticleRenderer(particleManager);
            moveLeft = false;
            moveRight = false;
            pause = false;
            initalUpdate = true;
            initalRender = true;
            hoverContinueMenu = true;
            selectMenuOption = false;
            hoverExitMenu = false;
            currentScore = 0;
            isGameOver = false;
            didWinGame = false;
            highScores = null;
            savingScores = false;
            loadingScores = false;
            loadingError = false;
            savingError = false;
            shouldExitNow = false;
            secondsWaiting = 0;
            preGame = true;
            bricksHitCnt = new Dictionary<Ball, int>();
            ballsAdded = 0;
            showEndScreen = false;
            // Stop music
            MediaPlayer.Stop();
        }
    }
}
