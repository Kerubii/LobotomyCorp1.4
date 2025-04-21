using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.Aleph
{
    public class Justitia : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("It remembers the balance of the Long Bird that never forgot others' sins.\n" +
                               "This weapon may be able to not only cut flesh but trace of sins as well.\n" +
							   "Attacks hit multiple times\n" +
							   "Has a chance to perform a special attack"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 15;
            Item.knockBack = 3;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<AlephB>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<Projectiles.JustitiaExtended>();
            Item.scale = 1.3f;
            AlternateAttack = false;
            PreviouslyHitNPC = -1;
            EGORiskLevel = RiskLevel.Aleph;
        }

        private bool AlternateAttack;
        private int PreviouslyHitNPC;

        public override float UseSpeedMultiplier(Player player)
        {
            if (AlternateAttack)
                return 0.35f;
            return base.UseSpeedMultiplier(player);
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.rand.NextBool(3))
            {
                Item.UseSound = null;
                AlternateAttack = true;
                Item.noMelee = true;
            }
            else
            {
                Item.UseSound = LobotomyCorp.WeaponSound("judgement1");
                AlternateAttack = false;
                Item.noMelee = false;
            }
            return base.CanUseItem(player);
        }

        public override bool? UseItemAlt(Player player)
        {
            if (AlternateAttack)
            {
                Item.noMelee = true;
            }
            else
            {
                Item.noMelee = false;
            }
            Item.noUseGraphic = AlternateAttack;
            //Jank way of ignoring IFrames
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            PreviouslyHitNPC = target.whoAmI;
        }

        public override void UpdateInventory(Player player)
        {
            if (PreviouslyHitNPC >= 0)
            {
                SetPlayerMeleeCooldown(player, ref PreviouslyHitNPC, 3, 0);
                /*
				Main.npc[PreviouslyHitNPC].immune[player.whoAmI] = 3;
				player.attackCD = 1;
				PreviouslyHitNPC = -1;*/
            }
        }

        public override void UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            if (AlternateAttack)
            {
                Item.useStyle = 5;
                Item.noUseGraphic = true;
            }
            else
            {
                Item.useStyle = 15;
                Item.noUseGraphic = false;
            }
            base.UseStyleAlt(player, heldItemFrame);
        }

        public override void UseItemHitboxAlt(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            noHitbox = false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (!AlternateAttack)
            {
                velocity.Y = 0;
                velocity.X = Item.shootSpeed * Math.Sign(velocity.X);
            }
            else
            {
                type = ModContent.ProjectileType<Projectiles.JustitiaAlt>();
                velocity.Normalize();
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (player.blind)
                damage += 0.2f;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.5f;
            modifiers.ScalingArmorPenetration += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.LightsBane)
            .AddIngredient(ItemID.Feather, 10)
            .AddIngredient(ItemID.Bone, 10)
            .AddIngredient(ItemID.Silk, 20)
            .AddTile(Mod, "BlackBox3")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.BloodButcherer)
            .AddIngredient(ItemID.Feather, 10)
            .AddIngredient(ItemID.Bone, 10)
            .AddIngredient(ItemID.Silk, 20)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }
    }
}