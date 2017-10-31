using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AsteroidEngine
{
    /// <summary>
    /// The player class.
    /// It handles player movement and sprite control
    /// </summary>
    public class Sprite
    {
        private Color _color;
        private Vector2 _velocity;
        private Vector2 _position;
        private Rectangle _rectangle;
        private Texture2D _texture;
        private Rectangle? _bounds;

        /// <summary>
        /// Creates a Player at an optional given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="speed"></param>
        /// <param name="angle"></param>
        public Sprite(Vector2 position, float speed = 0, float angle = 0, Rectangle? bounds = null)
        {
            _position = position;
            _velocity = new Vector2((float)(speed * Math.Cos(angle)), 
                                    (float)(speed * Math.Sin(angle)));

            _texture = null;
            _color = Color.White;

            _bounds = bounds;
        }

        protected Texture2D Texture => _texture;

        public Vector2 Position => _position;

        public Rectangle Rectangle => _rectangle;

        public bool isCollided { get; private set; }

        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice, string assetName)
        {
            _texture = content.Load<Texture2D>(assetName);

            OnContentLoad(content, graphicsDevice);
        }

        protected virtual void OnContentLoad(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            UpdateRectangle();
        }

        private void UpdateRectangle()
        {
            _rectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);
        }

        public virtual void Unload()
        {
            _texture.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            _position += _velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

            UpdateRectangle();
            CheckBounds();
        }

        private void CheckBounds()
        {
            if (_bounds == null) return;

            Vector2 change = Vector2.Zero;

            if (_rectangle.Left <= _bounds.Value.Left)
            {
                change.X = _bounds.Value.Left - _rectangle.Left;
            }
            else if (_rectangle.Right >= _bounds.Value.Right)
            {
                change.X = _bounds.Value.Right - _rectangle.Right;
            }

            if (_rectangle.Top <= _bounds.Value.Top)
            {
                change.Y = _bounds.Value.Top - _rectangle.Top;
            }
            else if (_rectangle.Bottom >= _bounds.Value.Bottom)
            {
                change.Y = _bounds.Value.Bottom - _rectangle.Bottom;
            }

            if (change == Vector2.Zero) return;

            _position = new Vector2((int)_position.X + change.X, (int)_position.Y + change.Y);
            UpdateRectangle();
        }

        public bool checkCollided(Sprite target)
        {
            bool intersects = _rectangle.Intersects(target._rectangle);

            isCollided = intersects;
            target.isCollided = intersects;
            return intersects;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, _position, _color);
        }
    }
}
