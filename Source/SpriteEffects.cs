#region File Description
//-----------------------------------------------------------------------------
// SpriteEffects.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace SpriteEffects
{
	/// <summary>
	/// Sample demonstrating how shaders can be used to
	/// apply special effects to sprite rendering.
	/// </summary>
	public class SpriteEffectsGame : Microsoft.Xna.Framework.Game
	{
		#region Fields

		GraphicsDeviceManager graphics;
		KeyboardState lastKeyboardState = new KeyboardState();
		GamePadState lastGamePadState = new GamePadState();
		KeyboardState currentKeyboardState = new KeyboardState();
		GamePadState currentGamePadState = new GamePadState();

		// Effects used by this sample.
		Effect normalmapEffect;

		// Textures used by this sample.
		Texture2D catTexture;
		Texture2D catNormalmapTexture;
		Texture2D glacierTexture;
		Texture2D waterfallTexture;

		// SpriteBatch instance used to render all the effects.
		SpriteBatch spriteBatch;

		#endregion

		#region Initialization

		public SpriteEffectsGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Loads graphics content.
		/// </summary>
		protected override void LoadContent()
		{
			normalmapEffect = Content.Load<Effect>("normalmap");
			catTexture = Content.Load<Texture2D>("cat");
			catNormalmapTexture = Content.Load<Texture2D>("cat_normalmap");
			glacierTexture = Content.Load<Texture2D>("glacier");
			waterfallTexture = Content.Load<Texture2D>("waterfall");

			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic.
		/// </summary>
		protected override void Update(GameTime gameTime)
		{
			HandleInput();
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			DrawNormalmap(gameTime);
			base.Draw(gameTime);
		}

		/// <summary>
		/// Effect uses a normalmap texture to apply realtime lighting while
		/// drawing a 2D sprite.
		/// </summary>
		void DrawNormalmap(GameTime gameTime)
		{
			// Draw the background image.
			spriteBatch.Begin();
			spriteBatch.Draw(glacierTexture, GraphicsDevice.Viewport.Bounds, Color.White);
			spriteBatch.End();

			// Animate the light direction.
			Vector2 spinningLight = MoveInCircle(gameTime, 1.5f);

			double time = gameTime.TotalGameTime.TotalSeconds;

			float tiltUpAndDown = 0.5f + (float)Math.Cos(time * 0.75) * 0.1f;

			Vector3 lightDirection = new Vector3(spinningLight * tiltUpAndDown,
						1 - tiltUpAndDown);

			lightDirection.Normalize();

			normalmapEffect.Parameters["LightDirection"].SetValue(lightDirection);

			// Set the normalmap texture.
			graphics.GraphicsDevice.Textures[1] = catNormalmapTexture;

			// Begin the sprite batch.
			spriteBatch.Begin(0, null, null, null, null, normalmapEffect);

			// Draw the sprite.
			spriteBatch.Draw(catTexture, CenterOnScreen(catTexture), Color.Azure);

			// End the sprite batch.
			spriteBatch.End();
		}

		/// <summary>
		/// Helper calculates the destination position needed
		/// to center a sprite in the middle of the screen.
		/// </summary>
		Vector2 CenterOnScreen(Texture2D texture)
		{
			Viewport viewport = graphics.GraphicsDevice.Viewport;

			int x = (viewport.Width - texture.Width) / 2;
			int y = (viewport.Height - texture.Height) / 2;

			return new Vector2(x, y);
		}

		/// <summary>
		/// Helper for moving a value around in a circle.
		/// </summary>
		static Vector2 MoveInCircle(GameTime gameTime, float speed)
		{
			double time = gameTime.TotalGameTime.TotalSeconds * speed;

			float x = (float)Math.Cos(time);
			float y = (float)Math.Sin(time);

			return new Vector2(x, y);
		}

		#endregion

		#region Handle Input

		/// <summary>
		/// Handles input for quitting the game.
		/// </summary>
		private void HandleInput()
		{
			lastKeyboardState = currentKeyboardState;
			lastGamePadState = currentGamePadState;

			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);

			// Check for exit.
			if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed)
			{
				Exit();
			}
		}

		#endregion
	}

}
