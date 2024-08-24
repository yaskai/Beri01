using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00
{
    public class Background
    {
        Game1 game;

        public Texture2D layer0;
        public Texture2D layer1;
        public Texture2D layer2;

        public Vector2 pos0;
        public Vector2 pos1;
        public Vector2 pos2;

        public Vector2 offset0;
        public Vector2 offset1;
        public Vector2 offset2;

        public Vector2 scollSpeed;

        public bool _lockScroll = false;

        public Background(Texture2D layer0, Texture2D layer1, Texture2D layer2, Game1 game)
        {
            this.layer0 = layer0;
            this.layer1 = layer1;
            this.layer2 = layer2;

            this.game = game;

            offset0 = new Vector2(380, -90);
            offset1 = new Vector2(0, -90);
            offset2 = new Vector2(0, 0);

            pos0 = offset0;
            pos1 = offset1;
            pos2 = offset2;
        }

        public void Update(GameTime gameTime)
        {
            pos0.X += scollSpeed.X * (float)(game.delta) * 2.1f;
            pos1.X += scollSpeed.X * (float)(game.delta) * 3.5f;
            pos2.X += scollSpeed.X * (float)(game.delta) * 4.5f;

            //scollSpeed.X = -0.5f;

            if (layer0 != null)
            {
                //pos0.X = (game.camera.position.X * -0.09f) + offset0.X;
                pos0.Y = (game.camera.position.Y * -0.06f) + offset0.Y;

                if (pos0.X > layer0.Width * 2)
                {
                    pos0.X = -layer0.Width * 2;
                }
                else if (pos0.X < -layer0.Width * 2)
                {
                    pos0.X = layer0.Width * 2;
                }

                //if (pos0.Y < 0) pos0.Y = 0; else if (pos0.Y > layer0.Height) pos0.Y = layer0.Height;
            }

            if (layer1 != null)
            {
                //pos1.X = (game.camera.position.X * -0.12f) + offset1.X;
                //pos1.Y = (game.camera.position.Y * -0.11f) + offset1.Y;

                if (pos1.X > layer1.Width)
                {
                    pos1.X = 0;
                }
                else if (pos1.X < 0)
                {
                    pos1.X = layer1.Width;
                }
            }

            if (layer2 != null)
            {
                if (pos2.X > layer2.Width)
                {
                    pos2.X = 0;
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (layer0 != null)
            {
                _spriteBatch.Draw(layer0, pos0, 
                    null, Color.White, 0, 
                    new Vector2(1, 1), new Vector2(2, 2), 
                    SpriteEffects.None, 0);
                
                _spriteBatch.Draw(layer0, new Vector2(pos0.X - (layer0.Width * 2) + 2, pos0.Y),
                    null, Color.White, 0,
                    new Vector2(1, 1), new Vector2(2, 2),
                    SpriteEffects.None, 0);

                _spriteBatch.Draw(layer0, new Vector2(pos0.X + (layer0.Width * 2) - 2, pos0.Y),
                    null, Color.White, 0,
                    new Vector2(1, 1), new Vector2(2, 2),
                    SpriteEffects.None, 0);
            }

            if (layer1 != null)
            {
                _spriteBatch.Draw(layer1, pos1,
                    null, Color.White, 0,
                    new Vector2(1, 1), new Vector2(2, 2),
                    SpriteEffects.None, 0);

                _spriteBatch.Draw(layer1, new Vector2(pos1.X - layer1.Width, pos1.Y),
                    null, Color.White, 0,
                    new Vector2(1, 1), new Vector2(2, 2),
                    SpriteEffects.None, 0);

                _spriteBatch.Draw(layer1, new Vector2(pos1.X + layer1.Width, pos1.Y),
                    null, Color.White, 0,
                    new Vector2(1, 1), new Vector2(2, 2),
                    SpriteEffects.None, 0);
            }

            if (layer2 != null)
            {
                _spriteBatch.Draw(layer2,
                pos2,
                Color.White);
            }
        }

        public void Reset()
        {
            pos0 = offset0;
            pos1 = offset1;
            pos2 = offset2;

            scollSpeed.X = 0;
            scollSpeed.Y = 0;
        }
    }
}
