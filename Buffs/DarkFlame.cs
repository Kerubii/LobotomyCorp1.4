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
	public class DarkFlame : ModBuff
	{
        private static AuraBehavior buffAura = new DarkFlameAura();

		public override void SetStaticDefaults()
        {
			// DisplayName.SetDefault("Dark Flame");
			// Description.SetDefault("70% increased Magic Bullet damage");
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<LobotomyWawPlayer>().MagicBulletDarkFlame = true;

            //Vector2 center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -11);
            //Dust d = Dust.NewDustPerfect(center, 172, new Vector2(-1f * player.direction, 0));
            //d.fadeIn = 0.2f;
            //d.noGravity = true;
            DarkFlameDust(player);
            player.GetModPlayer<LobotomyModPlayer>().CurrentAura = buffAura;
        }

        private void DarkFlameDust(Player player)
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

    public class DarkFlameAura : AuraBehavior
    {
        public int intensity => 3;

        public Texture2D GetTexture(Mod mod) { return mod.Assets.Request<Texture2D>("Misc/FlameParticlesL").Value; }

        public Rectangle GetSourceRect(Texture2D texture, int index)
        {
            return texture.Frame(4, 1, index);
        }

        public Color GetColor(PlayerDrawSet drawInfo, AuraParticle particle) 
        {
            Color color = Color.Lerp(Color.DarkBlue, Color.Black, (particle.particleTime / 20f)) * 0.9f;
            if (particle.particleTime > 5)
                color *= 1f - (particle.particleTime - 5) / 20f;
            return color;
        }

        public void SpawnParam(Player player, int dir, float gravDir, float time, AuraParticle particle, int index)
        {
            particle.textureIndex = Main.rand.Next(4);
            particle.Position.Y += player.height / 2 - 10;
            particle.Velocity = new Vector2(Main.rand.NextFloat(-2,2), Main.rand.NextFloat(-2, -3));

            particle.Rotation = Main.rand.NextFloat(6.28f);
            particle.Scale = 1f;
        }

        public void Behavior(Player player, int dir, float gravDir, float time, AuraParticle particle)
        {
            particle.Position += particle.Velocity;
            particle.Scale -= 0.04f;
            particle.Velocity.Y *= 0.9f;
            particle.Velocity.X *= 0.95f;

            if (particle.particleTime > 20f)
            {
                particle.Active = false;
            }
        }
    }
}