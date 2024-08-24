using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;

namespace Beri00.Entities
{
    public class Enemy
    {
        public bool isActive = true;
        public bool inFrame = false;

        Game1 game;
        Handler handler;
        Spritesheet ss;

        string type;

        public Vector2 position;
        private Vector2 prevPosition;
        public Vector2 tileMapOrigin;
        public Vector2 velocity;
        public string dir = "l";

        public int tileMapX = 0;
        public int tileMapY = 0;

        public Rectangle bounds;

        private Rectangle lft;
        private Rectangle rgt;
        private Rectangle bot;
        private Rectangle top;

        private Vector2 drawPosition;

        private bool ledgeLft = false;
        private bool ledgeRgt = false;

        private SpriteEffects effects = SpriteEffects.None;
        Animation walk;

        private Vector2 nextPos;
        public Rectangle nextBounds;

        public bool hurt = false;
        public int damageTimer = 0;
        public int hp = 2;
        public bool hitPlayer = false;
        public bool onGround = false;

        private float gravity = 0.3f;
        private int healTimer = 120;

        public Enemy(Vector2 position, string type, Handler handler, Game1 game)
        {
            this.game = game;
            this.handler = handler;
            this.type = type;
            this.position = position;

            switch (type)
            {
                case "can00":
                    ss = game.can_enemy_spritesheet;
                    walk = new Animation(5, 14, Vector2.Zero, new Vector2(32, 32), ss, game.can_enemy_spritesheetPNG, game);
                    break;
                case "can01":
                    ss = game.can_enemy_spritesheet;
                    break;
            }

            tileMapOrigin.X = (int)Math.Round(position.X / 32, 0);
            tileMapOrigin.Y = (int)Math.Round(position.Y / 32, 0);

            tileMapX = (int)tileMapOrigin.X;
            tileMapY = (int)tileMapOrigin.Y;

            nextPos = position;
        }

        public void Update(GameTime gameTime)
        {
            /*
            if (position.X > game.camera.position.X - (game.virtualWidth / 2) - 32
                && position.X < game.camera.position.X + (game.virtualWidth / 2) + 32
                && position.Y > game.camera.position.Y - (game.virtualHeight / 2) - 32
                && position.Y < game.camera.position.Y + (game.virtualHeight / 2) + 32)
            {
                inFrame = true;
            }
            else inFrame = false;
            
            //position.X += velocity.X * (float)game.delta;
            //position.Y += velocity.Y * (float)game.delta;
            */
            switch (type)
            {
                case "can00":
                    walk.Update(gameTime);
                    break;
                case "can01":
                    break;
            }
            /*
            if (inFrame)
            {
                if (dir == "l") effects = SpriteEffects.None; else if (dir == "r") effects = SpriteEffects.FlipHorizontally;

                tileMapX = (int)Math.Round(position.X / 32, 0);
                tileMapY = (int)Math.Round(position.Y / 32, 0);
            }

            SetColliders();
            */
        }

        public void FixedUpdate()
        {
            //if (!inFrame) isActive = false;
            CheckGround();

            prevPosition = position;
            
            if (position.X > game.camera.position.X - (game.virtualWidth / 2) - 32
                && position.X < game.camera.position.X + (game.virtualWidth / 2) + 32
                && position.Y > game.camera.position.Y - (game.virtualHeight / 2) - 32
                && position.Y < game.camera.position.Y + (game.virtualHeight / 2) + 32)
            {
                inFrame = true;
            }
            else inFrame = false;

            position.X += velocity.X;
            position.Y += velocity.Y;
            if (!onGround) velocity.Y += gravity;

            switch (type)
            {
                case "can00":
                if (!hurt) BasicWalk();
                break;

                case "can01":
                break;
            }

            if (inFrame)
            {
                if (dir == "l") effects = SpriteEffects.None; else if (dir == "r") effects = SpriteEffects.FlipHorizontally;

                tileMapX = (int)Math.Round(position.X / 32, 0);
                tileMapY = (int)Math.Round(position.Y / 32, 0);
            }

            SetColliders();

            damageTimer --;
            if (damageTimer < 0) 
            {
                damageTimer = 0;
                if (hp < 1) isActive = false;
            }

            if(hp < 2) healTimer --;
            if (healTimer < 0)
            {
                if (hp < 2) hp = 2;
                hurt = false;
                healTimer = 120;
            }

            if (hitPlayer)
            {
                foreach (Player p in handler.players)
                {
                    if (p.damageTimer - p.damageTimer / 2 > 0) 
                    {
                        int mod = 6;
                        if (!hurt) mod = 8;
                        if (p.hitDir == "l")
                        {
                            velocity.X = -1 - (p.damageTimer / mod);
                            if (game.tilemap.ContainsCollider(new Vector2(position.X - 1, position.Y + 16)))
                            {
                                velocity.X = 0;
                            }
                        }
                        else if (p.hitDir == "r")
                        {
                            velocity.X = 1 + (p.damageTimer / mod);
                            if (game.tilemap.ContainsCollider(new Vector2(position.X + 33, position.Y + 16)))
                            {
                                velocity.X = 0;
                            }
                        }
                    }

                    if (p.damageTimer <= 0) hitPlayer = false;
                }
            }

            if (hurt)
            {
                if (velocity.X > 0) velocity.X --;
                else if (velocity.X < 0) velocity.X++;
            }

            if (position.Y > game.tilemap.h * 32) isActive = false;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            drawPosition = Vector2.Lerp(prevPosition, position, game.ALPHA);
            drawPosition.Y -= 2;

            switch (type)
            {
                case "can00":
                    DrawCan00(_spriteBatch);
                    break;
                case "can01":
                    DrawCan01(_spriteBatch);
                    break;
            }
        }

        private void BasicWalk()
        {
            //walk.Update(gameTime);

            if (dir == "l")
            {
                //velocity.X = -1;
                if (walk.frame == 0)
                {
                    velocity.X = -1f;
                }
                else if (walk.frame == 1)
                {
                    velocity.X = -1f;
                }
                else if (walk.frame == 2)
                {
                    velocity.X = -0.8f;
                }
                else if (walk.frame == 3)
                {
                    velocity.X = -0.6f;
                }
                else if (walk.frame == 4)
                {
                    velocity.X = -0.2f;
                }
            }
            else if (dir == "r")
            {
                //velocity.X = 1;
                if (walk.frame == 0)
                {
                    velocity.X = 1f;
                }
                else if (walk.frame == 1)
                {
                    velocity.X = 1f;
                }
                else if (walk.frame == 2)
                {
                    velocity.X = 0.8f;
                }
                else if (walk.frame == 3)
                {
                    velocity.X = 0.6f;
                }
                else if (walk.frame == 4)
                {
                    velocity.X = 0.2f;
                }
            }

            velocity.X *= 2;
            nextPos = position + velocity;
            nextBounds = new Rectangle((int)nextPos.X, (int)nextPos.Y + 4, 32, 28);

            Rectangle ledgeCheckLft = new Rectangle((int)position.X - 1, (int)position.Y + 33, 1, 1);
            Rectangle ledgeCheckRgt = new Rectangle((int)position.X + 32, (int)position.Y + 33, 1, 1);

            ledgeLft = false;
            ledgeRgt = false;

            foreach (Tile t in handler.tiles)
            {
                if (t.position.X > position.X - 64 &&
                    t.position.X < position.X + 64 &&
                    t.position.Y > position.Y - 64 &&
                    t.position.Y < position.Y + 64)
                {
                    Rectangle tileBox = new Rectangle((int)t.position.X, (int)t.position.Y, 32, 32);

                    if (lft.Intersects(tileBox))
                    {
                        dir = "r";
                    }
                    else if (rgt.Intersects(tileBox))
                    {
                        dir = "l";
                    }

                    if (ledgeCheckLft.Intersects(tileBox))
                    {
                        ledgeLft = true;
                    }

                    if (ledgeCheckRgt.Intersects(tileBox))
                    {
                        ledgeRgt = true;
                    }
                }
            }

            if (dir == "l")
            {
                if (!ledgeLft) dir = "r";
            }
            else if (dir == "r")
            {
                if (!ledgeRgt) dir = "l";
            }
        }

        private void DrawCan00(SpriteBatch _spriteBatch)
        {
            //_spriteBatch.Draw(ss.frames[0, 0], position, Color.White);
            Color c = Color.White;
            /*
            if(!hurt) walk.Draw(_spriteBatch, drawPosition, new Vector2(1, 1), effects, c);
            else if (hurt) 
            {
                if (damageTimer > 0)
                {
                    _spriteBatch.Draw(ss.frames[5, 0], drawPosition, null, c, 0, Vector2.Zero, Vector2.One, effects, 3);
                }
                else _spriteBatch.Draw(ss.frames[6, 0], drawPosition, null, c, 0, Vector2.Zero, Vector2.One, effects, 3);
            }
            */
            if (hp == 2)
            {
                walk.Draw(_spriteBatch, drawPosition, new Vector2(1, 1), effects, c);
            }
            else
            {
                if (damageTimer > 0)
                {
                    _spriteBatch.Draw(ss.frames[6, 0], drawPosition, null, c, 0, Vector2.Zero, Vector2.One, effects, 3);
                }
                else _spriteBatch.Draw(ss.frames[5, 0], new Vector2(drawPosition.X, drawPosition.Y + 1), null, c, 0, Vector2.Zero, Vector2.One, effects, 3);
            }
            
        }

        private void DrawCan01(SpriteBatch _spriteBatch)
        {

        }

        private void SetColliders()
        {
            lft = new Rectangle((int)position.X, (int)position.Y + 1, 1, 30);
            rgt = new Rectangle((int)position.X + 31, (int)position.Y + 1, 1, 30);
            bot = new Rectangle((int)position.X + 1, (int)position.Y + 32, 30, 1);
            top = new Rectangle((int)position.X + 1, (int)position.Y, 30, 1);

            bounds = new Rectangle((int)position.X, (int)position.Y + 4, 32, 28);
        }

        public void OnStomp()
        {
            if (!hurt)
            {
                hurt = true;

                if (dir == "l")
                {
                    velocity.X = 4;
                }
                else if (dir == "r")
                {
                    velocity.X = -4;
                }
            }

            hp --;
            damageTimer = 6;
            healTimer = 240; 
        }

        public void CheckGround()
        {
            onGround = false;

            Vector2 groundA = new Vector2(position.X + 1, position.Y + 34);
            Vector2 groundB = new Vector2(position.X + 31, position.Y + 34);

            if (game.tilemap.ContainsCollider(groundA) || game.tilemap.ContainsCollider(groundB))
            {
                onGround = true;
                velocity.Y = 0;
                if (position.Y % 32 != 0) 
                {
                    position.X = tileMapX * 32;
                    position.Y = tileMapY * 32;
                }
            }
            else
            {
                onGround = false;
                hurt = true;
                if (hp == 2) hp = 1;
                healTimer = 120;
            }
        }
    }
}
