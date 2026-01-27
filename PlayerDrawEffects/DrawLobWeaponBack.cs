using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System;
using LobotomyCorp.Players;
using rail;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.Items.Ruina.Technology;
using LobotomyCorp.Items.Ruina.Language;

namespace LobotomyCorp.PlayerDrawEffects
{
    public class DrawLobWeaponBack : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;
            if (!Player.HeldItem.IsAir && Player.ItemAnimationActive)
            {
                Color color = Lighting.GetColor((int)(drawInfo.Position.X + drawInfo.drawPlayer.width * 0.5) / 16, (int)((drawInfo.Position.Y + Player.height * 0.5) / 16.0));

                if (Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Technology.SolemnLamentR>())
                {
                    LobotomyGlobalItem item = Player.HeldItem.GetGlobalItem<LobotomyGlobalItem>();

                    if (!item.CustomDraw)
                        return;

                    Texture2D texture = SolemnLamentR.SolemnGun1.Value;

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

                else if (drawInfo.heldItem.type == ModContent.ItemType<Items.Ruina.Language.CrimsonScarR>())
                {
                    if (Player.altFunctionUse == 2)
                    {
                        float prog = 1f - Player.itemAnimation / (float)Player.itemAnimationMax;
                        LobotomyWawPlayer wawPlayer = Player.GetModPlayer<LobotomyWawPlayer>();
                        Texture2D tex = CrimsonScarR.Gun.Value;
                        Vector2 textureCenter = tex.Size() / 2;

                        float rot = Player.itemRotation;
                        if (wawPlayer.CrimsonScarLowHealthActive)
                        {
                            float rotOff = (float)Math.Sin(3.14f + prog * 9.42f);
                            if (rotOff > 0)
                                rot -= rotOff * MathHelper.ToRadians(60) * Player.direction;
                        }

                        Vector2 itemlocation;
                        itemlocation.X = Player.position.X + Player.width * 0.5f - 52 * 0.5f - (Player.direction * 2);
                        itemlocation.Y = Player.MountedCenter.Y - tex.Height * 0.5f;

                        Vector2 position = itemlocation - Main.screenPosition;
                        float num = 10f;
                        Vector2 result = textureCenter;
                        result.X = num;
                        result.Y += 4;
                        ItemLoader.HoldoutOffset(Player.gravDir, Player.HeldItem.type, ref result);

                        Vector2 playerItemPos = result;

                        int x = (int)playerItemPos.X;
                        textureCenter.Y = playerItemPos.Y;
                        Vector2 origin = new Vector2(-x, tex.Height / 2);
                        if (Player.direction == -1)
                        {
                            origin = new Vector2(tex.Width + x, tex.Height / 2);
                        }
                        position.X += textureCenter.X;
                        position.Y += textureCenter.Y;

                        drawInfo.DrawDataCache.Add(
                            new DrawData(
                                tex,
                                position,
                                tex.Frame(),
                                color,
                                rot,
                                origin,
                                Player.HeldItem.scale,
                                drawInfo.playerEffect,
                                0
                            ));
                    }
                }

                else if (Player.HeldItem.type == ModContent.ItemType<Items.Ruina.Art.FaintAromaS>() && Player.heldProj > -1 && Main.projectile[Player.heldProj].type == ModContent.ProjectileType<Projectiles.FaintAromaS>())
                {
                    Projectile projectile = Main.projectile[Player.heldProj];

                    Texture2D tex = TextureAssets.Projectile[projectile.type].Value;
                    float rot = projectile.ai[1];
                    Vector2 ownerMountedCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(8, 0).RotatedBy(rot);
                    Vector2 position = ownerMountedCenter - Main.screenPosition;
                    position.X += 8f * Player.direction;
                    Vector2 origin = new Vector2(2, 42);

                    drawInfo.DrawDataCache.Add(
                        new DrawData(tex, position, tex.Frame(), color, rot + MathHelper.ToRadians(45), origin, projectile.scale * 1.2f, 0, 0));

                }
            }
        }
    }
}
