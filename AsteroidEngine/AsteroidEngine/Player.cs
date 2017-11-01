using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidEngine
{
    public class Player : Sprite
    {
        protected Projectile _projectile;
        protected List<Projectile> _projectiles;

        protected const float MAX_VELOCITY = 10000.0f;
        protected const float MAX_ROTATION_SPEED = (float)Math.PI;
        protected const float TURN_IMPULSE = (float) Math.PI;
        protected const float DRAG_COEFFICIENT = 0.008f;
        protected const float ACCELERATION = 160.0f;
        protected const float PROJECTILE_SPEED = 600.0f;

        protected Vector2 _velocity;
        protected Vector2 _acceleration;
        protected float _rotationSpeed;
        protected float _rotationAcceleration;
        protected Rectangle? _bounds;

        protected KeyboardState _prevState;

        public Player(Vector2 position, float angle = 0, float scale = 1.0f, Rectangle? bounds = null) :
            base(position, angle, scale)
        {
            _angle = angle;
            _bounds = bounds;

            _velocity = Vector2.Zero;
            _acceleration = Vector2.Zero;
            _rotationSpeed = 0;
            _rotationAcceleration = 0;

            _projectiles = new List<Projectile>();

            _prevState = Keyboard.GetState();
        }

        public Vector2 Velocity => _velocity;
        public float RotationSpeed => _rotationSpeed;

        public override void LoadContent(ContentManager content, GraphicsDevice graphicsDevice, string assetName)
        {
            base.LoadContent(content, graphicsDevice, assetName);
            OnContentLoad(content, graphicsDevice);
        }

        protected override void OnContentLoad(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            base.OnContentLoad(contentManager, graphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput(gameTime);
            UpdatePosition(gameTime);
            UpdateRotation(gameTime);
            base.Update(gameTime);
            UpdateProjectiles(gameTime);
            CheckBounds();
        }

        protected virtual void UpdateInput(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            _rotationAcceleration = 0;
            _acceleration = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _rotationAcceleration = -TURN_IMPULSE;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                _rotationAcceleration = TURN_IMPULSE;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                _acceleration = new Vector2((float)Math.Cos(_angle) * ACCELERATION,
                                            (float)Math.Sin(_angle) * ACCELERATION);
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                _acceleration = new Vector2((float)Math.Cos(_angle) * -ACCELERATION,
                                            (float)Math.Sin(_angle) * -ACCELERATION);
            }
            if (keyboardState.IsKeyDown(Keys.Space) && _prevState.IsKeyUp(Keys.Space))
            {
                FireProjectile();
            }

            _prevState = keyboardState;
        }

        protected virtual void UpdatePosition(GameTime gameTime)
        {
            _velocity.X -= _velocity.X * DRAG_COEFFICIENT;
            _velocity.Y -= _velocity.Y * DRAG_COEFFICIENT;

            _velocity.X += _acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _velocity.Y += _acceleration.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _velocity.X = MathEx.Encapsulate(_velocity.X, -MAX_VELOCITY, MAX_VELOCITY);
            _velocity.Y = MathEx.Encapsulate(_velocity.Y, -MAX_VELOCITY, MAX_VELOCITY);

            _position.X += _velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _position.Y += _velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected virtual void UpdateRotation(GameTime gameTime)
        {
            // Drag
            _rotationSpeed -= _rotationSpeed * DRAG_COEFFICIENT;

            // Acceleration
            _rotationSpeed += _rotationAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _rotationSpeed = MathEx.Encapsulate(_rotationSpeed, 
                                                -MAX_ROTATION_SPEED, 
                                                MAX_ROTATION_SPEED);

            _angle += _rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _angle %= MathHelper.TwoPi;
        }

        protected override void UpdateRectangle()
        {
            base.UpdateRectangle();
        }

        protected virtual void UpdateProjectiles(GameTime gameTime)
        {
            foreach (Projectile projectile in _projectiles)
            {
                projectile.Update(gameTime);
            }
        }

        public void NewProjectileType(Projectile projectile)
        {
            _projectile = projectile;
        }

        protected void FireProjectile()
        {
            if (_projectiles.Count >= 5) return;

            Vector2 projectileVelocity = new Vector2((_velocity.X + PROJECTILE_SPEED) * 
                                                        (float)Math.Cos(_angle),
                                                     (_velocity.Y + PROJECTILE_SPEED) * 
                                                        (float)Math.Sin(_angle));

            Projectile newProjectile = new Projectile(_projectile, _position, projectileVelocity, _angle);

            _projectiles.Add(newProjectile);
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

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            foreach (Projectile projectile in _projectiles)
            {
                projectile.Draw(spriteBatch, gameTime);
            }
        }
    }
}
