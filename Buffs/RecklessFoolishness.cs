using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class RecklessFoolishness : ModBuff
	{
        private static AuraBehavior BuffAura => new RecklessAura1();
        private static AuraBehavior BuffAura2 => new RecklessAura2();

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Reckless Foolishness");
			// Description.SetDefault("");
			BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            int gift = Main.LocalPlayer.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift;
            if (gift < 600)
            {
                tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.RecklessFoolishness.Description2");
            }
            else
                tip = Language.GetTextValue("Mods.LobotomyCorp.Buffs.RecklessFoolishness.Description");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
			player.moveSpeed += 0.15f;

            player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGiftActive = true;
            int gift = player.GetModPlayer<LobotomyHePlayer>().LifeForADareDevilGift;
            if (gift < 600)
            {
                player.endurance -= 0.15f;
                LobotomyModPlayer.ModPlayer(player).CurrentAura = BuffAura2;
            }
            else if (gift < 1200)
            {
                player.endurance -= 0.1f;
                LobotomyModPlayer.ModPlayer(player).CurrentAura = BuffAura;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class RecklessAura1 : AuraBehavior
    {
        public int intensity => 3;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("Misc/FlameParticlesL").Value; }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) { return Color.Orange * 0.9f * (1f - particle.particleTime / 13f); }

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

    public class RecklessAura2 : AuraBehavior
    {
        public int intensity => 3;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("Misc/FlameParticlesL").Value; }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) { return Color.Red * 0.9f * (1f - particle.particleTime / 13f); }

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