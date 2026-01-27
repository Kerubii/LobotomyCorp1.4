using LobotomyCorp.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;

namespace LobotomyCorp.ModSystems
{
    [Autoload(Side = ModSide.Client)]
    class LobWorkSystem : ModSystem
    {
        private UserInterface LobworkInterface;
        internal LobWorking LobworkUI;

        private UserInterface NihilInterface;
        internal NihilSelection NihilUI;

        public static Asset<Texture2D> LobotomyUITexture = null;
        public static Asset<Texture2D> LobPanelBorder1 = null;
        public static Asset<Texture2D> LobPanelBorder2 = null;
        public static Asset<Texture2D> LobPanelBorder3 = null;
        public static Asset<Texture2D> LobPanelBorder4 = null;

        public static Asset<Texture2D> LobPanelBackground1 = null;
        public static Asset<Texture2D> LobPanelBackground2 = null;
        public static Asset<Texture2D> LobPanelBackground3 = null;
        public static Asset<Texture2D> LobPanelBackground4 = null;

        public static Texture2D LobotomyUIGetTexture => LobotomyUITexture.Value;

        public static Color LobYellow => new Color(241, 241, 152);
        public static Color LobOrange => new Color(255, 149, 64);

        private static string[] instinctWorkProccess;
        private static string[] insightWorkProccess;
        private static string[] attachmentWorkProccess;
        private static string[] repressionWorkProccess;

        private AbnormalityData currentAbnoDataLoaded;
        public AbnormalityData GetAbnoData => currentAbnoDataLoaded;

        public void LoadWorkUI()
        {
            LobworkUI.StartTest();
            AbnormalityData data = new OneBadManyGood();
            LobworkUI.ApplyData(data, (WorkType)Main.rand.Next(4));
            LobworkInterface?.SetState(LobworkUI);
        }

        public void UnloadWorkUI()
        {
            LobworkUI.CloseTest();
            LobworkInterface?.SetState(null);
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                LobworkInterface = new UserInterface();
                LobotomyUITexture = Mod.Assets.Request<Texture2D>("UI/UITextures/LCUIBig");

                LobPanelBorder1 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBorder1");
                LobPanelBorder2 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBorder2");
                LobPanelBorder3 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBorder3");
                LobPanelBorder4 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBorder4");

                LobPanelBackground1 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBackground1");
                LobPanelBackground2 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBackground2");
                LobPanelBackground3 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBackground3");
                LobPanelBackground4 = Mod.Assets.Request<Texture2D>("UI/UITextures/LCPanelBackground4");

                LobworkUI = new LobWorking();
                LobworkUI.Activate();

                currentAbnoDataLoaded = new OneBadManyGood();

                /*
                instinctWorkProccess = new string[26];
                for (int i = 0; i < 26; i++)
                {
                    string key = "WorkProcess.R." + (i + 1);
                    instinctWorkProccess[i] = AbnormalityData.GetLocalizedFileNonAbno(key);
                }

                insightWorkProccess = new string[27];
                for (int i = 0; i < 27; i++)
                {
                    string key = "WorkProcess.W." + (i + 1);
                    insightWorkProccess[i] = AbnormalityData.GetLocalizedFileNonAbno(key);
                }

                attachmentWorkProccess = new string[25];
                for (int i = 0; i < 25; i++)
                {
                    string key = "WorkProcess.B." + (i + 1);
                    attachmentWorkProccess[i] = AbnormalityData.GetLocalizedFileNonAbno(key);
                }

                repressionWorkProccess = new string[26];
                for (int i = 0; i < 26; i++)
                {
                    string key = "WorkProcess.P." + (i + 1);
                    repressionWorkProccess[i] = AbnormalityData.GetLocalizedFileNonAbno(key);
                }*/
            }
        }

        public static string[] GetWorkProccessText(WorkType type)
        {
            switch (type)
            {
                case WorkType.Instinct:
                    return LoadWorkProcessKey("R", 26);
                case WorkType.Insight:
                    return LoadWorkProcessKey("W", 27);
                case WorkType.Attachment:
                    return LoadWorkProcessKey("B", 25);
                case WorkType.Repression:
                    return LoadWorkProcessKey("P", 26);
                default: return null;
            }
        }

        private static string[] LoadWorkProcessKey(string key, int amount)
        {
            string[] res = new string[amount];
            for (int i = 0; i < amount; i++)
            {
                string valueKey = "WorkProcess." + key + "." + (i + 1);
                res[i] = AbnormalityData.GetLocalizedFileNonAbno(valueKey);
            }
            return res;
        }

        public override void Unload()
        {
            LobworkInterface = null;
        
            instinctWorkProccess = null;
            insightWorkProccess = null;
            attachmentWorkProccess = null;
            repressionWorkProccess = null;
        }

        public override void PostUpdatePlayers()
        {
            /*
            if (Main.LocalPlayer.controlInv)
            {
                LoadWorkUI();
            }*/
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (LobworkInterface?.CurrentState != null)
                LobworkInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LobotomyCorp: WorkUI",
                    delegate
                    {
                        if (LobworkInterface?.CurrentState != null)
                        {
                            LobworkInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public static void LobWorkingInitialize(WorkType workType, AbnormalityData abnoData)
        {
            //currentAbnoDataLoaded = abnoData.LoadWorktype(workType);
        }
    }
}
