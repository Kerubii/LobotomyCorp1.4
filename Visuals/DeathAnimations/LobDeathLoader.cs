using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace LobotomyCorp.Visuals.DeathAnimations
{
    public class LobDeathLoader : ILoadable
    {
        internal static readonly List<LobDeathAnimation> deathAnimations = [];
        internal static readonly List<Asset<Texture2D>> deathTexture = [];
        public static IReadOnlyList<LobDeathAnimation> DeathAnimations => deathAnimations;
        public static IReadOnlyList<Asset<Texture2D>> DeathTexture => deathTexture;

        public void Load(Mod mod)
        {
        }

        public void Unload()
        {
        }

        internal static int Add(LobDeathAnimation deathAnim)
        {
            int type = deathAnimations.Count;
            deathAnimations.Add(deathAnim);
            return type;
        }

        internal static void AddTex(Asset<Texture2D> tex)
        {
            deathTexture.Add(tex);
        }

        /// <summary>
        /// Returns DeathAnimation Type based on special conditions denoted by DeathReason and Player
        /// </summary>
        /// <param name="reason"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static int DeathSpecialCondition(PlayerDeathReason reason, Player player)
        {
            for (int i = 0; i < deathAnimations.Count; i++)
            {
                if (deathAnimations[i].SpecialDeathCondition(reason, player))
                    return i;
            }
            return -1;
        }
    }
}