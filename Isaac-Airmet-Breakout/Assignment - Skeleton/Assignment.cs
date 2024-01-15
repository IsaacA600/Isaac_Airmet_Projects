using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

// Adapted from Professor Dean
namespace CS5410
{
    // Main game
    public class Assignment : Game
    {
        private GraphicsDeviceManager graphics;
        private IGameState currentState;
        private GameStateEnum nextStateEnum = GameStateEnum.MainMenu;
        private Dictionary<GameStateEnum, IGameState> m_states;

        public Assignment()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            graphics.ApplyChanges();

            // Create all the game states here
            m_states = new Dictionary<GameStateEnum, IGameState>
            {
                { GameStateEnum.MainMenu, new MainMenuView() },
                { GameStateEnum.GamePlay, new GamePlayView() },
                { GameStateEnum.HighScores, new HighScoresView() },
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
            // Special case for exiting the game
            if (nextStateEnum == GameStateEnum.Exit)
            {
                Exit();
            }
            
            // Update the current game state
            currentState.update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Render current game state, then swap if needed
            currentState.render(gameTime);

            currentState = m_states[nextStateEnum];

            base.Draw(gameTime);
        }
    }
}
