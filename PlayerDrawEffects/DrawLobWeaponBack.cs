using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System;
using LobotomyCorp.Players;

namespace LobotomyCorp.PlayerDrawEffects
{
    public class DrawLobWeaponBack : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
            if (!Player.HeldItem.IsAir && Player.itemAnimation != 0 && Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Technology.SolemnLamentR>())
            {
                LobotomyGlobalItem item = Player.HeldItem.GetGlobalItem<LobotomyGlobalItem>();

                if (!item.CustomDraw)
                    return;

                Texture2D texture = Mod.Assets.Request<Texture2D>("Items/Ruina/Technology/SolemnLamentS1").Value;

                Color color = Lighting.GetColor((int)(Player.Center.X / 16f), (int)(Player.Center.Y / 16f));

                Vector2 position = drawInfo.ItemLocation - Main.screenPosition;
                Vector2 origin = new Vector2(Player.direction == 1 ? 0 : texture.Width, texture.Height);
                float rot = Player.itemRotation;

                if (Player.HeldItem.useStyle == 5)
                {
                    Vector2 textureCenter = new Vector2((float)(texture.Width / 2f), (float)(texture.Height / 2f));

                    float num = 10f;
                    Vector2 result = textureCenter;
                    result.X = num;
                    ItemLoader.HoldoutOffset(Player.gravDir, Player.HeldItem.type, ref result);

                    /*if (Player.GetModPlayer<LobotomyModPlayer>().SolemnSwitch)
                    {
                        Player.itemRotation -= MathHelper.ToRadians(45) * Player.direction;
                        rot += MathHelper.ToRadians(45) * Player.direction;
                    }*/

                    Vector2 PlayerItemPos = result;

                    int x = (int)PlayerItemPos.X;
                    textureCenter.Y = PlayerItemPos.Y;
                    origin = new Vector2(-x, texture.Height / 2);
                    if (Player.direction == -1)
                    {
                        origin = new Vector2(texture.Width + x, texture.Height / 2);
                    }
                    position.X += textureCenter.X + (!Player.GetModPlayer<LobotomyWawPlayer>().SolemnSwitch ? 6 : 3) * Player.direction - 28;
                    position.Y += textureCenter.Y;

                    if (!Player.GetModPlayer<LobotomyWawPlayer>().SolemnSwitch)
                    {
                        //rot -= MathHelper.ToRadians(30 + 75 * (1 - (float)Player.itemAnimation / (float)Player.itemAnimationMax)) * Player.direction;
                        float prog = (float)Player.itemAnimation / (float)Player.itemAnimationMax;
                        if (prog > 0.2f)
                        {
                            prog = (prog - 0.2f) / .8f;
                            float limit = 105;
                            if (Player.GetModPlayer<LobotomyWawPlayer>().SolemnLamentFireRate > 1f)
                                limit = 75;
                            rot -= MathHelper.ToRadians(limit * (float)Math.Sin(3.14f * prog)) * Player.direction;
                        }
                    }
                }

                drawInfo.DrawDataCache.Add(
                    new DrawData(
                        texture, //pass our glowmask's texture
                        position, //pass the position we should be drawing at from the PlayerDrawInfo we pass into this method. Always use this and not Player.itemLocation.
                        texture.Frame(), //our source rectangle should be the entire frame of our texture. If our mask was animated it would be the current frame of the animation.
                        color, //since we want our glowmask to glow, we tell it to draw with Color.White. This will make it ignore all lighting
                        rot, //the rotation of the Player's item based on how they used it. This allows our glowmask to rotate with swingng swords or guns pointing in a direction.
                        origin, //the origin that our mask rotates about. This needs to be adjusted based on the Player's direction, thus the ternary expression.
                        Player.HeldItem.scale, //scales our mask to match the item's scale
                        drawInfo.playerEffect, //the PlayerDrawInfo that was passed to this will tell us if we need to flip the sprite or not.
                        0 //we dont need to worry about the layer depth here
                    ));
            }

            if (!Player.HeldItem.IsAir && Player.itemAnimation != 0 && Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Art.FaintAromaS>() && Player.heldProj > -1 && Main.projectile[Player.heldProj].type == ModContent.ProjectileType<Projectiles.FaintAromaS>())
            {
                Projectile projectile = Main.projectile[Player.heldProj];

                Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
                float rot = projectile.ai[1];
                Vector2 ownerMountedCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(8, 0).RotatedBy(rot);
                Vector2 position = ownerMountedCenter - Main.screenPosition;
                position.X += 8f * Player.direction;
                Vector2 origin = new Vector2(2, 42);

                drawInfo.DrawDataCache.Add(
                    new DrawData(tex, position, tex.Frame(), Lighting.GetColor((int)Player.position.X / 16, (int)Player.position.Y / 16), rot + MathHelper.ToRadians(45), origin, projectile.scale * 1.2f, 0, 0));
            }
        }
    }
}
