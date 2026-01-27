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
    class LobUISystem : ModSystem
    {
        private UserInterface LobUIInterface;
        internal NihilSelection NihilUI;

        internal static LobUISystem Instance;

        public override void Load()
        {
            Instance = this;

            if (!Main.dedServ)
            {
                LobUIInterface = new UserInterface();

                NihilUI = new NihilSelection();
                NihilUI.Activate();
            }
        }

        public override void Unload()
        {
            LobUIInterface = null;
        }

        public bool UINotInUse()
        {
            return LobUIInterface?.CurrentState == null;
        }

        public void NihilUIActivate()
        {
            NihilUI.ResetTime();
            LobUIInterface?.SetState(NihilUI);
            Main.NewText("Activated");
        }

        public void ClearUI()
        {
            LobUIInterface?.SetState(null);
            Main.NewText("Deactivated");
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (LobUIInterface?.CurrentState != null)
                LobUIInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "LobotomyCorp: LobcorpWeaponUI",
                    delegate
                    {
                        if (LobUIInterface?.CurrentState != null)
                        {
                            LobUIInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
