using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace LobotomyCorp.Projectiles.Realized.Nihil
{
	public class NihilCard : ILoadable
	{
		public static Asset<Texture2D> CardTexture;

        public void Load(Mod mod)
        {
            CardTexture = ModContent.Request<Texture2D>((GetType().Namespace + ".NihilCard").Replace('.', '/'));
        }

        public void Unload()
        {
            CardTexture = null;
        }
    }
}