//css_ref ../../tModLoader.dll
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace LobotomyCorp.Misc.Dusts
{
	public class BlueStarSceneEffect : ModSceneEffect
	{
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int Music => MusicLoader.GetMusicSlot(LobotomyCorp.Instance, "Sounds/Music/BlueStar_Bgm");

        public override bool IsSceneEffectActive(Player player)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<SoundOfAStarBlueStar>())
                    return true;
            }
            return false;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("LobotomyCorp:BlueStar", isActive);
        }
    }
}