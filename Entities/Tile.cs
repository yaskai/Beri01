using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Beri00.Entities
{
    public class Tile
    {
        public bool isActive = true;

        Spritesheet spritesheet;
        Tilemap tilemap;

        public Vector2 position;
        public Vector2 tilePosition;
        public Vector2 spritesheet_coords;
        public bool slope = false;
        public bool center = false;

        public Tile(Vector2 position, Vector2 tilePosition, Vector2 spritesheet_coords, Spritesheet spritesheet, bool slope, Tilemap tilemap)
        {
            this.position = position;
            this.tilePosition = tilePosition;
            this.spritesheet_coords = spritesheet_coords;
            this.spritesheet = spritesheet;
            this.slope = slope;
            this.tilemap = tilemap;
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

            //DrawColliders(_spriteBatch);
        }

        public void DrawColliders(SpriteBatch _spriteBatch)
        {
            int c = (int)tilePosition.X;
            int r = (int)tilePosition.Y;

            if (slope)
            {
                switch (tilemap.mapData[c, r])
                {
                    case '2':
                    DrawHeightMap(_spriteBatch, position, tilemap.slope_heightmap_2);
                    break;

                    case '3':
                    DrawHeightMap(_spriteBatch, position, tilemap.slope_heightmap_3);
                    break;

                    case '4':
                    DrawHeightMap(_spriteBatch, position, tilemap.slope_heightmap_4);
                    break;

                    case '5':
                    DrawHeightMap(_spriteBatch, position, tilemap.slope_heightmap_5);
                    break;
                }
            }
        }

        public void DrawHeightMap(SpriteBatch _spriteBatch, Vector2 pos, int[] map_array)
        {
            //_spriteBatch.Draw(tilemap.game.blank_spritesheet.frames[1, 0], new Rectangle())

            for (int i = 0; i < map_array.Length; i++)
            {
                _spriteBatch.Draw(tilemap.game.blank_spritesheet.frames[13, 0], new Rectangle((int)pos.X + i, (int)pos.Y + map_array[i], 1, 2), Color.Violet);
                //_spriteBatch.Draw(blank_spritesheet.blank, new Rectangle(0, i * 4, virtualWidth, 2), Color.Black * 0.065f);
            }
        }
    }
}
