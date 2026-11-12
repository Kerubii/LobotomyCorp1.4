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

namespace LobotomyCorp.Buffs
{//Lasts 20 seconds. Applies after ticking WolfSated to 4. It will check every time the value changes.
	public class CobaltScarWolf : ModBuff
	{
			// DisplayName.SetDefault("Destined to be the Big Bad Wolf");
			// Description.SetDefault("If I can fill my stomach… I’ll be satisfied…");
			int Aura = 0;
	 public override void Update(Player player, ref int buffIndex)
	        {
			
			ShadowMistDust(player);
			player.GetModPlayer<LobotomyWawPlayer>().CobaltScarWolf = true;
			player.moveSpeed += 0.33f; //Adjust this as needed.
            player.wereWolf = true;
			
			if (Aura < 90)
			{
				Aura += 1;
			player.GetModPlayer<LobotomyWawPlayer>().CobaltScarMist = true; 
			AuraBehavior shadowMist = new DarkFlameAura();
            player.GetModPlayer<LobotomyModPlayer>().CurrentAura.Add(shadowMist); //Should work.
            
			player.endurance += 0.25f; // -25% damage taken, flat for 1.5s.

			}
			
			}
	
	        private void ShadowMistDust(Player player)
        {
            int num = player.bodyFrame.Y / 56;
            if (num >= Main.OffsetsPlayerHeadgear.Length)
                num = 0;
            Vector2 value = Main.OffsetsPlayerHeadgear[num];
            value *= player.Directions;
            Vector2 value2 = new Vector2((float)(player.width / 2), (float)(player.height / 2)) + value + (player.MountedCenter - player.Center);
            if (player.face == 19)
            {
                value2.Y -= 5f * player.gravDir;
            }
            if (player.head == 276)
            {
                value2.X += 2.5f * (float)player.direction;
            }
            float y = -11.5f * player.gravDir;
            Vector2 vector = new Vector2(3 * player.direction - ((player.direction == 1) ? 1 : 0), y) + Vector2.UnitY * player.gfxOffY + value2;
            Vector2 value3 = Vector2.Zero;
            if (player.mount.Active && player.mount.Cart)
            {
                int num2 = Math.Sign(player.velocity.X);
                if (num2 == 0)
                {
                    num2 = player.direction;
                }
                value3 = Terraria.Utils.RotatedBy(new Vector2(MathHelper.Lerp(0f, -8f, player.fullRotation / ((float)Math.PI / 4f)), MathHelper.Lerp(0f, 2f, Math.Abs(player.fullRotation / ((float)Math.PI/4f)))), player.fullRotation);
                if (num2 == Math.Sign(player.fullRotation))
                {
                    value3 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(player.fullRotation / ((float)Math.PI / 4f)));
                }
            }


            Vector2 pos1 = player.position + vector + value3;

            Dust dust = Dust.NewDustPerfect(player.Center, 111);
            dust.position = pos1 + (player.position - player.oldPosition);
            dust.noGravity = true;
            dust.velocity = new Vector2(-1.5f * player.direction, 0);
            dust.scale = 1.2f;
        }
    }
	

}