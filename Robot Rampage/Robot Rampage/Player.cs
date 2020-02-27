using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Robot_Rampage
{
    static class Player
    {
        #region Declarations
        public static Sprite BaseSprite;
        public static Sprite TurretSprite;

        private static Vector2 baseAngle = Vector2.Zero;
        private static Vector2 turretAngle = Vector2.Zero;
        private static float playerSpeed = 90f;
        #endregion

        #region Initialization
        public static void Initialize(Texture2D texture, Rectangle baseInitialFrame, 
            int baseFrameCount, Rectangle turretInitialFrame, int turretFrameCount,
            Vector2 worldLocation)
        {
            int frameWidth = baseInitialFrame.Width;
            int frameHeight = baseInitialFrame.Height;

            BaseSprite = new Sprite(worldLocation, texture, baseInitialFrame, Vector2.Zero);
            BaseSprite.BoundingXPadding = 4;
            BaseSprite.BoundingYPadding = 4;
            BaseSprite.AnimateWhenStopped = false;

            for (int i = 1; i < baseFrameCount; i++)
            {
                BaseSprite.AddFrame(new Rectangle(baseInitialFrame.X + (frameHeight * i),
                    baseInitialFrame.Y, frameWidth, frameHeight));
            }

            TurretSprite = new Sprite(worldLocation, texture, turretInitialFrame, Vector2.Zero);
            TurretSprite.Animate = false;

            for (int i = 1; i < turretFrameCount; i++)
            {
                BaseSprite.AddFrame(new Rectangle(turretInitialFrame.X + (frameHeight * i),
                    turretInitialFrame.Y, frameWidth, frameHeight));
            }
        }
        #endregion

        #region Input Handling
        private static Vector2 handleKeyboardMovement(KeyboardState keyState)
        {
            Vector2 keyMovement = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W))
            {
                keyMovement.Y--;
            }

            if (keyState.IsKeyDown(Keys.A))
            {
                keyMovement.X--;
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                keyMovement.Y++;
            }

            if (keyState.IsKeyDown(Keys.D))
            {
                keyMovement.X++;
            }

            return keyMovement;
        }

        private static Vector2 handleGamePadMovement(GamePadState gamePadState)
        {
            return new Vector2(gamePadState.ThumbSticks.Left.X, -gamePadState.ThumbSticks.Left.Y);
        }

        private static Vector2 handleKeyboardShots(KeyboardState keyboardState)
        {
            Vector2 keyShots = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.NumPad1))
            {
                keyShots = new Vector2(-1, 1);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad2))
            {
                keyShots = new Vector2(0, 1);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad3))
            {
                keyShots = new Vector2(1, 1);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad4))
            {
                keyShots = new Vector2(-1, 0);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad6))
            {
                keyShots = new Vector2(1, 0);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad7))
            {
                keyShots = new Vector2(-1, -1);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad8))
            {
                keyShots = new Vector2(0, -1);
            }

            if (keyboardState.IsKeyDown(Keys.NumPad9))
            {
                keyShots = new Vector2(1, -1);
            }

            return keyShots;
        }

        private static Vector2 handleGamePadShots(GamePadState gamePadState)
        {
            return new Vector2(gamePadState.ThumbSticks.Right.X, -gamePadState.ThumbSticks.Right.Y);
        }

        private static void handleInput(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 moveAngle = Vector2.Zero;
            Vector2 fireAngle = Vector2.Zero;

            moveAngle += handleKeyboardMovement(Keyboard.GetState());
            moveAngle += handleGamePadMovement(GamePad.GetState(PlayerIndex.One));

            fireAngle += handleKeyboardShots(Keyboard.GetState());
            fireAngle += handleGamePadShots(GamePad.GetState(PlayerIndex.One));

            if (moveAngle != Vector2.Zero)
            {
                moveAngle.Normalize();
                baseAngle = moveAngle;
            }

            if (fireAngle != Vector2.Zero)
            {
                fireAngle.Normalize();
                turretAngle = fireAngle;
            }

            BaseSprite.RotateTo(baseAngle);
            TurretSprite.RotateTo(turretAngle);

            BaseSprite.Velocity = moveAngle * playerSpeed;
        }
        #endregion

        #region Update and Draw
        public static void Update(GameTime gameTime)
        {
            handleInput(gameTime);
            BaseSprite.Update(gameTime);
            TurretSprite.WorldLocation = BaseSprite.WorldLocation;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            BaseSprite.Draw(spriteBatch);
            TurretSprite.Draw(spriteBatch);
        }
        #endregion
    }
}
