using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameStates { TitleScreen, Playing, PlayerDead, GameOver };
        GameStates gameState = GameStates.TitleScreen;
        Texture2D titleScreen;
        Texture2D spriteSheet;

        StarField starField;
        AsteroidManager asteroidManager;

        PlayerManager playerManager;

        EnemyManager enemyManager;

        ExplosionManager explosionManager;

        CollisionManager collisionManager;

        SpriteFont comicSansFont;

        private float playerDeathDelayTime = 5f;
        private float playerDeathTimer = 0f;
        private float titleScreenTimer = 0f;
        private float titleScreenDelayTime = 1f;

        private int playerStartingLives = 3;
        private Vector2 playerStartLocation = new Vector2(390, 550);
        private Vector2 scoreLocation = new Vector2(20, 10);
        private Vector2 livesLocation = new Vector2(20, 40);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");

            comicSansFont = Content.Load<SpriteFont>(@"Fonts\ComicSans");

            starField = new StarField(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, 
                200, new Vector2(0, 30f), spriteSheet, new Rectangle(0, 450, 2, 2));

            asteroidManager = new AsteroidManager(10, spriteSheet, new Rectangle(0, 0, 50, 50), 
                20, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);

            playerManager = new PlayerManager(spriteSheet, new Rectangle(0, 150, 50, 50), 3,
                new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));

            enemyManager = new EnemyManager(spriteSheet, new Rectangle(0, 200, 50, 50), 6, playerManager, 
                new Rectangle(0,0,this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));

            explosionManager = new ExplosionManager(spriteSheet, new Rectangle(0, 100, 50, 50), 3,
                new Rectangle(0, 450, 2, 2));

            collisionManager = new CollisionManager(asteroidManager, playerManager, enemyManager, explosionManager);

            SoundManager.Initialize(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    titleScreenTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (titleScreenTimer >= titleScreenDelayTime)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Space) || (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed))
                        {
                            playerManager.LivesRemaining = playerStartingLives;
                            playerManager.PlayerScore = 0;
                            ResetGame();
                            gameState = GameStates.Playing;
                        }
                    }
                    break;
                case GameStates.Playing:
                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    playerManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    collisionManager.CheckCollisions();

                    if (playerManager.Destroyed)
                    {
                        playerDeathTimer = 0;
                        enemyManager.Active = false;
                        playerManager.LivesRemaining--;
                        if (playerManager.LivesRemaining < 0)
                        {
                            gameState = GameStates.GameOver;
                        }
                        else
                        {
                            gameState = GameStates.PlayerDead;
                        }
                    }
                    break;
                case GameStates.PlayerDead:
                    playerDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);

                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        ResetGame();
                        gameState = GameStates.Playing;
                    }
                    break;
                case GameStates.GameOver:
                    playerDeathTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    starField.Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);

                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        gameState = GameStates.TitleScreen;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen, 
                    new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height),
                    Color.White);
            }
            if ((gameState == GameStates.Playing) || (gameState == GameStates.PlayerDead) ||
                (gameState == GameStates.GameOver))
            {
                starField.Draw(spriteBatch);
                asteroidManager.Draw(spriteBatch);
                playerManager.Draw(spriteBatch);
                enemyManager.Draw(spriteBatch);
                explosionManager.Draw(spriteBatch);

                spriteBatch.DrawString(comicSansFont, "Score: " + playerManager.PlayerScore.ToString(), scoreLocation, Color.White);

                if (playerManager.LivesRemaining >= 0)
                {
                    spriteBatch.DrawString(comicSansFont, "Ships Remaining: " + playerManager.LivesRemaining.ToString(), livesLocation, Color.White);
                }
            }
            if ((gameState == GameStates.GameOver))
            {
                spriteBatch.DrawString(comicSansFont, "G A M E O V E R !",
                    new Vector2(this.Window.ClientBounds.Width / 2 - comicSansFont.MeasureString("G A M E O V E R !").X / 2,
                    this.Window.ClientBounds.Height / 2 - comicSansFont.MeasureString("G A M E O V E R !").Y / 2), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ResetGame()
        {
            playerManager.playerSprite.Location = playerStartLocation;
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }
            enemyManager.Enemies.Clear();
            enemyManager.Active = true;
            playerManager.PlayerShotManager.Shots.Clear();
            enemyManager.EnemyShotManager.Shots.Clear();
            playerManager.Destroyed = false;
        }
    }
}
