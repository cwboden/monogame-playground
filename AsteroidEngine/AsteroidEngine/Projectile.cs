using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidEngine
{
    public class Projectile : Sprite
    {
        protected Vector2 _velocity;
        protected Rectangle? _bounds;

        public Projectile(Vector2 position, Vector2 velocity, float angle = 0, float scale = 1.0f, Rectangle? bounds = null) :
            base(position, angle, scale)
        {
            _velocity = velocity;
            _bounds = bounds;
        }

        public Projectile(Projectile other, Vector2 position, Vector2 velocity, float angle) :
            base(position, angle, other._scaleValue)
        {
            _velocity = velocity;
            _bounds = other._bounds;

            _texture = other._texture;
            _rectangle = other._rectangle;
            _origin = other._origin;
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePosition(gameTime);
            base.Update(gameTime);

            CheckBounds();
        }

        protected virtual void UpdatePosition(GameTime gameTime)
        {
            _position.X += _velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _position.Y += _velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void CheckBounds()
        {
            if (_bounds == null) return;

            if (_rectangle.Left >= _bounds.Value.Right ||
                _rectangle.Right <= _bounds.Value.Left)
            {
                _velocity.X = -_velocity.X;
            }
            if (_rectangle.Top >= _bounds.Value.Bottom ||
                _rectangle.Bottom <= _bounds.Value.Top)
            {
                _velocity.Y = -_velocity.Y;
            }
        }
    }
}
