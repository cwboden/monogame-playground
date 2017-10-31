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
        private readonly Color _color;
        private readonly Vector2 _velocity;
        private float _angle;
        private float _rotationSpeed;
        private Vector2 _position;
        private Rectangle _rectangle;
        private Texture2D _texture;
        private Rectangle? _bounds;
        private Vector2 _origin;

        /// <summary>
        /// Creates a Player at an optional given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="speed"></param>
        /// <param name="angle"></param>
        public Sprite(Vector2 position, float speed = 0, float angle = 0, float rotationSpeed = 0, Rectangle? bounds = null)
        {
            _position = position;
            _angle = angle;
            _rotationSpeed = rotationSpeed;

            _velocity = new Vector2((float)(speed * Math.Cos(angle)), 
                                    (float)(speed * Math.Sin(angle)));

            _texture = null;
            _color = Color.White;

            _bounds = bounds;
        }

        protected Texture2D Texture => _texture;

        public Vector2 Position => _position;

        public Rectangle Rectangle => _rectangle;

        public Vector2 Origin => _origin;

        public bool IsCollided { get; private set; }

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
            Vector2 topLeft = _position - _origin;

            _rectangle = new Rectangle((int)topLeft.X, (int)topLeft.Y, _texture.Width, _texture.Height);
        }

        public virtual void Unload()
        {
            _texture.Dispose();
        }

        public void Update(GameTime gameTime)
        {
            _position += _velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

            UpdateRotation(gameTime);
            UpdateRectangle();
            CheckBounds();
        }

        private void UpdateRotation(GameTime gameTime)
        {
            _angle += (float) (_rotationSpeed * gameTime.ElapsedGameTime.TotalSeconds);

            if (_angle < 0)
            {
                _angle = MathHelper.TwoPi - Math.Abs(_angle);
            }
            else if (_angle > MathHelper.TwoPi)
            {
                _angle = _angle - MathHelper.TwoPi;
            }
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
            bool intersects = _rectangle.Intersects(target._rectangle) && PerPixelCollision(target); ;

            IsCollided = intersects;
            target.IsCollided = intersects;
            return intersects;
        }

        private bool PerPixelCollision(Sprite target)
        {
            var sourceColors = new Color[_texture.Width * _texture.Height];
            _texture.GetData(sourceColors);

            var targetColors = new Color[target._texture.Width * target._texture.Height];
            target._texture.GetData(targetColors);

            var left = Math.Max(_rectangle.Left, target._rectangle.Left);
            var top = Math.Max(_rectangle.Top, target._rectangle.Top);
            var right = Math.Min(_rectangle.Right, target._rectangle.Right);
            var bottom = Math.Min(_rectangle.Bottom, target._rectangle.Bottom);

            var width = right - left;
            var height = bottom - top;

            var intersectingRectangle = new Rectangle(left, top, width, height);

            for (var x = intersectingRectangle.Left; x < intersectingRectangle.Right; ++x)
            {
                for (var y = intersectingRectangle.Top; y < intersectingRectangle.Bottom; ++y)
                {
                    var sourceColor = sourceColors[(x - _rectangle.Left) + (y - _rectangle.Top) * _texture.Width];
                    var targetColor = targetColors[(x - target._rectangle.Left) + (y - target._rectangle.Top) * target._texture.Width];

                    if (sourceColor.A > 0 && targetColor.A > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, _position, null, null, _origin, _angle, null, _color);
        }
    }
}
