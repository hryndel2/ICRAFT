using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using NoiseTest;

namespace Game2
{
    public class Block
    {
        public Rectangle Rectangle { get; private set; }
        private Texture2D texture;
        private Rectangle sourceRectangle;

        public Block(Texture2D texture, Rectangle rectangle, Rectangle sourceRectangle)
        {
            this.texture = texture;
            Rectangle = rectangle;
            this.sourceRectangle = sourceRectangle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rectangle, sourceRectangle, Color.White);
        }
    }

    public class Game1 : Game
    {
        public static int ScreenWidth = 1200; // Set your screen width
        public static int ScreenHeight = 1000;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;
        Texture2D tileSetTexture; // Texture atlas
        Player player;
        Block[,] blocks; // 2D массив блоков

        // OpenSimplexNoise instance
        private OpenSimplexNoise noiseGenerator;
        private const int mapWidth = 50; // Ширина карты в тайлах
        private const int mapHeight = 50; // Высота карты в тайлах
        private const int tileSize = 20; // Размер одного тайла
        private const double smoothness = 10.0; // Сглаживание шума

        // Типы тайлов из атласа
        private enum TileType
        {
            Air = -1,
            Grass = 0,
            Dirt = 1,
            Stone = 2
        }

        // Camera instance
        private Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            noiseGenerator = new OpenSimplexNoise();

            // Установка размера окна
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 900;
            graphics.IsFullScreen = false;

            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerTexture = Content.Load<Texture2D>("player");
            tileSetTexture = Content.Load<Texture2D>("TileMap"); // Load texture atlas

            player = new Player(playerTexture, new Vector2(60, 400));
            camera = new Camera(); // Initialize camera
            // Генерация мира
            GenerateTerrain();
        }

        private void GenerateTerrain()
        {
            blocks = new Block[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                double noise = noiseGenerator.Evaluate(x / smoothness, 0);
                int groundLevel = (int)((noise + 1) * mapHeight / 4) + mapHeight / 3;

                for (int y = 0; y < mapHeight; y++)
                {
                    TileType tileType = DetermineTileType(y, groundLevel);

                    if (tileType != TileType.Air)
                    {
                        Rectangle tileRect = new Rectangle(
                            x * tileSize,
                            y * tileSize,
                            tileSize,
                            tileSize
                        );

                        Rectangle sourceRect = GetSourceRectangle(tileType);

                        blocks[x, y] = new Block(tileSetTexture, tileRect, sourceRect);
                    }
                }
            }
        }

        private TileType DetermineTileType(int currentY, int groundLevel)
        {
            if (currentY > groundLevel + 3) return TileType.Stone;
            if (currentY > groundLevel) return TileType.Dirt;
            if (currentY == groundLevel) return TileType.Grass;
            return TileType.Air;
        }

        private Rectangle GetSourceRectangle(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Grass: return new Rectangle(0, 0, 20, 20);
                case TileType.Dirt: return new Rectangle(20, 0, 20, 20);
                case TileType.Stone: return new Rectangle(40, 0, 20, 20);
                default: return Rectangle.Empty;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime, blocks); // Pass blocks to the player

            camera.Follow(player); // Update camera to follow the player

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);

            // Рисуем все блоки
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (blocks[x, y] != null)
                    {
                        blocks[x, y].Draw(spriteBatch);
                    }
                }
            }

            player.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}