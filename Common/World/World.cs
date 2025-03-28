using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrarianSky.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using System.Text.Json;
using System.IO;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.Social;
using Terraria.WorldBuilding;

namespace TerrarianSky.Common.World
{
    internal class World
    {

        public String worldName;

        public readonly record struct TileData(TileTypeData TileTypeData, WallTypeData WallTypeData, TileWallWireStateData TileWallWireStateData,
            LiquidData LiquidData, TileWallBrightnessInvisibilityData TileWallBrightnessInvisibilityData);
        public readonly record struct ChestData(List<Item> Items, int EatingAnimationTime, int Frame, int FrameCounter, string Name, int X, int Y);


        public TileData[,] tiles;
        public List<ChestData> chests = [];
        public List<Item> items = [];
        public List<NPC> npcs = [];
        public List<Projectile> projectiles = [];
        public List<Rain> rain = [];
        public List<Sign> signs = [];
        public MoonPhase moonPhase;
        public double time;
        public bool dayTime;
        public Point spawnPoint;


        public World()
        {
            SaveWorld();
        }
        public void SaveWorld()
        {
            //Save Tiles
            tiles = new TileData[Main.maxTilesX, Main.maxTilesY];
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    tiles[x, y] = new(tile.Get<TileTypeData>(), tile.Get<WallTypeData>(), tile.Get<TileWallWireStateData>(),
                        tile.Get<LiquidData>(), tile.Get<TileWallBrightnessInvisibilityData>());
                }
            }
            //Reset and save Chests
            chests.Clear();
            foreach(Chest chest in Main.chest)
            {
                if (chest == null) break;
                chests.Add(Util.CopyOfChestData(chest));
            }
            //Save World Items
            items = Util.CopyOfList<Item>(Main.item, Util.CopyOfItem);
            //Save NPCs
            npcs = Util.CopyOfList<NPC>(Main.npc, Util.CopyOfNPC);
            //Save Projectiles
            projectiles = Util.CopyOfList<Projectile>(Main.projectile, Util.CopyOfProjectile);
            //Save MoonPhase
            moonPhase = Main.GetMoonPhase();
            //Save Weather
            //rain = Util.CopyOfList<Rain>(Main.rain, Util.CopyOfRain);
            //Save Signs
            signs = Util.CopyOfList<Sign>(Main.sign, Util.CopyOfSign);
            //Save Time
            time = Main.time;
            dayTime = Main.dayTime;
            //Save Name
            worldName = Main.worldName;
            //Save Spawn Point
            spawnPoint.X = Main.spawnTileX;
            spawnPoint.Y = Main.spawnTileY;
            //TO DO: save reset dials cooldown
        }

        public void LoadWorld() {
            //Restore Tiles
		    if (tiles == null) {
			    return;
		    }
		    for (int x = 0; x < Main.maxTilesX; x++)
            {
			    for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Util.ReplaceTile(Main.tile[x, y], tiles[x, y]);
			    }
		    }
            //Restore Chest Data
            int i = 0;
            foreach(ChestData chestData in chests)
            {
                if (Main.chest[i] == null)
                    Main.chest[i] = new();
                Util.ReplaceChest(Main.chest[i++], chestData);
            }
            //Resets additional chests in Main.chest
            while (i < Main.chest.Length)
            {
                Main.chest[i++] = null;
            }
            //Reset Clouds
            Cloud.resetClouds();
            //Load World Items
            Main.item = Util.CopyOfList<Item>(items, Util.CopyOfItem).ToArray();
            //Load NPCs
            Main.npc = Util.CopyOfList<NPC>(npcs, Util.CopyOfNPC).ToArray();
            //Load Projectiles
            Main.projectile = Util.CopyOfList<Projectile>(projectiles, Util.CopyOfProjectile).ToArray();
            //Load Weather
            //Main.rain = Util.CopyOfList<Rain>(rain, Util.CopyOfRain).ToArray();
            //Load Signs
            Main.sign = Util.CopyOfList<Sign>(signs, Util.CopyOfSign).ToArray();
            //Load Moon Phase
            Main.moonPhase = (int)moonPhase;
            //Load Time
            Main.time = time;
            Main.dayTime = dayTime;
            //Load Name
            Main.worldName = worldName;
            //Load Spawn Point
            Main.spawnTileX = spawnPoint.X;
            Main.spawnTileY = spawnPoint.Y;
            //Main.LoadWorlds();
            //Main.weatherCounter;
            //Main.player;
            //Main.star;
        }

        public void WriteWorld()
        {
            //string fileName = worldName + ".json";
            //string jsonString = JsonSerializer.Serialize(this, options);
            //Main.NewText("File: " + fileName);
            //File.WriteAllText("D:\\Documenti\\My Games\\Terraria\\tModLoader\\ModSources\\AllDesert\\Saves\\" + fileName, jsonString);
            World currentWorld = new();
            this.LoadWorld();

		    int num;
		    byte[] array;
		    using (MemoryStream memoryStream = new(7000000)) {
			    using (BinaryWriter writer = new(memoryStream)) {
				    WorldFile.SaveWorld_Version2(writer);
			    }

			    array = memoryStream.ToArray();
			    num = array.Length;
		    }

            //int dotIndex = Main.worldPathName.LastIndexOf('.');
            //string writePath = Main.worldPathName.Insert(dotIndex, "_" + worldName);
            string writePath = Main.worldPathName + "_" + worldName;

            FileUtilities.Write(writePath, array, num, false);

            currentWorld.LoadWorld();
        }

        public static World ReadWorld(string name)
        {
            Main.checkXMas();
			Main.checkHalloween();
			// Patch note: flag ^ is used in WorldIO call below.

			try {
				//using MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(Main.worldPathName.Insert(Main.worldPathName.LastIndexOf('.'), "_" + name)));
                using MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(Main.worldPathName + "_" + name));
				using BinaryReader binaryReader = new BinaryReader(memoryStream);
				try {
					WorldGen.loadFailed = false;
					WorldGen.loadSuccess = false;

					int num3 = WorldFile.LoadWorld_Version2(binaryReader);

					binaryReader.Close();
					memoryStream.Close();

					SystemLoader.OnWorldLoad();
					//WorldIO.Load(Main.worldPathName, flag);

					if (num3 != 0)
						WorldGen.loadFailed = true;
					else
						WorldGen.loadSuccess = true;

					if (WorldGen.loadFailed || !WorldGen.loadSuccess)
						return new();

					WorldFile.ClearTempTiles();
					WorldGen.gen = true;
					GenVars.waterLine = Main.maxTilesY;
					Liquid.QuickWater(2);
					WorldGen.WaterCheck();
					int num4 = 0;
					Liquid.quickSettle = true;
					int num5 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					float num6 = 0f;
					while (Liquid.numLiquid > 0 && num4 < 100000) {
						num4++;
						float num7 = (float)(num5 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num5;
						if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num5)
							num5 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;

						if (num7 > num6)
							num6 = num7;
						else
							num7 = num6;

						Main.statusText = Lang.gen[27].Value + " " + (int)(num7 * 100f / 2f + 50f) + "%";
						Liquid.UpdateLiquid();
					}

					Liquid.quickSettle = false;
					Main.weatherCounter = WorldGen.genRand.Next(3600, 18000);
					Cloud.resetClouds();
					WorldGen.WaterCheck();
					WorldGen.gen = false;
					NPC.setFireFlyChance();

					NPC.SetWorldSpecificMonstersByWorldID();
				}
				catch (Exception) {
					WorldGen.loadFailed = true;
					WorldGen.loadSuccess = false;
					try {
						binaryReader.Close();
						memoryStream.Close();
						return new();
					}
					catch {
						return new();
					}
				}
			}
			catch (Exception)
            {
				WorldGen.loadFailed = true;
				WorldGen.loadSuccess = false;
				return new();
			}

            return new();
        }
    }
}
