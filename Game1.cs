using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Threading;

namespace Beri00
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public int virtualWidth = 320 * 2;
        public int virtualHeight = 180 * 2;
        public int actualWidth = 320 * 4;
        public int actualHeight = 180 * 4;


        public Vector2 scale = new Vector2(1, 1);

        public float delta = 0;
        //public float delta1 = (int)(1000 / (float)60);
        public float delta1 = (int)(1000 / (float)60);
        public double fps = 0;
        private double displayFps = 0;
        private float fpsTimer = 0;

        private float previousT = 0;
        private float accumulator = 0.0f;
        private float maxFrameTime = 128;
        //private float maxFrameTime = 96;
        public float ALPHA = 0;

        public int targetFps = 60;
        private float previousDraw = 0;
        private float drawAccumulator = 0.0f;

 
        Handler handler;
        public Camera camera;

        public Tilemap tilemap;
        public Background bg;
        public enum State
        {
            Title,
            Main,
            GameOver
        };

        public State _state = State.Title;

        public bool debugView = false;
        private bool scanLines = true;

        public SpriteFont debugFont;

        public Texture2D player_spritesheetPNG;
        public Spritesheet player_spritesheet;

        public Texture2D tile_spritesheetPNG;
        public Spritesheet tile_spritesheet;

        public Texture2D hazard_spritesheetPNG;
        public Spritesheet hazard_spritesheet;

        public Texture2D can_enemy_spritesheetPNG;
        public Spritesheet can_enemy_spritesheet;

        public Texture2D blankPNG;
        public Spritesheet blank_spritesheet;

        public Texture2D tree00;

        public Texture2D test_bg_layer0;
        public Texture2D test_bg_layer1;
        public Texture2D test_bg_layer2;

        private Texture2D bg_tileset;
        public Spritesheet bg_tileset_ss;

        public Texture2D fblockPNG;
        public Spritesheet fblock_ss;

        public Texture2D doorImg;

        public double respawnEffectTimer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //base.Initialize();

            actualWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            actualHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            scale = new Vector2(actualWidth / virtualWidth, actualHeight / virtualHeight);
            //scale = new Vector2(1, 1);

            this.IsFixedTimeStep = false;
            //TargetElapsedTime = TimeSpan.FromTicks((long)1000.0f / 60);
            this._graphics.SynchronizeWithVerticalRetrace = false;

            _graphics.IsFullScreen = false;
            Window.IsBorderless = true;

            _graphics.PreferredBackBufferWidth = actualWidth;
            _graphics.PreferredBackBufferHeight = actualHeight;

            _graphics.ApplyChanges();
            base.Initialize();

            handler = new Handler();
            tilemap = new Tilemap(this, handler, tile_spritesheet);
            camera = new Camera(handler, this, tilemap);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            /*
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            */
            
            /*
            renderTarget = new RenderTarget2D(
                GraphicsDevice,
                actualWidth,
                actualHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            
            */
            debugFont = Content.Load<SpriteFont>("debugFont");

            blankPNG = Content.Load<Texture2D>("blank_pixels");
            blank_spritesheet = new Spritesheet(GraphicsDevice, blankPNG, new Vector2(1, 1));
            blank_spritesheet.LoadContent(Content);

            player_spritesheetPNG = Content.Load<Texture2D>("beri_spritesheet_32x32");
            player_spritesheet = new Spritesheet(GraphicsDevice, player_spritesheetPNG, new Vector2(32, 32));
            player_spritesheet.LoadContent(Content);

            tree00 = Content.Load<Texture2D>("beri_palm08");

            tile_spritesheetPNG = Content.Load<Texture2D>("tileset00_32x32");
            tile_spritesheet = new Spritesheet(GraphicsDevice, tile_spritesheetPNG, new Vector2(32, 32));
            tile_spritesheet.LoadContent(Content);

            hazard_spritesheetPNG = Content.Load<Texture2D>("beri_hazard_spritesheet32x32");
            hazard_spritesheet = new Spritesheet(GraphicsDevice, hazard_spritesheetPNG, new Vector2(32, 32));
            hazard_spritesheet.LoadContent(Content);

            can_enemy_spritesheetPNG = Content.Load<Texture2D>("can_enemy_spritesheet32x32");
            can_enemy_spritesheet = new Spritesheet(GraphicsDevice, can_enemy_spritesheetPNG, new Vector2(32, 32));
            can_enemy_spritesheet.LoadContent(Content);

            //test_bg_layer0 = Content.Load<Texture2D>("bg_test_desert00");
            test_bg_layer0 = Content.Load<Texture2D>("beri_ocean01");
            test_bg_layer1 = Content.Load<Texture2D>("bg_test_desert00");

            bg = new Background(test_bg_layer0, null, null, this);

            bg_tileset = Content.Load<Texture2D>("bg_tileset00");
            bg_tileset_ss = new Spritesheet(GraphicsDevice, bg_tileset, new Vector2(32, 32));
            bg_tileset_ss.LoadContent(Content);

            fblockPNG = Content.Load<Texture2D>("fblock_sheet_32x32");
            fblock_ss = new Spritesheet(GraphicsDevice, fblockPNG, new Vector2(32, 32));
            fblock_ss.LoadContent(Content);

            doorImg = Content.Load<Texture2D>("door0032x32");

            //flip_preview = new Animation(5, 10, new Vector2(1, 4), new Vector2(32, 32), player_spritesheet, player_spritesheetPNG, this);
            //flip_preview = new Animation(8, 10, new Vector2(1, 4), new Vector2(32, 32), player_spritesheet, player_spritesheetPNG, this);
        }

        protected override void Update(GameTime gameTime)
        {
            delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.1f;
            //delta = (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
            
            if (previousT == 0)
            {
                previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }

            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float frameTime = now - previousT;

            if (frameTime > maxFrameTime)
            {
                frameTime = maxFrameTime;
            }
         
            previousT = now;
 
            accumulator += frameTime;

            while (accumulator >= delta1)
            {
                FixedUpdate(gameTime);
                accumulator -= delta1;
            }
 
            // this value stores how far we are in the current frame. For example, when the 
            // value of ALPHA is 0.5, it means we are halfway between the last frame and the 
            // next upcoming frame.
            ALPHA = (accumulator / delta1);

            respawnEffectTimer -= delta * 25;
            if (respawnEffectTimer < 0) respawnEffectTimer = 0;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kb = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (kb.IsKeyDown(Keys.V))
            {
                debugView = true;
                if (kb.IsKeyDown(Keys.LeftControl)) debugView = false;
            }

            if (padState.IsButtonDown(Buttons.Start)) debugView = false;

            // TODO: Add your update logic here
            switch (_state)
            {
                case State.Title:
                    if (kb.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.Start) || padState.IsButtonDown(Buttons.A)) StartGame();
                    break;
                case State.Main:
                    //camera.Update(gameTime);
                    handler.Update(gameTime);
                    //tilemap.Update(gameTime);

                    //camera.Move(gameTime);
                    camera.Update(gameTime);

                    if (bg != null) bg.Update(gameTime);

                    //if (kb.IsKeyDown(Keys.LeftControl) && kb.IsKeyDown(Keys.P)) tilemap.WritePlatformListToDebug();
                    //flip_preview.Update(gameTime);
                    break;
                case State.GameOver:
                    break;
            }
            
            //delta = gameTime.ElapsedGameTime.TotalMilliseconds / 10;

            //camera.Update(gameTime);

            base.Update(gameTime);
        }

        private void FixedUpdate(GameTime gameTime)
        {
            //System.Diagnostics.Debug.WriteLine("FixedUpdate, delta1 " + delta1);
            camera.Move(gameTime);
            handler.FixedUpdate();
            tilemap.Update(gameTime);

            fpsTimer --;
            if (fpsTimer <= 0)
            {
                displayFps = fps;
                fpsTimer = 5;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            FixedDraw(gameTime);
            //Thread.Sleep(1);
            //System.Threading.Thread.Sleep(Math.Abs((int)(1000 / (targetFps) - gameTime.ElapsedGameTime.TotalMilliseconds)));
        }

        private void FixedDraw(GameTime gameTime)
        {
            fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;
            //fps += (((1 / gameTime.ElapsedGameTime.TotalSeconds) - fps) * 0.1);
            //System.Diagnostics.Debug.WriteLine("fps : " + fps);

            //GraphicsDevice.SetRenderTarget(renderTarget);
            //if (debugView) GraphicsDevice.Clear(Color.Black); else if (!debugView) GraphicsDevice.Clear(Color.DeepSkyBlue);
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here 

            // draw with scale only (no position transformations)
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.PointClamp, null, null, null, camera.scale);
            switch (_state)
            {
                case State.Title:
                    break;
                case State.Main:
                    if (bg != null) bg.Draw(_spriteBatch);
                    break;
                case State.GameOver:
                    break;
            }
            _spriteBatch.End();
            
            if (_state == State.Main)
            {
                _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
                tilemap.DrawBgTiles(_spriteBatch);
                _spriteBatch.End();

                _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
                tilemap.Draw(_spriteBatch);
                _spriteBatch.End();
            }
            
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);

            switch (_state)
            {
                case State.Title:
                    break;
                case State.Main:
                    //tilemap.Draw(_spriteBatch);
                    handler.Draw(_spriteBatch);
                    break;
                case State.GameOver:
                    break;
            }

            _spriteBatch.End();

            if (respawnEffectTimer > 0) DrawTransition(_spriteBatch);

            if (scanLines) DrawCRT(_spriteBatch);

            // draw without camera transformations
            /*
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null, null, null);
            _spriteBatch.DrawString(debugFont, "FPS: " + fps, new Vector2(32, 32), Color.White, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 1);
            _spriteBatch.End();
            */
            
            if (debugView)
            {
                if (_state == State.Main)
                {
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
                    tilemap.DrawDebugView(_spriteBatch);
                    _spriteBatch.DrawString(debugFont, "FPS: " + displayFps, new Vector2(0, 160), Color.LimeGreen);
                    //flip_preview.Draw(_spriteBatch, new Vector2(actualWidth - 128, 32), new Vector2(4, 4), SpriteEffects.None, Color.White);
                    _spriteBatch.End();
                }
            }
            else
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
                _spriteBatch.Draw(blank_spritesheet.frames[0, 0], new Rectangle(24, 32, 140, 32), Color.White * 0.5f);
                _spriteBatch.DrawString(debugFont, "FPS:" + Math.Truncate(displayFps), new Vector2(32, 32), Color.White, 0, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 1);
                //_spriteBatch.DrawString(debugFont, "delta: " + delta * 100, new Vector2(32, 64), Color.White, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 1);
                _spriteBatch.End();
            }
            
            /*
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null,  null, null);
            _spriteBatch.Draw(renderTarget,
                        Vector2.Zero,
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        0f);
            _spriteBatch.End();
            */
            base.Draw(gameTime);
        }

        private void StartGame()
        {
            _state = State.Main;
            tilemap.LoadMap();
            //Entities.Player p = new Entities.Player(new Vector2(16, 72), this, handler, player_spritesheet);
            //handler.players.Add(p);
            if (bg != null) bg.Reset();
            System.Diagnostics.Debug.WriteLine("game started");
        }

        public void ResetLevel()
        {
            handler.ResetScene();
            tilemap.ResetMap();
            if (bg != null) bg.Reset();
            System.Diagnostics.Debug.WriteLine("tilemap reset");
        }

        public void NextLevel()
        {
            
        }

        private void DrawCRT(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.scale);
            for (int i = 0; i < virtualHeight / 3; i++)
            {
                _spriteBatch.Draw(blank_spritesheet.blank, new Rectangle(0, i * 3, virtualWidth, 2), Color.Black * 0.1f);
            }
            _spriteBatch.End();
        }

        public void DrawTransition(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.scale);
            /*
            int rectWidth;
            rectWidth = (int)(respawnEffectTimer);
            _spriteBatch.Draw(player_spritesheet.blank, new Rectangle(0, 0, rectWidth, actualHeight), Color.Black);
            _spriteBatch.Draw(player_spritesheet.blank, new Rectangle(actualWidth - rectWidth, 0, rectWidth, actualHeight), Color.Black);
            */

            int rectHeight;
            rectHeight = (int)(respawnEffectTimer / scale.Y);
            //_spriteBatch.Draw(player_spritesheet.blank, new Rectangle(0, 0, actualWidth, rectHeight), Color.Black);
            //_spriteBatch.Draw(player_spritesheet.blank, new Rectangle(0, actualHeight - rectHeight, actualWidth, rectHeight), Color.Black);
            _spriteBatch.Draw(blank_spritesheet.blank, new Rectangle(0, 0, virtualWidth, rectHeight), Color.Black);
            _spriteBatch.Draw(blank_spritesheet.blank, new Rectangle(0, virtualHeight - rectHeight, virtualWidth, rectHeight), Color.Black);
            _spriteBatch.End();
        }
    }
}
