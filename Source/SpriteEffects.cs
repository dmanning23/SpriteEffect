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
		Texture2D cubeTexture;
		Texture2D cubeNormalmapTexture;
		Texture2D catTexture;
		Texture2D catNormalmapTexture;

		// SpriteBatch instance used to render all the effects.
		SpriteBatch spriteBatch;

		RenderTarget2D lightsTarget;
		RenderTarget2D mainTarget;

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
			cubeTexture = Content.Load<Texture2D>("cube");
			cubeNormalmapTexture = Content.Load<Texture2D>("cube_normalmap");

			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			var pp = GraphicsDevice.PresentationParameters;
			lightsTarget = new RenderTarget2D(
				GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
			mainTarget = new RenderTarget2D(
				GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
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
			////Animate the light direction.
			//Vector3 lightDirection = new Vector3(0.0f, -1.0f, 0.9f);
			//lightDirection.Normalize();

			//Animate the light direction.
			Vector2 spinningLight = MoveInCircle(gameTime, 1.5f);
			double time = gameTime.TotalGameTime.TotalSeconds;
			float tiltUpAndDown = 0.5f + (float)Math.Cos(time * 0.75) * 0.1f;
			Vector3 lightDirection = new Vector3(spinningLight * tiltUpAndDown, 1 - tiltUpAndDown);
			lightDirection.Normalize();


			////Render to lightmap texture
			GraphicsDevice.SetRenderTarget(lightsTarget);
			GraphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin();
			RenderStuff(catNormalmapTexture);
			spriteBatch.End();

			//render to color texture
			GraphicsDevice.SetRenderTarget(mainTarget);
			GraphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin();
			RenderStuff(catTexture);
			spriteBatch.End();

			//render both together with pixel shader
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.CornflowerBlue);
			normalmapEffect.Parameters["LightDirection"].SetValue(lightDirection);

			// Set the normalmap texture.
			graphics.GraphicsDevice.Textures[1] = lightsTarget;

			// Begin the sprite batch.
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, normalmapEffect);
			spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
			spriteBatch.End();



			normalmapEffect.Parameters["LightDirection"].SetValue(lightDirection);
			normalmapEffect.Parameters["hasNormal"].SetValue(true);

			// Set the normalmap texture.
			graphics.GraphicsDevice.Textures[1] = catNormalmapTexture;

			// Begin the sprite batch.
			spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, normalmapEffect);
			Vector2 catPos = new Vector2(320.0f, 128);
			float rotation = 0.0f;
			spriteBatch.Draw(catTexture, catPos, null, Color.White, rotation, Vector2.Zero, 1.0f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1.0f);
			catPos.X += 200.0f;

			// Set the normalmap texture.
			normalmapEffect.Parameters["hasNormal"].SetValue(false);
			//normalmapEffect.Parameters["LightDirection"].SetValue(new Vector3(lightDirection.X, lightDirection.Y * -1.0f, lightDirection.Z));
			spriteBatch.Draw(cubeTexture, catPos, null, Color.White, rotation, Vector2.Zero, 1.5f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1.0f);
			spriteBatch.End();
		}

		void RenderStuff(Texture2D tex)
		{
			Vector2 catPos = new Vector2(0, 128);
			float rotation = 0.0f;
			spriteBatch.Draw(tex, catPos, null, Color.White, rotation, Vector2.Zero, 1.0f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1.0f);
			catPos.X += 180.0f;
			spriteBatch.Draw(tex, catPos, null, Color.White, rotation, Vector2.Zero, 1.0f, Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipVertically, 1.0f);
		}

		/// <summary>
		/// Helper calculates the destination position needed
		/// to center a sprite in the middle of the screen.
		/// </summary>
		Vector2 TextureOrigin(Vector2 pos, Texture2D texture)
		{
			float x = pos.X + (texture.Width * 0.5f);
			float y = pos.Y + (texture.Height * 0.5f);

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
