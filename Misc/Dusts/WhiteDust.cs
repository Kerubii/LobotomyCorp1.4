//css_ref ../../tModLoader.dll
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Misc.Dusts
{
	public class WhiteDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
            dust.noLight = false;
			dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 8, 8);
        }
    }
}