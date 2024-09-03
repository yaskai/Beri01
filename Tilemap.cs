using Beri00.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Beri00
{
    public class Tilemap
    {
        public Game1 game;
        private Handler handler;
        private Spritesheet ss;

        public static int mapWidth = 512;
        public static int mapHeight = 512;

        public int w = mapWidth;
        public int h = mapHeight;

        public int screenWidth = 24;
        public int screenHeight = 14;

        public int frameWidth = 24;
        public int frameHeight = 14;

        private String[] maps = new string[128];
        private Vector2[,] graphicsMap = new Vector2[mapWidth, mapHeight];
        private String[] renderMap;
        public char[,] mapData = new char[mapWidth, mapHeight];
        public Entities.Tile[,] tileList = new Entities.Tile[mapWidth, mapHeight];
        public bool[,] hasCollider = new bool[mapWidth, mapHeight];

        public Vector2 scale = new Vector2(1, 1);

        public int frameLft = 0, frameRgt = 0, frameTop = 0, frameBot = 0;

        public string currentMap = "";

        private int[,] bgMapData = new int[mapWidth, mapHeight];
        private Vector2[,] bgGraphicsMap = new Vector2[mapWidth, mapHeight];
        
        public Entities.Platform[,] platforms = new Entities.Platform[mapWidth, mapHeight];

        public enum adjacency
        {
            TopLeft,
            TopCenter,
            TopRight,
            CenterLeft,
            Center,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight,
            FloorLeft,
            FloorCenter,
            FloorRight,
            WallTop,
            WallCenter,
            WallBottom,
            Island
        };
        private adjacency[,] adjacencies = new adjacency[mapWidth, mapHeight];
        /*
        public int[] slope_heightmap_2 = new int[32];
        public int[] slope_heightmap_3 = new int[32];
        public int[] slope_heightmap_4 = new int[32];
        public int[] slope_heightmap_5 = new int[32]; 
        */
        
        public int[] slope_heightmap_2 = new int[64];
        public int[] slope_heightmap_3 = new int[64];
        public int[] slope_heightmap_4 = new int[64];
        public int[] slope_heightmap_5 = new int[64]; 
        
        public Tilemap(Game1 game, Handler handler, Spritesheet ss)
        {
            this.game = game;
            this.handler = handler;
            this.ss = ss;

            screenWidth = (game.virtualWidth / 32) + 2;
            screenHeight = ((game.actualHeight / 32) / 2) + 2;

            //frameWidth = screenWidth + 8;
            //frameHeight = screenHeight + 12;

            frameWidth = screenWidth * 4;
            frameHeight = screenHeight * 4;

            renderMap = new String[screenWidth * screenHeight];

            Init();
        }

        private void Init()
        {
            /*
            slope_heightmap_2 = InitSlopeArray(4, 27, 32, -1);
            slope_heightmap_3 = InitSlopeArray(4, 13, 32, -1);
            slope_heightmap_4 = InitSlopeArray(4, 32, 32,  1);
            slope_heightmap_5 = InitSlopeArray(4, 16, 32,  1);
            */
            
            slope_heightmap_2 = InitSlopeArray(4, 32, 64, -1);
            slope_heightmap_3 = InitSlopeArray(4,  0, 64, -1);
            slope_heightmap_4 = InitSlopeArray(4,  0, 64,  1);
            slope_heightmap_5 = InitSlopeArray(4, 30, 64,  1);
            
            /*
            slope_heightmap_2 = InitSlopeArray(4, 28, 32, -1);
            slope_heightmap_3 = InitSlopeArray(4, 18, 32, -1);
            slope_heightmap_4 = InitSlopeArray(4, 32, 32,  1);
            slope_heightmap_5 = InitSlopeArray(4, 32, 32,  1);
            */
            Debug.WriteLine("*********************************************************");
            for (int x = 0; x < 32; x ++) Debug.Write("[" + slope_heightmap_2[x] + "]");
        }

        public void LoadMap()
        {
            handler.ClearScene();
            //GenerateMap(maps[0]);
            //System.Diagnostics.Debug.WriteLine("tilemap loaded");

            StreamReader reader = new StreamReader("maps/testLevel00.txt");

            string lvlWidth = reader.ReadLine();
            string lvlHeight = reader.ReadLine();

            mapWidth = Int32.Parse(lvlWidth);
            mapHeight = Int32.Parse(lvlHeight);

            string newMap = "";

            for (int r = 0; r < mapHeight; r++)
            {
                string line = reader.ReadLine();
                newMap += line;
            }

            string seperator = reader.ReadLine();

            string bgBuffer = "";
            for (int r = 0; r < mapHeight; r++)
            {
                string line = reader.ReadLine();
                bgBuffer += line;
            }

            GenerateMap(newMap);
            GenerateBg(bgBuffer);
            currentMap = newMap;

            w = mapWidth;
            h = mapHeight;
        }

        public void ResetMap()
        {
            //ResetGenerate(maps[0]);
            ResetGenerate(currentMap);
        }

        public void GenerateMap(String map)
        {
            char[] chars = map.ToCharArray();
            for (int i = 0; i < map.Length; i++)
            {
                String s = chars[i] + "";

                int c = i % mapWidth;
                int r = i / mapWidth;
                Vector2 pos = new Vector2(c * (32 * scale.X), r * (32 * scale.Y) - 1);

                switch (s)
                {
                    case "1":
                        NewTile(map, i);
                        hasCollider[c, r] = true;
                        break;
                    case "2":
                        NewSlope(map, i, 2);
                        break;

                    case "3":
                        NewSlope(map, i, 3);
                        break;

                    case "4":
                        NewSlope(map, i, 4);
                        break;
                    case "5":
                        NewSlope(map, i, 5);
                        break; 
                    case "@":
                        if (handler.players.Count == 0) NewObject(pos, "player");
                        break;
                    case "^":
                        //NewObject(pos, "spike_floor");
                        //bjectMap[c, r] = "spike_floor";
                        break;
                    case "v":
                        //NewObject(pos, "spike_ceiling");
                        //objectMap[c, r] = "spike_ceiling";
                        break;
                    case "e":
                        NewObject(pos, "can_enemy00");
                        //objectMap[c, r] = "can_enemy00";
                        break;
                    case "f":
                        NewPlatform(c, r, 1);
                        break;
                }

                if (c < mapWidth && r < mapHeight) mapData[c, r] = chars[i];
            }

            UpdateAllTiles();
        }

        private void UpdateTile(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char lt = '.';
            char rt = '.';
            char lb = '.';
            char rb = '.';

            if (c > 0) lft = mapData[c - 1, r];
            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            if (r > 0) top = mapData[c, r - 1];
            if (r < mapHeight - 1) bot = mapData[c, r + 1];

            if (c > 0 && r > 0) lt = mapData[c - 1, r - 1];
            if (c < mapWidth - 1 && r > 0) rt = mapData[c + 1, r - 1];
            if (c > 0 && r < mapHeight - 1) lb = mapData[c - 1, r + 1];
            if (c < mapWidth - 1 && r < mapHeight - 1) rb = mapData[c + 1, r + 1];

            // check for slope tiles
            if (lft == '3') lft = '1';
            if (rgt == '4') rgt = '1';

            switch (top)
            {
                case '2':
                top = '1';
                break;

                case '3':
                top = '1';
                break;

                case '4':
                top = '1';
                break;

                case '5':
                top = '1';
                break;
            }

            switch (lt)
            {
                case '2':
                lt = '1';
                break;

                case '3':
                lt = '1';
                break;
            }

            switch (rt)
            {
                case '2':
                rt = '1';
                break;

                case '3':
                rt = '1';
                break;
            }

            if (lft != '1' && rgt == '1' && top != '1' && bot == '1')
            {
                // top left corner
                graphicsMap[c, r] = new Vector2(0, 0);
                adjacencies[c, r] = adjacency.TopLeft;
            }
            else if (lft == '1' && rgt == '1' && top != '1' && bot == '1')
            {
                // middle floor
                graphicsMap[c, r] = new Vector2(1, 0);
                adjacencies[c, r] = adjacency.TopCenter;
            }
            else if (lft == '1' && rgt != '1' && top != '1' && bot == '1')
            {
                // top right corner
                graphicsMap[c, r] = new Vector2(2, 0);
                adjacencies[c, r] = adjacency.TopRight;
            }
            else if (lft != '1' && rgt == '1' && top == '1' && bot == '1')
            {
                // left wall
                graphicsMap[c, r] = new Vector2(0, 1);
                adjacencies[c, r] = adjacency.CenterLeft;
            }
            else if (lft == '1' && rgt == '1' && top == '1' && bot == '1')
            {
                // center
                graphicsMap[c, r] = new Vector2(1, 1);
                adjacencies[c, r] = adjacency.Center;
            }
            else if (lft == '1' && rgt != '1' && top == '1' && bot == '1')
            {
                // right wall
                graphicsMap[c, r] = new Vector2(2, 1);
                adjacencies[c, r] = adjacency.CenterRight;
            }
            else if (lft != '1' && rgt == '1' && top ==  '1' && bot != '1')
            {
                // bottom left corner
                graphicsMap[c, r] = new Vector2(0, 2);
                adjacencies[c, r] = adjacency.BottomLeft;
            } 
            else if (lft == '1' && rgt == '1' && top == '1' && bot != '1')
            {
                // middle ceiling
                graphicsMap[c, r] = new Vector2(1, 2);
                adjacencies[c, r] = adjacency.BottomCenter;
            }
            else if (lft == '1' && rgt != '1' && top == '1' && bot != '1')
            {
                // bottom right corner
                graphicsMap[c, r] = new Vector2(2, 2);
                adjacencies[c, r] = adjacency.BottomLeft;
            }
            else if (lft != '1' && rgt != '1' && top != '1' && bot == '1')
            {
                // '
                graphicsMap[c, r] = new Vector2(3, 0);
                adjacencies[c, r] = adjacency.WallTop;
            }
            else if (lft != '1' && rgt != '1' && top == '1' && bot == '1')
            {
                // -
                graphicsMap[c, r] = new Vector2(3, 1);
                adjacencies[c, r] = adjacency.WallCenter;
            }
            else if (lft != '1' && rgt != '1' && top == '1' && bot != '1')
            {
                // .
                graphicsMap[c, r] = new Vector2(3, 2);
                adjacencies[c, r] = adjacency.WallBottom;
            }
            else if (lft != '1' && rgt == '1' && top != '1' && bot != '1')
            {
                // *--
                graphicsMap[c, r] = new Vector2(0, 3);
                adjacencies[c, r] = adjacency.FloorLeft;
            }
            else if (lft == '1' && rgt == '1' && top != '1' && bot != '1')
            {
                // -*-
                graphicsMap[c, r] = new Vector2(1, 3);
                adjacencies[c, r] = adjacency.FloorCenter;
            }
            else if (lft == '1' && rgt != '1' && top != '1' && bot != '1')
            {
                // --*
                graphicsMap[c, r] = new Vector2(2, 3);
                adjacencies[c, r] = adjacency.FloorRight;
            }
            else 
            {
                graphicsMap[c, r] = new Vector2(3, 3);
                adjacencies[c, r] = adjacency.Island;
            }
        }

        private void SecondaryTileUpdate(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char lt = '.';
            char rt = '.';
            char lb = '.';
            char rb = '.';

            if (c > 0) lft = mapData[c - 1, r];
            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            if (r > 0) top = mapData[c, r - 1];
            if (r < mapHeight - 1) bot = mapData[c, r + 1];

            if (c > 0 && r > 0) lt = mapData[c - 1, r - 1];
            if (c < mapWidth - 1 && r > 0) rt = mapData[c + 1, r - 1];
            if (c > 0 && r < mapHeight - 1) lb = mapData[c - 1, r + 1];
            if (c < mapWidth - 1 && r < mapHeight - 1) rb = mapData[c + 1, r + 1];

            if (r == 1) top = '1';
            if (r == mapHeight - 1) bot = '1';

                        // check for slope tiles
            if (lft == '3') lft = '1';
            if (rgt == '4') rgt = '5';

            switch (top)
            {
                case '2':
                top = '1';
                break;

                case '3':
                top = '1';
                break;

                case '4':
                top = '1';
                break;

                case '5':
                top = '1';
                break;
            }

            switch (lt)
            {
                case '2':
                lt = '1';
                break;

                case '3':
                lt = '1';
                break;

                case '4':
                lt = '1';
                break;

                case '5':
                lt = '1';
                break;
            }

            switch (rt)
            {
                case '2':
                rt = '1';
                break;

                case '3':
                rt = '1';
                break;

                case '4':
                rt = '1';
                break;

                case '5':
                rt = '1';
                break;
            }

            Vector2 coords = graphicsMap[c, r];
            
            if (adjacencies[c, r] == adjacency.Center)
            {
                if (lt == '1' && rt == '1' && lb == '1' && rb == '1')
                {
                    coords = new Vector2(1, 1);
                }
                else if (lt != '1' && rt == '1' && lb == '1' && rb == '1')
                {
                    coords = new Vector2(4, 0);
                }
                else if (lt == '1' && rt != '1' && lb == '1' && rb == '1')
                {
                    coords = new Vector2(4, 1);
                }
                else if (lt == '1' && rt == '1' && lb != '1' && rb == '1')
                {
                    coords = new Vector2(4, 2);
                }
                else if (lt == '1' && rt == '1' && lb == '1' && rb != '1')
                {
                    coords = new Vector2(4, 3);
                }
                else if (lt != '1' && rt != '1') coords = new Vector2(3, 4);

                tileList[c, r].center = true;
            }
            else if (adjacencies[c, r] == adjacency.CenterRight)
            {
                if (lft == '1')
                {
                    if (lt != '1' && lb != '1')
                    {
                        coords = new Vector2(0, 4);
                    }
                    else if (lt != '1' && lb == '1')
                    {
                        coords = new Vector2(1, 4);
                    }
                    else if (lt == '1' && lb != '1')
                    {
                        coords = new Vector2(2, 4);
                    }
                }
            }
            else if (adjacencies[c, r] == adjacency.CenterLeft)
            {
                if (rgt == '1')
                {
                    if (rt != '1' && rb != '1')
                    {
                        coords = new Vector2(0, 5);
                    }
                    else if (rt != '1' && rb == '1')
                    {
                        coords = new Vector2(1, 5);
                    }
                    else if (rt == '1' && rb != '1')
                    {
                        coords = new Vector2(2, 5);
                    }
                }
            }
            else if (adjacencies[c, r] == adjacency.BottomCenter)
            {
                if (lt != '1' && rt == '1')
                {
                    coords = new Vector2(3, 5);
                }
                else if (lt == '1' && rt != '1')
                {
                    coords = new Vector2(4, 5);
                }
            }

            graphicsMap[c, r] = coords;
        }

        private void ExtraAdjustments(int c, int r)
        {
            char lft = '.';
            char rgt = '.';
            char top = '.';
            char bot = '.';

            char lt = '.';
            char rt = '.';
            char lb = '.';
            char rb = '.';

            if (c > 0) lft = mapData[c - 1, r];
            if (c < mapWidth - 1) rgt = mapData[c + 1, r];
            if (r > 0) top = mapData[c, r - 1];
            if (r < mapHeight - 1) bot = mapData[c, r + 1];

            if (c > 0 && r > 0) lt = mapData[c - 1, r - 1];
            if (c < mapWidth - 1 && r > 0) rt = mapData[c + 1, r - 1];
            if (c > 0 && r < mapHeight - 1) lb = mapData[c - 1, r + 1];
            if (c < mapWidth - 1 && r < mapHeight - 1) rb = mapData[c + 1, r + 1];

            if (r == 1) top = '1';
            if (r == mapHeight - 1) bot = '1';

            if (mapData[c, r] == '1')
            {
                switch (adjacencies[c, r])
                {
                    case adjacency.Center:
                    if (top == '2' && lft == '3')
                    {
                        graphicsMap[c, r] = new Vector2(0, 7);
                    }

                    if (top == '5' && rgt == '4')
                    {
                        graphicsMap[c, r] = new Vector2(1, 7);
                    }

                    if (top == '2' && lft == '1')
                    {
                        graphicsMap[c, r] = new Vector2(0, 7);
                    }

                    if (top == '5' && rgt == '1')
                    {
                        graphicsMap[c, r] = new Vector2(1, 7);
                    }

                    break;

                    case adjacency.CenterLeft:
                    if (top == '2') graphicsMap[c, r] = new Vector2(4, 6);
                    break;

                    case adjacency.CenterRight:
                    if (top == '5') graphicsMap[c, r] = new Vector2(4, 7);
                    break;
                }
                
            }
        }

        private void UpdateAllTiles()
        {
            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                char t = mapData[c, r];

                if (t == '1')
                    UpdateTile(c, r);
                    SecondaryTileUpdate(c, r);
                    ExtraAdjustments(c, r);

                if (t == '1' || t == '2' || t == '3' || t == '4' || t == '5')
                {
                    tileList[c, r].spritesheet_coords = graphicsMap[c, r];
                }
            }
        }

        private void GenerateBg(string bg)
        {
            int[] bgBuffer = new int[mapWidth * mapHeight];

            for (int i = 0; i < bg.Length; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;
                int index = GetIndex(c, r);
                
                string s = bg[index] + "";
                int n = Int32.Parse(s);

                bgBuffer[index] = n;
                bgMapData[c, r] = bgBuffer[index];
            }

            UpdateAllBg();
        }

        private void UpdateBgTile(int c, int r)
        {
            int lft = 0;
            int rgt = 0;
            int top = 0;
            int bot = 0;

            if (c > 0) lft = bgMapData[c - 1, r];
            if (c < mapWidth - 1) rgt = bgMapData[c + 1, r];
            if (r > 0) top = bgMapData[c, r - 1];
            if (r < mapHeight - 1) bot = bgMapData[c, r + 1];

            if (r == mapHeight - 1) bot = 1;
            if (r == 0) top = 1;

            if (lft != 1 && rgt == 1 && top != 1 && bot == 1)
            {
                // top left corner
                bgGraphicsMap[c, r] = new Vector2(0, 0);
            }
            else if (lft == 1 && rgt == 1 && top != 1 && bot == 1)
            {
                // middle floor
                bgGraphicsMap[c, r] = new Vector2(1, 0);
            }
            else if (lft == 1 && rgt != 1 && top != 1 && bot == 1)
            {
                // top right corner
                bgGraphicsMap[c, r] = new Vector2(2, 0);
            }
            else if (lft != 1 && rgt == 1 && top == 1 && bot == 1)
            {
                // left wall
                bgGraphicsMap[c, r] = new Vector2(0, 1);
            }
            else if (lft == 1 && rgt == 1 && top == 1 && bot == 1)
            {
                // center
                bgGraphicsMap[c, r] = new Vector2(1, 1);
            }
            else if (lft == 1 && rgt != 1 && top == 1 && bot == 1)
            {
                // right wall
                bgGraphicsMap[c, r] = new Vector2(2, 1);
            }
            else if (lft != 1 && rgt == 1 && top == 1 && bot != 1)
            {
                // bottom left corner
                bgGraphicsMap[c, r] = new Vector2(0, 2);
            } 
            else if (lft == 1 && rgt == 1 && top == 1 && bot != 1)
            {
                // middle ceiling
                bgGraphicsMap[c, r] = new Vector2(1, 2);
            }
            else if (lft == 1 && rgt != 1 && top == 1 && bot != 1)
            {
                // bottom right corner
                bgGraphicsMap[c, r] = new Vector2(2, 2);
            }
            else if (lft != 1 && rgt != 1 && top != 1 && bot == 1)
            {
                // '
                bgGraphicsMap[c, r] = new Vector2(3, 0);
            }
            else if (lft != 1 && rgt != 1 && top == 1 && bot == 1)
            {
                // -
                bgGraphicsMap[c, r] = new Vector2(3, 1);
            }
            else if (lft != 1 && rgt != 1 && top == 1 && bot != 1)
            {
                // .
                bgGraphicsMap[c, r] = new Vector2(3, 2);
            }
            else if (lft != 1 && rgt == 1 && top != 1 && bot != 1)
            {
                // *--
                bgGraphicsMap[c, r] = new Vector2(0, 3);
            }
            else if (lft == 1 && rgt == 1 && top != 1 && bot != 1)
            {
                // -*-
                bgGraphicsMap[c, r] = new Vector2(1, 3);
            }
            else if (lft == 1 && rgt != 1 && top != 1 && bot != 1)
            {
                // --*
                bgGraphicsMap[c, r] = new Vector2(2, 3);
            }
            else 
            {
                bgGraphicsMap[c, r] = new Vector2(3, 3);
            }            
        }

        private void UpdateAllBg()
        {
            for (int i = 0; i < mapWidth * mapHeight; i++)
            {
                int c = i % mapWidth;
                int r = i / mapWidth;

                UpdateBgTile(c, r);
            }
        }

        private void ResetGenerate(String map)
        {
            char[] chars = map.ToCharArray();
            for (int i = 0; i < map.Length; i++)
            {
                String s = chars[i] + "";

                int c = i % mapWidth;
                int r = i / mapWidth;
                Vector2 pos = new Vector2(c * (32 * scale.X), r * (32 * scale.Y) - 1);

                switch (s)
                {
                    case "e":
                        NewObject(pos, "can_enemy00");
                        break;

                    case "f":
                        Platform p1 = platforms[c, r];
                        p1.breaking = false;
                        p1.hidden = false;
                        p1.Reset();
                        break;
                }

                if (c < mapWidth && r < mapHeight) mapData[c, r] = chars[i];
            }
        }

        private void NewObject(Vector2 position, String type)
        {
            switch (type)
            {
                case "player":
                    Entities.Player player = new Entities.Player(position, game, handler, game.player_spritesheet, this);
                    handler.players.Add(player);

                    game.camera.position = player.position;
                    break;
                case "spike_floor":
                    Entities.Hazard spike0 = new Entities.Hazard(position, new Vector2(32, 14),
                        new Vector2(0, 0), "static", Vector2.Zero,
                        game.hazard_spritesheet, handler, game);
                    handler.hazards.Add(spike0);

                    spike0.tx = (int)(position.X / 32);
                    spike0.ty = (int)(position.Y / 32) + 1;
                    break;
                case "spike_ceiling":
                    Entities.Hazard spike1 = new Entities.Hazard(position, new Vector2(32, 14),
                        new Vector2(1, 0), "static", Vector2.Zero,
                        game.hazard_spritesheet, handler, game);
                    handler.hazards.Add(spike1);
                    break;
                case "can_enemy00":
                    Entities.Enemy can00 = new Entities.Enemy(position, "can00", handler, game);
                    handler.enemies.Add(can00);
                    break;
            }
        }
        
        private void NewPlatform(int c, int r, int type)
        {
            Vector2 pos = new Vector2(c * 32, r * 32);
            Platform p = new Platform(pos, 1, handler, game);
            platforms[c, r] = p;
            handler.platforms.Add(platforms[c, r]);
            platforms[c, r].position.X -= 1;
            platforms[c, r].position.Y -= 1;
        }

        private void NewTile(String map, int n)
        {
            char[] chars = map.ToCharArray();

            int x = n % mapWidth;
            int y = n / mapWidth;
            Vector2 pos = new Vector2(x * (32 * scale.X), y * (32 * scale.Y));

            // check for adjacent tiles
            String i = ".";
            String l = ".";
            String r = ".";
            String t = ".";
            String b = ".";

            String lt = ".";
            String rt = ".";
            String lb = ".";
            String rb = ".";


            if (x > 0) l = chars[(x - 1) + mapWidth * y] + "";
            if (n < map.Length - 1) r = chars[(x + 1) + mapWidth * y] + "";
            if (y > 0) t = chars[x + mapWidth * (y - 1)] + "";
            if (y < mapHeight - 1) b = chars[x + mapWidth * (y + 1)] + "";

            /*
            if (x > 0 && y > 0) lt = chars[(x - 1) + mapWidth * (y - 1)] + "";
            if (n < map.Length - 1 && y > 0) rt = chars[(x + 1) + mapWidth * (y - 1)] + "";
            if (x > 0 && y < mapHeight - 1) lb = chars[(x - 1) + mapWidth * (y + 1)] + "";
            if (n < map.Length - 1 && y < mapHeight - 1) rb = chars[(x + 1) + mapWidth * (y + 1)] + "";
            */

            if (x > 2 && x < map.Length - 2 && y > 2 && y < mapHeight - 2)
            {
                int ltx = x - 1;
                int lty = y - 1;

                int rtx = x + 1;
                int rty = y - 1;

                int lbx = x - 1;
                int lby = y + 1;

                int rbx = x + 1;
                int rby = y + 1;

                lt = chars[GetIndex(ltx, lty)] + "";
                rt = chars[GetIndex(rtx, rty)] + "";
                lb = chars[GetIndex(lbx, lby)] + "";
                rb = chars[GetIndex(rbx, rby)] + "";
            }

            i = chars[n] + "";


            Vector2 ss_coords = new Vector2(0, 0);
            graphicsMap[x, y] = ss_coords;
            Entities.Tile tile = new Entities.Tile(pos, new Vector2(x, y), ss_coords, game.tile_spritesheet, false, this);
            handler.tiles.Add(tile);
            tileList[x, y] = tile;
        }

        private void NewSlope(String map, int n, int symbol)
        {
            char[] chars = map.ToCharArray();

            int x = n % mapWidth;
            int y = n / mapWidth;
            Vector2 pos = new Vector2(x * (32 * scale.X), y * (32 * scale.Y));

            Vector2 ss_coords = Vector2.Zero;

            switch (symbol)
            {
                case 2:
                ss_coords = new Vector2(0, 6);
                break;

                case 3:
                ss_coords = new Vector2(1, 6);
                break;

                case 4:
                ss_coords = new Vector2(2, 6);
                break;

                case 5:
                ss_coords = new Vector2(3, 6);
                break;
            }

            graphicsMap[x, y] = ss_coords;
            Entities.Tile tile = new Entities.Tile(pos, new Vector2(x, y), ss_coords, game.tile_spritesheet, true, this);
            handler.tiles.Add(tile);
            tileList[x, y] = tile;
        }

        public void Update(GameTime gameTime)
        {
            int cx = game.camera.x;
            int cy = game.camera.y;

            frameLft = cx - 12;
            if (frameLft < 0) frameLft = 0;

            frameRgt = cx + 12;
            if (frameRgt > mapWidth) frameRgt = mapWidth;

            frameTop = cy - 7;
            if (frameTop < 0) frameTop = 0;

            frameBot = cy + 7;
            if (frameBot > mapHeight) frameBot = mapHeight;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            DrawTiles(_spriteBatch);
            //foreach (Entities.Tile t in handler.tiles) t.Draw(_spriteBatch);
        }

        private int GetIndex(int x, int y)
        {
            return x + mapWidth * y;
        }

        public void DrawBgTiles(SpriteBatch _spritebatch)
        {
            int lft = frameLft;
            int rgt = frameRgt;
            int top = frameTop;
            int bot = frameBot;

            for (int r = top; r < bot; r++)
            {
                for (int c = lft; c < rgt; c++)
                {
                    if (bgMapData[c, r] == 1)
                    {
                        int ssx = (int)bgGraphicsMap[c, r].X;
                        int ssy = (int)bgGraphicsMap[c, r].Y;
                        _spritebatch.Draw(game.bg_tileset_ss.frames[ssx, ssy], new Vector2(c * 32 - 1, r * 32 - 1), Color.White);
                    }
                }
            }
        }

        public void DrawTiles(SpriteBatch _spriteBatch)
        {
            int lft = frameLft;
            int rgt = frameRgt;
            int top = frameTop;
            int bot = frameBot;

            int cx = game.camera.x;
            int cy = game.camera.y;

            for (int r = top; r < bot; r++)
            {
                for (int c = lft; c < rgt; c++)
                {
                    switch (mapData[c, r])
                    {
                        case '1': case '2': case '3': case '4': case '5':
                        /*
                        int ssx = (int)graphicsMap[c, r].X;
                        int ssy = (int)graphicsMap[c, r].Y;

                        _spriteBatch.Draw(ss.frames[ssx, ssy],
                        new Vector2((c * 32) + 1, (r * 32) + 1),
                        //new Vector2((c * 32), (r * 32)), 
                        null, 
                        Color.White, 
                        0, 
                        new Vector2(1, 1), 
                        new Vector2(1, 1), 
                        SpriteEffects.None, 
                        1);
                        */
                        tileList[c, r].Draw(_spriteBatch);
                        //tileList[c, r].DrawColliders(_spriteBatch);
                        break;
                        /*
                        case '2':
                        tileList[c, r].Draw(_spriteBatch);
                        break;

                        case '3':
                        tileList[c, r].Draw(_spriteBatch);
                        break;

                        case '4':
                        tileList[c, r].Draw(_spriteBatch);
                        break;

                        case '5':
                        tileList[c, r].Draw(_spriteBatch);
                        break;
                        */
                        case 't':
                        _spriteBatch.Draw(game.tree00,
                        new Vector2((c * 32), (r * 32) + 1),
                        null,
                        Color.White,
                        0,
                        new Vector2(1, 1),
                        new Vector2(2, 2),
                        SpriteEffects.None,
                        1);
                        break;

                        case '^':
                        _spriteBatch.Draw(game.hazard_spritesheet.frames[0, 0],
                        new Vector2(c * 32, r * 32 - 1),
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        new Vector2(1, 1),
                        SpriteEffects.None,
                        1);
                        break;
                        
                        case 'v':
                        _spriteBatch.Draw(game.hazard_spritesheet.frames[1, 0],
                        new Vector2(c * 32, r * 32 - 1),
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        new Vector2(1, 1),
                        SpriteEffects.None,
                        1);
                        break;

                        case 'f':
                        if (platforms[c, r] != null) platforms[c, r].Draw(_spriteBatch);
                        break;

                        case 'd':
                        _spriteBatch.Draw(game.doorImg, new Vector2(c * 32, r * 32 - 1), Color.White);
                        break;
                    }
                }
            }
        }

        public void DrawDebugView(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(game.blank_spritesheet.blank, new Rectangle(0, 0, mapWidth * 12, mapHeight * 12), Color.Black * 0.5f);
            
            _spriteBatch.DrawString(game.debugFont, "w: " + w, new Vector2(0, 170), Color.White);
            _spriteBatch.DrawString(game.debugFont, "h: " + h, new Vector2(0, 180), Color.White);

            int spacing = 10;
            //int spacing = 12;

            int xMod = 0;
            if (frameLft > 24)
            {
                xMod = ((frameLft - 48) * spacing) + 24;
            }

            int lf = frameLft - 64;
            if (lf < 0) lf = 0;

            int rg = frameRgt + 64;
            if (rg > mapWidth) rg = mapWidth;

            for (int r = 0; r < mapHeight; r++)
            {
                for (int c = 0; c < mapWidth; c++)
                {
                    //_spriteBatch.DrawString(game.debugFont, "" + debugMap[c, r], new Vector2((c * 16) + 32, (r * 16) + 32), Color.Red);

                    bool inFrame = false;
                    Color color = Color.Red;

                    foreach (Entities.Player p in handler.players)
                    {
                        if (c < p.tileMapX + (screenWidth / 2) &&
                            c > p.tileMapX - (screenWidth / 2) &&
                            r < p.tileMapY + (screenHeight / 2) &&
                            r > p.tileMapY - (screenHeight / 2)) inFrame = true;

                        if (p.tileMapX == c &&
                            p.tileMapY == r)
                        {
                            _spriteBatch.DrawString(game.debugFont, "@", new Vector2((c * spacing) - xMod, (r * spacing)), Color.Yellow);
                        }
                        else
                        {
                            if (inFrame) color = Color.LightSeaGreen; else if (!inFrame) color = Color.Red;
                            _spriteBatch.DrawString(game.debugFont, "" + mapData[c, r], new Vector2((c * spacing) - xMod, (r * spacing)), color);
                        }
                    }

                    foreach (Entities.Enemy e in handler.enemies)
                    {
                        if (e.tileMapX == c &&
                            e.tileMapY == r)
                        {
                            _spriteBatch.DrawString(game.debugFont, "e", new Vector2((c * spacing) - xMod, (r * spacing)), Color.White);
                        }
                    }

                    foreach (Entities.Hazard h in handler.hazards)
                    {
                        if (h.tx == c && h.ty == r)
                        {
                            if (h.sprite_coords.X == 0 && h.sprite_coords.Y == 0)
                            {
                                _spriteBatch.DrawString(game.debugFont, "^", new Vector2((c * spacing) - xMod, (r * spacing)), Color.White);
                            }
                            else if (h.sprite_coords.X == 1 && h.sprite_coords.Y == 0)
                            {
                                _spriteBatch.DrawString(game.debugFont, "v", new Vector2((c * spacing) - xMod, (r * spacing)), Color.White);
                            }
                        }
                    }
                }
            }
        }

        public Vector2 TileCoordsFromPoint(Vector2 point)
        {
            Vector2 val = Vector2.Zero;
            int tx = (int)point.X / 32;
            int ty = (int)point.Y / 32;
            return val;
        }

        public char TileFromPoint(Vector2 point)
        {
            char ch = '0';

            int tx = (int)point.X / 32;
            int ty = (int)point.Y / 32;

            if (point.X > 0 && point.X < w * 32 &&
                point.Y > 0 && point.Y < h  * 32)
            {   
                
                ch = mapData[tx, ty];
            }

            return ch;
        }

        public bool ContainsTile(int x, int y)
        {
            bool contain = false;

            if (x > -1 && x < w + 1 &&
                y > -1 && y < h + 1)
            {
                if (mapData[x, y].Equals('1') && tileList[x, y].center == false) contain = true;
            }

            return contain;
        }

        public bool ContainsPlatform(int x, int y)
        {
            bool contain = false;

            if (x > -1 && x < w + 1 &&
                y > -1 && y < h + 1)
            {
                if (mapData[x, y].Equals('f')) contain = true;
            }

            return contain;
        }

        public bool ContainsCollider(Vector2 point)
        {
            bool contain = false;

            int tx = (int)(point.X / 32);
            int ty = (int)(point.Y / 32);

            if (point.X > 0 && point.X < w * 32 &&
                point.Y > 0 && point.Y < h  * 32)
            {
                if (ContainsTile(tx, ty))
                {
                    Tile t = tileList[tx, ty];

                    if (point.X < t.position.X + 32 &&
                        point.X > t.position.X &&
                        point.Y < t.position.Y + 32 &&
                        point.Y > t.position.Y)
                    {
                        if (!t.center) contain = true;
                    }
                }
                else if (ContainsPlatform(tx, ty))
                {
                    Platform plt = platforms[tx, ty];
                    if (plt.collision == true)
                    {
                        if (point.X < plt.position.X + 32 &&
                        point.X > plt.position.X &&
                        point.Y < plt.position.Y + 32 &&
                        point.Y > plt.position.Y)
                        {
                            contain = true;
                        }
                    }
                }
            }
            return contain;
        }

        public bool ContainsSlope(Vector2 point)
        {
            bool contain = false;

            char ch = TileFromPoint(point);
            switch (ch)
            {
                case '2':
                contain = true;
                break;

                case '3':
                contain = true;
                break;

                case '4':
                contain = true;
                break;

                case '5':
                contain = true;
                break;
            }

            return contain;
        }

        public Entities.Tile GetTile(int c, int r)
        {
            Entities.Tile t = null;
            
            if (ContainsTile(c, r)) t = tileList[c, r];
            return t;
        }

        public int[] InitSlopeArray(int step_w, int start, int length, int slope)
        {
            int[] new_array = new int[length];
            int step = 0;
            int y = start;

            for (int x = 0; x < length; x++)
            {
                //y += 2 * slope;
                if (step == step_w)
                {
                    y += 2 * slope;
                    step = 0;
                }

                new_array[x] = y;
                step++;
            }
            return new_array;
        }
    }
}
