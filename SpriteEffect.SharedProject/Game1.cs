using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ResolutionBuddy;
using System;
//using BloomBuddy;

namespace SpriteEffects
{
	/// <summary>
	/// Sample demonstrating how pixel shaders can be used to apply special effects to sprite rendering.
	/// </summary>
	public class Game1 : Game
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

		//BloomComponent bloom;

		#endregion

		#region Initialization

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

#if __IOS__
			var resolution = new ResolutionComponent(this, graphics, new Point(1280, 720), new Point(1280, 720), true, true);
#else
			var resolution = new ResolutionComponent(this, graphics, new Point(1280, 720), new Point(1280, 720), false, false);
#endif

			//bloom = new BloomComponent(this);
			//bloom.Settings = BloomSettings.PresetSettings[0];
			//Components.Add(bloom);
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
			maskNormalEffect = Content.Load<Effect>("AnimationBuddyShader");

			catTexture = Content.Load<Texture2D>("cat");
			catNormalmapTexture = Content.Load<Texture2D>("CatNormalMap");
			cubeTexture = Content.Load<Texture2D>("cube");
			cubeNormalmapTexture = Content.Load<Texture2D>("CubeNormalMap");
			cubeMask = Content.Load<Texture2D>("cube_mask");
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
#if !__IOS__
				Exit();
#endif
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			//bloom.BeginDraw();

			//This is the light direction to use to light any norma. maps.
			Vector2 dir = MoveInCircle(gameTime, 1.0f);
			Vector3 lightDirection = new Vector3(1f, 0f, .2f);
			lightDirection.Normalize();

			var rotation = (float)gameTime.TotalGameTime.TotalSeconds * .25f;

			//Clear the device to XNA blue.
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//Set the light directions.
			normalmapEffect.Parameters["LightDirection"].SetValue(lightDirection);
			normalmapEffect.Parameters["NormalTexture"].SetValue(catNormalmapTexture);
			normalmapEffect.Parameters["AmbientColor"].SetValue(new Vector3(.45f, .45f, .45f));
			normalmapEffect.Parameters["LightColor"].SetValue(new Vector3(1f, 1f, 1f));

			maskNormalEffect.Parameters["LightDirection"].SetValue(lightDirection);
			maskNormalEffect.Parameters["NormalTexture"].SetValue(catNormalmapTexture);
			maskNormalEffect.Parameters["HasNormal"].SetValue(true);
			maskNormalEffect.Parameters["AmbientColor"].SetValue(new Vector3(.45f, .45f, .45f));
			maskNormalEffect.Parameters["LightColor"].SetValue(new Vector3(1f, 1f, 1f));
			maskNormalEffect.Parameters["Rotation"].SetValue(rotation);
			maskNormalEffect.Parameters["ColorMaskTexture"].SetValue(cubeMask);
			maskNormalEffect.Parameters["HasColorMask"].SetValue(false);
			maskNormalEffect.Parameters["ColorMask"].SetValue(new Vector4(1f, 1f, 1f, 1f));
			maskNormalEffect.Parameters["FlipHorizontal"].SetValue(false);

			lightmap.Parameters["LightDirection"].SetValue(lightDirection);
			lightmap.Parameters["NormalTexture"].SetValue(catNormalmapTexture);

			// Set the normalmap texture.
			Vector2 pos = Vector2.Zero;

			//Draw the plain texture, first in white and then with red tint.
			pos = Vector2.Zero;
			DrawPassthrough(pos);

			//Draw the light map, first in white and then with red tint.
			pos = Vector2.Zero;
			DrawLightmap(pos);

			var catMid = new Vector2(catTexture.Width * 0.5f, catTexture.Height * 0.5f);

			//Draw the lit texture.
			maskNormalEffect.Parameters["FlipHorizontal"].SetValue(true);

			pos = Vector2.Zero;
			pos.X += catTexture.Width * 2f;
			spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, maskNormalEffect, Resolution.TransformationMatrix());
			spriteBatch.Draw(catTexture,
				pos + catMid,
				null,
				Color.White,
				rotation,
				catMid,
				Vector2.One,
				Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally,
				1f);
			pos.Y += catTexture.Height;

			rotation = -rotation;
			maskNormalEffect.Parameters["Rotation"].SetValue(rotation);
			maskNormalEffect.Parameters["NormalTexture"].SetValue(cubeNormalmapTexture);
			maskNormalEffect.Parameters["HasColorMask"].SetValue(true);
			maskNormalEffect.Parameters["FlipHorizontal"].SetValue(false);

			spriteBatch.Draw(cubeTexture,
				pos + catMid,
				null,
				Color.Green,
				rotation,
				catMid,
				Vector2.One,
				Microsoft.Xna.Framework.Graphics.SpriteEffects.None,
				1f);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void DrawLightmap(Vector2 pos)
		{
			pos.X += cubeTexture.Width;
			spriteBatch.Begin(0, null, null, null, null, lightmap, Resolution.TransformationMatrix());
			spriteBatch.Draw(blank, pos, Color.White);
			pos.Y += cubeTexture.Height;
			spriteBatch.Draw(blank, pos, Color.Red);
			spriteBatch.End();
		}

		private void DrawPassthrough(Vector2 pos)
		{
			spriteBatch.Begin(0, null, null, null, null, passThrough, Resolution.TransformationMatrix());
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
