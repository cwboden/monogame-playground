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
        protected readonly Color _color;
        protected float _angle;
        protected float _scaleValue;
        protected Vector2 _scale;
        protected Vector2 _position;
        protected Vector2 _origin;
        protected Rectangle _rectangle;
        protected Texture2D _texture;
        protected Matrix _transform;

        public Sprite(Vector2 position, float angle = 0, float scale = 1.0f)
        {
            _position = position;
            _angle = angle;
            _scale = new Vector2(scale, scale);
            _scaleValue = scale;

            _texture = null;
            _color = Color.White;
            _origin = Vector2.Zero;
        }

        public float Angle => _angle;

        public Texture2D Texture { get { return this._texture; } private set { this._texture = value; } }

        public Vector2 Position => _position;

        public Rectangle Rectangle => _rectangle;

        public Vector2 Origin => _origin;

        public Matrix Transform => _transform;

        public bool IsCollided { get; private set; }

        public virtual void LoadContent(ContentManager content, GraphicsDevice graphicsDevice, string assetName)
        {
            _texture = content.Load<Texture2D>(assetName);

            OnContentLoad(content, graphicsDevice);
        }

        protected virtual void OnContentLoad(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);

            UpdateTransformMatrix();
            UpdateRectangle();
        }

        private void UpdateTransformMatrix()
        {
            // SRT Basis = Reverse Origin * Scale * Rotation * Translation
            _transform = Matrix.CreateTranslation(new Vector3(-_origin, 0)) *
                            Matrix.CreateScale(_scaleValue) *
                            Matrix.CreateRotationZ(_angle) *
                            Matrix.CreateTranslation(new Vector3(_position, 0));
        }

        protected virtual void UpdateRectangle()
        {
            Vector2 topLeft = Vector2.Transform(Vector2.Zero, _transform);
            Vector2 topRight = Vector2.Transform(new Vector2(_texture.Width, 0), _transform);
            Vector2 bottomLeft = Vector2.Transform(new Vector2(0, _texture.Height), _transform);
            Vector2 bottomRight = Vector2.Transform(new Vector2(_texture.Width, _texture.Height), _transform);

            Vector2 min = new Vector2(MathEx.Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X),
                                        MathEx.Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y));
            Vector2 max = new Vector2(MathEx.Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X),
                                    MathEx.Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y));

            _rectangle = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public virtual void Unload()
        {
            _texture.Dispose();
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateTransformMatrix();
            UpdateRectangle();
        }

        public bool CheckCollided(Sprite target)
        {
            bool isIntersected = Collision.HasRectangularCollision(this, target) &&
                                 Collision.HasPerPixelCollision(this, target);

            IsCollided = isIntersected;
            target.IsCollided = isIntersected;

            return isIntersected;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, _position, null, null, _origin, _angle, _scale, _color);
        }
    }
}
