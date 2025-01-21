using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    public class Player
    {
        public Vector2 Position => position;
        public Rectangle Hitbox => new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float speed = 200f;
        private float jumpForce = -500f;
        private bool isJumping = false;

        public Player(Texture2D texture, Vector2 startPosition)
        {
            this.texture = texture;
            this.position = startPosition;
            this.velocity = Vector2.Zero;
        }

        public void Update(GameTime gameTime, Block[,] blocks)
        {
            var keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Управление 
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                velocity.X = -speed;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                velocity.X = speed;
            }
            else
            {
                velocity.X = 0;
            }

            // Прыжок
            if (keyboardState.IsKeyDown(Keys.Space) && !isJumping)
            {
                isJumping = true;
                velocity.Y = jumpForce;
            }

            // Применение гравитации
            velocity.Y += 980f * deltaTime;

            // Обновление позиции
            position += velocity * deltaTime;

            // Обработка коллизий
            HandleCollisions(blocks);
        }

private void HandleCollisions(Block[,] blocks)
{
    bool isOnGround = false; 
    Vector2 newPosition = position; // временная переменная для новой позиции

    // Проверка на столкновения с блоками по вертикали
    foreach (var block in blocks)
    {
        if (block != null && Hitbox.Intersects(block.Rectangle))
        {
            
            if (velocity.Y > 0) // падаем
            {
                newPosition.Y = block.Rectangle.Top - Hitbox.Height; // приземляемся сверху
                isOnGround = true; 
                isJumping = false; 
                velocity.Y = 0; // Сбрасываем вертикальную скорость
            }
            else if (velocity.Y < 0) // прыжок
            {
                newPosition.Y = block.Rectangle.Bottom;
                velocity.Y = 0; 
            }
        }
    }

    // Обновляем позицию по вертикали
    position = newPosition;

    // Проверка на столкновения с блоками по горизонтали
    newPosition = position; // Сбрасываем временную переменную для горизонтальных коллизий
    foreach (var block in blocks)
    {
        if (block != null && Hitbox.Intersects(block.Rectangle))
        {
            if (velocity.X > 0) // движемся вправо
            {
                newPosition.X = block.Rectangle.Left - Hitbox.Width; 
                velocity.X = 0;
            }
            else if (velocity.X < 0) // движемся влево
            {
                newPosition.X = block.Rectangle.Right; 
                velocity.X = 0; 
            }
        }
    }

    position = newPosition;

    // Если персонаж на земле, сбрасываем вертикальную скорость
    if (isOnGround)
    {
        velocity.Y = 0; 
    }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}