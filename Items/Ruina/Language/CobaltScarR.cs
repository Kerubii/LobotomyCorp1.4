using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Players;
using LobotomyCorp.Players.DamageType;
using LobotomyCorp.Projectiles.Realized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

//CS is short for CobaltScar

namespace LobotomyCorp.Items.Ruina.Language
{
    public class CobaltScarR : SEgoItem
    {
        int WolfSated = 0; 
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Cobalt Scar"); // By default, capitalization in classnames will damage spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault(GetTooltip());
		}

        public override void SetDefaults() //Base Stats, This is supposed to be a shortsword with extended range, like Starlight. Help me Pls
        {
            EgoColor = LobotomyCorp.WawRarity;

            Item.width = 32; //
            Item.height = 32; //
            Item.value = 3000; //
            Item.rare = ModContent.RarityType<WawR>(); //
            Item.damage = 65; //
            Item.knockBack = 2f; //
            Item.shootSpeed = 25f; // Makes it so it's actually fast swipes.

            Item.shoot = ModContent.ProjectileType<Projectiles.Realized.CobaltScarRSlash>();
            Item.useTime = 8; 
            Item.useAnimation = 8;
            Item.useStyle = 13; //It's a damn Rapier
            Item.autoReuse = true;
            Item.DamageType = LobItemBase.RWBPEnable(Item, DamageClass.Melee, ModContent.GetInstance<ExtractorRed>());
            LobItemBase.ConvertVanillaDamageToExtractor(Item);
            Item.noMelee = true;
            Item.noUseGraphic = true; //Disable Weapon Sprite
            //Item.reuseDelay = 2;
        }

        public override float UseSpeedMultiplier(Player player) //Modifies usespeed and animationspeed
        {
			if (player.altFunctionUse == 2 && !player.HasBuff<Buffs.CobaltScarFailedConsume>() && !player.HasBuff<Buffs.CobaltScarConsuming>() && !player.HasBuff<Buffs.CobaltScarWolf>()&& !player.HasBuff<Buffs.CobaltScarRecentBite>())
			{
				return 0.3f; //yes the bite is slow as shit. 26 frames.
			}
            else if (player.altFunctionUse == 2 && player.HasBuff<Buffs.CobaltScarWolf>() && !player.HasBuff<Buffs.CobaltScarRecentHowl>())
            {
				return 0.1f; //Howl takes about a second and a third. 
			}
            else if (player.HasBuff<Buffs.CobaltScarWolf>())
            {
                return 1.5f; //Swipe Faster while being a wolf. You earned it.
            }
			return base.UseSpeedMultiplier(player);
        }



        //public override bool SafeCanUseItem(Player player) 
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse == 2 && !player.HasBuff<Buffs.CobaltScarFailedConsume>() && !player.HasBuff<Buffs.CobaltScarConsuming>() && !player.HasBuff<Buffs.CobaltScarWolf>()&& !player.HasBuff<Buffs.CobaltScarRecentBite>())
            //checks if the player has failed to eat something, if it is eating something, transformed, or just took a boss bite, in that order.
            {
                knockback = 0f;
                type = ModContent.ProjectileType<Projectiles.Realized.CobaltScarRBite1>(); //this will spawn x than
            }

			else if (player.altFunctionUse == 2 && player.HasBuff<Buffs.CobaltScarWolf>() && !player.HasBuff<Buffs.CobaltScarRecentHowl>())
            {
				
            //Howl, Deals damage based on how many stacks of CobaltScarScarsAmount the enemies have. White Damage if you really want to play into it.
                //velocity = 0;//Don't move the hitbox nor the centerspot of graphics.
                knockback = 2f;
                type = ModContent.ProjectileType<Projectiles.Realized.CobaltScarRHowl>(); //Spawns the spawner projectile.
                player.AddBuff(ModContent.BuffType<Buffs.CobaltScarRecentHowl>(), 480);
			}
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
  
        }






        public override bool AltFunctionUse(Player player)
        {
            return true;
        }




        

        //UpdateInventory and UpdateDead are where WolfSated is used/Modified.
        public override void UpdateInventory(Player player) //yes this will only apply if the user has this in their inventory. If you want to play with stuff that takes away items to deny buffs, feel free to do so.
        {
            int Biteindex = player.FindBuffIndex(ModContent.BuffType<Buffs.CobaltScarRecentBite>());
            int Consumingindex = player.FindBuffIndex(ModContent.BuffType<Buffs.CobaltScarConsuming>());

            if (Biteindex != -1 && player.buffTime[Biteindex] == 230) { //checks 10 ticks after biting a non worm boss. 
                WolfSated += 1;
            }

            if (Consumingindex != -1 && player.buffTime[Consumingindex] == 2 && !player.HasBuff<Buffs.CobaltScarFailedConsume>()) {
                WolfSated += 4;
            }//Best thing I can add in for a check right before it expires.
            if ( player.HasBuff<Buffs.CobaltScarFailedConsume>()|| player.HasBuff<Buffs.CobaltScarWolf>() || player.dead)
            {
                WolfSated = 0; 
            }

            if (WolfSated >= 4 && !player.HasBuff<Buffs.CobaltScarFailedConsume>()&& !player.HasBuff<Buffs.CobaltScarRecentBite>())
            {

                player.AddBuff(ModContent.BuffType<Buffs.CobaltScarWolf>(), 1290);
                

                //Add Howl Here.
                player.AddBuff(ModContent.BuffType<Buffs.CobaltScarRecentHowl>(), 180); // Lower CD for this one.
                WolfSated = 0;
            }


        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CobaltScar>())
            .AddIngredient(ItemID.MoonStone)
            .AddIngredient(ItemID.BloodMoonStarter)
            .AddIngredient(ItemID.LunarTabletFragment, 2)
            .AddTile<Tiles.BlackBox3>()
            .AddCondition(RedMistCond)
            .Register();
        }

        
    }
}
