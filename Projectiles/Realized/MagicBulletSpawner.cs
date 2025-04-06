using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.Audio;
using Terraria.GameContent;
using System.IO;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using LobotomyCorp.Players;

namespace LobotomyCorp.Projectiles.Realized
{
    public class MagicBulletSpawner : ModProjectile
    {
        public static Asset<Texture2D> MagicBulletPortal;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                MagicBulletPortal = Mod.Assets.Request<Texture2D>("Projectiles/Realized/MagicBulletSpawner", AssetRequestMode.ImmediateLoad);

                Main.QueueMainThreadAction(() =>
                {
                    LobotomyCorp.PremultiplyTexture(MagicBulletPortal.Value);
                });
            }
        }

        public override void Unload()
        {
            MagicBulletPortal = null;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("ANYTHING YOU SAY?");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;

            Projectile.tileCollide = false;
            Projectile.friendly = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.netImportant = true;
        }

        public override void AI()
        {
            int shotTime = 15;
            string Sound = "Matan_NormalShot";
            if (Projectile.hostile)
            {
                shotTime = 55;
                Sound = "Matan_FinalShot";
            }

            if (Projectile.ai[1] == 0)
            {
                //if (Projectile.ai[0] > 0)
                    //Main.NewText("Target name: " + Main.npc[(int)Projectile.ai[0] - 1].FullName);
                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/" + Sound) with { Volume = 0.25f }, Projectile.Center);
                Projectile.rotation = Main.rand.NextFloat(3.14f);
            }

            if (Projectile.ai[1] < shotTime)
            {
                if (Projectile.ai[0] > 0)
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0] - 1];

                    Vector2 spawnerPosition = Projectile.velocity;
                    spawnerPosition.Normalize();
                    Projectile.Center = npc.Center - spawnerPosition * 128f;
                }
                else
                {
                    Vector2 spawnerPosition = Projectile.velocity;
                    spawnerPosition.Normalize();
                    Projectile.Center = Main.player[Projectile.owner].Center - spawnerPosition * 128f;
                }
            }

            if (Projectile.ai[1] == shotTime)
            {
                int damage = Projectile.damage;
                if (Main.player[Projectile.owner].GetModPlayer<LobotomyWawPlayer>().MagicBulletDarkFlame)
                    damage = (int)(damage * 1.7f);
                //WHY IS ITS TRAIL OFFSETTED SO MUCH???
                Vector2 offset = Projectile.velocity * 2;
                if (Projectile.ai[0] > 0 && Projectile.owner == Main.myPlayer)
                {
                    int p = Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center - offset, Projectile.velocity, ModContent.ProjectileType<MagicBulletR>(), damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
                    Main.projectile[p].friendly = Projectile.friendly;
                    Main.projectile[p].hostile = Projectile.hostile;
                    Main.projectile[p].netUpdate = true;
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center - offset, Projectile.velocity, ModContent.ProjectileType<MagicBulletR>(), damage, Projectile.knockBack, 255, Projectile.owner + 1);
                    if (Main.projectile[p].ModProjectile is MagicBulletR)
                    {
                        ((MagicBulletR)Main.projectile[p].ModProjectile).PlayerTarget = true;
                    }
                    Main.projectile[p].friendly = Projectile.friendly;
                    Main.projectile[p].hostile = Projectile.hostile;
                    Main.projectile[p].netUpdate = true;
                }
            }

            Projectile.rotation += 0.1f;

            if (Projectile.ai[1] > shotTime + 20)
                Projectile.scale -= 0.05f;
            if (Projectile.scale <= 0)
                Projectile.Kill();

            Projectile.ai[1]++;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*
            if (Projectile.localAI[0] == 0)
                return false;*/

            Texture2D tex = MagicBulletPortal.Value;
            float rot = Projectile.velocity.ToRotation();
            Vector2 position = Projectile.Center - new Vector2(32, 0).RotatedBy(rot);
            Rectangle frame = tex.Frame();
            Vector2 origin = frame.Size() / 2;
            Vector2 scale = new Vector2(0.66f, 1f) * Projectile.scale;
            Color color = Color.White;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            float rotate = Projectile.rotation / (2f * (float)Math.PI);

            var resizeShader = GameShaders.Misc["LobotomyCorp:Rotate"];
            resizeShader.UseOpacity(1f);
            resizeShader.UseShaderSpecificData(LobotomyCorp.ShaderRotation(rotate));
            resizeShader.Apply(null);

            float progress = 1f;
            if (Projectile.ai[1] < 10)
            {
                progress = Projectile.ai[1] / 10f;
            }
            scale *= progress;

            Main.EntitySpriteDraw(tex, position - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, (Rectangle?)(frame), color, rot, origin, scale * 0.66f, SpriteEffects.None, 0);

            position = Projectile.Center - new Vector2(32 * (1f - scale.Y), 0).RotatedBy(rot);
            Main.EntitySpriteDraw(tex, position - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, (Rectangle?)(frame), color, rot, origin, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPvp(Player target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
