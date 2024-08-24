using Microsoft.Xna.Framework;
using System;

namespace Beri00
{
    public class Camera
    {
        Game1 game;
        Handler handler;
        Tilemap tilemap;

        public Matrix transform { get; private set; }
        public Matrix inverseTransform { get; private set; }
        public Matrix scale { get; private set; }

        public Vector2 position;
        private Vector2 prevPositon;
        public Vector2 velocity;

        public int x;
        public int y;

        private Vector2 target = new Vector2(0, 0);
        private Vector2 lastPos = new Vector2(0, 0);

        private float delta;

        public Camera(Handler handler, Game1 game, Tilemap tilemap)
        {
            this.handler = handler;
            this.game = game;
            this.tilemap = tilemap;
        }

        public void Move(GameTime gameTime)
        {
            prevPositon = position;

            foreach (Entities.Player p in handler.players)
            {
                //Vector2 playerPos = p.position;
                //playerPos.Round();
                Vector2 playerPos = p.drawPosition;
                //playerPos.X += p.position.X + 16;
                //playerPos.Y += p.position.Y + 16;

                if (!p.lockCamera)
                {
                    //position.X += ((p.position.X - position.X)) * (float)(game.delta * 0.1f);
                    //position.X += (((p.position.X + 16) - position.X) * delta) * 0.095f;
                    //position.Y += (((p.lastGroundPosition.Y + p.velocity.Y * (float)game.delta) - position.Y) * 0.05f) * (float)(game.delta);

                    //position.X += (((p.position.X + 16) - position.X)) * 0.095f;
                    //position.X = p.position.X + 16;
                    position.X = p.drawPosition.X + 16;

                    /*
                    position = new Vector2(
                    MathHelper.Lerp(position.X, p.position.X, 0.08f),
                    MathHelper.Lerp(position.Y, p.position.Y, 0.08f));
                    */

                    /*
                    if (playerPos.Y <= position.Y + game.virtualHeight / 3f && playerPos.Y >= position.Y - game.virtualHeight / 3.5f)
                    {
                        position.Y += ((p.lastGroundPosition.Y - position.Y) * 0.5f) * (float)(game.delta / 15f);
                    }
                    else
                    {
                        position.Y += (((playerPos.Y + p.velocity.Y * (float)game.delta) - position.Y) * 0.05f) * (float)(game.delta);
                    }
                    */

                    if (playerPos.Y <= p.lastGroundPosition.Y + game.virtualHeight / 4 && playerPos.Y >= p.lastGroundPosition.Y - game.virtualHeight / 2.25f)
                    {
                        //position.Y += ((p.lastGroundPosition.Y - position.Y) * 0.5f) * (delta / 15f);
                        position.Y += ((p.lastGroundPosition.Y - position.Y) * 0.5f) * (game.delta1 * 0.1f / 15f);
                    }
                    else
                    {
                        //position.Y += (((playerPos.Y + p.velocity.Y * (float)game.delta) - position.Y) * 0.05f) * (delta);
                        position.Y += (((playerPos.Y + p.velocity.Y * (game.delta1 * 0.1f) - position.Y) * 0.05f) * (game.delta1 * 0.1f));
                    }

                    //position.X = p.position.X;
                    //position.Y = p.position.Y;

                    // lock camera in level bounds for 5x5 virtual scale
                    /*
                    if (position.X < 15 * 32) position.X = 15 * 32;
                    if (position.X > (game.tilemap.w - 16) * 32) position.X = (game.tilemap.w - 16) * 32;
                    if (position.Y < 6 * 32) position.Y = 6 * 32;
                    if (position.Y > (game.tilemap.h - 9) * 32) position.Y = (game.tilemap.h - 9) * 32;
                    */
                    /*
                    // lock camera for 4x4 virtual scale
                    if (position.X < 12 * 32) position.X = 12 * 32;
                    if (position.X > (game.tilemap.w - 12) * 32) position.X = (game.tilemap.w - 12) * 32;
                    if (position.Y < 7 * 32) position.Y = 7 * 32;
                    if (position.Y > (game.tilemap.h - 7) * 32) position.Y = (game.tilemap.h - 7) * 32;
                    */

                    if (position.X < 10 * 32) position.X = 10 * 32;
                    if (position.X > (game.tilemap.w - 10) * 32 - 2) position.X = (game.tilemap.w - 10) * 32 - 2;
                    if (position.Y < 0) position.Y = 0;
                    if (position.Y > (game.tilemap.h - 6) * 32) position.Y = (game.tilemap.h - 6) * 32;
                    //Math.Truncate(position.X);
                    //Math.Truncate(position.Y);

                    float difX = 0;
                    float difY = 0;

                    difX = position.X - lastPos.X;
                    difY = position.Y - lastPos.Y;

                    //game.bg.scollSpeed.X = -difX * 2;

                    if (game.bg != null)
                    {
                        if (difX != 0)
                        {
                            if (p.velocity.X > 1f)
                            {
                                //scroll bg left
                                game.bg.scollSpeed.X = -p.velocity.X * 0.05f;
                            }
                            else if (p.velocity.X < -0.7f)
                            {
                                //scroll bg right
                                game.bg.scollSpeed.X = -p.velocity.X * 0.05f;
                            }
                            else
                            {
                                game.bg.scollSpeed.X = 0;
                            }
                        }
                        else game.bg.scollSpeed.X = 0;
                    }


                    //if (position.X < lastPos.X) game.bg.ScrollX(2.5f); else if (position.X > lastPos.X) game.bg.ScrollX(-2.5f);
                    //if (position.Y < lastPos.Y) game.bg.ScrollY(-1); else if (position.Y > lastPos.Y) game.bg.ScrollY(1);

                    lastPos = position;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            //delta = game.ALPHA;
            delta = game.delta;

            var scaleX = Math.Abs((float)game.actualWidth / game.virtualWidth);
            var scaleY = Math.Abs((float)game.actualHeight / game.virtualHeight);
            var scale = Matrix.CreateScale(scaleX, scaleY, 0);
            //var scale = Matrix.CreateScale(2, 2, 0f);
            var inverseScale = Matrix.Invert(scale);

            /*
            var offset = Matrix.CreateTranslation(0, game.virtualHeight, 0);
            var pos = transform = Matrix.CreateTranslation(
            -position.X,
            -position.Y,
            0);
            
            transform = pos * offset * scale;
            inverseTransform = Matrix.Invert(transform);
            this.scale = scale;
            */

            x = (int)position.X / 32;
            y = (int)position.Y / 32;

            Vector2 lerpPos = Vector2.LerpPrecise(prevPositon, position, game.ALPHA);
            //Vector2 lerpPos = Vector2.Lerp(prevPositon, position, game.ALPHA);

            var offset = Matrix.CreateTranslation(game.virtualWidth / 2f, game.virtualHeight / 2f, 0);
            var pos = transform = Matrix.CreateTranslation(
            //-position.X - 16,
            -lerpPos.X,
            -lerpPos.Y,
            0);

            transform = pos * offset * scale;
            inverseTransform = Matrix.Invert(transform);
            this.scale = scale;
        }
    }
}
