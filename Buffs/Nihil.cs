using Terraria;
using Terraria.ModLoader;
using LobotomyCorp.Players;

namespace LobotomyCorp.Buffs
{
	public class Nihil : ModBuff
	{
		public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public sealed override void Update(Player player, ref int buffIndex)
        {
            LobotomyAlephPlayer modPlayer = player.GetModPlayer<LobotomyAlephPlayer>();
            modPlayer.NihilMode = GetMode;
            bool notDebuffed = true;
            player.statDefense += defense(notDebuffed);
            player.GetDamage(GetDamageType) += damage(notDebuffed);
            player.moveSpeed += speed(notDebuffed);
            player.maxMinions += minionSlots(notDebuffed);
            player.GetCritChance(GetDamageType) += critical(notDebuffed);

            ExtraEffects(player, notDebuffed);
        }

        public virtual void ExtraEffects(Player player, bool notDebuffed)
        {

        }

        public virtual LobotomyAlephPlayer.NihilType GetMode => LobotomyAlephPlayer.NihilType.Nihil;

        public virtual DamageClass GetDamageType => DamageClass.Summon;

        public int DefenseA = 58;
        public int DefenseB = 0;

        private int defense(bool ndebuff)
        { return ndebuff ? DefenseA : DefenseB; }

        public float DamageA = .9f;
        public float DamageB = .12f;

        private float damage(bool ndebuff)
        { return ndebuff ? DamageA : DamageB; }

        public float SpeedA = .15f;
        public float SpeedB = .03f;

        private float speed(bool ndebuff)
        { return ndebuff ? SpeedA : SpeedB; }

        public int MinionSlotsA = 6;
        public int MinionSlotsB = 0;

        private int minionSlots(bool ndebuff)
        { return ndebuff ? MinionSlotsA : MinionSlotsB; }

        public float CriticalA = 0;
        public float CriticalB = 0;

        private float critical(bool ndebuff)
        { return ndebuff ? CriticalA : CriticalB; }
    }
}