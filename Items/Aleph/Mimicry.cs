using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace LobotomyCorp.Items.Aleph
{
    public class Mimicry : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Penitence"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("The yearning to imitate the human form is sloppily reflected on the E.G.O.\n" +
							   "As if it were a reminder that it should remain a mere desire.\n" +
							   "It can deliver a powerful downswing that should be impossible for a human.\n" +
							   "Can be charged for 300% increased damage\n" +
							   "Recovers 25% damage dealt on hit"); */
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        private bool MimicryHeal = false;

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.channel = true;
            Item.rare = ModContent.RarityType<AlephB>();
            MimicryHeal = false;
            //Item.UseSound = SoundID.Item1;
            EGORiskLevel = RiskLevel.Aleph;
        }

        public override bool CanUseItem(Player player)
        {
            Item.scale = 1f;
            LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper = 0;
            MimicryHeal = false;
            return true;
        }

        public override void HoldItem(Player player)
        {
            
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override void UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            LobotomyModPlayer modPlayer = LobotomyModPlayer.ModPlayer(player);
            if (player.channel)
            {
                player.itemAnimation = player.itemAnimationMax;//(int)(Item.useAnimation * ItemLoader.UseSpeedMultiplier(Item, player)) - 1;
                player.itemRotation += Main.rand.NextFloat(-0.06f, 0.06f);
                if (modPlayer.ChargeWeaponHelper < 1f)
                    modPlayer.ChargeWeaponHelper += 0.0166f * player.GetAttackSpeed(DamageClass.Melee);
                else
                {
                    modPlayer.ChargeWeaponHelper = 1f;
                    player.channel = false;
                }
                //Item.scale = 1f + 0.5f * modPlayer.ChargeWeaponHelper;
            }
            if (player.itemAnimation == player.itemAnimationMax - 1)
            {
                SoundStyle swingSound;
                if (modPlayer.ChargeWeaponHelper >= 0.9f)
                    swingSound = new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Skill3_Finish") with { Volume = 0.3f };
                else
                    swingSound = new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Attack1") with { Volume = 0.3f };

                SoundEngine.PlaySound(swingSound, player.Center);
            }
        }

        public override void ModifyItemScale(Player player, ref float scale)
        {
            scale += 1f * LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper;
            base.ModifyItemScale(player, ref scale);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage += 3f * LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper;
        }

        public override void UseItemHitboxAlt(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            noHitbox = player.channel;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (MimicryHeal || target.type == NPCID.TargetDummy)
                return;

            MimicryHeal = true;
            int heal = (int)(damageDone * 0.25f);
            if (heal > 40)
            {
                heal = 40; 
            }
            player.HealEffect(heal);
            player.statLife += heal;
            if (Main.myPlayer == player.whoAmI && LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper >= 0.9f)
                Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.MimicrySEffect>(), 0, 0, player.whoAmI, player.direction);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BreakerBlade)
            .AddIngredient(ItemID.SoulofNight, 8)
            .AddIngredient(ItemID.Vertebrae, 5)
            .AddTile(Mod, "BlackBox3")
            .Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (!player.channel && Main.rand.NextBool(3))
            {
                Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Blood);
            }
        }
    }
}