using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Beri00
{
    public class Spritesheet
    {
        private readonly GraphicsDevice _graphicsDevice;
        private Texture2D texture;

        public Vector2 tileSize;

        public Texture2D blank;
        public Texture2D[,] frames = new Texture2D[64, 64];

        public Spritesheet(GraphicsDevice graphicsDevice, Texture2D texture, Vector2 tileSize)
        {
            _graphicsDevice = graphicsDevice;
            this.texture = texture;
            this.tileSize = tileSize;
        }

        public void LoadContent(ContentManager content)
        {
            int ssWidth = texture.Width;
            int ssHeight = texture.Height;

            int cols = (ssWidth / (int)tileSize.X) + 1;
            int rows = (ssHeight / (int)tileSize.Y) + 1;

            for (int r = 1; r < rows; r++)
            {
                for (int c = 1; c < cols; c++)
                {
                    frames[c - 1, r - 1] = GrabSprite(texture, c, r, (int)tileSize.X, (int)tileSize.Y);
                }
            }

            blank = GrabSprite(texture, 1, 1, 1, 1);
        }

        public Texture2D GrabSprite(Texture2D ss, int col, int row, int width, int height)
        {
            Rectangle sourceRectangle = new Rectangle((col * (int)tileSize.X) - (int)tileSize.X, (row * (int)tileSize.Y) - (int)tileSize.Y, width, height);


            Texture2D cropTexture = new Texture2D(_graphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            ss.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);
            return cropTexture;
        }
    }
}
