using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ResolutionBuddy;
using System;

namespace SpriteEffects
{
	/// <summary>
	/// Sample demonstrating how pixel shaders can be used to apply special effects to sprite rendering.
	/// </summary>
	public class SpriteEffectsGame : Game
	{
		#region Fields

		GraphicsDeviceManager graphics;

		/// <summary>
		/// Shader to draw the texture with the supplied color.
		/// </summary>
		private Effect passThrough;

		/// <summary>
		/// Shader to draw the texture with inverted color.
		/// </summary>
		private Effect inverseColor;

		/// <summary>
		/// Shader to draw the light map, using the supplied normal map and light direction.
		/// </summary>
		private Effect lightmap;

		/// <summary>
		/// Shader to draw the texture, light correctly using the supplied normal map
		/// </summary>
		private Effect normalmapEffect;
		

		// Textures used by this sample.
		Texture2D cubeTexture;
		Texture2D cubeNormalmapTexture;
		Texture2D catTexture;
		Texture2D catNormalmapTexture;
		Texture2D blank;

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
			Resolution.Init(graphics);
			Resolution.SetDesiredResolution(1280, 720);
			Resolution.SetScreenResolution(1280, 720, false);

			passThrough = Content.Load<Effect>("PassThrough");
			inverseColor = Content.Load<Effect>("InverseColor");
			lightmap = Content.Load<Effect>("LightMap");
			normalmapEffect = Content.Load<Effect>("normalmap");

			catTexture = Content.Load<Texture2D>("cat");
			catNormalmapTexture = Content.Load<Texture2D>("CatNormalMap");
			cubeTexture = Content.Load<Texture2D>("cube");
			cubeNormalmapTexture = Content.Load<Texture2D>("CubeNormalMap");
			blank = Content.Load<Texture2D>("blank");

			spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic.
		/// </summary>
		protected override void Update(GameTime gameTime)
		{
			if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
				Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			Resolution.ResetViewport();

			//This is the light direction to use to light any norma. maps.
			Vector2 dir = MoveInCircle(gameTime, 1.0f);
			Vector3 lightDirection = new Vector3(dir.X, dir.Y, 0f);
			lightDirection.Normalize();

			//Clear the device to XNA blue.
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//Set the light directions.
			//lightmap.Parameters["LightDirection"].SetValue(lightDirection);
			normalmapEffect.Parameters["LightDirection"].SetValue(lightDirection);
			normalmapEffect.Parameters["NormalTexture"].SetValue(catNormalmapTexture);
			normalmapEffect.Parameters["AmbientColor"].SetValue(new Vector3(.4f, 0.4f, 0.4f));
			normalmapEffect.Parameters["LightColor"].SetValue(new Vector3(1f, 1f, 1f));
			lightmap.Parameters["LightDirection"].SetValue(lightDirection);
			lightmap.Parameters["NormalTexture"].SetValue(catNormalmapTexture);

			// Set the normalmap texture.
			//graphics.GraphicsDevice.Textures[1] = catTexture;
			Vector2 pos = Vector2.Zero;

			//Draw the plain texture, first in white and then with red tint.
			pos = Vector2.Zero;
			spriteBatch.Begin(0, null, null, null, null, passThrough);
			spriteBatch.Draw(catTexture, pos, Color.White);
			pos.Y += catTexture.Height;
			spriteBatch.Draw(catTexture, pos, Color.Red);
			spriteBatch.End();

			////Draw the inverse texture, first in white and then with red tint.
			//pos = Vector2.Zero;
			//pos.X += catTexture.Width;
			//spriteBatch.Begin(0, null, null, null, null, inverseColor);
			//spriteBatch.Draw(catTexture, pos, Color.White);
			//pos.Y += catTexture.Height;
			//spriteBatch.Draw(catTexture, pos, Color.Red);
			//spriteBatch.End();

			//Draw the light map, first in white and then with red tint.
			pos = Vector2.Zero;
			pos.X += cubeTexture.Width;
			spriteBatch.Begin(0, null, null, null, null, lightmap);
			spriteBatch.Draw(blank, pos, Color.White);
			pos.Y += cubeTexture.Height;
			spriteBatch.Draw(blank, pos, Color.Red);
			spriteBatch.End();

			//Draw the lit texture.
			pos = Vector2.Zero;
			pos.X += catTexture.Width * 2f;
			spriteBatch.Begin(0, null, null, null, null, normalmapEffect);
			spriteBatch.Draw(catTexture, pos, Color.White);
			pos.Y += catTexture.Height;
			spriteBatch.Draw(catTexture, pos, Color.Red);
			spriteBatch.End();
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
	}
}
