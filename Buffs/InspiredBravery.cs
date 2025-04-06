using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class InspiredBravery : ModBuff
	{
        private AuraBehavior BuffAura;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Inspired Bravery");
			// Description.SetDefault("10% increased movement speed, melee speed and counter damage");
			BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            BuffAura = new InspiredAura();
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
			player.moveSpeed += 0.1f;
            player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGiftActive = true;
            LobotomyModPlayer.ModPlayer(player).CurrentAura = BuffAura;
            if (player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift < 1200)
            {
                player.AddBuff(ModContent.BuffType<RecklessFoolishness>(), 10);
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class InspiredAura : AuraBehavior
    {
        public int intensity => 3;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("Misc/FlameParticlesL").Value; }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) { return Color.Blue * 0.9f * (1f - particle.particleTime / 13f); }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(4);
            particle.Position.Y -= 12;
            particle.Position.X += (8 * (float)Math.Sin(0.436332f * time));
            particle.Velocity.Y -= 2f;
            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = 1f;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position.Y += particle.Velocity.Y;
            particle.Scale -= 0.04f;

            if (particle.particleTime > 10)
            {
                particle.Active = false;
            }
        }
    }
}