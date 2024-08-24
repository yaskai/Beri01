using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Formats.Asn1;

namespace Beri00.Entities
{
    public class Player
    {
        public bool isActive = true;

        Game1 game;
        Handler handler;
        Spritesheet ss;
        Tilemap tilemap;

        private Vector2 scale = new Vector2(1, 1);

        Animation run;
        Animation fall;
        Animation fly;
        Animation flip;

        public Vector2 position;
        private string dir = "r";

        public Vector2 drawPosition;

        public int tileMapX = 0;
        public int tileMapY = 0;

        public Vector2 tileLft;
        public Vector2 tileRgt;
        public Vector2 tileTop;
        public Vector2 tileBot;

        private SpriteEffects effects = SpriteEffects.None;

        private enum State
        {
            idle,
            run,
            jump,
            fall,
            land,
            fly,
            turn,
            death
        };

        private State animState = State.idle;

        public enum InputMethod
        {
            Keyboard,
            Controller
        };

        public InputMethod method = InputMethod.Keyboard;

        private float delta;

        public Vector2 velocity;
        private Vector2 maxVelocity;
        private Vector2 acceleration;
        private float accelerationMultiplier = 0.09f;
        private float decelerationMultiplier = 0.15f;
        private float speed = 0;
        private Vector2 prevPosition = Vector2.Zero;

        private float gravity = 0.065f;

        private bool onGround = true;
        private double groundTimer = 1;
        private double airTimer = 0;
        public Vector2 lastGroundPosition;

        private bool onSlope = false;
        private int slopeDir = 0;

        private bool jumpBuffered = false;
        private double jumpBufferTimer = 0;

        private double turnTimer = 0;

        private double jumpTimer = 0;
        private double jumpReleaseTimer = 0;
        private bool jumpEnded = true;
        private double jumpTimerMax = 45;
        private bool jumpCut = false;

        private bool GLIDE = false;
        private double glideTimer = 0;

        private double flyTimer = 0;
        //private double flyTimerMax = 115;
        private double flyTimerMax = 75;
        private bool flyUsed = false;
        public bool FLY = false;
        private double flyEndTimer = 0;

        private double trailCooldown = 5;
        private double vibrateResetTimer = 5;

        public double respawnTimer = 30;

        private bool onWallLft = false;
        private bool onWallRgt = false;
        private double wallJumpTimer = 0;
        private string wallJumpDir = "n";

        private string flipDir = "r";

        public Rectangle bounds;

        public Rectangle lft;
        public Rectangle rgt;
        public Rectangle bot;
        public Rectangle top;

        public Rectangle wallLft;
        public Rectangle wallRgt;

        public bool lockCamera = false;

        private Vector2 checkpointPosition;
        private int enemyCollisionTimer = 0;
        private int fixedJumpTimer;
        private int fixedJumpBufferTimer = 0;
        public Rectangle hitBox;
        //public int hp = 3000;
        public int hp = 5;
        public int damageTimer = 0;
        private int invFrames = 0;
        public string hitDir = "r";

        public double landBuffer = 0;

        private int maxFlips = 1;
        
        private bool onDoor = false;

        private Vector2 debugTilePos;
        private String debugMessage0 = "?";
        private string debugMessage1 = "?";

        public Player(Vector2 position, Game1 game, Handler handler, Spritesheet ss, Tilemap tilemap)
        {
            this.position = position;
            this.game = game;
            this.handler = handler;
            this.ss = ss;
            this.tilemap = tilemap;

            run = new Animation(4, 10, new Vector2(0, 1), new Vector2(32, 32), ss, game.player_spritesheetPNG, game);
            fly = new Animation(4, 9, new Vector2(1, 3), new Vector2(32, 8), ss, game.player_spritesheetPNG, game);
            fall = new Animation(1, 20, new Vector2(3, 2), new Vector2(32, 32), ss, game.player_spritesheetPNG, game);
            flip = new Animation(8, 7, new Vector2(1, 4), new Vector2(32, 32), ss, game.player_spritesheetPNG, game);

            run.spriteLayer = 3;
            fly.spriteLayer = 2;
            fall.spriteLayer = 3;
            flip.spriteLayer = 3;

            checkpointPosition = position;
        }

        public void Update(GameTime gameTime)
        {
            delta = game.delta;

            KeyboardState kb = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (padState.IsConnected) method = InputMethod.Controller; else method = InputMethod.Keyboard;

            ManageTimers();
            if (onGround) fly.SetFrame(0);

            if (animState != State.death)
            {
                if (method == InputMethod.Keyboard) KeyboardInput(kb, padState); else if (method == InputMethod.Controller) ControllerInput(padState, kb);
            }

            if (wallJumpTimer > 0)
            {
                dir = wallJumpDir;
            }

            switch (animState)
            {
                case State.idle:
                    break;
                case State.run:
                    run.Update(gameTime);
                    break;
                case State.jump:
                    if (jumpTimer <= 25) flip.Update(gameTime);
                    break;
                case State.fall:
                    if (jumpTimer <= 25) flip.Update(gameTime);
                    fall.Update(gameTime);
                    break;
                case State.land:
                    break;
                case State.fly:
                    jumpTimer = 0;
                    flip.loops = maxFlips;
                    if (flyTimer > 3) fly.Update(gameTime); else fly.SetFrame(0); 
                    break;
                case State.turn:
                    break;
                case State.death:
                    break;
            }

            //LimitPhysicsValues();
            SetBounds();

            if (dir == "l")
            {
                velocity.X = acceleration.X * -1;
            }
            else if (dir == "r")
            {
                velocity.X = acceleration.X;
            }
        }

        public void FixedUpdate()
        {
            //ManageTimers();

            if (animState != State.death)
            {
                CalculateTilemapPosition();

                //Collision();
                //if (dir == "l") MoveX(-acceleration.X); else if (dir == "r") MoveX(acceleration.X);

                prevPosition = position;

                damageTimer --;
                if (damageTimer < 0) damageTimer = 0;

                enemyCollisionTimer --;
                if (enemyCollisionTimer < 0) enemyCollisionTimer = 0;
                
                Collision();
                
                if (acceleration.X != 0)
                {
                    if (dir == "l")
                    {
                        //if (!(damageTimer > 0 && hitDir == "r")) MoveX(-acceleration.X * game.delta1 * 0.1f);
                        MoveX(-acceleration.X * game.delta1 * 0.1f);
                    }
                    else if (dir == "r")
                    {
                        //if (!(damageTimer > 0 && hitDir == "l")) MoveX(acceleration.X * game.delta1 * 0.1f);
                        MoveX(acceleration.X * game.delta1 * 0.1f);
                    }
                }
                
                if (velocity.Y != 0) MoveY(velocity.Y * game.delta1 * 0.1f);
                velocity.Y += gravity * game.delta1 * 0.1f;

                if (wallJumpTimer > 0)
                {
                    if (wallJumpDir == "l")
                    {
                        MoveX(-4.25f);
                        dir = "l";
                    }
                    else if (wallJumpDir == "r")
                    {
                        MoveX(4.25f);
                        dir = "r";
                    }
                }
                
                if (damageTimer > 0 && onGround)
                {
                    if (hitDir == "l")
                    {
                        MoveX(damageTimer * 0.45f);
                    }
                    else if (hitDir == "r")
                    {
                        MoveX(-damageTimer * 0.45f);
                    }
                    
                    velocity.X = 0;
                    acceleration.X = 0;
                    /*
                    if (groundTimer > 2 && !onGround) 
                    {
                        if (hitDir == "l") MoveX((damageTimer + 1) * 0.15f);
                        else if (hitDir == "r") MoveX(-(damageTimer + 1) * 0.15f);

                        damageTimer = 0;
                    }
                    */
                }
            }

            invFrames --;
            if (invFrames < 0) invFrames = 0;

            if (hp < 1)
            {
                Die();
                hp = 3;
            }

            LimitPhysicsValues();
            //SetBounds();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            drawPosition = Vector2.Lerp(prevPosition, position, game.ALPHA);

            Color c = Color.White;
            if (damageTimer > 0 && (damageTimer % 2 == 0)) c = Color.Black * 0.5f;
            if (dir == "l") effects = SpriteEffects.FlipHorizontally; else if (dir == "r") effects = SpriteEffects.None;

            switch (animState)
            {
                case State.idle:
                    //_spriteBatch.Draw(ss.frames[0, 0], new Rectangle((int)position.X, (int)position.Y, 32, 32), null, Color.White, 0, Vector2.Zero, effects, 1);
                    _spriteBatch.Draw(ss.frames[0, 0], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                    break;

                case State.run:
                    run.Draw(_spriteBatch, new Vector2(drawPosition.X, drawPosition.Y - 1), new Vector2(1, 1), effects, c);
                    break;

                case State.jump:
                    _spriteBatch.Draw(ss.frames[0, 4], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                    break;

                case State.fall:
                    if (jumpTimer > 35)
                    {
                        //_spriteBatch.Draw(ss.frames[0, 2], new Rectangle((int)position.X, (int)position.Y, 32, 32), null, Color.White, 0, Vector2.Zero, effects, 1);
                        _spriteBatch.Draw(ss.frames[0, 2], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                    }
                    else if (jumpTimer <= 35 && flip.loops < maxFlips)
                    {
                        flip.Draw(_spriteBatch, drawPosition, new Vector2(1, 1), effects, c);
                    }
                    else
                    {
                        /*
                        if (velocity.Y <= 1f)
                        {
                            _spriteBatch.Draw(ss.frames[1, 4], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                        }
                        else if (velocity.Y > 1f)
                        {
                            _spriteBatch.Draw(ss.frames[2, 2], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                        }
                        */

                        _spriteBatch.Draw(ss.frames[2, 2], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                    }
                    break;

                case State.land:
                    _spriteBatch.Draw(ss.frames[4, 2], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                    break;

                case State.fly:
                    fly.Draw(_spriteBatch, new Vector2(drawPosition.X, drawPosition.Y - 1), scale, SpriteEffects.None, c);
                    _spriteBatch.Draw(ss.frames[0, 3], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), effects, 3);
                    break;

                case State.turn:
                    if (dir == "r")
                    {
                        _spriteBatch.Draw(ss.frames[4, 1], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), SpriteEffects.FlipHorizontally, 3);
                    }
                    else if (dir == "l")
                    {
                        _spriteBatch.Draw(ss.frames[4, 1], drawPosition, null, c, 0, new Vector2(1, 1), new Vector2(1, 1), SpriteEffects.None, 3);
                    }

                    break;
                case State.death:
                    break;
            }
            /*
            _spriteBatch.Draw(game.blank_spritesheet.frames[12, 0],
            debugTilePos,
            null,
            Color.White,
            0,
            new Vector2(1, 1),
            new Vector2(32, 32),
            SpriteEffects.None,
            1);
            
            debugTilePos = new Vector2(drawPosition.X + 32, drawPosition.Y + 32);
            */
            //_spriteBatch.DrawString(game.debugFont, debugMessage0, new Vector2(position.X, position.Y - 16), Color.White);
            //_spriteBatch.DrawString(game.debugFont, debugMessage1, position, Color.White);
        }

        private void Collision()
        {
            onGround = false;
            onWallLft = false;
            onWallRgt = false;
            onDoor = false;
            onSlope = false;

            if (position.X < 0) position.X = 0;
            if (position.X > (tilemap.w - 1) * 32) position.X = (tilemap.w - 1) * 32;
            if (position.Y > tilemap.h * 32) Die();

            Vector2 lftWall = new Vector2(position.X - 2, position.Y + 16);
            Vector2 rgtWall = new Vector2(position.X + 34, position.Y + 16);

            if (tilemap.ContainsCollider(lftWall)) onWallLft = true; 
            else if (tilemap.ContainsCollider(rgtWall)) onWallRgt = true;

            int px = tileMapX;
            int py = tileMapY;

            int flft = px - 1;
            //int flft = game.camera.x - 12;
            if (flft < 0) flft = 0;

            int frgt = px + 2;
            //int frgt = game.camera.x + 12;
            if (frgt > tilemap.w) frgt = tilemap.w;

            int ftop = py - 1;
            //int ftop = game.camera.y - 7;
            if (ftop < 0) ftop = 0;

            int fbot = py + 2;
            //int fbot = game.camera.y + 7;
            if (fbot > tilemap.h) fbot = tilemap.h;

            // Slope checks
            /*
            Vector2 pointA = new Vector2(position.X, position.Y + 31);
            Vector2 pointB = new Vector2(position.X + 31, position.Y + 31);

            for (int n = 0; n < 32; n++)
            {
                Vector2 newPointA = new Vector2(position.X, position.Y + 31 + n);
                Vector2 newPointB = new Vector2(position.X + 31, position.Y + 31 + n);

                char a = tilemap.TileFromPoint(newPointA);
                char b = tilemap.TileFromPoint(newPointB);
                
                int a_col = (int)tilemap.TileCoordsFromPoint(newPointA).X;
                int a_row = (int)tilemap.TileCoordsFromPoint(newPointA).Y;
                int b_col = (int)tilemap.TileCoordsFromPoint(newPointB).X;
                int b_row = (int)tilemap.TileCoordsFromPoint(newPointB).Y;

                switch (a)
                {
                    case '2': case '3': case '4': case '5':
                    Rectangle slope_a = new Rectangle(a_col * 32, a_row * 32, 32, 32);
                    if (this.bounds.Intersects(slope_a)) SlopeCollision(a_col, a_row);
                    break;
                }

                switch (b)
                {
                    case '2': case '3': case '4': case '5':
                    Rectangle slope_b = new Rectangle(b_col * 32, b_row * 32, 32, 32);
                    if (this.bounds.Intersects(slope_b)) SlopeCollision(b_col, b_row);
                    break;
                }
            }
            */
            for (int r = ftop; r < fbot; r++)
            {
                for (int c = flft; c < frgt; c++)
                {
                    switch (tilemap.mapData[c, r])
                    {
                        case '2': case '3': case '4': case '5':
                        SlopeCollision(c, r);
                        onSlope = true;
                        break;

                        case '1':
                        Tile t = tilemap.tileList[c, r];
                        Rectangle tilebox = new Rectangle((int)t.position.X, (int)t.position.Y, 32, 32);
                        
                        //if (lft.Intersects(tilebox)) onWallLft = true; else if (rgt.Intersects(tilebox)) onWallRgt = true;
                        if (bot.Intersects(tilebox) && !onSlope)
                        {
                            lastGroundPosition.X = position.X;
                            lastGroundPosition.Y = t.position.Y - 32;
                            
                            onGround = true;
                            float dif = (position.Y + 32) - t.position.Y;
                            position.Y -= dif;
                            velocity.Y = 0;
                            
                            //position.Y = t.position.Y - 32;
                            //velocity.Y = 0;
                        }
                        break;

                        case '^':
                        Rectangle h0 = new Rectangle(c * 32, r * 32 + 16, 32, 32 - 16);
                        if (bounds.Intersects(h0)) Die();
                        break;

                        case 'v':
                        Rectangle h1 = new Rectangle(c * 32, r * 32, 32, 32 - 16);
                        if (bounds.Intersects(h1)) Die();
                        break;
                        
                        case 'f':
                        Platform p0 = tilemap.platforms[c, r];
                        Rectangle platBox0 = new Rectangle((int)(p0.position.X), (int)p0.position.Y + 1, 32, 16);
                        if (bot.Intersects(platBox0) && p0.collision)
                        {
                            lastGroundPosition.X = position.X;
                            lastGroundPosition.Y = p0.position.Y - 31;
                            
                            onGround = true;
                            float dif = (position.Y + 31) - p0.position.Y;
                            position.Y -= dif;
                            velocity.Y = 0;

                            if (p0.type == 1)
                            {
                                if (!p0.breaking)
                                {
                                    p0.breaking = true;
                                }
                            }
                            
                            //position.Y = t.position.Y - 32;
                            //velocity.Y = 0;
                        }
                        break;

                        case 'd':
                        Rectangle doorBox = new Rectangle(c * 32, r * 32, 32, 32);
                        if (this.bounds.Intersects(doorBox)) onDoor = true;
                        break;
                    }
                }
            }

            // slope check
            
            
            if (enemyCollisionTimer <= 0) EnemyCollision();
        }

        private void KeyboardInput(KeyboardState kb, GamePadState padState)
        {
            // horizontal movement
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D))
            {
                //if ((onGround && animState == State.idle) || (onGround && animState == State.turn && acceleration.X > 0.2f)) animState = State.run;
                if (onGround && (animState == State.idle || (animState == State.turn && turnTimer <= 0))) animState = State.run;

                if (kb.IsKeyDown(Keys.A))
                {
                    // turn check
                    if (dir == "r" && acceleration.X > 0)
                    {
                        //acceleration.X = 0;
                        acceleration *= 0.025f;
                        if (onGround) animState = State.turn;
                        turnTimer = 5;
                    }

                    dir = "l";
                    //MoveX(-1 * (float)game.delta);
                    //MoveX((float)game.delta * -acceleration.X); // move left
                }
                else if (kb.IsKeyDown(Keys.D))
                {
                    // turn check
                    if (dir == "l" && acceleration.X > 0)
                    {
                        //acceleration.X = 0;
                        acceleration *= 0.025f;
                        if (onGround) animState = State.turn;
                        turnTimer = 5;
                    }

                    dir = "r";
                    //MoveX(1 * (float)game.delta);
                    //MoveX((float)game.delta * acceleration.X); // move right
                }
                // add horizontal acceleration
                acceleration.X += delta * accelerationMultiplier;

            }
            else if (kb.IsKeyUp(Keys.A) && kb.IsKeyUp(Keys.D))
            {
                if (onGround && animState == State.run) animState = State.idle;
                acceleration.X -= delta * decelerationMultiplier;
                if (acceleration.X < 0.2f) acceleration.X = 0;
            }

            // jump
            if (onGround)
            {
                if (kb.IsKeyDown(Keys.Space))
                {
                    if (jumpReleaseTimer <= 0)
                    {
                        if (acceleration.X < 2.05f)
                        {
                            StartJump(7.75f, kb, padState);
                        }
                        else if (acceleration.X >= 2.05f)
                        {
                            StartJump(8.25f, kb, padState);
                        }
                    }
                    jumpReleaseTimer = 2;
                }
            }
            else if (!onGround)
            {
                if (!jumpEnded && jumpTimer > 0 && jumpTimer < 35) EndJump(kb, padState);
                if (jumpEnded && kb.IsKeyDown(Keys.Space)) 
                {
                    //jumpReleaseTimer = 10;

                    if (wallJumpTimer <= 0 && jumpReleaseTimer <= 0 && (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D)))
                    {
                        if (onWallLft)
                        {
                            WallJump(4.5f, "r");
                        }
                        else if (onWallRgt)
                        {
                            WallJump(4.5f, "l");
                        }
                    }

                    FLY = false;

                    if (flyTimer == flyTimerMax && jumpReleaseTimer <= 0 && jumpTimer <= 35 && wallJumpTimer <= 0)
                    {
                        //velocity.Y *= 0.5f;
                        acceleration.X *= 0.9f;
                        animState = State.fly;
                        FLY = true;
                    }
                    else if (flyTimer < flyTimerMax && flyTimer > 0 && wallJumpTimer <= 0)
                    {
                        if (animState == State.fly)
                        {
                            /*
                            if (acceleration.X > 0)
                            {
                                velocity.Y -= (float)game.delta * 0.25f;
                                if (velocity.Y < 0f) velocity.Y = 0f;
                            }
                            */
                            //velocity.Y -= (float)game.delta * 0.25f;
                            //if (velocity.Y < 0f) velocity.Y = 0f;
                            if (flyTimer > 0) velocity.Y = 0f;
                            FLY = true;
                        }
                    }

                    //jumpReleaseTimer = 10;
                }
            }

            if (kb.IsKeyDown(Keys.Space)) jumpReleaseTimer = 2;
            if (animState == State.fly && kb.IsKeyUp(Keys.Space)) animState = State.fall;
        }

        private void ControllerInput(GamePadState padState, KeyboardState kb)
        {
            // horizontal movement
            if (padState.ThumbSticks.Left.X < 0 || padState.ThumbSticks.Left.X > 0)
            {
                //if ((onGround && animState == State.idle) || (onGround && animState == State.turn && acceleration.X > 0.2f)) animState = State.run;
                if (onGround && (animState == State.idle || (animState == State.turn && turnTimer <= 0))) animState = State.run;

                if (padState.ThumbSticks.Left.X < 0)
                {
                    // turn check
                    if (dir == "r" && acceleration.X > 0)
                    {
                        acceleration *= 0.025f;
                        if (onGround) animState = State.turn;
                        turnTimer = 5;
                    }

                    dir = "l";
                }
                else if (padState.ThumbSticks.Left.X > 0)
                {
                    // turn check
                    if (dir == "l" && acceleration.X > 0)
                    {
                        acceleration *= 0.025f;
                        if (onGround) animState = State.turn;
                        turnTimer = 5;
                    }

                    dir = "r";
                }
                // add horizontal acceleration
                acceleration.X += delta * accelerationMultiplier;
            }
            else if (padState.ThumbSticks.Left.X == 0)
            {
                if (onGround && animState == State.run) animState = State.idle;
                acceleration.X -= delta * decelerationMultiplier;
                if (acceleration.X < 0.2f) acceleration.X = 0;
            }

            //buffer jump
            if (!onGround && velocity.Y > 0 && jumpReleaseTimer < 1.7f)
            {
                if (padState.IsButtonDown(Buttons.A))
                {
                    jumpBuffered = true;
                    jumpBufferTimer = 5;
                }
            }
            
            if (onGround && jumpBuffered)
            {
                if (acceleration.X < 2.05f)
                {
                    StartJump(7.75f, kb, padState);
                }
                else if (acceleration.X >= 2.05f)
                {
                    StartJump(8.25f, kb, padState);
                }
            }

            // jump
            if (onGround || (!onGround && airTimer > 4))
            {
                if (padState.IsButtonDown(Buttons.A))
                {
                    if (jumpReleaseTimer <= 0)
                    {
                        if (acceleration.X < 2.05f)
                        {
                            //StartJump(7.75f, kb, padState);
                            StartJump(7.65f, kb, padState);
                        }
                        else if (acceleration.X >= 2.05f)
                        {
                            //StartJump(8.25f, kb, padState);
                            StartJump(8.15f, kb, padState);
                        }
                    }
                    jumpReleaseTimer = 2;
                }
            }
            else if (!onGround)
            {
                if (!jumpEnded && jumpTimer > 0 && jumpTimer < 35) EndJump(kb, padState);
                if (jumpEnded && padState.IsButtonDown(Buttons.A))
                {
                    //jumpReleaseTimer = 10;

                    if (wallJumpTimer <= 0 && jumpReleaseTimer <= 0 && padState.ThumbSticks.Left.X != 0 && animState != State.fly && flyEndTimer <= 0)
                    {
                        if (onWallLft)
                        {
                            WallJump(3.45f, "r");
                        }
                        else if (onWallRgt)
                        {
                            WallJump(3.45f, "l");
                        }
                    }

                    FLY = false;

                    if (flyTimer == flyTimerMax && jumpReleaseTimer <= 0 && jumpTimer <= 35 && wallJumpTimer <= 0)
                    {
                        //velocity.Y *= 0.5f;
                        acceleration.X *= 0.9f;
                        animState = State.fly;
                        fly.SetFrame(0);
                        FLY = true;

                        //GamePad.SetVibration(PlayerIndex.One, 0.25f, 0.25f);
                    }
                    else if (flyTimer < flyTimerMax && flyTimer > 0 && wallJumpTimer <= 0)
                    {
                        if (animState == State.fly)
                        {
                            if (flyTimer > flyTimerMax - 2)
                            {
                                
                                //vibrateResetTimer = 7;
                            }
                            /*
                            if (acceleration.X > 0)
                            {
                                velocity.Y -= (float)game.delta * 0.25f;
                                if (velocity.Y < 0f) velocity.Y = 0f;
                            }
                            */

                            //velocity.Y -= (float)game.delta * 0.25f;
                            //if (velocity.Y < 0f) velocity.Y = 0f;
                            if (flyTimer > 0 && flyTimer < flyTimerMax - 1) velocity.Y = 0f;
                            FLY = true;
                        }
                    }

                    //jumpReleaseTimer = 10;
                }
            }

            //if (flyTimer < flyTimerMax && flyTimer > flyTimerMax - 3) GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);

            //if (padState.IsButtonDown(Buttons.A) && !jumpBuffered) jumpReleaseTimer = 2;
            if (padState.IsButtonDown(Buttons.A)) jumpReleaseTimer = 4;
            if (animState == State.fly && padState.IsButtonUp(Buttons.A))
            {
                animState = State.fall;
                //GamePad.SetVibration(PlayerIndex.One, 0.25f, 0.25f);
                //vibrateResetTimer = 8;

                jumpReleaseTimer = 2;
                //jumpBuffered = true;
                //jumpBufferTimer = 2;

                if (flyTimer > flyTimerMax - 2)
                {
                    flyUsed = false;
                    flyTimer = flyTimerMax;
                    FLY = false;
                }
            }

            if (onDoor)
            {
                if (padState.IsButtonDown(Buttons.DPadUp))
                {
                    //game.NextLevel();
                    Die();
                }
            }
        }

        private void MoveX(float amount)
        {   
            int passes = 4;
            float smallAmount = amount / passes;

            Vector2 nextA = Vector2.Zero;
            Vector2 nextB = Vector2.Zero;

            float newAmount = 0;

            for (int step = 0; step < passes; step++)
            {
                if (amount < 0)
                {
                    nextA = new Vector2(position.X + (smallAmount * step) - 1, position.Y + 1);
                    nextB = new Vector2(position.X + (smallAmount * step) - 1, position.Y + 31);
                }
                else if (amount > 0)
                {
                    nextA = new Vector2(position.X + 32 + (smallAmount * step) + 1, position.Y + 1);
                    nextB = new Vector2(position.X + 32 + (smallAmount * step) + 1, position.Y + 31);
                }

                if (tilemap.ContainsCollider(nextA) || tilemap.ContainsCollider(nextB))
                {
                    newAmount = 0;
                }
                else 
                {
                    /*
                    int nextB_col = (int)nextB.X / 32;
                    int nextB_row = (int)nextB.Y / 32;

                    bool slopeCheck = false;
                    if (tilemap.ContainsSlope(nextB)) slopeCheck = true;
                    
                    newAmount += smallAmount;
                    

                    if (slopeCheck) SlopeCollision(nextB_col, nextB_row);
                    */
                    newAmount += smallAmount;
                }
            }

            if (newAmount == 0) acceleration.X = 0;
            position.X += newAmount;
        }

        private void MoveY(float amount)
        {
            int passes = 4;
            float smallAmount = amount / passes;

            Vector2 nextA = Vector2.Zero;
            Vector2 nextB = Vector2.Zero;

            Vector2 nextCornerA = Vector2.Zero;
            Vector2 nextCornerB = Vector2.Zero;

            float newAmount = 0;

            for (int step = 0; step < passes; step++)
            {
                if (amount < 0)
                {
                    nextA = new Vector2(position.X + 2, (position.Y - 1) + (smallAmount * step));
                    nextB = new Vector2(position.X + 30, (position.Y - 1) + (smallAmount * step));
                    Vector2 nextC = new Vector2(nextA.X + 14, nextA.Y);

                    if (tilemap.ContainsCollider(nextA) || tilemap.ContainsCollider(nextB) || tilemap.ContainsCollider(nextC))
                    {
                        newAmount = 0;
                        velocity.Y = 0;
                    }
                    else newAmount += smallAmount;
                }
                else if (amount > 0)
                {
                    nextA = new Vector2(position.X + 3, position.Y + 32 + (smallAmount * step));
                    nextB = new Vector2(position.X + (32 - 3), position.Y + 32 + (smallAmount * step));
                    
                    if (tilemap.ContainsCollider(nextA) || tilemap.ContainsCollider(nextB))
                    {
                        newAmount = 0;
                        //position.Y -= smallAmount;

                        position.Y = tileMapY * 32;
                    }
                    else 
                    {
                        nextCornerA = new Vector2(position.X, position.Y + 32 + (smallAmount * step));
                        nextCornerB = new Vector2(position.X + 32, position.Y + 32 + (smallAmount * step));

                        bool coll_cornerA = false;
                        bool coll_cornerB = false;

                        int cornerCollisions = 0;

                        if (tilemap.ContainsCollider(nextCornerA))
                        {
                            coll_cornerA = true;
                            cornerCollisions++;
                        }

                        if (tilemap.ContainsCollider(nextCornerB))
                        {
                            coll_cornerB = true;
                            cornerCollisions++;
                        }

                        if (cornerCollisions == 1)
                        {
                            if (coll_cornerA)
                            {
                                position.X++;
                            }
                            else if (coll_cornerB)
                            {
                                position.X--;
                            }
                        }

                        newAmount += smallAmount;
                    }
                }
            }
            
            position.Y += newAmount;
        }

        private void SlopeCollision(int c, int r)
        {
            int[] heightmap = tilemap.slope_heightmap_2;
            char tileChar = '0';

            switch (tileChar)
            {
                case '2':
                heightmap = tilemap.slope_heightmap_2;
                break;

                case '3':
                heightmap = tilemap.slope_heightmap_3;
                break;

                case '4':
                heightmap = tilemap.slope_heightmap_4;
                break;

                case '5':
                heightmap = tilemap.slope_heightmap_5;
                break;
            }

            Tile t = tilemap.tileList[c, r];
            int relativeX = (int)(position.X + 32 - t.position.X);
            //int relativeX = (int)(position.X - t.position.X);

            //Vector2 point = new Vector2(position.X + 31, position.Y + 31);

            if (relativeX < 0) relativeX = 0; else if (relativeX > 31) relativeX = 31;

            if (relativeX >= 0 && relativeX < 32)
            {
                debugMessage0 = new string("relativeX: " + relativeX);
                debugMessage1 = new string("hmap: " + heightmap[relativeX]);
                //debugTilePos = new Vector2(t.position.X, t.position.Y + 32 - heightmap[relativeX]);

                if (position.Y + 31 >= t.position.Y + heightmap[relativeX])
                {
                    position.Y = t.position.Y + heightmap[relativeX] - 31;
                    onGround = true;
                }

                //Rectangle box = new Rectangle((int)t.position.X, (int)t.position.Y + heightmap[relativeX] - 32, 32, 32);
                //if (this.bounds.Intersects(box)) position.Y = t.position.Y + heightmap[relativeX] - 32;

                //position.Y = t.position.Y + heightmap[relativeX] - 32;
            }
        }

        private void StartJump(float amount, KeyboardState kb, GamePadState padState)
        {
            onGround = false;
            velocity.Y = amount * -1;
            jumpTimer = jumpTimerMax;
            jumpReleaseTimer = 2;
            jumpEnded = false;
            position.Y -= delta;
            acceleration.X *= 0.85f;

            animState = State.jump;
            flip.SetFrame(0);
            flip.loops = 0;

            flipDir = dir;

            jumpBuffered = false;
            jumpBufferTimer = 0;

            //EndJump(kb);
        }

        private void EndJump(KeyboardState kb, GamePadState padState)
        {
            jumpCut = false;

            if (method == InputMethod.Keyboard)
            {
                if (kb.IsKeyUp(Keys.Space))
                {
                    velocity.Y *= 0.25f;
                    flip.loops = maxFlips;
                    jumpCut = true;
                }
            }
            else if (method == InputMethod.Controller)
            {
                if (padState.IsButtonUp(Buttons.A))
                {
                    velocity.Y *= 0.25f;
                    flip.loops = maxFlips;
                    jumpCut = true;
                }
            }

            jumpEnded = true;
            animState = State.fall;
            //Debug.WriteLine("JUMP ENDED AT " + jumpTimer);
        }

        private void WallJump(float amount, string dir)
        {
            velocity.Y = amount * -1;
            if (dir == "l")
            {
                wallJumpDir = "l";
            }
            else if (dir == "r")
            {
                wallJumpDir = "r";
            }

            wallJumpTimer = 20;

            flip.loops = maxFlips;

            GLIDE = false;
            glideTimer = 0;
        }

        private void SetSpeed()
        {
            if (acceleration.X <= 0.01f) 
            {
                speed = 0;
            }
            else if (acceleration.X > 0.01f && acceleration.X <= 1f)
            {
                speed = 8;
            }
            else if (acceleration.X > 1f && acceleration.X < 2f)
            {
                speed = 10;
            }
            else if (acceleration.X >= 2f)
            {
                speed = 12;
            }
        }
        private void LimitPhysicsValues()
        {
            if (animState != State.jump &&
                animState != State.fall &&
                animState != State.fly)
            {
                // on ground
                accelerationMultiplier = 0.09f;
                decelerationMultiplier = 0.15f;

                if (acceleration.X < 0f) acceleration.X = 0f; else if (acceleration.X > 2.0f * 0.85f) acceleration.X = 2.0f  * 0.85f;
            }
            else
            {
                // off ground
                if (animState == State.fly)
                {
                    accelerationMultiplier = 0.045f;
                    decelerationMultiplier = 0.035f;
                    if (acceleration.X < 0f) acceleration.X = 0f; else if (acceleration.X > 2.5f * 0.95f) acceleration.X = 2.5f * 0.95f;
                }
                else
                {
                    accelerationMultiplier = 0.085f;
                    decelerationMultiplier = 0.1f;
                    if (acceleration.X < 0f) acceleration.X = 0f; else if (acceleration.X > 2.15f * 0.85f) acceleration.X = 2.15f * 0.85f;
                }
            }

            if (velocity.X < -3f) velocity.X = -3f; else if (velocity.X > 3f) velocity.X = 3f;
            if (acceleration.Y < -4f) acceleration.Y = -4f; else if (acceleration.Y > 1f) acceleration.Y = 1f;

            if (animState == State.fly)
            {
                //gravity = 0.05f;
                //gravity = 0.001f;
                //gravity = 0f;

                gravity = 0.25f;

                if (velocity.Y < -2f) velocity.Y = -2f; else if (velocity.Y > 2.5f) velocity.Y = 2.5f;
            }
            else
            {
                //gravity = 0.065f;
                //gravity = 0.275f;

                // glide/hang time
                if (jumpTimer > 0 && !jumpCut)
                {
                    if (velocity.Y >= -0.05f && !GLIDE) 
                    {
                        glideTimer = 15;
                        GLIDE = true;
                    }
                }

                if (jumpTimer > 0)
                {
                    if (jumpCut)
                    {
                        if (jumpTimer > 10 && velocity.Y < 1.25f) gravity = 0.285f / 1.75f; else gravity = 0.285f;
                    }
                    else if (!jumpCut)
                    {
                        if (velocity.Y < 0.5f && velocity.Y > -1.5f && jumpTimer > 1)
                        {
                            gravity = 0.285f / 1.45f;
                        }
                        else gravity = 0.285f;
                    }
                }
                else
                {
                    gravity = 0.275f;
                }

                //if (GLIDE) gravity = 0.075f;
                if (GLIDE) gravity = 0.06f;

                if (onGround)
                {
                    GLIDE = false;
                    glideTimer = 0;
                }

                if (velocity.Y < -10f) velocity.Y = -10f; else if (velocity.Y > 4f) velocity.Y = 4f;
            }

            if (animState == State.death)
            {
                velocity = Vector2.Zero;
                acceleration = Vector2.Zero;
            }
        }

        private void ManageTimers()
        {
            glideTimer -= delta;
            if (glideTimer < 0)
            {
                GLIDE = false;
                glideTimer = 0;
            }

            flyEndTimer -= delta;
            if (flyEndTimer < 0) flyEndTimer = 0; else jumpReleaseTimer = 2;

            jumpBufferTimer -= delta;
            if (jumpBufferTimer < 0)
            {
                jumpBufferTimer = 0;
                jumpBuffered = false;
            }

            respawnTimer -= delta;
            if (respawnTimer < 0)
            {
                if (animState == State.death) Respawn();
                respawnTimer = 0;
            }

            vibrateResetTimer -= delta;
            if (vibrateResetTimer < 0)
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0);
                vibrateResetTimer = 10;
            }

            trailCooldown -= delta;
            if (trailCooldown < 0) trailCooldown = 0;

            wallJumpTimer -= delta;
            if (wallJumpTimer < 0) 
            {
                wallJumpTimer = 0;
                //lockCamera = false;
            }
            //else if (wallJumpTimer > 0) lockCamera = true;

            jumpTimer -= delta;
            if (jumpTimer < 0)
            {
                jumpCut = false;
                jumpTimer = 0;
            }

            turnTimer -= delta;
            if (turnTimer < 0) turnTimer = 0;

            if (airTimer < 0) airTimer = 0;

            if (onGround)
            {
                jumpTimer = 0;
                airTimer = 10;
                jumpCut = false;

                groundTimer -= delta;
                if (groundTimer < 10 && groundTimer > 2)
                {
                    if (landBuffer <= 0) animState = State.land;
                    if (acceleration.X > 2.15f * 0.85f) acceleration.X = 2.15f * 0.85f;
                }
                if (groundTimer < 2 && animState == State.land) animState = State.idle;
                if (groundTimer < 0) groundTimer = 0;
                flyTimer = flyTimerMax;
                
                if (jumpTimer <= 44)
                {
                    jumpTimer = 0;
                    flip.loops = maxFlips;
                }
            }
            else if (!onGround)
            {
                if (velocity.Y != 0)
                {
                    groundTimer = 10;
                }

                airTimer -= delta;
                if (animState == State.fly)
                {
                    flyTimer -= delta;
                    if (flyTimer < 0 && fly.frame != 1 && fly.frame != 2)
                    {
                        animState = State.fall;
                        flyTimer = 0;
                        //GamePad.SetVibration(PlayerIndex.One, 0.25f, 0.25f);
                        vibrateResetTimer = 7;

                        //jumpReleaseTimer = 1f;
                        //jumpBuffered = false;

                        flyEndTimer = 2;
                    }
                }

                if (animState == State.run || animState == State.idle || animState == State.land) animState = State.fall;
            }

            jumpReleaseTimer -= delta;
            if (jumpReleaseTimer < 0) jumpReleaseTimer = 0;

            landBuffer -= delta;
            if (landBuffer < 0) landBuffer = 0;
        }

        private void ManageFixedTimers()
        {

        }

        private void SetBounds()
        {
            bounds = new Rectangle((int)position.X, (int)position.Y, 32, 32);

            //lft = new Rectangle((int)position.X - 1, (int)position.Y + 1, 1, 30);
            //rgt = new Rectangle((int)position.X + 33, (int)position.Y + 1, 1, 30);
            bot = new Rectangle((int)position.X + 3, (int)position.Y + 32, 32 - 3, 1);
            //top = new Rectangle((int)position.X + 1, (int)position.Y, 30, 2);
            hitBox = new Rectangle((int)(position.X) + 8, (int)(position.Y) + 4, 24, 20);
        }

        private void CalculateTilemapPosition()
        {
            tileMapX = (int)Math.Round((position.X) / 32, 0);
            tileMapY = (int)Math.Round((position.Y) / 32, 0);
            
            tileLft = new Vector2(tileMapX - 1, tileMapY);
            tileRgt = new Vector2(tileMapX + 1, tileMapY);
            tileTop = new Vector2(tileMapX, tileMapY - 1);
            tileBot = new Vector2(tileMapX, tileMapY + 1);

            
            if (tileMapX < 0) tileMapX = 0;
            if (tileMapX > tilemap.w) tileMapX = tilemap.w;
            if (tileMapY < 0) tileMapY = 0;
            if (tileMapY > tilemap.h) tileMapY = tilemap.h;
            
            if (position.Y > tilemap.h * 32) tileMapY = tilemap.h;

            if (tileLft.X < 0) tileLft.X = 0;
            if (tileRgt.X > game.tilemap.w) tileRgt.X = game.tilemap.w;
            if (tileTop.Y < 0) tileTop.Y = 0;
            if (tileBot.Y > game.tilemap.h) tileBot.Y = game.tilemap.h;

            //tileMapX = (int)Math.Round(position.X / 64, 0);
            //tileMapY = (int)Math.Round(position.Y / 64, 0);
        }

        private void Die()
        {
            GamePad.SetVibration(PlayerIndex.One, 0.85f, 0.72f);
            vibrateResetTimer = 30;

            animState = State.death;
            respawnTimer = 80;

            //game.ResetLevel();

            if (game.bg != null) game.bg.scollSpeed.X = 0;
            velocity.X = 0;
            acceleration.X = 0;

            damageTimer = 0;
        }

        private void Respawn()
        {
            animState = State.idle;
            position = checkpointPosition;

            game.camera.position = position;
            game.respawnEffectTimer = game.virtualHeight * 2;

            //respawnTimer = 0;
            game.ResetLevel();

            dir = "r";
            hp = 3;
        }

        private void EnemyCollision()
        {
            foreach (Enemy e in handler.enemies)
            {
                if (e.inFrame)
                {
                    if (animState == State.fly)
                    {
                        if (this.hitBox.Intersects(e.bounds))
                        {
                            if (position.Y <= e.position.Y + 20)
                            {
                                // stomp
                                FLY = false;
                                flyUsed = false;
                                animState = State.fall;


                                jumpReleaseTimer = 5;
                                flyTimer = flyTimerMax;

                                e.OnStomp();
                                invFrames = 24;

                                position.Y = e.position.Y - 32;
                                velocity.Y = -5.25f;

                                damageTimer = 0;
                            }
                            else 
                            {
                                // damage
                                if (damageTimer <= 0)
                                {
                                    if (invFrames <= 0) hp--;

                                    damageTimer = 12;
                                    invFrames = 24;

                                    if (e.position.X + 16 < position.X) hitDir = "l";
                                    else if (e.position.X + 16 > position.X) hitDir = "r";
                                    
                                    if (invFrames <= 0) e.hitPlayer = true;
                                }
                            }
                        }
                    }
                    else 
                    {
                        if (this.bounds.Intersects(e.bounds) /*&& this.bounds.Intersects(e.nextBounds)*/)
                        {
                        
                            if (position.Y + 32 < e.position.Y + 16 && velocity.Y > 0.2f)
                            {
                                position.Y = e.position.Y - 32;
                                velocity.Y = -5;
                                //e.isActive = false;
                                e.OnStomp();
                                invFrames = 24;

                                flyTimer = flyTimerMax;
                                damageTimer = 0;
                            }
                            else if (hitBox.Intersects(e.bounds))
                            {
                                if (damageTimer <= 0)
                                {
                                    if (invFrames <= 0) hp--;

                                    damageTimer = 12;
                                    

                                    //hitDir = dir;
                                    if (e.position.X + 16 < position.X) hitDir = "l";
                                    else if (e.position.X + 16 > position.X) hitDir = "r";
                                    if(invFrames <= 0) e.hitPlayer = true;

                                    invFrames = 24;
                                }
                            }
                        }   
                    }
                }
            }
        }
    }
}