using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace LobotomyCorp
{
	public class ZayinB : ModRarity // B - Base
	{
		public override Color RarityColor => new(150, 255, 150); // Color of the Green Rarity (2)

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
            // Makes it so reforges don't affect the rarity
		}
	}
	public class TethB : ModRarity
	{
		public override Color RarityColor => new(150, 150, 255); // Color of the Blue Rarity (1)

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class HeB : ModRarity
	{
		public override Color RarityColor => new(255, 255, 150); // Color of the Yellow Rarity (8)
		// also made it less vibrant so its different from the realization color

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class WawB : ModRarity
	{
		public override Color RarityColor => new(180, 40, 255); // Color of the Purple Rarity (11)
		// also made it less vibrant so its different from the realization color

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class AlephB : ModRarity
	{
		public override Color RarityColor => new(255, 40, 100); // Color of the Red Rarity (10)
		// also made it less vibrant so its different from the realization color

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class ZayinR : ModRarity // R - Realized
	{
		public override Color RarityColor => LobotomyCorp.ZayinRarity;

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class TethR : ModRarity
	{
		public override Color RarityColor => LobotomyCorp.TethRarity;

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class HeR : ModRarity
	{
		public override Color RarityColor => LobotomyCorp.HeRarity;

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class WawR : ModRarity
	{
		public override Color RarityColor => LobotomyCorp.WawRarity;

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
	public class AlephR : ModRarity
	{
		public override Color RarityColor => LobotomyCorp.AlephRarity;

		public override int GetPrefixedRarity(int offset, float valueMult) {
			return Type;
		}
	}
}