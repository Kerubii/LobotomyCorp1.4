using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LobotomyCorp.Utils;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using System.Security.Cryptography.Pkcs;
using LobotomyCorp.Players;
using Microsoft.Build.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using Terraria.Graphics;
using LobotomyCorp.Buffs;
using System.Threading;
using System.Security.Policy;
using Terraria.Audio;
using rail;
using ReLogic.Content;

namespace LobotomyCorp.Projectiles.Realized
{
	public class SwordSharpenedWithTearsRSword : ModProjectile
	{
        public static Asset<Texture2D> Glow;

        public override void Load()
        {
            Glow = ModContent.Request<Texture2D>(Texture + "Glow");
        }

        public override void SetStaticDefaults() {
            //DisplayName.SetDefault("Spear");
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.scale = 1f;
			Projectile.alpha = 0;
            Projectile.timeLeft = 10000;

			//Projectile.hide = true;
			//Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
            Projectile.extraUpdates = 3;
            Projectile.minion = true;
            Projectile.minionSlots = 1; 

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
		}

        private float state
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private float timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private NPC currentTarget
        {
            get => Main.npc[(int)Projectile.ai[2]];
        }

        private int setTarget
        {
            set => Projectile.ai[2] = value;
        }

        private float trailOpacity
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        private float extraUpdateMult { get => Projectile.extraUpdates + 1; }


        public override void AI() {
            Player owner = Main.player[Projectile.owner];
            if (!checkActive(owner))
            {
                return;
            }
            starParticleUpdate();

            LobotomyWawPlayer modOwner = owner.GetModPlayer<LobotomyWawPlayer>();
            // Idle State
            if (state <= 0)
            {
                float order = getOrder();
                float total = LobotomyWawPlayer.SwordSharpenedTotalOwned(owner) - 1;
                if (total <= 0)
                {
                    total = 1;
                    order = 0.5f;
                }
                // dir dictates the position of the idle state
                float dir = (order / total);
                if (owner.direction == 1)
                    dir = 1f - dir;

                // Position Offset
                Projectile.localAI[0]++;
                // Decrease Swordtrail Opacity;
                if (trailOpacity > 0)
                {
                    trailOpacity -= 0.005f;
                    if (trailOpacity < 0)
                        trailOpacity = 0;
                }

                bool move = true;
                bool rotation = true;
                Vector2 targetPos = owner.MountedCenter + new Vector2(-60 + 120 * dir, -10 - 42 * (float)Math.Sin(3.14f * dir));
                targetPos.Y += 2 * (float)Math.Sin(6.28f * Projectile.localAI[0] / (120 * extraUpdateMult));

                int current = -1;
                for (int i = 0; i < 3; i++)
                {
                    if (modOwner.SwordSharpenedCurrentSword[i] == order)
                    {
                        current = i;
                        break;
                    }
                }

                bool autoTarget = owner.ownedProjectileCounts[ModContent.ProjectileType<SwordSharpenedWithTearsRSword>()] > 0;
                // If the player has "Justice" and has the sword queued, use Manual aiming
                if (modOwner.SwordSharpenedJustice)
                {
                    blessed = true;
                    targetPos.X += -30 + 60 * dir;
                    targetPos.Y -= 32;
                    if (current > -1 && Main.myPlayer == Projectile.owner)
                    {
                        autoTarget = false;
                        rotation = false;
                        float rot = owner.MountedCenter.AngleTo(Main.MouseWorld);
                        Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, rot, 0.3f / extraUpdateMult);
                        targetPos = owner.MountedCenter + new Vector2(60, 0).RotatedBy(rot + MathHelper.ToRadians(120 * current));

                        if (owner.channel)
                        {
                            if (owner.altFunctionUse == 2)
                            {
                                if (state == 0)
                                {
                                    state = -2;
                                    timer = 30;
                                }
                            }
                            else if (current == 0)
                            {

                                if (state == 0)
                                {
                                    timer = 0;
                                    state = -1;
                                }
                                timer++;

                                if (Projectile.scale < 1.2f)
                                {
                                    Projectile.scale += 0.02f;
                                }
                            }
                            owner.itemTime = owner.itemAnimation = owner.itemAnimationMax;
                        }
                        else if (owner.itemAnimation > 0 && timer > 0 && state < 0)
                        {
                            Projectile.scale = 1f;
                            float speed = 8f;
                            
                            if (state == -2)
                            {
                                state = 8;
                                Projectile.rotation += MathHelper.ToRadians(360 * 4);
                                SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Gaho") with { Volume = 0.2f, MaxInstances = 0 }, Projectile.Center);
                                speed = 0.5f;
                                rot = owner.MountedCenter.AngleTo(Projectile.Center);
                            }
                            else
                            {
                                state = 5;
                                if (timer > 30 * extraUpdateMult)
                                {
                                    state = 12;
                                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Atk_Strong") with { Volume = 0.2f }, Projectile.Center);
                                    speed = 16f;
                                }
                                else
                                {
                                    string sound = "LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Hori_gaho";
                                    if (Main.rand.NextBool(2))
                                        sound = "LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Stab_gaho";
                                    SoundEngine.PlaySound(new SoundStyle(sound) with { Volume = 0.2f, MaxInstances = 0 }, Projectile.Center);
                                }
                            }
                            Projectile.velocity = new Vector2(speed, 0).RotatedBy(rot);
                            Projectile.rotation = rot;
                            timer = 30 * extraUpdateMult;
                            trailOpacity = 1;
                            move = false;
                            modOwner.SwordSharpenedFindNextValidSword();
                            Projectile.netUpdate = true;
                        }
                    }
                }
                // Auto target an enemy
                if (autoTarget)
                {
                    blessed = modOwner.SwordSharpenedDespair;
                    if (timer < 0 && Main.myPlayer == Projectile.owner)
                    {
                        int target = -1;
                        int range = 1000;
                        Projectile.Minion_FindTargetInRange(range, ref target, false);
                        if (target > -1)
                        {
                            setTarget = target;
                            int delay = (int)(Main.rand.Next(110, 201) * (1f / total));
                            // Tell other swords with no targets to dont attack
                            foreach (Projectile p in Main.projectile)
                            {
                                if (p.active && LobotomyWawPlayer.SwordSharpenedIsType(p.type) && p.whoAmI != Projectile.whoAmI && p.owner == Projectile.owner && p.ai[1] <= 0 && p.ai[0] <= 5)
                                {
                                    p.ai[0] = delay;
                                    p.netUpdate = true;
                                }
                            }
                            Vector2 delta2 = Vector2.Normalize(Projectile.Center - owner.MountedCenter) * 8;
                            Projectile.velocity = delta2 / extraUpdateMult;
                            Projectile.netUpdate = true;
                            Projectile.rotation = Projectile.velocity.ToRotation();
                            rotation = false;
                            move = false;
                            state++;
                            timer = 15 * extraUpdateMult;
                            trailOpacity = 1;
                            string sound = "LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Hori";
                            if (Main.rand.NextBool(2))
                                sound = "LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Stab";
                            if (modOwner.SwordSharpenedDespair)
                                sound += "_gaho";
                            SoundEngine.PlaySound(new SoundStyle(sound) with { Volume = 0.2f, MaxInstances = 0 }, Projectile.Center);
                        }
                    }
                    timer--;
                }            
                if (move)
                {
                    Vector2 delta = targetPos - Projectile.Center;
                    float speed = 14f / extraUpdateMult;
                    float lerpValue = Terraria.Utils.GetLerpValue(200f, 600f, delta.Length(), true);
                    if (delta.Length() >= 3000f)
                        Projectile.Center = targetPos;
                    Projectile.velocity = delta;
                    if (delta.Length() > speed)
                    {
                        Projectile.velocity *= speed / Projectile.velocity.Length();
                    }
                    float targetAngle = MathHelper.WrapAngle(owner.direction > 0 ? 0.349066f : 2.79253f);
                    if (rotation)
                        Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, targetAngle, 0.3f / extraUpdateMult);
                }
            }
            else
            {
                // Stabbing Delay
                if (state == 1)
                {
                    Vector2 delta = currentTarget.Center - Projectile.Center;
                    float targetRot = delta.ToRotation();
                    //Projectile.rotation = (float)Terraria.Utils.Lerp(Projectile.rotation, targetRot, 0.4f);
                    Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, targetRot, 0.3f / extraUpdateMult);
                    Projectile.velocity *= 0.993f;
                    timer--;
                    if (timer <= 0)
                    {
                        delta.Normalize();
                        Projectile.rotation = targetRot;
                        Projectile.velocity = 16 * delta;
                        state++;
                        timer = 30 * extraUpdateMult;
                    }
                }
                // Stab Motion
                else if (state == 2 || state == 5 || state == 9 || state == 12 || state == 14)
                {
                    Vector2 offset = new Vector2(45, 0).RotatedBy(Projectile.rotation);
                    if (Collision.SolidCollision(Projectile.position + offset, Projectile.width, Projectile.height))
                    {
                        timer = 0;
                        Projectile.velocity *= 0;
                    }

                    timer--;
                    if (timer <= 0)
                    {
                        if (state == 14)
                            state = 3;
                        else
                            state++;
                        if (timer == 0)
                            timer = 60 * extraUpdateMult;
                        else
                            timer = 0;
                        Projectile.velocity *= 0.4f / extraUpdateMult;
                    }
                }
                // Unstab Motion
                else if (state == 3)
                {
                    Projectile.velocity *= 0.988f;
                    timer++;
                    float unstuck = 10;
                    float downtime = 25;
                    if (timer > (unstuck + downtime) * extraUpdateMult)
                    {
                        if (modOwner.SwordSharpenedCurrentSword.Contains(-1))
                        {
                            int order = getOrder();
                            modOwner.SwordSharpenedAddSwordToQueue(order);
                        }
                        state = 0;
                        blessed = false;
                    }
                    else if (timer > unstuck * extraUpdateMult)
                    {
                        Projectile.rotation += MathHelper.ToRadians(downtime * (1f - (timer - unstuck * extraUpdateMult) / (downtime * extraUpdateMult))) / extraUpdateMult * Projectile.spriteDirection;
                    }
                    else if (timer == unstuck * extraUpdateMult)
                    {
                        Projectile.spriteDirection = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 delta = owner.Center - Projectile.Center;
                        delta.Normalize();
                        Projectile.velocity = (delta * 18 / extraUpdateMult).RotatedByRandom(MathHelper.ToRadians(30));
                        SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Gaho") with { Volume = 0.2f, PitchVariance = 0.2f, MaxInstances = 0}, Projectile.Center);
                    }
                }
                // Spawn Despair sword when missing
                else if (state == 6 || state == 10 || state == 13)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int i = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SwordSharpenedWithTearsRDespair>(), 15, 0, Projectile.owner);
                        Main.projectile[i].rotation = Projectile.rotation;
                    }

                    Projectile.Center = owner.Center;
                    Projectile.velocity = Vector2.Zero;
                    if (modOwner.SwordSharpenedCurrentSword.Contains(-1))
                    {
                        int order = getOrder();
                        modOwner.SwordSharpenedAddSwordToQueue(order);
                    }
                    state = 0;
                    blessed = false;
                    timer = 0;
                    setTarget = -1;
                    trailOpacity = 0f;
                }
                // Alternate Attack Spread
                else if (state == 8)
                {
                    float rot = 0;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        rot = Projectile.Center.AngleTo(Main.MouseWorld);
                        Projectile.rotation = (float)Terraria.Utils.Lerp(Projectile.rotation, rot, 0.3f / extraUpdateMult);
                    }
                    timer--;
                    if (timer <= 0)
                    {
                        state = 5;
                        timer = 30 * extraUpdateMult;
                        Projectile.velocity = new Vector2(16f, 0).RotatedBy(rot);
                        Projectile.rotation = rot;
                    }
                }
                // Unused, doesn't look that good, set state to 9 to activate
                // Home back into enemy
                else if (state == 11)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    timer--;
                    float spacing = 16 * 3 + currentTarget.width;
                    if (!currentTarget.active)
                    {
                        int newTar = -1;
                        Projectile.Minion_FindTargetInRange(1000, ref newTar, true);
                        if (newTar == -1)
                        {
                            state = 0;
                            timer = 0;
                            blessed = false;
                        }
                        else
                        {
                            setTarget = newTar;
                            timer = 0;
                        }
                    }
                    else
                    {
                        AIHelper.ChaseTargetDirectAccel(Projectile.Center, currentTarget.Center, ref Projectile.velocity, 16f, .8f, spacing);
                    }
                    if (timer <= 0)
                    {
                        Vector2 delta2 = currentTarget.Center.DirectionTo(Projectile.Center) * 8;
                        Projectile.velocity = delta2;
                        state = 1;
                        timer = 15 * extraUpdateMult;
                        trailOpacity = 1;
                        string sound = "LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Hori";
                        if (Main.rand.NextBool(2))
                            sound = "LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Stab";
                        if (modOwner.SwordSharpenedDespair)
                            sound += "_gaho";
                        SoundEngine.PlaySound(new SoundStyle(sound) with { Volume = 0.2f, MaxInstances = 0 }, Projectile.Center);
                    }
                }
            }
		}

        private bool checkActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<SwordSharpened>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<SwordSharpened>()))
            {
                Projectile.timeLeft = 2;
            }
            return true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (state == 2 || state == 5 || state == 9 || (state == 11 && target.whoAmI == Projectile.ai[2]) || state == 12 || state == 14)
                return base.CanHitNPC(target);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (state == 2)
            {
                Projectile.velocity *= 0;
                timer = 0;
                state++;
                Projectile.netUpdate = true;
            }
            else if (state == 5)
            {
                state = 2;
                Projectile.netUpdate = true;
            }
            else if (state == 9)
            {
                state = 11;
                timer = 60 * 3 * extraUpdateMult;
                setTarget = target.whoAmI;
                Projectile.netUpdate = true;
            }
            else if (state == 12)
            {
                state = 14;
                if (timer > 10 * extraUpdateMult)
                    timer = 10 * extraUpdateMult;
                Projectile.netUpdate = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (state == 5)
            {
                modifiers.SourceDamage += 0.6f;
            }
            else if (state == 12 || state == 14)
                modifiers.SourceDamage += 1.2f;
            if (blessed)
                modifiers.CritDamage.Flat += target.defDefense;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 offset = new Vector2(45, 0).RotatedBy(Projectile.rotation);
            hitbox.X += (int)offset.X;
            hitbox.Y += (int)offset.Y;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (trailOpacity > 0)
                DrawTrail(Projectile, trailOpacity, new Color(103, 187, 242));

            Vector2 position = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + 2.35619f;
            position.Y += Projectile.gfxOffY;

            Texture2D tex = Glow.Value;
            Color color = Color.White;
            color.A = 180;
            color *= 0.2f;
            for (int i = 0; i < 8; i++)
            {
                float prog = (float)(Main.timeForVisualEffects % 90 / 90);
                Vector2 offset = new Vector2(3 + 1 * (float)Math.Sin(6.28f * prog), 0).RotatedBy(0.785f * i);
                float scale = 1f + 0.1f * (float)Math.Sin(3.14f * prog);
                Main.EntitySpriteDraw(tex, position + offset, null, color * Projectile.Opacity, rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            if (trailOpacity > 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    if (i == 0)
                        continue;
                    float opacity = (1f - i / 120f) * trailOpacity;
                    Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                    Main.EntitySpriteDraw(tex, oldPos, null, color * 0.1f * opacity, Projectile.oldRot[i] + 2.35619f, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }

            tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, position, null, lightColor * Projectile.Opacity, rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        KnightOfDespairStars[] starsParticle = new KnightOfDespairStars[30];
        private int pairTracker = -1;
        private int starsSpawnTimer = 0;
        private bool blessed = false;

        private void starParticleUpdate()
        {
            if (blessed)
                starsSpawnTimer++;

            if (starsSpawnTimer >= 60)
            {
                for (int i = 0; i < starsParticle.Length; i++)
                {
                    if (starsParticle[i] == null || !starsParticle[i].Active)
                    {
                        Vector2 pos = Projectile.position + (Vector2.UnitX * Main.rand.Next(-45, 45)).RotatedBy(Projectile.rotation);
                        Vector2 vel = new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f));
                        starsParticle[i] = KnightOfDespairStars.Create(pos, Projectile.width, Projectile.height, pairTracker, vel);
                        pairTracker = i;
                        break;
                    }
                }
                starsSpawnTimer = 0;
            }
            for (int i = 0; i < starsParticle.Length; i++)
            {
                if (starsParticle[i] != null && starsParticle[i].Active)
                {
                    starsParticle[i].Update(0.04f/4f);
                    if (starsParticle[i].pair >= 0 && !starsParticle[starsParticle[i].pair].Active)
                    {
                        starsParticle[i].pair = -1;
                    }
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            for (int i = 0; i < starsParticle.Length; i++)
            {
                if (starsParticle[i] != null && starsParticle[i].Active)
                {
                    if (starsParticle[i].pair < 0)
                        starsParticle[i].Draw(Main.spriteBatch, null);
                    else
                        starsParticle[i].Draw(Main.spriteBatch, starsParticle[starsParticle[i].pair]);
                }
            }
        }

        public static void DrawTrail(Projectile Projectile, float trailOpacity, Color color)
        {
            SlashTrail trail = new SlashTrail(48, 1.57f);
            float time = Main.GlobalTimeWrappedHourly % 3f / 3f;
            Player player = Main.player[Projectile.owner];
            float offset = MathHelper.Max(1f, (float)player.maxMinions);
            float num14 = (float)Projectile.identity % offset / offset + time;
            trail.color = color * trailOpacity;

            MiscShaderData miscShaderData = GameShaders.Misc["EmpressBlade"];
            int num = 1;
            int num2 = 0;
            int num3 = 0;
            float w = 0.6f;
            miscShaderData.UseShaderSpecificData(new Vector4(num, num2, num3, w));
            miscShaderData.Apply();
            trail.DrawTrail(Projectile, miscShaderData);
        }

        private int getOrder()
        {
            int order = 0;
            if (Projectile.whoAmI > 0)
            {
                for (int i = Projectile.whoAmI - 1; i >= 0; i--)
                {
                    if (Main.projectile[i].active && LobotomyWawPlayer.SwordSharpenedIsType(Main.projectile[i].type) && Main.projectile[i].owner == Projectile.owner) order++;
                }
            }
            return order;
        }
    }

    public class SwordSharpenedWithTearsRDespair : ModProjectile
    {
        public override string Texture => "LobotomyCorp/Projectiles/Realized/SwordSharpenedWithTearsRSword";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 3000;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.extraUpdates = 3;
        }

        private float mode
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private float timer
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private float state
        {
            get => Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            LobotomyWawPlayer modOwner = owner.GetModPlayer<LobotomyWawPlayer>();

            timer++;
            if (state == 0)
            {
                if (timer > 60 * 4)
                {
                    state++;
                    timer = 0;

                    Projectile.velocity = new Vector2(320, 0).RotateRandom(6.28f);
                    Vector2 targetPos = owner.MountedCenter + Projectile.velocity;
                    Projectile.rotation = Projectile.AngleTo(targetPos) + 6.28f;
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Gaho") with { Volume = 0.2f, MaxInstances = 0 }, Projectile.Center);
                    Projectile.netUpdate = true;
                }
            }
            else if (state == 1)
            {
                float lerp = timer / 240f;
                Vector2 targetPos = owner.MountedCenter + Projectile.velocity;
                if (lerp > 1f)
                {
                    Projectile.Center = targetPos;
                    Projectile.rotation = Terraria.Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + 3.14f, 0.025f * 0.6f);
                }
                else
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.25f * lerp);
                    Projectile.rotation = (float)Terraria.Utils.Lerp(Projectile.rotation, MathHelper.WrapAngle(Projectile.velocity.ToRotation() + 3.14f), 0.01f);
                }

                if (timer > 120 * 4)
                {
                    state = 3;
                    timer = 0;
                    Projectile.rotation = Projectile.velocity.ToRotation() + 3.14f;
                }
            }
            else if (state == 3)
            {
                float lerp = timer / 120f;
                Vector2 targetPos = owner.MountedCenter + Projectile.velocity - new Vector2(48 * lerp, 0).RotatedBy(Projectile.rotation);
                Projectile.Center = targetPos;

                if (timer > 30 * 4)
                {
                    state++;
                    timer = 0;
                    Projectile.velocity = Vector2.Normalize(owner.Center - Projectile.Center) * 10f;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    SoundEngine.PlaySound(new SoundStyle("LobotomyCorp/Sounds/Item/Natural/KnightOfDespair_Parring") with { Volume = 0.2f, MaxInstances = 0 }, Projectile.Center);
                }
            }
            else if (state == 4)
            {
                if (Projectile.timeLeft > 120 * 3)
                    Projectile.timeLeft = 120 * 3;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return state == 4;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (state == 4)
                return target.whoAmI == Projectile.owner;
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.immune = false;
            target.immuneTime = 0;
            LobotomyWawPlayer modPlayer = target.GetModPlayer<LobotomyWawPlayer>();
            modPlayer.SwordSharpenedImpaledBy(Projectile, 0);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SwordSharpenedWithTearsRSword.DrawTrail(Projectile, 1f, Color.DarkBlue);

            Vector2 position = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + 2.35619f;
            position = Projectile.Center - Main.screenPosition;
            position.Y += Projectile.gfxOffY;
            Color color = Color.Blue;
            if (state == 0 && timer < 240)
            {
                float lerp = timer / 240f;
                position.X += Main.rand.NextFloat(-3, 3) * lerp;
                position.Y += Main.rand.NextFloat(-3, 3) * lerp;
                color = Color.Lerp(Color.White, color, lerp);
            }

            Texture2D tex = SwordSharpenedWithTearsRSword.Glow.Value;
            
            color.A = 180;
            color *= 0.2f;
            for (int i = 0; i < 8; i++)
            {
                float prog = (float)(Main.timeForVisualEffects % 90 / 90);
                Vector2 offset = new Vector2(3 + 1 * (float)Math.Sin(6.28f * prog), 0).RotatedBy(0.785f * i);
                float scale = 1f + 0.1f * (float)Math.Sin(3.14f * prog);
                Main.EntitySpriteDraw(tex, position + offset, null, color * Projectile.Opacity, rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }

            tex = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(tex, position, null, lightColor * Projectile.Opacity, rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 offset = new Vector2(45, 0).RotatedBy(Projectile.rotation);
            hitbox.X += (int)offset.X;
            hitbox.Y += (int)offset.Y;
        }
    }

    class KnightOfDespairStars
    {
        private Vector2 position;
        private Vector2 velocity;
        private float opacity;
        private float size;
        public int pair;
        public bool Active;

        public void Update(float change)
        {
            if (Active)
            {
                size += change;
                if (size >= 1f)
                {
                    size = 1f;
                    opacity -= change;
                    if (opacity <= 0f)
                    {
                        Active = false;
                    }
                }
                position += velocity;
            }
        }

        public static KnightOfDespairStars Create(Vector2 position, int width, int height, int pair, Vector2? velocity = null)
        {
            KnightOfDespairStars stars = new KnightOfDespairStars();
            stars.position = position + new Vector2(Main.rand.Next(width), Main.rand.Next(height));
            if (velocity == null)
                stars.velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
            else
                stars.velocity = (Vector2)velocity;
            stars.opacity = 1f;
            stars.size = 0f;
            stars.pair = pair;
            stars.Active = true;
            return stars;
        }

        public void Draw(SpriteBatch sp, KnightOfDespairStars pair)
        {
            Texture2D tex = TextureAssets.Extra[57].Value;
            sp.Draw(tex, position - Main.screenPosition, null, Color.White * opacity, 0, tex.Size()/2, size/2, 0, 0);

            if (pair != null && pair.Active)
            {
                float op = (float)Math.Min(opacity, pair.opacity);
                Color color = new Color(134, 241, 224) * op;
                Terraria.Utils.DrawLine(sp, position, pair.position, color * opacity, color
                    * pair.opacity, 2f);
            }
        }
    }
}
