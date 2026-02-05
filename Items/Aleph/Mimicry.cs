using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }

        private bool MimicryHeal = false;

        public override void LobSetDefaults()
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
            Item.UseSound = new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Attack1") with { Volume = 0.3f };
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
            if (Item.useStyle == 15)
            {
                float rotation = rot(player);
                PseudoUseStyleSwing(player, heldItemFrame, rotation);
            }

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
                SoundStyle swingSound = (SoundStyle) Item.UseSound;
                if (modPlayer.ChargeWeaponHelper >= 0.9f)
                    swingSound = new SoundStyle("LobotomyCorp/Sounds/Item/Nullthing_Skill3_Finish") with { Volume = 0.3f };

                SoundEngine.PlaySound(swingSound, player.Center);
            }
        }

        public override void UseItemFrameAlt(Player player)
        {
            float rotation = rot(player);
            LobItemFrame(player, rotation - 90);
        }

        private float rot(Player player)
        {
            float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            float rotation = 0;
            int startRot = 85;
            int endRot = 120;
            int total = startRot + endRot;

            if (player.altFunctionUse == 2)
            {
                startRot = 85;
                endRot = 230;
                total = startRot + endRot;

                if (prog < 0.6f)
                {
                    prog = (prog) / 0.6f;
                    prog = Easing.EaseOutCubic(prog);
                    rotation = (-startRot + total * prog);//(float)Math.Sin(1.57f * prog));
                }
                else
                {
                    rotation = endRot;
                }

                return rotation;
            }
            

            if (prog < 0.3f)
            {
                prog = (prog) / 0.3f;
                prog = Easing.EaseOutCubic(prog);
                rotation = (-startRot + total * prog);//(float)Math.Sin(1.57f * prog));
            }
            else
            {
                rotation = endRot;
            }
            return rotation;
        }

        public override void ModifyItemScale(Player player, ref float scale)
        {
            scale += 1f * LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            float add = 4.5f * LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper;
            if (LobotomyModPlayer.ModPlayer(player).ChargeWeaponHelper >= 0.95f)
                add = 6f;
            damage += add;
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
            int healMax = 30;
            if (heal > healMax)
            {
                heal = healMax; 
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