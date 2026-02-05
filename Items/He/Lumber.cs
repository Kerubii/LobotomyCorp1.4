using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class Lumber : LobCorpHeavy
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void LobSetDefaults()
        {
            Item.damage = 96;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.useStyle = 15;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<HeB>();
            SwingSound = new SoundStyle("LobotomyCorp/Sounds/Item/Lumberjack_Atk2") with { Volume = 0.5f, PitchVariance = 0.1f }; ;
            Item.autoReuse = true;
            EGORiskLevel = RiskLevel.He;
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return RedMistMaskUpgrade(player, RiskLevel.He);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (RedMistMaskUpgrade(player, RiskLevel.He))
            {
                int life = Math.Clamp(player.statLife, 100, player.statLifeMax2) - 100;
                float attackspeed = 1.2f * (1f - life / (player.statLifeMax2 - 100f));
                return 1f + attackspeed;
            }

            return base.UseSpeedMultiplier(player);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (RedMistMaskUpgrade(player, RiskLevel.He))
            {
                int life = Math.Clamp(player.statLife, 100, player.statLifeMax2) - 100;
                float add = 0.5f * (life / (player.statLifeMax2 - 100f));
                damage += add + player.GetModPlayer<LobotomyModPlayer>().ChargeWeaponHelper;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life < 0 && target.lifeMax > 20)
            {
                int number = Item.NewItem(target.GetSource_FromThis(), target.position, target.Size, ItemID.Heart);
                if (Main.netMode == 1)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }
        }

        public override void UseStyleAlt(Player player, Rectangle heldItemFrame)
        {
            if (RedMistMaskUpgrade(player, RiskLevel.He) && player.itemAnimation > (int)(player.itemAnimationMax * 0.6f))
            {
                float rotation = lumberRotation(player);
                
                PseudoUseStyleSwing(player, heldItemFrame, rotation);
            }
        }

        private float lumberRotation(Player player)
        {
            float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            float rotation = 0;

            if (prog < 0.4f)
            {
                prog = prog / 0.4f;
                prog = prog * prog;
                rotation = (90 - 150 * prog);// * player.direction;
            }
            return rotation;
        }

        public override bool? UseItemAlt(Player player)
        {
            if (RedMistMaskUpgrade(player, RiskLevel.He))
            {
                if (player.channel)
                {
                    int releasePoint = (int)(player.itemAnimationMax * 0.6f);
                    if (player.itemAnimation < releasePoint + 1)
                    {
                        player.itemAnimation = releasePoint + 1;
                        float add = 1f / (player.itemAnimationMax - releasePoint);
                        player.GetModPlayer<LobotomyModPlayer>().ChargeWeaponHelper += add;
                        if (player.GetModPlayer<LobotomyModPlayer>().ChargeWeaponHelper > 2f)
                            player.GetModPlayer<LobotomyModPlayer>().ChargeWeaponHelper = 2f;
                    }
                }
            }

            return base.UseItemAlt(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronAxe)
            .AddIngredient(ItemID.IronBar, 20)
            .AddIngredient(ItemID.Wood, 50)
            .AddTile(Mod, "BlackBox2")
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.LeadAxe)
            .AddIngredient(ItemID.LeadBar, 20)
            .AddIngredient(ItemID.Wood, 50)
            .AddTile(Mod, "BlackBox2")
            .Register();
        }
    }
}