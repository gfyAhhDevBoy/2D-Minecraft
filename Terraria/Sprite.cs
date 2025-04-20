using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Terraria
{
    internal class Sprite
    {
        protected Texture2D _texture;
        protected Vector2 _position;
        public Color Color { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float X
        {
            get { return Position.X; }
            set
            {
                Position = new Vector2(value, Position.Y);
            }
        }

        public float Y
        {
            get { return Position.Y; }
            set
            {
                Position = new Vector2(Position.X, value);
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                int x = 0;
                int y = 0;
                int width = _texture.Width;
                int height = _texture.Height;

                return new((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)(width * Scale), (int)(height * Scale));
            }
        }

        public Sprite()
        {
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
            Scale = 1f;
            Origin = new Vector2(0, 0);
            Color = Color.White;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(_texture, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(_texture, Rectangle, null, Color, Rotation, new(), SpriteEffects.None, 0);
        }
    }
}
