using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Visuals.ParticlesAura;

namespace LobotomyCorp.Buffs //Unused.
{
	public class ShadowMist : ModBuff
	{
		public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Shadow Mist");
			// Description.SetDefault("AWOOOOOOOOOOOOOO");
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {


            //Vector2 center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -11);
            //Dust d = Dust.NewDustPerfect(center, 172, new Vector2(-1f * player.direction, 0));
            //d.fadeIn = 0.2f;
            //d.noGravity = true;
            
            AuraBehavior shadowMist = new DarkFlameAura();
            player.GetModPlayer<LobotomyModPlayer>().CurrentAura.Add(shadowMist); //Should work.
            player.GetModPlayer<LobotomyWawPlayer>().CobaltScarMist = true; //Should hide the player.
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            
        }

        //Moved the eye to the Wolf Buff.
    }
}