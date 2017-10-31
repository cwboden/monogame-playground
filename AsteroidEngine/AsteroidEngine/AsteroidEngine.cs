using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AsteroidEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class AsteroidEngine : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private DebugSprite _sprite1, _sprite2;
        private Color _clearColor, _collisionColor;

        private readonly Rectangle _gameDimensions;

        public AsteroidEngine()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };
            Content.RootDirectory = "Content";

            _gameDimensions = new Rectangle(0, 0, 1280, 720);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _sprite1 = new DebugSprite(new Vector2(0, (_graphics.GraphicsDevice.Viewport.Height / 2.0f) - 120), 
                                        Color.White, 70, 0, _gameDimensions);
            _sprite2 = new DebugSprite(new Vector2(_graphics.GraphicsDevice.Viewport.Width, (_graphics.GraphicsDevice.Viewport.Height / 2.0f) + 120), 
                                        Color.White, 60, (float) Math.PI, _gameDimensions);

            _clearColor = Color.CornflowerBlue;
            _collisionColor = Color.Red;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _sprite1.LoadContent(Content, GraphicsDevice, "ship");
            _sprite2.LoadContent(Content, GraphicsDevice, "ship");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            _sprite1.Unload();
            _sprite2.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Poll for current input state of the keyboard
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _sprite1.Update(gameTime);
            _sprite2.Update(gameTime);

            _sprite1.checkCollided(_sprite2);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (_sprite1.IsCollided || _sprite2.IsCollided)
            {
                GraphicsDevice.Clear(_collisionColor);
            }
            else
            {
                GraphicsDevice.Clear(_clearColor);
            }

            _spriteBatch.Begin();
            _sprite1.Draw(_spriteBatch, gameTime);
            _sprite2.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
