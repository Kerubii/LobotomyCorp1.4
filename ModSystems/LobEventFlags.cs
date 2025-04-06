using System.Collections.Generic;
using LobotomyCorp.UI;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

namespace LobotomyCorp.ModSystems
{
    class LobEventFlags : ModSystem
    {
        public static bool downedRedMist = false;
        public static bool binahIntroTalk = false;
        public static bool binahRedmistTalk = false;
        public static bool binahDoneTalk = false;
        public static bool downedAnArbiter = false;
        public static bool killedByRedMist = true;

        public enum FlagIDs
        {
            DownedRedMist,
            BinahIntroTalk,
            BinahRedmistTalk,
            BinahDoneTalk,
            DownedAnArbiter,
            KilledByRedMist
        }

        public override void ClearWorld()
        {
            downedRedMist = false;
            binahIntroTalk = false;
            binahRedmistTalk = false;
            binahDoneTalk = false;
            downedAnArbiter = false;
            killedByRedMist = true;
        }

        
        public override void SaveWorldData(TagCompound tag)
        {
            if (downedRedMist)
            {
                tag["downedRedMist"] = true;
            }

            if (binahIntroTalk)
            {
                tag["binahIntroTalk"] = true;
            }

            if (binahDoneTalk)
            {
                tag["binahDoneTalk"] = true;
            }

            if (downedAnArbiter)
            {
                tag["downedAnArbiter"] = true;
            }
        }

        
        public override void LoadWorldData(TagCompound tag)
        {
            downedRedMist = tag.ContainsKey("downedRedMist");
            binahIntroTalk = tag.ContainsKey("binahIntroTalk");
            binahDoneTalk = tag.ContainsKey("binahDoneTalk");
            downedAnArbiter = tag.ContainsKey("downedAnArbiter");
        }

        public override void NetSend(BinaryWriter writer)
        {
            // Order of operations is important and has to match that of NetReceive
            var flags = new BitsByte();
            flags[0] = downedRedMist;
            flags[1] = downedAnArbiter;
            // flags[1] = downedOtherBoss;
            writer.Write(flags);

        }

        public override void NetReceive(BinaryReader reader)
        {
            // Order of operations is important and has to match that of NetSend
            BitsByte flags = reader.ReadByte();
            downedRedMist = flags[0];
            downedAnArbiter = flags[1];
        }

        public static void debugEventReset()
        {
            downedRedMist = false;
            binahIntroTalk = false;
            binahRedmistTalk = false;
            binahDoneTalk = false;
            downedAnArbiter = false;
        }

        /// <summary>
        /// Used by LobotomyCorp to recieve data sent by BinahEntitySendPacket
        /// </summary>
        /// <param name="flagID"></param>
        /// <param name="flagValue"></param>
        public void BinahEntityRecievePacket(byte flagID, bool flagValue)
        {
            switch(flagID)
            {
                case 0:
                    downedRedMist = flagValue;
                    break;
                case 1:
                    binahIntroTalk = flagValue;
                    break;
                case 2:
                    binahRedmistTalk = flagValue;
                    break;
                case 3:
                    binahDoneTalk = flagValue;
                    break;
                case 4:
                    downedAnArbiter = flagValue;
                    break;
                case 5:
                    killedByRedMist = flagValue;
                    break;

            }
        }

        /// <summary>
        /// Manually sends an update to a mod flag, used mostly by the power of Binah Yaps (Black Box 3)
        /// </summary>
        /// <param name="flagID"></param>
        /// <param name="flagValue"></param>
        public static void BinahEntitySendPacket(FlagIDs flagID, bool flagValue)
        {
            ModPacket bossSpawn = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            bossSpawn.Write((byte)3);
            bossSpawn.Write((byte)flagID);
            bossSpawn.Write(flagValue);
        }
    }
}
