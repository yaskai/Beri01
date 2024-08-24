using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00
{
    public class Animation
    {
        Game1 game;
        Spritesheet ss;

        int totalFrames;
        public float speed;
        Vector2 tileSize;
        Vector2 startFrame;
        Texture2D spritesheet;

        int ssWidth;
        int ssHeight;
        int cols;
        int rows;

        int c;
        int r;

        float frameTimer = 0;
        public int frame = 0;

        int startC;
        int startR;

        public int f { get; set; }
        public int loops = 0;
        public int spriteLayer = 2;

        public Animation(int frames, int speed, Vector2 startFrame, Vector2 tileSize, Spritesheet ss, Texture2D spritesheet, Game1 game)
        {
            this.totalFrames = frames;
            this.speed = speed;
            this.ss = ss;
            this.spritesheet = spritesheet;
            this.startFrame = startFrame;
            this.game = game;

            ssWidth = spritesheet.Width;
            ssHeight = spritesheet.Height;

            cols = ssWidth / (int)tileSize.X;
            rows = ssHeight / (int)tileSize.Y;

            startC = (int)startFrame.X;
            startR = (int)startFrame.Y;

            c = startC;
            r = startR;
        }

        public void Update(GameTime gameTime)
        {
            //var delta = (float)(gameTime.ElapsedGameTime.TotalSeconds * 100);
            var delta = game.delta;
            //var delta = game.ALPHA;

            f = frame;
            if (f == 0)
            {
                c = startC;
                r = startR;
            }

            if (frameTimer >= speed)
            {
                c++;

                if (c > cols - 1)
                {
                    c = 0;
                    r++;
                }

                if (r > rows - 1)
                {
                    c = startC;
                    r = startR;
                }

                frame++;

                if (frame > totalFrames - 1)
                {
                    c = startC;
                    r = startR;
                    frame = 0;
                    loops++;
                }

                frameTimer = 0;
            }

            frameTimer += 1 * delta;
        }

        public void Draw(SpriteBatch _spriteBatch, Vector2 position, Vector2 scale, SpriteEffects spriteEffects, Color color)
        {
            _spriteBatch.Draw(ss.frames[c, r], position, null, color, 0, Vector2.Zero, scale, spriteEffects, spriteLayer);
        }

        public void SetFrame(int n)
        {
            frame = n;
        }
    }
}