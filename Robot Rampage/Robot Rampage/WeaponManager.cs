using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Robot_Rampage
{
    static class WeaponManager
    {

        #region Declarations
        static public List<Particle> Shots = new List<Particle>();
        static public Texture2D Texture;
        static public Rectangle shotRectangle = new Rectangle(0, 128, 32, 32);
        static public float WeaponSpeed = 600f;

        static private float shotTimer = 0f;
        static private float shotMinTimer = 0.15f;
        static private float rocketMinTimer = 0.5f;

        public enum WeaponType { Normal, Triple, Rocket };
        static public WeaponType CurrentWeaponType = WeaponType.Normal;
        static public float WeaponTimeRemainng = 30.0f;
        static public float weaponTimeDefault = 30.0f;
        static private float tripleWeaponSplitAngle = 15;

        static public List<Sprite> PowerUps = new List<Sprite>();
        static private int maxActivePowerUps = 5;
        static private float timeSinceLastPowerUp = 0.0f;
        static private float timeBetweenPowerUps = 2.0f;

        static private Random rand = new Random();
        #endregion

        #region Properties
        static public float WeaponFireDelay 
        {
            get { return (CurrentWeaponType == WeaponType.Rocket) ? rocketMinTimer : shotMinTimer; }
        }

        static public bool CanFireWeapon 
        {
            get { return (shotTimer >= WeaponFireDelay); }
        }
        #endregion

        #region Weapons Management Methods
        public static void FireWeapon(Vector2 location, Vector2 velocity)
        {
            switch (CurrentWeaponType)
            {
                case WeaponType.Normal:
                    AddShot(location, velocity, 0);
                    break;
                case WeaponType.Triple:
                    AddShot(location, velocity, 0);
                    float baseAngle = (float)Math.Atan2(velocity.Y, velocity.X);
                    float offset = MathHelper.ToRadians(tripleWeaponSplitAngle);
                    Vector2 newVelocity;
                    newVelocity = new Vector2((float)Math.Cos(baseAngle - offset), (float)Math.Sin(baseAngle - offset)) * velocity.Length();
                    AddShot(location, newVelocity, 0);
                    newVelocity = new Vector2((float)Math.Cos(baseAngle + offset), (float)Math.Sin(baseAngle + offset)) * velocity.Length();
                    AddShot(location, newVelocity, 0);
                    break;
                case WeaponType.Rocket:
                    AddShot(location, velocity, 1);
                    break;
            }
            shotTimer = 0.0f;
        }

        private static void CheckWeaponUpgradeExpire(float elapsed)
        {
            if (CurrentWeaponType != WeaponType.Normal)
            {
                WeaponTimeRemainng -= elapsed;
                if (WeaponTimeRemainng <= 0)
                {
                    CurrentWeaponType = WeaponType.Normal;
                }
            }
        }

        private static void CheckPowerUpSpawns(float elapsed)
        {
            timeSinceLastPowerUp += elapsed;
            if (timeSinceLastPowerUp >= timeBetweenPowerUps)
            {
                WeaponType type = WeaponType.Triple;
                if (rand.Next(0, 5) < 2)
                {
                    type = WeaponType.Rocket;
                }
                TryToSpawnPowerUp(rand.Next(0, TileMap.MapWidth), rand.Next(0, TileMap.MapHeight), type);
            }
        }

        private static void TryToSpawnPowerUp(int x, int y, WeaponType type)
        {
            if (PowerUps.Count >= maxActivePowerUps)
            {
                return;
            }
            
            Rectangle thisDestination = TileMap.SquareWorldRectangle(new Vector2(x, y));

            foreach (Sprite powerup in PowerUps)
            {
                if (powerup.WorldRectangle == thisDestination)
                {
                    return;
                }
            }

            if (!TileMap.IsWallTile(x, y))
            {
                Sprite newPowerUp = new Sprite(new Vector2(thisDestination.X, thisDestination.Y), Texture, new Rectangle(64, 128, 32, 32), Vector2.Zero);
                newPowerUp.Animate = false;
                newPowerUp.CollisionRadius = 14;
                newPowerUp.AddFrame(new Rectangle(96, 128, 32, 32));
                if (type == WeaponType.Rocket)
                {
                    newPowerUp.Frame = 1;
                }
                PowerUps.Add(newPowerUp);
                timeSinceLastPowerUp = 0.0f;
            }
        }
        #endregion

        #region Effects Manager Methods
        private static void CreateLargeExplosion(Vector2 location)
        {
            EffectsManager.AddLargeExplosion(location + new Vector2(-10, -10));
            EffectsManager.AddLargeExplosion(location + new Vector2(-10, 10));
            EffectsManager.AddLargeExplosion(location + new Vector2(10, -10));
            EffectsManager.AddLargeExplosion(location + new Vector2(10, 10));
        }

        private static void AddShot(Vector2 location, Vector2 velocity, int frame)
        {
            Particle shot = new Particle(location, Texture, shotRectangle, velocity, Vector2.Zero, 400f, 120, Color.White, Color.White);
            shot.AddFrame(new Rectangle(shotRectangle.X + shotRectangle.Width, shotRectangle.Y, shotRectangle.Width, shotRectangle.Height));
            shot.Animate = false;
            shot.Frame = frame;
            shot.RotateTo(velocity);
            Shots.Add(shot);
        }
        #endregion

        #region Collision Detection
        private static void CheckShotWallImpacts(Sprite shot)
        {
            if (shot.Expired)
            {
                return;
            }

            if (TileMap.IsWallTile(TileMap.GetSquareAtPixel(shot.WorldCenter)))
            {
                shot.Expired = true;
                if (shot.Frame == 0)
                {
                    EffectsManager.AddSparksEffect(shot.WorldCenter, shot.Velocity);
                }
                else
                {
                    CreateLargeExplosion(shot.WorldCenter);
                }
            }
        }

        private static void CheckPowerUpPickups()
        {
            for (int i = PowerUps.Count - 1; i >= 0; i--)
            {
                if (Player.BaseSprite.IsCircleColliding(PowerUps[i].WorldCenter, PowerUps[i].CollisionRadius))
                {
                    switch (PowerUps[i].Frame)
                    {
                        case 0:
                            CurrentWeaponType = WeaponType.Triple;
                            break;
                        case 1:
                            CurrentWeaponType = WeaponType.Rocket;
                            break;
                    }
                    WeaponTimeRemainng = weaponTimeDefault;
                    PowerUps.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Update and Draw
        static public void Update (GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            shotTimer += elapsed;
            CheckWeaponUpgradeExpire(elapsed);

            for (int i = Shots.Count - 1; i >= 0; i--)
            {
                Shots[i].Update(gameTime);
                CheckShotWallImpacts(Shots[i]);
                if (Shots[i].Expired)
                {
                    Shots.RemoveAt(i);
                }
            }
            CheckPowerUpSpawns(elapsed);
            CheckPowerUpPickups();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle sprite in Shots)
            {
                sprite.Draw(spriteBatch);
            }
            foreach (Sprite sprite in PowerUps)
            {
                sprite.Draw(spriteBatch);
            }
        }
        #endregion
    }
}
