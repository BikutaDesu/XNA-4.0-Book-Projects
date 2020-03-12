using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Robot_Rampage
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D spriteSheet;
        Texture2D titleScreen;
        SpriteFont arialFont;

        
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
            this.IsMouseVisible = true;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.ApplyChanges();



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

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            arialFont = Content.Load<SpriteFont>(@"Fonts\ArialFont");

            Camera.WorldRectangle = new Rectangle(0, 0, 1600, 1600);
            Camera.ViewPortWidth = 800;
            Camera.ViewPortHeight = 600;

            TileMap.Initialize(spriteSheet);
            TileMap.GenerateRandomMap();

            Player.Initialize(spriteSheet, new Rectangle(0, 64, 32, 32), 6,
                new Rectangle(0, 96, 32, 32), 1, new Vector2(300, 300));

            EffectsManager.Initialize(spriteSheet,new Rectangle(0, 288, 2, 2),
                new Rectangle(0, 256, 32, 32), 3);


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

            Player.Update(gameTime);

            EffectsManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            TileMap.Draw(spriteBatch);
            Player.Draw(spriteBatch);
            EffectsManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
