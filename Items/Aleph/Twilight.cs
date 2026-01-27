using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace LobotomyCorp.Items.Aleph
{
    public class Twilight : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Just like how the ever-watching eyes...\n" +
                               "The scale that could measure any and all sin...\n" +
                               "The beak that could swallow everything protected the peace of the Black Forest...\n" +
                               "The wielder of this armament may also bring peace as they did\n" +
							   "Alternate attack to perform to leap forward and back and slash the target\n" +
							   "Alternate attack be used again after 8 seconds while holding this weapon"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 68;
            Item.scale = 1.3f;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<AlephB>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<Projectiles.TwilightSlash>();
            Item.shootSpeed = 24f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            EGORiskLevel = RiskLevel.Aleph;

            SpecialAttackTimer = 0;
        }

        private int SpecialAttackTimer = 0;

        public override void HoldItem(Player player)
        {
            if (SpecialAttackTimer > 0)
            {
                if (SpecialAttackTimer == 1)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
                    for (int index1 = 0; index1 < 5; ++index1)
                    {
                        int index2 = Dust.NewDust(player.position, player.width, player.height, 45, 0.0f, 0.0f, byte.MaxValue, new Color(), Main.rand.Next(20, 26) * 0.1f);
                        Main.dust[index2].noLight = true;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity *= 0.5f;
                    }
                }
                SpecialAttackTimer--;
            }
            base.HoldItem(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return SpecialAttackTimer <= 0;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<Projectiles.TwilightSpecial>();
                damage = (int)(damage * 1.3f);
                velocity.Normalize();
                velocity *= 11f;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = LobotomyCorp.WeaponSound("judgement2_1");
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.useStyle = ItemUseStyleID.Shoot;
                SpecialAttackTimer = 480;
            }
            else
            {
                Item.noUseGraphic = false;
                Item.useStyle = 15;
                Item.UseSound = LobotomyCorp.WeaponSound("judgement1");
                Item.noMelee = false;
            }
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                return 0.29f;
            }
            return base.UseSpeedMultiplier(player);
        }

        public override void UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            ResetPlayerAttackCooldown(player);
            /*
			if (Main.myPlayer == player.whoAmI && player.itemAnimation > (int)(player.itemAnimationMax * 0.7f))
			{
				int amount = 1 + Main.rand.Next(3);
				for (int i = 0; i < amount; i++)
				{
					float dist = 56f + 200 / (amount + 1) * (i + 1);
					Vector2 vel = new Vector2(dist, Main.rand.NextFloat(-32, 32)).RotatedBy(MathHelper.ToRadians(ItemRotation(player) - 90));
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, vel, ModContent.ProjectileType<Projectiles.TwilightEye>(), 0, 0, player.whoAmI, MathHelper.ToRadians(5) * player.direction);
				}
			}*/
            base.UseStyleAlt(player, heldItemFrame);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            /*
			if (LobotomyModPlayer.ModPlayer(player).TwilightSpecial < 9)
            {
				LobotomyModPlayer.ModPlayer(player).TwilightSpecial++;
				if (LobotomyModPlayer.ModPlayer(player).TwilightSpecial == 9)
                {
					SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
					for (int index1 = 0; index1 < 5; ++index1)
					{
						int index2 = Dust.NewDust(player.position, player.width, player.height, 45, 0.0f, 0.0f, (int)byte.MaxValue, new Color(), (float)Main.rand.Next(20, 26) * 0.1f);
						Main.dust[index2].noLight = true;
						Main.dust[index2].noGravity = true;
						Main.dust[index2].velocity *= 0.5f;
					}
					LobotomyModPlayer.ModPlayer(player).TwilightSpecial++;
				}
			}*/
            //target.immune[player.whoAmI] = player.itemAnimation;

            float angle = Main.rand.NextFloat(6.28f);
            Vector2 velocity = new Vector2(16f, 0f).RotatedBy(angle);
            Projectile.NewProjectile(player.GetSource_FromThis(), target.Center - velocity * 15, velocity, ModContent.ProjectileType<Projectiles.TwilightStrikes>(), hit.Damage / (int)(4 * 1.5f), 0, player.whoAmI, target.whoAmI, 3);
        }

        public override bool? CanHitNPC(Player player, NPC target)
        {
            if (target.immune[player.whoAmI] > 0)
                return false;

            return base.CanHitNPC(player, target);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(Mod, "Justitia")
            .AddIngredient(ItemID.SoulofLight, 5)
            .AddIngredient(ItemID.SoulofNight, 10)
            //.AddIngredient(ItemID.DarkShard)
            .AddRecipeGroup("LobotomyCorp:MechTrioSoul", 2)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}