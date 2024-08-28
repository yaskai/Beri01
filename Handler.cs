using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00
{
    public class Handler
    {
        public Random rand;

        public List<Entities.Player> players;
        public List<Entities.Tile> tiles;
        public List<Entities.Trail> trails;
        public List<Entities.Hazard> hazards;
        public List<Entities.Enemy> enemies;
        public List<Entities.Platform> platforms;
        public Handler()
        {
            players = new List<Entities.Player>();
            tiles = new List<Entities.Tile>();
            trails = new List<Entities.Trail>();
            hazards = new List<Entities.Hazard>();
            enemies = new List<Entities.Enemy>();
            platforms = new List<Entities.Platform>();

            rand = new Random();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Entities.Player p in players) p.Update(gameTime);
            foreach (Entities.Trail tr in trails) tr.Update(gameTime);
            //foreach (Entities.Hazard h in hazards) h.Update(gameTime);
            foreach (Entities.Enemy e in enemies) e.Update(gameTime);
            foreach (Entities.Platform plt in platforms) plt.Update(gameTime);
            //RemoveEntities();
        }

        public void FixedUpdate()
        {
            foreach (Entities.Player p in players) p.FixedUpdate();
            foreach (Entities.Enemy e in enemies) e.FixedUpdate();
            foreach (Entities.Platform plt in platforms) plt.FixedUpdate(); 
            foreach (Entities.Hazard h in hazards) h.FixedUpdate();
            RemoveEntities();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            //foreach (Entities.Tile t in tiles) t.Draw(_spriteBatch);
            foreach(Entities.Platform plt in platforms) plt.Draw(_spriteBatch);
            foreach (Entities.Hazard h in hazards) h.Draw(_spriteBatch);
            foreach (Entities.Trail tr in trails) tr.Draw(_spriteBatch);
            foreach (Entities.Enemy e in enemies) if(e.inFrame) e.Draw(_spriteBatch);
            foreach (Entities.Player p in players) p.Draw(_spriteBatch);
        }

        private void RemoveEntities()
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                if (!platforms[i].isActive)
                {
                    platforms.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].isActive)
                {
                    players.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < trails.Count; i++)
            {
                if (!trails[i].isActive)
                {
                    trails.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < hazards.Count; i++)
            {
                if (!hazards[i].isActive)
                {
                    hazards.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].isActive)
                {
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < hazards.Count; i++)
            {
                if (!hazards[i].isActive)
                {
                    hazards.RemoveAt(i);
                    i--;
                }
            }
        }

        public void ClearScene()
        {
            foreach (Entities.Tile t in tiles) t.isActive = false;
            foreach (Entities.Trail tr in trails) tr.isActive = false;
            foreach (Entities.Hazard h in hazards) h.isActive = false;
            foreach (Entities.Enemy e in enemies) e.isActive = false;
            foreach (Entities.Platform plt in platforms) plt.isActive = false;
            RemoveEntities();
        }

        public void ResetScene()
        {
            foreach (Entities.Trail tr in trails) tr.isActive = false;
            foreach (Entities.Enemy e in enemies) e.isActive = false;
            foreach (Entities.Platform plt in platforms) plt.breakTimer = 30;
            RemoveEntities();
        }
    }
}
