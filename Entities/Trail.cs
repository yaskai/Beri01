using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00.Entities
{
    public class Trail
    {
        public bool isActive = true;

        Game1 game;
        private Texture2D texture;
        private Vector2 position;
        private float alpha;
        private double duration;
        private bool fade;
        private string dir = "n";

        private double startDuration;
        private SpriteEffects effect;

        public Trail(Texture2D texture, Vector2 position, float alpha, double duration, bool fade, Game1 game, String dir)
        {
            this.texture = texture;
            this.position = position;
            this.alpha = alpha;
            this.duration = duration;
            this.fade = fade;
            this.game = game;

            startDuration = duration;

            if (dir == "l") effect = SpriteEffects.FlipHorizontally; else if (dir == "r" || dir == "n") effect = SpriteEffects.None;
        }

        public void Update(GameTime gameTime)
        {
            duration -= game.delta;

            if (fade)
            {
                //alpha = alpha - (float)((startDuration / duration) * (float)game.delta);
                alpha -= (float)game.delta / 50;
            }

            if (duration < 0)
            {
                isActive = false;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(texture, 
                position, 
                null, 
                Color.White * alpha, 
                0, 
                new Vector2(1, 1), 
                new Vector2(1, 1), 
                effect, 1);
        }
    }
}
