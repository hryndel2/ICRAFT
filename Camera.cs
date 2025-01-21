using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game2
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        private Vector2 _position;
        private float _zoom;
        private float _rotation;

        public Camera()
        {
            _position = Vector2.Zero;
            _zoom = 1f;
            _rotation = 0f;
        }

        public void Follow(Player target)
        {
            // Center the camera on the player
            _position = target.Position - new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            Transform = Matrix.CreateTranslation(new Vector3(-_position, 0)) *
                         Matrix.CreateRotationZ(_rotation) *
                         Matrix.CreateScale(new Vector3(_zoom, _zoom, 1));
        }
    }
}