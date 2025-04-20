using Microsoft.Xna.Framework;

namespace Terraria
{
    internal class Camera
    {
        public Vector2 Position;
        public float Zoom = 1f;
        public float Rotation = 0f;

        public Matrix GetViewMatrix()
        {
            //return Matrix.CreateTranslation(new Vector3(-Position, 0f)) * Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom, Zoom, 1f);
            Vector2 snapped = new Vector2((int)Position.X, (int)Position.Y);
            return Matrix.CreateTranslation(new(-snapped, 0f)) * Matrix.CreateScale(Zoom, Zoom, 1f);
        }

        public void SetCenter(Vector2 center, int screenWidth, int screenHeight)
        {
            Position = center - new Vector2(screenWidth / 2, screenHeight / 2);
        }
    }
}
