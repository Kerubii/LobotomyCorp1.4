//css_ref ../../tModLoader.dll
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Misc
{
    class MiscAssets : ILoadable
    {
        private string Misc => "LobotomyCorp/Misc/";

        //static Asset<Texture2D> BloodTex = ModContent.Request<Texture2D>("Hell yeah");
        public static Asset<Texture2D> BlueStarShine;

        public void Load(Mod mod)
        {
            BlueStarShine = ModContent.Request<Texture2D>(Misc + "BlueStarShine");
        }

        public void Unload()
        {
            BlueStarShine = null;
        }
    }
}