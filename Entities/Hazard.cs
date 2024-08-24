using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00.Entities
{
    public class Hazard
    {
        public bool isActive = true;
        public bool inFrame = true;

        Game1 game;
        Handler handler;
        Spritesheet ss;

        public Vector2 position;
        public int tx;
        public int ty;
        public Vector2 size;
        public Vector2 sprite_coords;
        public string motion = "static";
        private Vector2 range;

        public Rectangle bounds;

        public Hazard(Vector2 position, Vector2 size, Vector2 sprite_coords, string motion, Vector2 range, Spritesheet ss, Handler handler, Game1 game)
        {
            this.position = position;
            this.size = size;
            this.sprite_coords = sprite_coords;
            this.motion = motion;
            this.range = range;
            this.ss = ss;
            this.handler = handler;
            this.game = game;

            this.tx = (int)(position.X / 32);
            this.ty = (int)(position.Y + 16 / 32);
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void FixedUpdate()
        {
            if (motion != "static")
            {
                Move();
                SetBounds();
            }
            else if (motion == "static")
            {
                if (inFrame)
                {
                    SetBounds();
                }
            }

            int frameLft = game.tilemap.frameLft;
            int frameRgt = game.tilemap.frameRgt;

            if (!(tx > frameLft && tx < frameRgt))
            {
                isActive = false;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (motion != "static")
            {
                _spriteBatch.Draw(ss.frames[(int)sprite_coords.X,
                (int)sprite_coords.Y],
                position,
                Color.White);
            }
        }

        private void Move()
        {
            switch (motion)
            {
                case "horizontal":
                    break;

                case "vertical":
                    break;

                case "circle":
                    break;
            }
        }

        private void SetBounds()
        {
            /*
            if (sprite_coords.X == 0 && sprite_coords.Y == 0)
            {
                bounds = new Rectangle((int)position.X, (int)position.Y + 18, (int)size.X, (int)size.Y);
            }
            else
            {
                bounds = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            }
            */
        }
    }
}
