using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LobotomyCorp.Items.He
{
    public class LifeForADaredevil : LobCorpLight
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("An ancient sword.\n" +
                               "Just as its archetype desired, it will be useless in the hands of the frightened\n" +
                               "Ignores target's defense"); */
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Katana);
            Item.useStyle = 15;
            Item.damage = 32;
            Item.DamageType = DamageClass.Generic;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = LobotomyCorp.WeaponSound("katana");
            EGORiskLevel = RiskLevel.He;
        }

        public override void UseItemHitboxAlt(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, 32, 32);
            if (!Main.dedServ)
            {
                Rectangle hitboxSize = Item.GetDrawHitbox(Item.type, player);
                hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, hitboxSize.Width, hitboxSize.Height);
            }
            float adjustedItemScale = player.GetAdjustedItemScale(Item);
            hitbox.Width = (int)(hitbox.Height * adjustedItemScale * 0.8f);
            hitbox.Height = (int)(hitbox.Height * adjustedItemScale * 0.8f);
            if (player.direction == -1)
            {
                hitbox.X -= hitbox.Width;
            }
            if (player.gravDir == 1f)
            {
                hitbox.Y -= hitbox.Height;
            }

            float prog = 1f - player.itemAnimation / (float)player.itemAnimationMax;
            if (prog < .2f)
            {
                if (player.direction == 1)
                {
                    hitbox.X -= hitbox.Width * 1;
                }
                hitbox.Width *= 2;
                hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                hitbox.Height = (int)(hitbox.Height * 1.4);
            }
            else if (prog < .4f)
            {
                if (player.direction == -1)
                {
                    hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
                }
                hitbox.Width = (int)(hitbox.Width * 1.4);
                hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
                hitbox.Height = (int)(hitbox.Height * 1.4);
            }
            else
                noHitbox = true;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
        }

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage.Base = (int)(player.statLifeMax2 * (float)Item.damage * 0.01f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.IronBar, 10)
            .AddIngredient(ItemID.LeadBar, 10)
            .AddIngredient(ItemID.Obsidian, 5)
            .AddTile(Mod, "BlackBox")
            .Register();
        }
    }
}