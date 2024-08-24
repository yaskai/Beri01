using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Beri00.Entities
{
    public class Tile
    {
        public bool isActive = true;

        Spritesheet spritesheet;

        public Vector2 position;
        public Vector2 tilePosition;
        public Vector2 spritesheet_coords;
        public bool slope = false;
        public bool center = false;

        public Tile(Vector2 position, Vector2 tilePosition, Vector2 spritesheet_coords, Spritesheet spritesheet, bool slope)
        {
            this.position = position;
            this.tilePosition = tilePosition;
            this.spritesheet_coords = spritesheet_coords;
            this.spritesheet = spritesheet;
            this.slope = slope;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(spritesheet.frames[(int)spritesheet_coords.X, (int)spritesheet_coords.Y],
                        position,
                        null, 
                        Color.White, 
                        0, 
                        new Vector2(1, 1), 
                        new Vector2(1, 1), 
                        SpriteEffects.None, 
                        2);
        }
    }
}
