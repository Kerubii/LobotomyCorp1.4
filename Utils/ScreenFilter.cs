using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace LobotomyCorp.Util
{
    /// <summary>
    /// Overlayer puts a texture on the screen, it would stretch the image to fit the entire screen so it might look wack, Client-side only pls
    /// </summary>
	public class ScreenFilter
    {
        public Texture2D FilterTexture;
        private bool active;
        public int Time;
        public float Opacity;

        public virtual void Initialize()
        {
            active = false;
        }

        public ScreenFilter()
        {
            Time = 0;
            Opacity = 1f;
            active = true;

            Initialize();
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        /// <summary>
        /// Custom behavior
        /// </summary>
        public virtual void Update()
        {
            Opacity -= 0.1f;
        }

        /// <summary>
        /// Conditions for the Overlay to dissapear
        /// </summary>
        /// <returns></returns>
        public virtual bool DeActive()
        {
            return Opacity <= 0;
        }

        public virtual void Draw(SpriteBatch sp, float configOpacity)
        {
            Color color = Color.White * Opacity;
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            Vector2 textureSize = FilterTexture.Size();
            Vector2 scale = screenSize / textureSize;

            sp.Draw(FilterTexture, Vector2.Zero, FilterTexture.Frame(), color * configOpacity, 0, Vector2.Zero, scale, 0, 0);
        }
    }
}
