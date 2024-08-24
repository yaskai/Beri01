using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00.Entities
{
    public class Platform
    {
        public bool isActive = true;

        Game1 game;
        Handler handler;
        private Random rand;

        public Vector2 position;
        public Vector2 tilePos;
        public int type;

        Spritesheet break_ss;
        Animation break_anim;

        Spritesheet oneWay_ss;

        public int breakTimer = 400;
        public bool breaking = false;
        public bool hidden = false;
        public bool collision = true;
        private Vector2 drawOffset = Vector2.Zero;
        private float alpha = 1f;
        public Platform(Vector2 position, int type, Handler handler, Game1 game)
        {
            this.position = position;
            this.type = type;
            this.handler = handler;
            this.game = game;

            rand = handler.rand;

            break_ss = game.fblock_ss;
            break_anim = new Animation(20, 7, new Vector2(0, 0), new Vector2(32, 32), break_ss, game.fblockPNG, game);

            tilePos.X = (int)(position.X / 32);
            tilePos.Y = (int)(position.Y / 32);
        }

        public void Update(GameTime gameTime)
        {
            if (break_anim.frame < 1) break_anim.speed = 24; else break_anim.speed = 7;

            if (break_anim.loops > 0 || break_anim.f > 18)
            {
                breaking = false;
                hidden = true;
                collision = false;
            }

            if (breaking)
            {
                break_anim.Update(gameTime);
                if (break_anim.f > 12) collision = false;
                else collision = true;
            }
            else if (!breaking)
            {
                if (hidden)
                {
                    break_anim.f = 19;
                    collision = false;
                    breakTimer--;
                }
                else
                {
                    break_anim.f = 0;
                }
            }
        }

        public void FixedUpdate()
        {
            if (type == 0)
            {

            }
            else if (type == 1)
            {
                /*
                if (breaking)
                {
                    breakTimer --;
                }
                */

                if (breaking)
                {
                    if (break_anim.f < 4)
                    {
                        int range = 1;
                        drawOffset.X = rand.Next(-range, range);
                        drawOffset.Y = rand.Next(-range, range);
                    }
                    else if (break_anim.f > 12)
                    {
                        drawOffset.Y ++;
                        alpha -= 0.1f;
                    }
                    else 
                    {
                        drawOffset = Vector2.Zero;
                        alpha = 1f;
                    };
                }
                else 
                {
                    drawOffset = Vector2.Zero;
                    alpha = 1f;
                }

                if (hidden)
                {
                    breakTimer --;
                    if (breakTimer <= 0)
                    {
                        Reset();
                    }
                }
                else breakTimer = 7200;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (type == 0)
            {

            }
            else if (type == 1)
            {
                /*
                if (breaking)
                {
                    
                    if (breakTimer > 0)
                    {
                        break_anim.Draw(_spriteBatch, position, Vector2.One, SpriteEffects.None, Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(game.fblock_ss.frames[19, 0], position, Color.White);
                    }
                    
                }
                */

                if (breaking)
                {
                    break_anim.Draw(_spriteBatch, position + drawOffset, Vector2.One, SpriteEffects.None, Color.White * alpha);
                }
                else if (!breaking && !hidden)
                {
                    _spriteBatch.Draw(game.fblock_ss.frames[0, 0], position, Color.White);
                }
                /*
                else if (hidden)
                {
                    // dont draw
                }
                */
            }
        }

        public void Reset()
        {
            breakTimer = 400;
            collision = true;
            hidden = false;
            breaking = false;
            break_anim = new Animation(20, 7, new Vector2(0, 0), new Vector2(32, 32), break_ss, game.fblockPNG, game);
        }
    }
}