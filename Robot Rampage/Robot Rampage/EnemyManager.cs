using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Robot_Rampage
{
    class EnemyManager
    {
        #region Declarations
        public static List<Enemy> Enemies = new List<Enemy>();
        public static Texture2D enemyTexture;
        public static Rectangle enemyInitialFrame;
        public static int MaxActiveEnemies = 30;
        #endregion

        #region Initialization
        public static void Initialize(Texture2D texture, Rectangle initialFrame)
        {
            enemyTexture = texture;
            enemyInitialFrame = initialFrame;
        }
        #endregion

        #region Enemy Management
        public static void AddEnemy(Vector2 squareLocation)
        {
            int startX = (int)squareLocation.X;
            int startY = (int)squareLocation.Y;
            Rectangle squareRect = TileMap.SquareWorldRectangle(startX, startY);
            Enemy newEnemy = new Enemy(new Vector2(squareRect.X, squareRect.Y), enemyTexture, enemyInitialFrame);
            newEnemy.currentTargetSquare = squareLocation;
            Enemies.Add(newEnemy);
        }
        #endregion

        #region Update and Draw
        public static void Update(GameTime gameTime)
        {
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                Enemies[i].Update(gameTime);
                if (Enemies[i].Destroyed)
                {
                    Enemies.RemoveAt(i);
                }
            }
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
        #endregion
    }
}
