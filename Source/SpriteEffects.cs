using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ResolutionBuddy;
using System;
using BloomBuddy;

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

		/// <summary>
		/// Shader to draw the texture, light correctly using the supplied normal map
		/// </summary>
		//private Effect rotatedNormalEffect;

		private Effect maskNormalEffect;
		

		// Textures used by this sample.
		Texture2D cubeTexture;
		Texture2D cubeNormalmapTexture;
		private Texture2D cubeMask;
		Texture2D catTexture;
		Texture2D catNormalmapTexture;
		Texture2D blank;

		// SpriteBatch instance used to render all the effects.
		SpriteBatch spriteBatch;

		BloomComponent bloom;

		#endregion

		#region Initialization

		public SpriteEffectsGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			Resolution.Init(graphics);
			Resolution.SetDesiredResolution(1280, 720);
			Resolution.SetScreenResolution(1280, 720, false);

			bloom = new BloomComponent(this);
			bloom.Settings = BloomSettings.PresetSettings[0];
			Components.Add(bloom);
		}

		/// <summary>
		/// Loads graphics content.
		/// </summary>
		protected override void LoadContent()
		{
			passThrough = Content.Load<Effect>("PassThrough");
			inverseColor = Content.Load<Effect>("InverseColor");
			lightmap = Content.Load<Effect>("LightMap");
			normalmapEffect = Content.Load<Effect>("normalmap");
			//rotatedNormalEffect = Content.Load<Effect>("RotationNormalMap");
			maskNormalEffect = Content.Load<Effect>("PaletteSwapRotationNormalMap");

			catTexture = Content.Load<Texture2D>("cat");
			catNormalmapTexture = Content.Load<Texture2D>("CatNormalMap");
			cubeTexture = Content.Load<Texture2D>("cube");
			cubeNormalmapTexture = Content.Load<Texture2D>("CubeNormalMap");
			cubeMask = Content.Load<Texture2D>("cube_Mask");
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
			bloom.BeginDraw();

			Resolution.ResetViewport();

			//This is the light direction to use to light any norma. maps.
			Vector2 dir = MoveInCircle(gameTime, 1.0f);
			Vector3 lightDirection = new Vector3(0f, 1f, .2f);
			lightDirection.Normalize();

			var rotation = (float) gameTime.TotalGameTime.TotalSeconds*.25f;

			//Clear the device to XNA blue.
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//Set the light directions.
			//lightmap.Parameters["LightDirection"].SetValue(lightDirection);
			normalmapEffect.Parameters["LightDirection"].SetValue(lightDirection);
			normalmapEffect.Parameters["NormalTexture"].SetValue(catNormalmapTexture);
			normalmapEffect.Parameters["AmbientColor"].SetValue(new Vector3(.45f, .45f, .45f));
			normalmapEffect.Parameters["LightColor"].SetValue(new Vector3(1f, 1f, 1f));

			//rotatedNormalEffect.Parameters["LightDirection"].SetValue(lightDirection);
			//rotatedNormalEffect.Parameters["NormalTexture"].SetValue(catNormalmapTexture);
			//rotatedNormalEffect.Parameters["AmbientColor"].SetValue(new Vector3(.45f, .45f, .45f));
			//rotatedNormalEffect.Parameters["LightColor"].SetValue(new Vector3(1f, 1f, 1f));
			//rotatedNormalEffect.Parameters["Rotation"].SetValue(rotation);

			maskNormalEffect.Parameters["LightDirection"].SetValue(lightDirection);
			maskNormalEffect.Parameters["NormalTexture"].SetValue(catNormalmapTexture);
			maskNormalEffect.Parameters["HasNormal"].SetValue(true);
			maskNormalEffect.Parameters["AmbientColor"].SetValue(new Vector3(.45f, .45f, .45f));
			maskNormalEffect.Parameters["LightColor"].SetValue(new Vector3(1f, 1f, 1f));
			maskNormalEffect.Parameters["Rotation"].SetValue(rotation);
			maskNormalEffect.Parameters["PaletteSwapTexture"].SetValue(cubeMask);
			maskNormalEffect.Parameters["HasPaletteSwap"].SetValue(false);
			maskNormalEffect.Parameters["PaletteSwapColor"].SetValue(new Vector4(1f, 0f, 0f, 1f));

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

			var catMid = new Vector2(catTexture.Width*0.5f, catTexture.Height*0.5f);

			//Draw the lit texture.
			pos = Vector2.Zero;
			pos.X += catTexture.Width * 2f;
			spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, maskNormalEffect);
			spriteBatch.Draw(catTexture,
				pos + catMid, 
				null,
				Color.White,
				rotation,
				catMid,
				Vector2.One,
				Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
				1f);
			pos.Y += catTexture.Height;
			//spriteBatch.End();

			rotation = -rotation;
			maskNormalEffect.Parameters["Rotation"].SetValue(rotation);
			maskNormalEffect.Parameters["NormalTexture"].SetValue(cubeNormalmapTexture);
			maskNormalEffect.Parameters["HasPaletteSwap"].SetValue(true);

			//spriteBatch.Begin(0, null, null, null, null, rotatedNormalEffect);
			spriteBatch.Draw(cubeTexture,
				pos + catMid,
				null,
				Color.White,
				rotation,
				catMid,
				Vector2.One,
				Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
				1f);
			spriteBatch.End();

			base.Draw(gameTime);
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
