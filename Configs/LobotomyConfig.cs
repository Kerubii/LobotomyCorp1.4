using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace LobotomyCorp.Configs
{
    public class LobotomyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("GeneralConfiguration")]
        //[Label("Expanded Realized EGO Tooltips")]
        [DefaultValue(true)]
        public bool ExtraPassivesShow;

        [Header("VisualEffects")]

        //[Label("Screenshake Enabled")]
        [DefaultValue(true)]
        public bool ScreenShakeEnabled;

        [DefaultValue(true)]
        public bool BloomEnable;

        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float ScreenEffectOpacity;

        [Header("Sound")]

        [DefaultValue(0f)]
        [Range(0f, 1f)]
        public float SolemnDingDongChance;

        /*
        [DefaultValue(false)]
        public bool ExtractorClass
        */
    }

    public class LobotomyServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //[Label("Test Items")]
        //[Tooltip("Include unfinished items and NPCs. Requires a Reload")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool TestItemEnable { get; set; }
    }
}