using LobotomyCorp.ModSystems;
using LobotomyCorp.UI;
using LobotomyCorp.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace LobotomyCorp.Tiles
{
	public class BlackBox3 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BlackBox3TileEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Black Box - Extractor");
			AddMapEntry(new Color(0, 0, 0), name);
			DustType = DustID.Wraith;
			TileID.Sets.DisableSmartCursor[Type] = true;
            AdjTiles = new int[] { ModContent.TileType<BlackBox>() , ModContent.TileType<BlackBox2>() };
		}

		public override bool RightClick(int i, int j)
        {
			//if (!Main.hardMode)
			//return false;
			bool entityExists = false;
            if (TileUtils.TryGetTileEntityAs(i, j, out BlackBox3TileEntity entity))
			{
				if (entity.isTalking)
				{
					return false;
				}
				entityExists = true;
			}
			HitWire(i, j);
			if (Main.expertMode && !LobEventFlags.binahDoneTalk && Main.tile[i, j].TileFrameY >= 18 * 3)
			{
				//Create Binah's speech
				if (entityExists)
				{
					if (!entity.isTalking)
					{
						if (Main.netMode == NetmodeID.MultiplayerClient)
						{
							return true;
                            if (!LobEventFlags.binahIntroTalk)
                            {
                                LobotomyCorp.TileEntityBlackBoxInitialize(entity.Position.X, entity.Position.Y, 0);
                                LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.BinahIntroTalk, true);
                                LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.KilledByRedMist, false);
                            }
                            else if (LobEventFlags.downedRedMist)
                            {
                                entity.InitiateText(3);
                                LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.BinahDoneTalk, true);
                            }
                            else if (LobEventFlags.killedByRedMist)
                            {
                                if (!LobEventFlags.binahRedmistTalk)
                                {
                                    entity.InitiateText(1);
                                    LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.BinahRedmistTalk, true);
                                }
                                else
                                    entity.InitiateText(2);
                                LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.KilledByRedMist, false);
                            }

                            //NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, entity.ID, entity.Position.X, entity.Position.Y);
                        }
						else
						{
                            if (!LobEventFlags.binahIntroTalk)
                            {
                                entity.InitiateText(0);
                                LobEventFlags.binahIntroTalk = true;
                                LobEventFlags.killedByRedMist = false;
                            }
                            else if (LobEventFlags.downedRedMist)
                            {
                                entity.InitiateText(3);
                                LobEventFlags.binahDoneTalk = true;
                            }
                            else if (LobEventFlags.killedByRedMist)
                            {
                                if (!LobEventFlags.binahRedmistTalk)
                                {
                                    entity.InitiateText(1);
                                    LobEventFlags.binahRedmistTalk = true;
                                }
                                else
                                    entity.InitiateText(2);
                                LobEventFlags.killedByRedMist = false;
                            }
                        }
					}
				}
			}

            return true;
		}
				
		public override void HitWire(int i, int j)
		{
			//if (!Main.hardMode)
			//{
				//return;
			//}	

			int x = i - Main.tile[i, j].TileFrameX / 18 % 2;
			int y = j - Main.tile[i, j].TileFrameY / 18 % 3;
			for (int l = x; l < x + 2; l++)
			{
				for (int m = y; m < y + 3; m++)
				{
					if (Main.tile[l, m].HasTile && Main.tile[l, m].TileType == Type)
					{
						if (Main.tile[l, m].TileFrameY < 54)
						{
							Main.tile[l, m].TileFrameY += 54;
						}
						else
						{
							Main.tile[l, m].TileFrameY -= 54;
						}
					}
				}
			}
			if (Wiring.running)
			{
				Wiring.SkipWire(x, y);
				Wiring.SkipWire(x, y + 1);
				Wiring.SkipWire(x, y + 2);
				Wiring.SkipWire(x + 1, y);
				Wiring.SkipWire(x + 1, y + 1);
				Wiring.SkipWire(x + 1, y + 2);
			}
			NetMessage.SendTileSquare(-1, x, y + 1, 3);
		}
        /*
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<Items.BlackBox3>());
		}*/

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<BlackBox3TileEntity>().Kill(i, j);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY < 54)
				return;
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			spriteBatch.Draw(
				TextureAssets.Tile[tile.TileType].Value, 
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.TileFrameX + 36, tile.TileFrameY, 16, 16),
				Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(Mod.Find<ModItem>("BlackBox3").Type);
        }
    }

	public class BlackBox3TileEntity : ModTileEntity
	{
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<BlackBox3>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Sync the entire multitile's area.  Modify "width" and "height" to the size of your multitile in tiles
                int width = 2;
                int height = 3;
                NetMessage.SendTileSquare(Main.myPlayer, i, j, width, height);

                // Sync the placement of the tile entity with other clients
                // The "type" parameter refers to the tile type which placed the tile entity, so "Type" (the type of the tile entity) needs to be used here instead
                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            int placedEntity = Place(i, j);
            return placedEntity;
        }

		private int current = -1;
		private int mode = 0;
		private int dialogue = 0;
		public bool isTalking = false;

        public override void Update()
        {
            if (LobEventFlags.downedRedMist && !LobEventFlags.binahDoneTalk && !isTalking)
			{
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Player player = Main.LocalPlayer;
					if (CheckIfPlayerNear(player))
					{
						InitiateText(3);
						LobEventFlags.binahDoneTalk = true;
						TileLoader.HitWire(Position.X, Position.Y, Main.tile[Position.X, Position.Y].TileType);
					}
				}
				else
				{
					foreach (Player player in Main.ActivePlayers)
					{
						if (CheckIfPlayerNear(player))
						{
							InitiateText(3);
							LobEventFlags.BinahEntitySendPacket(LobEventFlags.FlagIDs.BinahDoneTalk, true);
							TileLoader.HitWire(Position.X, Position.Y, Main.tile[Position.X, Position.Y].TileType);

                            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
                            break;
						}
					}
				}
			}

			if (isTalking)
			{
                if (!SuppressionText.IsActive(current))
				{
					dialogue++;
					bool isDone = false;
					switch(mode)
					{
						case 0:
							if (dialogue == 8)
							{
								SpawnShellNearestPlayer();
							}
							if (dialogue >= 9)
								isDone = true; 
							break;
                        case 1:
                            if (dialogue == 2)
                            {
                                SpawnShellNearestPlayer();
                            }
                            if (dialogue >= 4)
                                isDone = true; 
							break;
                        case 2:
							if (dialogue >= 1)
							{
                                SpawnShellNearestPlayer();
                                isDone = true;
							}
							break;
                        case 3:
                            if (dialogue >= 6)
                                isDone = true; 
							break;
						default:
							isDone = true;
							break;
                    }
					if (isDone)
					{
						isTalking = false;
						TileLoader.HitWire(Position.X, Position.Y, Main.tile[Position.X, Position.Y].TileType);
					}
					else
					{
                        current = CreateText();
                    }
                }
            }
            if (Main.netMode == NetmodeID.Server)
            {
                //NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

		private void SpawnShellNearestPlayer()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ModContent.ItemType<Items.ItemTiles.DisciplinaryShell>());
				return;
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (CheckIfPlayerNear(player))
                {
					player.QuickSpawnItem(player.GetSource_FromThis(), ModContent.ItemType<Items.ItemTiles.DisciplinaryShell>());
                    break;
                }
            }
        }

		private bool CheckIfPlayerNear(Player player)
		{
			return player.active && !player.dead && Vector2.DistanceSquared(new Vector2(Position.X, Position.Y) * 16, player.Center) < 96 * 96;
		}

		public void InitiateText(int which)
		{
			isTalking = true;
			mode = which;
			dialogue = 0;
			current = CreateText();

            Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 1, 1, 1);
        }

        public void InitiateText(string text)
        {
            isTalking = true;
            mode = -1;
            dialogue = 0;
            current = CreateText(text);

            Dust.NewDust(new Vector2(Position.X, Position.Y) * 16, 1, 1, 1);
        }

        private int CreateText()
		{
            string text = SuppressionText.GetSephirahTalk("Binah", mode + 1, dialogue + 1);

            float time = 0.5f;
            int fade = 90;
            Vector2 pos = new Vector2(Position.X + 1, Position.Y - 1) * 16;

            return SuppressionText.AddText(text, pos, Main.rand.NextFloat(-0.12f, 0.12f), 0.5f, Color.Yellow, time, fade, 0, 0);
        }

		private int CreateText(string text)
		{
            float time = 0.5f;
            int fade = 90;
            Vector2 pos = new Vector2(Position.X + 1, Position.Y - 1) * 16;

            return SuppressionText.AddText(text, pos, Main.rand.NextFloat(-0.12f, 0.12f), 0.5f, Color.Yellow, time, fade, 0, 0);
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
				NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
			writer.Write(current);
			writer.Write(mode);
			writer.Write(dialogue);
			writer.Write(isTalking);
        }

        public override void NetReceive(BinaryReader reader)
        {
			current = reader.ReadInt32();
			mode = reader.ReadInt32();
			dialogue = reader.ReadInt32();
			isTalking = reader.ReadBoolean();
        }
    }
}