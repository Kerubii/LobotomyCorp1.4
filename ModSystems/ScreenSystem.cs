using LobotomyCorp.Configs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace LobotomyCorp.ModSystems
{
    class ScreenSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["LobotomyCorp:RedMistOverlay"].IsActive() && !NPC.AnyNPCs(ModContent.NPCType<NPCs.RedMist.RedMist>()))
            {
                Filters.Scene["LobotomyCorp:RedMistOverlay"].Deactivate();
            }
            if (Main.netMode != NetmodeID.Server)
            {
                if (FragmentShader)
                {
                    
                    Filters.Scene["LobotomyCorp:FragmentScreen"].GetShader().UseIntensity(0.0001f * (float)Main.timeForVisualEffects);
                    if (FragmentShaderTime > 0)
                        FragmentShaderTime--;
                    else
                        FragmentShader = false;
                }
                else
                {
                    Filters.Scene["LobotomyCorp:FragmentScreen"].Opacity = 0;
                    Filters.Scene["LobotomyCorp:FragmentScreen"].Deactivate();
                }
            }
            
            base.PostUpdateEverything();
        }

        public void FragmentScreenActivate()
        {
            if (!Filters.Scene["LobotomyCorp:FragmentScreen"].IsActive())
            {
                Filters.Scene.Activate("LobotomyCorp:FragmentScreen");
                Filters.Scene["LobotomyCorp:FragmentScreen"].Opacity = 0;
            }
            FragmentShader = true;
            FragmentShaderTime = 60;
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup rec = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + "Evil Powder", new[]
            {
                (int)ItemID.ViciousPowder,
                (int)ItemID.VilePowder
            });
            RecipeGroup.RegisterGroup("LobotomyCorp:EvilPowder", rec);

            rec = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + "Butterflies", new[]
            {
                (int)ItemID.JuliaButterfly,
                ItemID.MonarchButterfly,
                ItemID.PurpleEmperorButterfly,
                ItemID.RedAdmiralButterfly,
                ItemID.SulphurButterfly,
                ItemID.TreeNymphButterfly,
                ItemID.UlyssesButterfly,
                ItemID.ZebraSwallowtailButterfly,
                ItemID.GoldButterfly,
                ItemID.HellButterfly
            });
            RecipeGroup.RegisterGroup("LobotomyCorp:Butterflies", rec);

            rec = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + "Dungeon Lanterns", new[]
            {
                (int)ItemID.ChainLantern,
                ItemID.BrassLantern,
                ItemID.CagedLantern,
                ItemID.CarriageLantern,
                ItemID.AlchemyLantern,
                ItemID.DiablostLamp,
                ItemID.OilRagSconse
            });
            RecipeGroup.RegisterGroup("LobotomyCorp:DungeonLantern", rec);

            rec = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + "Boss Masks", new[]
           {
                (int)ItemID.KingSlimeMask,
                (int)2112,
                2103,
                2111,
                2108,
                1281,
                5109,
                2105,
                4959,
                2113,
                2106,
                2107,
                2109,
                2110,
                4784,
                2588,
                3372,
                3373,
                3864,
                3865,
                3863
            });
            RecipeGroup.RegisterGroup("LobotomyCorp:BossMasks", rec);

            rec = new RecipeGroup(() => "Either Soul of Light or Night", new[]
            {
                (int)ItemID.SoulofLight,
                ItemID.SoulofNight
            });
            RecipeGroup.RegisterGroup("LobotomyCorp:DualSoul", rec);
        }

        public override void ModifyScreenPosition()
        {
            if (modifiedCamera)
            {
                Main.screenPosition = modifiedScreenPosition;
                modifiedCamera = false;
            }

            if (ScreenShakeTimer > 0)
            {
                Main.screenPosition.X += Main.rand.NextFloat(-ScreenShakeIntensity, ScreenShakeIntensity);
                Main.screenPosition.Y += Main.rand.NextFloat(-ScreenShakeIntensity, ScreenShakeIntensity);
                ScreenShakeIntensity -= ScreenShakeDecay;
                ScreenShakeTimer--;
            }
            base.ModifyScreenPosition();
        }

        private int ScreenShakeTimer;
        private float ScreenShakeIntensity;
        private float ScreenShakeDecay;

        /// <summary>
        /// Local Version, Screenshakes everyone's screen unless Main.myPlayer == whoAmI
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Intensity"></param>
        /// <param name="decay"></param>
        /// <param name="Forced"></param>
        public void ScreenShake(int Time, float Intensity, float decay = 0, bool Forced = true)
        {
            if (!ModContent.GetInstance<LobotomyConfig>().ScreenShakeEnabled || (!Forced && ScreenShakeTimer > 0))
                return;

            ScreenShakeTimer = Time;
            ScreenShakeIntensity = Intensity;
            ScreenShakeDecay = decay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="player"></param>
        /// <param name="Time"></param>
        /// <param name="Intensity"></param>
        /// <param name="decay"></param>
        /// <param name="falloff"></param>
        /// <param name="Forced"></param>
        public void ScreenShakeGlobal( Vector2 origin, Player player, int Time, float Intensity, float decay = 0, bool Forced = true)
        {

        }

        private bool modifiedCamera;
        private Vector2 modifiedScreenPosition;

        public Vector2 RedEyesSpecialCamera(Vector2 cameraCenter, Vector2? lerpTo = null, float lerp = 0f)
        {
            modifiedCamera = true;
            Vector2 position = cameraCenter + Vector2.UnitY * (Main.player[Main.myPlayer].gfxOffY - 1);
            if (lerpTo != null)
            {
                position = Vector2.Lerp(position, (Vector2)lerpTo, lerp);
            }
            modifiedScreenPosition.X = position.X - Main.screenWidth / 2;
            modifiedScreenPosition.Y = position.Y - Main.screenHeight / 2;
            Main.SetCameraLerp(0.1f, 15);
            return position;
        }

        private float impactZoom = 0;
        private int impactTime = 0;

        public readonly int ZOOMTYPE_QUICK = 0;
        public readonly int ZOOMTYPE_SLOW = 0;

        /// <summary>
        /// Zoom is automatically calculated, Type
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Intensity"></param>
        /// <param name="targetPlayer"></param>
        /// <param name="Forced"></param>
        public void ImpactZoom(int Time, float Intensity, int Type, int TargetPlayer = -1, bool Forced = true)
        {

        }

        public bool FragmentShader = false;
        private int FragmentShaderTime = 0;
    }
}
