using LobotomyCorp.Buffs;
using LobotomyCorp.Configs;
using LobotomyCorp.Items.Ruina.Literature;
using LobotomyCorp.Misc;
using LobotomyCorp.Misc.LobSky;
using LobotomyCorp.ModSystems;
using LobotomyCorp.Players;
using LobotomyCorp.Projectiles;
using LobotomyCorp.Tiles;
using LobotomyCorp.UI;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace LobotomyCorp
{
    public enum RiskLevel
    {
        Zayin,
        Teth,
        He,
        Waw,
        Aleph,
        Unknown
    }
    public class LobotomyCorp : Mod
    {
        internal static LobotomyCorp Instance;

        public static Asset<Texture2D> ArcanaSlaveLaser = null;
        public static Asset<Texture2D> ArcanaSlaveLaser2 = null;
        public static Asset<Texture2D> ArcanaSlaveBackground = null;
        public static Asset<Texture2D> ArcanaSlaveBase = null;
        public static Asset<Texture2D> VillainMark = null;

        public static Asset<Texture2D> KingPortal1 = null;
        public static Asset<Texture2D> KingPortal2 = null;
        public static Asset<Texture2D> KingPortal3 = null;

        public static Asset<Texture2D> MagicBulletBullet = null;

        public static Asset<Texture2D> CircleGlow = null;

        public static Asset<Texture2D> RedShoesGlittering = null;

        public static Asset<Texture2D> Slash2 = null;

        public static Color PositivePE => new Color(18, 255, 86);
        public static Color NegativePE => new Color(239, 77, 61);

        public static Color RedDamage => new Color(203, 39, 70);
        public static Color WhiteDamage => new Color(238, 230, 191);
        public static Color BlackDamage => new Color(127, 75, 129);
        public static Color PaleDamage => new Color(65, 244, 188);

        public static Color ZayinRarity => new Color(33, 249, 0);
        public static Color TethRarity => new Color(26, 161, 255);
        public static Color HeRarity => new Color(255, 250, 4);
        public static Color WawRarity => new Color(122, 48, 241);
        public static Color AlephRarity => new Color(255, 1, 0);

        public static Color FragmentShaderColor => new Color(255, 128, 255);

        public static DynamicSpriteFont LobNorwester;

        public static ModKeybind SynchronizeEGO;

        public class WeaponSounds
        {
            public static SoundStyle Axe;
            public static SoundStyle BowGun;
            public static SoundStyle Cannon;
            public static SoundStyle Dagger;
            public static SoundStyle Fist;
            public static SoundStyle Gun;
            public static SoundStyle Hammer;
            public static SoundStyle Mace;
            public static SoundStyle Rapier;
            public static SoundStyle Revolver;
            public static SoundStyle Rifle;
            public static SoundStyle Spear;
        }

        public static Dictionary<string, CustomShaderData> LobcorpShaders = new Dictionary<string, CustomShaderData>();

        public static void LookingAt(Vector2 pos, int type = 30)
        {
            Dust d = Dust.NewDustPerfect(pos, type);
            d.noGravity = true;
            d.velocity *= 0;
        }

        public static void RiskLevelResist(ref int damage, RiskLevel ego, RiskLevel risk)
        {

        }

        public override void Load()
        {
            Instance = this;

            if (!Main.dedServ)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    ArcanaSlaveLaser = Assets.Request<Texture2D>("Projectiles/QueenLaser/Laser", AssetRequestMode.ImmediateLoad);
                    ArcanaSlaveLaser2 = Assets.Request<Texture2D>("Projectiles/QueenLaser/Laser2", AssetRequestMode.ImmediateLoad);
                    ArcanaSlaveBackground = Assets.Request<Texture2D>("Projectiles/QueenLaser/CircleBackground", AssetRequestMode.ImmediateLoad);
                    ArcanaSlaveBase = Assets.Request<Texture2D>("Projectiles/QueenLaser/LaserBase", AssetRequestMode.ImmediateLoad);
                    VillainMark = Assets.Request<Texture2D>("Misc/VillainMark", AssetRequestMode.ImmediateLoad);
                    KingPortal1 = Assets.Request<Texture2D>("Projectiles/KingPortal/KingPortal1", AssetRequestMode.ImmediateLoad);
                    KingPortal2 = Assets.Request<Texture2D>("Projectiles/KingPortal/KingPortal2", AssetRequestMode.ImmediateLoad);
                    KingPortal3 = Assets.Request<Texture2D>("Projectiles/KingPortal/KingPortal3", AssetRequestMode.ImmediateLoad);

                    MiscAssets.LoadStaticAssets(this);

                    MagicBulletBullet = Assets.Request<Texture2D>("Projectiles/MagicBulletBullet");

                    CircleGlow = Assets.Request<Texture2D>("Misc/CircleGlow", AssetRequestMode.ImmediateLoad);

                    RedShoesGlittering = Assets.Request<Texture2D>("Misc/RedShoesGlitter", AssetRequestMode.ImmediateLoad);

                    Slash2 = Assets.Request<Texture2D>("Projectiles/Slash2A", AssetRequestMode.ImmediateLoad);

                    Main.QueueMainThreadAction(() =>
                    {
                        PremultiplyTexture(ArcanaSlaveLaser.Value);
                        PremultiplyTexture(ArcanaSlaveLaser2.Value);
                        PremultiplyTexture(ArcanaSlaveBackground.Value);
                        PremultiplyTexture(ArcanaSlaveBase.Value);
                        PremultiplyTexture(VillainMark.Value);

                        PremultiplyTexture(KingPortal1.Value);
                        PremultiplyTexture(KingPortal2.Value);
                        PremultiplyTexture(KingPortal3.Value);

                        PremultiplyTexture(MagicBulletBullet.Value);

                        PremultiplyTexture(CircleGlow.Value);

                        PremultiplyTexture(RedShoesGlittering.Value);

                        PremultiplyTexture(Slash2.Value);
                    });

                    //Ref<Effect> punishingRef = new Ref<Effect>(GetEffect("Effects/PunishingBird"));
                    Asset<Effect> TrailRef = Assets.Request<Effect>("Effects/SwordTrail", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> GlowTrail = Assets.Request<Effect>("Effects/GlowTrail", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> ArcanaSlaveRef = Assets.Request<Effect>("Effects/ArcanaSlave", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> FourthMatchFlame = Assets.Request<Effect>("Effects/FourthMatchFlame", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> GenericTrail = Assets.Request<Effect>("Effects/GenericTrail", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> BladeTrail = Assets.Request<Effect>("Effects/BladeTrail", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> BrokenScreen = Assets.Request<Effect>("Effects/BrokenShader", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> RedMistEffect = Assets.Request<Effect>("Effects/OverlayShader", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> Fragment = Assets.Request<Effect>("Effects/FragmentUniverse", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> Fragment2 = Assets.Request<Effect>("Effects/FragmentEnlightened", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> Fragment3 = Assets.Request<Effect>("Effects/FragmentScreen", AssetRequestMode.ImmediateLoad);
                    Asset<Effect> SwirleyDist = Assets.Request<Effect>("Effects/SwirlDistortion", AssetRequestMode.ImmediateLoad);
                    //GameShaders.Misc["Punish"] = new MiscShaderData(punishingRef, "PunishingBird");

                    GameShaders.Misc["LobotomyCorp:Rotate"] = new MiscShaderData(ArcanaSlaveRef, "ArcanaResize").UseSaturation(0f);

                    ScreenShaderData shaderData = new ScreenShaderData(BrokenScreen, "BrokenScreenShader");
                    shaderData.UseImage(Assets.Request<Texture2D>("Misc/CameraFilterPack_TV_BrokenGlass5", AssetRequestMode.ImmediateLoad).Value, 0, SamplerState.LinearWrap);
                    Filters.Scene["LobotomyCorp:BrokenScreen"] = new Filter(shaderData, EffectPriority.Medium);

                    shaderData = new ScreenShaderData(SwirleyDist, "SwirlDistortion");
                    Filters.Scene["LobotomyCorp:Swirley"] = new Filter(shaderData, EffectPriority.Medium);
                    Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseColor(0.8f, 0, 0);
                    Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseTargetPosition(Vector2.Zero);
                    Filters.Scene["LobotomyCorp:Swirley"].GetShader().UseProgress(0);

                    shaderData = new ScreenShaderData(Fragment3, "FragmentScreen");
                    shaderData.UseImage(Assets.Request<Texture2D>("Misc/Fragment", AssetRequestMode.ImmediateLoad).Value, 0, SamplerState.LinearWrap);
                    shaderData.UseColor(1f, 128f / 255f, 1f);
                    Filters.Scene["LobotomyCorp:FragmentScreen"] = new Filter(shaderData, EffectPriority.VeryHigh);

                    shaderData = new ScreenShaderData(RedMistEffect, "OverlayRedMist");
                    shaderData.UseImage(Assets.Request<Texture2D>("Misc/Hexagons2", AssetRequestMode.ImmediateLoad).Value, 0, SamplerState.LinearWrap);
                    shaderData.UseImage(Assets.Request<Texture2D>("Misc/Hexagons", AssetRequestMode.ImmediateLoad).Value, 1, SamplerState.LinearWrap);
                    shaderData.UseImage(Assets.Request<Texture2D>("Misc/VignetteA", AssetRequestMode.ImmediateLoad).Value, 2, SamplerState.LinearWrap);
                    shaderData.UseColor(Color.Red);
                    shaderData.UseSecondaryColor(new Color(1, 0.8f, 0.8f));
                    Filters.Scene["LobotomyCorp:RedMistOverlay"] = new Filter(shaderData, EffectPriority.Medium);

                    Asset<Effect> screenRef = Assets.Request<Effect>("Effects/ShockwaveEffect"); // The path to the compiled shader file.
                    shaderData = new ScreenShaderData(screenRef, "Shockwave");
                    Filters.Scene["LobotomyCorp:Shockwave"] = new Filter(shaderData, EffectPriority.VeryHigh);
                    Filters.Scene["LobotomyCorp:Shockwave"].Load();

                    shaderData = new ScreenShaderData(Assets.Request<Effect>("Effects/GrayScaleInvertShader"), "GrayScale");
                    Filters.Scene["LobotomyCorp:GrayscaleShader"] = new Filter(shaderData, EffectPriority.Low);
                    shaderData = new ScreenShaderData(Assets.Request<Effect>("Effects/GrayScaleInvertShader"), "Invert");
                    Filters.Scene["LobotomyCorp:InvertShader"] = new Filter(shaderData, EffectPriority.VeryLow);

                    //Texture2D blankTexture = TextureManager.BlankTexture;

                    //TextureManager.BlankTexture = GetTexture("Projectiles/Help");
                    //GameShaders.Misc["TrailingShader"] = new MiscShaderData(TrailRef, "Trail").UseImage("f");

                    //LobcorpShaders["FairySlash"] = new CustomShaderData(TrailRef, "Trail").UseImage1(this, "Projectiles/Help");
                    //LobcorpShaders["TestSlash"] = new CustomShaderData(TestTrail, "TestTrail").UseImage1(this, "Projectiles/TestSlash");
                    //LobcorpShaders["TwilightSlash"] = new CustomShaderData(TrailRef, "Trail").UseImage1(this, "Projectiles/TwilightTrail");

                    CustomShaderData shader = new CustomShaderData(TrailRef, "Trail").UseImage1(this, "Projectiles/MimicrySBlur2A");
                    shader.UseImage2(this, "Projectiles/MimicrySBlur2");
                    LobcorpShaders["MimicrySlash"] = shader;

                    shader = new CustomShaderData(FourthMatchFlame, "FourthMatch").UseImage1(this, "Projectiles/FourthMatchFlameGigaSlashA");
                    shader.UseImage2(this, "Projectiles/FourthMatchFlameGigaSlashFire");
                    shader.UseImage3(this, "Projectiles/FourthMatchFlameGigaSlashB");
                    LobcorpShaders["FourthMatchFlame"] = shader;

                    shader = new CustomShaderData(TrailRef, "Trail").UseImage1(this, "Projectiles/WingbeatSlashA");
                    shader.UseImage2(this, "Projectiles/WingbeatSlashA");
                    shader.UseImage3(this, "Projectiles/WingbeatSlashB");
                    LobcorpShaders["WingbeatSlash"] = shader;

                    shader = new CustomShaderData(GlowTrail, "GlowTrail").UseImage1(this, "Misc/GenericWindTrail");
                    shader.UseImage2(this, "Misc/GenericWindTrailG");
                    LobcorpShaders["WindTrail"] = shader;

                    shader = new CustomShaderData(GenericTrail, "GenericTrail").UseImage1(this, "Misc/GenTrail");
                    LobcorpShaders["GenericTrail"] = shader;

                    shader = new CustomShaderData(TrailRef, "Trail").UseImage1(this, "Misc/MagicBulletTrailA");
                    shader.UseImage2(this, "Misc/MagicBulletTrailB");
                    shader.UseImage3(this, "Misc/MagicBulletTrailA");
                    LobcorpShaders["MagicBulletTrail"] = shader;

                    shader = new CustomShaderData(TrailRef, "Trail");
                    LobcorpShaders["TextureTrail"] = shader;
                    //TextureManager.BlankTexture = blankTexture;

                    shader = new CustomShaderData(BladeTrail, "BlaidTrail");
                    LobcorpShaders["SwingTrail"] = shader;

                    shader = new CustomShaderData(BladeTrail, "BlaidTrail");
                    shader.UseImage1(this, "Projectiles/Realized/RedEyesSlash");
                    shader.UseImage2(this, "Projectiles/Realized/RedEyesSlashA");
                    shader.UseImage3(this, "Misc/FX_Tex_Noise_Plasma1");
                    LobcorpShaders["RedEyesTrail"] = shader;

                    shader = new CustomShaderData(Fragment, "Fragment");
                    shader.UseImage1(this, "Misc/Fragment");
                    LobcorpShaders["Fragment"] = shader;

                    shader = new CustomShaderData(Fragment2, "FragmentEn");
                    shader.UseImage1(this, "Misc/Fragment");
                    LobcorpShaders["FragmentEnlightened"] = shader;
                    // Use CustomShaderDate to define the length of the border :3

                    // Custom Skies
                    Filters.Scene["LobotomyCorp:BlueStar"] = Filters.Scene["LobotomyCorp:Swirley"];
                    SkyManager.Instance["LobotomyCorp:BlueStar"] = new BlueStarSky();

                    WeaponSounds.Axe = WeaponSound("axe", true, 2);
                    WeaponSounds.BowGun = WeaponSound("bowGun");
                    WeaponSounds.Cannon = WeaponSound("cannon");
                    WeaponSounds.Dagger = WeaponSound("dagger", true, 2);
                    WeaponSounds.Fist = WeaponSound("fist", true, 2);
                    WeaponSounds.Gun = WeaponSound("gun");
                    WeaponSounds.Hammer = WeaponSound("hammer");
                    WeaponSounds.Mace = WeaponSound("mace", true, 2);
                    WeaponSounds.Rapier = WeaponSound("rapier", true, 2);
                    WeaponSounds.Revolver = WeaponSound("revolver");
                    WeaponSounds.Rifle = WeaponSound("rifle");
                    WeaponSounds.Spear = WeaponSound("spear", true, 2);
                }

                PlatformID id = Environment.OSVersion.Platform;
                if (id == PlatformID.Win32NT)
                    LobNorwester = ModContent.Request<DynamicSpriteFont>("LobotomyCorp/Fonts/Norwester", AssetRequestMode.ImmediateLoad).Value;
                else
                    LobNorwester = FontAssets.CombatText[1].Value;
            }
        }

        public override void Unload()
        {
            ArcanaSlaveLaser = null;
            ArcanaSlaveBackground = null;

            KingPortal1 = null;
            KingPortal2 = null;
            KingPortal3 = null;

            MagicBulletBullet = null;

            Slash2 = null;

            /*PositivePE = new Color();
            NegativePE = new Color();
            ZayinRarity = new Color();
            TethRarity = new Color();
            HeRarity = new Color();
            WawRarity = new Color();
            AlephRarity = new Color();*/

            Instance = null;
        }
        /*
        public override void Close()
        {
            var slots = new int[] {
                MusicLoader.GetMusicSlot(this, "LobotomyCorp/Sounds/Music/PMSecondWarning"),
                MusicLoader.GetMusicSlot(this, "LobotomyCorp/Sounds/Music/TilaridsDistortedNight"),
                MusicLoader.GetMusicSlot(this, "LobotomyCorp/Sounds/Music/TilaridsInsigniaDecay")
            };

            foreach (var slot in slots)
            {
                if (((LegacyAudioSystem)Main.audioSystem).AudioTracks.IndexInRange(slot) && Main.audioSystem.IsTrackPlaying(slot) == true)
                {
                    ((LegacyAudioSystem)Main.audioSystem).AudioTracks[slot].Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
                }
            }

            base.Close();
        }*/
        
        public enum modPackets
        {
            SpawnBoss,
            DestroyTileSpecial,
            RedMistGoldTeleport,
            TalkSuppressionTextSync = 6,
            BlessingSync,
            SwordSharpenedVisualImpale,
            DeathAnimSync
        }

        //Mod Packets        
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            //ID
            byte id = reader.ReadByte();
            if (id == 0) // Red Mist Spawn Sync
            {
                int i = reader.ReadInt32();
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int player = reader.ReadInt32();
                NPC.SpawnBoss(x, y, i, 0);
                //ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("RM Spawned at "+x+", "+y), Color.White);
            }
            else if (id == 1) // Kill Shells
            {

            }
            else if (id == 2)// Red Mist 
            {

            }
            else if (id == 3)// LobEventFlagRead
            {
                LobEventFlags flags = ModContent.GetInstance<LobEventFlags>();
                if (flags != null)
                {
                    byte flagid = reader.ReadByte();
                    bool state = reader.ReadBoolean();
                    flags.BinahEntityRecievePacket(flagid, state);
                }
            }
            else if (id == 4)// Supression Text stuff
            {

            }
            else if (id == 5)// Tile Entity update for Binah
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                if (TileUtils.TryGetTileEntityAs(x, y, out BlackBox3TileEntity entity))
                {
                    entity.InitiateText(reader.ReadInt32());
                }
            }
            else if (id == (int)modPackets.TalkSuppressionTextSync)
            {
                byte npc = reader.ReadByte();
                if (npc == 0)
                {
                    bool direction = reader.ReadBoolean();
                    int posx = reader.ReadInt32();
                    int posy = reader.ReadInt32();
                    string text = reader.ReadString();

                    SuppressionText.AddText(text, new Vector2(posx, posy), Main.rand.NextFloat(-0.12f, 0.12f), 0.5f, Color.Red, 0.75f, 30, direction ? -1 : 1, 0);
                }
            }
            else if (id == (int)modPackets.BlessingSync)
            {
                int owner = reader.ReadInt32();
                int reciever = reader.ReadInt32();
                Main.player[reciever].GetModPlayer<LobotomyWawPlayer>().SwordSharpenedBlessingBestower = owner;
                if (Main.netMode == NetmodeID.Server)
                {
                    
                    NetworkBlessingSync(owner, reciever);
                    //ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Blessing data recieved"), Color.White);
                }
            }
            else if (id == (int)modPackets.SwordSharpenedVisualImpale)
            {
                int owner = reader.ReadInt32();
                Vector3 pos = new Vector3(reader.ReadInt16(), reader.ReadInt16(), reader.ReadSingle());
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkSharpenedVisual(pos, owner);
                    //ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Blessing data recieved"), Color.White);
                }
                else
                {
                    LobotomyWawPlayer wawPlayer = Main.player[owner].GetModPlayer<LobotomyWawPlayer>();
                    wawPlayer.SwordSharpenedImpalePosition[wawPlayer.SwordSharpenedImpaledCount] = pos;
                    wawPlayer.SwordSharpenedImpaledCount++;
                    Main.player[owner].AddBuff(ModContent.BuffType<Despair>(), 60);
                    //Main.NewText("Recieved");
                }
            }
            else if (id == (int)modPackets.DeathAnimSync)
            {
                int who = reader.ReadInt32();
                int anim = reader.ReadInt32();
                if (Main.netMode == NetmodeID.Server)
                {
                    SendDeathAnimationSync(who, anim);
                }
                else
                {
                    Player player = Main.player[who];
                    player.GetModPlayer<LobotomyDeathPlayer>().SetPlayerDeathAnimation(player, anim);
                }
            }
        }

        public static void RedMistGoldRushTeleport(int redMist, int portal)
        {

        }

        public static void SendBossSpawnCoords(int bossType, int posX, int posY, int target)
        {
            ModPacket bossSpawn = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            bossSpawn.Write((byte)0);
            bossSpawn.Write(bossType);
            bossSpawn.Write(posX);
            bossSpawn.Write(posY);
            bossSpawn.Write(target);
            bossSpawn.Send(ignoreClient: Main.myPlayer);
        }

        public static void TileEntityBlackBoxInitialize(int posX, int posY, int i)
        {
            ModPacket packet = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            packet.Write((byte)5);
            packet.Write(posX);
            packet.Write(posY);
            packet.Write(i);
        }

        public static void TalkNetUpdate(int id, string talkid, Vector2 position, int direction)
        {
            ModPacket packet = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            packet.Write((byte)modPackets.TalkSuppressionTextSync);
            packet.Write((byte)id);
            packet.Write(direction < 0 ? false : true);
            packet.Write((int)position.X);
            packet.Write((int)position.Y);
            packet.Write(talkid);
            packet.Send();
        }

        public static void SendKillTile(int i)
        {

        }

        public static void NetworkBlessingSync(int owner, int reciever)
        {
            ModPacket packet = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            packet.Write((byte)modPackets.BlessingSync);
            packet.Write(owner);
            packet.Write(reciever);
            packet.Send(ignoreClient: owner);
            //Main.NewText("Blessing Sent");
        }

        public static void NetworkSharpenedVisual(Vector3 pos, int whoAmI)
        {
            ModPacket packet = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            packet.Write((byte)modPackets.SwordSharpenedVisualImpale);
            packet.Write(whoAmI);
            packet.Write((short)pos.X);
            packet.Write((short)pos.Y);
            packet.Write(pos.Z);
            packet.Send(ignoreClient: whoAmI);
            //Main.NewText("Impale Sent");
        }

        public static void SendDeathAnimationSync(int whoAmI, int type)
        {
            ModPacket packet = ModContent.GetInstance<LobotomyCorp>().GetPacket();
            packet.Write((byte)modPackets.DeathAnimSync);
            packet.Write(whoAmI);
            packet.Write(type);
            packet.Send(ignoreClient: whoAmI);
        }

        //General Static stuffs

        public static bool LamentValid(NPC t, Projectile p)
        {
            bool valid = true;
            float health = (float)t.life / (float)t.lifeMax;
            foreach (NPC n in Main.ActiveNPCs)
            {
                float health2 = (float)n.life / (float)n.lifeMax;
                if (!n.dontTakeDamage && !n.friendly && n.life > 0 && n.whoAmI != t.whoAmI && health2 < health && n.chaseable && n.CanBeChasedBy(p) && n.realLife < 0)
                {
                    valid = false;
                    break;
                }
            }
            return valid;
        }

        public static void PremultiplyTexture(Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }

        public static void PremultiplyTexture(Asset<Texture2D> asset)
        {
            Texture2D texture = asset.Value;
            Color[] buffer = new Color[texture.Width * texture.Height];
            asset.Value.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            asset.Value.SetData(buffer);
        }

        public static float Lerp(float x, float x2, float progress, bool reverse = false)
        {
            if (progress < 0)
                progress = 0;
            if (progress > 1f)
                progress = 1f;
            if (reverse)
                return x2 * (1 - progress) + x * progress;
            else
                return x * (1 - progress) + x2 * progress;
        }
    
        public static Vector4 ShaderRotation(float progress)
        {
            float sin = (float)Math.Sin(6.28f * progress);
            float cos = (float)Math.Cos(6.28f * progress);
            return new Vector4(sin, cos, 0f, 0f);
        }
        
        public static SoundStyle WeaponSound(string name, bool PitchVariance = true, int Variance = 1)
        {
            float Volume = 0.1f;
            if (PitchVariance)
            {
                if (Variance > 1)
                    return new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/" + name, Variance) with { Volume = Volume, MaxInstances = 2,PitchVariance = 0.1f };
                else
                    return new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/" + name) with { Volume = Volume, MaxInstances = 2, PitchVariance = 0.1f };
            }
            else
            {
                if (Variance > 1)
                    return new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/" + name, Variance) with { Volume = Volume, MaxInstances = 2 };
                else
                    return new SoundStyle("LobotomyCorp/Sounds/Item/LWeapons/" + name) with { Volume = Volume, MaxInstances = 2 };
            }
        }

        /// <summary>
        /// NameLocation as FolderName/FileName
        /// </summary>
        /// <param name="NameLocation"></param>
        /// <param name="instances"></param>
        /// <param name="Variance"></param>
        /// <param name="Pitch"></param>
        /// <returns></returns>
        public static SoundStyle ItemLobSound(string NameLocation, int instances = 1, int Variance = 1, float Pitch = 0f, float Volume = 1f)
        {
            if (Variance == 1)
            {
                return new SoundStyle("LobotomyCorp/Sounds/Item/" + NameLocation) with { Volume = 0.5f * Volume, MaxInstances = instances, PitchVariance = Pitch };
            }
            else
            {
                return new SoundStyle("LobotomyCorp/Sounds/Item/" + NameLocation, Variance) with { Volume = 0.5f, MaxInstances = instances, PitchVariance = Pitch };
            }
        }

        public static void ScreenShake(int Time, float Intensity, float decay = 0, bool Forced = true)
        {
            ModContent.GetInstance<ScreenSystem>().ScreenShake(Time, Intensity, decay, Forced);
        }
    }

    struct LCMWorleyNoise
    {
        private float _variability;
        private int _columns;
        private int _rows;
        private int _width;
        private int _height;

        public LCMWorleyNoise(float randomSeed, int columns = 5, int rows = 5)
        {
            _variability = randomSeed;
            _columns = columns;
            _rows = rows;
            _width = 100;
            _height = 100;
        }

        public void CreateWorleyNoise(float time = 0)
        {
            Texture2D voronoise;
            Color[] data = new Color[_width * _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float result = GenerateWorley(new Vector2(x / (float)_width, y / (float)_height), _columns, _rows);
                    data[x + (y * 100)] = new Color(result, result, result);
                }
            }
            Main.QueueMainThreadAction(() =>
            {
                voronoise = new Texture2D(Main.graphics.GraphicsDevice, 100, 100);
                voronoise.SetData(data);
                voronoise.SaveAsPng(new FileStream(testFilePath, FileMode.Create), 100, 100);
            });
        }

        private static string cacheFolder = Path.Combine(Main.SavePath, "Mods", "Cache");
        private static string testFileName = "Worley.png";
        private static string testFilePath = Path.Combine(cacheFolder, testFileName);

        private float GenerateWorley(Vector2 uv, int columns, int rows)
        {
            uv.X *= columns;
            uv.Y *= rows;

            Vector2 uvFloor = new Vector2((float)Math.Floor(uv.X), (float)Math.Floor(uv.Y));
            uv -= uvFloor;

            float minimumDist = 1f;

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Vector2 neighbor = new Vector2(x, y);
                    Vector2 Point = random(uvFloor + neighbor);

                    Vector2 diff = neighbor + Point - uv;
                    float dist = diff.Length();
                    minimumDist = (float)Math.Min(minimumDist, dist);
                }
            }

            return minimumDist;
        }

        private Vector2 random(Vector2 uv)
        {
            if (uv.X == 0)
            {
                uv += new Vector2((float)Math.Sin(_variability * 0.2f), (float)Math.Sin(_variability * 0.5f)) * 0.4f;
            }

            float dot = (float)Math.Sin(Vector2.Dot(uv, new Vector2(321.2f * _variability, 21.43f)));
            float fract = dot - (float)Math.Floor(dot);
            dot = (float)Math.Sin(Vector2.Dot(uv, new Vector2(142.7f * _variability * 0.84f, 421.3f * _variability * 0.2f)));
            float fract2 = dot - (float)Math.Floor(dot);
            return new Vector2(fract, fract2);
        }
    }
}