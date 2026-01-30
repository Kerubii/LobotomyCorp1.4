using LobotomyCorp.Configs;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace LobotomyCorp.Items.Ruina.General
{
    public class RemorseR : SEgoItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults() 
		{
            Item.damage = 15;
			Item.DamageType = DamageClass.Default;
			Item.width = 24;
			Item.height = 24;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ModContent.RarityType<TethR>();
			Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<RemorseHammer>();
            Item.shootSpeed = 16f;
			Item.autoReuse = true;            
		}

        public override bool LobModifyTooltips(List<TooltipLine> tooltips, ref int num)
        {
            num = Item.damage + 25;

            Color rColor = Color.Lerp(Color.Yellow, Color.Cyan, 0.5f + 0.5f * (float)Math.Sin(6.28f * (Main.timeForVisualEffects % 120 / 120f)));
            var RealizedEGOTooltip = new TooltipLine(Mod, "PositivePassive", $"{Terraria.Localization.Language.GetTextValue("Mods.LobotomyCorp.EgoItemTooltip.RealizedEgo")}") { OverrideColor = rColor};
            int index = tooltips.FindIndex(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            tooltips.Insert(index, RealizedEGOTooltip);
            index += 2;

            bool ExtraShow = ModContent.GetInstance<LobotomyConfig>().ExtraPassivesShow;
            //int tooltipIndex = tooltips.IndexOf()
            var Passive = new TooltipLine(Mod, "PositivePassive", $"{PassiveInitialize(GetPassiveList(num), ExtraShow)}")
            { OverrideColor = LobotomyCorp.PositivePE };
            if (Passive != null)
                Passive.Text = Lang.SupportGlyphs(Passive.Text);
            tooltips.Insert(index++, Passive);

            Passive = new TooltipLine(Mod, "NegativePassive", $"{PassiveInitialize(GetPassiveList(num), ExtraShow, true)}") { OverrideColor = LobotomyCorp.NegativePE };
            if (Passive != null)
                Passive.Text = Lang.SupportGlyphs(Passive.Text);
            tooltips.Insert(index++, Passive);

            if (NPC.downedGolemBoss)
            {
                if (Main.LocalPlayer.HasBuff<Buffs.RemorseCrack>())
                {
                    Passive = new TooltipLine(Mod, "ExtraPassive", $"{Terraria.Localization.Language.GetTextValue("Mods.LobotomyCorp.Items." + ItemName() + ".CrackInTheMind")}") { OverrideColor = rColor };
                    tooltips.Insert(index, Passive);
                }
                else
                {
                    Passive = new TooltipLine(Mod, "ExtraPassive", $"{Terraria.Localization.Language.GetTextValue("Mods.LobotomyCorp.Items." + ItemName() + ".AlephVersion")}") { OverrideColor = rColor };
                    tooltips.Insert(index, Passive);
                }
            }
            else
            {
                Passive = new TooltipLine(Mod, "ExtraPassive", $"{Terraria.Localization.Language.GetTextValue("Mods.LobotomyCorp.Items." + ItemName() + ".AlephUpgrade")}") { OverrideColor = rColor };
                tooltips.Insert(index, Passive);
            }

            return false;
        }

        private string PassiveInitialize(string PassiveList, bool Extra, bool Negative = false)
        {
            string[] Passive = PassiveList.Split('|');// PassiveText.Split('|');
            string result = "Voided";
            if (!Negative)
            {
                Passive = Passive[0].Split(new[] { "\n" }, StringSplitOptions.None);
            }
            else
            {
                Passive = Passive[1].Split(new[] { "\n" }, StringSplitOptions.None);
            }

            if (!Extra && Passive.Length > 0)
            {
                for (int i = 0; i < Passive.Length; i++)
                {
                    int dashLocation = Passive[i].IndexOf(" - ");
                    if (dashLocation <= 0)
                        continue;
                    Passive[i] = Passive[i].Substring(0, dashLocation);
                }
            }
            result = string.Join("\n", Passive);
            if (result.EndsWith("\n"))
                result = result.Substring(0, result.Length - 1);

            /*
            if (Negative)
            {
                result += "\nThese Items are incomplete";
            }*/
            return result;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.5f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<RemorseNail>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WoodenHammer)
            .AddIngredient(ItemID.MythrilBar, 5)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.WoodenHammer)
            .AddIngredient(ItemID.OrichalcumBar, 5)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }
    }
}