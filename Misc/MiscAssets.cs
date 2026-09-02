//css_ref ../../tModLoader.dll
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using static LobotomyCorp.LobotomyCorp;

namespace LobotomyCorp.Misc
{
    class MiscAssets
    {
        private static string Misc => "Misc/";
        private static string ShaderTextures => "Misc/ShaderTextures/";

        //static Asset<Texture2D> BloodTex = ModContent.Request<Texture2D>("Hell yeah");
        public static Asset<Texture2D> BloodTexture;
        public static Asset<Texture2D> BloodTrail;
        public static Asset<Texture2D> BloodTrailStraight;
        public static Asset<Texture2D> BlueStarShine;
        public static Asset<Texture2D> BrokenGlassVignette;
        public static Asset<Texture2D> BrokenGlass;
        public static Asset<Texture2D> SolemnDingScreen;
        public static Asset<Texture2D> SolemnDongScreen;
        public static Asset<Texture2D> TerrariaExtra201;
        public static Asset<Texture2D> TerrariaExtra209;
        public static Asset<Texture2D> FistTrail;
        public static Asset<Texture2D> FlameTrail;
        public static Asset<Texture2D> FlatColor;
        public static Asset<Texture2D> FragmentBackground;
        public static Asset<Texture2D> PlasmaNoise;
        public static Asset<Texture2D> PlasmaNoiseForArcs;
        public static Asset<Texture2D> TexTrail1;
        public static Asset<Texture2D> TexTrail2;
        public static Asset<Texture2D> WindTrail;
        public static Asset<Texture2D> WindTrailYellow;
        public static Asset<Texture2D> GeneralTrail;
        public static Asset<Texture2D> Gradient;
        public static Asset<Texture2D> KingOfGreedTexture;
        public static Asset<Texture2D> HexagonsColor;
        public static Asset<Texture2D> HexagonsBW;
        public static Asset<Texture2D> HexagonsColorAlt;
        public static Asset<Texture2D> MagicBulletTrailA;
        public static Asset<Texture2D> MagicBulletTrailC;
        public static Asset<Texture2D> NoiseTexture;
        public static Asset<Texture2D> OilTexture;
        public static Asset<Texture2D> PurpleNebula;
        public static Asset<Texture2D> RedShoesGlitter;
        public static Asset<Texture2D> RainbowTexture;
        public static Asset<Texture2D> SheetNote2A;
        public static Asset<Texture2D> SheetNote2A2;
        public static Asset<Texture2D> SheetNote2B;
        public static Asset<Texture2D> SheetNote2B2;
        public static Asset<Texture2D> SheetNote2RGBGlow;
        public static Asset<Texture2D> SheetNote2WA;
        public static Asset<Texture2D> SheetNote2WAGlow;
        public static Asset<Texture2D> SheetNoteA;
        public static Asset<Texture2D> SheetNoteRGB;
        public static Asset<Texture2D> StarColor;
        public static Asset<Texture2D> TrailSmoke;
        public static Asset<Texture2D> TrailPierce;
        public static Asset<Texture2D> TrailSpear;
        public static Asset<Texture2D> TrailSpear2;
        public static Asset<Texture2D> Vignette;
        public static Asset<Texture2D> VignetteAlpha;
        public static Asset<Texture2D> VillainMark;
        public static Asset<Texture2D> Worley;



        public static Asset<Texture2D> GreedNihilTexture;
        public static Asset<Texture2D> PenitenceGradient;
        public static Asset<Texture2D> RemorseBright;
        public static Asset<Texture2D> RemorseBrown;
        public static Asset<Texture2D> RedEyesSlash;
        public static Asset<Texture2D> RedEyesSlashA;
        public static Asset<Texture2D> SodaTexture;

        public static void LoadStaticAssets(Mod mod)
        {
            BloodTexture = mod.Assets.Request<Texture2D>(Misc + "BloodTexture");
            BloodTrail = mod.Assets.Request<Texture2D>(Misc + "BloodTrail");
            BloodTrailStraight = mod.Assets.Request<Texture2D>(Misc + "BloodTrail2");
            BlueStarShine = mod.Assets.Request<Texture2D>(Misc + "BlueStarShine");
            BrokenGlassVignette = mod.Assets.Request<Texture2D>(Misc + "CameraFilterPack_TV_BrokenGlass4");
            BrokenGlass = mod.Assets.Request<Texture2D>(Misc + "CameraFilterPack_TV_BrokenGlass5");
            SolemnDingScreen = mod.Assets.Request<Texture2D>(Misc + "Ding");
            SolemnDongScreen = mod.Assets.Request<Texture2D>(Misc + "Dong");
            TerrariaExtra201 = mod.Assets.Request<Texture2D>(Misc + "Extra_201");
            TerrariaExtra209 = mod.Assets.Request<Texture2D>(Misc + "Extra_209");
            FistTrail = mod.Assets.Request<Texture2D>(Misc + "FistTrail");
            FlameTrail = mod.Assets.Request<Texture2D>(Misc + "flametrail");
            FlatColor = mod.Assets.Request<Texture2D>(Misc + "FlatColor");
            FragmentBackground = mod.Assets.Request<Texture2D>(Misc + "Fragment");
            PlasmaNoise = mod.Assets.Request<Texture2D>(Misc + "FX_Tex_Noise_Plasma1");
            PlasmaNoiseForArcs = mod.Assets.Request<Texture2D>(Misc + "FX_Tex_Noise_Plasma2");
            TexTrail1 = mod.Assets.Request<Texture2D>(Misc + "FX_Tex_Trail1");
            TexTrail2 = mod.Assets.Request<Texture2D>(Misc + "FX_Tex_Trail2");
            WindTrail = mod.Assets.Request<Texture2D>(Misc + "GenericWindTrail");
            WindTrailYellow = mod.Assets.Request<Texture2D>(Misc + "GenericWindTrailG");
            GeneralTrail = mod.Assets.Request<Texture2D>(Misc + "GenTrail");
            Gradient = mod.Assets.Request<Texture2D>(Misc + "gradient");
            KingOfGreedTexture = mod.Assets.Request<Texture2D>(Misc + "GreedSnippetTexture");
            HexagonsColor = mod.Assets.Request<Texture2D>(Misc + "Hexagons");
            HexagonsBW = mod.Assets.Request<Texture2D>(Misc + "Hexagons2");
            HexagonsColorAlt = mod.Assets.Request<Texture2D>(Misc + "HexagonsAlt");
            MagicBulletTrailA = mod.Assets.Request<Texture2D>(Misc + "MagicBulletTrailA");
            MagicBulletTrailC = mod.Assets.Request<Texture2D>(Misc + "MagicBulletTrailB");
            NoiseTexture = mod.Assets.Request<Texture2D>(Misc + "Noise4");
            OilTexture = mod.Assets.Request<Texture2D>(Misc + "oil");
            PurpleNebula = mod.Assets.Request<Texture2D>(Misc + "PurpleNebula5");
            RedShoesGlitter = mod.Assets.Request<Texture2D>(Misc + "RedShoesGlitter");
            RainbowTexture = mod.Assets.Request<Texture2D>(Misc + "Seamless_Rainbow");
            SheetNote2A = mod.Assets.Request<Texture2D>(Misc + "SheetNote2A");
            SheetNote2A2 = mod.Assets.Request<Texture2D>(Misc + "SheetNote2A2");
            SheetNote2B = mod.Assets.Request<Texture2D>(Misc + "SheetNote2B");
            SheetNote2B2 = mod.Assets.Request<Texture2D>(Misc + "SheetNote2B2");
            SheetNote2RGBGlow = mod.Assets.Request<Texture2D>(Misc + "SheetNote2RGBGlow");
            SheetNote2WA = mod.Assets.Request<Texture2D>(Misc + "SheetNote2WA");
            SheetNote2WAGlow = mod.Assets.Request<Texture2D>(Misc + "SheetNote2WAGlow");
            SheetNoteA = mod.Assets.Request<Texture2D>(Misc + "SheetNoteA");
            SheetNoteRGB = mod.Assets.Request<Texture2D>(Misc + "SheetNoteRGB");
            StarColor = mod.Assets.Request<Texture2D>(Misc + "StarColor");
            TrailSmoke = mod.Assets.Request<Texture2D>(Misc + "Trail52");
            TrailPierce = mod.Assets.Request<Texture2D>(Misc + "Trail70");
            TrailSpear = mod.Assets.Request<Texture2D>(Misc + "Trail71");
            TrailSpear2 = mod.Assets.Request<Texture2D>(Misc + "Trail713");
            Vignette = mod.Assets.Request<Texture2D>(Misc + "Vignette");
            VignetteAlpha = mod.Assets.Request<Texture2D>(Misc + "VignetteA");
            VillainMark = mod.Assets.Request<Texture2D>(Misc + "VillainMark");
            Worley = mod.Assets.Request<Texture2D>(Misc + "Worley");

            GreedNihilTexture = mod.Assets.Request<Texture2D>(ShaderTextures + "GreedSnippetTextureNihil");
            PenitenceGradient = mod.Assets.Request<Texture2D>(ShaderTextures + "PenitenceGradient");
            RemorseBright = mod.Assets.Request<Texture2D>(ShaderTextures + "RemorseBright");
            RemorseBrown = mod.Assets.Request<Texture2D>(ShaderTextures + "RemorseBrown");
            RedEyesSlash = mod.Assets.Request<Texture2D>(ShaderTextures + "RedEyesSlash");
            RedEyesSlashA = mod.Assets.Request<Texture2D>(ShaderTextures + "RedEyesSlashA");
            SodaTexture = mod.Assets.Request<Texture2D>(ShaderTextures + "SodaTexture");
        }

        public static void PreMultiplyTextures()
        {

        }
    }
}