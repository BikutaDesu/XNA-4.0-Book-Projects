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

        // Temporary Demo Code Begin
        Sprite tempSprite;
        Sprite tempSprite2;
        // Temporary Demo Code End

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

            // Temporary Demo Code Begin
            tempSprite = new Sprite(
                new Vector2(100, 100),
                spriteSheet,
                new Rectangle(0, 64, 32, 32),
                Vector2.Zero);
            tempSprite2 = new Sprite(
                new Vector2(200, 200),
                spriteSheet,
                new Rectangle(0, 160, 32, 32),
                Vector2.Zero);
            // Temporary Demo Code End

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

            // Temporary Demo Code Begin
            Vector2 spriteMove = Vector2.Zero;
            Vector2 cameraMove = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                spriteMove.X = -1;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                spriteMove.X = 1;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                spriteMove.Y = -1;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                spriteMove.Y = 1;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraMove.X = -1;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraMove.X = 1;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                cameraMove.Y = -1;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                cameraMove.Y = 1;

            Camera.Move(cameraMove);

            tempSprite.Velocity = spriteMove * 60;
            tempSprite.Update(gameTime);
            tempSprite2.Update(gameTime);
            // Temporary Demo Code End


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
            tempSprite.Draw(spriteBatch);
            tempSprite2.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
