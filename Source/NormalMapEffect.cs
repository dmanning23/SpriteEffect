using Microsoft.Xna.Framework.Graphics;

namespace SpriteEffects
{
    /// <summary>
    /// The default effect used by SpriteBatch.
    /// </summary>
    public class NormalMapEffect : Effect
    {
        #region Effect Parameters


        #endregion

//		static internal readonly byte[] Bytecode = LoadEffectResource(
//#if DIRECTX
//			"Microsoft.Xna.Framework.Graphics.Effect.Resources.SpriteEffect.dx11.mgfxo"
//#elif PSM
//			"MonoGame.Framework.PSMobile.PSSuite.Graphics.Resources.SpriteEffect.cgx" //FIXME: This shader is totally incomplete
//#else
//			"SpriteEffects.normalmap.mgfxo"
//#endif
//		);

        #region Methods

        /// <summary>
        /// Creates a new SpriteEffect.
        /// </summary>
		public NormalMapEffect(GraphicsDevice device, byte[] bytecode)
			: base(device, bytecode)
        {
        }

        /// <summary>
        /// Creates a new SpriteEffect by cloning parameter settings from an existing instance.
        /// </summary>
		protected NormalMapEffect(NormalMapEffect cloneSource)
            : base(cloneSource)
        {
        }


        /// <summary>
        /// Creates a clone of the current SpriteEffect instance.
        /// </summary>
        public override Effect Clone()
        {
			return new NormalMapEffect(this);
        }

        #endregion
    }
}
