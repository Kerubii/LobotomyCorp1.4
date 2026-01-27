using System;
using System.Collections.Generic;
using System.Linq;
using LobotomyCorp.Buffs;
using LobotomyCorp.Items.Aleph;
using LobotomyCorp.Items.Waw;
using LobotomyCorp.ModSystems;
using LobotomyCorp.NPCs.RedMist;
using LobotomyCorp.PlayerDrawEffects;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Visuals.DeathAnimations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace LobotomyCorp.Players
{
    public class LobotomyDeathPlayer : ModPlayer
    {
        public override void Load()
        {
            On_Main.DrawPlayers_AfterProjectiles += DrawDeathSpot;
            On_Player.KillMe += SpawnDeathAnimation;
        }

        public override void SetStaticDefaults()
        {
            deathActive = false;
        }

        private void DrawDeathSpot(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            bool draw = false;

            foreach (Player player in Main.player)
            {
                if (player.active)
                {
                    if (!draw)
                    {
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                        draw = true;
                    }
                    LobotomyDeathPlayer dPlayer = player.GetModPlayer<LobotomyDeathPlayer>();
                    if (dPlayer.deathActive && dPlayer.deathAnimation.GetActive)
                    {
                        dPlayer.deathAnimation.Draw();
                    }
                }
            }
            if (draw)
                Main.spriteBatch.End();

            orig(self);
        }

        private void SpawnDeathAnimation(On_Player.orig_KillMe orig, Player self, PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp)
        {
            orig(self, damageSource, dmg, hitDirection, pvp);
            // Ensure its just local and gives it to other players since I trust the player Client more than other Clients
            if (self.whoAmI == Main.myPlayer)
            {
                int death = GetDeathAnimation(damageSource, self);
                if (death != -1)
                {
                    SetPlayerDeathAnimation(self, death);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        LobotomyCorp.SendDeathAnimationSync(self.whoAmI, death);
                    }
                }
            }            
        }

        public bool deathActive = false;
        private LobDeathAnimation deathAnimation;

        private static int GetDeathAnimation(PlayerDeathReason damageSource, Player player)
        {
            int type = -1;
            int? projIndex = damageSource.SourceProjectileLocalIndex;

            Entity ent;
            if (damageSource.TryGetCausingEntity(out ent))
            {
                if (ent is Projectile proj)
                {
                    LobotomyGlobalProjectile lobProj = proj.GetGlobalProjectile<LobotomyGlobalProjectile>();
                    type = lobProj.DeathAnimation;
                }
            }
            if (type == -1)
            {
                type = LobDeathLoader.DeathSpecialCondition(damageSource, player);
            }
            return type;
        }

        public override void UpdateDead()
        {
            if (deathActive)
            {
                deathAnimation.UpTick();
                deathActive = deathAnimation.GetActive;
            }
        }

        public override void PostUpdate()
        {
            if (deathActive)
            {
                deathAnimation.UpTick();
                deathActive = deathAnimation.GetActive;
            }
        }
        
        public void SetPlayerDeathAnimation(Player player, int death)
        {
            LobDeathAnimation deathAnim = LobDeathLoader.deathAnimations[death].Clone();
            deathAnim.Defaults(player.Center, player.Clone() as Player);

            /*self.headPosition = new Vector2(-10000, 10000);
            self.bodyPosition = new Vector2(-10000, 10000);
            self.legPosition = new Vector2(-10000, 10000);
            self.headVelocity *= 0;
            self.bodyVelocity *= 0;
            self.legPosition *= 0;*/
            player.immuneAlpha = 255;

            LobotomyDeathPlayer dPlayer = player.GetModPlayer<LobotomyDeathPlayer>();
            dPlayer.deathActive = true;
            dPlayer.deathAnimation = deathAnim;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                int death = GetDeathAnimation(damageSource, Player);
                if (death != 0)
                {
                    if (genDust)
                        genDust = false;
                }
            }

            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }
    }
}