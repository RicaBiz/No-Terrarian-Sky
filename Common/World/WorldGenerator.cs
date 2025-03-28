using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Events;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;

namespace TerrarianSky.Common.World
{
    internal class WorldGenerator
    {
        private WorldGenConfiguration configuration = WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");
        private GenerationProgress progress;
        public static List<GenPass> vanillaGenPasses = [];

        public WorldGenerator()
        {
            ResetProgress();
            ResetVanillaGenPasses();
        }

        private void ResetProgress()
        {
            progress = new();
        }

        public World GenerateWorld(string worldName, WorldGenerationData worldGenerationData)
        {
            WorldGen.clearWorld();

			ResetGenVars();
			ResetProgress();

			List<GenPass> tasks = new(vanillaGenPasses);
            //tasks.Insert(worldType.PassIndex, worldType.GenPass);
			foreach (var genPass in worldGenerationData.GenPasses)
				tasks.Add(genPass);
            tasks.RemoveAll(pass => !worldGenerationData.AbledPasses.Contains(pass.Name));
			foreach (var pass in tasks)
			{
				pass.Apply(progress, configuration.GetPassConfiguration(pass.Name));
			}
			WorldGen.EveryTileFrame();
			//Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld);
			Main.worldName = worldName;
            return new();
        }

		private void ResetGenVars()
		{
			int seed = Main.ActiveWorldFileData.Seed;
			WorldGen.remixWorldGen = WorldGen.tempRemixWorldGen;
			WorldGen.tenthAnniversaryWorldGen = WorldGen.tempTenthAnniversaryWorldGen;
			WorldGen.drunkWorldGen = false;
			WorldGen.drunkWorldGenText = false;
			Main.afterPartyOfDoom = false;
			if (seed == 5162020 || WorldGen.everythingWorldGen)
			{
				WorldGen.drunkWorldGen = true;
				WorldGen.drunkWorldGenText = true;
				Main.drunkWorld = true;
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
				if (!Main.dayTime)
				{
					Main.time = 0.0;
				}
			}
			else if (seed == 5162021 || seed == 5162011)
			{
				WorldGen.tenthAnniversaryWorldGen = true;
			}
			Main.notTheBeesWorld = WorldGen.notTheBees;
			if (WorldGen.notTheBees)
			{
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
			}
			Main.noTrapsWorld = WorldGen.noTrapsWorldGen;
			if (WorldGen.noTrapsWorldGen)
			{
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
			}
			if (WorldGen.getGoodWorldGen)
			{
				Main.getGoodWorld = true;
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
			}
			else
			{
				Main.getGoodWorld = false;
			}
			Main.tenthAnniversaryWorld = WorldGen.tenthAnniversaryWorldGen;
			if (WorldGen.tenthAnniversaryWorldGen)
			{
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
			}
			Main.dontStarveWorld = WorldGen.dontStarveWorldGen;
			if (WorldGen.dontStarveWorldGen)
			{
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
			}
			Main.remixWorld = WorldGen.remixWorldGen;
			if (WorldGen.remixWorldGen)
			{
				Main.rand = new UnifiedRandom();
				seed = Main.rand.Next(999999999);
			}
			Main.zenithWorld = WorldGen.everythingWorldGen;
			Main.lockMenuBGChange = true;
			GenVars.configuration = configuration;
			Hooks.ProcessWorldGenConfig(ref GenVars.configuration);
			WorldGen._lastSeed = seed;
			GenVars.structures = new StructureMap();
			GenVars.desertHiveHigh = Main.maxTilesY;
			GenVars.desertHiveLow = 0;
			GenVars.desertHiveLeft = Main.maxTilesX;
			GenVars.desertHiveRight = 0;
			GenVars.worldSurfaceLow = 0.0;
			GenVars.worldSurface = 0.0;
			GenVars.worldSurfaceHigh = 0.0;
			GenVars.rockLayerLow = 0.0;
			GenVars.rockLayer = 0.0;
			GenVars.rockLayerHigh = 0.0;
			GenVars.copper = 7;
			GenVars.iron = 6;
			GenVars.silver = 9;
			GenVars.gold = 8;
			GenVars.dungeonSide = 0;
			GenVars.jungleHut = (ushort)WorldGen.genRand.Next(5);
			GenVars.shellStartXLeft = 0;
			GenVars.shellStartYLeft = 0;
			GenVars.shellStartXRight = 0;
			GenVars.shellStartYRight = 0;
			GenVars.PyrX = null;
			GenVars.PyrY = null;
			GenVars.numPyr = 0;
			GenVars.jungleMinX = -1;
			GenVars.jungleMaxX = -1;
			GenVars.snowMinX = new int[Main.maxTilesY];
			GenVars.snowMaxX = new int[Main.maxTilesY];
			GenVars.snowTop = 0;
			GenVars.snowBottom = 0;
			GenVars.skyLakes = 1;
			if (Main.maxTilesX > 8000)
			{
				GenVars.skyLakes++;
			}
			if (Main.maxTilesX > 6000)
			{
				GenVars.skyLakes++;
			}
			GenVars.beachBordersWidth = 275;
			GenVars.beachSandRandomCenter = GenVars.beachBordersWidth + 5 + 40;
			GenVars.beachSandRandomWidthRange = 20;
			GenVars.beachSandDungeonExtraWidth = 40;
			GenVars.beachSandJungleExtraWidth = 20;
			GenVars.oceanWaterStartRandomMin = 220;
			GenVars.oceanWaterStartRandomMax = GenVars.oceanWaterStartRandomMin + 40;
			GenVars.oceanWaterForcedJungleLength = 275;
			GenVars.leftBeachEnd = 0;
			GenVars.rightBeachStart = 0;
			GenVars.evilBiomeBeachAvoidance = GenVars.beachSandRandomCenter + 60;
			GenVars.evilBiomeAvoidanceMidFixer = 50;
			GenVars.lakesBeachAvoidance = GenVars.beachSandRandomCenter + 20;
			GenVars.smallHolesBeachAvoidance = GenVars.beachSandRandomCenter + 20;
			GenVars.surfaceCavesBeachAvoidance = GenVars.beachSandRandomCenter + 20;
			GenVars.surfaceCavesBeachAvoidance2 = GenVars.beachSandRandomCenter + 20;
			GenVars.jungleOriginX = 0;
			GenVars.snowOriginLeft = 0;
			GenVars.snowOriginRight = 0;
			GenVars.logX = -1;
			GenVars.logY = -1;
			GenVars.dungeonLocation = 0;
		}

		private static bool growGrassUnderground = false;
		private static int heartCount;
		private static bool skipFramingDuringGen = false;
		private void ResetVanillaGenPasses()
		{
			if (vanillaGenPasses.Count != 0)
				return;

			vanillaGenPasses.Add(new PassLegacy("Reset", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					GenVars.crimsonLeft = false;
				}
				else
				{
					GenVars.crimsonLeft = true;
				}
				GenVars.numOceanCaveTreasure = 0;
				GenVars.skipDesertTileCheck = false;
				growGrassUnderground = false;
				WorldGen.gen = true;
				Liquid.ReInit();
				WorldGen.noTileActions = true;
				progress.Message = "";
				WorldGen.SetupStatueList();
				WorldGen.RandomizeWeather();
				Main.cloudAlpha = 0f;
				Main.maxRaining = 0f;
				Main.raining = false;
				heartCount = 0;
				GenVars.extraBastStatueCount = 0;
				GenVars.extraBastStatueCountMax = 2;
				Main.checkXMas();
				Main.checkHalloween();
				ResetGenerator();
				GenVars.UndergroundDesertLocation = Rectangle.Empty;
				GenVars.UndergroundDesertHiveLocation = Rectangle.Empty;
				GenVars.numLarva = 0;
				List<int> list = new List<int> { 274, 220, 112, 218, 3019 };
				if (WorldGen.remixWorldGen)
				{
					list = new List<int> { 274, 220, 683, 218, 3019 };
				}
				List<int> list2 = new List<int>();
				while (list.Count > 0)
				{
					int index = WorldGen.genRand.Next(list.Count);
					int item = list[index];
					list2.Add(item);
					list.RemoveAt(index);
				}
				GenVars.hellChestItem = list2.ToArray();
				int num = 86400;
				Main.slimeRainTime = -WorldGen.genRand.Next(num * 2, num * 3);
				Main.cloudBGActive = -WorldGen.genRand.Next(8640, 86400);
				skipFramingDuringGen = false;
				SavedOreTiers.Copper = 7;
				SavedOreTiers.Iron = 6;
				SavedOreTiers.Silver = 9;
				SavedOreTiers.Gold = 8;
				GenVars.copperBar = 20;
				GenVars.ironBar = 22;
				GenVars.silverBar = 21;
				GenVars.goldBar = 19;
				if (WorldGen.genRand.Next(2) == 0)
				{
					GenVars.copper = 166;
					GenVars.copperBar = 703;
					SavedOreTiers.Copper = 166;
				}
				if ((!WorldGen.dontStarveWorldGen || WorldGen.drunkWorldGen) && WorldGen.genRand.Next(2) == 0)
				{
					GenVars.iron = 167;
					GenVars.ironBar = 704;
					SavedOreTiers.Iron = 167;
				}
				if (WorldGen.genRand.Next(2) == 0)
				{
					GenVars.silver = 168;
					GenVars.silverBar = 705;
					SavedOreTiers.Silver = 168;
				}
				if ((!WorldGen.dontStarveWorldGen || WorldGen.drunkWorldGen) && WorldGen.genRand.Next(2) == 0)
				{
					GenVars.gold = 169;
					GenVars.goldBar = 706;
					SavedOreTiers.Gold = 169;
				}
				WorldGen.crimson = WorldGen.genRand.Next(2) == 0;
				if (WorldGen.WorldGenParam_Evil == 0)
				{
					WorldGen.crimson = false;
				}
				if (WorldGen.WorldGenParam_Evil == 1)
				{
					WorldGen.crimson = true;
				}
				if (GenVars.jungleHut == 0)
				{
					GenVars.jungleHut = 119;
				}
				else if (GenVars.jungleHut == 1)
				{
					GenVars.jungleHut = 120;
				}
				else if (GenVars.jungleHut == 2)
				{
					GenVars.jungleHut = 158;
				}
				else if (GenVars.jungleHut == 3)
				{
					GenVars.jungleHut = 175;
				}
				else if (GenVars.jungleHut == 4)
				{
					GenVars.jungleHut = 45;
				}
				Main.worldID = WorldGen.genRand.Next(int.MaxValue);
				WorldGen.RandomizeTreeStyle();
				WorldGen.RandomizeCaveBackgrounds();
				WorldGen.RandomizeBackgrounds(WorldGen.genRand);
				WorldGen.RandomizeMoonState(WorldGen.genRand);
				WorldGen.TreeTops.CopyExistingWorldInfoForWorldGeneration();
				GenVars.dungeonSide = ((WorldGen.genRand.Next(2) != 0) ? 1 : (-1));
				if (WorldGen.remixWorldGen)
				{
					if (GenVars.dungeonSide == -1)
					{
						double num2 = 1.0 - (double)WorldGen.genRand.Next(20, 35) * 0.01;
						GenVars.jungleOriginX = (int)((double)Main.maxTilesX * num2);
					}
					else
					{
						double num3 = (double)WorldGen.genRand.Next(20, 35) * 0.01;
						GenVars.jungleOriginX = (int)((double)Main.maxTilesX * num3);
					}
				}
				else
				{
					int minValue = 15;
					int maxValue = 30;
					if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.remixWorldGen)
					{
						minValue = 25;
						maxValue = 35;
					}
					if (GenVars.dungeonSide == -1)
					{
						double num4 = 1.0 - (double)WorldGen.genRand.Next(minValue, maxValue) * 0.01;
						GenVars.jungleOriginX = (int)((double)Main.maxTilesX * num4);
					}
					else
					{
						double num5 = (double)WorldGen.genRand.Next(minValue, maxValue) * 0.01;
						GenVars.jungleOriginX = (int)((double)Main.maxTilesX * num5);
					}
				}
				int num6 = WorldGen.genRand.Next(Main.maxTilesX);
				if (WorldGen.drunkWorldGen)
				{
					GenVars.dungeonSide *= -1;
				}
				if (GenVars.dungeonSide == 1)
				{
					while ((double)num6 < (double)Main.maxTilesX * 0.6 || (double)num6 > (double)Main.maxTilesX * 0.75)
					{
						num6 = WorldGen.genRand.Next(Main.maxTilesX);
					}
				}
				else
				{
					while ((double)num6 < (double)Main.maxTilesX * 0.25 || (double)num6 > (double)Main.maxTilesX * 0.4)
					{
						num6 = WorldGen.genRand.Next(Main.maxTilesX);
					}
				}
				if (WorldGen.drunkWorldGen)
				{
					GenVars.dungeonSide *= -1;
				}
				int num7 = WorldGen.genRand.Next(50, 90);
				double num8 = (double)Main.maxTilesX / 4200.0;
				num7 += (int)((double)WorldGen.genRand.Next(20, 40) * num8);
				num7 += (int)((double)WorldGen.genRand.Next(20, 40) * num8);
				int num9 = num6 - num7;
				num7 = WorldGen.genRand.Next(50, 90);
				num7 += (int)((double)WorldGen.genRand.Next(20, 40) * num8);
				num7 += (int)((double)WorldGen.genRand.Next(20, 40) * num8);
				int num10 = num6 + num7;
				if (num9 < 0)
				{
					num9 = 0;
				}
				if (num10 > Main.maxTilesX)
				{
					num10 = Main.maxTilesX;
				}
				GenVars.snowOriginLeft = num9;
				GenVars.snowOriginRight = num10;
				GenVars.leftBeachEnd = WorldGen.genRand.Next(GenVars.beachSandRandomCenter - GenVars.beachSandRandomWidthRange, GenVars.beachSandRandomCenter + GenVars.beachSandRandomWidthRange);
				if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.remixWorldGen)
				{
					GenVars.leftBeachEnd = GenVars.beachSandRandomCenter + GenVars.beachSandRandomWidthRange;
				}
				if (GenVars.dungeonSide == 1)
				{
					GenVars.leftBeachEnd += GenVars.beachSandDungeonExtraWidth;
				}
				else
				{
					GenVars.leftBeachEnd += GenVars.beachSandJungleExtraWidth;
				}
				GenVars.rightBeachStart = Main.maxTilesX - WorldGen.genRand.Next(GenVars.beachSandRandomCenter - GenVars.beachSandRandomWidthRange, GenVars.beachSandRandomCenter + GenVars.beachSandRandomWidthRange);
				if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.remixWorldGen)
				{
					GenVars.rightBeachStart = Main.maxTilesX - (GenVars.beachSandRandomCenter + GenVars.beachSandRandomWidthRange);
				}
				if (GenVars.dungeonSide == -1)
				{
					GenVars.rightBeachStart -= GenVars.beachSandDungeonExtraWidth;
				}
				else
				{
					GenVars.rightBeachStart -= GenVars.beachSandJungleExtraWidth;
				}
				int num11 = 50;
				if (GenVars.dungeonSide == -1)
				{
					GenVars.dungeonLocation = WorldGen.genRand.Next(GenVars.leftBeachEnd + num11, (int)((double)Main.maxTilesX * 0.2));
				}
				else
				{
					GenVars.dungeonLocation = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), GenVars.rightBeachStart - num11);
				}
				int num12 = 0;
				if (Main.maxTilesX >= 8400)
				{
					num12 = 2;
				}
				else if (Main.maxTilesX >= 6400)
				{
					num12 = 1;
				}
				GenVars.extraBastStatueCountMax = 2 + num12;
				Main.tileSolid[659] = false;
			}));
			vanillaGenPasses.Add(new TerrainPass());
			vanillaGenPasses.Add(new PassLegacy("Dunes", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[1].Value;
				int random = passConfig.Get<WorldGenRange>("Count").GetRandom(WorldGen.genRand);
				double num13 = passConfig.Get<double>("ChanceOfPyramid");
				if (WorldGen.drunkWorldGen)
				{
					num13 = 1.0;
				}
				double num14 = (double)Main.maxTilesX / 4200.0;
				GenVars.PyrX = new int[random + 3];
				GenVars.PyrY = new int[random + 3];
				DunesBiome dunesBiome = GenVars.configuration.CreateBiome<DunesBiome>();
				for (int i = 0; i < random; i++)
				{
					progress.Set((double)i / (double)random);
					Point origin = Point.Zero;
					bool flag = false;
					int num15 = 0;
					while (!flag)
					{
						origin = WorldGen.RandomWorldPoint(0, 500, 0, 500);
						bool flag2 = Math.Abs(origin.X - GenVars.jungleOriginX) < (int)(600.0 * num14);
						bool flag3 = Math.Abs(origin.X - Main.maxTilesX / 2) < 300;
						bool flag4 = origin.X > GenVars.snowOriginLeft - 300 && origin.X < GenVars.snowOriginRight + 300;
						num15++;
						if (num15 >= Main.maxTilesX)
						{
							flag2 = false;
						}
						if (num15 >= Main.maxTilesX * 2)
						{
							flag4 = false;
						}
						flag = !(flag2 || flag3 || flag4);
					}
					dunesBiome.Place(origin, GenVars.structures);
					if (WorldGen.genRand.NextDouble() <= num13)
					{
						int num16 = WorldGen.genRand.Next(origin.X - 200, origin.X + 200);
						for (int j = 0; j < Main.maxTilesY; j++)
						{
							if (Main.tile[num16, j].HasTile)
							{
								GenVars.PyrX[GenVars.numPyr] = num16;
								GenVars.PyrY[GenVars.numPyr] = j + 20;
								GenVars.numPyr++;
								break;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Ocean Sand", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Language.GetTextValue("WorldGeneration.OceanSand");
				for (int k = 0; k < 3; k++)
				{
					progress.Set((double)k / 3.0);
					int num17 = WorldGen.genRand.Next(Main.maxTilesX);
					while ((double)num17 > (double)Main.maxTilesX * 0.4 && (double)num17 < (double)Main.maxTilesX * 0.6)
					{
						num17 = WorldGen.genRand.Next(Main.maxTilesX);
					}
					int num18 = WorldGen.genRand.Next(35, 90);
					if (k == 1)
					{
						double num19 = (double)Main.maxTilesX / 4200.0;
						num18 += (int)((double)WorldGen.genRand.Next(20, 40) * num19);
					}
					if (WorldGen.genRand.Next(3) == 0)
					{
						num18 *= 2;
					}
					if (k == 1)
					{
						num18 *= 2;
					}
					int num20 = num17 - num18;
					num18 = WorldGen.genRand.Next(35, 90);
					if (WorldGen.genRand.Next(3) == 0)
					{
						num18 *= 2;
					}
					if (k == 1)
					{
						num18 *= 2;
					}
					int num21 = num17 + num18;
					if (num20 < 0)
					{
						num20 = 0;
					}
					if (num21 > Main.maxTilesX)
					{
						num21 = Main.maxTilesX;
					}
					if (k == 0)
					{
						num20 = 0;
						num21 = GenVars.leftBeachEnd;
					}
					else if (k == 2)
					{
						num20 = GenVars.rightBeachStart;
						num21 = Main.maxTilesX;
					}
					else if (k == 1)
					{
						continue;
					}
					int num22 = WorldGen.genRand.Next(50, 100);
					for (int l = num20; l < num21; l++)
					{
						if (WorldGen.genRand.Next(2) == 0)
						{
							num22 += WorldGen.genRand.Next(-1, 2);
							if (num22 < 50)
							{
								num22 = 50;
							}
							if (num22 > 200)
							{
								num22 = 200;
							}
						}
						for (int m = 0; (double)m < (Main.worldSurface + Main.rockLayer) / 2.0; m++)
						{
							if (Main.tile[l, m].HasTile)
							{
								if (l == (num20 + num21) / 2 && WorldGen.genRand.Next(6) == 0)
								{
									GenVars.PyrX[GenVars.numPyr] = l;
									GenVars.PyrY[GenVars.numPyr] = m;
									GenVars.numPyr++;
								}
								int num23 = num22;
								if (l - num20 < num23)
								{
									num23 = l - num20;
								}
								if (num21 - l < num23)
								{
									num23 = num21 - l;
								}
								num23 += WorldGen.genRand.Next(5);
								for (int n = m; n < m + num23; n++)
								{
									if (l > num20 + WorldGen.genRand.Next(5) && l < num21 - WorldGen.genRand.Next(5))
									{
										Main.tile[l, n].TileType = 53;
									}
								}
								break;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Sand Patches", delegate
			{
				int num24 = (int)((double)Main.maxTilesX * 0.013);
				if (WorldGen.remixWorldGen)
				{
					num24 /= 4;
				}
				for (int num25 = 0; num25 < num24; num25++)
				{
					int num26 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num27 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
					if (WorldGen.remixWorldGen)
					{
						num27 = WorldGen.genRand.Next((int)Main.rockLayer - 100, Main.maxTilesY - 350);
					}
					while ((double)num26 > (double)Main.maxTilesX * 0.46 && (double)num26 < (double)Main.maxTilesX * 0.54 && (double)num27 < Main.worldSurface + 150.0)
					{
						num26 = WorldGen.genRand.Next(0, Main.maxTilesX);
						num27 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
					}
					int num28 = WorldGen.genRand.Next(15, 70);
					int steps = WorldGen.genRand.Next(20, 130);
					WorldGen.TileRunner(num26, num27, num28, steps, 53);
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Tunnels", delegate
			{
				int num29 = (int)((double)Main.maxTilesX * 0.0015);
				if (WorldGen.remixWorldGen)
				{
					num29 = (int)((double)num29 * 1.5);
				}
				for (int num30 = 0; num30 < num29; num30++)
				{
					if (GenVars.numTunnels >= GenVars.maxTunnels - 1)
					{
						break;
					}
					int[] array = new int[10];
					int[] array2 = new int[10];
					int num31 = WorldGen.genRand.Next(450, Main.maxTilesX - 450);
					if (!WorldGen.remixWorldGen)
					{
						if (WorldGen.tenthAnniversaryWorldGen)
						{
							num31 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
						}
						else
						{
							while ((double)num31 > (double)Main.maxTilesX * 0.4 && (double)num31 < (double)Main.maxTilesX * 0.6)
							{
								num31 = WorldGen.genRand.Next(450, Main.maxTilesX - 450);
							}
						}
					}
					int num32 = 0;
					bool flag5;
					do
					{
						flag5 = false;
						for (int num33 = 0; num33 < 10; num33++)
						{
							for (num31 %= Main.maxTilesX; !Main.tile[num31, num32].HasTile; num32++)
							{
							}
							if (Main.tile[num31, num32].TileType == 53)
							{
								flag5 = true;
							}
							array[num33] = num31;
							array2[num33] = num32 - WorldGen.genRand.Next(11, 16);
							num31 += WorldGen.genRand.Next(5, 11);
						}
					}
					while (flag5);
					GenVars.tunnelX[GenVars.numTunnels] = array[5];
					GenVars.numTunnels++;
					for (int num34 = 0; num34 < 10; num34++)
					{
						WorldGen.TileRunner(array[num34], array2[num34], WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(6, 9), 0, addTile: true, -2.0, -0.3);
						WorldGen.TileRunner(array[num34], array2[num34], WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(6, 9), 0, addTile: true, 2.0, -0.3);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Mount Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				GenVars.numMCaves = 0;
				progress.Message = Lang.gen[2].Value;
				int num35 = (int)((double)Main.maxTilesX * 0.001);
				if (WorldGen.remixWorldGen)
				{
					num35 = (int)((double)num35 * 1.5);
				}
				for (int num36 = 0; num36 < num35; num36++)
				{
					progress.Set((double)num36 / (double)num35);
					int num37 = 0;
					bool flag6 = false;
					bool flag7 = false;
					int num38 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
					while (!flag7)
					{
						flag7 = true;
						if (!WorldGen.remixWorldGen)
						{
							while (num38 > Main.maxTilesX / 2 - 90 && num38 < Main.maxTilesX / 2 + 90)
							{
								num38 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
							}
						}
						for (int num39 = 0; num39 < GenVars.numMCaves; num39++)
						{
							if (Math.Abs(num38 - GenVars.mCaveX[num39]) < 100)
							{
								num37++;
								flag7 = false;
								break;
							}
						}
						if (num37 >= Main.maxTilesX / 5)
						{
							flag6 = true;
							break;
						}
					}
					if (!flag6)
					{
						for (int num40 = 0; (double)num40 < Main.worldSurface; num40++)
						{
							if (Main.tile[num38, num40].HasTile)
							{
								for (int num41 = num38 - 50; num41 < num38 + 50; num41++)
								{
									for (int num42 = num40 - 25; num42 < num40 + 25; num42++)
									{
										if (Main.tile[num41, num42].HasTile && (Main.tile[num41, num42].TileType == 53 || Main.tile[num41, num42].TileType == 151 || Main.tile[num41, num42].TileType == 274))
										{
											flag6 = true;
										}
									}
								}
								if (!flag6)
								{
									WorldGen.Mountinater(num38, num40);
									GenVars.mCaveX[GenVars.numMCaves] = num38;
									GenVars.mCaveY[GenVars.numMCaves] = num40;
									GenVars.numMCaves++;
									break;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dirt Wall Backgrounds", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[3].Value;
				int num43 = 0;
				for (int num44 = 1; num44 < Main.maxTilesX - 1; num44++)
				{
					ushort num45 = 2;
					double value = (double)num44 / (double)Main.maxTilesX;
					progress.Set(value);
					bool flag8 = false;
					num43 += WorldGen.genRand.Next(-1, 2);
					if (num43 < 0)
					{
						num43 = 0;
					}
					if (num43 > 10)
					{
						num43 = 10;
					}
					for (int num46 = 0; (double)num46 < Main.worldSurface + 10.0 && !((double)num46 > Main.worldSurface + (double)num43); num46++)
					{
						if (Main.tile[num44, num46].HasTile)
						{
							num45 = (ushort)((Main.tile[num44, num46].TileType != 147) ? 2u : 40u);
						}
						if (flag8 && Main.tile[num44, num46].WallType != 64)
						{
							Main.tile[num44, num46].WallType = num45;
						}
						if (Main.tile[num44, num46].HasTile && Main.tile[num44 - 1, num46].HasTile && Main.tile[num44 + 1, num46].HasTile && Main.tile[num44, num46 + 1].HasTile && Main.tile[num44 - 1, num46 + 1].HasTile && Main.tile[num44 + 1, num46 + 1].HasTile)
						{
							flag8 = true;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Rocks In Dirt", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[4].Value;
				double num47 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.00015;
				for (int num48 = 0; (double)num48 < num47; num48++)
				{
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow + 1), WorldGen.genRand.Next(4, 15), WorldGen.genRand.Next(5, 40), 1);
				}
				progress.Set(0.34);
				num47 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.0002;
				for (int num49 = 0; (double)num49 < num47; num49++)
				{
					int num50 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num51 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh + 1);
					if (!Main.tile[num50, num51 - 10].HasTile)
					{
						num51 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh + 1);
					}
					WorldGen.TileRunner(num50, num51, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1);
				}
				progress.Set(0.67);
				num47 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.0045;
				for (int num52 = 0; (double)num52 < num47; num52++)
				{
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh + 1), WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 23), 1);
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dirt In Rocks", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[5].Value;
				double num53 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.005;
				for (int num54 = 0; (double)num54 < num53; num54++)
				{
					progress.Set((double)num54 / num53);
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 0);
				}
				if (WorldGen.remixWorldGen)
				{
					for (int num55 = 0; num55 < Main.maxTilesX; num55++)
					{
						for (int num56 = (int)Main.worldSurface + WorldGen.genRand.Next(-1, 3); num56 < Main.maxTilesY; num56++)
						{
							if (Main.tile[num55, num56].HasTile)
							{
								if (Main.tile[num55, num56].TileType == 0)
								{
									Main.tile[num55, num56].TileType = 1;
								}
								else if (Main.tile[num55, num56].TileType == 1)
								{
									Main.tile[num55, num56].TileType = 0;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Clay", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[6].Value;
				for (int num57 = 0; num57 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num57++)
				{
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow), WorldGen.genRand.Next(4, 14), WorldGen.genRand.Next(10, 50), 40);
				}
				progress.Set(0.25);
				if (WorldGen.remixWorldGen)
				{
					for (int num58 = 0; num58 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 7E-05); num58++)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayer - 25, Main.maxTilesY - 350), WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(5, 50), 40);
					}
				}
				else
				{
					for (int num59 = 0; num59 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05); num59++)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh + 1), WorldGen.genRand.Next(8, 14), WorldGen.genRand.Next(15, 45), 40);
					}
					progress.Set(0.5);
					for (int num60 = 0; num60 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num60++)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh + 1), WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(5, 50), 40);
					}
				}
				progress.Set(0.75);
				for (int num61 = 5; num61 < Main.maxTilesX - 5; num61++)
				{
					for (int num62 = 1; (double)num62 < Main.worldSurface - 1.0; num62++)
					{
						if (Main.tile[num61, num62].HasTile)
						{
							for (int num63 = num62; num63 < num62 + 5; num63++)
							{
								if (Main.tile[num61, num63].TileType == 40)
								{
									Main.tile[num61, num63].TileType = 0;
								}
							}
							break;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Small Holes", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[7].Value;
				double worldSurfaceHigh = GenVars.worldSurfaceHigh;
				for (int num64 = 0; num64 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015); num64++)
				{
					double value2 = (double)num64 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015);
					progress.Set(value2);
					int TileType = -1;
					if (WorldGen.genRand.Next(5) == 0)
					{
						TileType = -2;
					}
					int num65 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num66 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY);
					if (!WorldGen.remixWorldGen && WorldGen.tenthAnniversaryWorldGen)
					{
						while ((double)num65 < (double)Main.maxTilesX * 0.2 && (double)num65 > (double)Main.maxTilesX * 0.8 && (double)num66 < GenVars.worldSurface)
						{
							num65 = WorldGen.genRand.Next(0, Main.maxTilesX);
							num66 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY);
						}
					}
					else
					{
						while (((num65 < GenVars.smallHolesBeachAvoidance || num65 > Main.maxTilesX - GenVars.smallHolesBeachAvoidance) && (double)num66 < worldSurfaceHigh) || ((double)num65 > (double)Main.maxTilesX * 0.45 && (double)num65 < (double)Main.maxTilesX * 0.55 && (double)num66 < GenVars.worldSurface))
						{
							num65 = WorldGen.genRand.Next(0, Main.maxTilesX);
							num66 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY);
						}
					}
					int num67 = WorldGen.genRand.Next(2, 5);
					int num68 = WorldGen.genRand.Next(2, 20);
					if (WorldGen.remixWorldGen && (double)num66 > Main.rockLayer)
					{
						num67 = (int)((double)num67 * 0.8);
						num68 = (int)((double)num68 * 0.9);
					}
					WorldGen.TileRunner(num65, num66, num67, num68, TileType);
					num65 = WorldGen.genRand.Next(0, Main.maxTilesX);
					num66 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY);
					while (((num65 < GenVars.smallHolesBeachAvoidance || num65 > Main.maxTilesX - GenVars.smallHolesBeachAvoidance) && (double)num66 < worldSurfaceHigh) || ((double)num65 > (double)Main.maxTilesX * 0.45 && (double)num65 < (double)Main.maxTilesX * 0.55 && (double)num66 < GenVars.worldSurface))
					{
						num65 = WorldGen.genRand.Next(0, Main.maxTilesX);
						num66 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY);
					}
					num67 = WorldGen.genRand.Next(8, 15);
					num68 = WorldGen.genRand.Next(7, 30);
					if (WorldGen.remixWorldGen && (double)num66 > Main.rockLayer)
					{
						num67 = (int)((double)num67 * 0.7);
						num68 = (int)((double)num68 * 0.9);
					}
					WorldGen.TileRunner(num65, num66, num67, num68, TileType);
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dirt Layer Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[8].Value;
				double worldSurfaceHigh2 = GenVars.worldSurfaceHigh;
				int num69 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05);
				if (WorldGen.remixWorldGen)
				{
					num69 *= 2;
				}
				for (int num70 = 0; num70 < num69; num70++)
				{
					double value3 = (double)num70 / (double)num69;
					progress.Set(value3);
					if (GenVars.rockLayerHigh <= (double)Main.maxTilesY)
					{
						int type2 = -1;
						if (WorldGen.genRand.Next(6) == 0)
						{
							type2 = -2;
						}
						int num71 = WorldGen.genRand.Next(0, Main.maxTilesX);
						int num72 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.rockLayerHigh + 1);
						while (((num71 < GenVars.smallHolesBeachAvoidance || num71 > Main.maxTilesX - GenVars.smallHolesBeachAvoidance) && (double)num72 < worldSurfaceHigh2) || ((double)num71 >= (double)Main.maxTilesX * 0.45 && (double)num71 <= (double)Main.maxTilesX * 0.55 && (double)num72 < Main.worldSurface))
						{
							num71 = WorldGen.genRand.Next(0, Main.maxTilesX);
							num72 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.rockLayerHigh + 1);
						}
						int num73 = WorldGen.genRand.Next(5, 15);
						int num74 = WorldGen.genRand.Next(30, 200);
						if (WorldGen.remixWorldGen)
						{
							num73 = (int)((double)num73 * 1.1);
							num74 = (int)((double)num74 * 1.9);
						}
						WorldGen.TileRunner(num71, num72, num73, num74, type2);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Rock Layer Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[9].Value;
				int num75 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013);
				if (WorldGen.remixWorldGen)
				{
					num75 = (int)((double)num75 * 1.1);
				}
				for (int num76 = 0; num76 < num75; num76++)
				{
					double value4 = (double)num76 / (double)num75;
					progress.Set(value4);
					if (GenVars.rockLayerHigh <= (double)Main.maxTilesY)
					{
						int type3 = -1;
						if (WorldGen.genRand.Next(10) == 0)
						{
							type3 = -2;
						}
						int num77 = WorldGen.genRand.Next(6, 20);
						int num78 = WorldGen.genRand.Next(50, 300);
						if (WorldGen.remixWorldGen)
						{
							num77 = (int)((double)num77 * 0.7);
							num78 = (int)((double)num78 * 0.7);
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY), num77, num78, type3);
					}
				}
				if (WorldGen.remixWorldGen)
				{
					num75 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013 * 0.4);
					for (int num79 = 0; num79 < num75; num79++)
					{
						if (GenVars.rockLayerHigh <= (double)Main.maxTilesY)
						{
							int type4 = -1;
							if (WorldGen.genRand.Next(10) == 0)
							{
								type4 = -2;
							}
							int num80 = WorldGen.genRand.Next(7, 26);
							int steps2 = WorldGen.genRand.Next(50, 200);
							double num81 = (double)WorldGen.genRand.Next(100, 221) * 0.1;
							double num82 = (double)WorldGen.genRand.Next(-10, 11) * 0.02;
							int i2 = WorldGen.genRand.Next(0, Main.maxTilesX);
							int j2 = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
							WorldGen.TileRunner(i2, j2, num80, steps2, type4, addTile: false, num81, num82, noYChange: true);
							WorldGen.TileRunner(i2, j2, num80, steps2, type4, addTile: false, 0.0 - num81, 0.0 - num82, noYChange: true);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Surface Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[10].Value;
				int num83 = (int)((double)Main.maxTilesX * 0.002);
				int num84 = (int)((double)Main.maxTilesX * 0.0007);
				int num85 = (int)((double)Main.maxTilesX * 0.0003);
				if (WorldGen.remixWorldGen)
				{
					num83 *= 3;
					num84 *= 3;
					num85 *= 3;
				}
				for (int num86 = 0; num86 < num83; num86++)
				{
					int num87 = WorldGen.genRand.Next(0, Main.maxTilesX);
					while (((double)num87 > (double)Main.maxTilesX * 0.45 && (double)num87 < (double)Main.maxTilesX * 0.55) || num87 < GenVars.leftBeachEnd + 20 || num87 > GenVars.rightBeachStart - 20)
					{
						num87 = WorldGen.genRand.Next(0, Main.maxTilesX);
					}
					for (int num88 = 0; (double)num88 < GenVars.worldSurfaceHigh; num88++)
					{
						if (Main.tile[num87, num88].HasTile)
						{
							WorldGen.TileRunner(num87, num88, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(5, 50), -1, addTile: false, (double)WorldGen.genRand.Next(-10, 11) * 0.1, 1.0);
							break;
						}
					}
				}
				progress.Set(0.2);
				for (int num89 = 0; num89 < num84; num89++)
				{
					int num90 = WorldGen.genRand.Next(0, Main.maxTilesX);
					while (((double)num90 > (double)Main.maxTilesX * 0.43 && (double)num90 < (double)Main.maxTilesX * 0.5700000000000001) || num90 < GenVars.leftBeachEnd + 20 || num90 > GenVars.rightBeachStart - 20)
					{
						num90 = WorldGen.genRand.Next(0, Main.maxTilesX);
					}
					for (int num91 = 0; (double)num91 < GenVars.worldSurfaceHigh; num91++)
					{
						if (Main.tile[num90, num91].HasTile)
						{
							WorldGen.TileRunner(num90, num91, WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(50, 130), -1, addTile: false, (double)WorldGen.genRand.Next(-10, 11) * 0.1, 2.0);
							break;
						}
					}
				}
				progress.Set(0.4);
				for (int num92 = 0; num92 < num85; num92++)
				{
					int num93 = WorldGen.genRand.Next(0, Main.maxTilesX);
					while (((double)num93 > (double)Main.maxTilesX * 0.4 && (double)num93 < (double)Main.maxTilesX * 0.6) || num93 < GenVars.leftBeachEnd + 20 || num93 > GenVars.rightBeachStart - 20)
					{
						num93 = WorldGen.genRand.Next(0, Main.maxTilesX);
					}
					for (int num94 = 0; (double)num94 < GenVars.worldSurfaceHigh; num94++)
					{
						if (Main.tile[num93, num94].HasTile)
						{
							WorldGen.TileRunner(num93, num94, WorldGen.genRand.Next(12, 25), WorldGen.genRand.Next(150, 500), -1, addTile: false, (double)WorldGen.genRand.Next(-10, 11) * 0.1, 4.0);
							WorldGen.TileRunner(num93, num94, WorldGen.genRand.Next(8, 17), WorldGen.genRand.Next(60, 200), -1, addTile: false, (double)WorldGen.genRand.Next(-10, 11) * 0.1, 2.0);
							WorldGen.TileRunner(num93, num94, WorldGen.genRand.Next(5, 13), WorldGen.genRand.Next(40, 170), -1, addTile: false, (double)WorldGen.genRand.Next(-10, 11) * 0.1, 2.0);
							break;
						}
					}
				}
				progress.Set(0.6);
				for (int num95 = 0; num95 < (int)((double)Main.maxTilesX * 0.0004); num95++)
				{
					int num96 = WorldGen.genRand.Next(0, Main.maxTilesX);
					while (((double)num96 > (double)Main.maxTilesX * 0.4 && (double)num96 < (double)Main.maxTilesX * 0.6) || num96 < GenVars.leftBeachEnd + 20 || num96 > GenVars.rightBeachStart - 20)
					{
						num96 = WorldGen.genRand.Next(0, Main.maxTilesX);
					}
					for (int num97 = 0; (double)num97 < GenVars.worldSurfaceHigh; num97++)
					{
						if (Main.tile[num96, num97].HasTile)
						{
							WorldGen.TileRunner(num96, num97, WorldGen.genRand.Next(7, 12), WorldGen.genRand.Next(150, 250), -1, addTile: false, 0.0, 1.0, noYChange: true);
							break;
						}
					}
				}
				progress.Set(0.8);
				double num98 = (double)Main.maxTilesX / 4200.0;
				for (int num99 = 0; (double)num99 < 5.0 * num98; num99++)
				{
					try
					{
						int num100 = (int)Main.rockLayer;
						int num101 = Main.maxTilesY - 400;
						if (num100 >= num101)
						{
							num100 = num101 - 1;
						}
						WorldGen.Caverer(WorldGen.genRand.Next(GenVars.surfaceCavesBeachAvoidance2, Main.maxTilesX - GenVars.surfaceCavesBeachAvoidance2), WorldGen.genRand.Next(num100, num101));
					}
					catch
					{
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Wavy Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (WorldGen.dontStarveWorldGen)
				{
					progress.Message = Language.GetTextValue("WorldGeneration.WavyCaves");
					double num102 = (double)Main.maxTilesX / 4200.0;
					num102 *= num102;
					int num103 = (int)(35.0 * num102);
					if (Main.remixWorld)
					{
						num103 /= 3;
					}
					int num104 = 0;
					int num105 = 80;
					for (int num106 = 0; num106 < num103; num106++)
					{
						double num107 = (double)num106 / (double)(num103 - 1);
						progress.Set(num107);
						int num108 = WorldGen.genRand.Next((int)Main.worldSurface + 100, Main.UnderworldLayer - 100);
						int num109 = 0;
						while (Math.Abs(num108 - num104) < num105)
						{
							num109++;
							if (num109 > 100)
							{
								break;
							}
							num108 = WorldGen.genRand.Next((int)Main.worldSurface + 100, Main.UnderworldLayer - 100);
						}
						num104 = num108;
						int num110 = 80;
						int startX = num110 + (int)((double)(Main.maxTilesX - num110 * 2) * num107);
						try
						{
							WorldGen.WavyCaverer(startX, num108, 12 + WorldGen.genRand.Next(3, 6), 0.25 + WorldGen.genRand.NextDouble(), WorldGen.genRand.Next(300, 500), -1);
						}
						catch
						{
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Generate Ice Biome", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[56].Value;
				GenVars.snowTop = (int)Main.worldSurface;
				int num111 = GenVars.lavaLine - WorldGen.genRand.Next(160, 200);
				int num112 = GenVars.lavaLine;
				if (WorldGen.remixWorldGen)
				{
					num112 = Main.maxTilesY - 250;
					num111 = num112 - WorldGen.genRand.Next(160, 200);
				}
				int num113 = GenVars.snowOriginLeft;
				int num114 = GenVars.snowOriginRight;
				int num115 = 10;
				for (int num116 = 0; num116 <= num112 - 140; num116++)
				{
					progress.Set((double)num116 / (double)(num112 - 140));
					num113 += WorldGen.genRand.Next(-4, 4);
					num114 += WorldGen.genRand.Next(-3, 5);
					if (num116 > 0)
					{
						num113 = (num113 + GenVars.snowMinX[num116 - 1]) / 2;
						num114 = (num114 + GenVars.snowMaxX[num116 - 1]) / 2;
					}
					if (GenVars.dungeonSide > 0)
					{
						if (WorldGen.genRand.Next(4) == 0)
						{
							num113++;
							num114++;
						}
					}
					else if (WorldGen.genRand.Next(4) == 0)
					{
						num113--;
						num114--;
					}
					GenVars.snowMinX[num116] = num113;
					GenVars.snowMaxX[num116] = num114;
					for (int num117 = num113; num117 < num114; num117++)
					{
						if (num116 < num111)
						{
							if (Main.tile[num117, num116].WallType == 2)
							{
								Main.tile[num117, num116].WallType = 40;
							}
							switch (Main.tile[num117, num116].TileType)
							{
							case 0:
							case 2:
							case 23:
							case 40:
							case 53:
								Main.tile[num117, num116].TileType = 147;
								break;
							case 1:
								Main.tile[num117, num116].TileType = 161;
								break;
							}
						}
						else
						{
							num115 += WorldGen.genRand.Next(-3, 4);
							if (WorldGen.genRand.Next(3) == 0)
							{
								num115 += WorldGen.genRand.Next(-4, 5);
								if (WorldGen.genRand.Next(3) == 0)
								{
									num115 += WorldGen.genRand.Next(-6, 7);
								}
							}
							if (num115 < 0)
							{
								num115 = WorldGen.genRand.Next(3);
							}
							else if (num115 > 50)
							{
								num115 = 50 - WorldGen.genRand.Next(3);
							}
							for (int num118 = num116; num118 < num116 + num115; num118++)
							{
								if (Main.tile[num117, num118].WallType == 2)
								{
									Main.tile[num117, num118].WallType = 40;
								}
								switch (Main.tile[num117, num118].TileType)
								{
								case 0:
								case 2:
								case 23:
								case 40:
								case 53:
									Main.tile[num117, num118].TileType = 147;
									break;
								case 1:
									Main.tile[num117, num118].TileType = 161;
									break;
								}
							}
						}
					}
					if (GenVars.snowBottom < num116)
					{
						GenVars.snowBottom = num116;
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Grass", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				double num119 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.002;
				for (int num120 = 0; (double)num120 < num119; num120++)
				{
					progress.Set((double)num120 / num119);
					int num121 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
					int num122 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh);
					if (num122 >= Main.maxTilesY)
					{
						num122 = Main.maxTilesY - 2;
					}
					if (Main.tile[num121 - 1, num122].HasTile && Main.tile[num121 - 1, num122].TileType == 0 && Main.tile[num121 + 1, num122].HasTile && Main.tile[num121 + 1, num122].TileType == 0 && Main.tile[num121, num122 - 1].HasTile && Main.tile[num121, num122 - 1].TileType == 0 && Main.tile[num121, num122 + 1].HasTile && Main.tile[num121, num122 + 1].TileType == 0)
					{
						var tile = Main.tile[num121, num122];
						tile.HasTile = true;
						Main.tile[num121, num122].TileType = 2;
					}
					num121 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
					num122 = WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow);
					if (num122 >= Main.maxTilesY)
					{
						num122 = Main.maxTilesY - 2;
					}
					if (Main.tile[num121 - 1, num122].HasTile && Main.tile[num121 - 1, num122].TileType == 0 && Main.tile[num121 + 1, num122].HasTile && Main.tile[num121 + 1, num122].TileType == 0 && Main.tile[num121, num122 - 1].HasTile && Main.tile[num121, num122 - 1].TileType == 0 && Main.tile[num121, num122 + 1].HasTile && Main.tile[num121, num122 + 1].TileType == 0)
					{
						var tile = Main.tile[num121, num122];
						tile.HasTile = true;
						Main.tile[num121, num122].TileType = 2;
					}
				}
			}));
			vanillaGenPasses.Add(new JunglePass());
			vanillaGenPasses.Add(new PassLegacy("Mud Caves To Grass", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[77].Value;
				NotTheBees();
				for (int num123 = 0; num123 < Main.maxTilesX; num123++)
				{
					for (int num124 = 0; num124 < Main.maxTilesY; num124++)
					{
						if (Main.tile[num123, num124].HasTile)
						{
							WorldGen.grassSpread = 0;
							WorldGen.SpreadGrass(num123, num124, 59, 60);
						}
						progress.Set(0.2 * ((double)(num123 * Main.maxTilesY + num124) / (double)(Main.maxTilesX * Main.maxTilesY)));
					}
				}
				WorldGen.SmallConsecutivesFound = 0;
				WorldGen.SmallConsecutivesEliminated = 0;
				double num125 = Main.maxTilesX - 20;
				for (int num126 = 10; num126 < Main.maxTilesX - 10; num126++)
				{
					ScanTileColumnAndRemoveClumps(num126);
					double num127 = (double)(num126 - 10) / num125;
					progress.Set(0.2 + num127 * 0.8);
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Full Desert", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[78].Value;
				Main.tileSolid[484] = false;
				int num128 = 0;
				int num129 = GenVars.dungeonSide;
				int num130 = Main.maxTilesX / 2;
				int num131 = WorldGen.genRand.Next(num130) / 8;
				num131 += num130 / 8;
				int x = num130 + num131 * -num129;
				int num132 = 0;
				DesertBiome desertBiome = GenVars.configuration.CreateBiome<DesertBiome>();
				while (!desertBiome.Place(new Point(x, (int)GenVars.worldSurfaceHigh + 25), GenVars.structures))
				{
					num131 = WorldGen.genRand.Next(num130) / 2;
					num131 += num130 / 8;
					num131 += WorldGen.genRand.Next(num132 / 12);
					x = num130 + num131 * -num129;
					if (++num132 > Main.maxTilesX / 4)
					{
						num129 *= -1;
						num132 = 0;
						num128++;
						if (num128 >= 2)
						{
							GenVars.skipDesertTileCheck = true;
						}
					}
				}
				if (WorldGen.remixWorldGen)
				{
					for (int num133 = 50; num133 < Main.maxTilesX - 50; num133++)
					{
						for (int num134 = (int)Main.rockLayer + WorldGen.genRand.Next(-1, 2); num134 < Main.maxTilesY - 50; num134++)
						{
							if ((Main.tile[num133, num134].TileType == 396 || Main.tile[num133, num134].TileType == 397 || Main.tile[num133, num134].TileType == 53) && !WorldGen.SolidTile(num133, num134 - 1))
							{
								for (int num135 = num134; num135 < num134 + WorldGen.genRand.Next(4, 7) && Main.tile[num133, num135 + 1].HasTile && (Main.tile[num133, num135].TileType == 396 || Main.tile[num133, num135].TileType == 397); num135++)
								{
									Main.tile[num133, num135].TileType = 53;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Floating Islands", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				GenVars.numIslandHouses = 0;
				GenVars.skyIslandHouseCount = 0;
				progress.Message = Lang.gen[12].Value;
				int num136 = (int)((double)Main.maxTilesX * 0.0008);
				int num137 = 0;
				double num138 = num136 + GenVars.skyLakes;
				for (int num139 = 0; (double)num139 < num138; num139++)
				{
					progress.Set((double)num139 / num138);
					int num140 = Main.maxTilesX;
					while (--num140 > 0)
					{
						bool flag9 = true;
						int num141 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
						while (num141 > Main.maxTilesX / 2 - 150 && num141 < Main.maxTilesX / 2 + 150)
						{
							num141 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
						}
						for (int num142 = 0; num142 < GenVars.numIslandHouses; num142++)
						{
							if (num141 > GenVars.floatingIslandHouseX[num142] - 180 && num141 < GenVars.floatingIslandHouseX[num142] + 180)
							{
								flag9 = false;
								break;
							}
						}
						if (flag9)
						{
							flag9 = false;
							int num143 = 0;
							for (int num144 = 200; (double)num144 < Main.worldSurface; num144++)
							{
								if (Main.tile[num141, num144].HasTile)
								{
									num143 = num144;
									flag9 = true;
									break;
								}
							}
							if (flag9)
							{
								int num145 = 0;
								num140 = -1;
								int val = WorldGen.genRand.Next(90, num143 - 100);
								val = Math.Min(val, (int)GenVars.worldSurfaceLow - 50);
								if (num137 >= num136)
								{
									GenVars.skyLake[GenVars.numIslandHouses] = true;
									WorldGen.CloudLake(num141, val);
								}
								else
								{
									GenVars.skyLake[GenVars.numIslandHouses] = false;
									if (WorldGen.drunkWorldGen && !WorldGen.remixWorldGen)
									{
										if (WorldGen.genRand.Next(2) == 0)
										{
											num145 = 3;
											WorldGen.SnowCloudIsland(num141, val);
										}
										else
										{
											num145 = 1;
											WorldGen.DesertCloudIsland(num141, val);
										}
									}
									else
									{
										if (WorldGen.remixWorldGen && WorldGen.drunkWorldGen)
										{
											num145 = ((GenVars.crimsonLeft && num141 < Main.maxTilesX / 2) ? 5 : ((GenVars.crimsonLeft || num141 <= Main.maxTilesX / 2) ? 4 : 5));
										}
										else if (WorldGen.getGoodWorldGen || WorldGen.remixWorldGen)
										{
											num145 = ((!WorldGen.crimson) ? 4 : 5);
										}
										else if (Main.tenthAnniversaryWorld)
										{
											num145 = 6;
										}
										WorldGen.CloudIsland(num141, val);
									}
								}
								GenVars.floatingIslandHouseX[GenVars.numIslandHouses] = num141;
								GenVars.floatingIslandHouseY[GenVars.numIslandHouses] = val;
								GenVars.floatingIslandStyle[GenVars.numIslandHouses] = num145;
								GenVars.numIslandHouses++;
								num137++;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Mushroom Patches", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				//IL_02a9: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_02b4: Unknown result TileType (might be due to invalid IL or missing references)
				progress.Message = Lang.gen[13].Value;
				if (WorldGen.remixWorldGen)
				{
					for (int num146 = 10; num146 < Main.maxTilesX - 10; num146++)
					{
						for (int num147 = Main.maxTilesY + WorldGen.genRand.Next(3) - 350; num147 < Main.maxTilesY - 10; num147++)
						{
							if (Main.tile[num146, num147].TileType == 0)
							{
								Main.tile[num146, num147].TileType = 59;
							}
						}
					}
				}
				double num148 = (double)Main.maxTilesX / 700.0;
				if (num148 > (double)GenVars.maxMushroomBiomes)
				{
					num148 = GenVars.maxMushroomBiomes;
				}
				for (int num149 = 0; (double)num149 < num148; num149++)
				{
					int num150 = 0;
					bool flag10 = true;
					while (flag10)
					{
						int num151 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
						if (num150 > Main.maxTilesX / 4)
						{
							num151 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.975));
						}
						int num152 = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next((int)Main.rockLayer + 50, Main.maxTilesY - 300) : WorldGen.genRand.Next((int)Main.worldSurface + 50, (int)Main.rockLayer - 50));
						flag10 = false;
						int num153 = 100;
						int num154 = 500;
						for (int num155 = num151 - num153; num155 < num151 + num153; num155 += 3)
						{
							for (int num156 = num152 - num153; num156 < num152 + num153; num156 += 3)
							{
								if (WorldGen.InWorld(num155, num156))
								{
									if (Main.tile[num155, num156].TileType == 147 || Main.tile[num155, num156].TileType == 161 || Main.tile[num155, num156].TileType == 162 || Main.tile[num155, num156].TileType == 60 || Main.tile[num155, num156].TileType == 368 || Main.tile[num155, num156].TileType == 367)
									{
										flag10 = true;
										break;
									}
									if (GenVars.UndergroundDesertLocation.Contains(new Point(num155, num156)))
									{
										flag10 = true;
										break;
									}
								}
								else
								{
									flag10 = true;
								}
							}
						}
						if (!flag10)
						{
							for (int num157 = 0; num157 < GenVars.numMushroomBiomes; num157++)
							{
								if (Vector2D.Distance(GenVars.mushroomBiomesPosition[num157].ToVector2D(), new Vector2D((double)num151, (double)num152)) < (double)num154)
								{
									flag10 = true;
								}
							}
						}
						if (!flag10 && GenVars.numMushroomBiomes < GenVars.maxMushroomBiomes)
						{
							WorldGen.ShroomPatch(num151, num152);
							for (int num158 = 0; num158 < 5; num158++)
							{
								int i3 = num151 + WorldGen.genRand.Next(-40, 41);
								int j3 = num152 + WorldGen.genRand.Next(-40, 41);
								WorldGen.ShroomPatch(i3, j3);
							}
							GenVars.mushroomBiomesPosition[GenVars.numMushroomBiomes].X = num151;
							GenVars.mushroomBiomesPosition[GenVars.numMushroomBiomes].Y = num152;
							GenVars.numMushroomBiomes++;
						}
						num150++;
						if (num150 > Main.maxTilesX / 2)
						{
							break;
						}
					}
				}
				for (int num159 = 0; num159 < Main.maxTilesX; num159++)
				{
					progress.Set((double)num159 / (double)Main.maxTilesX);
					for (int num160 = (int)Main.worldSurface; num160 < Main.maxTilesY; num160++)
					{
						if (WorldGen.InWorld(num159, num160, 50) && Main.tile[num159, num160].HasTile)
						{
							WorldGen.grassSpread = 0;
							WorldGen.SpreadGrass(num159, num160, 59, 70, repeat: false);
						}
					}
				}
				for (int num161 = 0; num161 < Main.maxTilesX; num161++)
				{
					for (int num162 = (int)Main.worldSurface; num162 < Main.maxTilesY; num162++)
					{
						if (Main.tile[num161, num162].HasTile && Main.tile[num161, num162].TileType == 70)
						{
							int type5 = 59;
							for (int num163 = num161 - 1; num163 <= num161 + 1; num163++)
							{
								for (int num164 = num162 - 1; num164 <= num162 + 1; num164++)
								{
									if (Main.tile[num163, num164].HasTile)
									{
										if (!Main.tile[num163 - 1, num164].HasTile && !Main.tile[num163 + 1, num164].HasTile)
										{
											WorldGen.KillTile(num163, num164);
										}
										else if (!Main.tile[num163, num164 - 1].HasTile && !Main.tile[num163, num164 + 1].HasTile)
										{
											WorldGen.KillTile(num163, num164);
										}
									}
									else if (Main.tile[num163 - 1, num164].HasTile && Main.tile[num163 + 1, num164].HasTile)
									{
										WorldGen.PlaceTile(num163, num164, type5);
										if (Main.tile[num163 - 1, num162].TileType == 70)
										{
											Main.tile[num163 - 1, num162].TileType = 59;
										}
										if (Main.tile[num163 + 1, num162].TileType == 70)
										{
											Main.tile[num163 + 1, num162].TileType = 59;
										}
									}
									else if (Main.tile[num163, num164 - 1].HasTile && Main.tile[num163, num164 + 1].HasTile)
									{
										WorldGen.PlaceTile(num163, num164, type5);
										if (Main.tile[num163, num162 - 1].TileType == 70)
										{
											Main.tile[num163, num162 - 1].TileType = 59;
										}
										if (Main.tile[num163, num162 + 1].TileType == 70)
										{
											Main.tile[num163, num162 + 1].TileType = 59;
										}
									}
								}
							}
							if (WorldGen.genRand.Next(4) == 0)
							{
								int x2 = num161 + WorldGen.genRand.Next(-20, 21);
								int y = num162 + WorldGen.genRand.Next(-20, 21);
								if (WorldGen.InWorld(x2, y) && Main.tile[x2, y].TileType == 59)
								{
									Main.tile[x2, y].TileType = 70;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Marble", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[80].Value;
				int num165 = passConfig.Get<WorldGenRange>("Count").GetRandom(WorldGen.genRand);
				double num166 = (double)(Main.maxTilesX - 160) / (double)num165;
				MarbleBiome marbleBiome = GenVars.configuration.CreateBiome<MarbleBiome>();
				int num167 = 0;
				int num168 = 0;
				while (num168 < num165)
				{
					double num169 = (double)num168 / (double)num165;
					progress.Set(num169);
					Point origin2 = WorldGen.RandomRectanglePoint((int)(num169 * (double)(Main.maxTilesX - 160)) + 80, (int)GenVars.rockLayer + 20, (int)num166, Main.maxTilesY - ((int)GenVars.rockLayer + 40) - 200);
					if (WorldGen.remixWorldGen)
					{
						origin2 = WorldGen.RandomRectanglePoint((int)(num169 * (double)(Main.maxTilesX - 160)) + 80, (int)GenVars.worldSurface + 100, (int)num166, (int)GenVars.rockLayer - (int)GenVars.worldSurface - 100);
					}
					while ((double)origin2.X > (double)Main.maxTilesX * 0.45 && (double)origin2.X < (double)Main.maxTilesX * 0.55)
					{
						origin2.X = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
					}
					num167++;
					if (marbleBiome.Place(origin2, GenVars.structures))
					{
						num168++;
						num167 = 0;
					}
					else if (num167 > Main.maxTilesX * 10)
					{
						num165 = num168;
						num168++;
						num167 = 0;
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Granite", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[81].Value;
				int num170 = passConfig.Get<WorldGenRange>("Count").GetRandom(WorldGen.genRand);
				double num171 = (double)(Main.maxTilesX - 200) / (double)num170;
				List<Point> list3 = new List<Point>(num170);
				int num172 = 0;
				int num173 = 0;
				while (num173 < num170)
				{
					double num174 = (double)num173 / (double)num170;
					progress.Set(num174);
					Point point = WorldGen.RandomRectanglePoint((int)(num174 * (double)(Main.maxTilesX - 200)) + 100, (int)GenVars.rockLayer + 20, (int)num171, Main.maxTilesY - ((int)GenVars.rockLayer + 40) - 200);
					if (WorldGen.remixWorldGen)
					{
						point = WorldGen.RandomRectanglePoint((int)(num174 * (double)(Main.maxTilesX - 200)) + 100, (int)GenVars.worldSurface + 100, (int)num171, (int)GenVars.rockLayer - (int)GenVars.worldSurface - 100);
					}
					while ((double)point.X > (double)Main.maxTilesX * 0.45 && (double)point.X < (double)Main.maxTilesX * 0.55)
					{
						point.X = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
					}
					num172++;
					if (GraniteBiome.CanPlace(point, GenVars.structures))
					{
						list3.Add(point);
						num173++;
					}
					else if (num172 > Main.maxTilesX * 10)
					{
						num170 = num173;
						num173++;
						num172 = 0;
					}
				}
				GraniteBiome graniteBiome = GenVars.configuration.CreateBiome<GraniteBiome>();
				for (int num175 = 0; num175 < num170; num175++)
				{
					graniteBiome.Place(list3[num175], GenVars.structures);
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dirt To Mud", delegate (GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[14].Value;
				double num176 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.001;
				for (int num177 = 0; (double)num177 < num176; num177++)
				{
					progress.Set((double)num177 / num176);
					if (WorldGen.remixWorldGen)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)GenVars.rockLayerLow), WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 59, addTile: false, 0.0, 0.0, noYChange: false, overRide: true, 53);
					}
					else
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 59, addTile: false, 0.0, 0.0, noYChange: false, overRide: true, 53);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Silt", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[15].Value;
				for (int num178 = 0; num178 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0001); num178++)
				{
					int num179 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num180 = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
					if (WorldGen.remixWorldGen)
					{
						num180 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
					}
					if (Main.tile[num179, num180].WallType != 187 && Main.tile[num179, num180].WallType != 216)
					{
						WorldGen.TileRunner(num179, num180, WorldGen.genRand.Next(5, 12), WorldGen.genRand.Next(15, 50), 123);
					}
				}
				for (int num181 = 0; num181 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0005); num181++)
				{
					int num182 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num183 = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
					if (WorldGen.remixWorldGen)
					{
						num183 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
					}
					if (Main.tile[num182, num183].WallType != 187 && Main.tile[num182, num183].WallType != 216)
					{
						WorldGen.TileRunner(num182, num183, WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 5), 123);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Shinies", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[16].Value;
				if (WorldGen.remixWorldGen)
				{
					for (int num184 = 0; num184 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); num184++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.copper = 7;
							}
							else
							{
								GenVars.copper = 166;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), GenVars.copper);
					}
					for (int num185 = 0; num185 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num185++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.copper = 7;
							}
							else
							{
								GenVars.copper = 166;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), GenVars.copper);
					}
					for (int num186 = 0; num186 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num186++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.copper = 7;
							}
							else
							{
								GenVars.copper = 166;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.copper);
					}
					for (int num187 = 0; num187 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num187++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.iron = 6;
							}
							else
							{
								GenVars.iron = 167;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(2, 5), GenVars.iron);
					}
					for (int num188 = 0; num188 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num188++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.iron = 6;
							}
							else
							{
								GenVars.iron = 167;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), GenVars.iron);
					}
					for (int num189 = 0; num189 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num189++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.iron = 6;
							}
							else
							{
								GenVars.iron = 167;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.iron);
					}
					for (int num190 = 0; num190 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.6E-05); num190++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.silver = 9;
							}
							else
							{
								GenVars.silver = 168;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer - 100, Main.maxTilesY - 250), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), GenVars.silver);
					}
					for (int num191 = 0; num191 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015); num191++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.silver = 9;
							}
							else
							{
								GenVars.silver = 168;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.silver);
					}
					for (int num192 = 0; num192 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num192++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.silver = 9;
							}
							else
							{
								GenVars.silver = 168;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.silver);
					}
					for (int num193 = 0; num193 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num193++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.gold = 8;
							}
							else
							{
								GenVars.gold = 169;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), GenVars.gold);
					}
					for (int num194 = 0; num194 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num194++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.gold = 8;
							}
							else
							{
								GenVars.gold = 169;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow - 20), WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), GenVars.gold);
					}
					if (WorldGen.drunkWorldGen)
					{
						for (int num195 = 0; num195 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.25E-05 / 2.0); num195++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 204);
						}
						for (int num196 = 0; num196 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.25E-05 / 2.0); num196++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 22);
						}
					}
					if (WorldGen.crimson)
					{
						for (int num197 = 0; num197 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 4.25E-05); num197++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 204);
						}
					}
					else
					{
						for (int num198 = 0; num198 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 4.25E-05); num198++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 22);
						}
					}
				}
				else
				{
					for (int num199 = 0; num199 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); num199++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.copper = 7;
							}
							else
							{
								GenVars.copper = 166;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), GenVars.copper);
					}
					for (int num200 = 0; num200 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num200++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.copper = 7;
							}
							else
							{
								GenVars.copper = 166;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), GenVars.copper);
					}
					for (int num201 = 0; num201 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num201++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.copper = 7;
							}
							else
							{
								GenVars.copper = 166;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.copper);
					}
					for (int num202 = 0; num202 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num202++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.iron = 6;
							}
							else
							{
								GenVars.iron = 167;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurfaceHigh), WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(2, 5), GenVars.iron);
					}
					for (int num203 = 0; num203 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num203++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.iron = 6;
							}
							else
							{
								GenVars.iron = 167;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), GenVars.iron);
					}
					for (int num204 = 0; num204 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num204++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.iron = 6;
							}
							else
							{
								GenVars.iron = 167;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.iron);
					}
					for (int num205 = 0; num205 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.6E-05); num205++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.silver = 9;
							}
							else
							{
								GenVars.silver = 168;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), GenVars.silver);
					}
					for (int num206 = 0; num206 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015); num206++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.silver = 9;
							}
							else
							{
								GenVars.silver = 168;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.silver);
					}
					for (int num207 = 0; num207 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num207++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.silver = 9;
							}
							else
							{
								GenVars.silver = 168;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow), WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), GenVars.silver);
					}
					for (int num208 = 0; num208 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num208++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.gold = 8;
							}
							else
							{
								GenVars.gold = 169;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), GenVars.gold);
					}
					for (int num209 = 0; num209 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num209++)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								GenVars.gold = 8;
							}
							else
							{
								GenVars.gold = 169;
							}
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)GenVars.worldSurfaceLow - 20), WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), GenVars.gold);
					}
					if (WorldGen.drunkWorldGen)
					{
						for (int num210 = 0; num210 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.25E-05 / 2.0); num210++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 204);
						}
						for (int num211 = 0; num211 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.25E-05 / 2.0); num211++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 22);
						}
					}
					if (WorldGen.crimson)
					{
						for (int num212 = 0; num212 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.25E-05); num212++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 204);
						}
					}
					else
					{
						for (int num213 = 0; num213 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.25E-05); num213++)
						{
							WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(4, 8), 22);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Webs", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[17].Value;
				for (int num214 = 0; num214 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0006); num214++)
				{
					int num215 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num216 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY - 20);
					if (num214 < GenVars.numMCaves)
					{
						num215 = GenVars.mCaveX[num214];
						num216 = GenVars.mCaveY[num214];
					}
					if (!Main.tile[num215, num216].HasTile && ((double)num216 > Main.worldSurface || Main.tile[num215, num216].WallType > 0))
					{
						while (!Main.tile[num215, num216].HasTile && num216 > (int)GenVars.worldSurfaceLow)
						{
							num216--;
						}
						num216++;
						int num217 = 1;
						if (WorldGen.genRand.Next(2) == 0)
						{
							num217 = -1;
						}
						for (; !Main.tile[num215, num216].HasTile && num215 > 10 && num215 < Main.maxTilesX - 10; num215 += num217)
						{
						}
						num215 -= num217;
						if ((double)num216 > Main.worldSurface || Main.tile[num215, num216].WallType > 0)
						{
							WorldGen.TileRunner(num215, num216, WorldGen.genRand.Next(4, 11), WorldGen.genRand.Next(2, 4), 51, addTile: true, num217, -1.0, noYChange: false, overRide: false);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Underworld", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[18].Value;
				progress.Set(0.0);
				int num218 = Main.maxTilesY - WorldGen.genRand.Next(150, 190);
				for (int num219 = 0; num219 < Main.maxTilesX; num219++)
				{
					num218 += WorldGen.genRand.Next(-3, 4);
					if (num218 < Main.maxTilesY - 190)
					{
						num218 = Main.maxTilesY - 190;
					}
					if (num218 > Main.maxTilesY - 160)
					{
						num218 = Main.maxTilesY - 160;
					}
					for (int num220 = num218 - 20 - WorldGen.genRand.Next(3); num220 < Main.maxTilesY; num220++)
					{
						if (num220 >= num218)
						{
							var tile = Main.tile[num219, num220];
							tile.HasTile = false;
							SetLiquidToLava(Main.tile[num219, num220], false);
							Main.tile[num219, num220].LiquidAmount = 0;
						}
						else
						{
							Main.tile[num219, num220].TileType = 57;
						}
					}
				}
				int num221 = Main.maxTilesY - WorldGen.genRand.Next(40, 70);
				for (int num222 = 10; num222 < Main.maxTilesX - 10; num222++)
				{
					num221 += WorldGen.genRand.Next(-10, 11);
					if (num221 > Main.maxTilesY - 60)
					{
						num221 = Main.maxTilesY - 60;
					}
					if (num221 < Main.maxTilesY - 100)
					{
						num221 = Main.maxTilesY - 120;
					}
					for (int num223 = num221; num223 < Main.maxTilesY - 10; num223++)
					{
						if (!Main.tile[num222, num223].HasTile)
						{
							SetLiquidToLava(Main.tile[num222, num223], true);
							Main.tile[num222, num223].LiquidAmount = byte.MaxValue;
						}
					}
				}
				for (int num224 = 0; num224 < Main.maxTilesX; num224++)
				{
					if (WorldGen.genRand.Next(50) == 0)
					{
						int num225 = Main.maxTilesY - 65;
						while (!Main.tile[num224, num225].HasTile && num225 > Main.maxTilesY - 135)
						{
							num225--;
						}
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), num225 + WorldGen.genRand.Next(20, 50), WorldGen.genRand.Next(15, 20), 1000, 57, addTile: true, 0.0, WorldGen.genRand.Next(1, 3), noYChange: true);
					}
				}
				Liquid.QuickWater(-2);
				for (int num226 = 0; num226 < Main.maxTilesX; num226++)
				{
					double num227 = (double)num226 / (double)(Main.maxTilesX - 1);
					progress.Set(num227 / 2.0 + 0.5);
					if (WorldGen.genRand.Next(13) == 0)
					{
						int num228 = Main.maxTilesY - 65;
						while ((Main.tile[num226, num228].LiquidAmount > 0 || Main.tile[num226, num228].HasTile) && num228 > Main.maxTilesY - 140)
						{
							num228--;
						}
						if ((!WorldGen.drunkWorldGen && !WorldGen.remixWorldGen) || WorldGen.genRand.Next(3) == 0 || !((double)num226 > (double)Main.maxTilesX * 0.4) || !((double)num226 < (double)Main.maxTilesX * 0.6))
						{
							WorldGen.TileRunner(num226, num228 - WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(5, 30), 1000, 57, addTile: true, 0.0, WorldGen.genRand.Next(1, 3), noYChange: true);
						}
						double num229 = WorldGen.genRand.Next(1, 3);
						if (WorldGen.genRand.Next(3) == 0)
						{
							num229 *= 0.5;
						}
						if ((!WorldGen.drunkWorldGen && !WorldGen.remixWorldGen) || WorldGen.genRand.Next(3) == 0 || !((double)num226 > (double)Main.maxTilesX * 0.4) || !((double)num226 < (double)Main.maxTilesX * 0.6))
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								WorldGen.TileRunner(num226, num228 - WorldGen.genRand.Next(2, 5), (int)((double)WorldGen.genRand.Next(5, 15) * num229), (int)((double)WorldGen.genRand.Next(10, 15) * num229), 57, addTile: true, 1.0, 0.3);
							}
							if (WorldGen.genRand.Next(2) == 0)
							{
								num229 = WorldGen.genRand.Next(1, 3);
								WorldGen.TileRunner(num226, num228 - WorldGen.genRand.Next(2, 5), (int)((double)WorldGen.genRand.Next(5, 15) * num229), (int)((double)WorldGen.genRand.Next(10, 15) * num229), 57, addTile: true, -1.0, 0.3);
							}
						}
						WorldGen.TileRunner(num226 + WorldGen.genRand.Next(-10, 10), num228 + WorldGen.genRand.Next(-10, 10), WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(5, 10), -2, addTile: false, WorldGen.genRand.Next(-1, 3), WorldGen.genRand.Next(-1, 3));
						if (WorldGen.genRand.Next(3) == 0)
						{
							WorldGen.TileRunner(num226 + WorldGen.genRand.Next(-10, 10), num228 + WorldGen.genRand.Next(-10, 10), WorldGen.genRand.Next(10, 30), WorldGen.genRand.Next(10, 20), -2, addTile: false, WorldGen.genRand.Next(-1, 3), WorldGen.genRand.Next(-1, 3));
						}
						if (WorldGen.genRand.Next(5) == 0)
						{
							WorldGen.TileRunner(num226 + WorldGen.genRand.Next(-15, 15), num228 + WorldGen.genRand.Next(-15, 10), WorldGen.genRand.Next(15, 30), WorldGen.genRand.Next(5, 20), -2, addTile: false, WorldGen.genRand.Next(-1, 3), WorldGen.genRand.Next(-1, 3));
						}
					}
				}
				for (int num230 = 0; num230 < Main.maxTilesX; num230++)
				{
					WorldGen.TileRunner(WorldGen.genRand.Next(20, Main.maxTilesX - 20), WorldGen.genRand.Next(Main.maxTilesY - 180, Main.maxTilesY - 10), WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 7), -2);
				}
				if (WorldGen.drunkWorldGen || WorldGen.remixWorldGen)
				{
					for (int num231 = 0; num231 < Main.maxTilesX * 2; num231++)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.35), (int)((double)Main.maxTilesX * 0.65)), WorldGen.genRand.Next(Main.maxTilesY - 180, Main.maxTilesY - 10), WorldGen.genRand.Next(5, 20), WorldGen.genRand.Next(5, 10), -2);
					}
				}
				for (int num232 = 0; num232 < Main.maxTilesX; num232++)
				{
					if (!Main.tile[num232, Main.maxTilesY - 145].HasTile)
					{
						Main.tile[num232, Main.maxTilesY - 145].LiquidAmount = byte.MaxValue;
						SetLiquidToLava(Main.tile[num232, Main.maxTilesY - 145], true);
					}
					if (!Main.tile[num232, Main.maxTilesY - 144].HasTile)
					{
						Main.tile[num232, Main.maxTilesY - 144].LiquidAmount = byte.MaxValue;
						SetLiquidToLava(Main.tile[num232, Main.maxTilesY - 144], true);
					}
				}
				for (int num233 = 0; num233 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num233++)
				{
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(3, 7), 58);
				}
				if (WorldGen.remixWorldGen)
				{
					int num234 = (int)((double)Main.maxTilesX * 0.38);
					int num235 = (int)((double)Main.maxTilesX * 0.62);
					int num236 = num234;
					int num237 = Main.maxTilesY - 1;
					int num238 = Main.maxTilesY - 135;
					int num239 = Main.maxTilesY - 160;
					bool flag11 = false;
					Liquid.QuickWater(-2);
					for (; num237 < Main.maxTilesY - 1 || num236 < num235; num236++)
					{
						if (!flag11)
						{
							num237 -= WorldGen.genRand.Next(1, 4);
							if (num237 < num238)
							{
								flag11 = true;
							}
						}
						else if (num236 >= num235)
						{
							num237 += WorldGen.genRand.Next(1, 4);
							if (num237 > Main.maxTilesY - 1)
							{
								num237 = Main.maxTilesY - 1;
							}
						}
						else
						{
							if ((num236 <= Main.maxTilesX / 2 - 5 || num236 >= Main.maxTilesX / 2 + 5) && WorldGen.genRand.Next(4) == 0)
							{
								if (WorldGen.genRand.Next(3) == 0)
								{
									num237 += WorldGen.genRand.Next(-1, 2);
								}
								else if (WorldGen.genRand.Next(6) == 0)
								{
									num237 += WorldGen.genRand.Next(-2, 3);
								}
								else if (WorldGen.genRand.Next(8) == 0)
								{
									num237 += WorldGen.genRand.Next(-4, 5);
								}
							}
							if (num237 < num239)
							{
								num237 = num239;
							}
							if (num237 > num238)
							{
								num237 = num238;
							}
						}
						for (int num240 = num237; num240 > num237 - 20; num240--)
						{
							Main.tile[num236, num240].LiquidAmount = 0;
						}
						for (int num241 = num237; num241 < Main.maxTilesY; num241++)
						{
							Main.tile[num236, num241].Clear(TileDataType.All);
							var tile = Main.tile[num236, num241];
							tile.HasTile = true;
							Main.tile[num236, num241].TileType = 57;
						}
					}
					Liquid.QuickWater(-2);
					for (int num242 = num234; num242 < num235 + 15; num242++)
					{
						for (int num243 = Main.maxTilesY - 300; num243 < num238 + 20; num243++)
						{
							Main.tile[num242, num243].LiquidAmount = 0;
							if (Main.tile[num242, num243].TileType == 57 && Main.tile[num242, num243].HasTile && (!Main.tile[num242 - 1, num243 - 1].HasTile || !Main.tile[num242, num243 - 1].HasTile || !Main.tile[num242 + 1, num243 - 1].HasTile || !Main.tile[num242 - 1, num243].HasTile || !Main.tile[num242 + 1, num243].HasTile || !Main.tile[num242 - 1, num243 + 1].HasTile || !Main.tile[num242, num243 + 1].HasTile || !Main.tile[num242 + 1, num243 + 1].HasTile))
							{
								Main.tile[num242, num243].TileType = 633;
							}
						}
					}
					for (int num244 = num234; num244 < num235 + 15; num244++)
					{
						for (int num245 = Main.maxTilesY - 200; num245 < num238 + 20; num245++)
						{
							if (Main.tile[num244, num245].TileType == 633 && Main.tile[num244, num245].HasTile && !Main.tile[num244, num245 - 1].HasTile && WorldGen.genRand.Next(3) == 0)
							{
								WorldGen.TryGrowingTreeByType(634, num244, num245);
							}
						}
					}
				}
				else if (!WorldGen.drunkWorldGen)
				{
					for (int num246 = 25; num246 < Main.maxTilesX - 25; num246++)
					{
						if ((double)num246 < (double)Main.maxTilesX * 0.17 || (double)num246 > (double)Main.maxTilesX * 0.83)
						{
							for (int num247 = Main.maxTilesY - 300; num247 < Main.maxTilesY - 100 + WorldGen.genRand.Next(-1, 2); num247++)
							{
								if (Main.tile[num246, num247].TileType == 57 && Main.tile[num246, num247].HasTile && (!Main.tile[num246 - 1, num247 - 1].HasTile || !Main.tile[num246, num247 - 1].HasTile || !Main.tile[num246 + 1, num247 - 1].HasTile || !Main.tile[num246 - 1, num247].HasTile || !Main.tile[num246 + 1, num247].HasTile || !Main.tile[num246 - 1, num247 + 1].HasTile || !Main.tile[num246, num247 + 1].HasTile || !Main.tile[num246 + 1, num247 + 1].HasTile))
								{
									Main.tile[num246, num247].TileType = 633;
								}
							}
						}
					}
					for (int num248 = 25; num248 < Main.maxTilesX - 25; num248++)
					{
						if ((double)num248 < (double)Main.maxTilesX * 0.17 || (double)num248 > (double)Main.maxTilesX * 0.83)
						{
							for (int num249 = Main.maxTilesY - 200; num249 < Main.maxTilesY - 50; num249++)
							{
								if (Main.tile[num248, num249].TileType == 633 && Main.tile[num248, num249].HasTile && !Main.tile[num248, num249 - 1].HasTile && WorldGen.genRand.Next(3) == 0)
								{
									WorldGen.TryGrowingTreeByType(634, num248, num249);
								}
							}
						}
					}
				}
				WorldGen.AddHellHouses();
				if (WorldGen.drunkWorldGen)
				{
					for (int num250 = 25; num250 < Main.maxTilesX - 25; num250++)
					{
						for (int num251 = Main.maxTilesY - 300; num251 < Main.maxTilesY - 100 + WorldGen.genRand.Next(-1, 2); num251++)
						{
							if (Main.tile[num250, num251].TileType == 57 && Main.tile[num250, num251].HasTile && (!Main.tile[num250 - 1, num251 - 1].HasTile || !Main.tile[num250, num251 - 1].HasTile || !Main.tile[num250 + 1, num251 - 1].HasTile || !Main.tile[num250 - 1, num251].HasTile || !Main.tile[num250 + 1, num251].HasTile || !Main.tile[num250 - 1, num251 + 1].HasTile || !Main.tile[num250, num251 + 1].HasTile || !Main.tile[num250 + 1, num251 + 1].HasTile))
							{
								Main.tile[num250, num251].TileType = 633;
							}
						}
					}
					for (int num252 = 25; num252 < Main.maxTilesX - 25; num252++)
					{
						for (int num253 = Main.maxTilesY - 200; num253 < Main.maxTilesY - 50; num253++)
						{
							if (Main.tile[num252, num253].TileType == 633 && Main.tile[num252, num253].HasTile && !Main.tile[num252, num253 - 1].HasTile && WorldGen.genRand.Next(3) == 0)
							{
								WorldGen.TryGrowingTreeByType(634, num252, num253);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Corruption", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				int num254 = Main.maxTilesX;
				int num255 = 0;
				int num256 = Main.maxTilesX;
				int num257 = 0;
				for (int num258 = 0; num258 < Main.maxTilesX; num258++)
				{
					for (int num259 = 0; (double)num259 < Main.worldSurface; num259++)
					{
						if (Main.tile[num258, num259].HasTile)
						{
							if (Main.tile[num258, num259].TileType == 60)
							{
								if (num258 < num254)
								{
									num254 = num258;
								}
								if (num258 > num255)
								{
									num255 = num258;
								}
							}
							else if (Main.tile[num258, num259].TileType == 147 || Main.tile[num258, num259].TileType == 161)
							{
								if (num258 < num256)
								{
									num256 = num258;
								}
								if (num258 > num257)
								{
									num257 = num258;
								}
							}
						}
					}
				}
				int num260 = 10;
				num254 -= num260;
				num255 += num260;
				num256 -= num260;
				num257 += num260;
				int num261 = 500;
				int num262 = 100;
				bool flag12 = WorldGen.crimson;
				double num263 = (double)Main.maxTilesX * 0.00045;
				if (WorldGen.remixWorldGen)
				{
					num263 *= 2.0;
				}
				else if (WorldGen.tenthAnniversaryWorldGen)
				{
					num261 *= 2;
					num262 *= 2;
				}
				if (WorldGen.drunkWorldGen)
				{
					flag12 = true;
					num263 /= 2.0;
				}
				if (flag12)
				{
					progress.Message = Lang.gen[72].Value;
					for (int num264 = 0; (double)num264 < num263; num264++)
					{
						int num265 = num256;
						int num266 = num257;
						int num267 = num254;
						int num268 = num255;
						double value5 = (double)num264 / num263;
						progress.Set(value5);
						bool flag13 = false;
						int num269 = 0;
						int num270 = 0;
						int num271 = 0;
						while (!flag13)
						{
							flag13 = true;
							int num272 = Main.maxTilesX / 2;
							int num273 = 200;
							if (WorldGen.drunkWorldGen)
							{
								num273 = 100;
								num269 = ((!GenVars.crimsonLeft) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.5), Main.maxTilesX - num261) : WorldGen.genRand.Next(num261, (int)((double)Main.maxTilesX * 0.5)));
							}
							else
							{
								num269 = WorldGen.genRand.Next(num261, Main.maxTilesX - num261);
							}
							num270 = num269 - WorldGen.genRand.Next(200) - 100;
							num271 = num269 + WorldGen.genRand.Next(200) + 100;
							if (num270 < GenVars.evilBiomeBeachAvoidance)
							{
								num270 = GenVars.evilBiomeBeachAvoidance;
							}
							if (num271 > Main.maxTilesX - GenVars.evilBiomeBeachAvoidance)
							{
								num271 = Main.maxTilesX - GenVars.evilBiomeBeachAvoidance;
							}
							if (num269 < num270 + GenVars.evilBiomeAvoidanceMidFixer)
							{
								num269 = num270 + GenVars.evilBiomeAvoidanceMidFixer;
							}
							if (num269 > num271 - GenVars.evilBiomeAvoidanceMidFixer)
							{
								num269 = num271 - GenVars.evilBiomeAvoidanceMidFixer;
							}
							if (GenVars.dungeonSide < 0 && num270 < 400)
							{
								num270 = 400;
							}
							else if (GenVars.dungeonSide > 0 && num270 > Main.maxTilesX - 400)
							{
								num270 = Main.maxTilesX - 400;
							}
							if (num270 < GenVars.dungeonLocation + num262 && num271 > GenVars.dungeonLocation - num262)
							{
								flag13 = false;
							}
							if (!WorldGen.remixWorldGen)
							{
								if (!WorldGen.tenthAnniversaryWorldGen)
								{
									if (num269 > num272 - num273 && num269 < num272 + num273)
									{
										flag13 = false;
									}
									if (num270 > num272 - num273 && num270 < num272 + num273)
									{
										flag13 = false;
									}
									if (num271 > num272 - num273 && num271 < num272 + num273)
									{
										flag13 = false;
									}
								}
								if (num269 > GenVars.UndergroundDesertLocation.X && num269 < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width)
								{
									flag13 = false;
								}
								if (num270 > GenVars.UndergroundDesertLocation.X && num270 < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width)
								{
									flag13 = false;
								}
								if (num271 > GenVars.UndergroundDesertLocation.X && num271 < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width)
								{
									flag13 = false;
								}
								if (num270 < num266 && num271 > num265)
								{
									num265++;
									num266--;
									flag13 = false;
								}
								if (num270 < num268 && num271 > num267)
								{
									num267++;
									num268--;
									flag13 = false;
								}
							}
						}
						WorldGen.CrimStart(num269, (int)GenVars.worldSurfaceLow - 10);
						for (int num274 = num270; num274 < num271; num274++)
						{
							for (int num275 = (int)GenVars.worldSurfaceLow; (double)num275 < Main.worldSurface - 1.0; num275++)
							{
								if (Main.tile[num274, num275].HasTile)
								{
									int num276 = num275 + WorldGen.genRand.Next(10, 14);
									for (int num277 = num275; num277 < num276; num277++)
									{
										if (Main.tile[num274, num277].TileType == 60 && num274 >= num270 + WorldGen.genRand.Next(5) && num274 < num271 - WorldGen.genRand.Next(5))
										{
											Main.tile[num274, num277].TileType = 662;
										}
									}
									break;
								}
							}
						}
						double num278 = Main.worldSurface + 40.0;
						for (int num279 = num270; num279 < num271; num279++)
						{
							num278 += (double)WorldGen.genRand.Next(-2, 3);
							if (num278 < Main.worldSurface + 30.0)
							{
								num278 = Main.worldSurface + 30.0;
							}
							if (num278 > Main.worldSurface + 50.0)
							{
								num278 = Main.worldSurface + 50.0;
							}
							bool flag14 = false;
							for (int num280 = (int)GenVars.worldSurfaceLow; (double)num280 < num278; num280++)
							{
								if (Main.tile[num279, num280].HasTile)
								{
									if (Main.tile[num279, num280].TileType == 53 && num279 >= num270 + WorldGen.genRand.Next(5) && num279 <= num271 - WorldGen.genRand.Next(5))
									{
										Main.tile[num279, num280].TileType = 234;
									}
									if ((double)num280 < Main.worldSurface - 1.0 && !flag14)
									{
										if (Main.tile[num279, num280].TileType == 0)
										{
											WorldGen.grassSpread = 0;
											WorldGen.SpreadGrass(num279, num280, 0, 199);
										}
										else if (Main.tile[num279, num280].TileType == 59)
										{
											WorldGen.grassSpread = 0;
											WorldGen.SpreadGrass(num279, num280, 59, 662);
										}
									}
									flag14 = true;
									if (Main.tile[num279, num280].WallType == 216)
									{
										Main.tile[num279, num280].WallType = 218;
									}
									else if (Main.tile[num279, num280].WallType == 187)
									{
										Main.tile[num279, num280].WallType = 221;
									}
									if (Main.tile[num279, num280].TileType == 1)
									{
										if (num279 >= num270 + WorldGen.genRand.Next(5) && num279 <= num271 - WorldGen.genRand.Next(5))
										{
											Main.tile[num279, num280].TileType = 203;
										}
									}
									else if (Main.tile[num279, num280].TileType == 2)
									{
										Main.tile[num279, num280].TileType = 199;
									}
									else if (Main.tile[num279, num280].TileType == 60)
									{
										Main.tile[num279, num280].TileType = 662;
									}
									else if (Main.tile[num279, num280].TileType == 161)
									{
										Main.tile[num279, num280].TileType = 200;
									}
									else if (Main.tile[num279, num280].TileType == 396)
									{
										Main.tile[num279, num280].TileType = 401;
									}
									else if (Main.tile[num279, num280].TileType == 397)
									{
										Main.tile[num279, num280].TileType = 399;
									}
								}
							}
						}
						int num281 = WorldGen.genRand.Next(10, 15);
						for (int num282 = 0; num282 < num281; num282++)
						{
							int num283 = 0;
							bool flag15 = false;
							int num284 = 0;
							while (!flag15)
							{
								num283++;
								int x3 = WorldGen.genRand.Next(num270 - num284, num271 + num284);
								int num285 = WorldGen.genRand.Next((int)(Main.worldSurface - (double)(num284 / 2)), (int)(Main.worldSurface + 100.0 + (double)num284));
								while (WorldGen.oceanDepths(x3, num285))
								{
									x3 = WorldGen.genRand.Next(num270 - num284, num271 + num284);
									num285 = WorldGen.genRand.Next((int)(Main.worldSurface - (double)(num284 / 2)), (int)(Main.worldSurface + 100.0 + (double)num284));
								}
								if (num283 > 100)
								{
									num284++;
									num283 = 0;
								}
								if (!Main.tile[x3, num285].HasTile)
								{
									for (; !Main.tile[x3, num285].HasTile; num285++)
									{
									}
									num285--;
								}
								else
								{
									while (Main.tile[x3, num285].HasTile && (double)num285 > Main.worldSurface)
									{
										num285--;
									}
								}
								if ((num284 > 10 || (Main.tile[x3, num285 + 1].HasTile && Main.tile[x3, num285 + 1].TileType == 203)) && !WorldGen.IsTileNearby(x3, num285, 26, 3))
								{
									WorldGen.Place3x2(x3, num285, 26, 1);
									if (Main.tile[x3, num285].TileType == 26)
									{
										flag15 = true;
									}
								}
								if (num284 > 100)
								{
									flag15 = true;
								}
							}
						}
					}
					WorldGen.CrimPlaceHearts();
				}
				if (WorldGen.drunkWorldGen)
				{
					flag12 = false;
				}
				if (!flag12)
				{
					progress.Message = Lang.gen[20].Value;
					for (int num286 = 0; (double)num286 < num263; num286++)
					{
						int num287 = num256;
						int num288 = num257;
						int num289 = num254;
						int num290 = num255;
						double value6 = (double)num286 / num263;
						progress.Set(value6);
						bool flag16 = false;
						int num291 = 0;
						int num292 = 0;
						int num293 = 0;
						while (!flag16)
						{
							flag16 = true;
							int num294 = Main.maxTilesX / 2;
							int num295 = 200;
							num291 = ((!WorldGen.drunkWorldGen) ? WorldGen.genRand.Next(num261, Main.maxTilesX - num261) : (GenVars.crimsonLeft ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.5), Main.maxTilesX - num261) : WorldGen.genRand.Next(num261, (int)((double)Main.maxTilesX * 0.5))));
							num292 = num291 - WorldGen.genRand.Next(200) - 100;
							num293 = num291 + WorldGen.genRand.Next(200) + 100;
							if (num292 < GenVars.evilBiomeBeachAvoidance)
							{
								num292 = GenVars.evilBiomeBeachAvoidance;
							}
							if (num293 > Main.maxTilesX - GenVars.evilBiomeBeachAvoidance)
							{
								num293 = Main.maxTilesX - GenVars.evilBiomeBeachAvoidance;
							}
							if (num291 < num292 + GenVars.evilBiomeAvoidanceMidFixer)
							{
								num291 = num292 + GenVars.evilBiomeAvoidanceMidFixer;
							}
							if (num291 > num293 - GenVars.evilBiomeAvoidanceMidFixer)
							{
								num291 = num293 - GenVars.evilBiomeAvoidanceMidFixer;
							}
							if (num292 < GenVars.dungeonLocation + num262 && num293 > GenVars.dungeonLocation - num262)
							{
								flag16 = false;
							}
							if (!WorldGen.remixWorldGen)
							{
								if (!WorldGen.tenthAnniversaryWorldGen)
								{
									if (num291 > num294 - num295 && num291 < num294 + num295)
									{
										flag16 = false;
									}
									if (num292 > num294 - num295 && num292 < num294 + num295)
									{
										flag16 = false;
									}
									if (num293 > num294 - num295 && num293 < num294 + num295)
									{
										flag16 = false;
									}
								}
								if (num291 > GenVars.UndergroundDesertLocation.X && num291 < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width)
								{
									flag16 = false;
								}
								if (num292 > GenVars.UndergroundDesertLocation.X && num292 < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width)
								{
									flag16 = false;
								}
								if (num293 > GenVars.UndergroundDesertLocation.X && num293 < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width)
								{
									flag16 = false;
								}
								if (num292 < num288 && num293 > num287)
								{
									num287++;
									num288--;
									flag16 = false;
								}
								if (num292 < num290 && num293 > num289)
								{
									num289++;
									num290--;
									flag16 = false;
								}
							}
						}
						int num296 = 0;
						for (int num297 = num292; num297 < num293; num297++)
						{
							if (num296 > 0)
							{
								num296--;
							}
							if (num297 == num291 || num296 == 0)
							{
								for (int num298 = (int)GenVars.worldSurfaceLow; (double)num298 < Main.worldSurface - 1.0; num298++)
								{
									if (Main.tile[num297, num298].HasTile || Main.tile[num297, num298].WallType > 0)
									{
										if (num297 == num291)
										{
											num296 = 20;
											WorldGen.ChasmRunner(num297, num298, WorldGen.genRand.Next(150) + 150, makeOrb: true);
										}
										else if (WorldGen.genRand.Next(35) == 0 && num296 == 0)
										{
											num296 = 30;
											bool makeOrb = true;
											WorldGen.ChasmRunner(num297, num298, WorldGen.genRand.Next(50) + 50, makeOrb);
										}
										break;
									}
								}
							}
							for (int num299 = (int)GenVars.worldSurfaceLow; (double)num299 < Main.worldSurface - 1.0; num299++)
							{
								if (Main.tile[num297, num299].HasTile)
								{
									int num300 = num299 + WorldGen.genRand.Next(10, 14);
									for (int num301 = num299; num301 < num300; num301++)
									{
										if (Main.tile[num297, num301].TileType == 60 && num297 >= num292 + WorldGen.genRand.Next(5) && num297 < num293 - WorldGen.genRand.Next(5))
										{
											Main.tile[num297, num301].TileType = 661;
										}
									}
									break;
								}
							}
						}
						double num302 = Main.worldSurface + 40.0;
						for (int num303 = num292; num303 < num293; num303++)
						{
							num302 += (double)WorldGen.genRand.Next(-2, 3);
							if (num302 < Main.worldSurface + 30.0)
							{
								num302 = Main.worldSurface + 30.0;
							}
							if (num302 > Main.worldSurface + 50.0)
							{
								num302 = Main.worldSurface + 50.0;
							}
							bool flag17 = false;
							for (int num304 = (int)GenVars.worldSurfaceLow; (double)num304 < num302; num304++)
							{
								if (Main.tile[num303, num304].HasTile)
								{
									if (Main.tile[num303, num304].TileType == 53 && num303 >= num292 + WorldGen.genRand.Next(5) && num303 <= num293 - WorldGen.genRand.Next(5))
									{
										Main.tile[num303, num304].TileType = 112;
									}
									if ((double)num304 < Main.worldSurface - 1.0 && !flag17)
									{
										if (Main.tile[num303, num304].TileType == 0)
										{
											WorldGen.grassSpread = 0;
											WorldGen.SpreadGrass(num303, num304, 0, 23);
										}
										else if (Main.tile[num303, num304].TileType == 59)
										{
											WorldGen.grassSpread = 0;
											WorldGen.SpreadGrass(num303, num304, 59, 661);
										}
									}
									flag17 = true;
									if (Main.tile[num303, num304].WallType == 216)
									{
										Main.tile[num303, num304].WallType = 217;
									}
									else if (Main.tile[num303, num304].WallType == 187)
									{
										Main.tile[num303, num304].WallType = 220;
									}
									if (Main.tile[num303, num304].TileType == 1)
									{
										if (num303 >= num292 + WorldGen.genRand.Next(5) && num303 <= num293 - WorldGen.genRand.Next(5))
										{
											Main.tile[num303, num304].TileType = 25;
										}
									}
									else if (Main.tile[num303, num304].TileType == 2)
									{
										Main.tile[num303, num304].TileType = 23;
									}
									else if (Main.tile[num303, num304].TileType == 60)
									{
										Main.tile[num303, num304].TileType = 661;
									}
									else if (Main.tile[num303, num304].TileType == 161)
									{
										Main.tile[num303, num304].TileType = 163;
									}
									else if (Main.tile[num303, num304].TileType == 396)
									{
										Main.tile[num303, num304].TileType = 400;
									}
									else if (Main.tile[num303, num304].TileType == 397)
									{
										Main.tile[num303, num304].TileType = 398;
									}
								}
							}
						}
						for (int num305 = num292; num305 < num293; num305++)
						{
							for (int num306 = 0; num306 < Main.maxTilesY - 50; num306++)
							{
								if (Main.tile[num305, num306].HasTile && Main.tile[num305, num306].TileType == 31)
								{
									int num307 = num305 - 13;
									int num308 = num305 + 13;
									int num309 = num306 - 13;
									int num310 = num306 + 13;
									for (int num311 = num307; num311 < num308; num311++)
									{
										if (num311 > 10 && num311 < Main.maxTilesX - 10)
										{
											for (int num312 = num309; num312 < num310; num312++)
											{
												if (Math.Abs(num311 - num305) + Math.Abs(num312 - num306) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0 && Main.tile[num311, num312].TileType != 31)
												{
													var tile1 = Main.tile[num311, num312];
													tile1.HasTile = true;
													Main.tile[num311, num312].TileType = 25;
													if (Math.Abs(num311 - num305) <= 1 && Math.Abs(num312 - num306) <= 1)
													{
														var tile2 = Main.tile[num311, num312];
														tile2.HasTile = false;
													}
												}
												if (Main.tile[num311, num312].TileType != 31 && Math.Abs(num311 - num305) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num312 - num306) <= 2 + WorldGen.genRand.Next(3))
												{
													var tile = Main.tile[num311, num312];
													tile.HasTile = false;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Lakes", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[19].Value;
				double num313 = (double)Main.maxTilesX / 4200.0;
				int num314 = WorldGen.genRand.Next((int)(num313 * 3.0), (int)(num313 * 6.0));
				for (int num315 = 0; num315 < num314; num315++)
				{
					int num316 = Main.maxTilesX / 4;
					if (GenVars.numLakes >= GenVars.maxLakes - 1)
					{
						break;
					}
					double value7 = (double)num315 / (double)num314;
					progress.Set(value7);
					while (num316 > 0)
					{
						bool flag18 = false;
						num316--;
						int num317 = WorldGen.genRand.Next(GenVars.lakesBeachAvoidance, Main.maxTilesX - GenVars.lakesBeachAvoidance);
						if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.remixWorldGen)
						{
							num317 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.15), (int)((double)Main.maxTilesX * 0.85));
						}
						else
						{
							while ((double)num317 > (double)Main.maxTilesX * 0.45 && (double)num317 < (double)Main.maxTilesX * 0.55)
							{
								num317 = WorldGen.genRand.Next(GenVars.lakesBeachAvoidance, Main.maxTilesX - GenVars.lakesBeachAvoidance);
							}
						}
						for (int num318 = 0; num318 < GenVars.numLakes; num318++)
						{
							if (Math.Abs(num317 - GenVars.LakeX[num318]) < 150)
							{
								flag18 = true;
								break;
							}
						}
						for (int num319 = 0; num319 < GenVars.numMCaves; num319++)
						{
							if (Math.Abs(num317 - GenVars.mCaveX[num319]) < 100)
							{
								flag18 = true;
								break;
							}
						}
						for (int num320 = 0; num320 < GenVars.numTunnels; num320++)
						{
							if (Math.Abs(num317 - GenVars.tunnelX[num320]) < 100)
							{
								flag18 = true;
								break;
							}
						}
						if (!flag18)
						{
							int num321 = (int)GenVars.worldSurfaceLow - 20;
							while (!Main.tile[num317, num321].HasTile)
							{
								num321++;
								if ((double)num321 >= Main.worldSurface || Main.tile[num317, num321].WallType > 0)
								{
									flag18 = true;
									break;
								}
							}
							if (Main.tile[num317, num321].TileType == 53)
							{
								flag18 = true;
							}
							if (!flag18)
							{
								int num322 = 50;
								for (int num323 = num317 - num322; num323 <= num317 + num322; num323++)
								{
									for (int num324 = num321 - num322; num324 <= num321 + num322; num324++)
									{
										if (Main.tile[num323, num324].TileType == 203 || Main.tile[num323, num324].TileType == 25)
										{
											flag18 = true;
											break;
										}
									}
								}
								if (!flag18)
								{
									int num325 = num321;
									num322 = 20;
									while (!WorldGen.SolidTile(num317 - num322, num321) || !WorldGen.SolidTile(num317 + num322, num321))
									{
										num321++;
										if ((double)num321 > Main.worldSurface - 50.0)
										{
											flag18 = true;
										}
									}
									if (num321 - num325 <= 10)
									{
										num322 = 60;
										for (int num326 = num317 - num322; num326 <= num317 + num322; num326++)
										{
											int y2 = num321 - 20;
											if (Main.tile[num326, y2].HasTile || Main.tile[num326, y2].WallType > 0)
											{
												flag18 = true;
											}
										}
										if (!flag18)
										{
											int num327 = 0;
											for (int num328 = num317 - num322; num328 <= num317 + num322; num328++)
											{
												for (int num329 = num321; num329 <= num321 + num322 * 2; num329++)
												{
													if (WorldGen.SolidTile(num328, num329))
													{
														num327++;
													}
												}
											}
											int num330 = (num322 * 2 + 1) * (num322 * 2 + 1);
											if (!((double)num327 < (double)num330 * 0.8) && !GenVars.UndergroundDesertLocation.Intersects(new Rectangle(num317 - 8, num321 - 8, 16, 16)))
											{
												WorldGen.SonOfLakinater(num317, num321);
												GenVars.LakeX[GenVars.numLakes] = num317;
												GenVars.numLakes++;
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dungeon", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				int dungeonLocation = GenVars.dungeonLocation;
				int num331 = (int)((Main.worldSurface + Main.rockLayer) / 2.0) + WorldGen.genRand.Next(-200, 200);
				int num332 = (int)((Main.worldSurface + Main.rockLayer) / 2.0) + 200;
				int num333 = num331;
				bool flag19 = false;
				for (int num334 = 0; num334 < 10; num334++)
				{
					if (WorldGen.SolidTile(dungeonLocation, num333 + num334))
					{
						flag19 = true;
						break;
					}
				}
				if (!flag19)
				{
					for (; num333 < num332 && !WorldGen.SolidTile(dungeonLocation, num333 + 10); num333++)
					{
					}
				}
				if (WorldGen.drunkWorldGen)
				{
					num333 = (int)Main.worldSurface + 70;
				}
				WorldGen.MakeDungeon(dungeonLocation, num333);
			}));
			vanillaGenPasses.Add(new PassLegacy("Slush", delegate
			{
				for (int num335 = GenVars.snowTop; num335 < GenVars.snowBottom; num335++)
				{
					for (int num336 = GenVars.snowMinX[num335]; num336 < GenVars.snowMaxX[num335]; num336++)
					{
						switch (Main.tile[num336, num335].TileType)
						{
						case 123:
							Main.tile[num336, num335].TileType = 224;
							break;
						case 59:
						{
							bool flag20 = true;
							int num337 = 3;
							for (int num338 = num336 - num337; num338 <= num336 + num337; num338++)
							{
								for (int num339 = num335 - num337; num339 <= num335 + num337; num339++)
								{
									if (Main.tile[num338, num339].TileType == 60 || Main.tile[num338, num339].TileType == 70 || Main.tile[num338, num339].TileType == 71 || Main.tile[num338, num339].TileType == 72)
									{
										flag20 = false;
										break;
									}
								}
							}
							if (flag20)
							{
								Main.tile[num336, num335].TileType = 224;
							}
							break;
						}
						case 1:
							Main.tile[num336, num335].TileType = 161;
							break;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Mountain Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[21].Value;
				for (int num340 = 0; num340 < GenVars.numMCaves; num340++)
				{
					int i4 = GenVars.mCaveX[num340];
					int j4 = GenVars.mCaveY[num340];
					WorldGen.CaveOpenater(i4, j4);
					WorldGen.Cavinator(i4, j4, WorldGen.genRand.Next(40, 50));
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Beaches", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				int num341 = 50;
				progress.Message = Lang.gen[22].Value;
				bool floridaStyle = false;
				bool floridaStyle2 = false;
				if (WorldGen.genRand.Next(4) == 0)
				{
					if (WorldGen.genRand.Next(2) == 0)
					{
						floridaStyle = true;
					}
					else
					{
						floridaStyle2 = true;
					}
				}
				for (int num342 = 0; num342 < 2; num342++)
				{
					int num343 = 0;
					int num344 = 0;
					if (num342 == 0)
					{
						num343 = 0;
						num344 = WorldGen.genRand.Next(GenVars.oceanWaterStartRandomMin, GenVars.oceanWaterStartRandomMax);
						if (GenVars.dungeonSide == 1)
						{
							num344 = GenVars.oceanWaterForcedJungleLength;
						}
						int num345 = GenVars.leftBeachEnd - num341;
						if (num344 > num345)
						{
							num344 = num345;
						}
						int num346 = 0;
						double num347 = 1.0;
						int num348;
						for (num348 = 0; !Main.tile[num344 - 1, num348].HasTile; num348++)
						{
						}
						GenVars.shellStartYLeft = num348;
						num348 += WorldGen.genRand.Next(1, 5);
						for (int num349 = num344 - 1; num349 >= num343; num349--)
						{
							if (num349 > 30)
							{
								num346++;
								num347 = TuneOceanDepth(num346, num347, floridaStyle);
							}
							else
							{
								num347 += 1.0;
							}
							int num350 = WorldGen.genRand.Next(15, 20);
							for (int num351 = 0; (double)num351 < (double)num348 + num347 + (double)num350; num351++)
							{
								if ((double)num351 < (double)num348 + num347 * 0.75 - 3.0)
								{
									var tile = Main.tile[num349, num351];
									tile.HasTile = false;
									if (num351 > num348)
									{
										Main.tile[num349, num351].LiquidAmount = byte.MaxValue;
										SetLiquidToLava(Main.tile[num349, num351], false);
									}
									else if (num351 == num348)
									{
										Main.tile[num349, num351].LiquidAmount = 127;
										if (GenVars.shellStartXLeft == 0)
										{
											GenVars.shellStartXLeft = num349;
										}
									}
								}
								else if (num351 > num348)
								{
									Main.tile[num349, num351].TileType = 53;
									var tile = Main.tile[num349, num351];
									tile.HasTile = true;
								}
								Main.tile[num349, num351].WallType = 0;
							}
						}
					}
					else
					{
						num343 = Main.maxTilesX - WorldGen.genRand.Next(GenVars.oceanWaterStartRandomMin, GenVars.oceanWaterStartRandomMax);
						num344 = Main.maxTilesX;
						if (GenVars.dungeonSide == -1)
						{
							num343 = Main.maxTilesX - GenVars.oceanWaterForcedJungleLength;
						}
						int num352 = GenVars.rightBeachStart + num341;
						if (num343 < num352)
						{
							num343 = num352;
						}
						double num353 = 1.0;
						int num354 = 0;
						int num355;
						for (num355 = 0; !Main.tile[num343, num355].HasTile; num355++)
						{
						}
						GenVars.shellStartXRight = 0;
						GenVars.shellStartYRight = num355;
						num355 += WorldGen.genRand.Next(1, 5);
						for (int num356 = num343; num356 < num344; num356++)
						{
							if (num356 < num344 - 30)
							{
								num354++;
								num353 = TuneOceanDepth(num354, num353, floridaStyle2);
							}
							else
							{
								num353 += 1.0;
							}
							int num357 = WorldGen.genRand.Next(15, 20);
							for (int num358 = 0; (double)num358 < (double)num355 + num353 + (double)num357; num358++)
							{
								if ((double)num358 < (double)num355 + num353 * 0.75 - 3.0)
								{
									var tile = Main.tile[num356, num358];
									tile.HasTile = false;
									if (num358 > num355)
									{
										Main.tile[num356, num358].LiquidAmount = byte.MaxValue;
										SetLiquidToLava(Main.tile[num356, num358], false);
									}
									else if (num358 == num355)
									{
										Main.tile[num356, num358].LiquidAmount = 127;
										if (GenVars.shellStartXRight == 0)
										{
											GenVars.shellStartXRight = num356;
										}
									}
								}
								else if (num358 > num355)
								{
									Main.tile[num356, num358].TileType = 53;
									var tile = Main.tile[num356, num358];
									tile.HasTile = true;
								}
								Main.tile[num356, num358].WallType = 0;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Gems", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[23].Value;
				Main.tileSolid[484] = false;
				for (int num359 = 63; num359 <= 68; num359++)
				{
					double value8 = (double)(num359 - 63) / 6.0;
					progress.Set(value8);
					double num360 = 0.0;
					switch (num359)
					{
					case 67:
						num360 = (double)Main.maxTilesX * 0.5;
						break;
					case 66:
						num360 = (double)Main.maxTilesX * 0.45;
						break;
					case 63:
						num360 = (double)Main.maxTilesX * 0.3;
						break;
					case 65:
						num360 = (double)Main.maxTilesX * 0.25;
						break;
					case 64:
						num360 = (double)Main.maxTilesX * 0.1;
						break;
					case 68:
						num360 = (double)Main.maxTilesX * 0.05;
						break;
					}
					num360 *= 0.2;
					for (int num361 = 0; (double)num361 < num360; num361++)
					{
						int num362 = WorldGen.genRand.Next(0, Main.maxTilesX);
						int num363 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
						while (Main.tile[num362, num363].TileType != 1)
						{
							num362 = WorldGen.genRand.Next(0, Main.maxTilesX);
							num363 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
						}
						WorldGen.TileRunner(num362, num363, WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), num359);
					}
				}
				for (int num364 = 0; num364 < 2; num364++)
				{
					int num365 = 1;
					int num366 = 5;
					int num367 = Main.maxTilesX - 5;
					if (num364 == 1)
					{
						num365 = -1;
						num366 = Main.maxTilesX - 5;
						num367 = 5;
					}
					for (int num368 = num366; num368 != num367; num368 += num365)
					{
						if (num368 <= GenVars.UndergroundDesertLocation.Left || num368 >= GenVars.UndergroundDesertLocation.Right)
						{
							for (int num369 = 10; num369 < Main.maxTilesY - 10; num369++)
							{
								if (Main.tile[num368, num369].HasTile && Main.tile[num368, num369 + 1].HasTile && Main.tileSand[Main.tile[num368, num369].TileType] && Main.tileSand[Main.tile[num368, num369 + 1].TileType])
								{
									ushort type6 = Main.tile[num368, num369].TileType;
									int x4 = num368 + num365;
									int num370 = num369 + 1;
									if (!Main.tile[x4, num369].HasTile && !Main.tile[x4, num370].HasTile)
									{
										for (; !Main.tile[x4, num370].HasTile; num370++)
										{
										}
										num370--;
										var tile = Main.tile[num368, num369];
										tile.HasTile = false;
										tile = Main.tile[x4, num370];
										tile.HasTile = true;
										Main.tile[x4, num370].TileType = type6;
									}
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Gravitating Sand", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[24].Value;
				for (int num371 = 0; num371 < Main.maxTilesX; num371++)
				{
					double value9 = (double)num371 / (double)(Main.maxTilesX - 1);
					progress.Set(value9);
					bool flag21 = false;
					int num372 = 0;
					for (int num373 = Main.maxTilesY - 1; num373 > 0; num373--)
					{
						if (WorldGen.SolidOrSlopedTile(num371, num373))
						{
							ushort type7 = Main.tile[num371, num373].TileType;
							if (flag21 && num373 < (int)Main.worldSurface && num373 != num372 - 1 && TileID.Sets.Falling[type7])
							{
								for (int num374 = num373; num374 < num372; num374++)
								{
									Main.tile[num371, num374].ResetToType(type7);
								}
							}
							flag21 = true;
							num372 = num373;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Create Ocean Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				int maxValue2 = 3;
				if (WorldGen.remixWorldGen)
				{
					maxValue2 = 2;
				}
				for (int num375 = 0; num375 < 2; num375++)
				{
					if ((num375 != 0 || GenVars.dungeonSide <= 0) && (num375 != 1 || GenVars.dungeonSide >= 0) && (WorldGen.genRand.Next(maxValue2) == 0 || WorldGen.drunkWorldGen || WorldGen.tenthAnniversaryWorldGen))
					{
						progress.Message = Lang.gen[90].Value;
						int num376 = WorldGen.genRand.Next(55, 95);
						if (num375 == 1)
						{
							num376 = WorldGen.genRand.Next(Main.maxTilesX - 95, Main.maxTilesX - 55);
						}
						int num377;
						for (num377 = 0; !Main.tile[num376, num377].HasTile; num377++)
						{
						}
						WorldGen.oceanCave(num376, num377);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Shimmer", delegate
			{
				//IL_027f: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_0284: Unknown result TileType (might be due to invalid IL or missing references)
				int num378 = 50;
				int num379 = (int)(Main.worldSurface + Main.rockLayer) / 2 + num378;
				int num380 = (int)((double)((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;
				if (num380 > Main.maxTilesY - 330 - 100 - 30)
				{
					num380 = Main.maxTilesY - 330 - 100 - 30;
				}
				if (num380 <= num379)
				{
					num380 = num379 + 50;
				}
				int num381 = WorldGen.genRand.Next(num379, num380);
				int num382 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.11)));
				int num383 = (int)Main.worldSurface + 150;
				int num384 = (int)(Main.rockLayer + Main.worldSurface + 200.0) / 2;
				if (num384 <= num383)
				{
					num384 = num383 + 50;
				}
				if (WorldGen.tenthAnniversaryWorldGen)
				{
					num381 = WorldGen.genRand.Next(num383, num384);
				}
				int num385 = 0;
				while (!WorldGen.ShimmerMakeBiome(num382, num381))
				{
					num385++;
					if (WorldGen.tenthAnniversaryWorldGen && num385 < 10000)
					{
						num381 = WorldGen.genRand.Next(num383, num384);
						num382 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.11)));
					}
					else if (num385 > 20000)
					{
						num381 = WorldGen.genRand.Next((int)Main.worldSurface + 100 + 20, num380);
						num382 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.2)));
					}
					else
					{
						num381 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 20, num380);
						num382 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.89), Main.maxTilesX - 200) : WorldGen.genRand.Next(200, (int)((double)Main.maxTilesX * 0.11)));
					}
				}
				GenVars.shimmerPosition = new Vector2D((double)num382, (double)num381);
				int num386 = 200;
				GenVars.structures.AddProtectedStructure(new Rectangle(num382 - num386 / 2, num381 - num386 / 2, num386, num386));
			}));
			vanillaGenPasses.Add(new PassLegacy("Clean Up Dirt", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[25].Value;
				for (int num387 = 3; num387 < Main.maxTilesX - 3; num387++)
				{
					double num388 = (double)num387 / (double)Main.maxTilesX;
					progress.Set(0.5 * num388);
					bool flag22 = true;
					for (int num389 = 0; (double)num389 < Main.worldSurface; num389++)
					{
						if (flag22)
						{
							if (Main.tile[num387, num389].WallType == 2 || Main.tile[num387, num389].WallType == 40 || Main.tile[num387, num389].WallType == 64 || Main.tile[num387, num389].WallType == 86)
							{
								Main.tile[num387, num389].WallType = 0;
							}
							if (Main.tile[num387, num389].TileType != 53 && Main.tile[num387, num389].TileType != 112 && Main.tile[num387, num389].TileType != 234)
							{
								if (Main.tile[num387 - 1, num389].WallType == 2 || Main.tile[num387 - 1, num389].WallType == 40 || Main.tile[num387 - 1, num389].WallType == 40)
								{
									Main.tile[num387 - 1, num389].WallType = 0;
								}
								if ((Main.tile[num387 - 2, num389].WallType == 2 || Main.tile[num387 - 2, num389].WallType == 40 || Main.tile[num387 - 2, num389].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num387 - 2, num389].WallType = 0;
								}
								if ((Main.tile[num387 - 3, num389].WallType == 2 || Main.tile[num387 - 3, num389].WallType == 40 || Main.tile[num387 - 3, num389].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num387 - 3, num389].WallType = 0;
								}
								if (Main.tile[num387 + 1, num389].WallType == 2 || Main.tile[num387 + 1, num389].WallType == 40 || Main.tile[num387 + 1, num389].WallType == 40)
								{
									Main.tile[num387 + 1, num389].WallType = 0;
								}
								if ((Main.tile[num387 + 2, num389].WallType == 2 || Main.tile[num387 + 2, num389].WallType == 40 || Main.tile[num387 + 2, num389].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num387 + 2, num389].WallType = 0;
								}
								if ((Main.tile[num387 + 3, num389].WallType == 2 || Main.tile[num387 + 3, num389].WallType == 40 || Main.tile[num387 + 3, num389].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num387 + 3, num389].WallType = 0;
								}
								if (Main.tile[num387, num389].HasTile)
								{
									flag22 = false;
								}
							}
						}
						else if (Main.tile[num387, num389].WallType == 0 && Main.tile[num387, num389 + 1].WallType == 0 && Main.tile[num387, num389 + 2].WallType == 0 && Main.tile[num387, num389 + 3].WallType == 0 && Main.tile[num387, num389 + 4].WallType == 0 && Main.tile[num387 - 1, num389].WallType == 0 && Main.tile[num387 + 1, num389].WallType == 0 && Main.tile[num387 - 2, num389].WallType == 0 && Main.tile[num387 + 2, num389].WallType == 0 && !Main.tile[num387, num389].HasTile && !Main.tile[num387, num389 + 1].HasTile && !Main.tile[num387, num389 + 2].HasTile && !Main.tile[num387, num389 + 3].HasTile)
						{
							flag22 = true;
						}
					}
				}
				for (int num390 = Main.maxTilesX - 5; num390 >= 5; num390--)
				{
					double num391 = (double)num390 / (double)Main.maxTilesX;
					progress.Set(1.0 - 0.5 * num391);
					bool flag23 = true;
					for (int num392 = 0; (double)num392 < Main.worldSurface; num392++)
					{
						if (flag23)
						{
							if (Main.tile[num390, num392].WallType == 2 || Main.tile[num390, num392].WallType == 40 || Main.tile[num390, num392].WallType == 64)
							{
								Main.tile[num390, num392].WallType = 0;
							}
							if (Main.tile[num390, num392].TileType != 53)
							{
								if (Main.tile[num390 - 1, num392].WallType == 2 || Main.tile[num390 - 1, num392].WallType == 40 || Main.tile[num390 - 1, num392].WallType == 40)
								{
									Main.tile[num390 - 1, num392].WallType = 0;
								}
								if ((Main.tile[num390 - 2, num392].WallType == 2 || Main.tile[num390 - 2, num392].WallType == 40 || Main.tile[num390 - 2, num392].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num390 - 2, num392].WallType = 0;
								}
								if ((Main.tile[num390 - 3, num392].WallType == 2 || Main.tile[num390 - 3, num392].WallType == 40 || Main.tile[num390 - 3, num392].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num390 - 3, num392].WallType = 0;
								}
								if (Main.tile[num390 + 1, num392].WallType == 2 || Main.tile[num390 + 1, num392].WallType == 40 || Main.tile[num390 + 1, num392].WallType == 40)
								{
									Main.tile[num390 + 1, num392].WallType = 0;
								}
								if ((Main.tile[num390 + 2, num392].WallType == 2 || Main.tile[num390 + 2, num392].WallType == 40 || Main.tile[num390 + 2, num392].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num390 + 2, num392].WallType = 0;
								}
								if ((Main.tile[num390 + 3, num392].WallType == 2 || Main.tile[num390 + 3, num392].WallType == 40 || Main.tile[num390 + 3, num392].WallType == 40) && WorldGen.genRand.Next(2) == 0)
								{
									Main.tile[num390 + 3, num392].WallType = 0;
								}
								if (Main.tile[num390, num392].HasTile)
								{
									flag23 = false;
								}
							}
						}
						else if (Main.tile[num390, num392].WallType == 0 && Main.tile[num390, num392 + 1].WallType == 0 && Main.tile[num390, num392 + 2].WallType == 0 && Main.tile[num390, num392 + 3].WallType == 0 && Main.tile[num390, num392 + 4].WallType == 0 && Main.tile[num390 - 1, num392].WallType == 0 && Main.tile[num390 + 1, num392].WallType == 0 && Main.tile[num390 - 2, num392].WallType == 0 && Main.tile[num390 + 2, num392].WallType == 0 && !Main.tile[num390, num392].HasTile && !Main.tile[num390, num392 + 1].HasTile && !Main.tile[num390, num392 + 2].HasTile && !Main.tile[num390, num392 + 3].HasTile)
						{
							flag23 = true;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Pyramids", delegate
			{
				Rectangle undergroundDesertLocation = GenVars.UndergroundDesertLocation;
				if (Main.tenthAnniversaryWorld)
				{
					int x5 = undergroundDesertLocation.Center.X;
					int j5 = undergroundDesertLocation.Top - 10;
					WorldGen.Pyramid(x5, j5);
				}
				for (int num393 = 0; num393 < GenVars.numPyr; num393++)
				{
					int num394 = GenVars.PyrX[num393];
					int num395 = GenVars.PyrY[num393];
					if (num394 > 300 && num394 < Main.maxTilesX - 300 && (GenVars.dungeonSide >= 0 || !((double)num394 < (double)GenVars.dungeonX + (double)Main.maxTilesX * 0.15)) && (GenVars.dungeonSide <= 0 || !((double)num394 > (double)GenVars.dungeonX - (double)Main.maxTilesX * 0.15)) && (!Main.tenthAnniversaryWorld || !undergroundDesertLocation.Contains(num394, num395)))
					{
						for (; !Main.tile[num394, num395].HasTile && (double)num395 < Main.worldSurface; num395++)
						{
						}
						if (!((double)num395 >= Main.worldSurface) && Main.tile[num394, num395].TileType == 53)
						{
							int num396 = Main.maxTilesX;
							for (int num397 = 0; num397 < num393; num397++)
							{
								int num398 = Math.Abs(num394 - GenVars.PyrX[num397]);
								if (num398 < num396)
								{
									num396 = num398;
								}
							}
							int num399 = 220;
							if (WorldGen.drunkWorldGen)
							{
								num399 /= 2;
							}
							if (num396 >= num399)
							{
								num395--;
								WorldGen.Pyramid(num394, num395);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dirt Rock Wall Runner", delegate
			{
				for (int num400 = 0; num400 < Main.maxTilesX; num400++)
				{
					int num401 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
					int num402 = WorldGen.genRand.Next(10, (int)Main.worldSurface);
					if (Main.tile[num401, num402].WallType == 2)
					{
						WorldGen.DirtyRockRunner(num401, num402);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Living Trees", delegate
			{
				int num403 = 200;
				double num404 = (double)Main.maxTilesX / 4200.0;
				int num405 = WorldGen.genRand.Next(0, (int)(2.0 * num404) + 1);
				if (num405 == 0 && WorldGen.genRand.Next(2) == 0)
				{
					num405++;
				}
				if (WorldGen.drunkWorldGen)
				{
					num405 += (int)(2.0 * num404);
				}
				else if (Main.tenthAnniversaryWorld)
				{
					num405 += (int)(3.0 * num404);
				}
				else if (WorldGen.remixWorldGen)
				{
					num405 += (int)(2.0 * num404);
				}
				for (int num406 = 0; num406 < num405; num406++)
				{
					bool flag24 = false;
					int num407 = 0;
					while (!flag24)
					{
						num407++;
						if (num407 > Main.maxTilesX / 2)
						{
							flag24 = true;
						}
						int num408 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
						if (WorldGen.tenthAnniversaryWorldGen && !WorldGen.remixWorldGen)
						{
							num408 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.15), (int)((float)Main.maxTilesX * 0.85f));
						}
						if (num408 <= Main.maxTilesX / 2 - num403 || num408 >= Main.maxTilesX / 2 + num403)
						{
							int num409;
							for (num409 = 0; !Main.tile[num408, num409].HasTile && (double)num409 < Main.worldSurface; num409++)
							{
							}
							if (Main.tile[num408, num409].TileType == 0)
							{
								num409--;
								if (num409 > 150)
								{
									bool flag25 = true;
									for (int num410 = num408 - 50; num410 < num408 + 50; num410++)
									{
										for (int num411 = num409 - 50; num411 < num409 + 50; num411++)
										{
											if (Main.tile[num410, num411].HasTile)
											{
												switch (Main.tile[num410, num411].TileType)
												{
												case 41:
												case 43:
												case 44:
												case 189:
												case 196:
												case 460:
												case 481:
												case 482:
												case 483:
													flag25 = false;
													break;
												}
											}
										}
									}
									for (int num412 = 0; num412 < GenVars.numMCaves; num412++)
									{
										if (num408 > GenVars.mCaveX[num412] - 50 && num408 < GenVars.mCaveX[num412] + 50)
										{
											flag25 = false;
											break;
										}
									}
									if (flag25)
									{
										flag24 = WorldGen.GrowLivingTree(num408, num409);
										if (flag24)
										{
											for (int num413 = -1; num413 <= 1; num413++)
											{
												if (num413 != 0)
												{
													int num414 = num408;
													int num415 = WorldGen.genRand.Next(4);
													if (WorldGen.drunkWorldGen || Main.tenthAnniversaryWorld)
													{
														num415 += WorldGen.genRand.Next(2, 5);
													}
													else if (WorldGen.remixWorldGen)
													{
														num415 += WorldGen.genRand.Next(1, 6);
													}
													for (int num416 = 0; num416 < num415; num416++)
													{
														num414 += WorldGen.genRand.Next(13, 31) * num413;
														if (num414 <= Main.maxTilesX / 2 - num403 || num414 >= Main.maxTilesX / 2 + num403)
														{
															int num417 = num409;
															if (Main.tile[num414, num417].HasTile)
															{
																while (Main.tile[num414, num417].HasTile)
																{
																	num417--;
																}
															}
															else
															{
																for (; !Main.tile[num414, num417].HasTile; num417++)
																{
																}
																num417--;
															}
															flag25 = true;
															for (int num418 = num408 - 50; num418 < num408 + 50; num418++)
															{
																for (int num419 = num409 - 50; num419 < num409 + 50; num419++)
																{
																	if (Main.tile[num418, num419].HasTile)
																	{
																		switch (Main.tile[num418, num419].TileType)
																		{
																		case 41:
																		case 43:
																		case 44:
																		case 189:
																		case 196:
																		case 460:
																		case 481:
																		case 482:
																		case 483:
																			flag25 = false;
																			break;
																		}
																	}
																}
															}
															if (flag25)
															{
																WorldGen.GrowLivingTree(num414, num417, patch: true);
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				Main.tileSolid[192] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Wood Tree Walls", delegate
			{
				for (int num420 = 25; num420 < Main.maxTilesX - 25; num420++)
				{
					for (int num421 = 25; (double)num421 < Main.worldSurface; num421++)
					{
						if (Main.tile[num420, num421].TileType == 191 || Main.tile[num420, num421 - 1].TileType == 191 || Main.tile[num420 - 1, num421].TileType == 191 || Main.tile[num420 + 1, num421].TileType == 191 || Main.tile[num420, num421 + 1].TileType == 191)
						{
							bool flag26 = true;
							for (int num422 = num420 - 1; num422 <= num420 + 1; num422++)
							{
								for (int num423 = num421 - 1; num423 <= num421 + 1; num423++)
								{
									if (num422 != num420 && num423 != num421 && Main.tile[num422, num423].TileType != 191 && Main.tile[num422, num423].WallType != 244)
									{
										flag26 = false;
									}
								}
							}
							if (flag26)
							{
								Main.tile[num420, num421].WallType = 244;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Altars", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				//IL_01ff: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_0204: Unknown result TileType (might be due to invalid IL or missing references)
				Main.tileSolid[484] = false;
				progress.Message = Lang.gen[26].Value;
				int num424 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3.3E-06);
				if (WorldGen.remixWorldGen)
				{
					num424 *= 3;
				}
				for (int num425 = 0; num425 < num424; num425++)
				{
					progress.Set((double)num425 / (double)num424);
					for (int num426 = 0; num426 < 10000; num426++)
					{
						int num427 = WorldGen.genRand.Next(281, Main.maxTilesX - 3 - 280);
						while ((double)num427 > (double)Main.maxTilesX * 0.45 && (double)num427 < (double)Main.maxTilesX * 0.55)
						{
							num427 = WorldGen.genRand.Next(281, Main.maxTilesX - 3 - 280);
						}
						int num428 = WorldGen.genRand.Next((int)(Main.worldSurface * 2.0 + Main.rockLayer) / 3, (int)(Main.rockLayer + (double)((Main.maxTilesY - 350) * 2)) / 3);
						if (WorldGen.remixWorldGen)
						{
							num428 = WorldGen.genRand.Next(100, (int)((double)Main.maxTilesY * 0.9));
						}
						while (WorldGen.oceanDepths(num427, num428) || Vector2D.Distance(new Vector2D((double)num427, (double)num428), GenVars.shimmerPosition) < (double)WorldGen.shimmerSafetyDistance)
						{
							num427 = WorldGen.genRand.Next(281, Main.maxTilesX - 3 - 280);
							while ((double)num427 > (double)Main.maxTilesX * 0.45 && (double)num427 < (double)Main.maxTilesX * 0.55)
							{
								num427 = WorldGen.genRand.Next(281, Main.maxTilesX - 3 - 280);
							}
							num428 = WorldGen.genRand.Next((int)(Main.worldSurface * 2.0 + Main.rockLayer) / 3, (int)(Main.rockLayer + (double)((Main.maxTilesY - 350) * 2)) / 3);
							if (WorldGen.remixWorldGen)
							{
								num428 = WorldGen.genRand.Next(100, (int)((double)Main.maxTilesY * 0.9));
							}
						}
						int style = (WorldGen.crimson ? 1 : 0);
						if (WorldGen.drunkWorldGen)
						{
							style = ((GenVars.crimsonLeft ? (num427 < Main.maxTilesX / 2) : (num427 >= Main.maxTilesX / 2)) ? 1 : 0);
						}
						if (!WorldGen.IsTileNearby(num427, num428, 26, 3))
						{
							WorldGen.Place3x2(num427, num428, 26, style);
						}
						if (Main.tile[num427, num428].TileType == 26)
						{
							break;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Wet Jungle", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num429 = 0; num429 < Main.maxTilesX; num429++)
				{
					for (int num430 = (int)GenVars.worldSurfaceLow; (double)num430 < Main.worldSurface - 1.0; num430++)
					{
						if (Main.tile[num429, num430].HasTile)
						{
							if (Main.tile[num429, num430].TileType == 60)
							{
								Main.tile[num429, num430 - 1].LiquidAmount = byte.MaxValue;
								Main.tile[num429, num430 - 2].LiquidAmount = byte.MaxValue;
							}
							break;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Jungle Temple", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				int num431 = 0;
				progress.Message = Lang.gen[70].Value;
				long num432 = 0L;
				double num433 = 0.25;
				bool flag27 = false;
				while (true)
				{
					int num434 = (int)Main.rockLayer;
					int num435 = Main.maxTilesY - 500;
					if (num434 > num435 - 1)
					{
						num434 = num435 - 1;
					}
					int num436 = WorldGen.genRand.Next(num434, num435);
					int x6 = (int)(((WorldGen.genRand.NextDouble() * num433 + 0.1) * (double)(-GenVars.dungeonSide) + 0.5) * (double)Main.maxTilesX);
					if (WorldGen.remixWorldGen)
					{
						if (WorldGen.notTheBees)
						{
							x6 = ((GenVars.dungeonSide <= 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.6), (int)((double)Main.maxTilesX * 0.8)) : WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.4)));
						}
						else
						{
							x6 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
							while ((double)x6 > (double)Main.maxTilesX * 0.4 && (double)x6 < (double)Main.maxTilesX * 0.6)
							{
								x6 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
							}
						}
						while (Main.tile[x6, num436].HasTile || Main.tile[x6, num436].WallType > 0 || (double)num436 > Main.worldSurface - 5.0)
						{
							num436--;
						}
						num436++;
						if (Main.tile[x6, num436].HasTile && (Main.tile[x6, num436].TileType == 60 || Main.tile[x6, num436].TileType == 59))
						{
							int num437 = 10;
							bool flag28 = false;
							for (int num438 = x6 - num437; num438 <= num438 + num437; num438++)
							{
								for (int num439 = num436 - num437; num439 < num437; num439++)
								{
									if (Main.tile[num438, num439].TileType == 191 || Main.tileDungeon[Main.tile[num438, num439].TileType])
									{
										flag28 = true;
									}
								}
							}
							if (!flag28)
							{
								flag27 = true;
								num436 -= 10 + WorldGen.genRand.Next(10);
								WorldGen.makeTemple(x6, num436);
								break;
							}
						}
					}
					else if (Main.tile[x6, num436].HasTile && Main.tile[x6, num436].TileType == 60)
					{
						flag27 = true;
						WorldGen.makeTemple(x6, num436);
						break;
					}
					if (num432++ > 2000000)
					{
						if (num433 == 0.35)
						{
							num431++;
							if (num431 > 10)
							{
								break;
							}
						}
						num433 = Math.Min(0.35, num433 + 0.05);
						num432 = 0L;
					}
				}
				if (!flag27)
				{
					int x7 = Main.maxTilesX - GenVars.dungeonX;
					int y3 = (int)Main.rockLayer + 100;
					if (WorldGen.remixWorldGen)
					{
						x7 = (WorldGen.notTheBees ? ((GenVars.dungeonSide > 0) ? ((int)((double)Main.maxTilesX * 0.3)) : ((int)((double)Main.maxTilesX * 0.7))) : ((GenVars.dungeonSide > 0) ? ((int)((double)Main.maxTilesX * 0.4)) : ((int)((double)Main.maxTilesX * 0.6))));
					}
					WorldGen.makeTemple(x7, y3);
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Hives", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[71].Value;
				double num440 = (double)Main.maxTilesX / 4200.0;
				double num441 = 1 + WorldGen.genRand.Next((int)(5.0 * num440), (int)(8.0 * num440));
				if (WorldGen.drunkWorldGen)
				{
					num441 *= 0.667;
				}
				int num442 = 10000;
				HiveBiome hiveBiome = GenVars.configuration.CreateBiome<HiveBiome>();
				HoneyPatchBiome honeyPatchBiome = GenVars.configuration.CreateBiome<HoneyPatchBiome>();
				while (num441 > 0.0 && num442 > 0)
				{
					num442--;
					Point origin3 = WorldGen.RandomWorldPoint((int)(Main.worldSurface + Main.rockLayer) >> 1, 20, 300, 20);
					if (WorldGen.drunkWorldGen)
					{
						WorldGen.RandomWorldPoint((int)Main.worldSurface, 20, 300, 20);
					}
					if (hiveBiome.Place(origin3, GenVars.structures))
					{
						num441 -= 1.0;
						int num443 = WorldGen.genRand.Next(5);
						int num444 = 0;
						int num445 = 10000;
						while (num444 < num443 && num445 > 0)
						{
							double num446 = WorldGen.genRand.NextDouble() * 60.0 + 30.0;
							double num447 = WorldGen.genRand.NextDouble() * 6.2831854820251465;
							int num448 = (int)(Math.Cos(num447) * num446) + origin3.X;
							int y4 = (int)(Math.Sin(num447) * num446) + origin3.Y;
							num445--;
							if (num448 > 50 && num448 < Main.maxTilesX - 50 && honeyPatchBiome.Place(new Point(num448, y4), GenVars.structures))
							{
								num444++;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Jungle Chests", delegate
			{
				int num449 = WorldGen.genRand.Next(40, Main.maxTilesX - 40);
				int num450 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 400);
				double num451 = WorldGen.genRand.Next(7, 12);
				num451 *= (double)Main.maxTilesX / 4200.0;
				int num452 = 0;
				for (int num453 = 0; (double)num453 < num451; num453++)
				{
					bool flag29 = true;
					while (flag29)
					{
						num452++;
						num449 = WorldGen.genRand.Next(40, Main.maxTilesX / 2 - 40);
						if (GenVars.dungeonSide < 0)
						{
							num449 += Main.maxTilesX / 2;
						}
						num450 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 400);
						int num454 = WorldGen.genRand.Next(2, 4);
						int num455 = WorldGen.genRand.Next(2, 4);
						Rectangle area = new Rectangle(num449 - num454 - 1, num450 - num455 - 1, num454 + 1, num455 + 1);
						if (Main.tile[num449, num450].TileType == 60)
						{
							int num456 = 30;
							flag29 = false;
							for (int num457 = num449 - num456; num457 < num449 + num456; num457 += 3)
							{
								for (int num458 = num450 - num456; num458 < num450 + num456; num458 += 3)
								{
									if (Main.tile[num457, num458].HasTile && (Main.tile[num457, num458].TileType == 225 || Main.tile[num457, num458].TileType == 229 || Main.tile[num457, num458].TileType == 226 || Main.tile[num457, num458].TileType == 119 || Main.tile[num457, num458].TileType == 120))
									{
										flag29 = true;
									}
									if (Main.tile[num457, num458].WallType == 86 || Main.tile[num457, num458].WallType == 87)
									{
										flag29 = true;
									}
								}
							}
							if (!GenVars.structures.CanPlace(area, 1))
							{
								flag29 = true;
							}
						}
						if (!flag29)
						{
							ushort num459 = 0;
							if (GenVars.jungleHut == 119)
							{
								num459 = 23;
							}
							else if (GenVars.jungleHut == 120)
							{
								num459 = 24;
							}
							else if (GenVars.jungleHut == 158)
							{
								num459 = 42;
							}
							else if (GenVars.jungleHut == 175)
							{
								num459 = 45;
							}
							else if (GenVars.jungleHut == 45)
							{
								num459 = 10;
							}
							for (int num460 = num449 - num454 - 1; num460 <= num449 + num454 + 1; num460++)
							{
								for (int num461 = num450 - num455 - 1; num461 <= num450 + num455 + 1; num461++)
								{
									var tile = Main.tile[num460, num461];
									tile.HasTile = true;
									Main.tile[num460, num461].TileType = GenVars.jungleHut;
									Main.tile[num460, num461].LiquidAmount = 0;
									SetLiquidToLava(Main.tile[num460, num461], false);
								}
							}
							for (int num462 = num449 - num454; num462 <= num449 + num454; num462++)
							{
								for (int num463 = num450 - num455; num463 <= num450 + num455; num463++)
								{
									var tile = Main.tile[num462, num463];
									tile.HasTile = false;
									Main.tile[num462, num463].WallType = num459;
								}
							}
							bool flag30 = false;
							int num464 = 0;
							while (!flag30 && num464 < 100)
							{
								num464++;
								int num465 = WorldGen.genRand.Next(num449 - num454, num449 + num454 + 1);
								int num466 = WorldGen.genRand.Next(num450 - num455, num450 + num455 - 2);
								WorldGen.PlaceTile(num465, num466, 4, mute: true, forced: false, -1, 3);
								if (TileID.Sets.Torch[Main.tile[num465, num466].TileType])
								{
									flag30 = true;
								}
							}
							for (int num467 = num449 - num454 - 1; num467 <= num449 + num454 + 1; num467++)
							{
								for (int num468 = num450 + num455 - 2; num468 <= num450 + num455; num468++)
								{
									var tile = Main.tile[num467, num468];
									tile.HasTile = false;
								}
							}
							for (int num469 = num449 - num454 - 1; num469 <= num449 + num454 + 1; num469++)
							{
								for (int num470 = num450 + num455 - 2; num470 <= num450 + num455 - 1; num470++)
								{
									var tile = Main.tile[num469, num470];
									tile.HasTile = false;
								}
							}
							for (int num471 = num449 - num454 - 1; num471 <= num449 + num454 + 1; num471++)
							{
								int num472 = 4;
								int num473 = num450 + num455 + 2;
								while (!Main.tile[num471, num473].HasTile && num473 < Main.maxTilesY && num472 > 0)
								{
									var tile = Main.tile[num471, num473];
									tile.HasTile = true;
									Main.tile[num471, num473].TileType = 59;
									num473++;
									num472--;
								}
							}
							num454 -= WorldGen.genRand.Next(1, 3);
							int num474 = num450 - num455 - 2;
							while (num454 > -1)
							{
								for (int num475 = num449 - num454 - 1; num475 <= num449 + num454 + 1; num475++)
								{
									var tile = Main.tile[num475, num474];
									tile.HasTile = true;
									Main.tile[num475, num474].TileType = GenVars.jungleHut;
								}
								num454 -= WorldGen.genRand.Next(1, 3);
								num474--;
							}
							GenVars.JChestX[GenVars.numJChests] = num449;
							GenVars.JChestY[GenVars.numJChests] = num450;
							GenVars.structures.AddProtectedStructure(area);
							GenVars.numJChests++;
							num452 = 0;
						}
						else if (num452 > Main.maxTilesX * 10)
						{
							num453++;
							num452 = 0;
							break;
						}
					}
				}
				Main.tileSolid[137] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Settle Liquids", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[27].Value;
				if (WorldGen.notTheBees)
				{
					NotTheBees();
				}
				Liquid.worldGenTilesIgnoreWater(ignoreSolids: true);
				Liquid.QuickWater(3);
				WorldGen.WaterCheck();
				int num476 = 0;
				Liquid.quickSettle = true;
				int num477 = 10;
				while (num476 < num477)
				{
					int num478 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					num476++;
					double num479 = 0.0;
					int num480 = num478 * 5;
					while (Liquid.numLiquid > 0)
					{
						num480--;
						if (num480 < 0)
						{
							break;
						}
						double num481 = (double)(num478 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (double)num478;
						if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num478)
						{
							num478 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
						}
						if (num481 > num479)
						{
							num479 = num481;
						}
						else
						{
							num481 = num479;
						}
						if (num476 == 1)
						{
							progress.Set(num481 / 3.0 + 0.33);
						}
						int num482 = 10;
						if (num476 > num482)
						{
							num482 = num476;
						}
						Liquid.UpdateLiquid();
					}
					WorldGen.WaterCheck();
					progress.Set((double)num476 * 0.1 / 3.0 + 0.66);
				}
				Liquid.quickSettle = false;
				Liquid.worldGenTilesIgnoreWater(ignoreSolids: false);
				Main.tileSolid[484] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Remove Water From Sand", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num483 = 400; num483 < Main.maxTilesX - 400; num483++)
				{
					for (int num484 = 100; (double)num484 < Main.worldSurface - 1.0; num484++)
					{
						if (Main.tile[num483, num484].HasTile)
						{
							ushort type8 = Main.tile[num483, num484].TileType;
							if (type8 == 53 || type8 == 396 || type8 == 397 || type8 == 404 || type8 == 407 || type8 == 151)
							{
								int num485 = num484;
								while (num485 > 100)
								{
									num485--;
									if (Main.tile[num483, num485].HasTile)
									{
										break;
									}
									Main.tile[num483, num485].LiquidAmount = 0;
								}
							}
							break;
						}
					}
				}
				Main.tileSolid[192] = true;
			}));
			vanillaGenPasses.Add(new PassLegacy("Oasis", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (!WorldGen.notTheBees)
				{
					progress.Set(1.0);
					int num486 = Main.maxTilesX / 2100;
					num486 += WorldGen.genRand.Next(2);
					for (int num487 = 0; num487 < num486; num487++)
					{
						int num488 = WorldGen.beachDistance + 300;
						int num489 = Main.maxTilesX * 2;
						while (num489 > 0)
						{
							num489--;
							int x8 = WorldGen.genRand.Next(num488, Main.maxTilesX - num488);
							int y5 = WorldGen.genRand.Next(100, (int)Main.worldSurface);
							if (WorldGen.PlaceOasis(x8, y5))
							{
								num489 = -1;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Shell Piles", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (WorldGen.dontStarveWorldGen)
				{
					int num490 = (int)(5.0 * ((double)Main.maxTilesX / 4200.0));
					int num491 = 0;
					int num492 = 100;
					int num493 = Main.maxTilesX / 2;
					int num494 = num493 - num492;
					int num495 = num493 + num492;
					for (int num496 = 0; num496 < 80; num496++)
					{
						int num497 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
						if (num497 >= num494 && num497 <= num495)
						{
							num497 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
							if (num497 >= num494 && num497 <= num495)
							{
								continue;
							}
						}
						int y6 = (int)Main.worldSurface / 2;
						if (WorldGen.MarblePileWithStatues(num497, y6))
						{
							num491++;
							if (num491 >= num490)
							{
								break;
							}
						}
					}
				}
				if (!WorldGen.notTheBees)
				{
					progress.Set(1.0);
					if (WorldGen.genRand.Next(2) == 0)
					{
						int shellStartXLeft = GenVars.shellStartXLeft;
						int shellStartYLeft = GenVars.shellStartYLeft;
						for (int num498 = shellStartXLeft - 20; num498 <= shellStartXLeft + 20; num498++)
						{
							for (int num499 = shellStartYLeft - 10; num499 <= shellStartYLeft + 10; num499++)
							{
								if (Main.tile[num498, num499].HasTile && Main.tile[num498, num499].TileType == 53 && !Main.tile[num498, num499 - 1].HasTile && Main.tile[num498, num499 - 1].LiquidAmount == 0 && !Main.tile[num498 - 1, num499].HasTile && Main.tile[num498 - 1, num499].LiquidAmount > 0)
								{
									GenVars.shellStartXLeft = num498;
									GenVars.shellStartYLeft = num499;
								}
							}
						}
						GenVars.shellStartYLeft -= 50;
						GenVars.shellStartXLeft -= WorldGen.genRand.Next(5);
						if (WorldGen.genRand.Next(2) == 0)
						{
							GenVars.shellStartXLeft -= WorldGen.genRand.Next(10);
						}
						if (WorldGen.genRand.Next(3) == 0)
						{
							GenVars.shellStartXLeft -= WorldGen.genRand.Next(15);
						}
						if (WorldGen.genRand.Next(4) != 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXLeft, GenVars.shellStartYLeft);
						}
						int maxValue3 = WorldGen.genRand.Next(2, 4);
						if (WorldGen.genRand.Next(maxValue3) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXLeft - WorldGen.genRand.Next(10, 35), GenVars.shellStartYLeft);
						}
						if (WorldGen.genRand.Next(maxValue3) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXLeft - WorldGen.genRand.Next(40, 65), GenVars.shellStartYLeft);
						}
						if (WorldGen.genRand.Next(maxValue3) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXLeft - WorldGen.genRand.Next(70, 95), GenVars.shellStartYLeft);
						}
						if (WorldGen.genRand.Next(maxValue3) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXLeft - WorldGen.genRand.Next(100, 125), GenVars.shellStartYLeft);
						}
						if (WorldGen.genRand.Next(maxValue3) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXLeft + WorldGen.genRand.Next(10, 25), GenVars.shellStartYLeft);
						}
					}
					if (WorldGen.genRand.Next(2) == 0)
					{
						int shellStartXRight = GenVars.shellStartXRight;
						int shellStartYRight = GenVars.shellStartYRight;
						for (int num500 = shellStartXRight - 20; num500 <= shellStartXRight + 20; num500++)
						{
							for (int num501 = shellStartYRight - 10; num501 <= shellStartYRight + 10; num501++)
							{
								if (Main.tile[num500, num501].HasTile && Main.tile[num500, num501].TileType == 53 && !Main.tile[num500, num501 - 1].HasTile && Main.tile[num500, num501 - 1].LiquidAmount == 0 && !Main.tile[num500 + 1, num501].HasTile && Main.tile[num500 + 1, num501].LiquidAmount > 0)
								{
									GenVars.shellStartXRight = num500;
									GenVars.shellStartYRight = num501;
								}
							}
						}
						GenVars.shellStartYRight -= 50;
						GenVars.shellStartXRight += WorldGen.genRand.Next(5);
						if (WorldGen.genRand.Next(2) == 0)
						{
							GenVars.shellStartXLeft += WorldGen.genRand.Next(10);
						}
						if (WorldGen.genRand.Next(3) == 0)
						{
							GenVars.shellStartXLeft += WorldGen.genRand.Next(15);
						}
						if (WorldGen.genRand.Next(4) != 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXRight, GenVars.shellStartYRight);
						}
						int maxValue4 = WorldGen.genRand.Next(2, 4);
						if (WorldGen.genRand.Next(maxValue4) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXRight + WorldGen.genRand.Next(10, 35), GenVars.shellStartYRight);
						}
						if (WorldGen.genRand.Next(maxValue4) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXRight + WorldGen.genRand.Next(40, 65), GenVars.shellStartYRight);
						}
						if (WorldGen.genRand.Next(maxValue4) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXRight + WorldGen.genRand.Next(70, 95), GenVars.shellStartYRight);
						}
						if (WorldGen.genRand.Next(maxValue4) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXRight + WorldGen.genRand.Next(100, 125), GenVars.shellStartYRight);
						}
						if (WorldGen.genRand.Next(maxValue4) == 0)
						{
							WorldGen.ShellPile(GenVars.shellStartXRight - WorldGen.genRand.Next(10, 25), GenVars.shellStartYRight);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Smooth World", delegate (GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[60].Value;
				Main.tileSolid[GenVars.crackedType] = true;
				for (int num502 = 20; num502 < Main.maxTilesX - 20; num502++)
				{
					double value10 = (double)num502 / (double)Main.maxTilesX;
					progress.Set(value10);
					for (int num503 = 20; num503 < Main.maxTilesY - 20; num503++)
					{
						if (Main.tile[num502, num503].TileType != 48 && Main.tile[num502, num503].TileType != 137 && Main.tile[num502, num503].TileType != 232 && Main.tile[num502, num503].TileType != 191 && Main.tile[num502, num503].TileType != 151 && Main.tile[num502, num503].TileType != 274)
						{
							if (!Main.tile[num502, num503 - 1].HasTile && Main.tile[num502 - 1, num503].TileType != 136 && Main.tile[num502 + 1, num503].TileType != 136)
							{
								if (WorldGen.SolidTile(num502, num503) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[num502, num503].TileType])
								{
									if (!Main.tile[num502 - 1, num503].IsHalfBlock && !Main.tile[num502 + 1, num503].IsHalfBlock && Main.tile[num502 - 1, num503].Slope == 0 && Main.tile[num502 + 1, num503].Slope == 0)
									{
										if (WorldGen.SolidTile(num502, num503 + 1))
										{
											if (!WorldGen.SolidTile(num502 - 1, num503) && !Main.tile[num502 - 1, num503 + 1].IsHalfBlock && WorldGen.SolidTile(num502 - 1, num503 + 1) && WorldGen.SolidTile(num502 + 1, num503) && !Main.tile[num502 + 1, num503 - 1].HasTile)
											{
												if (WorldGen.genRand.Next(2) == 0)
												{
													WorldGen.SlopeTile(num502, num503, 2);
												}
												else
												{
													WorldGen.PoundTile(num502, num503);
												}
											}
											else if (!WorldGen.SolidTile(num502 + 1, num503) && !Main.tile[num502 + 1, num503 + 1].IsHalfBlock && WorldGen.SolidTile(num502 + 1, num503 + 1) && WorldGen.SolidTile(num502 - 1, num503) && !Main.tile[num502 - 1, num503 - 1].HasTile)
											{
												if (WorldGen.genRand.Next(2) == 0)
												{
													WorldGen.SlopeTile(num502, num503, 1);
												}
												else
												{
													WorldGen.PoundTile(num502, num503);
												}
											}
											else if (WorldGen.SolidTile(num502 + 1, num503 + 1) && WorldGen.SolidTile(num502 - 1, num503 + 1) && !Main.tile[num502 + 1, num503].HasTile && !Main.tile[num502 - 1, num503].HasTile)
											{
												WorldGen.PoundTile(num502, num503);
											}
											if (WorldGen.SolidTile(num502, num503))
											{
												if (WorldGen.SolidTile(num502 - 1, num503) && WorldGen.SolidTile(num502 + 1, num503 + 2) && !Main.tile[num502 + 1, num503].HasTile && !Main.tile[num502 + 1, num503 + 1].HasTile && !Main.tile[num502 - 1, num503 - 1].HasTile)
												{
													WorldGen.KillTile(num502, num503);
												}
												else if (WorldGen.SolidTile(num502 + 1, num503) && WorldGen.SolidTile(num502 - 1, num503 + 2) && !Main.tile[num502 - 1, num503].HasTile && !Main.tile[num502 - 1, num503 + 1].HasTile && !Main.tile[num502 + 1, num503 - 1].HasTile)
												{
													WorldGen.KillTile(num502, num503);
												}
												else if (!Main.tile[num502 - 1, num503 + 1].HasTile && !Main.tile[num502 - 1, num503].HasTile && WorldGen.SolidTile(num502 + 1, num503) && WorldGen.SolidTile(num502, num503 + 2))
												{
													if (WorldGen.genRand.Next(5) == 0)
													{
														WorldGen.KillTile(num502, num503);
													}
													else if (WorldGen.genRand.Next(5) == 0)
													{
														WorldGen.PoundTile(num502, num503);
													}
													else
													{
														WorldGen.SlopeTile(num502, num503, 2);
													}
												}
												else if (!Main.tile[num502 + 1, num503 + 1].HasTile && !Main.tile[num502 + 1, num503].HasTile && WorldGen.SolidTile(num502 - 1, num503) && WorldGen.SolidTile(num502, num503 + 2))
												{
													if (WorldGen.genRand.Next(5) == 0)
													{
														WorldGen.KillTile(num502, num503);
													}
													else if (WorldGen.genRand.Next(5) == 0)
													{
														WorldGen.PoundTile(num502, num503);
													}
													else
													{
														WorldGen.SlopeTile(num502, num503, 1);
													}
												}
											}
										}
										if (WorldGen.SolidTile(num502, num503) && !Main.tile[num502 - 1, num503].HasTile && !Main.tile[num502 + 1, num503].HasTile)
										{
											WorldGen.KillTile(num502, num503);
										}
									}
								}
								else if (!Main.tile[num502, num503].HasTile && Main.tile[num502, num503 + 1].TileType != 151 && Main.tile[num502, num503 + 1].TileType != 274)
								{
									if (Main.tile[num502 + 1, num503].TileType != 190 && Main.tile[num502 + 1, num503].TileType != 48 && Main.tile[num502 + 1, num503].TileType != 232 && WorldGen.SolidTile(num502 - 1, num503 + 1) && WorldGen.SolidTile(num502 + 1, num503) && !Main.tile[num502 - 1, num503].HasTile && !Main.tile[num502 + 1, num503 - 1].HasTile)
									{
										if (Main.tile[num502 + 1, num503].TileType == 495)
										{
											WorldGen.PlaceTile(num502, num503, Main.tile[num502 + 1, num503].TileType);
										}
										else
										{
											WorldGen.PlaceTile(num502, num503, Main.tile[num502, num503 + 1].TileType);
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											WorldGen.SlopeTile(num502, num503, 2);
										}
										else
										{
											WorldGen.PoundTile(num502, num503);
										}
									}
									if (Main.tile[num502 - 1, num503].TileType != 190 && Main.tile[num502 - 1, num503].TileType != 48 && Main.tile[num502 - 1, num503].TileType != 232 && WorldGen.SolidTile(num502 + 1, num503 + 1) && WorldGen.SolidTile(num502 - 1, num503) && !Main.tile[num502 + 1, num503].HasTile && !Main.tile[num502 - 1, num503 - 1].HasTile)
									{
										if (Main.tile[num502 - 1, num503].TileType == 495)
										{
											WorldGen.PlaceTile(num502, num503, Main.tile[num502 - 1, num503].TileType);
										}
										else
										{
											WorldGen.PlaceTile(num502, num503, Main.tile[num502, num503 + 1].TileType);
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											WorldGen.SlopeTile(num502, num503, 1);
										}
										else
										{
											WorldGen.PoundTile(num502, num503);
										}
									}
								}
							}
							else if (!Main.tile[num502, num503 + 1].HasTile && WorldGen.genRand.Next(2) == 0 && WorldGen.SolidTile(num502, num503) && !Main.tile[num502 - 1, num503].IsHalfBlock && !Main.tile[num502 + 1, num503].IsHalfBlock && Main.tile[num502 - 1, num503].Slope == 0 && Main.tile[num502 + 1, num503].Slope == 0 && WorldGen.SolidTile(num502, num503 - 1))
							{
								if (WorldGen.SolidTile(num502 - 1, num503) && !WorldGen.SolidTile(num502 + 1, num503) && WorldGen.SolidTile(num502 - 1, num503 - 1))
								{
									WorldGen.SlopeTile(num502, num503, 3);
								}
								else if (WorldGen.SolidTile(num502 + 1, num503) && !WorldGen.SolidTile(num502 - 1, num503) && WorldGen.SolidTile(num502 + 1, num503 - 1))
								{
									WorldGen.SlopeTile(num502, num503, 4);
								}
							}
							if (TileID.Sets.Conversion.Sand[Main.tile[num502, num503].TileType])
							{
								Tile.SmoothSlope(num502, num503, applyToNeighbors: false);
							}
						}
					}
				}
				for (int num504 = 20; num504 < Main.maxTilesX - 20; num504++)
				{
					for (int num505 = 20; num505 < Main.maxTilesY - 20; num505++)
					{
						if (WorldGen.genRand.Next(2) == 0 && !Main.tile[num504, num505 - 1].HasTile && Main.tile[num504, num505].TileType != 137 && Main.tile[num504, num505].TileType != 48 && Main.tile[num504, num505].TileType != 232 && Main.tile[num504, num505].TileType != 191 && Main.tile[num504, num505].TileType != 151 && Main.tile[num504, num505].TileType != 274 && Main.tile[num504, num505].TileType != 75 && Main.tile[num504, num505].TileType != 76 && WorldGen.SolidTile(num504, num505) && Main.tile[num504 - 1, num505].TileType != 137 && Main.tile[num504 + 1, num505].TileType != 137)
						{
							if (WorldGen.SolidTile(num504, num505 + 1) && WorldGen.SolidTile(num504 + 1, num505) && !Main.tile[num504 - 1, num505].HasTile)
							{
								WorldGen.SlopeTile(num504, num505, 2);
							}
							if (WorldGen.SolidTile(num504, num505 + 1) && WorldGen.SolidTile(num504 - 1, num505) && !Main.tile[num504 + 1, num505].HasTile)
							{
								WorldGen.SlopeTile(num504, num505, 1);
							}
						}
						if (((int)Main.tile[num504, num505].Slope) == 1 && !WorldGen.SolidTile(num504 - 1, num505))
						{
							WorldGen.SlopeTile(num504, num505);
							WorldGen.PoundTile(num504, num505);
						}
						if (((int)Main.tile[num504, num505].Slope) == 2 && !WorldGen.SolidTile(num504 + 1, num505))
						{
							WorldGen.SlopeTile(num504, num505);
							WorldGen.PoundTile(num504, num505);
						}
					}
				}
				Main.tileSolid[137] = true;
				Main.tileSolid[190] = false;
				Main.tileSolid[192] = false;
				Main.tileSolid[GenVars.crackedType] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Waterfalls", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[69].Value;
				Main.tileSolid[191] = false;
				for (int num506 = 20; num506 < Main.maxTilesX - 20; num506++)
				{
					double num507 = (double)num506 / (double)Main.maxTilesX;
					progress.Set(num507 * 0.5);
					for (int num508 = 20; num508 < Main.maxTilesY - 20; num508++)
					{
						if (WorldGen.SolidTile(num506, num508) && !Main.tile[num506 - 1, num508].HasTile && WorldGen.SolidTile(num506, num508 + 1) && !Main.tile[num506 + 1, num508].HasTile && (Main.tile[num506 - 1, num508].LiquidAmount > 0 || Main.tile[num506 + 1, num508].LiquidAmount > 0))
						{
							bool flag31 = true;
							int num509 = WorldGen.genRand.Next(8, 20);
							int num510 = WorldGen.genRand.Next(8, 20);
							num509 = num508 - num509;
							num510 += num508;
							for (int num511 = num509; num511 <= num510; num511++)
							{
								if (Main.tile[num506, num511].IsHalfBlock)
								{
									flag31 = false;
								}
							}
							if ((Main.tile[num506, num508].TileType == 75 || Main.tile[num506, num508].TileType == 76) && WorldGen.genRand.Next(10) != 0)
							{
								flag31 = false;
							}
							if (flag31)
							{
								WorldGen.PoundTile(num506, num508);
							}
						}
					}
				}
				for (int num512 = 20; num512 < Main.maxTilesX - 20; num512++)
				{
					double num513 = (double)num512 / (double)Main.maxTilesX;
					progress.Set(num513 * 0.5 + 0.5);
					for (int num514 = 20; num514 < Main.maxTilesY - 20; num514++)
					{
						if (Main.tile[num512, num514].TileType != 48 && Main.tile[num512, num514].TileType != 232 && WorldGen.SolidTile(num512, num514) && WorldGen.SolidTile(num512, num514 + 1))
						{
							if (!WorldGen.SolidTile(num512 + 1, num514) && Main.tile[num512 - 1, num514].IsHalfBlock && Main.tile[num512 - 2, num514].LiquidAmount > 0)
							{
								WorldGen.PoundTile(num512, num514);
							}
							if (!WorldGen.SolidTile(num512 - 1, num514) && Main.tile[num512 + 1, num514].IsHalfBlock && Main.tile[num512 + 2, num514].LiquidAmount > 0)
							{
								WorldGen.PoundTile(num512, num514);
							}
						}
					}
				}
				Main.tileSolid[191] = true;
			}));
			vanillaGenPasses.Add(new PassLegacy("Ice", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (WorldGen.notTheBees)
				{
					NotTheBees();
				}
				progress.Set(1.0);
				for (int num515 = 10; num515 < Main.maxTilesX - 10; num515++)
				{
					for (int num516 = (int)Main.worldSurface; num516 < Main.maxTilesY - 100; num516++)
					{
						if (Main.tile[num515, num516].LiquidAmount > 0 && (!IsLava(Main.tile[num515, num516]) || WorldGen.remixWorldGen))
						{
							WorldGen.MakeWateryIceThing(num515, num516);
						}
					}
				}
				Main.tileSolid[226] = false;
				Main.tileSolid[162] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Wall Variety", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				//IL_0093: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_0098: Unknown result TileType (might be due to invalid IL or missing references)
				progress.Message = Lang.gen[79].Value;
				double num517 = (double)(Main.maxTilesX * Main.maxTilesY) / 5040000.0;
				int num518 = (int)(300.0 * num517);
				int num519 = num518;
				ShapeData shapeData = new ShapeData();
				while (num518 > 0)
				{
					progress.Set(1.0 - (double)num518 / (double)num519);
					Point point2 = WorldGen.RandomWorldPoint((int)GenVars.worldSurface, 2, 190, 2);
					while (Vector2D.Distance(new Vector2D((double)point2.X, (double)point2.Y), GenVars.shimmerPosition) < (double)WorldGen.shimmerSafetyDistance)
					{
						point2 = WorldGen.RandomWorldPoint((int)GenVars.worldSurface, 2, 190, 2);
					}
					Tile tile = Main.tile[point2.X, point2.Y];
					Tile tile2 = Main.tile[point2.X, point2.Y - 1];
					ushort num520 = 0;
					if (tile.TileType == 60)
					{
						num520 = (ushort)(204 + WorldGen.genRand.Next(4));
					}
					else if (tile.TileType == 1 && tile2.WallType == 0)
					{
						num520 = ((!WorldGen.remixWorldGen) ? (((double)point2.Y < GenVars.rockLayer) ? ((ushort)(196 + WorldGen.genRand.Next(4))) : ((point2.Y >= GenVars.lavaLine) ? ((ushort)(208 + WorldGen.genRand.Next(4))) : ((ushort)(212 + WorldGen.genRand.Next(4))))) : (((double)point2.Y > GenVars.rockLayer) ? ((ushort)(196 + WorldGen.genRand.Next(4))) : ((point2.Y <= GenVars.lavaLine || WorldGen.genRand.Next(2) != 0) ? ((ushort)(212 + WorldGen.genRand.Next(4))) : ((ushort)(208 + WorldGen.genRand.Next(4))))));
					}
					if (tile.HasTile && num520 != 0 && !tile2.HasTile)
					{
						bool foundInvalidTile = false;
						bool flag32 = ((tile.TileType != 60) ? WorldUtils.Gen(new Point(point2.X, point2.Y - 1), new ShapeFloodFill(1000), Actions.Chain(new Modifiers.IsNotSolid(), new Actions.Blank().Output(shapeData), new Actions.ContinueWrapper(Actions.Chain(new Modifiers.IsTouching(true, 60, 147, 161, 396, 397, 70, 191), new Modifiers.IsTouching(true, 147, 161, 396, 397, 70, 191), new Actions.Custom(delegate
						{
							foundInvalidTile = true;
							return true;
						}))))) : WorldUtils.Gen(new Point(point2.X, point2.Y - 1), new ShapeFloodFill(1000), Actions.Chain(new Modifiers.IsNotSolid(), new Actions.Blank().Output(shapeData), new Actions.ContinueWrapper(Actions.Chain(new Modifiers.IsTouching(true, 147, 161, 396, 397, 70, 191), new Actions.Custom(delegate
						{
							foundInvalidTile = true;
							return true;
						}))))));
						if (shapeData.Count > 50 && flag32 && !foundInvalidTile)
						{
							WorldUtils.Gen(new Point(point2.X, point2.Y), new ModShapes.OuterOutline(shapeData, useDiagonals: true, useInterior: true), Actions.Chain(new Modifiers.SkipWalls(87), new Actions.PlaceWall(num520)));
							num518--;
						}
						shapeData.Clear();
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Life Crystals", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (WorldGen.getGoodWorldGen)
				{
					Main.tileSolid[56] = false;
				}
				if (WorldGen.notTheBees)
				{
					NotTheBees();
				}
				progress.Message = Lang.gen[28].Value;
				double num521 = (double)(Main.maxTilesX * Main.maxTilesY) * 2E-05;
				if (WorldGen.tenthAnniversaryWorldGen)
				{
					num521 *= 1.2;
				}
				if (Main.starGame)
				{
					num521 *= Main.starGameMath(0.2);
				}
				for (int num522 = 0; num522 < (int)num521; num522++)
				{
					double value11 = (double)num522 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05);
					progress.Set(value11);
					bool flag33 = false;
					int num523 = 0;
					while (!flag33)
					{
						int j6 = WorldGen.genRand.Next((int)(Main.worldSurface * 2.0 + Main.rockLayer) / 3, Main.maxTilesY - 300);
						if (WorldGen.remixWorldGen)
						{
							j6 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 400);
						}
						if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(Main.offLimitBorderTiles, Main.maxTilesX - Main.offLimitBorderTiles), j6))
						{
							flag33 = true;
						}
						else
						{
							num523++;
							if (num523 >= 10000)
							{
								flag33 = true;
							}
						}
					}
				}
				Main.tileSolid[225] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Statues", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[29].Value;
				int num524 = 0;
				double num525 = (double)Main.maxTilesX / 4200.0;
				int num526 = (int)((double)(GenVars.statueList.Length * 2) * num525);
				if (WorldGen.noTrapsWorldGen)
				{
					num526 *= 15;
					if (WorldGen.tenthAnniversaryWorldGen || WorldGen.notTheBees)
					{
						num526 /= 5;
					}
				}
				if (Main.starGame)
				{
					num526 = (int)((double)num526 * Main.starGameMath(0.2));
				}
				for (int num527 = 0; num527 < num526; num527++)
				{
					if (num524 >= GenVars.statueList.Length)
					{
						num524 = 0;
					}
					int x9 = GenVars.statueList[num524].X;
					int y7 = GenVars.statueList[num524].Y;
					double value12 = num527 / num526;
					progress.Set(value12);
					bool flag34 = false;
					int num528 = 0;
					while (!flag34)
					{
						int num529 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
						int num530 = WorldGen.genRand.Next((int)(Main.worldSurface * 2.0 + Main.rockLayer) / 3, Main.maxTilesY - 300);
						if (WorldGen.remixWorldGen)
						{
							WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 400);
						}
						while (WorldGen.oceanDepths(num529, num530))
						{
							num529 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
							num530 = WorldGen.genRand.Next((int)(Main.worldSurface * 2.0 + Main.rockLayer) / 3, Main.maxTilesY - 300);
							if (WorldGen.remixWorldGen)
							{
								WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 400);
							}
						}
						while (!Main.tile[num529, num530].HasTile)
						{
							num530++;
							if (num530 >= Main.maxTilesY)
							{
								break;
							}
						}
						if (num530 < Main.maxTilesY)
						{
							num530--;
							if (!IsShimmer(Main.tile[num529, num530]))
							{
								WorldGen.PlaceTile(num529, num530, x9, mute: true, forced: true, -1, y7);
							}
							if (Main.tile[num529, num530].HasTile && Main.tile[num529, num530].TileType == x9)
							{
								flag34 = true;
								if (GenVars.StatuesWithTraps.Contains(num524))
								{
									WorldGen.PlaceStatueTrap(num529, num530);
								}
								num524++;
							}
							else
							{
								num528++;
								if (num528 >= 10000)
								{
									flag34 = true;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Buried Chests", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[30].Value;
				Main.tileSolid[226] = true;
				Main.tileSolid[162] = true;
				Main.tileSolid[225] = true;
				CaveHouseBiome caveHouseBiome = GenVars.configuration.CreateBiome<CaveHouseBiome>();
				int random2 = passConfig.Get<WorldGenRange>("CaveHouseCount").GetRandom(WorldGen.genRand);
				int random3 = passConfig.Get<WorldGenRange>("UnderworldChestCount").GetRandom(WorldGen.genRand);
				int num531 = passConfig.Get<WorldGenRange>("CaveChestCount").GetRandom(WorldGen.genRand);
				int random4 = passConfig.Get<WorldGenRange>("AdditionalDesertHouseCount").GetRandom(WorldGen.genRand);
				if (Main.starGame)
				{
					num531 = (int)((double)num531 * Main.starGameMath(0.2));
				}
				int num532 = random2 + random3 + num531 + random4;
				int num533 = 10000;
				for (int num534 = 0; num534 < num531; num534++)
				{
					if (num533 <= 0)
					{
						break;
					}
					progress.Set((double)num534 / (double)num532);
					int num535 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num536 = WorldGen.genRand.Next((int)((GenVars.worldSurfaceHigh + 20.0 + Main.rockLayer) / 2.0), Main.maxTilesY - 230);
					if (WorldGen.remixWorldGen)
					{
						num536 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 400);
					}
					ushort wall = Main.tile[num535, num536].WallType;
					if (Main.wallDungeon[wall] || wall == 87 || WorldGen.oceanDepths(num535, num536) || !WorldGen.AddBuriedChest(num535, num536, 0, notNearOtherChests: false, -1, trySlope: false, 0))
					{
						num533--;
						num534--;
					}
				}
				num533 = 10000;
				for (int num537 = 0; num537 < random3; num537++)
				{
					if (num533 <= 0)
					{
						break;
					}
					progress.Set((double)(num537 + num531) / (double)num532);
					int num538 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num539 = WorldGen.genRand.Next(Main.UnderworldLayer, Main.maxTilesY - 50);
					if (Main.wallDungeon[Main.tile[num538, num539].WallType] || !WorldGen.AddBuriedChest(num538, num539, 0, notNearOtherChests: false, -1, trySlope: false, 0))
					{
						num533--;
						num537--;
					}
				}
				num533 = 10000;
				for (int num540 = 0; num540 < random2; num540++)
				{
					if (num533 <= 0)
					{
						break;
					}
					progress.Set((double)(num540 + num531 + random3) / (double)num532);
					int x10 = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
					int y8 = WorldGen.genRand.Next((int)(GenVars.worldSurfaceHigh + 20.0), Main.maxTilesY - 230);
					if (WorldGen.remixWorldGen)
					{
						y8 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 400);
					}
					if (WorldGen.oceanDepths(x10, y8) || !caveHouseBiome.Place(new Point(x10, y8), GenVars.structures))
					{
						num533--;
						num540--;
					}
				}
				num533 = 10000;
				Rectangle undergroundDesertHiveLocation = GenVars.UndergroundDesertHiveLocation;
				if ((double)undergroundDesertHiveLocation.Y < Main.worldSurface + 26.0)
				{
					int num541 = (int)Main.worldSurface + 26 - undergroundDesertHiveLocation.Y;
					undergroundDesertHiveLocation.Y += num541;
					undergroundDesertHiveLocation.Height -= num541;
				}
				for (int num542 = 0; num542 < random4; num542++)
				{
					if (num533 <= 0)
					{
						break;
					}
					progress.Set((double)(num542 + num531 + random3 + random2) / (double)num532);
					if (!caveHouseBiome.Place(WorldGen.RandomRectanglePoint(undergroundDesertHiveLocation), GenVars.structures))
					{
						num533--;
						num542--;
					}
				}
				Main.tileSolid[226] = false;
				Main.tileSolid[162] = false;
				Main.tileSolid[225] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Surface Chests", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[31].Value;
				for (int num543 = 0; num543 < (int)((double)Main.maxTilesX * 0.005); num543++)
				{
					double value13 = (double)num543 / ((double)Main.maxTilesX * 0.005);
					progress.Set(value13);
					bool flag35 = false;
					int num544 = 0;
					while (!flag35)
					{
						int num545 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						int num546 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)Main.worldSurface);
						if (WorldGen.remixWorldGen)
						{
							num546 = WorldGen.genRand.Next(Main.maxTilesY - 400, Main.maxTilesY - 150);
						}
						else
						{
							while (WorldGen.oceanDepths(num545, num546))
							{
								num545 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
								num546 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)Main.worldSurface);
							}
						}
						bool flag36 = false;
						if (!Main.tile[num545, num546].HasTile)
						{
							if (Main.tile[num545, num546].WallType == 2 || Main.tile[num545, num546].WallType == 59 || Main.tile[num545, num546].WallType == 244 || WorldGen.remixWorldGen)
							{
								flag36 = true;
							}
						}
						else
						{
							int num547 = 50;
							int num548 = num545;
							int num549 = num546;
							int num550 = 1;
							for (int num551 = num548 - num547; num551 <= num548 + num547; num551 += 2)
							{
								for (int num552 = num549 - num547; num552 <= num549 + num547; num552 += 2)
								{
									if ((double)num552 < Main.worldSurface && !Main.tile[num551, num552].HasTile && Main.tile[num551, num552].WallType == 244 && WorldGen.genRand.Next(num550) == 0)
									{
										num550++;
										flag36 = true;
										num545 = num551;
										num546 = num552;
									}
								}
							}
						}
						if (flag36 && WorldGen.AddBuriedChest(num545, num546, 0, notNearOtherChests: true, -1, trySlope: false, 0))
						{
							flag35 = true;
						}
						else
						{
							num544++;
							if (num544 >= 2000)
							{
								flag35 = true;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Jungle Chests Placement", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[32].Value;
				for (int num553 = 0; num553 < GenVars.numJChests; num553++)
				{
					double value14 = (double)num553 / (double)GenVars.numJChests;
					progress.Set(value14);
					int nextJungleChestItem = WorldGen.GetNextJungleChestItem();
					if (!WorldGen.AddBuriedChest(GenVars.JChestX[num553] + WorldGen.genRand.Next(2), GenVars.JChestY[num553], nextJungleChestItem, notNearOtherChests: false, 10, trySlope: false, 0))
					{
						for (int num554 = GenVars.JChestX[num553] - 1; num554 <= GenVars.JChestX[num553] + 1; num554++)
						{
							for (int num555 = GenVars.JChestY[num553]; num555 <= GenVars.JChestY[num553] + 2; num555++)
							{
								WorldGen.KillTile(num554, num555);
							}
						}
						for (int num556 = GenVars.JChestX[num553] - 1; num556 <= GenVars.JChestX[num553] + 1; num556++)
						{
							for (int num557 = GenVars.JChestY[num553]; num557 <= GenVars.JChestY[num553] + 3; num557++)
							{
								if (num557 < Main.maxTilesY)
								{
									var tile = Main.tile[num556, num557];
									tile.Slope = 0;
									tile.IsHalfBlock = false;
								}
							}
						}
						WorldGen.AddBuriedChest(GenVars.JChestX[num553], GenVars.JChestY[num553], nextJungleChestItem, notNearOtherChests: false, 10, trySlope: false, 0);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Water Chests", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[33].Value;
				for (int num558 = 0; num558 < GenVars.numOceanCaveTreasure; num558++)
				{
					int contain = WorldGen.genRand.NextFromList(new short[5] { 863, 186, 277, 187, 4404 });
					bool flag37 = false;
					double num559 = 2.0;
					while (!flag37 && num559 < 50.0)
					{
						num559 += 0.1;
						int num560 = WorldGen.genRand.Next(GenVars.oceanCaveTreasure[num558].X - (int)num559, GenVars.oceanCaveTreasure[num558].X + (int)num559 + 1);
						int num561 = WorldGen.genRand.Next(GenVars.oceanCaveTreasure[num558].Y - (int)num559 / 2, GenVars.oceanCaveTreasure[num558].Y + (int)num559 / 2 + 1);
						num560 = ((num560 >= Main.maxTilesX) ? ((int)((double)num560 + num559 / 2.0)) : ((int)((double)num560 - num559 / 2.0)));
						if (Main.tile[num560, num561].LiquidAmount > 250 && (Main.tile[num560, num561].LiquidType == 0 || WorldGen.notTheBees || WorldGen.remixWorldGen))
						{
							flag37 = WorldGen.AddBuriedChest(num560, num561, contain, notNearOtherChests: false, 17, trySlope: true, 0);
						}
					}
				}
				int num562 = 0;
				double num563 = (double)Main.maxTilesX / 4200.0;
				for (int num564 = 0; (double)num564 < 9.0 * num563; num564++)
				{
					double value15 = (double)num564 / (9.0 * num563);
					progress.Set(value15);
					int num565 = 0;
					num562++;
					int maxValue5 = 10;
					if (WorldGen.tenthAnniversaryWorldGen)
					{
						maxValue5 = 7;
					}
					if (WorldGen.genRand.Next(maxValue5) == 0)
					{
						num565 = 863;
					}
					else
					{
						switch (num562)
						{
						case 1:
							num565 = 186;
							break;
						case 2:
							num565 = 4404;
							break;
						case 3:
							num565 = 277;
							break;
						default:
							num565 = 187;
							num562 = 0;
							break;
						}
					}
					bool flag38 = false;
					int num566 = 0;
					while (!flag38)
					{
						int num567 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num568 = WorldGen.genRand.Next(1, Main.UnderworldLayer);
						while (Main.tile[num567, num568].LiquidAmount < 250 || (Main.tile[num567, num568].LiquidType != 0 && !WorldGen.notTheBees && !WorldGen.remixWorldGen))
						{
							num567 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
							num568 = WorldGen.genRand.Next(50, Main.UnderworldLayer);
						}
						flag38 = WorldGen.AddBuriedChest(num567, num568, num565, notNearOtherChests: false, 17, num567 < WorldGen.beachDistance || num567 > Main.maxTilesX - WorldGen.beachDistance, 0);
						num566++;
						if (num566 > 10000)
						{
							break;
						}
					}
					flag38 = false;
					num566 = 0;
					while (!flag38)
					{
						int num569 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num570 = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
						while (Main.tile[num569, num570].LiquidAmount < 250 || (Main.tile[num569, num570].LiquidType != 0 && !WorldGen.notTheBees))
						{
							num569 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
							num570 = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
						}
						flag38 = WorldGen.AddBuriedChest(num569, num570, num565, notNearOtherChests: false, 17, num569 < WorldGen.beachDistance || num569 > Main.maxTilesX - WorldGen.beachDistance, 0);
						num566++;
						if (num566 > 10000)
						{
							break;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Spider Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[64].Value;
				WorldGen.maxTileCount = 3500;
				int num571 = Main.maxTilesX / 2;
				int num572 = (int)((double)Main.maxTilesX * 0.005);
				if (WorldGen.getGoodWorldGen)
				{
					num572 *= 3;
				}
				if (WorldGen.notTheBees)
				{
					Main.tileSolid[225] = true;
				}
				for (int num573 = 0; num573 < num572; num573++)
				{
					double value16 = (double)num573 / ((double)Main.maxTilesX * 0.005);
					progress.Set(value16);
					int num574 = 0;
					int x11 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
					int y9 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 230);
					if (WorldGen.remixWorldGen)
					{
						y9 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
					}
					int num575 = WorldGen.countTiles(x11, y9, jungle: false, lavaOk: true);
					while ((num575 >= 3500 || num575 < 500) && num574 < num571)
					{
						num574++;
						x11 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						y9 = WorldGen.genRand.Next((int)Main.rockLayer + 30, Main.maxTilesY - 230);
						if (WorldGen.remixWorldGen)
						{
							y9 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
						}
						num575 = WorldGen.countTiles(x11, y9, jungle: false, lavaOk: true);
						if (WorldGen.shroomCount > 1)
						{
							num575 = 0;
						}
					}
					if (num574 < num571)
					{
						Spread.Spider(x11, y9);
					}
				}
				if (WorldGen.notTheBees)
				{
					Main.tileSolid[225] = false;
				}
				Main.tileSolid[162] = true;
			}));
			vanillaGenPasses.Add(new PassLegacy("Gem Caves", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (!WorldGen.notTheBees)
				{
					progress.Message = Lang.gen[64].Value;
					WorldGen.maxTileCount = 300;
					double num576 = (double)Main.maxTilesX * 0.003;
					if (WorldGen.tenthAnniversaryWorldGen)
					{
						num576 *= 1.5;
					}
					if (Main.starGame)
					{
						num576 *= Main.starGameMath(0.2);
					}
					for (int num577 = 0; (double)num577 < num576; num577++)
					{
						double value17 = (double)num577 / num576;
						progress.Set(value17);
						int num578 = 0;
						int x12 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						int y10 = WorldGen.genRand.Next((int)Main.rockLayer + 30, Main.maxTilesY - 230);
						if (WorldGen.remixWorldGen)
						{
							y10 = WorldGen.genRand.Next((int)Main.worldSurface + 30, (int)Main.rockLayer - 30);
						}
						int num579 = WorldGen.countTiles(x12, y10);
						while ((num579 >= 300 || num579 < 50 || WorldGen.lavaCount > 0 || WorldGen.iceCount > 0 || WorldGen.rockCount == 0) && num578 < 1000)
						{
							num578++;
							x12 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
							y10 = WorldGen.genRand.Next((int)Main.rockLayer + 30, Main.maxTilesY - 230);
							if (WorldGen.remixWorldGen)
							{
								y10 = WorldGen.genRand.Next((int)Main.worldSurface + 30, (int)Main.rockLayer - 30);
							}
							num579 = WorldGen.countTiles(x12, y10);
						}
						if (num578 < 1000)
						{
							WorldGen.gemCave(x12, y10);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Moss", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				//IL_039c: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_03a1: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_04ee: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_04f3: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_05cd: Unknown result TileType (might be due to invalid IL or missing references)
				//IL_05d2: Unknown result TileType (might be due to invalid IL or missing references)
				if (!WorldGen.notTheBees || WorldGen.remixWorldGen)
				{
					progress.Message = Lang.gen[61].Value;
					WorldGen.randMoss();
					int num580 = Main.maxTilesX / 2100;
					if (WorldGen.remixWorldGen)
					{
						num580 = (int)((double)num580 * 1.5);
					}
					else if (WorldGen.tenthAnniversaryWorldGen)
					{
						num580 *= 2;
					}
					int num581 = 0;
					int num582 = 0;
					while (num582 < num580)
					{
						int num583 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
						if (WorldGen.remixWorldGen)
						{
							num583 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
						}
						else if (WorldGen.tenthAnniversaryWorldGen)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								WorldGen.randMoss(justNeon: true);
							}
						}
						else if (WorldGen.getGoodWorldGen)
						{
							while ((double)num583 > (double)Main.maxTilesX * 0.42 && (double)num583 < (double)Main.maxTilesX * 0.48)
							{
								num583 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
							}
						}
						else if (!WorldGen.drunkWorldGen)
						{
							while ((double)num583 > (double)Main.maxTilesX * 0.38 && (double)num583 < (double)Main.maxTilesX * 0.62)
							{
								num583 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
							}
						}
						int num584 = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next((int)Main.rockLayer + 40, GenVars.lavaLine - 40) : WorldGen.genRand.Next((int)Main.worldSurface + 50, (int)Main.rockLayer - 50));
						bool flag39 = false;
						int num585 = 50;
						for (int num586 = num583 - num585; num586 <= num583 + num585; num586++)
						{
							for (int num587 = num584 - num585; num587 <= num584 + num585; num587++)
							{
								if (Main.tile[num586, num587].HasTile)
								{
									int type9 = Main.tile[num586, num587].TileType;
									if (WorldGen.remixWorldGen)
									{
										if (type9 == 60 || type9 == 161 || type9 == 147 || Main.tileDungeon[type9] || type9 == 25 || type9 == 203)
										{
											flag39 = true;
											num586 = num583 + num585 + 1;
											break;
										}
									}
									else if (type9 == 70 || type9 == 60 || type9 == 367 || type9 == 368 || type9 == 161 || type9 == 147 || type9 == 396 || type9 == 397 || Main.tileDungeon[type9])
									{
										flag39 = true;
										num586 = num583 + num585 + 1;
										break;
									}
								}
							}
						}
						if (flag39)
						{
							num581++;
							if (num581 > Main.maxTilesX)
							{
								num582++;
							}
						}
						else
						{
							num581 = 0;
							num582++;
							int maxY = GenVars.lavaLine;
							if (WorldGen.remixWorldGen)
							{
								maxY = (int)Main.rockLayer + 50;
							}
							WorldGen.neonMossBiome(num583, num584, maxY);
						}
					}
					WorldGen.maxTileCount = 2500;
					for (int num588 = 0; num588 < (int)((double)Main.maxTilesX * 0.01); num588++)
					{
						double value18 = (double)num588 / ((double)Main.maxTilesX * 0.01);
						progress.Set(value18);
						int num589 = 0;
						int num590 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						int num591 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, GenVars.waterLine);
						if (WorldGen.remixWorldGen)
						{
							num591 = WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer);
						}
						if (!(Vector2D.Distance(new Vector2D((double)num590, (double)num591), GenVars.shimmerPosition) < (double)WorldGen.shimmerSafetyDistance))
						{
							int num592 = WorldGen.countTiles(num590, num591);
							while ((num592 >= 2500 || num592 < 10 || WorldGen.lavaCount > 0 || WorldGen.iceCount > 0 || WorldGen.rockCount == 0 || WorldGen.shroomCount > 0) && num589 < 1000)
							{
								num589++;
								num590 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
								num591 = WorldGen.genRand.Next((int)Main.rockLayer + 30, Main.maxTilesY - 230);
								num592 = WorldGen.countTiles(num590, num591);
							}
							if (num589 < 1000)
							{
								WorldGen.setMoss(num590, num591);
								Spread.Moss(num590, num591);
							}
						}
					}
					for (int num593 = 0; num593 < Main.maxTilesX; num593++)
					{
						int num594 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num595 = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, GenVars.lavaLine) : WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300));
						if (!(Vector2D.Distance(new Vector2D((double)num594, (double)num595), GenVars.shimmerPosition) < (double)WorldGen.shimmerSafetyDistance) && Main.tile[num594, num595].TileType == 1)
						{
							WorldGen.setMoss(num594, num595);
							Main.tile[num594, num595].TileType = GenVars.mossTile;
						}
					}
					double num596 = (double)Main.maxTilesX * 0.05;
					while (num596 > 0.0)
					{
						int num597 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num598 = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, GenVars.lavaLine) : WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300));
						if (!(Vector2D.Distance(new Vector2D((double)num597, (double)num598), GenVars.shimmerPosition) < (double)WorldGen.shimmerSafetyDistance) && Main.tile[num597, num598].TileType == 1 && (!Main.tile[num597 - 1, num598].HasTile || !Main.tile[num597 + 1, num598].HasTile || !Main.tile[num597, num598 - 1].HasTile || !Main.tile[num597, num598 + 1].HasTile))
						{
							WorldGen.setMoss(num597, num598);
							Main.tile[num597, num598].TileType = GenVars.mossTile;
							num596 -= 1.0;
						}
					}
					num596 = (double)Main.maxTilesX * 0.065;
					if (WorldGen.remixWorldGen)
					{
						num596 *= 2.0;
					}
					while (num596 > 0.0)
					{
						int num599 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num600 = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next(GenVars.waterLine, Main.UnderworldLayer) : WorldGen.genRand.Next(GenVars.lavaLine, (int)Main.rockLayer + 50));
						if (Main.tile[num599, num600].TileType == 1 && (!Main.tile[num599 - 1, num600].HasTile || !Main.tile[num599 + 1, num600].HasTile || !Main.tile[num599, num600 - 1].HasTile || !Main.tile[num599, num600 + 1].HasTile))
						{
							int num601 = 25;
							int num602 = 0;
							for (int num603 = num599 - num601; num603 < num599 + num601; num603++)
							{
								for (int num604 = num600 - num601; num604 < num600 + num601; num604++)
								{
									if (Main.tile[num603, num604].LiquidAmount > 0 && IsLava(Main.tile[num603, num604]))
									{
										num602++;
									}
								}
							}
							if (num602 > 20)
							{
								Main.tile[num599, num600].TileType = 381;
								num596 -= 1.0;
							}
							else
							{
								num596 -= 0.002;
							}
						}
						num596 -= 0.001;
					}
					for (int num605 = 0; num605 < Main.maxTilesX; num605++)
					{
						for (int num606 = 0; num606 < Main.maxTilesY; num606++)
						{
							if (Main.tile[num605, num606].HasTile && Main.tileMoss[Main.tile[num605, num606].TileType])
							{
								for (int num607 = 0; num607 < 4; num607++)
								{
									int num608 = num605;
									int num609 = num606;
									if (num607 == 0)
									{
										num608--;
									}
									if (num607 == 1)
									{
										num608++;
									}
									if (num607 == 2)
									{
										num609--;
									}
									if (num607 == 3)
									{
										num609++;
									}
									try
									{
										WorldGen.grassSpread = 0;
										WorldGen.SpreadGrass(num608, num609, 1, Main.tile[num605, num606].TileType);
									}
									catch
									{
										WorldGen.grassSpread = 0;
										WorldGen.SpreadGrass(num608, num609, 1, Main.tile[num605, num606].TileType, repeat: false);
									}
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Temple", delegate
			{
				Main.tileSolid[162] = false;
				Main.tileSolid[226] = true;
				WorldGen.templePart2();
				Main.tileSolid[232] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Cave Walls", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[63].Value;
				WorldGen.maxTileCount = 1500;
				for (int num610 = 0; num610 < (int)((double)Main.maxTilesX * 0.04); num610++)
				{
					double num611 = (double)num610 / ((double)Main.maxTilesX * 0.04);
					progress.Set(num611 * 0.66);
					int num612 = 0;
					int x13 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
					int y11 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 220);
					if (WorldGen.remixWorldGen)
					{
						y11 = WorldGen.genRand.Next((int)Main.worldSurface + 25, (int)Main.rockLayer);
					}
					int num613 = WorldGen.countTiles(x13, y11, jungle: false, lavaOk: true);
					while ((num613 >= WorldGen.maxTileCount || num613 < 10) && num612 < 500)
					{
						num612++;
						x13 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						y11 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 220);
						if (WorldGen.remixWorldGen)
						{
							y11 = WorldGen.genRand.Next((int)Main.worldSurface + 25, (int)Main.rockLayer);
						}
						num613 = WorldGen.countTiles(x13, y11, jungle: false, lavaOk: true);
					}
					if (num612 < 500)
					{
						int num614 = WorldGen.genRand.Next(2);
						if ((double)WorldGen.shroomCount > (double)WorldGen.rockCount * 0.75)
						{
							num614 = 80;
						}
						else if (WorldGen.iceCount > 0)
						{
							switch (num614)
							{
							case 0:
								num614 = 40;
								break;
							case 1:
								num614 = 71;
								break;
							}
						}
						else if (WorldGen.lavaCount > 0)
						{
							num614 = 79;
						}
						else
						{
							num614 = WorldGen.genRand.Next(4);
							switch (num614)
							{
							case 0:
								num614 = 59;
								break;
							case 1:
								num614 = 61;
								break;
							case 2:
								num614 = 170;
								break;
							case 3:
								num614 = 171;
								break;
							}
						}
						Spread.Wall(x13, y11, num614);
					}
				}
				if (WorldGen.remixWorldGen)
				{
					WorldGen.maxTileCount = 1500;
					for (int num615 = 0; num615 < (int)((double)Main.maxTilesX * 0.04); num615++)
					{
						double num616 = (double)num615 / ((double)Main.maxTilesX * 0.04);
						progress.Set(num616 * 0.66);
						int num617 = 0;
						int x14 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						int y12 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 350);
						int num618 = WorldGen.countTiles(x14, y12, jungle: false, lavaOk: true);
						while ((num618 >= WorldGen.maxTileCount || num618 < 10) && num617 < 500)
						{
							num617++;
							x14 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
							y12 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 350);
							num618 = WorldGen.countTiles(x14, y12, jungle: false, lavaOk: true);
						}
						if (num617 < 500 && WorldGen.iceCount == 0 && WorldGen.lavaCount == 0 && WorldGen.sandCount == 0)
						{
							int wallType = ((WorldGen.genRand.Next(2) != 0) ? 63 : 2);
							Spread.Wall(x14, y12, wallType);
						}
					}
				}
				WorldGen.maxTileCount = 1500;
				double num619 = (double)Main.maxTilesX * 0.02;
				for (int num620 = 0; (double)num620 < num619; num620++)
				{
					double num621 = (double)num620 / ((double)Main.maxTilesX * 0.02);
					progress.Set(num621 * 0.33 + 0.66);
					int num622 = 0;
					int x15 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
					int y13 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.lavaLine);
					int num623 = 0;
					if (Main.tile[x15, y13].WallType == 64)
					{
						num623 = WorldGen.countTiles(x15, y13, jungle: true);
					}
					while ((num623 >= WorldGen.maxTileCount || num623 < 10) && num622 < 1000)
					{
						num622++;
						x15 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
						y13 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.lavaLine);
						if (!Main.wallHouse[Main.tile[x15, y13].WallType] && Main.tile[x15, y13].WallType != 244)
						{
							num623 = ((Main.tile[x15, y13].WallType == 64) ? WorldGen.countTiles(x15, y13, jungle: true) : 0);
						}
					}
					if (num622 < 1000)
					{
						Spread.Wall2(x15, y13, 15);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Jungle Trees", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[83].Value;
				for (int num624 = 0; num624 < Main.maxTilesX; num624++)
				{
					progress.Set((double)num624 / (double)Main.maxTilesX);
					for (int num625 = (int)Main.worldSurface - 1; num625 < Main.maxTilesY - 350; num625++)
					{
						if (WorldGen.genRand.Next(10) == 0 || WorldGen.drunkWorldGen)
						{
							WorldGen.GrowUndergroundTree(num624, num625);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Floating Island Houses", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num626 = 0; num626 < GenVars.numIslandHouses; num626++)
				{
					if (!GenVars.skyLake[num626])
					{
						WorldGen.IslandHouse(GenVars.floatingIslandHouseX[num626], GenVars.floatingIslandHouseY[num626], GenVars.floatingIslandStyle[num626]);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Quick Cleanup", delegate (GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				if (WorldGen.notTheBees)
				{
					NotTheBees();
				}
				Main.tileSolid[137] = false;
				Main.tileSolid[130] = false;
				for (int num627 = 20; num627 < Main.maxTilesX - 20; num627++)
				{
					for (int num628 = 20; num628 < Main.maxTilesY - 20; num628++)
					{
						if ((double)num628 < Main.worldSurface && WorldGen.oceanDepths(num627, num628) && Main.tile[num627, num628].TileType == 53 && Main.tile[num627, num628].HasTile)
						{
							if (Main.tile[num627, num628].BottomSlope)
							{
								var tile = Main.tile[num627, num628];
								tile.Slope = 0;
							}
							for (int num629 = num628 + 1; num629 < num628 + WorldGen.genRand.Next(4, 7) && (!Main.tile[num627, num629].HasTile || (Main.tile[num627, num629].TileType != 397 && Main.tile[num627, num629].TileType != 53)) && (!Main.tile[num627, num629 + 1].HasTile || (Main.tile[num627, num629 + 1].TileType != 397 && Main.tile[num627, num629 + 1].TileType != 53 && Main.tile[num627, num629 + 1].TileType != 495)) && (!Main.tile[num627, num629 + 2].HasTile || (Main.tile[num627, num629 + 2].TileType != 397 && Main.tile[num627, num629 + 2].TileType != 53 && Main.tile[num627, num629 + 2].TileType != 495)); num629++)
							{
								Main.tile[num627, num629].TileType = 0;
								var tile = Main.tile[num627, num629];
								tile.HasTile = true;
								tile.IsHalfBlock = false;
								tile.Slope = 0;
							}
						}
						if (Main.tile[num627, num628].WallType == 187 || Main.tile[num627, num628].WallType == 216)
						{
							if (Main.tile[num627, num628].TileType == 59 || Main.tile[num627, num628].TileType == 123 || Main.tile[num627, num628].TileType == 224)
							{
								Main.tile[num627, num628].TileType = 397;
							}
							if (Main.tile[num627, num628].TileType == 368 || Main.tile[num627, num628].TileType == 367)
							{
								Main.tile[num627, num628].TileType = 397;
							}
							if ((double)num628 <= Main.rockLayer)
							{
								Main.tile[num627, num628].LiquidAmount = 0;
							}
							else if (Main.tile[num627, num628].LiquidAmount > 0)
							{
								Main.tile[num627, num628].LiquidAmount = byte.MaxValue;
								SetLiquidToLava(Main.tile[num627, num628], true);
							}
						}
						if ((double)num628 < Main.worldSurface && Main.tile[num627, num628].HasTile && Main.tile[num627, num628].TileType == 53 && Main.tile[num627, num628 + 1].WallType == 0 && !WorldGen.SolidTile(num627, num628 + 1))
						{
							ushort num630 = 0;
							int num631 = 3;
							for (int num632 = num627 - num631; num632 <= num627 + num631; num632++)
							{
								for (int num633 = num628 - num631; num633 <= num628 + num631; num633++)
								{
									if (Main.tile[num632, num633].WallType > 0)
									{
										num630 = Main.tile[num632, num633].WallType;
										break;
									}
								}
							}
							if (num630 > 0)
							{
								Main.tile[num627, num628 + 1].WallType = num630;
								if (Main.tile[num627, num628].WallType == 0)
								{
									Main.tile[num627, num628].WallType = num630;
								}
							}
						}
						if (Main.tile[num627, num628].TileType != 19 && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[num627, num628].TileType])
						{
							if (Main.tile[num627, num628].TopSlope || Main.tile[num627, num628].IsHalfBlock)
							{
								if (Main.tile[num627, num628].TileType != 225 || !Main.tile[num627, num628].IsHalfBlock)
								{
									if (!WorldGen.SolidTile(num627, num628 + 1))
									{
										var tile = Main.tile[num627, num628];
										tile.HasTile = false;
									}
									if (Main.tile[num627 + 1, num628].TileType == 137 || Main.tile[num627 - 1, num628].TileType == 137)
									{
										var tile = Main.tile[num627, num628];
										tile.HasTile = false;
									}
								}
							}
							else if (Main.tile[num627, num628].BottomSlope)
							{
								if (!WorldGen.SolidTile(num627, num628 - 1))
								{
									var tile = Main.tile[num627, num628];
									tile.HasTile = false;
								}
								if (Main.tile[num627 + 1, num628].TileType == 137 || Main.tile[num627 - 1, num628].TileType == 137)
								{
									var tile = Main.tile[num627, num628];
									tile.HasTile = false;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Pots", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				Main.tileSolid[137] = true;
				Main.tileSolid[130] = true;
				progress.Message = Lang.gen[35].Value;
				if (WorldGen.noTrapsWorldGen)
				{
					Main.tileSolid[138] = true;
					int num634 = (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0004);
					if (WorldGen.remixWorldGen)
					{
						num634 /= 2;
					}
					for (int num635 = 0; num635 < num634; num635++)
					{
						int num636 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num637;
						for (num637 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 250); !Main.tile[num636, num637].HasTile && num637 < Main.maxTilesY - 250; num637++)
						{
						}
						num637--;
						if (!IsShimmer(Main.tile[num636, num637]))
						{
							WorldGen.PlaceTile(num636, num637, 138, mute: true);
							WorldGen.PlaceTile(num636 + 2, num637, 138, mute: true);
							WorldGen.PlaceTile(num636 + 1, num637 - 2, 138, mute: true);
						}
					}
					Main.tileSolid[138] = false;
				}
				double num638 = (double)(Main.maxTilesX * Main.maxTilesY) * 0.0008;
				if (Main.starGame)
				{
					num638 *= Main.starGameMath(0.2);
				}
				for (int num639 = 0; (double)num639 < num638; num639++)
				{
					double num640 = (double)num639 / num638;
					progress.Set(num640);
					bool flag40 = false;
					int num641 = 0;
					while (!flag40)
					{
						int num642 = WorldGen.genRand.Next((int)GenVars.worldSurfaceHigh, Main.maxTilesY - 10);
						if (num640 > 0.93)
						{
							num642 = Main.maxTilesY - 150;
						}
						else if (num640 > 0.75)
						{
							num642 = (int)GenVars.worldSurfaceLow;
						}
						int x16 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
						bool flag41 = false;
						for (int num643 = num642; num643 < Main.maxTilesY - 20; num643++)
						{
							if (!flag41)
							{
								if (Main.tile[x16, num643].HasTile && Main.tileSolid[Main.tile[x16, num643].TileType] && !IsLava(Main.tile[x16, num643 - 1]) && !IsShimmer(Main.tile[x16, num643 - 1]))
								{
									flag41 = true;
								}
							}
							else if (!((double)num643 < Main.worldSurface) || Main.tile[x16, num643].WallType != 0)
							{
								int style2 = WorldGen.genRand.Next(0, 4);
								int num644 = 0;
								int num645 = 0;
								if (num643 < Main.maxTilesY - 5)
								{
									num644 = Main.tile[x16, num643 + 1].TileType;
									num645 = Main.tile[x16, num643].WallType;
								}
								if (num644 == 147 || num644 == 161 || num644 == 162)
								{
									style2 = WorldGen.genRand.Next(4, 7);
								}
								if (num644 == 60)
								{
									style2 = WorldGen.genRand.Next(7, 10);
								}
								if (Main.wallDungeon[Main.tile[x16, num643].WallType])
								{
									style2 = WorldGen.genRand.Next(10, 13);
								}
								if (num644 == 41 || num644 == 43 || num644 == 44 || num644 == 481 || num644 == 482 || num644 == 483)
								{
									style2 = WorldGen.genRand.Next(10, 13);
								}
								if (num644 == 22 || num644 == 23 || num644 == 25)
								{
									style2 = WorldGen.genRand.Next(16, 19);
								}
								if (num644 == 199 || num644 == 203 || num644 == 204 || num644 == 200)
								{
									style2 = WorldGen.genRand.Next(22, 25);
								}
								if (num644 == 367)
								{
									style2 = WorldGen.genRand.Next(31, 34);
								}
								if (num644 == 226)
								{
									style2 = WorldGen.genRand.Next(28, 31);
								}
								if (num645 == 187 || num645 == 216)
								{
									style2 = WorldGen.genRand.Next(34, 37);
								}
								if (num643 > Main.UnderworldLayer)
								{
									style2 = WorldGen.genRand.Next(13, 16);
								}
								if (!WorldGen.oceanDepths(x16, num643) && !IsShimmer(Main.tile[x16, num643]) && WorldGen.PlacePot(x16, num643, 28, style2))
								{
									flag40 = true;
									break;
								}
								num641++;
								if (num641 >= 10000)
								{
									flag40 = true;
									break;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Hellforge", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[36].Value;
				for (int num646 = 0; num646 < Main.maxTilesX / 200; num646++)
				{
					double value19 = (double)num646 / (double)(Main.maxTilesX / 200);
					progress.Set(value19);
					bool flag42 = false;
					int num647 = 0;
					while (!flag42)
					{
						int num648 = WorldGen.genRand.Next(1, Main.maxTilesX);
						int num649 = WorldGen.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 30);
						try
						{
							if (Main.tile[num648, num649].WallType == 13 || Main.tile[num648, num649].WallType == 14)
							{
								for (; !Main.tile[num648, num649].HasTile && num649 < Main.maxTilesY - 20; num649++)
								{
								}
								num649--;
								WorldGen.PlaceTile(num648, num649, 77);
								if (Main.tile[num648, num649].TileType == 77)
								{
									flag42 = true;
								}
								else
								{
									num647++;
									if (num647 >= 10000)
									{
										flag42 = true;
									}
								}
							}
						}
						catch
						{
							num647++;
							if (num647 >= 10000)
							{
								flag42 = true;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Spreading Grass", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (!WorldGen.notTheBees || WorldGen.remixWorldGen)
				{
					progress.Message = Lang.gen[37].Value;
					for (int num650 = 50; num650 < Main.maxTilesX - 50; num650++)
					{
						for (int num651 = 50; (double)num651 <= Main.worldSurface; num651++)
						{
							if (Main.tile[num650, num651].HasTile)
							{
								int type10 = Main.tile[num650, num651].TileType;
								if (Main.tile[num650, num651].HasTile && type10 == 60)
								{
									for (int num652 = num650 - 1; num652 <= num650 + 1; num652++)
									{
										for (int num653 = num651 - 1; num653 <= num651 + 1; num653++)
										{
											if (Main.tile[num652, num653].HasTile && Main.tile[num652, num653].TileType == 0)
											{
												if (!Main.tile[num652, num653 - 1].HasTile)
												{
													Main.tile[num652, num653].TileType = 60;
												}
												else
												{
													Main.tile[num652, num653].TileType = 59;
												}
											}
										}
									}
								}
								else if (type10 == 1 || type10 == 40 || TileID.Sets.Ore[type10])
								{
									int num654 = 3;
									bool flag43 = false;
									ushort num655 = 0;
									for (int num656 = num650 - num654; num656 <= num650 + num654; num656++)
									{
										for (int num657 = num651 - num654; num657 <= num651 + num654; num657++)
										{
											if (Main.tile[num656, num657].HasTile)
											{
												if (Main.tile[num656, num657].TileType == 53 || num655 == 53)
												{
													num655 = 53;
												}
												else if (Main.tile[num656, num657].TileType == 59 || Main.tile[num656, num657].TileType == 60 || Main.tile[num656, num657].TileType == 147 || Main.tile[num656, num657].TileType == 161 || Main.tile[num656, num657].TileType == 199 || Main.tile[num656, num657].TileType == 23)
												{
													num655 = Main.tile[num656, num657].TileType;
												}
											}
											else if (num657 < num651 && Main.tile[num656, num657].WallType == 0)
											{
												flag43 = true;
											}
										}
									}
									if (flag43)
									{
										switch (num655)
										{
										case 23:
										case 199:
											if (Main.tile[num650, num651 - 1].HasTile)
											{
												num655 = 0;
											}
											break;
										case 59:
										case 60:
											if (num650 >= GenVars.jungleMinX && num650 <= GenVars.jungleMaxX)
											{
												num655 = (ushort)(Main.tile[num650, num651 - 1].HasTile ? 59u : 60u);
											}
											break;
										}
										Main.tile[num650, num651].TileType = num655;
									}
								}
							}
						}
					}
					for (int num658 = 10; num658 < Main.maxTilesX - 10; num658++)
					{
						bool flag44 = true;
						for (int num659 = 0; (double)num659 < Main.worldSurface - 1.0; num659++)
						{
							if (Main.tile[num658, num659].HasTile)
							{
								if (flag44 && Main.tile[num658, num659].TileType == 0)
								{
									try
									{
										WorldGen.grassSpread = 0;
										WorldGen.SpreadGrass(num658, num659);
									}
									catch
									{
										WorldGen.grassSpread = 0;
										WorldGen.SpreadGrass(num658, num659, 0, 2, repeat: false);
									}
								}
								if ((double)num659 > GenVars.worldSurfaceHigh)
								{
									break;
								}
								flag44 = false;
							}
							else if (Main.tile[num658, num659].WallType == 0)
							{
								flag44 = true;
							}
						}
					}
					if (WorldGen.remixWorldGen)
					{
						for (int num660 = 5; num660 < Main.maxTilesX - 5; num660++)
						{
							for (int num661 = (int)GenVars.rockLayerLow + WorldGen.genRand.Next(-1, 2); num661 < Main.maxTilesY - 200; num661++)
							{
								if (Main.tile[num660, num661].TileType == 0 && Main.tile[num660, num661].HasTile && (!Main.tile[num660 - 1, num661 - 1].HasTile || !Main.tile[num660, num661 - 1].HasTile || !Main.tile[num660 + 1, num661 - 1].HasTile || !Main.tile[num660 - 1, num661].HasTile || !Main.tile[num660 + 1, num661].HasTile || !Main.tile[num660 - 1, num661 + 1].HasTile || !Main.tile[num660, num661 + 1].HasTile || !Main.tile[num660 + 1, num661 + 1].HasTile))
								{
									Main.tile[num660, num661].TileType = 2;
								}
							}
						}
						for (int num662 = 5; num662 < Main.maxTilesX - 5; num662++)
						{
							for (int num663 = (int)GenVars.rockLayerLow + WorldGen.genRand.Next(-1, 2); num663 < Main.maxTilesY - 200; num663++)
							{
								if (Main.tile[num662, num663].TileType == 2 && !Main.tile[num662, num663 - 1].HasTile && WorldGen.genRand.Next(20) == 0)
								{
									WorldGen.PlaceTile(num662, num663 - 1, 27, mute: true);
								}
							}
						}
						int conversionType = 1;
						if (WorldGen.crimson)
						{
							conversionType = 4;
						}
						int num664 = Main.maxTilesX / 7;
						for (int num665 = 10; num665 < Main.maxTilesX - 10; num665++)
						{
							for (int num666 = 10; num666 < Main.maxTilesY - 10; num666++)
							{
								if ((double)num666 < Main.worldSurface + (double)WorldGen.genRand.Next(3) || num665 < num664 + WorldGen.genRand.Next(3) || num665 >= Main.maxTilesX - num664 - WorldGen.genRand.Next(3))
								{
									if (WorldGen.drunkWorldGen)
									{
										if (GenVars.crimsonLeft)
										{
											if (num665 < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
											{
												WorldGen.Convert(num665, num666, 4, 1);
											}
											else
											{
												WorldGen.Convert(num665, num666, 1, 1);
											}
										}
										else if (num665 < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
										{
											WorldGen.Convert(num665, num666, 1, 1);
										}
										else
										{
											WorldGen.Convert(num665, num666, 4, 1);
										}
									}
									else
									{
										WorldGen.Convert(num665, num666, conversionType, 1);
									}
									var tile = Main.tile[num665, num666];
									tile.TileColor = 0;
									tile.WallColor = 0;
								}
							}
						}
						if (WorldGen.remixWorldGen)
						{
							Main.tileSolid[225] = true;
							int num667 = (int)((double)Main.maxTilesX * 0.31);
							int num668 = (int)((double)Main.maxTilesX * 0.69);
							_ = Main.maxTilesY;
							int num669 = Main.maxTilesY - 135;
							_ = Main.maxTilesY;
							Liquid.QuickWater(-2);
							for (int num670 = num667; num670 < num668 + 15; num670++)
							{
								for (int num671 = Main.maxTilesY - 200; num671 < num669; num671++)
								{
									Main.tile[num670, num671].LiquidAmount = 0;
								}
							}
							Main.tileSolid[225] = false;
							Main.tileSolid[484] = false;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Surface Ore and Stone", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				int num672 = WorldGen.genRand.Next(Main.maxTilesX * 5 / 4200, Main.maxTilesX * 10 / 4200);
				for (int num673 = 0; num673 < num672; num673++)
				{
					int num674 = Main.maxTilesX / 420;
					while (num674 > 0)
					{
						num674--;
						int num675 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
						while ((double)num675 >= (double)Main.maxTilesX * 0.48 && (double)num675 <= (double)Main.maxTilesX * 0.52)
						{
							num675 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
						}
						int y14 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurface);
						bool flag45 = false;
						for (int num676 = 0; num676 < GenVars.numOrePatch; num676++)
						{
							if (Math.Abs(num675 - GenVars.orePatchX[num676]) < 200)
							{
								flag45 = true;
							}
						}
						if (!flag45 && WorldGen.OrePatch(num675, y14))
						{
							if (GenVars.numOrePatch < GenVars.maxOrePatch - 1)
							{
								GenVars.orePatchX[GenVars.numOrePatch] = num675;
								GenVars.numOrePatch++;
							}
							break;
						}
					}
				}
				num672 = WorldGen.genRand.Next(1, Main.maxTilesX * 7 / 4200);
				for (int num677 = 0; num677 < num672; num677++)
				{
					int num678 = Main.maxTilesX / 420;
					while (num678 > 0)
					{
						num678--;
						int num679 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
						while ((double)num679 >= (double)Main.maxTilesX * 0.47 && (double)num679 <= (double)Main.maxTilesX * 0.53)
						{
							num679 = WorldGen.genRand.Next(WorldGen.beachDistance, Main.maxTilesX - WorldGen.beachDistance);
						}
						int y15 = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.worldSurface);
						bool flag46 = false;
						for (int num680 = 0; num680 < GenVars.numOrePatch; num680++)
						{
							if (Math.Abs(num679 - GenVars.orePatchX[num680]) < 100)
							{
								flag46 = true;
							}
						}
						if (!flag46 && WorldGen.StonePatch(num679, y15))
						{
							break;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Place Fallen Log", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[85].Value;
				int num681 = Main.maxTilesX / 2100;
				num681 = ((!WorldGen.remixWorldGen) ? (num681 + WorldGen.genRand.Next(-1, 2)) : (num681 + WorldGen.genRand.Next(0, 2)));
				for (int num682 = 0; num682 < num681; num682++)
				{
					progress.Set((double)num682 / (double)num681);
					int num683 = WorldGen.beachDistance + 20;
					int num684 = 50000;
					int num685 = 5000;
					while (num684 > 0)
					{
						num684--;
						int num686 = WorldGen.genRand.Next(num683, Main.maxTilesX - num683);
						int num687 = WorldGen.genRand.Next(10, (int)Main.worldSurface);
						if (WorldGen.remixWorldGen)
						{
							num687 = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY - 350);
						}
						bool flag47 = false;
						if (num684 < num685)
						{
							flag47 = true;
						}
						if (num684 > num685 / 2)
						{
							while ((double)num686 > (double)Main.maxTilesX * 0.4 && (double)num686 < (double)Main.maxTilesX * 0.6)
							{
								num686 = WorldGen.genRand.Next(num683, Main.maxTilesX - num683);
							}
						}
						if (!Main.tile[num686, num687].HasTile && Main.tile[num686, num687].WallType == 0)
						{
							bool flag48 = true;
							if (WorldGen.remixWorldGen)
							{
								for (; !Main.tile[num686, num687].HasTile && Main.tile[num686, num687].WallType == 0 && num687 <= Main.maxTilesY - 350; num687++)
								{
								}
							}
							else
							{
								for (; !Main.tile[num686, num687].HasTile && Main.tile[num686, num687].WallType == 0 && (double)num687 <= Main.worldSurface; num687++)
								{
								}
							}
							if ((double)num687 > Main.worldSurface - 10.0 && !WorldGen.remixWorldGen)
							{
								flag48 = false;
							}
							else if (!flag47)
							{
								int num688 = 50;
								for (int num689 = num686 - num688; num689 < num686 + num688; num689++)
								{
									if (num689 > 10 && num689 < Main.maxTilesX - 10)
									{
										for (int num690 = num687 - num688; num690 < num687 + num688; num690++)
										{
											if (num690 > 10 && num690 < Main.maxTilesY - 10)
											{
												int type11 = Main.tile[num689, num690].TileType;
												switch (type11)
												{
												case 189:
													flag48 = false;
													break;
												case 53:
													flag48 = false;
													break;
												default:
													if (Main.tileDungeon[type11])
													{
														flag48 = false;
													}
													else if (TileID.Sets.Crimson[type11])
													{
														flag48 = false;
													}
													else if (TileID.Sets.Corrupt[type11])
													{
														flag48 = false;
													}
													break;
												}
											}
										}
									}
								}
								if (flag48)
								{
									int num691 = 10;
									int num692 = 10;
									for (int num693 = num686 - num691; num693 < num686 + num691; num693++)
									{
										for (int num694 = num687 - num692; num694 < num687 - 1; num694++)
										{
											if (Main.tile[num693, num694].HasTile && Main.tileSolid[Main.tile[num693, num694].TileType])
											{
												flag48 = false;
											}
											if (Main.tile[num693, num694].WallType != 0)
											{
												flag48 = false;
											}
										}
									}
								}
							}
							if (flag48 && (Main.tile[num686, num687 - 1].LiquidAmount == 0 || num684 < num685 / 5) && (Main.tile[num686, num687].TileType == 2 || (WorldGen.notTheBees && Main.tile[num686, num687].TileType == 60)) && (Main.tile[num686 - 1, num687].TileType == 2 || (WorldGen.notTheBees && Main.tile[num686 - 1, num687].TileType == 60)) && (Main.tile[num686 + 1, num687].TileType == 2 || (WorldGen.notTheBees && Main.tile[num686 + 1, num687].TileType == 60)))
							{
								num687--;
								WorldGen.PlaceTile(num686, num687, 488);
								if (Main.tile[num686, num687].HasTile && Main.tile[num686, num687].TileType == 488)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										GenVars.logX = num686;
										GenVars.logY = num687;
									}
									num684 = -1;
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Traps", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (!WorldGen.notTheBees || WorldGen.noTrapsWorldGen || WorldGen.remixWorldGen)
				{
					WorldGen.placingTraps = true;
					progress.Message = Lang.gen[34].Value;
					if (WorldGen.noTrapsWorldGen)
					{
						progress.Message = Lang.gen[91].Value;
					}
					double num695 = (double)Main.maxTilesX * 0.05;
					if (WorldGen.noTrapsWorldGen)
					{
						num695 = ((!WorldGen.tenthAnniversaryWorldGen && !WorldGen.notTheBees) ? (num695 * 100.0) : (num695 * 5.0));
					}
					else if (WorldGen.getGoodWorldGen)
					{
						num695 *= 1.5;
					}
					if (Main.starGame)
					{
						num695 *= Main.starGameMath(0.2);
					}
					for (int num696 = 0; (double)num696 < num695; num696++)
					{
						progress.Set((double)num696 / num695 / 2.0);
						for (int num697 = 0; num697 < 1150; num697++)
						{
							if (WorldGen.noTrapsWorldGen)
							{
								int num698 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
								int num699 = WorldGen.genRand.Next(50, Main.maxTilesY - 50);
								if (WorldGen.remixWorldGen)
								{
									num699 = WorldGen.genRand.Next(50, Main.maxTilesY - 210);
								}
								if (((double)num699 > Main.worldSurface || Main.tile[num698, num699].WallType > 0) && WorldGen.placeTrap(num698, num699))
								{
									break;
								}
							}
							else
							{
								int num700 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
								int num701 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 210);
								while (WorldGen.oceanDepths(num700, num701))
								{
									num700 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
									num701 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 210);
								}
								if (Main.tile[num700, num701].WallType == 0 && WorldGen.placeTrap(num700, num701))
								{
									break;
								}
							}
						}
					}
					if (WorldGen.noTrapsWorldGen)
					{
						num695 = Main.maxTilesX * 3;
						if (Main.remixWorld)
						{
							num695 = Main.maxTilesX / 3;
						}
						if (Main.starGame)
						{
							num695 *= Main.starGameMath(0.2);
						}
						for (int num702 = 0; (double)num702 < num695; num702++)
						{
							if (Main.remixWorld)
							{
								placeTNTBarrel(WorldGen.genRand.Next(50, Main.maxTilesX - 50), WorldGen.genRand.Next((int)Main.worldSurface, (int)((double)(Main.maxTilesY - 350) + Main.rockLayer) / 2));
							}
							else
							{
								placeTNTBarrel(WorldGen.genRand.Next(50, Main.maxTilesX - 50), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200));
							}
						}
					}
					num695 = (double)Main.maxTilesX * 0.003;
					if (WorldGen.noTrapsWorldGen)
					{
						num695 *= 5.0;
					}
					else if (WorldGen.getGoodWorldGen)
					{
						num695 *= 1.5;
					}
					for (int num703 = 0; (double)num703 < num695; num703++)
					{
						progress.Set((double)num703 / num695 / 2.0 + 0.5);
						for (int num704 = 0; num704 < 20000; num704++)
						{
							int num705 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.15), (int)((double)Main.maxTilesX * 0.85));
							int num706 = WorldGen.genRand.Next((int)Main.worldSurface + 20, Main.maxTilesY - 210);
							if (Main.tile[num705, num706].WallType == 187 && WorldGen.PlaceSandTrap(num705, num706))
							{
								break;
							}
						}
					}
					if (WorldGen.drunkWorldGen && !WorldGen.noTrapsWorldGen && !WorldGen.notTheBees)
					{
						for (int num707 = 0; num707 < 8; num707++)
						{
							progress.Message = Lang.gen[34].Value;
							num695 = 100.0;
							for (int num708 = 0; (double)num708 < num695; num708++)
							{
								progress.Set((double)num708 / num695);
								Thread.Sleep(10);
							}
						}
					}
					if (WorldGen.noTrapsWorldGen)
					{
						Main.tileSolid[138] = true;
					}
					WorldGen.placingTraps = false;
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Piles", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[89].Value;
				Main.tileSolid[229] = false;
				Main.tileSolid[190] = false;
				Main.tileSolid[196] = false;
				Main.tileSolid[189] = false;
				Main.tileSolid[202] = false;
				Main.tileSolid[460] = false;
				Main.tileSolid[484] = false;
				if (WorldGen.noTrapsWorldGen)
				{
					Main.tileSolid[138] = false;
				}
				for (int num709 = 0; (double)num709 < (double)Main.maxTilesX * 0.06; num709++)
				{
					int num710 = Main.maxTilesX / 2;
					bool flag49 = false;
					while (!flag49 && num710 > 0)
					{
						num710--;
						int num711 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num712 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300);
						while (WorldGen.oceanDepths(num711, num712))
						{
							num711 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
							num712 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300);
						}
						if (!Main.tile[num711, num712].HasTile)
						{
							int num713 = 186;
							for (; !Main.tile[num711, num712 + 1].HasTile && num712 < Main.maxTilesY - 5; num712++)
							{
							}
							int num714 = WorldGen.genRand.Next(22);
							if (num714 >= 16 && num714 <= 22)
							{
								num714 = WorldGen.genRand.Next(22);
							}
							if ((Main.tile[num711, num712 + 1].TileType == 0 || Main.tile[num711, num712 + 1].TileType == 1 || Main.tileMoss[Main.tile[num711, num712 + 1].TileType]) && WorldGen.genRand.Next(5) == 0)
							{
								num714 = WorldGen.genRand.Next(23, 29);
								num713 = 187;
							}
							if (num712 > Main.maxTilesY - 300 || Main.wallDungeon[Main.tile[num711, num712].WallType] || Main.tile[num711, num712 + 1].TileType == 30 || Main.tile[num711, num712 + 1].TileType == 19 || Main.tile[num711, num712 + 1].TileType == 25 || Main.tile[num711, num712 + 1].TileType == 203)
							{
								num714 = WorldGen.genRand.Next(7);
								num713 = 186;
							}
							if (Main.tile[num711, num712 + 1].TileType == 147 || Main.tile[num711, num712 + 1].TileType == 161 || Main.tile[num711, num712 + 1].TileType == 162)
							{
								num714 = WorldGen.genRand.Next(26, 32);
								num713 = 186;
							}
							if (Main.tile[num711, num712 + 1].TileType == 60)
							{
								num713 = 187;
								num714 = WorldGen.genRand.Next(6);
							}
							if ((Main.tile[num711, num712 + 1].TileType == 57 || Main.tile[num711, num712 + 1].TileType == 58) && WorldGen.genRand.Next(3) < 2)
							{
								num713 = 187;
								num714 = WorldGen.genRand.Next(6, 9);
							}
							if (Main.tile[num711, num712 + 1].TileType == 226)
							{
								num713 = 187;
								num714 = WorldGen.genRand.Next(18, 23);
							}
							if (Main.tile[num711, num712 + 1].TileType == 70)
							{
								num714 = WorldGen.genRand.Next(32, 35);
								num713 = 186;
							}
							if (Main.tile[num711, num712 + 1].TileType == 396 || Main.tile[num711, num712 + 1].TileType == 397 || Main.tile[num711, num712 + 1].TileType == 404)
							{
								num714 = WorldGen.genRand.Next(29, 35);
								num713 = 187;
							}
							if (Main.tile[num711, num712 + 1].TileType == 368)
							{
								num714 = WorldGen.genRand.Next(35, 41);
								num713 = 187;
							}
							if (Main.tile[num711, num712 + 1].TileType == 367)
							{
								num714 = WorldGen.genRand.Next(41, 47);
								num713 = 187;
							}
							if (num713 == 186 && num714 >= 7 && num714 <= 15 && WorldGen.genRand.Next(75) == 0)
							{
								num713 = 187;
								num714 = 17;
							}
							if (Main.wallDungeon[Main.tile[num711, num712].WallType] && WorldGen.genRand.Next(3) != 0)
							{
								flag49 = true;
							}
							else
							{
								if (!IsShimmer(Main.tile[num711, num712]))
								{
									WorldGen.PlaceTile(num711, num712, num713, mute: true, forced: false, -1, num714);
								}
								if (Main.tile[num711, num712].TileType == 186 || Main.tile[num711, num712].TileType == 187)
								{
									flag49 = true;
								}
								if (flag49 && num713 == 186 && num714 <= 7)
								{
									int num715 = WorldGen.genRand.Next(1, 5);
									for (int num716 = 0; num716 < num715; num716++)
									{
										int num717 = num711 + WorldGen.genRand.Next(-10, 11);
										int num718 = num712 - WorldGen.genRand.Next(5);
										if (!Main.tile[num717, num718].HasTile)
										{
											for (; !Main.tile[num717, num718 + 1].HasTile && num718 < Main.maxTilesY - 5; num718++)
											{
											}
											int x17 = WorldGen.genRand.Next(12, 36);
											WorldGen.PlaceSmallPile(num717, num718, x17, 0, 185);
										}
									}
								}
							}
						}
					}
				}
				for (int num719 = 0; (double)num719 < (double)Main.maxTilesX * 0.01; num719++)
				{
					int num720 = Main.maxTilesX / 2;
					bool flag50 = false;
					while (!flag50 && num720 > 0)
					{
						num720--;
						int num721 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num722 = WorldGen.genRand.Next(Main.maxTilesY - 300, Main.maxTilesY - 10);
						if (!Main.tile[num721, num722].HasTile)
						{
							int num723 = 186;
							for (; !Main.tile[num721, num722 + 1].HasTile && num722 < Main.maxTilesY - 5; num722++)
							{
							}
							int num724 = WorldGen.genRand.Next(22);
							if (num724 >= 16 && num724 <= 22)
							{
								num724 = WorldGen.genRand.Next(22);
							}
							if (num722 > Main.maxTilesY - 300 || Main.wallDungeon[Main.tile[num721, num722].WallType] || Main.tile[num721, num722 + 1].TileType == 30 || Main.tile[num721, num722 + 1].TileType == 19)
							{
								num724 = WorldGen.genRand.Next(7);
							}
							if ((Main.tile[num721, num722 + 1].TileType == 57 || Main.tile[num721, num722 + 1].TileType == 58) && WorldGen.genRand.Next(3) < 2)
							{
								num723 = 187;
								num724 = WorldGen.genRand.Next(6, 9);
							}
							if (Main.tile[num721, num722 + 1].TileType == 147 || Main.tile[num721, num722 + 1].TileType == 161 || Main.tile[num721, num722 + 1].TileType == 162)
							{
								num724 = WorldGen.genRand.Next(26, 32);
							}
							WorldGen.PlaceTile(num721, num722, num723, mute: true, forced: false, -1, num724);
							if (Main.tile[num721, num722].TileType == 186 || Main.tile[num721, num722].TileType == 187)
							{
								flag50 = true;
							}
							if (flag50 && num723 == 186 && num724 <= 7)
							{
								int num725 = WorldGen.genRand.Next(1, 5);
								for (int num726 = 0; num726 < num725; num726++)
								{
									int num727 = num721 + WorldGen.genRand.Next(-10, 11);
									int num728 = num722 - WorldGen.genRand.Next(5);
									if (!Main.tile[num727, num728].HasTile)
									{
										for (; !Main.tile[num727, num728 + 1].HasTile && num728 < Main.maxTilesY - 5; num728++)
										{
										}
										int x18 = WorldGen.genRand.Next(12, 36);
										WorldGen.PlaceSmallPile(num727, num728, x18, 0, 185);
									}
								}
							}
						}
					}
				}
				for (int num729 = 0; (double)num729 < (double)Main.maxTilesX * 0.003; num729++)
				{
					int num730 = Main.maxTilesX / 2;
					bool flag51 = false;
					while (!flag51 && num730 > 0)
					{
						num730--;
						int num731 = 186;
						int num732 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num733 = WorldGen.genRand.Next(10, (int)Main.worldSurface);
						while (WorldGen.oceanDepths(num732, num733))
						{
							num732 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
							num733 = WorldGen.genRand.Next(10, (int)Main.worldSurface);
						}
						if (!Main.tile[num732, num733].HasTile)
						{
							for (; !Main.tile[num732, num733 + 1].HasTile && num733 < Main.maxTilesY - 5; num733++)
							{
							}
							int num734 = WorldGen.genRand.Next(7, 13);
							if (num733 > Main.maxTilesY - 300 || Main.wallDungeon[Main.tile[num732, num733].WallType] || Main.tile[num732, num733 + 1].TileType == 30 || Main.tile[num732, num733 + 1].TileType == 19 || Main.tile[num732, num733 + 1].TileType == 25 || Main.tile[num732, num733 + 1].TileType == 203 || Main.tile[num732, num733 + 1].TileType == 234 || Main.tile[num732, num733 + 1].TileType == 112)
							{
								num734 = -1;
							}
							if (Main.tile[num732, num733 + 1].TileType == 147 || Main.tile[num732, num733 + 1].TileType == 161 || Main.tile[num732, num733 + 1].TileType == 162)
							{
								num734 = WorldGen.genRand.Next(26, 32);
							}
							if (Main.tile[num732, num733 + 1].TileType == 53)
							{
								num731 = 187;
								num734 = WorldGen.genRand.Next(52, 55);
							}
							if (Main.tile[num732, num733 + 1].TileType == 2 || Main.tile[num732 - 1, num733 + 1].TileType == 2 || Main.tile[num732 + 1, num733 + 1].TileType == 2)
							{
								num731 = 187;
								num734 = WorldGen.genRand.Next(14, 17);
							}
							if (Main.tile[num732, num733 + 1].TileType == 151 || Main.tile[num732, num733 + 1].TileType == 274)
							{
								num731 = 186;
								num734 = WorldGen.genRand.Next(7);
							}
							if (num734 >= 0)
							{
								WorldGen.PlaceTile(num732, num733, num731, mute: true, forced: false, -1, num734);
							}
							if (Main.tile[num732, num733].TileType == num731)
							{
								flag51 = true;
							}
						}
					}
				}
				for (int num735 = 0; (double)num735 < (double)Main.maxTilesX * 0.0035; num735++)
				{
					int num736 = Main.maxTilesX / 2;
					bool flag52 = false;
					while (!flag52 && num736 > 0)
					{
						num736--;
						int num737 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num738 = WorldGen.genRand.Next(10, (int)Main.worldSurface);
						if (!Main.tile[num737, num738].HasTile && Main.tile[num737, num738].WallType > 0)
						{
							int num739 = 186;
							for (; !Main.tile[num737, num738 + 1].HasTile && num738 < Main.maxTilesY - 5; num738++)
							{
							}
							int num740 = WorldGen.genRand.Next(7, 13);
							if (num738 > Main.maxTilesY - 300 || Main.wallDungeon[Main.tile[num737, num738].WallType] || Main.tile[num737, num738 + 1].TileType == 30 || Main.tile[num737, num738 + 1].TileType == 19)
							{
								num740 = -1;
							}
							if (Main.tile[num737, num738 + 1].TileType == 25)
							{
								num740 = WorldGen.genRand.Next(7);
							}
							if (Main.tile[num737, num738 + 1].TileType == 147 || Main.tile[num737, num738 + 1].TileType == 161 || Main.tile[num737, num738 + 1].TileType == 162)
							{
								num740 = WorldGen.genRand.Next(26, 32);
							}
							if (Main.tile[num737, num738 + 1].TileType == 2 || Main.tile[num737 - 1, num738 + 1].TileType == 2 || Main.tile[num737 + 1, num738 + 1].TileType == 2)
							{
								num739 = 187;
								num740 = WorldGen.genRand.Next(14, 17);
							}
							if (Main.tile[num737, num738 + 1].TileType == 151 || Main.tile[num737, num738 + 1].TileType == 274)
							{
								num739 = 186;
								num740 = WorldGen.genRand.Next(7);
							}
							if (num740 >= 0)
							{
								WorldGen.PlaceTile(num737, num738, num739, mute: true, forced: false, -1, num740);
							}
							if (Main.tile[num737, num738].TileType == num739)
							{
								flag52 = true;
							}
							if (flag52 && num740 <= 7)
							{
								int num741 = WorldGen.genRand.Next(1, 5);
								for (int num742 = 0; num742 < num741; num742++)
								{
									int num743 = num737 + WorldGen.genRand.Next(-10, 11);
									int num744 = num738 - WorldGen.genRand.Next(5);
									if (!Main.tile[num743, num744].HasTile)
									{
										for (; !Main.tile[num743, num744 + 1].HasTile && num744 < Main.maxTilesY - 5; num744++)
										{
										}
										int x19 = WorldGen.genRand.Next(12, 36);
										WorldGen.PlaceSmallPile(num743, num744, x19, 0, 185);
									}
								}
							}
						}
					}
				}
				for (int num745 = 0; (double)num745 < (double)Main.maxTilesX * 0.6; num745++)
				{
					int num746 = Main.maxTilesX / 2;
					bool flag53 = false;
					while (!flag53 && num746 > 0)
					{
						num746--;
						int num747 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num748 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
						if (Main.tile[num747, num748].WallType == 87 && WorldGen.genRand.Next(2) == 0)
						{
							num747 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
							num748 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
						}
						while (WorldGen.oceanDepths(num747, num748))
						{
							num747 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
							num748 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
						}
						if (!Main.tile[num747, num748].HasTile)
						{
							for (; !Main.tile[num747, num748 + 1].HasTile && num748 < Main.maxTilesY - 5; num748++)
							{
							}
							int num749 = WorldGen.genRand.Next(2);
							int num750 = WorldGen.genRand.Next(36);
							if (num750 >= 28 && num750 <= 35)
							{
								num750 = WorldGen.genRand.Next(36);
							}
							if (num749 == 1)
							{
								num750 = WorldGen.genRand.Next(25);
								if (num750 >= 16 && num750 <= 24)
								{
									num750 = WorldGen.genRand.Next(25);
								}
							}
							if (num748 > Main.maxTilesY - 300)
							{
								if (num749 == 0)
								{
									num750 = WorldGen.genRand.Next(12, 28);
								}
								if (num749 == 1)
								{
									num750 = WorldGen.genRand.Next(6, 16);
								}
							}
							if (Main.wallDungeon[Main.tile[num747, num748].WallType] || Main.tile[num747, num748 + 1].TileType == 30 || Main.tile[num747, num748 + 1].TileType == 19 || Main.tile[num747, num748 + 1].TileType == 25 || Main.tile[num747, num748 + 1].TileType == 203 || Main.tile[num747, num748].WallType == 87)
							{
								if (num749 == 0 && num750 < 12)
								{
									num750 += 12;
								}
								if (num749 == 1 && num750 < 6)
								{
									num750 += 6;
								}
								if (num749 == 1 && num750 >= 17)
								{
									num750 -= 10;
								}
							}
							if (Main.tile[num747, num748 + 1].TileType == 147 || Main.tile[num747, num748 + 1].TileType == 161 || Main.tile[num747, num748 + 1].TileType == 162)
							{
								if (num749 == 0 && num750 < 12)
								{
									num750 += 36;
								}
								if (num749 == 1 && num750 >= 20)
								{
									num750 += 6;
								}
								if (num749 == 1 && num750 < 6)
								{
									num750 += 25;
								}
							}
							if (Main.tile[num747, num748 + 1].TileType == 151 || Main.tile[num747, num748 + 1].TileType == 274)
							{
								if (num749 == 0)
								{
									num750 = WorldGen.genRand.Next(12, 28);
								}
								if (num749 == 1)
								{
									num750 = WorldGen.genRand.Next(12, 19);
								}
							}
							if (Main.tile[num747, num748 + 1].TileType == 368)
							{
								if (num749 == 0)
								{
									num750 = WorldGen.genRand.Next(60, 66);
								}
								if (num749 == 1)
								{
									num750 = WorldGen.genRand.Next(47, 53);
								}
							}
							if (Main.tile[num747, num748 + 1].TileType == 367)
							{
								if (num749 == 0)
								{
									num750 = WorldGen.genRand.Next(66, 72);
								}
								if (num749 == 1)
								{
									num750 = WorldGen.genRand.Next(53, 59);
								}
							}
							if (Main.wallDungeon[Main.tile[num747, num748].WallType] && WorldGen.genRand.Next(3) != 0)
							{
								flag53 = true;
							}
							else if (!IsShimmer(Main.tile[num747, num748]))
							{
								flag53 = WorldGen.PlaceSmallPile(num747, num748, num750, num749, 185);
							}
							if (flag53 && num749 == 1 && num750 >= 6 && num750 <= 15)
							{
								int num751 = WorldGen.genRand.Next(1, 5);
								for (int num752 = 0; num752 < num751; num752++)
								{
									int num753 = num747 + WorldGen.genRand.Next(-10, 11);
									int num754 = num748 - WorldGen.genRand.Next(5);
									if (!Main.tile[num753, num754].HasTile)
									{
										for (; !Main.tile[num753, num754 + 1].HasTile && num754 < Main.maxTilesY - 5; num754++)
										{
										}
										int x20 = WorldGen.genRand.Next(12, 36);
										WorldGen.PlaceSmallPile(num753, num754, x20, 0, 185);
									}
								}
							}
						}
					}
				}
				for (int num755 = 0; (double)num755 < (double)Main.maxTilesX * 0.02; num755++)
				{
					int num756 = Main.maxTilesX / 2;
					bool flag54 = false;
					while (!flag54 && num756 > 0)
					{
						num756--;
						int num757 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num758 = WorldGen.genRand.Next(15, (int)Main.worldSurface);
						while (WorldGen.oceanDepths(num757, num758))
						{
							num757 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
							num758 = WorldGen.genRand.Next(15, (int)Main.worldSurface);
						}
						if (!Main.tile[num757, num758].HasTile)
						{
							for (; !Main.tile[num757, num758 + 1].HasTile && num758 < Main.maxTilesY - 5; num758++)
							{
							}
							int num759 = WorldGen.genRand.Next(2);
							int num760 = WorldGen.genRand.Next(11);
							if (num759 == 1)
							{
								num760 = WorldGen.genRand.Next(5);
							}
							if (Main.tile[num757, num758 + 1].TileType == 147 || Main.tile[num757, num758 + 1].TileType == 161 || Main.tile[num757, num758 + 1].TileType == 162)
							{
								if (num759 == 0 && num760 < 12)
								{
									num760 += 36;
								}
								if (num759 == 1 && num760 >= 20)
								{
									num760 += 6;
								}
								if (num759 == 1 && num760 < 6)
								{
									num760 += 25;
								}
							}
							if (Main.tile[num757, num758 + 1].TileType == 2 && num759 == 1)
							{
								num760 = WorldGen.genRand.Next(38, 41);
							}
							if (Main.tile[num757, num758 + 1].TileType == 151 || Main.tile[num757, num758 + 1].TileType == 274)
							{
								if (num759 == 0)
								{
									num760 = WorldGen.genRand.Next(12, 28);
								}
								if (num759 == 1)
								{
									num760 = WorldGen.genRand.Next(12, 19);
								}
							}
							if (!Main.wallDungeon[Main.tile[num757, num758].WallType] && Main.tile[num757, num758 + 1].TileType != 30 && Main.tile[num757, num758 + 1].TileType != 19 && Main.tile[num757, num758 + 1].TileType != 41 && Main.tile[num757, num758 + 1].TileType != 43 && Main.tile[num757, num758 + 1].TileType != 44 && Main.tile[num757, num758 + 1].TileType != 481 && Main.tile[num757, num758 + 1].TileType != 482 && Main.tile[num757, num758 + 1].TileType != 483 && Main.tile[num757, num758 + 1].TileType != 45 && Main.tile[num757, num758 + 1].TileType != 46 && Main.tile[num757, num758 + 1].TileType != 47 && Main.tile[num757, num758 + 1].TileType != 175 && Main.tile[num757, num758 + 1].TileType != 176 && Main.tile[num757, num758 + 1].TileType != 177 && Main.tile[num757, num758 + 1].TileType != 53 && Main.tile[num757, num758 + 1].TileType != 25 && Main.tile[num757, num758 + 1].TileType != 203)
							{
								flag54 = WorldGen.PlaceSmallPile(num757, num758, num760, num759, 185);
							}
						}
					}
				}
				for (int num761 = 0; (double)num761 < (double)Main.maxTilesX * 0.15; num761++)
				{
					int num762 = Main.maxTilesX / 2;
					bool flag55 = false;
					while (!flag55 && num762 > 0)
					{
						num762--;
						int num763 = WorldGen.genRand.Next(25, Main.maxTilesX - 25);
						int num764 = WorldGen.genRand.Next(15, (int)Main.worldSurface);
						if (!Main.tile[num763, num764].HasTile && (Main.tile[num763, num764].WallType == 2 || Main.tile[num763, num764].WallType == 40))
						{
							for (; !Main.tile[num763, num764 + 1].HasTile && num764 < Main.maxTilesY - 5; num764++)
							{
							}
							int num765 = WorldGen.genRand.Next(2);
							int num766 = WorldGen.genRand.Next(11);
							if (num765 == 1)
							{
								num766 = WorldGen.genRand.Next(5);
							}
							if (Main.tile[num763, num764 + 1].TileType == 147 || Main.tile[num763, num764 + 1].TileType == 161 || Main.tile[num763, num764 + 1].TileType == 162)
							{
								if (num765 == 0 && num766 < 12)
								{
									num766 += 36;
								}
								if (num765 == 1 && num766 >= 20)
								{
									num766 += 6;
								}
								if (num765 == 1 && num766 < 6)
								{
									num766 += 25;
								}
							}
							if (Main.tile[num763, num764 + 1].TileType == 2 && num765 == 1)
							{
								num766 = WorldGen.genRand.Next(38, 41);
							}
							if (Main.tile[num763, num764 + 1].TileType == 151 || Main.tile[num763, num764 + 1].TileType == 274)
							{
								if (num765 == 0)
								{
									num766 = WorldGen.genRand.Next(12, 28);
								}
								if (num765 == 1)
								{
									num766 = WorldGen.genRand.Next(12, 19);
								}
							}
							if ((Main.tile[num763, num764].LiquidAmount != byte.MaxValue || Main.tile[num763, num764 + 1].TileType != 53 || Main.tile[num763, num764].WallType != 0) && !Main.wallDungeon[Main.tile[num763, num764].WallType] && Main.tile[num763, num764 + 1].TileType != 30 && Main.tile[num763, num764 + 1].TileType != 19 && Main.tile[num763, num764 + 1].TileType != 41 && Main.tile[num763, num764 + 1].TileType != 43 && Main.tile[num763, num764 + 1].TileType != 44 && Main.tile[num763, num764 + 1].TileType != 481 && Main.tile[num763, num764 + 1].TileType != 482 && Main.tile[num763, num764 + 1].TileType != 483 && Main.tile[num763, num764 + 1].TileType != 45 && Main.tile[num763, num764 + 1].TileType != 46 && Main.tile[num763, num764 + 1].TileType != 47 && Main.tile[num763, num764 + 1].TileType != 175 && Main.tile[num763, num764 + 1].TileType != 176 && Main.tile[num763, num764 + 1].TileType != 177 && Main.tile[num763, num764 + 1].TileType != 25 && Main.tile[num763, num764 + 1].TileType != 203)
							{
								flag55 = WorldGen.PlaceSmallPile(num763, num764, num766, num765, 185);
							}
						}
					}
				}
				Main.tileSolid[190] = true;
				Main.tileSolid[192] = true;
				Main.tileSolid[196] = true;
				Main.tileSolid[189] = true;
				Main.tileSolid[202] = true;
				Main.tileSolid[225] = true;
				Main.tileSolid[460] = true;
				Main.tileSolid[138] = true;
			}));
			vanillaGenPasses.Add(new PassLegacy("Spawn Point", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				int num767 = 5;
				bool flag56 = true;
				int num768 = Main.maxTilesX / 2;
				if (Main.tenthAnniversaryWorld && !WorldGen.remixWorldGen)
				{
					int num769 = GenVars.beachBordersWidth + 15;
					num768 = ((WorldGen.genRand.Next(2) != 0) ? (Main.maxTilesX - num769) : num769);
				}
				while (flag56)
				{
					int num770 = num768 + WorldGen.genRand.Next(-num767, num767 + 1);
					for (int num771 = 0; num771 < Main.maxTilesY; num771++)
					{
						if (Main.tile[num770, num771].HasTile)
						{
							Main.spawnTileX = num770;
							Main.spawnTileY = num771;
							break;
						}
					}
					flag56 = false;
					num767++;
					if ((double)Main.spawnTileY > Main.worldSurface)
					{
						flag56 = true;
					}
					if (Main.tile[Main.spawnTileX, Main.spawnTileY - 1].LiquidAmount > 0)
					{
						flag56 = true;
					}
				}
				int num772 = 10;
				while ((double)Main.spawnTileY > Main.worldSurface)
				{
					int num773 = WorldGen.genRand.Next(num768 - num772, num768 + num772);
					for (int num774 = 0; num774 < Main.maxTilesY; num774++)
					{
						if (Main.tile[num773, num774].HasTile)
						{
							Main.spawnTileX = num773;
							Main.spawnTileY = num774;
							break;
						}
					}
					num772++;
				}
				if (WorldGen.remixWorldGen)
				{
					int num775 = Main.maxTilesY - 10;
					while (WorldGen.SolidTile(Main.spawnTileX, num775))
					{
						num775--;
					}
					Main.spawnTileY = num775 + 1;
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Grass Wall", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				WorldGen.maxTileCount = 3500;
				progress.Set(1.0);
				for (int num776 = 50; num776 < Main.maxTilesX - 50; num776++)
				{
					for (int num777 = 0; (double)num777 < Main.worldSurface - 10.0; num777++)
					{
						if (WorldGen.genRand.Next(4) == 0)
						{
							bool flag57 = false;
							int num778 = -1;
							int num779 = -1;
							if (Main.tile[num776, num777].HasTile && Main.tile[num776, num777].TileType == 2 && (Main.tile[num776, num777].WallType == 2 || Main.tile[num776, num777].WallType == 63))
							{
								for (int num780 = num776 - 1; num780 <= num776 + 1; num780++)
								{
									for (int num781 = num777 - 1; num781 <= num777 + 1; num781++)
									{
										if (Main.tile[num780, num781].WallType == 0 && !WorldGen.SolidTile(num780, num781))
										{
											flag57 = true;
										}
									}
								}
								if (flag57)
								{
									for (int num782 = num776 - 1; num782 <= num776 + 1; num782++)
									{
										for (int num783 = num777 - 1; num783 <= num777 + 1; num783++)
										{
											if ((Main.tile[num782, num783].WallType == 2 || Main.tile[num782, num783].WallType == 15) && !WorldGen.SolidTile(num782, num783))
											{
												num778 = num782;
												num779 = num783;
											}
										}
									}
								}
							}
							if (flag57 && num778 > -1 && num779 > -1 && WorldGen.countDirtTiles(num778, num779) < WorldGen.maxTileCount)
							{
								try
								{
									ushort wallType2 = 63;
									if (WorldGen.dontStarveWorldGen && WorldGen.genRand.Next(3) != 0)
									{
										wallType2 = 62;
									}
									Spread.Wall2(num778, num779, wallType2);
								}
								catch
								{
								}
							}
						}
					}
				}
				for (int num784 = 5; num784 < Main.maxTilesX - 5; num784++)
				{
					for (int num785 = 10; (double)num785 < Main.worldSurface - 1.0; num785++)
					{
						if (Main.tile[num784, num785].WallType == 63 && WorldGen.genRand.Next(10) == 0)
						{
							Main.tile[num784, num785].WallType = 65;
						}
						if (Main.tile[num784, num785].HasTile && Main.tile[num784, num785].TileType == 0)
						{
							bool flag58 = false;
							for (int num786 = num784 - 1; num786 <= num784 + 1; num786++)
							{
								for (int num787 = num785 - 1; num787 <= num785 + 1; num787++)
								{
									if (Main.tile[num786, num787].WallType == 63 || Main.tile[num786, num787].WallType == 65)
									{
										flag58 = true;
										break;
									}
								}
							}
							if (flag58)
							{
								WorldGen.SpreadGrass(num784, num785);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Guide", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				if (Main.tenthAnniversaryWorld)
				{
					BirthdayParty.GenuineParty = true;
					BirthdayParty.PartyDaysOnCooldown = 5;
					if (WorldGen.getGoodWorldGen)
					{
						Main.afterPartyOfDoom = true;
					}
					int num788;
					if (WorldGen.remixWorldGen)
					{
						num788 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 441);
						NPC.savedTaxCollector = true;
					}
					else
					{
						num788 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 22);
					}
					Main.npc[num788].homeTileX = Main.spawnTileX;
					Main.npc[num788].homeTileY = Main.spawnTileY;
					Main.npc[num788].direction = 1;
					Main.npc[num788].homeless = true;
					Main.npc[num788].GivenName = Language.GetTextValue("GuideNames.Andrew");
					BirthdayParty.CelebratingNPCs.Add(num788);
					Point adjustedFloorPosition = GetAdjustedFloorPosition(Main.spawnTileX + 2, Main.spawnTileY);
					num788 = NPC.NewNPC(new EntitySource_WorldGen(), adjustedFloorPosition.X * 16, adjustedFloorPosition.Y * 16, 178);
					Main.npc[num788].homeTileX = adjustedFloorPosition.X;
					Main.npc[num788].homeTileY = adjustedFloorPosition.Y;
					Main.npc[num788].direction = -1;
					Main.npc[num788].homeless = true;
					Main.npc[num788].GivenName = Language.GetTextValue("SteampunkerNames.Whitney");
					BirthdayParty.CelebratingNPCs.Add(num788);
					adjustedFloorPosition = GetAdjustedFloorPosition(Main.spawnTileX - 2, Main.spawnTileY);
					num788 = NPC.NewNPC(new EntitySource_WorldGen(), adjustedFloorPosition.X * 16, adjustedFloorPosition.Y * 16, 663);
					Main.npc[num788].homeTileX = adjustedFloorPosition.X;
					Main.npc[num788].homeTileY = adjustedFloorPosition.Y;
					Main.npc[num788].direction = 1;
					Main.npc[num788].homeless = true;
					Main.npc[num788].GivenName = Language.GetTextValue("PrincessNames.Yorai");
					BirthdayParty.CelebratingNPCs.Add(num788);
					NPC.unlockedPrincessSpawn = true;
					adjustedFloorPosition = GetAdjustedFloorPosition(Main.spawnTileX + 4, Main.spawnTileY);
					num788 = NPC.NewNPC(new EntitySource_WorldGen(), adjustedFloorPosition.X * 16, adjustedFloorPosition.Y * 16, 208);
					Main.npc[num788].homeTileX = adjustedFloorPosition.X;
					Main.npc[num788].homeTileY = adjustedFloorPosition.Y;
					Main.npc[num788].direction = -1;
					Main.npc[num788].homeless = true;
					Main.npc[num788].GivenName = Language.GetTextValue("PartyGirlNames.Amanda");
					BirthdayParty.CelebratingNPCs.Add(num788);
					NPC.unlockedPartyGirlSpawn = true;
					adjustedFloorPosition = GetAdjustedFloorPosition(Main.spawnTileX - 4, Main.spawnTileY);
					if (Main.remixWorld)
					{
						num788 = NPC.NewNPC(new EntitySource_WorldGen(), adjustedFloorPosition.X * 16, adjustedFloorPosition.Y * 16, 681);
						Main.npc[num788].GivenName = Language.GetTextValue("SlimeNames_Rainbow.Slimestar");
						NPC.unlockedSlimeRainbowSpawn = true;
					}
					else
					{
						num788 = NPC.NewNPC(new EntitySource_WorldGen(), adjustedFloorPosition.X * 16, adjustedFloorPosition.Y * 16, 656);
						NPC.boughtBunny = true;
						Main.npc[num788].townNpcVariationIndex = 1;
					}
					Main.npc[num788].homeTileX = adjustedFloorPosition.X;
					Main.npc[num788].homeTileY = adjustedFloorPosition.Y;
					Main.npc[num788].direction = 1;
					Main.npc[num788].homeless = true;
				}
				else if (WorldGen.remixWorldGen)
				{
					int num789 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 441);
					Main.npc[num789].homeTileX = Main.spawnTileX;
					Main.npc[num789].homeTileY = Main.spawnTileY;
					Main.npc[num789].direction = 1;
					Main.npc[num789].homeless = true;
					NPC.savedTaxCollector = true;
				}
				else if (WorldGen.notTheBees)
				{
					int num790 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 17);
					Main.npc[num790].homeTileX = Main.spawnTileX;
					Main.npc[num790].homeTileY = Main.spawnTileY;
					Main.npc[num790].direction = 1;
					Main.npc[num790].homeless = true;
					NPC.unlockedMerchantSpawn = true;
				}
				else if (WorldGen.drunkWorldGen)
				{
					int num791 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 208);
					Main.npc[num791].homeTileX = Main.spawnTileX;
					Main.npc[num791].homeTileY = Main.spawnTileY;
					Main.npc[num791].direction = 1;
					Main.npc[num791].homeless = true;
					NPC.unlockedPartyGirlSpawn = true;
				}
				else if (WorldGen.getGoodWorldGen)
				{
					int num792 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 38);
					Main.npc[num792].homeTileX = Main.spawnTileX;
					Main.npc[num792].homeTileY = Main.spawnTileY;
					Main.npc[num792].direction = 1;
					Main.npc[num792].homeless = true;
					NPC.unlockedDemolitionistSpawn = true;
				}
				else
				{
					int num793 = NPC.NewNPC(new EntitySource_WorldGen(), Main.spawnTileX * 16, Main.spawnTileY * 16, 22);
					Main.npc[num793].homeTileX = Main.spawnTileX;
					Main.npc[num793].homeTileY = Main.spawnTileY;
					Main.npc[num793].direction = 1;
					Main.npc[num793].homeless = true;
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Sunflowers", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[39].Value;
				double num794 = (double)Main.maxTilesX * 0.002;
				for (int num795 = 0; (double)num795 < num794; num795++)
				{
					progress.Set((double)num795 / num794);
					int num796 = 0;
					int num797 = 0;
					_ = Main.maxTilesX / 2;
					int num798 = WorldGen.genRand.Next(Main.maxTilesX);
					num796 = num798 - WorldGen.genRand.Next(10) - 7;
					num797 = num798 + WorldGen.genRand.Next(10) + 7;
					if (num796 < 0)
					{
						num796 = 0;
					}
					if (num797 > Main.maxTilesX - 1)
					{
						num797 = Main.maxTilesX - 1;
					}
					int num799 = 1;
					int num800 = (int)Main.worldSurface - 1;
					for (int num801 = num796; num801 < num797; num801++)
					{
						for (int num802 = num799; num802 < num800; num802++)
						{
							if (Main.tile[num801, num802].TileType == 2 && Main.tile[num801, num802].HasTile && !Main.tile[num801, num802 - 1].HasTile)
							{
								WorldGen.PlaceTile(num801, num802 - 1, 27, mute: true);
							}
							if (Main.tile[num801, num802].HasTile)
							{
								break;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Planting Trees", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[40].Value;
				if (!WorldGen.drunkWorldGen && !Main.tenthAnniversaryWorld)
				{
					for (int num803 = 0; (double)num803 < (double)Main.maxTilesX * 0.003; num803++)
					{
						progress.Set((double)num803 / ((double)Main.maxTilesX * 0.003));
						int num804 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
						int num805 = WorldGen.genRand.Next(25, 50);
						for (int num806 = num804 - num805; num806 < num804 + num805; num806++)
						{
							for (int num807 = 20; (double)num807 < Main.worldSurface; num807++)
							{
								WorldGen.GrowEpicTree(num806, num807);
							}
						}
					}
				}
				WorldGen.AddTrees();
			}));
			vanillaGenPasses.Add(new PassLegacy("Herbs", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				double num808 = (double)Main.maxTilesX * 1.7;
				if (WorldGen.remixWorldGen)
				{
					num808 *= 5.0;
				}
				progress.Message = Lang.gen[41].Value;
				for (int num809 = 0; (double)num809 < num808; num809++)
				{
					progress.Set((double)num809 / num808);
					WorldGen.PlantAlch();
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Dye Plants", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num810 = 0; num810 < Main.maxTilesX; num810++)
				{
					WorldGen.plantDye(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next(100, Main.UnderworldLayer));
				}
				MatureTheHerbPlants();
				GrowGlowTulips();
			}));
			vanillaGenPasses.Add(new PassLegacy("Webs And Honey", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num811 = 100; num811 < Main.maxTilesX - 100; num811++)
				{
					int num812 = (int)Main.worldSurface;
					if (WorldGen.dontStarveWorldGen)
					{
						num812 = 50;
					}
					for (int num813 = num812; num813 < Main.maxTilesY - 100; num813++)
					{
						if (Main.tile[num811, num813].WallType == 86)
						{
							if (Main.tile[num811, num813].LiquidAmount > 0)
							{
								SetLiquidToHoney(Main.tile[num811, num813], true);
							}
							if (WorldGen.genRand.Next(3) == 0)
							{
								WorldGen.PlaceTight(num811, num813);
							}
						}
						if (Main.tile[num811, num813].WallType == 62)
						{
							Main.tile[num811, num813].LiquidAmount = 0;
							SetLiquidToLava(Main.tile[num811, num813], false);
						}
						if (Main.tile[num811, num813].WallType == 62 && !Main.tile[num811, num813].HasTile && WorldGen.genRand.Next(10) != 0)
						{
							int num814 = WorldGen.genRand.Next(2, 5);
							int num815 = num811 - num814;
							int num816 = num811 + num814;
							int num817 = num813 - num814;
							int num818 = num813 + num814;
							bool flag59 = false;
							for (int num819 = num815; num819 <= num816; num819++)
							{
								for (int num820 = num817; num820 <= num818; num820++)
								{
									if (WorldGen.SolidTile(num819, num820))
									{
										flag59 = true;
										break;
									}
								}
							}
							if (flag59)
							{
								WorldGen.PlaceTile(num811, num813, 51, mute: true);
								WorldGen.TileFrame(num811, num813);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Weeds", delegate (GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[42].Value;
				if (Main.halloween)
				{
					for (int num821 = 40; num821 < Main.maxTilesX - 40; num821++)
					{
						for (int num822 = 50; (double)num822 < Main.worldSurface; num822++)
						{
							if (Main.tile[num821, num822].HasTile && Main.tile[num821, num822].TileType == 2 && WorldGen.genRand.Next(15) == 0)
							{
								WorldGen.PlacePumpkin(num821, num822 - 1);
								int num823 = WorldGen.genRand.Next(5);
								for (int num824 = 0; num824 < num823; num824++)
								{
									WorldGen.GrowPumpkin(num821, num822 - 1, 254);
								}
							}
						}
					}
				}
				for (int num825 = 0; num825 < Main.maxTilesX; num825++)
				{
					progress.Set((double)num825 / (double)Main.maxTilesX);
					for (int num826 = 1; num826 < Main.maxTilesY; num826++)
					{
						if (Main.tile[num825, num826].TileType == 2 && Main.tile[num825, num826].HasUnactuatedTile)
						{
							if (!Main.tile[num825, num826 - 1].HasTile)
							{
								WorldGen.PlaceTile(num825, num826 - 1, 3, mute: true);
								Main.tile[num825, num826 - 1].CopyPaintAndCoating(Main.tile[num825, num826]);
							}
						}
						else if (Main.tile[num825, num826].TileType == 23 && Main.tile[num825, num826].HasUnactuatedTile)
						{
							if (!Main.tile[num825, num826 - 1].HasTile)
							{
								WorldGen.PlaceTile(num825, num826 - 1, 24, mute: true);
							}
						}
						else if (Main.tile[num825, num826].TileType == 199 && Main.tile[num825, num826].HasUnactuatedTile)
						{
							if (!Main.tile[num825, num826 - 1].HasTile)
							{
								WorldGen.PlaceTile(num825, num826 - 1, 201, mute: true);
							}
						}
						else if (Main.tile[num825, num826].TileType == 633 && Main.tile[num825, num826].HasUnactuatedTile && !Main.tile[num825, num826 - 1].HasTile)
						{
							WorldGen.PlaceTile(num825, num826 - 1, 637, mute: true);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Glowing Mushrooms and Jungle Plants", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num827 = 0; num827 < Main.maxTilesX; num827++)
				{
					for (int num828 = 0; num828 < Main.maxTilesY; num828++)
					{
						if (Main.tile[num827, num828].HasTile)
						{
							if (num828 >= (int)Main.worldSurface && Main.tile[num827, num828].TileType == 70 && !Main.tile[num827, num828 - 1].HasTile)
							{
								WorldGen.GrowTree(num827, num828);
								if (!Main.tile[num827, num828 - 1].HasTile)
								{
									WorldGen.GrowTree(num827, num828);
									if (!Main.tile[num827, num828 - 1].HasTile)
									{
										WorldGen.GrowTree(num827, num828);
										if (!Main.tile[num827, num828 - 1].HasTile)
										{
											WorldGen.PlaceTile(num827, num828 - 1, 71, mute: true);
										}
									}
								}
							}
							if (Main.tile[num827, num828].TileType == 60 && !Main.tile[num827, num828 - 1].HasTile)
							{
								WorldGen.PlaceTile(num827, num828 - 1, 61, mute: true);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Jungle Plants", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num829 = 0; num829 < Main.maxTilesX * 100; num829++)
				{
					int num830 = WorldGen.genRand.Next(40, Main.maxTilesX / 2 - 40);
					if (GenVars.dungeonSide < 0)
					{
						num830 += Main.maxTilesX / 2;
					}
					int num831;
					for (num831 = WorldGen.genRand.Next(Main.maxTilesY - 300); !Main.tile[num830, num831].HasTile && num831 < Main.maxTilesY - 300; num831++)
					{
					}
					if (Main.tile[num830, num831].HasTile && Main.tile[num830, num831].TileType == 60)
					{
						num831--;
						WorldGen.PlaceJunglePlant(num830, num831, 233, WorldGen.genRand.Next(8), 0);
						if (Main.tile[num830, num831].TileType != 233)
						{
							WorldGen.PlaceJunglePlant(num830, num831, 233, WorldGen.genRand.Next(12), 1);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Vines", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[43].Value;
				for (int num832 = 5; num832 < Main.maxTilesX - 5; num832++)
				{
					progress.Set((double)num832 / (double)Main.maxTilesX);
					int num833 = 0;
					ushort num834 = 52;
					int num835 = (int)Main.worldSurface;
					if (WorldGen.remixWorldGen)
					{
						num835 = Main.maxTilesY - 200;
					}
					for (int num836 = 0; num836 < num835; num836++)
					{
						if (num833 > 0 && !Main.tile[num832, num836].HasTile)
						{
							var tile = Main.tile[num832, num836];
							tile.HasTile = true;
							Main.tile[num832, num836].TileType = num834;
							Main.tile[num832, num836].CopyPaintAndCoating(Main.tile[num832, num836 - 1]);
							num833--;
						}
						else
						{
							num833 = 0;
						}
						if (Main.tile[num832, num836].HasTile && !Main.tile[num832, num836].BottomSlope && (Main.tile[num832, num836].TileType == 2 || (Main.tile[num832, num836].TileType == 192 && WorldGen.genRand.Next(4) == 0)) && WorldGen.GrowMoreVines(num832, num836))
						{
							num834 = 52;
							if (Main.tile[num832, num836].WallType == 68 || Main.tile[num832, num836].WallType == 65 || Main.tile[num832, num836].WallType == 66 || Main.tile[num832, num836].WallType == 63)
							{
								num834 = 382;
							}
							else if (Main.tile[num832, num836 + 1].WallType == 68 || Main.tile[num832, num836 + 1].WallType == 65 || Main.tile[num832, num836 + 1].WallType == 66 || Main.tile[num832, num836 + 1].WallType == 63)
							{
								num834 = 382;
							}
							if (WorldGen.remixWorldGen && WorldGen.genRand.Next(5) == 0)
							{
								num834 = 382;
							}
							if (WorldGen.genRand.Next(5) < 3)
							{
								num833 = WorldGen.genRand.Next(1, 10);
							}
						}
					}
					num833 = 0;
					for (int num837 = 5; num837 < Main.maxTilesY - 5; num837++)
					{
						if (num833 > 0 && !Main.tile[num832, num837].HasTile)
						{
							var tile = Main.tile[num832, num837];
							tile.HasTile = true;
							Main.tile[num832, num837].TileType = 62;
							num833--;
						}
						else
						{
							num833 = 0;
						}
						if (Main.tile[num832, num837].HasTile && Main.tile[num832, num837].TileType == 60 && !Main.tile[num832, num837].BottomSlope && WorldGen.GrowMoreVines(num832, num837))
						{
							if (WorldGen.notTheBees && num837 < Main.maxTilesY - 10 && Main.tile[num832, num837 - 1].HasTile && !Main.tile[num832, num837 - 1].BottomSlope && Main.tile[num832 + 1, num837 - 1].HasTile && !Main.tile[num832 + 1, num837 - 1].BottomSlope && (Main.tile[num832, num837 - 1].TileType == 60 || Main.tile[num832, num837 - 1].TileType == 444 || Main.tile[num832, num837 - 1].TileType == 230))
							{
								bool flag60 = true;
								for (int num838 = num832; num838 < num832 + 2; num838++)
								{
									for (int num839 = num837 + 1; num839 < num837 + 3; num839++)
									{
										if (Main.tile[num838, num839].HasTile && (!Main.tileCut[Main.tile[num838, num839].TileType] || Main.tile[num838, num839].TileType == 444))
										{
											flag60 = false;
											break;
										}
										if (Main.tile[num838, num839].LiquidAmount > 0 || Main.wallHouse[Main.tile[num838, num839].WallType])
										{
											flag60 = false;
											break;
										}
									}
									if (!flag60)
									{
										break;
									}
								}
								if (flag60 && WorldGen.CountNearBlocksTypes(num832, num837, WorldGen.genRand.Next(3, 10), 1, 444) > 0)
								{
									flag60 = false;
								}
								if (flag60)
								{
									for (int num840 = num832; num840 < num832 + 2; num840++)
									{
										for (int num841 = num837 + 1; num841 < num837 + 3; num841++)
										{
											WorldGen.KillTile(num840, num841);
										}
									}
									for (int num842 = num832; num842 < num832 + 2; num842++)
									{
										for (int num843 = num837 + 1; num843 < num837 + 3; num843++)
										{
											var tile = Main.tile[num842, num843];
											tile.HasTile = true;
											Main.tile[num842, num843].TileType = 444;
											Main.tile[num842, num843].TileFrameX = (short)((num842 - num832) * 18);
											Main.tile[num842, num843].TileFrameY = (short)((num843 - num837 - 1) * 18);
										}
									}
									continue;
								}
							}
							else if (num832 < Main.maxTilesX - 1 && num837 < Main.maxTilesY - 2 && Main.tile[num832 + 1, num837].HasTile && Main.tile[num832 + 1, num837].TileType == 60 && !Main.tile[num832 + 1, num837].BottomSlope && WorldGen.genRand.Next(40) == 0)
							{
								bool flag61 = true;
								for (int num844 = num832; num844 < num832 + 2; num844++)
								{
									for (int num845 = num837 + 1; num845 < num837 + 3; num845++)
									{
										if (Main.tile[num844, num845].HasTile && (!Main.tileCut[Main.tile[num844, num845].TileType] || Main.tile[num844, num845].TileType == 444))
										{
											flag61 = false;
											break;
										}
										if (Main.tile[num844, num845].LiquidAmount > 0 || Main.wallHouse[Main.tile[num844, num845].WallType])
										{
											flag61 = false;
											break;
										}
									}
									if (!flag61)
									{
										break;
									}
								}
								if (flag61 && WorldGen.CountNearBlocksTypes(num832, num837, 20, 1, 444) > 0)
								{
									flag61 = false;
								}
								if (flag61)
								{
									for (int num846 = num832; num846 < num832 + 2; num846++)
									{
										for (int num847 = num837 + 1; num847 < num837 + 3; num847++)
										{
											WorldGen.KillTile(num846, num847);
										}
									}
									for (int num848 = num832; num848 < num832 + 2; num848++)
									{
										for (int num849 = num837 + 1; num849 < num837 + 3; num849++)
										{
											var tile = Main.tile[num848, num849];
											tile.HasTile = true;
											Main.tile[num848, num849].TileType = 444;
											Main.tile[num848, num849].TileFrameX = (short)((num848 - num832) * 18);
											Main.tile[num848, num849].TileFrameY = (short)((num849 - num837 - 1) * 18);
										}
									}
									continue;
								}
							}
							if (WorldGen.genRand.Next(5) < 3)
							{
								num833 = WorldGen.genRand.Next(1, 10);
							}
						}
					}
					num833 = 0;
					for (int num850 = 0; num850 < Main.maxTilesY; num850++)
					{
						if (num833 > 0 && !Main.tile[num832, num850].HasTile)
						{
							var tile = Main.tile[num832, num850];
							tile.HasTile = true;
							Main.tile[num832, num850].TileType = 528;
							num833--;
						}
						else
						{
							num833 = 0;
						}
						if (Main.tile[num832, num850].HasTile && Main.tile[num832, num850].TileType == 70 && WorldGen.genRand.Next(5) == 0 && !Main.tile[num832, num850].BottomSlope && WorldGen.GrowMoreVines(num832, num850) && WorldGen.genRand.Next(5) < 3)
						{
							num833 = WorldGen.genRand.Next(1, 10);
						}
					}
					num833 = 0;
					for (int num851 = 0; num851 < Main.maxTilesY; num851++)
					{
						if (num833 > 0 && !Main.tile[num832, num851].HasTile)
						{
							var tile = Main.tile[num832, num851];
							tile.HasTile = true;
							Main.tile[num832, num851].TileType = 636;
							num833--;
						}
						else
						{
							num833 = 0;
						}
						if (Main.tile[num832, num851].HasTile && !Main.tile[num832, num851].BottomSlope && Main.tile[num832, num851].TileType == 23 && WorldGen.GrowMoreVines(num832, num851) && WorldGen.genRand.Next(5) < 3)
						{
							num833 = WorldGen.genRand.Next(1, 10);
						}
					}
					num833 = 0;
					for (int num852 = 0; num852 < Main.maxTilesY; num852++)
					{
						if (num833 > 0 && !Main.tile[num832, num852].HasTile)
						{
							var tile = Main.tile[num832, num852];
							tile.HasTile = true;
							Main.tile[num832, num852].TileType = 205;
							num833--;
						}
						else
						{
							num833 = 0;
						}
						if (Main.tile[num832, num852].HasTile && !Main.tile[num832, num852].BottomSlope && Main.tile[num832, num852].TileType == 199 && WorldGen.GrowMoreVines(num832, num852) && WorldGen.genRand.Next(5) < 3)
						{
							num833 = WorldGen.genRand.Next(1, 10);
						}
					}
					num833 = 0;
					for (int num853 = 0; num853 < Main.maxTilesY; num853++)
					{
						if (num833 > 0 && !Main.tile[num832, num853].HasTile)
						{
							var tile = Main.tile[num832, num853];
							tile.HasTile = true;
							Main.tile[num832, num853].TileType = 638;
							num833--;
						}
						else
						{
							num833 = 0;
						}
						if (Main.tile[num832, num853].HasTile && !Main.tile[num832, num853].BottomSlope && Main.tile[num832, num853].TileType == 633 && WorldGen.GrowMoreVines(num832, num853) && WorldGen.genRand.Next(5) < 3)
						{
							num833 = WorldGen.genRand.Next(1, 10);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Flowers", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[44].Value;
				int num854 = (int)((double)Main.maxTilesX * 0.004);
				if (WorldGen.remixWorldGen)
				{
					num854 *= 6;
				}
				for (int num855 = 0; num855 < num854; num855++)
				{
					progress.Set((double)num855 / (double)num854);
					int num856 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
					int num857 = WorldGen.genRand.Next(15, 30);
					int num858 = WorldGen.genRand.Next(15, 30);
					if (WorldGen.remixWorldGen)
					{
						num857 = WorldGen.genRand.Next(15, 45);
						num858 = WorldGen.genRand.Next(15, 45);
						int num859 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 350);
						if (GenVars.logX >= 0)
						{
							num856 = GenVars.logX;
							num859 = GenVars.logY;
							GenVars.logX = -1;
						}
						int num860 = WorldGen.genRand.NextFromList<int>(21, 24, 27, 30, 33, 36, 39, 42);
						for (int num861 = num856 - num857; num861 < num856 + num857; num861++)
						{
							for (int num862 = num859 - num858; num862 < num859 + num858; num862++)
							{
								if (Main.tile[num861, num862].TileType != 488 && !Main.tileSolid[Main.tile[num861, num862].TileType])
								{
									if (Main.tile[num861, num862].TileType == 3)
									{
										Main.tile[num861, num862].TileFrameX = (short)((num860 + WorldGen.genRand.Next(3)) * 18);
										if (WorldGen.genRand.Next(3) != 0)
										{
											Main.tile[num861, num862].TileType = 73;
										}
									}
									else if (Main.tile[num861, num862 + 1].WallType == 0 && (Main.tile[num861, num862 + 1].TileType == 2 || ((Main.tile[num861, num862 + 1].TileType == 40 || Main.tile[num861, num862 + 1].TileType == 1 || TileID.Sets.Ore[Main.tile[num861, num862 + 1].TileType]) && !Main.tile[num861, num862].HasTile)) && (!Main.tile[num861, num862].HasTile || Main.tile[num861, num862].TileType == 185 || Main.tile[num861, num862].TileType == 186 || Main.tile[num861, num862].TileType == 187 || (Main.tile[num861, num862].TileType == 5 && (double)num861 < (double)Main.maxTilesX * 0.48) || (double)num861 > (double)Main.maxTilesX * 0.52))
									{
										if (Main.tile[num861, num862 + 1].TileType == 40 || Main.tile[num861, num862 + 1].TileType == 1 || TileID.Sets.Ore[Main.tile[num861, num862 + 1].TileType])
										{
											Main.tile[num861, num862 + 1].TileType = 2;
											if (Main.tile[num861, num862 + 2].TileType == 40 || Main.tile[num861, num862 + 2].TileType == 1 || TileID.Sets.Ore[Main.tile[num861, num862 + 2].TileType])
											{
												Main.tile[num861, num862 + 2].TileType = 2;
											}
										}
										WorldGen.KillTile(num861, num862);
										if (WorldGen.genRand.Next(2) == 0)
										{
											var tile = Main.tile[num861, num862 + 1];
											tile.Slope = 0;
											tile.IsHalfBlock = false;
										}
										WorldGen.PlaceTile(num861, num862, 3);
										if (Main.tile[num861, num862].HasTile && Main.tile[num861, num862].TileType == 3)
										{
											Main.tile[num861, num862].TileFrameX = (short)((num860 + WorldGen.genRand.Next(3)) * 18);
											if (WorldGen.genRand.Next(3) != 0)
											{
												Main.tile[num861, num862].TileType = 73;
											}
										}
										if (Main.tile[num861, num862 + 2].TileType == 40 || Main.tile[num861, num862 + 2].TileType == 1 || TileID.Sets.Ore[Main.tile[num861, num862 + 2].TileType])
										{
											Main.tile[num861, num862 + 2].TileType = 0;
										}
									}
								}
							}
						}
					}
					else
					{
						for (int num863 = num858; (double)num863 < Main.worldSurface - (double)num858 - 1.0; num863++)
						{
							if (Main.tile[num856, num863].HasTile)
							{
								if (GenVars.logX >= 0)
								{
									num856 = GenVars.logX;
									num863 = GenVars.logY;
									GenVars.logX = -1;
								}
								int num864 = WorldGen.genRand.NextFromList<int>(21, 24, 27, 30, 33, 36, 39, 42);
								for (int num865 = num856 - num857; num865 < num856 + num857; num865++)
								{
									for (int num866 = num863 - num858; num866 < num863 + num858; num866++)
									{
										if (Main.tile[num865, num866].TileType != 488 && !Main.tileSolid[Main.tile[num865, num866].TileType])
										{
											if (Main.tile[num865, num866].TileType == 3)
											{
												Main.tile[num865, num866].TileFrameX = (short)((num864 + WorldGen.genRand.Next(3)) * 18);
												if (WorldGen.genRand.Next(3) != 0)
												{
													Main.tile[num865, num866].TileType = 73;
												}
											}
											else if (Main.tile[num865, num866 + 1].WallType == 0 && (Main.tile[num865, num866 + 1].TileType == 2 || ((Main.tile[num865, num866 + 1].TileType == 40 || Main.tile[num865, num866 + 1].TileType == 1 || TileID.Sets.Ore[Main.tile[num865, num866 + 1].TileType]) && !Main.tile[num865, num866].HasTile)) && (!Main.tile[num865, num866].HasTile || Main.tile[num865, num866].TileType == 185 || Main.tile[num865, num866].TileType == 186 || Main.tile[num865, num866].TileType == 187 || (Main.tile[num865, num866].TileType == 5 && (double)num865 < (double)Main.maxTilesX * 0.48) || (double)num865 > (double)Main.maxTilesX * 0.52))
											{
												if (Main.tile[num865, num866 + 1].TileType == 40 || Main.tile[num865, num866 + 1].TileType == 1 || TileID.Sets.Ore[Main.tile[num865, num866 + 1].TileType])
												{
													Main.tile[num865, num866 + 1].TileType = 2;
													if (Main.tile[num865, num866 + 2].TileType == 40 || Main.tile[num865, num866 + 2].TileType == 1 || TileID.Sets.Ore[Main.tile[num865, num866 + 2].TileType])
													{
														Main.tile[num865, num866 + 2].TileType = 2;
													}
												}
												WorldGen.KillTile(num865, num866);
												if (WorldGen.genRand.Next(2) == 0)
												{
													var tile = Main.tile[num865, num866 + 1];
													tile.Slope = 0;
													tile.IsHalfBlock = false;
												}
												WorldGen.PlaceTile(num865, num866, 3);
												if (Main.tile[num865, num866].HasTile && Main.tile[num865, num866].TileType == 3)
												{
													Main.tile[num865, num866].TileFrameX = (short)((num864 + WorldGen.genRand.Next(3)) * 18);
													if (WorldGen.genRand.Next(3) != 0)
													{
														Main.tile[num865, num866].TileType = 73;
													}
												}
												if (Main.tile[num865, num866 + 2].TileType == 40 || Main.tile[num865, num866 + 2].TileType == 1 || TileID.Sets.Ore[Main.tile[num865, num866 + 2].TileType])
												{
													Main.tile[num865, num866 + 2].TileType = 0;
												}
											}
										}
									}
								}
								break;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Mushrooms", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[45].Value;
				int num867 = (int)((double)Main.maxTilesX * 0.002);
				if (WorldGen.remixWorldGen)
				{
					num867 *= 9;
				}
				for (int num868 = 0; num868 < num867; num868++)
				{
					progress.Set((double)num868 / (double)num867);
					int num869 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num870 = WorldGen.genRand.Next(4, 10);
					int num871 = WorldGen.genRand.Next(15, 30);
					if (WorldGen.remixWorldGen)
					{
						num870 = WorldGen.genRand.Next(8, 17);
						num871 = WorldGen.genRand.Next(8, 17);
						int num872 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 350);
						if (Main.tile[num869, num872].HasTile)
						{
							for (int num873 = num869 - num870; num873 < num869 + num870; num873++)
							{
								for (int num874 = num872 - num871; num874 < num872 + num871; num874++)
								{
									if (num873 < 10)
									{
										break;
									}
									if (num874 < 0)
									{
										break;
									}
									if (num873 > Main.maxTilesX - 10)
									{
										break;
									}
									if (num874 > Main.maxTilesY - 10)
									{
										break;
									}
									if (Main.tile[num873, num874].TileType == 3 || Main.tile[num873, num874].TileType == 24)
									{
										Main.tile[num873, num874].TileFrameX = 144;
									}
									else if (Main.tile[num873, num874].TileType == 201)
									{
										Main.tile[num873, num874].TileFrameX = 270;
									}
								}
							}
						}
					}
					else
					{
						for (int num875 = 1; (double)num875 < Main.worldSurface - 1.0; num875++)
						{
							if (Main.tile[num869, num875].HasTile)
							{
								for (int num876 = num869 - num870; num876 < num869 + num870; num876++)
								{
									for (int num877 = num875 - num871; num877 < num875 + num871; num877++)
									{
										if (num876 < 10)
										{
											break;
										}
										if (num877 < 0)
										{
											break;
										}
										if (num876 > Main.maxTilesX - 10)
										{
											break;
										}
										if (num877 > Main.maxTilesY - 10)
										{
											break;
										}
										if (Main.tile[num876, num877].TileType == 3 || Main.tile[num876, num877].TileType == 24)
										{
											Main.tile[num876, num877].TileFrameX = 144;
										}
										else if (Main.tile[num876, num877].TileType == 201)
										{
											Main.tile[num876, num877].TileFrameX = 270;
										}
									}
								}
								break;
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Gems In Ice Biome", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num878 = 0; (double)num878 < (double)Main.maxTilesX * 0.25; num878++)
				{
					int num879 = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, GenVars.lavaLine) : WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300));
					int num880 = WorldGen.genRand.Next(GenVars.snowMinX[num879], GenVars.snowMaxX[num879]);
					if (Main.tile[num880, num879].HasTile && (Main.tile[num880, num879].TileType == 147 || Main.tile[num880, num879].TileType == 161 || Main.tile[num880, num879].TileType == 162 || Main.tile[num880, num879].TileType == 224))
					{
						int num881 = WorldGen.genRand.Next(1, 4);
						int num882 = WorldGen.genRand.Next(1, 4);
						int num883 = WorldGen.genRand.Next(1, 4);
						int num884 = WorldGen.genRand.Next(1, 4);
						int num885 = WorldGen.genRand.Next(12);
						int num886 = 0;
						num886 = ((num885 >= 3) ? ((num885 < 6) ? 1 : ((num885 < 8) ? 2 : ((num885 < 10) ? 3 : ((num885 >= 11) ? 5 : 4)))) : 0);
						for (int num887 = num880 - num881; num887 < num880 + num882; num887++)
						{
							for (int num888 = num879 - num883; num888 < num879 + num884; num888++)
							{
								if (!Main.tile[num887, num888].HasTile)
								{
									WorldGen.PlaceTile(num887, num888, 178, mute: true, forced: false, -1, num886);
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Random Gems", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num889 = 0; num889 < Main.maxTilesX; num889++)
				{
					int num890 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num891 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 300);
					if (!Main.tile[num890, num891].HasTile && !IsLava(Main.tile[num890, num891]) && !Main.wallDungeon[Main.tile[num890, num891].WallType] && Main.tile[num890, num891].WallType != 27)
					{
						int num892 = WorldGen.genRand.Next(12);
						int num893 = 0;
						num893 = ((num892 >= 3) ? ((num892 < 6) ? 1 : ((num892 < 8) ? 2 : ((num892 < 10) ? 3 : ((num892 >= 11) ? 5 : 4)))) : 0);
						WorldGen.PlaceTile(num890, num891, 178, mute: true, forced: false, -1, num893);
					}
				}
				for (int num894 = 0; num894 < Main.maxTilesX; num894++)
				{
					int num895 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num896 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300);
					if (!Main.tile[num895, num896].HasTile && !IsLava(Main.tile[num895, num896]) && (Main.tile[num895, num896].WallType == 216 || Main.tile[num895, num896].WallType == 187))
					{
						int num897 = WorldGen.genRand.Next(1, 4);
						int num898 = WorldGen.genRand.Next(1, 4);
						int num899 = WorldGen.genRand.Next(1, 4);
						int num900 = WorldGen.genRand.Next(1, 4);
						for (int num901 = num895 - num897; num901 < num895 + num898; num901++)
						{
							for (int num902 = num896 - num899; num902 < num896 + num900; num902++)
							{
								if (!Main.tile[num901, num902].HasTile)
								{
									WorldGen.PlaceTile(num901, num902, 178, mute: true, forced: false, -1, 6);
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Moss Grass", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num903 = 5; num903 < Main.maxTilesX - 5; num903++)
				{
					for (int num904 = 5; num904 < Main.maxTilesY - 5; num904++)
					{
						if (Main.tile[num903, num904].HasTile && Main.tileMoss[Main.tile[num903, num904].TileType])
						{
							for (int num905 = 0; num905 < 4; num905++)
							{
								int num906 = num903;
								int num907 = num904;
								if (num905 == 0)
								{
									num906--;
								}
								if (num905 == 1)
								{
									num906++;
								}
								if (num905 == 2)
								{
									num907--;
								}
								if (num905 == 3)
								{
									num907++;
								}
								if (!Main.tile[num906, num907].HasTile)
								{
									WorldGen.PlaceTile(num906, num907, 184, mute: true);
								}
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Muds Walls In Jungle", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				int num908 = 0;
				int num909 = 0;
				bool flag62 = false;
				for (int num910 = 5; num910 < Main.maxTilesX - 5; num910++)
				{
					for (int num911 = 0; (double)num911 < Main.worldSurface + 20.0; num911++)
					{
						if (Main.tile[num910, num911].HasTile && Main.tile[num910, num911].TileType == 60)
						{
							num908 = num910;
							flag62 = true;
							break;
						}
					}
					if (flag62)
					{
						break;
					}
				}
				flag62 = false;
				for (int num912 = Main.maxTilesX - 5; num912 > 5; num912--)
				{
					for (int num913 = 0; (double)num913 < Main.worldSurface + 20.0; num913++)
					{
						if (Main.tile[num912, num913].HasTile && Main.tile[num912, num913].TileType == 60)
						{
							num909 = num912;
							flag62 = true;
							break;
						}
					}
					if (flag62)
					{
						break;
					}
				}
				GenVars.jungleMinX = num908;
				GenVars.jungleMaxX = num909;
				for (int num914 = num908; num914 <= num909; num914++)
				{
					for (int num915 = 0; (double)num915 < Main.worldSurface + 20.0; num915++)
					{
						if (((num914 >= num908 + 2 && num914 <= num909 - 2) || WorldGen.genRand.Next(2) != 0) && ((num914 >= num908 + 3 && num914 <= num909 - 3) || WorldGen.genRand.Next(3) != 0) && (Main.tile[num914, num915].WallType == 2 || Main.tile[num914, num915].WallType == 59))
						{
							Main.tile[num914, num915].WallType = 15;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Larva", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				Main.tileSolid[229] = true;
				progress.Set(1.0);
				for (int num916 = 0; num916 < GenVars.numLarva; num916++)
				{
					int num917 = GenVars.larvaX[num916];
					int num918 = GenVars.larvaY[num916];
					for (int num919 = num917 - 1; num919 <= num917 + 1; num919++)
					{
						for (int num920 = num918 - 2; num920 <= num918 + 1; num920++)
						{
							if (num920 != num918 + 1)
							{
								var tile = Main.tile[num919, num920];
								tile.HasTile = false;
							}
							else
							{
								var tile = Main.tile[num919, num920];
								tile.HasTile = true;
								Main.tile[num919, num920].TileType = 225;
								tile.Slope = 0;
								tile.IsHalfBlock = false;
							}
						}
					}
					WorldGen.PlaceTile(num917, num918, 231, mute: true);
				}
				Main.tileSolid[232] = true;
				Main.tileSolid[162] = true;
			}));
			vanillaGenPasses.Add(new PassLegacy("Settle Liquids Again", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (WorldGen.getGoodWorldGen)
				{
					Main.tileSolid[56] = true;
				}
				progress.Message = Lang.gen[27].Value;
				if (WorldGen.notTheBees)
				{
					NotTheBees();
				}
				Liquid.worldGenTilesIgnoreWater(ignoreSolids: true);
				Liquid.QuickWater(3);
				WorldGen.WaterCheck();
				int num921 = 0;
				Liquid.quickSettle = true;
				int num922 = 10;
				while (num921 < num922)
				{
					int num923 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					num921++;
					double num924 = 0.0;
					int num925 = num923 * 5;
					while (Liquid.numLiquid > 0)
					{
						num925--;
						if (num925 < 0)
						{
							break;
						}
						double num926 = (double)(num923 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (double)num923;
						if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num923)
						{
							num923 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
						}
						if (num926 > num924)
						{
							num924 = num926;
						}
						else
						{
							num926 = num924;
						}
						if (num921 == 1)
						{
							progress.Set(num926 / 3.0 + 0.33);
						}
						Liquid.UpdateLiquid();
					}
					WorldGen.WaterCheck();
					progress.Set((double)num921 / (double)num922 / 3.0 + 0.66);
				}
				Liquid.quickSettle = false;
				Liquid.worldGenTilesIgnoreWater(ignoreSolids: false);
				Main.tileSolid[484] = false;
			}));
			vanillaGenPasses.Add(new PassLegacy("Cactus, Palm Trees, & Coral", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[38].Value;
				int num927 = 8;
				if (WorldGen.remixWorldGen)
				{
					num927 = 2;
				}
				int num928 = 400;
				int num929 = WorldGen.genRand.Next(3, 13);
				int num930 = WorldGen.genRand.Next(3, 13);
				WorldGen.genRand.Next(2, 6);
				WorldGen.genRand.Next(2, 6);
				int num931 = 380;
				for (int num932 = 0; num932 < GenVars.numOasis; num932++)
				{
					int num933 = (int)((double)GenVars.oasisWidth[num932] * 1.5);
					for (int num934 = GenVars.oasisPosition[num932].X - num933; num934 <= GenVars.oasisPosition[num932].X + num933; num934++)
					{
						for (int num935 = GenVars.oasisPosition[num932].Y - GenVars.oasisHeight; num935 <= GenVars.oasisPosition[num932].Y + GenVars.oasisHeight; num935++)
						{
							double num936 = 1.0;
							int num937 = 8;
							for (int num938 = num934 - num937; num938 <= num934 + num937; num938++)
							{
								for (int num939 = num935 - num937; num939 <= num935 + num937; num939++)
								{
									if (WorldGen.InWorld(num938, num939) && Main.tile[num938, num939] != null && Main.tile[num938, num939].HasTile && Main.tile[num938, num939].TileType == 323)
									{
										num936 = 0.13;
									}
								}
							}
							if (WorldGen.genRand.NextDouble() < num936)
							{
								WorldGen.GrowPalmTree(num934, num935);
							}
							if (PlantSeaOat(num934, num935))
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									GrowSeaOat(num934, num935);
								}
								if (WorldGen.genRand.Next(2) == 0)
								{
									GrowSeaOat(num934, num935);
								}
							}
							WorldGen.PlaceOasisPlant(num934, num935, 530);
						}
					}
				}
				for (int num940 = 0; num940 < 3; num940++)
				{
					progress.Set((double)num940 / 3.0);
					int num941;
					int num942;
					bool flag63;
					int maxValue6;
					switch (num940)
					{
					default:
						num941 = 5;
						num942 = num931;
						flag63 = false;
						maxValue6 = num929;
						break;
					case 1:
						num941 = num928;
						num942 = Main.maxTilesX - num928;
						flag63 = true;
						maxValue6 = num927;
						break;
					case 2:
						num941 = Main.maxTilesX - num931;
						num942 = Main.maxTilesX - 5;
						flag63 = false;
						maxValue6 = num930;
						break;
					}
					double num943 = Main.worldSurface - 1.0;
					if (WorldGen.remixWorldGen)
					{
						num943 = Main.maxTilesY - 50;
					}
					for (int num944 = num941; num944 < num942; num944++)
					{
						if (WorldGen.genRand.Next(maxValue6) == 0)
						{
							for (int num945 = 0; (double)num945 < num943; num945++)
							{
								Tile tile3 = Main.tile[num944, num945];
								if (tile3.HasTile && (tile3.TileType == 53 || tile3.TileType == 112 || tile3.TileType == 234))
								{
									Tile tile4 = Main.tile[num944, num945 - 1];
									if (!tile4.HasTile && tile4.WallType == 0)
									{
										if (flag63)
										{
											if (WorldGen.remixWorldGen)
											{
												if ((double)num945 > Main.worldSurface)
												{
													if (WorldGen.SolidTile(num944, num945) && Main.tile[num944, num945 + 1].TileType == 53 && Main.tile[num944, num945 + 2].TileType == 53)
													{
														int maxValue7 = 3;
														WorldGen.GrowPalmTree(num944, num945);
														if (!Main.tile[num944, num945 - 1].HasTile && WorldGen.genRand.Next(maxValue7) == 0)
														{
															WorldGen.PlantCactus(num944, num945);
														}
													}
												}
												else
												{
													int num946 = 0;
													for (int num947 = num944 - WorldGen.cactusWaterWidth; num947 < num944 + WorldGen.cactusWaterWidth; num947++)
													{
														for (int num948 = num945 - WorldGen.cactusWaterHeight; num948 < num945 + WorldGen.cactusWaterHeight; num948++)
														{
															num946 += Main.tile[num947, num948].LiquidAmount;
														}
													}
													if (num946 / 255 > WorldGen.cactusWaterLimit)
													{
														int maxValue8 = 4;
														if (WorldGen.genRand.Next(maxValue8) == 0)
														{
															WorldGen.GrowPalmTree(num944, num945);
														}
													}
													else
													{
														WorldGen.PlantCactus(num944, num945);
													}
												}
											}
											else
											{
												int num949 = 0;
												for (int num950 = num944 - WorldGen.cactusWaterWidth; num950 < num944 + WorldGen.cactusWaterWidth; num950++)
												{
													for (int num951 = num945 - WorldGen.cactusWaterHeight; num951 < num945 + WorldGen.cactusWaterHeight; num951++)
													{
														num949 += Main.tile[num950, num951].LiquidAmount;
													}
												}
												if (num949 / 255 > WorldGen.cactusWaterLimit)
												{
													int maxValue9 = 4;
													if (WorldGen.genRand.Next(maxValue9) == 0)
													{
														WorldGen.GrowPalmTree(num944, num945);
													}
												}
												else
												{
													WorldGen.PlantCactus(num944, num945);
												}
											}
										}
										else
										{
											if (Main.tile[num944, num945 - 2].LiquidAmount == byte.MaxValue && Main.tile[num944, num945 - 3].LiquidAmount == byte.MaxValue && Main.tile[num944, num945 - 4].LiquidAmount == byte.MaxValue)
											{
												if (WorldGen.genRand.Next(2) == 0)
												{
													WorldGen.PlaceTile(num944, num945 - 1, 81, mute: true);
												}
												else
												{
													WorldGen.PlaceTile(num944, num945 - 1, 324, mute: true, forced: false, -1, RollRandomSeaShellStyle());
												}
												break;
											}
											if (Main.tile[num944, num945 - 2].LiquidAmount == 0 && (double)num945 < Main.worldSurface)
											{
												WorldGen.PlaceTile(num944, num945 - 1, 324, mute: true, forced: false, -1, RollRandomSeaShellStyle());
												break;
											}
										}
									}
								}
							}
						}
						else
						{
							for (int num952 = 0; (double)num952 < num943; num952++)
							{
								if (PlantSeaOat(num944, num952))
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										GrowSeaOat(num944, num952);
									}
									if (WorldGen.genRand.Next(2) == 0)
									{
										GrowSeaOat(num944, num952);
									}
								}
								WorldGen.PlaceOasisPlant(num944, num952, 530);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Tile Cleanup", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[84].Value;
				for (int num953 = 40; num953 < Main.maxTilesX - 40; num953++)
				{
					progress.Set((double)(num953 - 40) / (double)(Main.maxTilesX - 80));
					for (int num954 = 40; num954 < Main.maxTilesY - 40; num954++)
					{
						if (Main.tile[num953, num954].HasTile && Main.tile[num953, num954].TopSlope && ((Main.tile[num953, num954].LeftSlope && Main.tile[num953 + 1, num954].IsHalfBlock) || (Main.tile[num953, num954].RightSlope && Main.tile[num953 - 1, num954].IsHalfBlock)))
						{
							var tile = Main.tile[num953, num954];
							tile.Slope = 0;
							tile.IsHalfBlock = true;
						}
						if (Main.tile[num953, num954].HasTile && Main.tile[num953, num954].LiquidAmount > 0 && TileID.Sets.SlowlyDiesInWater[Main.tile[num953, num954].TileType])
						{
							WorldGen.KillTile(num953, num954);
						}
						if (!Main.tile[num953, num954].HasTile && Main.tile[num953, num954].LiquidAmount == 0 && WorldGen.genRand.Next(3) != 0 && WorldGen.SolidTile(num953, num954 - 1))
						{
							int num955 = WorldGen.genRand.Next(15, 21);
							for (int num956 = num954 - 2; num956 >= num954 - num955; num956--)
							{
								if (Main.tile[num953, num956].LiquidAmount >= 128 && !IsShimmer(Main.tile[num953, num956]))
								{
									int num957 = 373;
									if (IsLava(Main.tile[num953, num956]))
									{
										num957 = 374;
									}
									else if (IsHoney(Main.tile[num953, num956]))
									{
										num957 = 375;
									}
									int maxValue10 = num954 - num956;
									if (WorldGen.genRand.Next(maxValue10) <= 1)
									{
										if (Main.tile[num953, num954].WallType == 86)
										{
											num957 = 375;
										}
										Main.tile[num953, num954].TileType = (ushort)num957;
										Main.tile[num953, num954].TileFrameX = 0;
										Main.tile[num953, num954].TileFrameY = 0;
										var tile = Main.tile[num953, num954];
										tile.HasTile = true;
										break;
									}
								}
							}
							if (!Main.tile[num953, num954].HasTile)
							{
								num955 = WorldGen.genRand.Next(3, 11);
								for (int num958 = num954 + 1; num958 <= num954 + num955; num958++)
								{
									if (Main.tile[num953, num958].LiquidAmount >= 200 && !IsShimmer(Main.tile[num953, num958]))
									{
										int num959 = 373;
										if (IsLava(Main.tile[num953, num958]))
										{
											num959 = 374;
										}
										else if (IsHoney(Main.tile[num953, num958]))
										{
											num959 = 375;
										}
										int num960 = num958 - num954;
										if (WorldGen.genRand.Next(num960 * 3) <= 1)
										{
											Main.tile[num953, num954].TileType = (ushort)num959;
											Main.tile[num953, num954].TileFrameX = 0;
											Main.tile[num953, num954].TileFrameY = 0;
											var tile = Main.tile[num953, num954];
											tile.HasTile = true;
											break;
										}
									}
								}
							}
							if (!Main.tile[num953, num954].HasTile && WorldGen.genRand.Next(4) == 0)
							{
								Tile tile5 = Main.tile[num953, num954 - 1];
								if (TileID.Sets.Conversion.Sandstone[tile5.TileType] || TileID.Sets.Conversion.HardenedSand[tile5.TileType])
								{
									Main.tile[num953, num954].TileType = 461;
									Main.tile[num953, num954].TileFrameX = 0;
									Main.tile[num953, num954].TileFrameY = 0;
									var tile = Main.tile[num953, num954];
									tile.HasTile = true;
								}
							}
						}
						if (Main.tile[num953, num954].TileType == 137)
						{
							int num961 = Main.tile[num953, num954].TileFrameY / 18;
							if (num961 <= 2 || num961 == 5)
							{
								int num962 = -1;
								if (Main.tile[num953, num954].TileFrameX >= 18)
								{
									num962 = 1;
								}
								if (Main.tile[num953 + num962, num954].IsHalfBlock || Main.tile[num953 + num962, num954].Slope != 0)
								{
									var tile = Main.tile[num953 + num962, num954];
									tile.HasTile = false;
								}
							}
						}
						else if (Main.tile[num953, num954].TileType == 162 && Main.tile[num953, num954 + 1].LiquidAmount == 0 && WorldGen.CanKillTile(num953, num954))
						{
							var tile = Main.tile[num953, num954];
							tile.HasTile = false;
						}
						if (Main.tile[num953, num954].WallType == 13 || Main.tile[num953, num954].WallType == 14)
						{
							Main.tile[num953, num954].LiquidAmount = 0;
						}
						if (Main.tile[num953, num954].TileType == 31)
						{
							int num963 = Main.tile[num953, num954].TileFrameX / 18;
							int num964 = 0;
							int num965 = num953;
							num964 += num963 / 2;
							num964 = (((!WorldGen.drunkWorldGen) ? WorldGen.crimson : (Main.tile[num953, num954].WallType == 83)) ? 1 : 0);
							num963 %= 2;
							num965 -= num963;
							int num966 = Main.tile[num953, num954].TileFrameY / 18;
							int num967 = 0;
							int num968 = num954;
							num967 += num966 / 2;
							num966 %= 2;
							num968 -= num966;
							for (int num969 = 0; num969 < 2; num969++)
							{
								for (int num970 = 0; num970 < 2; num970++)
								{
									int x21 = num965 + num969;
									int y16 = num968 + num970;
									var tile = Main.tile[x21, y16];
									tile.HasTile = true;
									tile.Slope = 0;
									tile.IsHalfBlock = false;
									Main.tile[x21, y16].TileType = 31;
									Main.tile[x21, y16].TileFrameX = (short)(num969 * 18 + 36 * num964);
									Main.tile[x21, y16].TileFrameY = (short)(num970 * 18 + 36 * num967);
								}
							}
						}
						if (Main.tile[num953, num954].TileType == 12)
						{
							int num971 = Main.tile[num953, num954].TileFrameX / 18;
							int num972 = 0;
							int num973 = num953;
							num972 += num971 / 2;
							num971 %= 2;
							num973 -= num971;
							int num974 = Main.tile[num953, num954].TileFrameY / 18;
							int num975 = 0;
							int num976 = num954;
							num975 += num974 / 2;
							num974 %= 2;
							num976 -= num974;
							for (int num977 = 0; num977 < 2; num977++)
							{
								for (int num978 = 0; num978 < 2; num978++)
								{
									int x22 = num973 + num977;
									int y17 = num976 + num978;
									var tile = Main.tile[x22, y17];
									tile.HasTile = true;
									tile.Slope = 0;
									tile.IsHalfBlock = false;
									Main.tile[x22, y17].TileType = 12;
									Main.tile[x22, y17].TileFrameX = (short)(num977 * 18 + 36 * num972);
									Main.tile[x22, y17].TileFrameY = (short)(num978 * 18 + 36 * num975);
								}
								if (!Main.tile[num977, num954 + 2].HasTile)
								{
									var tile = Main.tile[num977, num954 + 2];
									tile.HasTile = true;
									if (!Main.tileSolid[Main.tile[num977, num954 + 2].TileType] || Main.tileSolidTop[Main.tile[num977, num954 + 2].TileType])
									{
										Main.tile[num977, num954 + 2].TileType = 0;
									}
								}
								var tile2 = Main.tile[num977, num954 + 2];
								tile2.Slope = 0;
								tile2.IsHalfBlock = false;
							}
						}
						if (Main.tile[num953, num954].TileType == 639)
						{
							int num979 = Main.tile[num953, num954].TileFrameX / 18;
							int num980 = 0;
							int num981 = num953;
							num980 += num979 / 2;
							num979 %= 2;
							num981 -= num979;
							int num982 = Main.tile[num953, num954].TileFrameY / 18;
							int num983 = 0;
							int num984 = num954;
							num983 += num982 / 2;
							num982 %= 2;
							num984 -= num982;
							for (int num985 = 0; num985 < 2; num985++)
							{
								for (int num986 = 0; num986 < 2; num986++)
								{
									int x23 = num981 + num985;
									int y18 = num984 + num986;
									var tile = Main.tile[x23, y18];
									tile.HasTile = true;
									tile.Slope = 0;
									tile.IsHalfBlock = false;
									Main.tile[x23, y18].TileType = 639;
									Main.tile[x23, y18].TileFrameX = (short)(num985 * 18 + 36 * num980);
									Main.tile[x23, y18].TileFrameY = (short)(num986 * 18 + 36 * num983);
								}
								if (!Main.tile[num985, num954 + 2].HasTile)
								{
									var tile = Main.tile[num985, num954 + 2];
									tile.HasTile = true;
									if (!Main.tileSolid[Main.tile[num985, num954 + 2].TileType] || Main.tileSolidTop[Main.tile[num985, num954 + 2].TileType])
									{
										Main.tile[num985, num954 + 2].TileType = 0;
									}
								}
								var tile2 = Main.tile[num985, num954 + 2];
								tile2.Slope = 0;
								tile2.IsHalfBlock = false;
							}
						}
						if (TileID.Sets.BasicChest[Main.tile[num953, num954].TileType])
						{
							int num987 = Main.tile[num953, num954].TileFrameX / 18;
							int num988 = 0;
							ushort num989 = 21;
							int num990 = num953;
							int num991 = num954 - Main.tile[num953, num954].TileFrameY / 18;
							if (Main.tile[num953, num954].TileType == 467)
							{
								num989 = 467;
							}
							if (TileID.Sets.BasicChest[Main.tile[num953, num954].TileType])
							{
								num989 = Main.tile[num953, num954].TileType;
							}
							while (num987 >= 2)
							{
								num988++;
								num987 -= 2;
							}
							num990 -= num987;
							int num992 = Chest.FindChest(num990, num991);
							if (num992 != -1)
							{
								switch (Main.chest[num992].item[0].type)
								{
								case 1156:
									num988 = 23;
									break;
								case 1571:
									num988 = 24;
									break;
								case 1569:
									num988 = 25;
									break;
								case 1260:
									num988 = 26;
									break;
								case 1572:
									num988 = 27;
									break;
								}
							}
							for (int num993 = 0; num993 < 2; num993++)
							{
								for (int num994 = 0; num994 < 2; num994++)
								{
									int x24 = num990 + num993;
									int y19 = num991 + num994;
									var tile = Main.tile[x24, y19];
									tile.HasTile = true;
									tile.Slope = 0;
									tile.IsHalfBlock = false;
									Main.tile[x24, y19].TileType = num989;
									Main.tile[x24, y19].TileFrameX = (short)(num993 * 18 + 36 * num988);
									Main.tile[x24, y19].TileFrameY = (short)(num994 * 18);
								}
								if (!Main.tile[num993, num954 + 2].HasTile)
								{
									var tile = Main.tile[num993, num954 + 2];
									tile.HasTile = true;
									if (!Main.tileSolid[Main.tile[num993, num954 + 2].TileType] || Main.tileSolidTop[Main.tile[num993, num954 + 2].TileType])
									{
										Main.tile[num993, num954 + 2].TileType = 0;
									}
								}
								var tile2 = Main.tile[num993, num954 + 2];
								tile2.Slope = 0;
								tile2.IsHalfBlock = false;
							}
						}
						if (Main.tile[num953, num954].TileType == 28)
						{
							int num995 = Main.tile[num953, num954].TileFrameX / 18;
							int num996 = 0;
							int num997 = num953;
							while (num995 >= 2)
							{
								num996++;
								num995 -= 2;
							}
							num997 -= num995;
							int num998 = Main.tile[num953, num954].TileFrameY / 18;
							int num999 = 0;
							int num1000 = num954;
							while (num998 >= 2)
							{
								num999++;
								num998 -= 2;
							}
							num1000 -= num998;
							for (int num1001 = 0; num1001 < 2; num1001++)
							{
								for (int num1002 = 0; num1002 < 2; num1002++)
								{
									int x25 = num997 + num1001;
									int y20 = num1000 + num1002;
									var tile = Main.tile[x25, y20];
									tile.HasTile = true;
									tile.Slope = 0;
									tile.IsHalfBlock = false;
									Main.tile[x25, y20].TileType = 28;
									Main.tile[x25, y20].TileFrameX = (short)(num1001 * 18 + 36 * num996);
									Main.tile[x25, y20].TileFrameY = (short)(num1002 * 18 + 36 * num999);
								}
								if (!Main.tile[num1001, num954 + 2].HasTile)
								{
									var tile = Main.tile[num1001, num954 + 2];
									tile.HasTile = true;
									if (!Main.tileSolid[Main.tile[num1001, num954 + 2].TileType] || Main.tileSolidTop[Main.tile[num1001, num954 + 2].TileType])
									{
										Main.tile[num1001, num954 + 2].TileType = 0;
									}
								}
								var tile2 = Main.tile[num1001, num954 + 2];
								tile2.Slope = 0;
								tile2.IsHalfBlock = false;
							}
						}
						if (Main.tile[num953, num954].TileType == 26)
						{
							int num1003 = Main.tile[num953, num954].TileFrameX / 18;
							int num1004 = 0;
							int num1005 = num953;
							int num1006 = num954 - Main.tile[num953, num954].TileFrameY / 18;
							while (num1003 >= 3)
							{
								num1004++;
								num1003 -= 3;
							}
							num1005 -= num1003;
							num1004 = ((WorldGen.drunkWorldGen ? (Main.tile[num953, num954].WallType == 83) : WorldGen.crimson) ? 1 : 0);
							for (int num1007 = 0; num1007 < 3; num1007++)
							{
								for (int num1008 = 0; num1008 < 2; num1008++)
								{
									int x26 = num1005 + num1007;
									int y21 = num1006 + num1008;
									var tile = Main.tile[x26, y21];
									tile.HasTile = true;
									tile.Slope = 0;
									tile.IsHalfBlock = false;
									Main.tile[x26, y21].TileType = 26;
									Main.tile[x26, y21].TileFrameX = (short)(num1007 * 18 + 54 * num1004);
									Main.tile[x26, y21].TileFrameY = (short)(num1008 * 18);
								}
								if (!Main.tile[num1005 + num1007, num1006 + 2].HasTile || !Main.tileSolid[Main.tile[num1005 + num1007, num1006 + 2].TileType] || Main.tileSolidTop[Main.tile[num1005 + num1007, num1006 + 2].TileType])
								{
									var tile = Main.tile[num1005 + num1007, num1006 + 2];
									tile.HasTile = true;
									if (!TileID.Sets.Platforms[Main.tile[num1005 + num1007, num1006 + 2].TileType])
									{
										if (Main.tile[num1005 + num1007, num1006 + 2].TileType == 484)
										{
											Main.tile[num1005 + num1007, num1006 + 2].TileType = 397;
										}
										else if (TileID.Sets.Boulders[Main.tile[num1005 + num1007, num1006 + 2].TileType] || !Main.tileSolid[Main.tile[num1005 + num1007, num1006 + 2].TileType] || Main.tileSolidTop[Main.tile[num1005 + num1007, num1006 + 2].TileType])
										{
											Main.tile[num1005 + num1007, num1006 + 2].TileType = 0;
										}
									}
								}
								var tile2 = Main.tile[num1005 + num1007, num1006 + 2];
								tile2.Slope = 0;
								tile2.IsHalfBlock = false;
								if (Main.tile[num1005 + num1007, num1006 + 3].TileType == 28 && Main.tile[num1005 + num1007, num1006 + 3].TileFrameY % 36 >= 18)
								{
									Main.tile[num1005 + num1007, num1006 + 3].TileType = 0;
									tile2 = Main.tile[num1005 + num1007, num1006 + 3];
									tile2.HasTile = false;
								}
							}
							for (int num1009 = 0; num1009 < 3; num1009++)
							{
								if ((Main.tile[num1005 - 1, num1006 + num1009].TileType == 28 || Main.tile[num1005 - 1, num1006 + num1009].TileType == 12 || Main.tile[num1005 - 1, num1006 + num1009].TileType == 639) && Main.tile[num1005 - 1, num1006 + num1009].TileFrameX % 36 < 18)
								{
									Main.tile[num1005 - 1, num1006 + num1009].TileType = 0;
									var tile = Main.tile[num1005 - 1, num1006 + num1009];
									tile.HasTile = false;
								}
								if ((Main.tile[num1005 + 3, num1006 + num1009].TileType == 28 || Main.tile[num1005 + 3, num1006 + num1009].TileType == 12 || Main.tile[num1005 - 1, num1006 + num1009].TileType == 639) && Main.tile[num1005 + 3, num1006 + num1009].TileFrameX % 36 >= 18)
								{
									Main.tile[num1005 + 3, num1006 + num1009].TileType = 0;
									var tile = Main.tile[num1005 + 3, num1006 + num1009];
									tile.HasTile = false;
								}
							}
						}
						if (Main.tile[num953, num954].TileType == 237 && Main.tile[num953, num954 + 1].TileType == 232)
						{
							Main.tile[num953, num954 + 1].TileType = 226;
						}
						if (Main.tile[num953, num954].WallType == 87)
						{
							Main.tile[num953, num954].LiquidAmount = 0;
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Lihzahrd Altars", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num1010 = 0; num1010 < 3; num1010++)
				{
					for (int num1011 = 0; num1011 < 2; num1011++)
					{
						int x27 = GenVars.lAltarX + num1010;
						int y22 = GenVars.lAltarY + num1011;
						var tile = Main.tile[x27, y22];
						tile.HasTile = true;
						Main.tile[x27, y22].TileType = 237;
						Main.tile[x27, y22].TileFrameX = (short)(num1010 * 18);
						Main.tile[x27, y22].TileFrameY = (short)(num1011 * 18);
					}
					var tile2 = Main.tile[GenVars.lAltarX + num1010, GenVars.lAltarY + 2];
					tile2.HasTile = true;
					tile2.Slope = 0;
					tile2.IsHalfBlock = false;
					Main.tile[GenVars.lAltarX + num1010, GenVars.lAltarY + 2].TileType = 226;
				}
				for (int num1012 = 0; num1012 < 3; num1012++)
				{
					for (int num1013 = 0; num1013 < 2; num1013++)
					{
						int i5 = GenVars.lAltarX + num1012;
						int j7 = GenVars.lAltarY + num1013;
						WorldGen.SquareTileFrame(i5, j7);
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Micro Biomes", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[76].Value + "..Dead Man's Chests";
				_ = (double)(Main.maxTilesX * Main.maxTilesY) / 5040000.0;
				double num1014 = 10.0;
				if (WorldGen.getGoodWorldGen || WorldGen.noTrapsWorldGen)
				{
					num1014 *= 3.0;
				}
				DeadMansChestBiome deadMansChestBiome = GenVars.configuration.CreateBiome<DeadMansChestBiome>();
				List<int> possibleChestsToTrapify = deadMansChestBiome.GetPossibleChestsToTrapify(GenVars.structures);
				int random5 = passConfig.Get<WorldGenRange>("DeadManChests").GetRandom(WorldGen.genRand);
				int num1015 = 0;
				int num1016 = 3000;
				while (num1015 < random5 && possibleChestsToTrapify.Count > 0)
				{
					num1016--;
					if (num1016 <= 0)
					{
						break;
					}
					int num1017 = possibleChestsToTrapify[WorldGen.genRand.Next(possibleChestsToTrapify.Count)];
					Point origin4 = new Point(Main.chest[num1017].x, Main.chest[num1017].y);
					deadMansChestBiome.Place(origin4, GenVars.structures);
					num1015++;
					possibleChestsToTrapify.Remove(num1017);
				}
				progress.Message = Lang.gen[76].Value + "..Thin Ice";
				progress.Set(1.0 / num1014);
				if (!WorldGen.notTheBees || WorldGen.remixWorldGen)
				{
					ThinIceBiome thinIceBiome = GenVars.configuration.CreateBiome<ThinIceBiome>();
					int random6 = passConfig.Get<WorldGenRange>("ThinIcePatchCount").GetRandom(WorldGen.genRand);
					int num1018 = 0;
					int num1019 = 1000;
					int num1020 = 0;
					while (num1020 < random6)
					{
						if (thinIceBiome.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface + 20, 50, 200, 50), GenVars.structures))
						{
							num1020++;
							num1018 = 0;
						}
						else
						{
							num1018++;
							if (num1018 > num1019)
							{
								num1020++;
								num1018 = 0;
							}
						}
					}
				}
				progress.Message = Lang.gen[76].Value + "..Sword Shrines";
				progress.Set(0.1);
				progress.Set(2.0 / num1014);
				EnchantedSwordBiome enchantedSwordBiome = GenVars.configuration.CreateBiome<EnchantedSwordBiome>();
				int num1021 = passConfig.Get<WorldGenRange>("SwordShrineAttempts").GetRandom(WorldGen.genRand);
				double num1022 = passConfig.Get<double>("SwordShrinePlacementChance");
				if (WorldGen.tenthAnniversaryWorldGen)
				{
					num1021 *= 2;
					num1022 /= 2.0;
				}
				Point origin5 = default(Point);
				for (int num1023 = 0; num1023 < num1021; num1023++)
				{
					if ((num1023 == 0 && WorldGen.tenthAnniversaryWorldGen) || !(WorldGen.genRand.NextDouble() > num1022))
					{
						int num1024 = 0;
						while (num1024++ <= Main.maxTilesX)
						{
							origin5.Y = (int)GenVars.worldSurface + WorldGen.genRand.Next(50, 100);
							if (WorldGen.genRand.Next(2) == 0)
							{
								origin5.X = WorldGen.genRand.Next(50, (int)((double)Main.maxTilesX * 0.3));
							}
							else
							{
								origin5.X = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.7), Main.maxTilesX - 50);
							}
							if (enchantedSwordBiome.Place(origin5, GenVars.structures))
							{
								break;
							}
						}
					}
				}
				progress.Message = Lang.gen[76].Value + "..Campsites";
				progress.Set(0.2);
				progress.Set(3.0 / num1014);
				if (!WorldGen.notTheBees || WorldGen.remixWorldGen)
				{
					CampsiteBiome campsiteBiome = GenVars.configuration.CreateBiome<CampsiteBiome>();
					int random7 = passConfig.Get<WorldGenRange>("CampsiteCount").GetRandom(WorldGen.genRand);
					num1016 = 1000;
					int num1025 = 0;
					while (num1025 < random7)
					{
						num1016--;
						if (num1016 <= 0)
						{
							break;
						}
						if (campsiteBiome.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface, WorldGen.beachDistance, 200, WorldGen.beachDistance), GenVars.structures))
						{
							num1025++;
						}
					}
				}
				progress.Message = Lang.gen[76].Value + "..Explosive Traps";
				progress.Set(4.0 / num1014);
				if (!WorldGen.notTheBees || WorldGen.remixWorldGen)
				{
					MiningExplosivesBiome miningExplosivesBiome = GenVars.configuration.CreateBiome<MiningExplosivesBiome>();
					int num1026 = passConfig.Get<WorldGenRange>("ExplosiveTrapCount").GetRandom(WorldGen.genRand);
					if ((WorldGen.getGoodWorldGen || WorldGen.noTrapsWorldGen) && !WorldGen.notTheBees)
					{
						num1026 = (int)((double)num1026 * 1.5);
					}
					num1016 = 3000;
					int num1027 = 0;
					while (num1027 < num1026)
					{
						num1016--;
						if (num1016 <= 0)
						{
							break;
						}
						if (WorldGen.remixWorldGen)
						{
							if (miningExplosivesBiome.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface, WorldGen.beachDistance, (int)GenVars.rockLayer, WorldGen.beachDistance), GenVars.structures))
							{
								num1027++;
							}
						}
						else if (miningExplosivesBiome.Place(WorldGen.RandomWorldPoint((int)GenVars.rockLayer, WorldGen.beachDistance, 200, WorldGen.beachDistance), GenVars.structures))
						{
							num1027++;
						}
					}
				}
				progress.Message = Lang.gen[76].Value + "..Living Trees";
				progress.Set(0.3);
				progress.Set(5.0 / num1014);
				MahoganyTreeBiome mahoganyTreeBiome = GenVars.configuration.CreateBiome<MahoganyTreeBiome>();
				int random8 = passConfig.Get<WorldGenRange>("LivingTreeCount").GetRandom(WorldGen.genRand);
				int num1028 = 0;
				int num1029 = 0;
				while (num1028 < random8 && num1029 < 20000)
				{
					if (mahoganyTreeBiome.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface + 50, 50, 500, 50), GenVars.structures))
					{
						num1028++;
					}
					num1029++;
				}
				progress.Message = Lang.gen[76].Value + "..Long Minecart Tracks";
				progress.Set(0.4);
				progress.Set(6.0 / num1014);
				progress.Set(7.0 / num1014);
				TrackGenerator trackGenerator = new TrackGenerator();
				int random9 = passConfig.Get<WorldGenRange>("LongTrackCount").GetRandom(WorldGen.genRand);
				WorldGenRange worldGenRange = passConfig.Get<WorldGenRange>("LongTrackLength");
				int maxTilesX = Main.maxTilesX;
				int num1030 = 0;
				int num1031 = 0;
				while (num1031 < random9)
				{
					if (trackGenerator.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface, 10, 200, 10), worldGenRange.ScaledMinimum, worldGenRange.ScaledMaximum))
					{
						num1031++;
						num1030 = 0;
					}
					else
					{
						num1030++;
						if (num1030 > maxTilesX)
						{
							num1031++;
							num1030 = 0;
						}
					}
				}
				progress.Message = Lang.gen[76].Value + "..Standard Minecart Tracks";
				progress.Set(8.0 / num1014);
				random9 = passConfig.Get<WorldGenRange>("StandardTrackCount").GetRandom(WorldGen.genRand);
				worldGenRange = passConfig.Get<WorldGenRange>("StandardTrackLength");
				num1030 = 0;
				int num1032 = 0;
				while (num1032 < random9)
				{
					if (trackGenerator.Place(WorldGen.RandomWorldPoint((int)Main.worldSurface, 10, 200, 10), worldGenRange.ScaledMinimum, worldGenRange.ScaledMaximum))
					{
						num1032++;
						num1030 = 0;
					}
					else
					{
						num1030++;
						if (num1030 > maxTilesX)
						{
							num1032++;
							num1030 = 0;
						}
					}
				}
				progress.Message = Lang.gen[76].Value + "..Lava Traps";
				progress.Set(9.0 / num1014);
				if (!WorldGen.notTheBees)
				{
					double num1033 = (double)Main.maxTilesX * 0.02;
					if (WorldGen.noTrapsWorldGen)
					{
						num1014 *= 5.0;
					}
					else if (WorldGen.getGoodWorldGen)
					{
						num1014 *= 2.0;
					}
					for (int num1034 = 0; (double)num1034 < num1033; num1034++)
					{
						for (int num1035 = 0; num1035 < 10150; num1035++)
						{
							int x28 = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
							int y23 = WorldGen.genRand.Next(GenVars.lavaLine - 100, Main.maxTilesY - 210);
							if (WorldGen.placeLavaTrap(x28, y23))
							{
								break;
							}
						}
					}
				}
				progress.Set(1.0);
			}));
			vanillaGenPasses.Add(new PassLegacy("Water Plants", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Message = Lang.gen[88].Value;
				int num1036 = (int)Main.worldSurface;
				if (WorldGen.remixWorldGen)
				{
					num1036 = Main.maxTilesY - 200;
				}
				for (int num1037 = 20; num1037 < Main.maxTilesX - 20; num1037++)
				{
					progress.Set((double)num1037 / (double)Main.maxTilesX);
					for (int num1038 = 1; num1038 < num1036; num1038++)
					{
						if (WorldGen.genRand.Next(5) == 0 && Main.tile[num1037, num1038].LiquidAmount > 0)
						{
							if (!Main.tile[num1037, num1038].HasTile)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									WorldGen.PlaceLilyPad(num1037, num1038);
								}
								else
								{
									Point point3 = WorldGen.PlaceCatTail(num1037, num1038);
									if (WorldGen.InWorld(point3.X, point3.Y))
									{
										int num1039 = WorldGen.genRand.Next(14);
										for (int num1040 = 0; num1040 < num1039; num1040++)
										{
											WorldGen.GrowCatTail(point3.X, point3.Y);
										}
										WorldGen.SquareTileFrame(point3.X, point3.Y);
									}
								}
							}
							if ((!Main.tile[num1037, num1038].HasTile || Main.tile[num1037, num1038].TileType == 61 || Main.tile[num1037, num1038].TileType == 74) && PlaceBamboo(num1037, num1038))
							{
								int num1041 = WorldGen.genRand.Next(10, 20);
								for (int num1042 = 0; num1042 < num1041 && PlaceBamboo(num1037, num1038 - num1042); num1042++)
								{
								}
							}
						}
					}
					int num1043 = Main.UnderworldLayer;
					while ((double)num1043 > Main.worldSurface)
					{
						if (Main.tile[num1037, num1043].TileType == 53 && WorldGen.genRand.Next(3) != 0)
						{
							WorldGen.GrowCheckSeaweed(num1037, num1043);
						}
						else if (Main.tile[num1037, num1043].TileType == 549)
						{
							WorldGen.GrowCheckSeaweed(num1037, num1043);
						}
						num1043--;
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Stalac", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				progress.Set(1.0);
				for (int num1044 = 20; num1044 < Main.maxTilesX - 20; num1044++)
				{
					for (int num1045 = (int)Main.worldSurface; num1045 < Main.maxTilesY - 20; num1045++)
					{
						if ((Main.tenthAnniversaryWorld || WorldGen.drunkWorldGen || WorldGen.genRand.Next(5) == 0) && Main.tile[num1044, num1045 - 1].LiquidAmount == 0)
						{
							int num1046 = WorldGen.genRand.Next(7);
							int treeTileType = 0;
							switch (num1046)
							{
							case 0:
								treeTileType = 583;
								break;
							case 1:
								treeTileType = 584;
								break;
							case 2:
								treeTileType = 585;
								break;
							case 3:
								treeTileType = 586;
								break;
							case 4:
								treeTileType = 587;
								break;
							case 5:
								treeTileType = 588;
								break;
							case 6:
								treeTileType = 589;
								break;
							}
							WorldGen.TryGrowingTreeByType(treeTileType, num1044, num1045);
						}
						if (!WorldGen.oceanDepths(num1044, num1045) && !Main.tile[num1044, num1045].HasTile && WorldGen.genRand.Next(5) == 0)
						{
							if ((Main.tile[num1044, num1045 - 1].TileType == 1 || Main.tile[num1044, num1045 - 1].TileType == 147 || Main.tile[num1044, num1045 - 1].TileType == 161 || Main.tile[num1044, num1045 - 1].TileType == 25 || Main.tile[num1044, num1045 - 1].TileType == 203 || Main.tileStone[Main.tile[num1044, num1045 - 1].TileType] || Main.tileMoss[Main.tile[num1044, num1045 - 1].TileType]) && !Main.tile[num1044, num1045].HasTile && !Main.tile[num1044, num1045 + 1].HasTile)
							{
								var tile = Main.tile[num1044, num1045 - 1];
								tile.Slope = 0;
							}
							if ((Main.tile[num1044, num1045 + 1].TileType == 1 || Main.tile[num1044, num1045 + 1].TileType == 147 || Main.tile[num1044, num1045 + 1].TileType == 161 || Main.tile[num1044, num1045 + 1].TileType == 25 || Main.tile[num1044, num1045 + 1].TileType == 203 || Main.tileStone[Main.tile[num1044, num1045 + 1].TileType] || Main.tileMoss[Main.tile[num1044, num1045 + 1].TileType]) && !Main.tile[num1044, num1045].HasTile && !Main.tile[num1044, num1045 - 1].HasTile)
							{
								var tile = Main.tile[num1044, num1045 + 1];
								tile.Slope = 0;
							}
							WorldGen.PlaceTight(num1044, num1045);
						}
					}
					for (int num1047 = 5; num1047 < (int)Main.worldSurface; num1047++)
					{
						if ((Main.tile[num1044, num1047 - 1].TileType == 147 || Main.tile[num1044, num1047 - 1].TileType == 161) && WorldGen.genRand.Next(5) == 0)
						{
							if (!Main.tile[num1044, num1047].HasTile && !Main.tile[num1044, num1047 + 1].HasTile)
							{
								var tile = Main.tile[num1044, num1047 - 1];
								tile.Slope = 0;
							}
							WorldGen.PlaceTight(num1044, num1047);
						}
						if ((Main.tile[num1044, num1047 - 1].TileType == 25 || Main.tile[num1044, num1047 - 1].TileType == 203) && WorldGen.genRand.Next(5) == 0)
						{
							if (!Main.tile[num1044, num1047].HasTile && !Main.tile[num1044, num1047 + 1].HasTile)
							{
								var tile = Main.tile[num1044, num1047 - 1];
								tile.Slope = 0;
							}
							WorldGen.PlaceTight(num1044, num1047);
						}
						if ((Main.tile[num1044, num1047 + 1].TileType == 25 || Main.tile[num1044, num1047 + 1].TileType == 203) && WorldGen.genRand.Next(5) == 0)
						{
							if (!Main.tile[num1044, num1047].HasTile && !Main.tile[num1044, num1047 - 1].HasTile)
							{
								var tile = Main.tile[num1044, num1047 + 1];
								tile.Slope = 0;
							}
							WorldGen.PlaceTight(num1044, num1047);
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Remove Broken Traps", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				if (!WorldGen.noTrapsWorldGen || WorldGen.tenthAnniversaryWorldGen || WorldGen.notTheBees)
				{
					progress.Message = Lang.gen[82].Value;
					List<Point> list4 = new List<Point>();
					int num1048 = 50;
					for (int num1049 = num1048; num1049 < Main.maxTilesX - num1048; num1049++)
					{
						double value20 = (double)(num1049 - num1048) / (double)(Main.maxTilesX - num1048 * 2);
						progress.Set(value20);
						for (int num1050 = 50; num1050 < Main.maxTilesY - 50; num1050++)
						{
							if (Main.tile[num1049, num1050].RedWire && !list4.Contains(new Point(num1049, num1050)))
							{
								WorldGen.ClearBrokenTraps(new Point(num1049, num1050), list4);
							}
						}
					}
				}
			}));
			vanillaGenPasses.Add(new PassLegacy("Final Cleanup", delegate(GenerationProgress progress, GameConfiguration passConfig)
			{
				Main.tileSolid[484] = false;
				WorldGen.FillWallHolesInArea(new Rectangle(0, 0, Main.maxTilesX, (int)Main.worldSurface));
				progress.Message = Lang.gen[86].Value;
				for (int num1051 = 0; num1051 < Main.maxTilesX; num1051++)
				{
					progress.Set((double)num1051 / (double)Main.maxTilesX);
					int num1052 = 0;
					while (num1052 < Main.maxTilesY)
					{
						Tile tile6 = Main.tile[num1051, num1052];
						if (tile6.HasTile && !WorldGen.SolidTile(num1051, num1052 + 1))
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.TileType != 53)
							{
								tile6 = Main.tile[num1051, num1052];
								if (tile6.TileType != 112)
								{
									tile6 = Main.tile[num1051, num1052];
									if (tile6.TileType != 234)
									{
										tile6 = Main.tile[num1051, num1052];
										if (tile6.TileType != 224)
										{
											tile6 = Main.tile[num1051, num1052];
											if (tile6.TileType != 123)
											{
												goto IL_06be;
											}
										}
									}
								}
							}
							if ((double)num1052 < Main.worldSurface + 10.0)
							{
								tile6 = Main.tile[num1051, num1052 + 1];
								if (!tile6.HasTile)
								{
									tile6 = Main.tile[num1051, num1052 + 1];
									if (tile6.WallType != 191 && !WorldGen.oceanDepths(num1051, num1052))
									{
										int num1053 = 10;
										int num1054 = num1052 + 1;
										for (int num1055 = num1054; num1055 < num1054 + 10; num1055++)
										{
											tile6 = Main.tile[num1051, num1055];
											if (tile6.HasTile)
											{
												tile6 = Main.tile[num1051, num1055];
												if (tile6.TileType == 314)
												{
													num1053 = 0;
													break;
												}
											}
										}
										while (true)
										{
											tile6 = Main.tile[num1051, num1054];
											if (tile6.HasTile || num1053 <= 0 || num1054 >= Main.maxTilesY - 50)
											{
												break;
											}
											tile6 = Main.tile[num1051, num1054 - 1];
											tile6.Slope = 0;
											tile6 = Main.tile[num1051, num1054 - 1];
											tile6.IsHalfBlock = false;
											tile6 = Main.tile[num1051, num1054];
											tile6.HasTile = true;
											tile6 = Main.tile[num1051, num1054];
											ref ushort type12 = ref tile6.TileType;
											tile6 = Main.tile[num1051, num1052];
											type12 = tile6.TileType;
											tile6 = Main.tile[num1051, num1054];
											tile6.Slope = 0;
											tile6 = Main.tile[num1051, num1054];
											tile6.IsHalfBlock = false;
											num1054++;
											num1053--;
										}
										if (num1053 == 0)
										{
											tile6 = Main.tile[num1051, num1054];
											if (!tile6.HasTile)
											{
												tile6 = Main.tile[num1051, num1052];
												switch (tile6.TileType)
												{
												case 53:
													tile6 = Main.tile[num1051, num1054];
													tile6.TileType = 397;
													tile6 = Main.tile[num1051, num1054];
													tile6.HasTile = true;
													break;
												case 112:
													tile6 = Main.tile[num1051, num1054];
													tile6.TileType = 398;
													tile6 = Main.tile[num1051, num1054];
													tile6.HasTile = true;
													break;
												case 234:
													tile6 = Main.tile[num1051, num1054];
													tile6.TileType = 399;
													tile6 = Main.tile[num1051, num1054];
													tile6.HasTile = true;
													break;
												case 224:
													tile6 = Main.tile[num1051, num1054];
													tile6.TileType = 147;
													tile6 = Main.tile[num1051, num1054];
													tile6.HasTile = true;
													break;
												case 123:
													tile6 = Main.tile[num1051, num1054];
													tile6.TileType = 1;
													tile6 = Main.tile[num1051, num1054];
													tile6.HasTile = true;
													break;
												}
												goto IL_0690;
											}
										}
										tile6 = Main.tile[num1051, num1054];
										if (tile6.HasTile)
										{
											bool[] tileSolid = Main.tileSolid;
											tile6 = Main.tile[num1051, num1054];
											if (tileSolid[tile6.TileType])
											{
												bool[] tileSolidTop = Main.tileSolidTop;
												tile6 = Main.tile[num1051, num1054];
												if (!tileSolidTop[tile6.TileType])
												{
													tile6 = Main.tile[num1051, num1054];
													tile6.Slope = 0;
													tile6 = Main.tile[num1051, num1054];
													tile6.IsHalfBlock = false;
												}
											}
										}
										goto IL_0690;
									}
								}
							}
							bool[] tileSolid2 = Main.tileSolid;
							tile6 = Main.tile[num1051, num1052 + 1];
							if (tileSolid2[tile6.TileType])
							{
								bool[] tileSolidTop2 = Main.tileSolidTop;
								tile6 = Main.tile[num1051, num1052 + 1];
								if (!tileSolidTop2[tile6.TileType])
								{
									tile6 = Main.tile[num1051, num1052 + 1];
									if (!tile6.TopSlope)
									{
										tile6 = Main.tile[num1051, num1052 + 1];
										if (!tile6.IsHalfBlock)
										{
											goto IL_05aa;
										}
									}
									tile6 = Main.tile[num1051, num1052 + 1];
									tile6.Slope = 0;
									tile6 = Main.tile[num1051, num1052 + 1];
									tile6.IsHalfBlock = false;
									goto IL_0690;
								}
							}
							goto IL_05aa;
						}
						goto IL_06be;
						IL_05aa:
						tile6 = Main.tile[num1051, num1052];
						switch (tile6.TileType)
						{
						case 53:
							tile6 = Main.tile[num1051, num1052];
							tile6.TileType = 397;
							break;
						case 112:
							tile6 = Main.tile[num1051, num1052];
							tile6.TileType = 398;
							break;
						case 234:
							tile6 = Main.tile[num1051, num1052];
							tile6.TileType = 399;
							break;
						case 224:
							tile6 = Main.tile[num1051, num1052];
							tile6.TileType = 147;
							break;
						case 123:
							tile6 = Main.tile[num1051, num1052];
							tile6.TileType = 1;
							break;
						}
						goto IL_0690;
						IL_0753:
						tile6 = Main.tile[num1051, num1052];
						if (tile6.TileType != 485)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.TileType != 187)
							{
								tile6 = Main.tile[num1051, num1052];
								if (tile6.TileType != 165)
								{
									goto IL_07bb;
								}
							}
						}
						WorldGen.TileFrame(num1051, num1052);
						goto IL_07bb;
						IL_0825:
						tile6 = Main.tile[num1051, num1052];
						if (tile6.TileType == 26)
						{
							WorldGen.TileFrame(num1051, num1052);
						}
						bool[] isATreeTrunk = TileID.Sets.IsATreeTrunk;
						tile6 = Main.tile[num1051, num1052];
						if (!isATreeTrunk[tile6.TileType])
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.TileType != 323)
							{
								goto IL_0896;
							}
						}
						WorldGen.TileFrame(num1051, num1052);
						goto IL_0896;
						IL_0690:
						tile6 = Main.tile[num1051, num1052 - 1];
						if (tile6.TileType == 323)
						{
							WorldGen.TileFrame(num1051, num1052 - 1);
						}
						goto IL_06be;
						IL_07bb:
						tile6 = Main.tile[num1051, num1052];
						if (tile6.TileType == 28)
						{
							WorldGen.TileFrame(num1051, num1052);
						}
						tile6 = Main.tile[num1051, num1052];
						if (tile6.TileType != 10)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.TileType != 11)
							{
								goto IL_0825;
							}
						}
						WorldGen.TileFrame(num1051, num1052);
						goto IL_0825;
						IL_0896:
						tile6 = Main.tile[num1051, num1052];
						if (tile6.TileType == 137)
						{
							tile6 = Main.tile[num1051, num1052];
							tile6.Slope = 0;
							tile6 = Main.tile[num1051, num1052];
							tile6.IsHalfBlock = false;
						}
						tile6 = Main.tile[num1051, num1052];
						if (tile6.HasTile)
						{
							bool[] boulders = TileID.Sets.Boulders;
							tile6 = Main.tile[num1051, num1052];
							if (boulders[tile6.TileType])
							{
								tile6 = Main.tile[num1051, num1052];
								int num1056 = tile6.TileFrameX / 18;
								int num1057 = num1051;
								num1057 -= num1056;
								tile6 = Main.tile[num1051, num1052];
								int num1058 = tile6.TileFrameY / 18;
								int num1059 = num1052;
								num1059 -= num1058;
								bool flag64 = false;
								for (int num1060 = 0; num1060 < 2; num1060++)
								{
									Tile tile7 = Main.tile[num1057 + num1060, num1059 - 1];
									if (tile7 != null && tile7.HasTile && tile7.TileType == 26)
									{
										flag64 = true;
										break;
									}
									for (int num1061 = 0; num1061 < 2; num1061++)
									{
										int x29 = num1057 + num1060;
										int y24 = num1059 + num1061;
										tile6 = Main.tile[x29, y24];
										tile6.HasTile = true;
										tile6 = Main.tile[x29, y24];
										tile6.Slope = 0;
										tile6 = Main.tile[x29, y24];
										tile6.IsHalfBlock = false;
										tile6 = Main.tile[x29, y24];
										ref ushort type13 = ref tile6.TileType;
										tile6 = Main.tile[num1051, num1052];
										type13 = tile6.TileType;
										tile6 = Main.tile[x29, y24];
										tile6.TileFrameX = (short)(num1060 * 18);
										tile6 = Main.tile[x29, y24];
										tile6.TileFrameY = (short)(num1061 * 18);
									}
								}
								if (flag64)
								{
									ushort num1062 = 0;
									tile6 = Main.tile[num1051, num1052];
									if (tile6.TileType == 484)
									{
										num1062 = 397;
									}
									for (int num1063 = 0; num1063 < 2; num1063++)
									{
										for (int num1064 = 0; num1064 < 2; num1064++)
										{
											int x30 = num1057 + num1063;
											int y25 = num1059 + num1064;
											tile6 = Main.tile[x30, y25];
											tile6.HasTile = true;
											tile6 = Main.tile[x30, y25];
											tile6.Slope = 0;
											tile6 = Main.tile[x30, y25];
											tile6.IsHalfBlock = false;
											tile6 = Main.tile[x30, y25];
											tile6.TileType = num1062;
											tile6 = Main.tile[x30, y25];
											tile6.TileFrameX = 0;
											tile6 = Main.tile[x30, y25];
											tile6.TileFrameY = 0;
										}
									}
								}
							}
						}
						tile6 = Main.tile[num1051, num1052];
						if (tile6.TileType == 323)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.LiquidAmount > 0)
							{
								WorldGen.KillTile(num1051, num1052);
							}
						}
						bool[] wallDungeon = Main.wallDungeon;
						tile6 = Main.tile[num1051, num1052];
						if (wallDungeon[tile6.WallType])
						{
							tile6 = Main.tile[num1051, num1052];
							SetLiquidToLava(tile6, false);
							tile6 = Main.tile[num1051, num1052];
							if (tile6.HasTile)
							{
								tile6 = Main.tile[num1051, num1052];
								if (tile6.TileType == 56)
								{
									WorldGen.KillTile(num1051, num1052);
									tile6 = Main.tile[num1051, num1052];
									SetLiquidToLava(tile6, false);
									tile6 = Main.tile[num1051, num1052];
									tile6.LiquidAmount = byte.MaxValue;
								}
							}
						}
						tile6 = Main.tile[num1051, num1052];
						if (tile6.HasTile)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.TileType == 314)
							{
								int num1065 = 15;
								int num1066 = 1;
								int num1067 = num1052;
								while (num1052 - num1067 < num1065)
								{
									tile6 = Main.tile[num1051, num1067];
									tile6.LiquidAmount = 0;
									num1067--;
								}
								for (num1067 = num1052; num1067 - num1052 < num1066; num1067++)
								{
									tile6 = Main.tile[num1051, num1067];
									tile6.LiquidAmount = 0;
								}
							}
						}
						tile6 = Main.tile[num1051, num1052];
						if (tile6.HasTile)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.TileType == 332)
							{
								tile6 = Main.tile[num1051, num1052 + 1];
								if (!tile6.HasTile)
								{
									tile6 = Main.tile[num1051, num1052 + 1];
									tile6.ClearEverything();
									tile6 = Main.tile[num1051, num1052 + 1];
									tile6.HasTile = true;
									tile6 = Main.tile[num1051, num1052 + 1];
									tile6.TileType = 332;
								}
							}
						}
						if (num1051 > WorldGen.beachDistance && num1051 < Main.maxTilesX - WorldGen.beachDistance && (double)num1052 < Main.worldSurface)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.LiquidAmount > 0)
							{
								tile6 = Main.tile[num1051, num1052];
								if (tile6.LiquidAmount < byte.MaxValue)
								{
									tile6 = Main.tile[num1051 - 1, num1052];
									if (tile6.LiquidAmount < byte.MaxValue)
									{
										tile6 = Main.tile[num1051 + 1, num1052];
										if (tile6.LiquidAmount < byte.MaxValue)
										{
											tile6 = Main.tile[num1051, num1052 + 1];
											if (tile6.LiquidAmount < byte.MaxValue)
											{
												bool[] clouds = TileID.Sets.Clouds;
												tile6 = Main.tile[num1051 - 1, num1052];
												if (!clouds[tile6.TileType])
												{
													bool[] clouds2 = TileID.Sets.Clouds;
													tile6 = Main.tile[num1051 + 1, num1052];
													if (!clouds2[tile6.TileType])
													{
														bool[] clouds3 = TileID.Sets.Clouds;
														tile6 = Main.tile[num1051, num1052 + 1];
														if (!clouds3[tile6.TileType])
														{
															tile6 = Main.tile[num1051, num1052];
															tile6.LiquidAmount = 0;
														}
													}
												}
											}
										}
									}
								}
							}
						}
						num1052++;
						continue;
						IL_06be:
						tile6 = Main.tile[num1051, num1052];
						if (tile6.WallType != 187)
						{
							tile6 = Main.tile[num1051, num1052];
							if (tile6.WallType != 216)
							{
								goto IL_0753;
							}
						}
						tile6 = Main.tile[num1051, num1052];
						if (tile6.LiquidAmount > 0 && !WorldGen.remixWorldGen)
						{
							tile6 = Main.tile[num1051, num1052];
							tile6.LiquidAmount = byte.MaxValue;
							tile6 = Main.tile[num1051, num1052];
							SetLiquidToLava(tile6, true);
						}
						goto IL_0753;
					}
				}
				int num1068 = 0;
				int num1069 = 3;
				num1069 = WorldGen.GetWorldSize() switch
				{
					1 => 6, 
					2 => 9, 
					_ => 3, 
				};
				if (WorldGen.tenthAnniversaryWorldGen)
				{
					num1069 *= 5;
				}
				int num1070 = 50;
				int minValue2 = num1070;
				int minValue3 = num1070;
				int maxValue11 = Main.maxTilesX - num1070;
				int maxValue12 = Main.maxTilesY - 200;
				int num1071 = 3000;
				while (num1068 < num1069)
				{
					num1071--;
					if (num1071 <= 0)
					{
						break;
					}
					int x31 = WorldGen.genRand.Next(minValue2, maxValue11);
					int y26 = WorldGen.genRand.Next(minValue3, maxValue12);
					Tile tile8 = Main.tile[x31, y26];
					if (tile8.HasTile && tile8.TileType >= 0)
					{
						bool flag65 = TileID.Sets.Dirt[tile8.TileType];
						if (WorldGen.notTheBees)
						{
							flag65 = flag65 || TileID.Sets.Mud[tile8.TileType];
						}
						if (flag65)
						{
							num1068++;
							tile8.ClearTile();
							tile8.HasTile = true;
							tile8.TileType = 668;
						}
					}
				}
				if (WorldGen.noTrapsWorldGen)
				{
					FinishNoTraps();
				}
				if (Main.tenthAnniversaryWorld)
				{
					FinishTenthAnniversaryWorld();
				}
				if (WorldGen.drunkWorldGen)
				{
					FinishDrunkGen();
				}
				if (WorldGen.notTheBees)
				{
					NotTheBees();
					FinishNotTheBees();
				}
				if (WorldGen.getGoodWorldGen)
				{
					FinishGetGoodWorld();
				}
				if (WorldGen.remixWorldGen)
				{
					FinishRemixWorld();
				}
				if (GenVars.shimmerPosition.X != 0 && GenVars.shimmerPosition.Y != 0)
				{
					ShimmerCleanUp();
				}
				WorldGen.notTheBees = false;
				WorldGen.getGoodWorldGen = false;
				WorldGen.noTileActions = false;
				Main.tileSolid[659] = true;
				Main.tileSolid[GenVars.crackedType] = true;
				Main.tileSolid[484] = true;
				WorldGen.gen = false;
				Main.AnglerQuestSwap();
				skipFramingDuringGen = false;
				progress.Message = Lang.gen[87].Value;
			}));
		}

		private static void ResetGenerator()
		{
			GenVars.numOrePatch = 0;
			GenVars.numTunnels = 0;
			GenVars.numLakes = 0;
			GenVars.numMushroomBiomes = 0;
			GenVars.numOceanCaveTreasure = 0;
			GenVars.numOasis = 0;
			GenVars.mudWall = false;
			GenVars.hellChest = 0;
			GenVars.JungleX = 0;
			GenVars.numMCaves = 0;
			GenVars.numIslandHouses = 0;
			GenVars.skyIslandHouseCount = 0;
			GenVars.dEnteranceX = 0;
			GenVars.numDRooms = 0;
			GenVars.numDDoors = 0;
			GenVars.generatedShadowKey = false;
			GenVars.numDungeonPlatforms = 0;
			GenVars.numJChests = 0;
			GenVars.JungleItemCount = 0;
			GenVars.lastDungeonHall = Vector2D.Zero;
		}
		private static bool IsLava(Tile tile)
		{
			return tile.LiquidType == 1;
		}
		private static void SetLiquidToLava(Tile tile, bool lava)
		{
			SetIsLiquidType(tile, 1, lava);
		}
		private static bool IsHoney(Tile tile)
		{
			return tile.LiquidType == 2;
		}
		private static void SetLiquidToHoney(Tile tile, bool honey)
		{
			SetIsLiquidType(tile, 2, honey);
		}
		private static bool IsShimmer(Tile tile)
		{
			return tile.LiquidType == 3;
		}
		private static void SetLiquidToShimmer(Tile tile, bool shimmer)
		{
			SetIsLiquidType(tile, 3, shimmer);
		}
		private static void SetIsLiquidType(Tile tile, int liquidId, bool value)
		{
			if (value)
			{
				tile.LiquidType = liquidId;
			}
			else if (tile.LiquidType == liquidId)
			{
				tile.LiquidType = 0;
			}
		}
		private static void NotTheBees()
		{
			int num = Main.maxTilesX / 7;
			if (!notTheBees)
				return;

			for (int i = 0; i < Main.maxTilesX; i++) {
				for (int j = 0; j < Main.maxTilesY - 180; j++) {
					if (remixWorldGen && (i < num + genRand.Next(3) || i >= Main.maxTilesX - num - genRand.Next(3) || ((double)j > (Main.worldSurface * 2.0 + Main.rockLayer) / 3.0 + (double)genRand.Next(3) && j < Main.maxTilesY - 350 - genRand.Next(3))))
						continue;

					if (Main.tile[i, j].TileType == 52)
						Main.tile[i, j].TileType = 62;

					if ((SolidOrSlopedTile(i, j) || TileID.Sets.CrackedBricks[Main.tile[i, j].TileType]) && !TileID.Sets.Ore[Main.tile[i, j].TileType] && Main.tile[i, j].TileType != 123 && Main.tile[i, j].TileType != 40) {
						if (Main.tile[i, j].TileType == 191 || Main.tile[i, j].TileType == 383) {
							if (!remixWorldGen)
								Main.tile[i, j].TileType = 383;
						}
						else if (Main.tile[i, j].TileType == 192 || Main.tile[i, j].TileType == 384) {
							if (!remixWorldGen)
								Main.tile[i, j].TileType = 384;
						}
						else if (Main.tile[i, j].TileType != 151 && Main.tile[i, j].TileType != 662 && Main.tile[i, j].TileType != 661 && Main.tile[i, j].TileType != 189 && Main.tile[i, j].TileType != 196 && Main.tile[i, j].TileType != 120 && Main.tile[i, j].TileType != 158 && Main.tile[i, j].TileType != 175 && Main.tile[i, j].TileType != 45 && Main.tile[i, j].TileType != 119) {
							if (Main.tile[i, j].TileType >= 63 && Main.tile[i, j].TileType <= 68) {
								Main.tile[i, j].TileType = 230;
							}
							else if (Main.tile[i, j].TileType != 57 && Main.tile[i, j].TileType != 76 && Main.tile[i, j].TileType != 75 && Main.tile[i, j].TileType != 229 && Main.tile[i, j].TileType != 230 && Main.tile[i, j].TileType != 407 && Main.tile[i, j].TileType != 404) {
								if (Main.tile[i, j].TileType == 224) {
									Main.tile[i, j].TileType = 229;
								}
								else if (Main.tile[i, j].TileType == 53) {
									if (i < beachDistance + genRand.Next(3) || i > Main.maxTilesX - beachDistance - genRand.Next(3))
										Main.tile[i, j].TileType = 229;
								}
								else if ((i <= beachDistance - genRand.Next(3) || i >= Main.maxTilesX - beachDistance + genRand.Next(3) || (Main.tile[i, j].TileType != 397 && Main.tile[i, j].TileType != 396)) && Main.tile[i, j].TileType != 10 && Main.tile[i, j].TileType != 203 && Main.tile[i, j].TileType != 25 && Main.tile[i, j].TileType != 137 && Main.tile[i, j].TileType != 138 && Main.tile[i, j].TileType != 141) {
									if (Main.tileDungeon[Main.tile[i, j].TileType] || TileID.Sets.CrackedBricks[Main.tile[i, j].TileType]) {
										var tile = Main.tile[i, j];
										tile.TileColor = 14;
									}
									else if (Main.tile[i, j].TileType == 226) {
										var tile = Main.tile[i, j];
										tile.TileColor = 15;
									}
									else if (Main.tile[i, j].TileType != 202 && Main.tile[i, j].TileType != 70 && Main.tile[i, j].TileType != 48 && Main.tile[i, j].TileType != 232) {
										if (TileID.Sets.Conversion.Grass[Main.tile[i, j].TileType] || Main.tile[i, j].TileType == 60 || Main.tile[i, j].TileType == 70) {
											if (j > GenVars.lavaLine + genRand.Next(-2, 3) + 2)
												Main.tile[i, j].TileType = 70;
											else
												Main.tile[i, j].TileType = 60;
										}
										else if (Main.tile[i, j].TileType == 0 || Main.tile[i, j].TileType == 59) {
											Main.tile[i, j].TileType = 59;
										}
										else if (Main.tile[i, j].TileType != 633) {
											if (j > GenVars.lavaLine + genRand.Next(-2, 3) + 2)
												Main.tile[i, j].TileType = 230;
											else if (!remixWorldGen || (double)j > Main.worldSurface + (double)genRand.Next(-1, 2))
												Main.tile[i, j].TileType = 225;
										}
									}
								}
							}
						}
					}

					if (Main.tile[i, j].WallType!= 15 && Main.tile[i, j].WallType!= 64 && Main.tile[i, j].WallType!= 204 && Main.tile[i, j].WallType!= 205 && Main.tile[i, j].WallType!= 206 && Main.tile[i, j].WallType!= 207 && Main.tile[i, j].WallType!= 23 && Main.tile[i, j].WallType!= 24 && Main.tile[i, j].WallType!= 42 && Main.tile[i, j].WallType!= 10 && Main.tile[i, j].WallType!= 21 && Main.tile[i, j].WallType!= 82 && Main.tile[i, j].WallType!= 187 && Main.tile[i, j].WallType!= 216 && Main.tile[i, j].WallType!= 34 && Main.tile[i, j].WallType!= 244) {
						if (Main.tile[i, j].WallType== 87)
							{
								var tile = Main.tile[i, j];
								tile.WallColor = 15;
							}
						else if (Main.wallDungeon[Main.tile[i, j].WallType])
							{
								var tile = Main.tile[i, j];
								tile.WallColor = 14;
							}
						else if (Main.tile[i, j].WallType== 2)
							Main.tile[i, j].WallType= 2;
						else if (Main.tile[i, j].WallType== 196)
							Main.tile[i, j].WallType= 196;
						else if (Main.tile[i, j].WallType== 197)
							Main.tile[i, j].WallType= 197;
						else if (Main.tile[i, j].WallType== 198)
							Main.tile[i, j].WallType= 198;
						else if (Main.tile[i, j].WallType== 199)
							Main.tile[i, j].WallType= 199;
						else if (Main.tile[i, j].WallType== 63)
							Main.tile[i, j].WallType= 64;
						else if (Main.tile[i, j].WallType!= 3 && Main.tile[i, j].WallType!= 83 && Main.tile[i, j].WallType!= 73 && Main.tile[i, j].WallType!= 62 && Main.tile[i, j].WallType!= 13 && Main.tile[i, j].WallType!= 14 && Main.tile[i, j].WallType> 0 && (!remixWorldGen || (double)j > Main.worldSurface + (double)genRand.Next(-1, 2)))
							Main.tile[i, j].WallType= 86;
					}

					if (Main.tile[i, j].LiquidAmount > 0 && j <= GenVars.lavaLine + 2) {
						if ((double)j > Main.rockLayer && (i < beachDistance + 200 || i > Main.maxTilesX - beachDistance - 200))
							SetLiquidToHoney(Main.tile[i, j], false);
						else if (Main.wallDungeon[Main.tile[i, j].WallType])
							SetLiquidToHoney(Main.tile[i, j], false);
						else
							SetLiquidToHoney(Main.tile[i, j], true);
					}
				}
			}
		}

		
		private static int tileCounterMax = 20;
		private static void ScanTileColumnAndRemoveClumps(int x)
		{
			int num = 0;
			int y = 0;
			for (int i = 10; i < Main.maxTilesY - 10; i++)
			{
				if (Main.tile[x, i].HasTile && Main.tileSolid[Main.tile[x, i].TileType] && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[x, i].TileType])
				{
					if (num == 0)
					{
						y = i;
					}
					num++;
					continue;
				}
				if (num > 0 && num < tileCounterMax)
				{
					WorldGen.SmallConsecutivesFound++;
					if (WorldGen.tileCounter(x, y) < tileCounterMax)
					{
						WorldGen.SmallConsecutivesEliminated++;
						WorldGen.tileCounterKill();
					}
				}
				num = 0;
			}
		}

		private static double TuneOceanDepth(int count, double depth, bool floridaStyle = false)
		{
			if (!floridaStyle)
			{
				if (count < 3)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.2;
				}
				else if (count < 6)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.15;
				}
				else if (count < 9)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.1;
				}
				else if (count < 15)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.07;
				}
				else if (count < 50)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.05;
				}
				else if (count < 75)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.04;
				}
				else if (count < 100)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.03;
				}
				else if (count < 125)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.02;
				}
				else if (count < 150)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.01;
				}
				else if (count < 175)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.005;
				}
				else if (count < 200)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.001;
				}
				else if (count < 230)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.01;
				}
				else if (count < 235)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.05;
				}
				else if (count < 240)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.1;
				}
				else if (count < 245)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.05;
				}
				else if (count < 255)
				{
					depth += (double)WorldGen.genRand.Next(10, 20) * 0.01;
				}
			}
			else if (count < 3)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.001;
			}
			else if (count < 6)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.002;
			}
			else if (count < 9)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.004;
			}
			else if (count < 15)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.007;
			}
			else if (count < 50)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.01;
			}
			else if (count < 75)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.014;
			}
			else if (count < 100)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.019;
			}
			else if (count < 125)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.027;
			}
			else if (count < 150)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.038;
			}
			else if (count < 175)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.052;
			}
			else if (count < 200)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.08;
			}
			else if (count < 230)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.12;
			}
			else if (count < 235)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.16;
			}
			else if (count < 240)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.27;
			}
			else if (count < 245)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.43;
			}
			else if (count < 255)
			{
				depth += (double)WorldGen.genRand.Next(10, 20) * 0.6;
			}
			return depth;
		}

		private static bool placeTNTBarrel(int x, int y)
		{
			int num = y;
			while (!Main.tile[x, num].HasTile)
			{
				num++;
				if (num > Main.maxTilesY - 350)
				{
					return false;
				}
			}
			num--;
			if (IsShimmer(Main.tile[x, num]))
			{
				return false;
			}
			if (WorldGen.PlaceTile(x, num, 654))
			{
				return true;
			}
			return false;
		}

		private static Point GetAdjustedFloorPosition(int x, int y)
		{
			int num = x - 1;
			int num2 = y - 2;
			bool isEmpty = false;
			bool hasFloor = false;
			while (!isEmpty && num2 > Main.spawnTileY - 10)
			{
				Scan3By3(num, num2, out isEmpty, out hasFloor);
				if (!isEmpty)
				{
					num2--;
				}
			}
			while (!hasFloor && num2 < Main.spawnTileY + 10)
			{
				Scan3By3(num, num2, out isEmpty, out hasFloor);
				if (!hasFloor)
				{
					num2++;
				}
			}
			return new Point(num + 1, num2 + 2);
		}

		private static void Scan3By3(int topLeftX, int topLeftY, out bool isEmpty, out bool hasFloor)
		{
			isEmpty = true;
			hasFloor = false;
			for (int i = 0; i < 3; i++)
			{
				int num = 0;
				while (num < 3)
				{
					int i2 = topLeftX + i;
					int j = topLeftY + num;
					if (!WorldGen.SolidTile(i2, j))
					{
						num++;
						continue;
					}
					goto IL_0028;
				}
				continue;
				IL_0028:
				isEmpty = false;
				break;
			}
			for (int k = 0; k < 3; k++)
			{
				int i3 = topLeftX + k;
				int j2 = topLeftY + 3;
				if (WorldGen.SolidTile(i3, j2))
				{
					hasFloor = true;
					break;
				}
			}
		}

		private static void GrowGlowTulips()
		{
			int num = ((Main.maxTilesX > 4200) ? ((Main.maxTilesX <= 6400) ? 1 : 2) : 0);
			int num2 = 100;
			int num3 = 300;
			int num4 = 2;
			num4 = num switch
			{
				1 => 4, 
				2 => 6, 
				_ => 2, 
			};
			int num5 = 0;
			int num6 = 10000;
			int num7 = (int)((double)num6 * 0.75);
			while (num5 < num4)
			{
				num6--;
				if (num6 <= 0)
				{
					break;
				}
				int i = ((num5 >= num4 / 2 && (num6 <= num7 || WorldGen.genRand.Next(2) != 0)) ? WorldGen.genRand.Next(Main.maxTilesX - num3, Main.maxTilesX - num2) : WorldGen.genRand.Next(num2, num3));
				int j = ((!WorldGen.remixWorldGen) ? WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200) : WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 350));
				if (TryGrowingGlowTulip(i, j))
				{
					num5++;
				}
			}
		}

		private static void MatureTheHerbPlants()
		{
			for (int i = 10; i < Main.maxTilesX - 10; i++)
			{
				for (int j = 10; j < Main.maxTilesY - 10; j++)
				{
					if ((double)j > Main.rockLayer && (Main.tile[i, j + 1].TileType == 59 || Main.tile[i, j + 1].TileType == 0) && WorldGen.SolidTile(i, j + 1) && !Main.tile[i, j].HasTile && Main.tile[i, j].LiquidAmount == 0 && WorldGen.genRand.Next(25) == 0)
					{
						var tile = Main.tile[i, j];
						tile.HasTile = true;
						Main.tile[i, j].TileType = 82;
						Main.tile[i, j].TileFrameX = 36;
						Main.tile[i, j].TileFrameY = 0;
					}
					if (Main.tile[i, j].TileType == 82 && WorldGen.genRand.Next(3) == 0)
					{
						Main.tile[i, j].TileType = 83;
						if (Main.tile[i, j].TileFrameX == 36 && WorldGen.genRand.Next(2) == 0)
						{
							Main.tile[i, j].TileType = 84;
						}
						if (Main.tile[i, j].TileFrameX == 108 && WorldGen.genRand.Next(3) == 0)
						{
							Main.tile[i, j].TileType = 84;
						}
					}
				}
			}
		}

		
		private static bool HasValidGroundForGlowTulipBelowSpot(int x, int y)
		{
			if (!WorldGen.InWorld(x, y, 2))
			{
				return false;
			}
			Tile tile = Main.tile[x, y + 1];
			if (tile == null || !tile.HasTile)
			{
				return false;
			}
			ushort type = tile.TileType;
			if (type < 0)
			{
				return false;
			}
			if (type != 0 && type != 70 && type != 633 && type != 59 && type != 225 && !TileID.Sets.Conversion.Grass[type] && !TileID.Sets.Conversion.Stone[type] && !Main.tileMoss[type])
			{
				return false;
			}
			return WorldGen.SolidTileAllowBottomSlope(x, y + 1);
		}

		private static bool TryGrowingGlowTulip(int i, int j)
		{
			int num = 5;
			for (int k = 0; k < num; k++)
			{
				int num2 = WorldGen.genRand.Next(Math.Max(10, i - 10), Math.Min(Main.maxTilesX - 10, i + 10));
				int num3 = WorldGen.genRand.Next(Math.Max(10, j - 10), Math.Min(Main.maxTilesY - 10, j + 10));
				if (!HasValidGroundForGlowTulipBelowSpot(num2, num3) || !NoNearbyGlowTulips(num2, num3))
				{
					continue;
				}
				WorldGen.PlaceTile(num2, num3, 656, mute: true);
				Tile tile = Main.tile[num2, num3];
				if (tile.HasTile && tile.TileType == 656)
				{
					if (!WorldGen.generatingWorld && Main.netMode == 2 && Main.tile[num2, num3] != null && Main.tile[num2, num3].HasTile)
					{
						NetMessage.SendTileSquare(-1, num2, num3);
					}
					return true;
				}
			}
			return false;
		}

		private static bool NoNearbyGlowTulips(int i, int j)
		{
			int num = Utils.Clamp(i - 120, 10, Main.maxTilesX - 1 - 10);
			int num2 = Utils.Clamp(i + 120, 10, Main.maxTilesX - 1 - 10);
			int num3 = Utils.Clamp(j - 120, 10, Main.maxTilesY - 1 - 10);
			int num4 = Utils.Clamp(j + 120, 10, Main.maxTilesY - 1 - 10);
			for (int k = num; k <= num2; k++)
			{
				for (int l = num3; l <= num4; l++)
				{
					Tile tile = Main.tile[k, l];
					if (tile.HasTile && tile.TileType == 656)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool PlantSeaOat(int x, int y)
		{
			if (Main.tile[x, y].WallType > 0 || Main.tile[x, y].HasTile || Main.tile[x, y].LiquidAmount > 0 || !WorldGen.SolidTileAllowBottomSlope(x, y + 1) || !TileID.Sets.Conversion.Sand[Main.tile[x, y + 1].TileType])
			{
				return false;
			}
			if (!SeaOatWaterCheck(x, y))
			{
				return false;
			}
			var tile = Main.tile[x, y];
			tile.HasTile = true;
			tile.Slope = 0;
			tile.IsHalfBlock = false;
			Main.tile[x, y].TileType = 529;
			Main.tile[x, y].TileFrameX = (short)(WorldGen.genRand.Next(5) * 18);
			int num = 0;
			Main.tile[x, y].TileFrameY = (short)(num * 34);
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, x, y);
			}
			return true;
		}

		private static bool CheckSeaOat(int x, int y)
		{
			if (!SeaOatWaterCheck(x, y))
			{
				WorldGen.KillTile(x, y);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, null, 0, x, y);
				}
				return false;
			}
			return true;
		}

		private static bool GrowSeaOat(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			if (tile.TileFrameX < 180)
			{
				tile = Main.tile[x, y];
				tile.TileFrameX += 90;
			}
			if (Main.netMode == 2)
			{
				NetMessage.SendTileSquare(-1, x, y);
			}
			return false;
		}

		private static bool SeaOatWaterCheck(int x, int y)
		{
			int num = 45;
			int num2 = 20;
			int num3 = 20;
			int num5 = num + 1;
			int num6 = 0;
			bool flag = false;
			if (x <= WorldGen.beachDistance || x >= Main.maxTilesX - WorldGen.beachDistance)
			{
				flag = true;
				num = 65;
				num2 += 5;
			}
			for (int i = x - num; i <= x + num; i++)
			{
				for (int j = y - num2; j <= y + num2; j++)
				{
					if (WorldGen.InWorld(i, j) && !WorldGen.SolidTile(i, j) && Main.tile[i, j].LiquidAmount > 0)
					{
						num6 += Main.tile[i, j].LiquidAmount;
						int num7 = Math.Abs(i - x);
						if (num7 < num5)
						{
							num5 = num7;
						}
					}
				}
			}
			if (num6 / 255 >= num3)
			{
				if (flag)
				{
					return false;
				}
				return true;
			}
			if (flag)
			{
				return true;
			}
			return false;
		}

		private static int RollRandomSeaShellStyle()
		{
			int result = WorldGen.genRand.Next(2);
			if (WorldGen.genRand.Next(10) == 0)
			{
				result = 2;
			}
			if (WorldGen.genRand.Next(10) == 0)
			{
				result = 3;
			}
			if (WorldGen.genRand.Next(50) == 0)
			{
				result = 4;
			}
			return result;
		}

		private static bool PlaceBamboo(int x, int y)
		{
			int num = 2;
			int num2 = 5;
			int num3 = WorldGen.genRand.Next(1, 21);
			Tile tile = Main.tile[x, y];
			if (tile.WallType > 0 && (double)y <= Main.worldSurface)
			{
				return false;
			}
			if (tile.HasTile && tile.TileType == 314)
			{
				return false;
			}
			Tile tile2 = Main.tile[x, y + 1];
			if (tile2.TileType == 571 || tile2.TileType == 60)
			{
				int waterDepth = GetWaterDepth(x, y);
				if (waterDepth < num || waterDepth > num2)
				{
					return false;
				}
				int num4 = CountGrowingPlantTiles(x, y, 5, 571);
				int i = 1;
				if (tile2.TileType == 571)
				{
					for (; !WorldGen.SolidTile(x, y + i); i++)
					{
					}
					if (i + num4 / WorldGen.genRand.Next(1, 21) > num3)
					{
						return false;
					}
				}
				else
				{
					num4 += 25;
				}
				num4 += i * 2;
				if (num4 > WorldGen.genRand.Next(40, 61))
				{
					return false;
				}
				tile = Main.tile[x, y];
				tile.HasTile = true;
				tile.TileType = 571;
				tile.TileFrameX = 0;
				tile.TileFrameY = 0;
				tile.Slope = 0;
				tile.IsHalfBlock = false;
				WorldGen.SquareTileFrame(x, y);
				return true;
			}
			return false;
		}

		private static int GetWaterDepth(int x, int y)
		{
			int num = y;
			while (!WorldGen.SolidTile(x, num))
			{
				num++;
				if (num > Main.maxTilesY - 1)
				{
					return 0;
				}
			}
			num--;
			int num2 = num;
			while (Main.tile[x, num2].LiquidAmount > 0 && !WorldGen.SolidTile(x, num2))
			{
				num2--;
			}
			return num - num2;
		}

		private static int CountGrowingPlantTiles(int x, int y, int range, int type)
		{
			int num = 0;
			for (int i = x - range; i <= x + range; i++)
			{
				for (int j = y - range * 3; j <= y + range * 3; j++)
				{
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type)
					{
						num++;
					}
				}
			}
			return num;
		}

		
		private static void FinishNotTheBees()
		{
			if (!WorldGen.notTheBees)
			{
				return;
			}
			int num = 0;
			for (int i = 20; (double)i < Main.worldSurface; i++)
			{
				for (int j = 20; j < Main.maxTilesX - 20; j++)
				{
					if (Main.tile[j, i].HasTile && TileID.Sets.Clouds[Main.tile[j, i].TileType])
					{
						num = i;
						break;
					}
				}
			}
			for (int k = 25; k < Main.maxTilesX - 25; k++)
			{
				for (int l = 25; l < Main.maxTilesY - 25; l++)
				{
					if (Main.tile[k, l].TileType == 571)
					{
						WorldGen.TileFrame(k, l);
					}
					int num2 = 20;
					if (WorldGen.remixWorldGen)
					{
						num2 = 10;
					}
					if (Main.tile[k, l].TileType == 25 || (WorldGen.remixWorldGen && Main.tile[k, l].TileType == 23 && (double)l < Main.worldSurface))
					{
						for (int m = k - num2; m <= k + num2; m++)
						{
							for (int n = l - num2; n <= l + num2; n++)
							{
								if (Main.tile[m, n].TileType == 60)
								{
									if (Main.tile[m, n + 1].TileType == 444)
									{
										WorldGen.KillTile(m, n + 1);
									}
									Main.tile[m, n].TileType = 661;
									if (Main.tile[m, n - 1].TileType == 61 || Main.tile[m, n - 1].TileType == 74)
									{
										var tile = Main.tile[m, n - 1];
										tile.HasTile = false;
										WorldGen.PlaceTile(m, n - 1, 24);
									}
								}
								else if (Main.tile[m, n - 1].TileType == 233 || Main.tile[m, n - 1].TileType == 82)
								{
									WorldGen.KillTile(m, n - 1);
								}
							}
						}
					}
					else if (Main.tile[k, l].TileType == 203 || (WorldGen.remixWorldGen && Main.tile[k, l].TileType == 199 && (double)l < Main.worldSurface))
					{
						for (int num3 = k - num2; num3 <= k + num2; num3++)
						{
							for (int num4 = l - num2; num4 <= l + num2; num4++)
							{
								if (Main.tile[num3, num4].TileType == 60)
								{
									if (Main.tile[num3, num4 + 1].TileType == 444)
									{
										WorldGen.KillTile(num3, num4 + 1);
									}
									Main.tile[num3, num4].TileType = 662;
									if (Main.tile[num3, num4 - 1].TileType == 61 || Main.tile[num3, num4 - 1].TileType == 74)
									{
										var tile = Main.tile[num3, num4 - 1];
										tile.HasTile = false;
										WorldGen.PlaceTile(num3, num4 - 1, 201);
									}
									else if (Main.tile[num3, num4 - 1].TileType == 233 || Main.tile[num3, num4 - 1].TileType == 82)
									{
										WorldGen.KillTile(num3, num4 - 1);
									}
								}
							}
						}
					}
					if (Main.tile[k, l].TileType == 382 || Main.tile[k, l].TileType == 52)
					{
						Main.tile[k, l].TileType = 62;
					}
					if (l > GenVars.lavaLine + WorldGen.genRand.Next(-2, 3) + 2)
					{
						if (!WorldGen.remixWorldGen)
						{
							WorldGen.SpreadGrass(k, l, 59, 70);
						}
					}
					else
					{
						WorldGen.SpreadGrass(k, l, 59, 60);
					}
					if ((double)l > Main.rockLayer + 20.0 + (double)WorldGen.genRand.Next(-2, 3) && l <= GenVars.lavaLine + 2 - 20 - WorldGen.genRand.Next(-2, 3) && (k < WorldGen.beachDistance + 200 - 20 - WorldGen.genRand.Next(-2, 3) || k > Main.maxTilesX - WorldGen.beachDistance - 200 + 20 + WorldGen.genRand.Next(-2, 3)))
					{
						if (Main.tile[k, l].LiquidAmount > 0)
						{
							SetLiquidToHoney(Main.tile[k, l], false);
							SetLiquidToLava(Main.tile[k, l], false);
						}
						if (Main.tile[k, l].TileType == 59)
						{
							bool flag = false;
							for (int num5 = k - 1; num5 <= k + 1; num5++)
							{
								for (int num6 = l - 1; num6 <= l + 1; num6++)
								{
									if (Main.tile[num5, num6].TileType == 60)
									{
										flag = true;
									}
								}
							}
							if (!flag)
							{
								if ((double)l < (Main.rockLayer + (double)GenVars.lavaLine) / 2.0)
								{
									Main.tile[k, l].TileType = 161;
								}
								else
								{
									Main.tile[k, l].TileType = 147;
								}
							}
						}
					}
					if (!WorldGen.remixWorldGen)
					{
						if ((Main.tile[k, l].TileType == 7 || Main.tile[k, l].TileType == 166 || Main.tile[k, l].TileType == 6 || Main.tile[k, l].TileType == 167) && (double)l > ((double)GenVars.lavaLine + Main.rockLayer * 2.0) / 3.0 + (double)WorldGen.genRand.Next(-2, 3) + 2.0)
						{
							Main.tile[k, l].TileType = 0;
						}
					}
					else if (!WorldGen.remixWorldGen && (Main.tile[k, l].TileType == 123 || Main.tile[k, l].TileType == 40) && (double)l > ((double)GenVars.lavaLine + Main.rockLayer) / 2.0 + (double)WorldGen.genRand.Next(-2, 3) + 2.0)
					{
						Main.tile[k, l].TileType = 1;
					}
					if (l <= num || (Main.tile[k, l].LiquidAmount != 0 && (IsLava(Main.tile[k, l]) || IsShimmer(Main.tile[k, l]))))
					{
						continue;
					}
					if (WorldGen.getGoodWorldGen)
					{
						if (WorldGen.genRand.Next(150) == 0)
						{
							WorldGen.PlaceTile(k, l, 231, mute: true);
						}
					}
					else if (WorldGen.genRand.Next(25) == 0)
					{
						WorldGen.PlaceTile(k, l, 231, mute: true);
					}
				}
			}
			for (int num7 = 20; num7 < num; num7++)
			{
				for (int num8 = 20; num8 <= Main.maxTilesX - 20; num8++)
				{
					SetLiquidToHoney(Main.tile[num8, num7], false);
					if (Main.tile[num8, num7].TileType == 375)
					{
						Main.tile[num8, num7].TileType = 373;
					}
					if (!WorldGen.remixWorldGen)
					{
						if (Main.tile[num8, num7].TileType == 60)
						{
							Main.tile[num8, num7].TileType = 2;
							if (WorldGen.genRand.Next(2) == 0)
							{
								WorldGen.GrowTreeWithSettings(num8, num7, GrowTreeSettings.Profiles.VanityTree_Willow);
							}
							else
							{
								WorldGen.GrowTreeWithSettings(num8, num7, GrowTreeSettings.Profiles.VanityTree_Sakura);
							}
							if (!Main.tile[num8, num7 - 1].HasTile)
							{
								WorldGen.PlaceTile(num8, num7 - 1, 3);
							}
						}
						if (Main.tile[num8, num7].TileType == 59)
						{
							Main.tile[num8, num7].TileType = 0;
						}
					}
					else
					{
						WorldGen.GrowTree(num8, num7);
					}
				}
			}
		}

		private static void FinishGetGoodWorld()
		{
			int num = 0;
			for (int i = 20; (double)i < Main.worldSurface; i++)
			{
				for (int j = 20; j < Main.maxTilesX - 20; j++)
				{
					if (Main.tile[j, i].HasTile && TileID.Sets.Clouds[Main.tile[j, i].TileType])
					{
						num = i;
						break;
					}
				}
			}
			byte b = (byte)WorldGen.genRand.Next(13, 25);
			for (int k = 0; k < Main.maxTilesX; k++)
			{
				bool flag = false;
				for (int l = 0; l < Main.maxTilesY; l++)
				{
					if (!Main.tile[k, l].HasTile || !Main.tileDungeon[Main.tile[k, l].TileType])
					{
						continue;
					}
					if (Main.tile[k, l].TileType == 44)
					{
						b = (byte)WorldGen.genRand.Next(13, 15);
						if (WorldGen.genRand.Next(2) == 0)
						{
							b = (byte)WorldGen.genRand.Next(23, 25);
						}
					}
					if (Main.tile[k, l].TileType == 43)
					{
						b = (byte)WorldGen.genRand.Next(15, 19);
					}
					if (Main.tile[k, l].TileType == 41)
					{
						b = (byte)WorldGen.genRand.Next(19, 23);
					}
				}
				if (flag)
				{
					break;
				}
			}
			for (int m = 0; m < Main.maxTilesX; m++)
			{
				for (int n = 5; n < Main.maxTilesY - 5; n++)
				{
					if (Main.tile[m, n].HasTile && (Main.tileDungeon[Main.tile[m, n].TileType] || TileID.Sets.CrackedBricks[Main.tile[m, n].TileType]))
					{
						var tile = Main.tile[m, n];
						tile.TileColor = b;
					}
					if (Main.wallDungeon[Main.tile[m, n].WallType])
					{
						var tile = Main.tile[m, n];
						tile.WallColor = b;
					}
					if (Main.tile[m, n].HasTile)
					{
						bool flag2 = false;
						if (Main.tile[m, n].TileType == 226)
						{
							flag2 = true;
						}
						if (Main.tile[m, n].TileType == 137)
						{
							int num2 = Main.tile[m, n].TileFrameY / 18;
							if (num2 >= 1 && num2 <= 4)
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							var tile = Main.tile[m, n];
							tile.TileColor = 17;
						}
					}
					if (Main.tile[m, n].WallType == 87)
					{
						var tile = Main.tile[m, n];
						tile.WallColor = 25;
					}
					if (!Main.tile[m, n].HasTile)
					{
						continue;
					}
					if (!WorldGen.remixWorldGen && Main.tile[m, n].TileType == 57 && WorldGen.genRand.Next(15) == 0)
					{
						if (Main.tile[m, n - 1].TileType == 57)
						{
							var tile = Main.tile[m, n];
							tile.HasTile = false;
						}
						Main.tile[m, n].LiquidAmount = byte.MaxValue;
						SetLiquidToLava(Main.tile[m, n], true);
					}
					if (n < num && Main.tile[m, n].TileType == 2)
					{
						if (WorldGen.crimson)
						{
							Main.tile[m, n].TileType = 199;
						}
						else
						{
							Main.tile[m, n].TileType = 23;
						}
						if (Main.tile[m, n - 1].TileType == 3)
						{
							var tile = Main.tile[m, n - 1];
							tile.HasTile = false;
						}
						if (Main.tile[m, n - 1].TileType == 73)
						{
							var tile = Main.tile[m, n - 1];
							tile.HasTile = false;
						}
						if (Main.tile[m, n - 1].TileType == 27)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 596)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 616)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 82)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 83)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 186)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 187)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 185)
						{
							WorldGen.KillTile(m, n - 1);
						}
						if (Main.tile[m, n - 1].TileType == 227)
						{
							WorldGen.KillTile(m, n - 1);
						}
					}
				}
			}
			for (int num3 = 0; num3 < 8000 && Main.chest[num3] != null; num3++)
			{
				if (WorldGen.genRand.Next(10) != 0 || Main.chest[num3].item[1].stack == 0)
				{
					continue;
				}
				for (int num4 = 1; num4 < 40; num4++)
				{
					if (Main.chest[num3].item[num4].stack == 0)
					{
						Main.chest[num3].item[num4].SetDefaults(678);
						break;
					}
				}
			}
		}

		private static void FinishNoTraps()
		{
			Main.tileSolid[138] = false;
			for (int i = 50; i < Main.maxTilesX - 50; i++)
			{
				for (int j = 50; j < Main.maxTilesY - 50; j++)
				{
					Tile tile = Main.tile[i, j];
					if (WorldGen.genRand.Next(5) == 0 && tile.HasTile && tile.TileType == 12 && tile.TileFrameX == 0 && tile.TileFrameY == 0)
					{
						Main.tile[i, j].TileType = 665;
						Main.tile[i, j + 1].TileType = 665;
						Main.tile[i + 1, j].TileType = 665;
						Main.tile[i + 1, j + 1].TileType = 665;
					}
					if (i % 2 != 0 || j % 2 != 0 || !Main.tile[i, j].HasTile || (Main.tile[i, j].TileType != 105 && (Main.tile[i, j].TileType != 467 || (Main.tile[i, j].TileFrameX != 144 && Main.tile[i, j].TileFrameX != 162))))
					{
						continue;
					}
					bool flag = false;
					for (int k = i - 1; k <= i + 1; k++)
					{
						for (int l = j - 1; l <= j + 1; l++)
						{
							if (Main.tile[k, l].RedWire)
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						bool flag2 = false;
						int num = 25;
						int num2 = -1;
						int num3 = -1;
						for (int m = 0; m < num * num; m++)
						{
							num2 = WorldGen.genRand.Next(i - num, i + num + 1);
							num3 = WorldGen.genRand.Next(j - num, j + num + 1);
							if (Main.tile[num2, num3].RedWire)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							int num4 = i;
							int num5 = j;
							var temptile = Main.tile[num4, num5];
							temptile.RedWire = true;
							while (num4 != num2)
							{
								if (num4 < num2)
								{
									num4++;
								}
								if (num4 > num2)
								{
									num4--;
								}
								temptile = Main.tile[num4, num5];
								temptile.RedWire = true;
							}
							while (num5 != num3)
							{
								if (num5 < num3)
								{
									num5++;
								}
								if (num5 > num3)
								{
									num5--;
								}
								temptile = Main.tile[num4, num5];
								temptile.RedWire = true;
							}
						}
						else if (Main.tile[i, j].TileType == 105)
						{
							num = 15;
							bool flag3 = false;
							for (int n = 0; n < num * num; n++)
							{
								num2 = i + WorldGen.genRand.Next(-num, num + 1);
								num3 = j + WorldGen.genRand.Next(-num, num + 1);
								WorldGen.PlaceTile(num2, num3, 135, mute: true, forced: true, -1, WorldGen.genRand.Next(2, 4));
								if (Main.tile[num2, num3].TileType == 135)
								{
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								int num6 = i;
								int num7 = j;
								var temptile = Main.tile[num6, num7];
								temptile.RedWire = true;
								while (num6 != num2)
								{
									if (num6 < num2)
									{
										num6++;
									}
									if (num6 > num2)
									{
										num6--;
									}
									temptile = Main.tile[num6, num7];
									temptile.RedWire = true;
								}
								while (num7 != num3)
								{
									if (num7 < num3)
									{
										num7++;
									}
									if (num7 > num3)
									{
										num7--;
									}
									temptile = Main.tile[num6, num7];
									temptile.RedWire = true;
								}
							}
						}
					}
					if (Main.tile[i, j].TileType == 467)
					{
						int num8 = 8;
						for (int num9 = 0; num9 < num8 * num8; num9++)
						{
							int num10 = i + WorldGen.genRand.Next(-num8, num8 + 1);
							int num11 = j + WorldGen.genRand.Next(-num8, num8 + 1);
							if (Main.tile[num10, num11].TileType != 0 && Main.tile[num10, num11].TileType != 1 && !TileID.Sets.Ore[Main.tile[num10, num11].TileType] && Main.tile[num10, num11].TileType != 59 && Main.tile[num10, num11].TileType != 151)
							{
								continue;
							}
							bool flag4 = true;
							for (int num12 = num10 - 1; num12 <= num10 + 1; num12++)
							{
								for (int num13 = num11 - 1; num13 <= num11 + 1; num13++)
								{
									if (!WorldGen.SolidTile(num12, num13))
									{
										flag4 = false;
									}
								}
							}
							if (!flag4)
							{
								continue;
							}
							Tile tile2 = Main.tile[num10, num11];
							tile2.TileType = 141;
							tile2.TileFrameX = (tile2.TileFrameY = 0);
							tile2.Slope = 0;
							tile2.IsHalfBlock = false;
							WorldGen.TileFrame(num10, num11, resetFrame: true);
							if (Main.tile[num10, num11].TileType != 141)
							{
								continue;
							}
							int num14 = i;
							int num15 = j;
							var temptile = Main.tile[num14, num15];
							temptile.RedWire = true;
							while (num14 != num10)
							{
								if (num14 < num10)
								{
									num14++;
								}
								if (num14 > num10)
								{
									num14--;
								}
								temptile = Main.tile[num14, num15];
								temptile.RedWire = true;
							}
							while (num15 != num11)
							{
								if (num15 < num11)
								{
									num15++;
								}
								if (num15 > num11)
								{
									num15--;
								}
								temptile = Main.tile[num14, num15];
								temptile.RedWire = true;
							}
							break;
						}
					}
					else
					{
						j++;
					}
				}
			}
			Main.tileSolid[138] = true;
			for (int num16 = 0; num16 < 8000 && Main.chest[num16] != null; num16++)
			{
				if (WorldGen.genRand.Next(20) != 0 || Main.chest[num16].item[1].stack == 0)
				{
					continue;
				}
				for (int num17 = 1; num17 < 40; num17++)
				{
					if (Main.chest[num16].item[num17].stack == 0)
					{
						Main.chest[num16].item[num17].SetDefaults(5346);
						break;
					}
				}
			}
		}

		private static void FinishDrunkGen()
		{
			byte color = (byte)WorldGen.genRand.Next(13, 25);
			byte b = 16;
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				bool flag = false;
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					if (!Main.tile[i, j].HasTile || !Main.tileDungeon[Main.tile[i, j].TileType])
					{
						continue;
					}
					if (Main.tile[i, j].TileType == 44)
					{
						color = (byte)WorldGen.genRand.Next(13, 15);
						if (WorldGen.genRand.Next(2) == 0)
						{
							color = (byte)WorldGen.genRand.Next(23, 25);
						}
					}
					if (Main.tile[i, j].TileType == 43)
					{
						color = (byte)WorldGen.genRand.Next(15, 19);
					}
					if (Main.tile[i, j].TileType == 41)
					{
						color = (byte)WorldGen.genRand.Next(19, 23);
					}
				}
				if (flag)
				{
					break;
				}
			}
			for (int k = 10; k < Main.maxTilesX - 10; k++)
			{
				for (int l = 10; l < Main.maxTilesY - 10; l++)
				{
					if (Main.tile[k, l].HasTile && (Main.tileDungeon[Main.tile[k, l].TileType] || TileID.Sets.CrackedBricks[Main.tile[k, l].TileType]))
					{
						var tile = Main.tile[k, l];
						tile.TileColor = color;
					}
					if (Main.wallDungeon[Main.tile[k, l].WallType])
					{
						var tile = Main.tile[k, l];
						tile.WallColor = 25;
					}
					if (Main.tile[k, l].HasTile)
					{
						if (Main.tile[k, l].TileType == 60)
						{
							int num = 1;
							for (int m = k - num; m <= k + num; m++)
							{
								for (int n = l - num; n <= l + num; n++)
								{
									if (Main.tile[m, n].TileType == 147 || Main.tile[m, n].TileType == 161)
									{
										Main.tile[m, n].TileType = 59;
									}
								}
							}
						}
						bool flag2 = false;
						if (Main.tile[k, l].TileType == 226)
						{
							flag2 = true;
						}
						if (Main.tile[k, l].TileType == 137)
						{
							int num2 = Main.tile[k, l].TileFrameY / 18;
							if (num2 >= 1 && num2 <= 4)
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							var tile = Main.tile[k, l];
							tile.TileColor = b;
						}
					}
					if (Main.tile[k, l].WallType == 87)
					{
						var tile = Main.tile[k, l];
						tile.WallColor = b;
					}
				}
			}
			for (int num3 = 0; num3 < 8000 && Main.chest[num3] != null; num3++)
			{
				if (WorldGen.genRand.Next(15) == 0 && Main.chest[num3].item[1].stack != 0)
				{
					for (int num4 = 1; num4 < 40; num4++)
					{
						if (Main.chest[num3].item[num4].stack == 0)
						{
							Main.chest[num3].item[num4].SetDefaults(5001);
							break;
						}
					}
				}
				if (WorldGen.genRand.Next(30) != 0 || Main.chest[num3].item[1].stack == 0)
				{
					continue;
				}
				for (int num5 = 1; num5 < 40; num5++)
				{
					if (Main.chest[num3].item[num5].stack == 0)
					{
						Main.chest[num3].item[num5].SetDefaults(678);
						break;
					}
				}
			}
		}

		private static void FinishRemixWorld()
		{
			for (int i = 25; i < Main.maxTilesX - 25; i++)
			{
				for (int j = 25; j < Main.maxTilesY - 25; j++)
				{
					int conversionType = 1;
					if (WorldGen.crimson)
					{
						conversionType = 4;
					}
					if (WorldGen.notTheBees && (double)j < Main.worldSurface)
					{
						if (IsHoney(Main.tile[i, j]))
						{
							if (Main.tileLavaDeath[Main.tile[i, j].TileType])
							{
								WorldGen.KillTile(i, j);
							}
							SetLiquidToLava(Main.tile[i, j], true);
						}
						if (Main.tile[i, j].TileType == 375)
						{
							Main.tile[i, j].TileType = 374;
						}
						if (Main.tile[i, j].TileType == 230 || Main.tile[i, j].TileType == 229 || Main.tile[i, j].TileType == 659 || Main.tile[i, j].TileType == 56)
						{
							WorldGen.KillTile(i, j);
						}
						if (Main.tile[i, j].TileType == 82 || Main.tile[i, j].TileType == 83 || Main.tile[i, j].TileType == 84)
						{
							WorldGen.TileFrame(i, j);
						}
					}
					if ((double)j < Main.worldSurface + (double)WorldGen.genRand.Next(3))
					{
						if (WorldGen.drunkWorldGen)
						{
							if (GenVars.crimsonLeft)
							{
								if (i < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
								{
									WorldGen.Convert(i, j, 4, 1);
								}
								else
								{
									WorldGen.Convert(i, j, 1, 1);
								}
							}
							else if (i < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
							{
								WorldGen.Convert(i, j, 1, 1);
							}
							else
							{
								WorldGen.Convert(i, j, 4, 1);
							}
						}
						else
						{
							WorldGen.Convert(i, j, conversionType, 1);
						}
					}
					if ((double)j < Main.worldSurface - (double)WorldGen.genRand.Next(19, 22) && (Main.tile[i, j].WallType == 178 || Main.tile[i, j].WallType == 180))
					{
						Main.tile[i, j].WallType = 0;
					}
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == 56)
					{
						WorldGen.KillTile(i, j);
					}
					if (Main.tile[i, j].TileType == 189 || Main.tile[i, j].TileType == 196 || Main.tile[i, j].TileType == 202)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (GenVars.crimsonLeft)
							{
								if (i < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
								{
									Main.tile[i, j].TileType = 195;
								}
								else
								{
									Main.tile[i, j].TileType = 474;
								}
							}
							else if (i < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
							{
								Main.tile[i, j].TileType = 474;
							}
							else
							{
								Main.tile[i, j].TileType = 195;
							}
						}
						else if (WorldGen.crimson)
						{
							Main.tile[i, j].TileType = 195;
						}
						else
						{
							Main.tile[i, j].TileType = 474;
						}
					}
					if (Main.tile[i, j].WallType == 73 || Main.tile[i, j].WallType == 82)
					{
						if (WorldGen.drunkWorldGen)
						{
							if (GenVars.crimsonLeft)
							{
								if (i < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
								{
									Main.tile[i, j].WallType = 77;
								}
								else
								{
									Main.tile[i, j].WallType = 233;
								}
							}
							else if (i < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3))
							{
								Main.tile[i, j].WallType = 233;
							}
							else
							{
								Main.tile[i, j].WallType = 77;
							}
						}
						else if (WorldGen.crimson)
						{
							Main.tile[i, j].WallType = 77;
						}
						else
						{
							Main.tile[i, j].WallType = 233;
						}
					}
					if ((double)j > Main.rockLayer && j < Main.maxTilesY - 350 && Main.tile[i, j].TileType == 0 && Main.tile[i, j].HasTile && (!Main.tile[i - 1, j - 1].HasTile || !WorldGen.SolidTile(i, j - 1) || !Main.tile[i + 1, j - 1].HasTile || !Main.tile[i - 1, j].HasTile || !Main.tile[i + 1, j].HasTile || !Main.tile[i - 1, j + 1].HasTile || !Main.tile[i, j + 1].HasTile || !Main.tile[i + 1, j + 1].HasTile))
					{
						Main.tile[i, j].TileType = 2;
					}
				}
			}
			Liquid.QuickWater(-2);
			int num = (int)((double)Main.maxTilesX * 0.38);
			int num2 = (int)((double)Main.maxTilesX * 0.62);
			_ = Main.maxTilesY;
			int num3 = Main.maxTilesY - 135;
			_ = Main.maxTilesY;
			for (int k = num; k < num2 + 15; k++)
			{
				for (int l = Main.maxTilesY - 200; l < num3 + 10; l++)
				{
					Main.tile[k, l].LiquidAmount = 0;
					if (Main.tile[k, l].TileType == 58)
					{
						Main.tile[k, l].TileType = 57;
					}
				}
			}
			WorldGen.AddTrees(undergroundOnly: true);
			for (int m = 0; m < Main.maxTilesX; m++)
			{
				byte color = 22;
				byte color2 = 22;
				if (WorldGen.drunkWorldGen)
				{
					if ((GenVars.crimsonLeft && m < Main.maxTilesX / 2) || (!GenVars.crimsonLeft && m > Main.maxTilesX / 2))
					{
						color2 = 13;
						color = 13;
					}
				}
				else if (WorldGen.crimson)
				{
					color2 = 13;
					color = 13;
				}
				for (int n = 0; n < Main.maxTilesY; n++)
				{
					if (Main.tile[m, n].HasTile && (Main.tileDungeon[Main.tile[m, n].TileType] || TileID.Sets.CrackedBricks[Main.tile[m, n].TileType]))
					{
						var tile = Main.tile[m, n];
						tile.TileColor = color;
					}
					if (Main.wallDungeon[Main.tile[m, n].WallType])
					{
						var tile = Main.tile[m, n];
						tile.WallColor = 25;
						if (Main.tile[m, n].TileType == 19 && Main.tile[m, n].TileFrameY != 180)
						{
							tile.TileColor = color;
						}
					}
					if (Main.tile[m, n].HasTile)
					{
						bool flag = false;
						if (Main.tenthAnniversaryWorld)
						{
							if (Main.tile[m, n].TileType == 191)
							{
								flag = true;
							}
							if (Main.tile[m, n].HasTile && Main.tile[m, n].TileType == 151)
							{
								var tile = Main.tile[m, n];
								tile.TileColor = color;
							}
						}
						if (Main.tile[m, n].TileType == 226)
						{
							flag = true;
						}
						if (Main.tile[m, n].TileType == 137)
						{
							int num4 = Main.tile[m, n].TileFrameY / 18;
							if (num4 >= 1 && num4 <= 4)
							{
								flag = true;
							}
						}
						if (flag)
						{
							var tile = Main.tile[m, n];
							tile.TileColor = color2;
						}
					}
					if (Main.tile[m, n].WallType == 244)
					{
						var tile = Main.tile[m, n];
						tile.WallColor = 25;
					}
					if (Main.tile[m, n].WallType == 34)
					{
						var tile = Main.tile[m, n];
						tile.WallColor = 25;
					}
					if (Main.tile[m, n].WallType == 87)
					{
						var tile = Main.tile[m, n];
						tile.WallColor = 25;
						tile.TileColor = color2;
					}
				}
			}
			double num5 = (double)Main.maxTilesX / 4200.0;
			num5 *= (double)WorldGen.genRand.Next(2, 5);
			for (int num6 = 0; (double)num6 < num5; num6++)
			{
				int num7 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.39), (int)((double)Main.maxTilesX * 0.61));
				int num8 = WorldGen.genRand.Next(10, 31);
				for (int num9 = num7 - num8; num9 <= num7 + num8; num9++)
				{
					for (int num10 = Main.maxTilesY - 250; num10 < Main.maxTilesY - 25; num10++)
					{
						if (Main.tile[num9, num10].TileType == 637)
						{
							Main.tile[num9, num10].TileFrameX = (short)(18 * Main.rand.Next(6, 11));
						}
					}
				}
			}
			if (WorldGen.notTheBees)
			{
				for (int num11 = 3; num11 < Main.maxTilesX - 3; num11++)
				{
					bool flag2 = true;
					for (int num12 = 0; (double)num12 < Main.worldSurface; num12++)
					{
						if (flag2)
						{
							if (Main.tile[num11, num12].WallType == 86)
							{
								Main.tile[num11, num12].WallType = 0;
							}
							if (Main.tile[num11, num12].HasTile)
							{
								flag2 = false;
							}
						}
						else if (Main.tile[num11, num12].WallType == 0 && Main.tile[num11, num12 + 1].WallType == 0 && Main.tile[num11, num12 + 2].WallType == 0 && Main.tile[num11, num12 + 3].WallType == 0 && Main.tile[num11, num12 + 4].WallType == 0 && Main.tile[num11 - 1, num12].WallType == 0 && Main.tile[num11 + 1, num12].WallType == 0 && Main.tile[num11 - 2, num12].WallType == 0 && Main.tile[num11 + 2, num12].WallType == 0 && !Main.tile[num11, num12].HasTile && !Main.tile[num11, num12 + 1].HasTile && !Main.tile[num11, num12 + 2].HasTile && !Main.tile[num11, num12 + 3].HasTile)
						{
							flag2 = true;
						}
					}
				}
			}
			Liquid.QuickWater(-2);
			for (int num13 = 0; num13 < Main.maxTilesX; num13++)
			{
				for (int num14 = 0; num14 < Main.maxTilesY; num14++)
				{
					if (Main.tile[num13, num14].TileType == 518)
					{
						WorldGen.CheckLilyPad(num13, num14);
					}
				}
			}
		}
		
		private static void FinishTenthAnniversaryWorld()
		{
			if (!WorldGen.remixWorldGen)
			{
				if (!WorldGen.getGoodWorldGen && !WorldGen.drunkWorldGen)
				{
					ConvertSkyIslands(2, growTrees: true);
				}
				PaintTheDungeon(24, 24);
				PaintTheLivingTrees(12, 12);
				PaintTheTemple(10, 5);
				PaintTheClouds(12, 12);
				PaintTheSand(7, 7);
				PaintThePyramids(12, 12);
			}
			PaintTheTrees();
			PaintTheMushrooms();
			if (!WorldGen.getGoodWorldGen)
			{
				for (int i = 50; i < Main.maxTilesX - 50; i++)
				{
					for (int j = 50; j < Main.maxTilesY - 50; j++)
					{
						Tile tile = Main.tile[i, j];
						if (WorldGen.genRand.Next(4) == 0 && tile.HasTile && tile.TileType == 138 && tile.TileFrameX == 0 && tile.TileFrameY == 0)
						{
							Main.tile[i, j].TileType = 665;
							Main.tile[i, j + 1].TileType = 665;
							Main.tile[i + 1, j].TileType = 665;
							Main.tile[i + 1, j + 1].TileType = 665;
						}
					}
				}
			}
			if (!WorldGen.getGoodWorldGen)
			{
				ImproveAllChestContents();
			}
		}

		private static void ShimmerCleanUp()
		{
			WorldGen.ShimmerRemoveWater();
			int num = 120;
			int num2 = 90;
			int num3 = (int)GenVars.shimmerPosition.X - num;
			int num4 = (int)GenVars.shimmerPosition.X + num;
			int num5 = (int)GenVars.shimmerPosition.Y - num;
			int num6 = (int)GenVars.shimmerPosition.Y + num;
			_ = num / 4;
			for (int i = num5; i <= num6; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					if ((int)Math.Sqrt(Math.Pow(Math.Abs((double)j - GenVars.shimmerPosition.X), 2.0) + Math.Pow(Math.Abs((double)i - GenVars.shimmerPosition.Y), 2.0)) < num)
					{
						if (Main.tile[j, i].TileType == 22 || Main.tile[j, i].TileType == 204)
						{
							Main.tile[j, i].TileType = 1;
						}
						if (Main.tile[j, i].TileType == 51 || Main.tile[j, i].TileType == 56 || Main.tile[j, i].TileType == 229 || Main.tile[j, i].TileType == 230 || Main.tile[j, i].TileType == 659)
						{
							var tile = Main.tile[j, i];
							tile.HasTile = false;
						}
						if (TileID.Sets.Conversion.Moss[Main.tile[j, i].TileType])
						{
							Main.tile[j, i].TileType = 1;
						}
						if (Main.tile[j, i].TileType == 184)
						{
							var tile = Main.tile[j, i];
							tile.HasTile = false;
						}
					}
					if (((!((double)i > GenVars.shimmerPosition.Y)) ? ((int)Math.Sqrt(Math.Pow(Math.Abs((double)j - GenVars.shimmerPosition.X) * (1.0 + WorldGen.genRand.NextDouble() * 0.02), 2.0) + Math.Pow(Math.Abs((double)i - GenVars.shimmerPosition.Y) * 1.4 * (1.0 + WorldGen.genRand.NextDouble() * 0.02), 2.0))) : ((int)Math.Sqrt(Math.Pow(Math.Abs((double)j - GenVars.shimmerPosition.X) * (1.0 + WorldGen.genRand.NextDouble() * 0.02), 2.0) + Math.Pow(Math.Abs((double)i - GenVars.shimmerPosition.Y) * 1.2 * (1.0 + WorldGen.genRand.NextDouble() * 0.02), 2.0)))) < num2)
					{
						WorldGen.Convert(j, i, 0, 3);
					}
				}
			}
			int num8 = (int)GenVars.shimmerPosition.X;
			int num9 = (int)GenVars.shimmerPosition.Y;
			byte b = 127;
			Liquid.tilesIgnoreWater(ignoreSolids: true);
			while (Main.tile[num8, num9].LiquidAmount <= b || !IsShimmer(Main.tile[num8, num9]))
			{
				while (!Main.tile[num8, num9].HasTile)
				{
					Main.tile[num8, num9].LiquidAmount = b;
					SetLiquidToShimmer(Main.tile[num8, num9], true);
					num8--;
				}
				for (num8 = (int)GenVars.shimmerPosition.X; !Main.tile[num8, num9].HasTile; num8++)
				{
					Main.tile[num8, num9].LiquidAmount = b;
					SetLiquidToShimmer(Main.tile[num8, num9], true);
				}
				num8 = (int)GenVars.shimmerPosition.X;
				num9++;
				b = byte.MaxValue;
				if (Main.tile[num8, num9].HasTile)
				{
					break;
				}
			}
			if (WorldGen.tenthAnniversaryWorldGen)
			{
				int num10 = 170;
				for (int k = (int)GenVars.shimmerPosition.X - num10; (double)k <= GenVars.shimmerPosition.X + (double)num10; k++)
				{
					for (int l = (int)GenVars.shimmerPosition.Y + 40; l < Main.maxTilesY - 330 - 100; l++)
					{
						if (WorldGen.InWorld(k, l))
						{
							if (Main.tile[k, l].TileType == 375 || Main.tile[k, l].TileType == 374 || Main.tile[k, l].TileType == 373)
							{
								var tile = Main.tile[k, l];
								tile.HasTile = false;
							}
							if (Main.tile[k, l].LiquidAmount > 0 && !IsShimmer(Main.tile[k, l]))
							{
								WorldGen.Shimmerator(k, l);
							}
						}
					}
				}
			}
			Liquid.tilesIgnoreWater(ignoreSolids: false);
			for (int m = 10; m < Main.maxTilesX - 10; m++)
			{
				for (int n = 10; n < Main.maxTilesY - 10; n++)
				{
					if (Main.tile[m, n].LiquidAmount > 0 && IsShimmer(Main.tile[m, n]) && Main.tile[m, n].TileType == 5)
					{
						WorldGen.KillTile(m, n);
					}
				}
			}
		}

		
		private static void PaintTheMushrooms()
		{
			int num = Main.maxTilesY - 20;
			byte b = (byte)WorldGen.genRand.Next(1, 13);
			if (WorldGen.remixWorldGen)
			{
				b = 2;
				num = Main.maxTilesY - 500;
				int num2 = WorldGen.genRand.Next(5, 31);
				if (WorldGen.genRand.Next(2) == 0)
				{
					num2 = WorldGen.genRand.Next(5, 16);
				}
				for (int i = 20; i < Main.maxTilesX - 20; i++)
				{
					if (i % num2 == 0)
					{
						b++;
						if (b > 12)
						{
							b = 1;
						}
					}
					for (int j = Main.maxTilesY - 450; j < Main.maxTilesY - 20; j++)
					{
						Tile tile = Main.tile[i, j];
						if (tile.HasTile && (tile.TileType == 70 || tile.TileType == 578 || tile.TileType == 190 || tile.TileType == 71 || tile.TileType == 528 || (tile.TileType == 519 && tile.TileFrameY == 90)))
						{
							tile.TileColor = b;
						}
						if (tile.WallType == 80 || tile.WallType == 74)
						{
							tile.WallColor = b;
						}
					}
				}
			}
			b = (byte)WorldGen.genRand.Next(1, 13);
			int num3 = 0;
			for (int k = 20; k < Main.maxTilesX - 20; k++)
			{
				for (int l = 20; l < num; l++)
				{
					Tile tile2 = Main.tile[k, l];
					if (tile2.HasTile && (tile2.TileType == 70 || tile2.TileType == 578 || tile2.TileType == 190 || tile2.TileType == 71 || tile2.TileType == 528 || (tile2.TileType == 519 && tile2.TileFrameY == 90)))
					{
						tile2.TileColor = b;
						num3 = 10;
					}
					if (tile2.WallType == 80 || tile2.WallType == 74)
					{
						tile2.WallColor = b;
						num3 = 10;
					}
				}
				num3--;
				if (num3 == 0)
				{
					b += (byte)WorldGen.genRand.Next(1, 3);
					if (b > 12)
					{
						b = 1;
					}
				}
			}
		}

		private static void PaintTheTrees()
		{
			int num = 20;
			if (WorldGen.remixWorldGen)
			{
				num = (int)Main.worldSurface;
			}
			byte b = (byte)WorldGen.genRand.Next(1, 13);
			bool flag = false;
			for (int i = 20; i < Main.maxTilesX - 20; i++)
			{
				bool flag2 = false;
				for (int j = num; j < Main.maxTilesY - 20; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile && (tile.TileType == 5 || tile.TileType == 323 || tile.TileType == 596 || tile.TileType == 616))
					{
						tile.TileColor = b;
						flag2 = true;
						flag = true;
					}
				}
				if (flag && !flag2)
				{
					flag = false;
					b++;
					if (b > 12)
					{
						b = 1;
					}
				}
			}
		}

		private static void PaintTheSand(byte tilePaintColor, byte wallPaintColor)
		{
			for (int i = 20; i < Main.maxTilesX - 20; i++)
			{
				for (int j = 20; j < Main.maxTilesY - 20; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile && (tile.TileType == 53 || tile.TileType == 396 || tile.TileType == 397))
					{
						tile.TileColor = tilePaintColor;
						if ((double)j > Main.worldSurface)
						{
							if (Main.tile[i, j - 1].TileType == 165 || Main.tile[i, j - 1].TileType == 185 || Main.tile[i, j - 1].TileType == 186 || Main.tile[i, j - 1].TileType == 187)
							{
								var tile2 = Main.tile[i, j - 1];
								tile2.TileColor = tilePaintColor;
							}
							if (Main.tile[i, j - 2].TileType == 165 || Main.tile[i, j - 2].TileType == 185 || Main.tile[i, j - 2].TileType == 186 || Main.tile[i, j - 2].TileType == 187)
							{
								var tile2 = Main.tile[i, j - 2];
								tile2.TileColor = tilePaintColor;
							}
							if (Main.tile[i, j + 1].TileType == 165)
							{
								var tile2 = Main.tile[i, j + 1];
								tile2.TileColor = tilePaintColor;
							}
							if (Main.tile[i, j + 2].TileType == 165)
							{
								var tile2 = Main.tile[i, j + 2];
								tile2.TileColor = tilePaintColor;
							}
						}
					}
					if (tile.WallType == 187 || tile.WallType == 216)
					{
						tile.WallColor = tilePaintColor;
					}
				}
			}
		}

		private static void PaintThePurityGrass(byte tilePaintColor, byte wallPaintColor)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile)
					{
						if (tile.TileType == 2)
						{
							tile.TileColor = tilePaintColor;
						}
						else if (tile.TileType == 185 || tile.TileType == 186 || tile.TileType == 187)
						{
							Tile tile2 = tile;
							int num = j;
							while (num < Main.maxTilesY - 20 && (tile2.TileType == 185 || tile2.TileType == 186 || tile2.TileType == 187 || tile2.TileType == 3 || tile2.TileType == 73))
							{
								tile2 = Main.tile[i, ++num];
							}
							if (tile2.TileType == 2)
							{
								tile.TileColor = tilePaintColor;
							}
						}
					}
					if (tile.WallType == 66 || tile.WallType == 63)
					{
						tile.WallColor = wallPaintColor;
					}
				}
			}
		}

		private static void PaintThePyramids(byte tilePaintColor, byte wallPaintColor)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile && tile.TileType == 151)
					{
						tile.TileColor = tilePaintColor;
					}
					if (tile.WallType == 34)
					{
						tile.WallColor = wallPaintColor;
					}
				}
			}
		}

		private static void PaintTheTemple(byte tilePaintColor, byte wallPaintColor)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile)
					{
						bool flag = false;
						if (tile.TileType == 226)
						{
							flag = true;
						}
						if (tile.TileType == 137)
						{
							int num = tile.TileFrameY / 18;
							if (num >= 1 && num <= 4)
							{
								flag = true;
							}
						}
						if (flag)
						{
							tile.TileColor = tilePaintColor;
						}
					}
					if (tile.WallType == 87)
					{
						tile.WallColor = wallPaintColor;
					}
				}
			}
		}

		private static void PaintTheClouds(byte tilePaintColor, byte wallPaintColor)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile && (tile.TileType == 189 || tile.TileType == 196 || tile.TileType == 460))
					{
						tile.TileColor = tilePaintColor;
					}
					if (tile.WallType == 73)
					{
						tile.WallColor = wallPaintColor;
					}
				}
			}
		}

		private static void PaintTheDungeon(byte tilePaintColor, byte wallPaintColor)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile)
					{
						if (Main.tileDungeon[tile.TileType] || TileID.Sets.CrackedBricks[tile.TileType])
						{
							tile.TileColor = tilePaintColor;
						}
						if (tile.TileType == 19)
						{
							int num = tile.TileFrameY / 18;
							if (num >= 6 && num <= 12)
							{
								tile.TileColor = tilePaintColor;
							}
						}
					}
					if (Main.wallDungeon[tile.WallType])
					{
						tile.WallColor = wallPaintColor;
					}
				}
			}
		}

		private static void PaintTheLivingTrees(byte livingTreePaintColor, byte livingTreeWallPaintColor)
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.HasTile)
					{
						if (tile.WallType == 244)
						{
							tile.TileColor = livingTreePaintColor;
						}
						else if (tile.TileType == 192 || tile.TileType == 191)
						{
							tile.TileColor = livingTreePaintColor;
						}
						else if (tile.TileType == 52 || tile.TileType == 382)
						{
							int x = i;
							int y = j;
							GetVineTop(i, j, out x, out y);
							if (Main.tile[x, y].TileType == 192)
							{
								tile.TileColor = livingTreePaintColor;
							}
						}
						else if (tile.TileType == 187)
						{
							Tile tile2 = tile;
							int num = 0;
							while (tile2.TileType == 187)
							{
								num++;
								tile2 = Main.tile[i, j + num];
							}
							if (tile2.TileType == 192)
							{
								tile.TileColor = livingTreePaintColor;
							}
						}
					}
					if (tile.WallType == 244)
					{
						tile.WallColor = livingTreeWallPaintColor;
					}
				}
			}
		}

		private static void ConvertSkyIslands(int convertType, bool growTrees)
		{
			int num = 0;
			for (int i = 20; (double)i < Main.worldSurface; i++)
			{
				for (int j = 20; j < Main.maxTilesX - 20; j++)
				{
					Tile tile = Main.tile[j, i];
					if (tile.HasTile && TileID.Sets.Clouds[tile.TileType])
					{
						num = i;
						break;
					}
				}
			}
			for (int k = 20; k <= Main.maxTilesX - 20; k++)
			{
				for (int l = 20; l < num; l++)
				{
					Tile tile2 = Main.tile[k, l];
					Tile tile3 = Main.tile[k, l - 1];
					if (tile2.HasTile && tile2.TileType == 2)
					{
						if (tile3.TileType == 596 || tile3.TileType == 616)
						{
							WorldGen.KillTile(k, l - 1);
						}
						WorldGen.Convert(k, l, convertType, 1);
						ushort type = tile3.TileType;
						if ((uint)(type - 82) <= 1u || (uint)(type - 185) <= 2u || type == 227)
						{
							WorldGen.KillTile(k, l - 1);
						}
						if (growTrees && WorldGen._genRand.Next(3) == 0)
						{
							WorldGen.GrowTree(k, l);
						}
					}
				}
			}
		}

		private static void ImproveAllChestContents()
		{
			for (int i = 0; i < 8000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest == null)
				{
					continue;
				}
				for (int j = 0; j < 40; j++)
				{
					Item item = chest.item[j];
					if (item != null && !item.IsAir)
					{
						GiveItemGoodPrefixes(item);
					}
				}
			}
		}

		private static void GiveItemGoodPrefixes(Item item)
		{
			if (item.accessory)
			{
				PrefixItemFromOptions(item, TenthAnniversaryWorldInfo.GoodPrefixIdsForAccessory);
			}
			if (item.CountsAsClass(DamageClass.Melee))
			{
				PrefixItemFromOptions(item, TenthAnniversaryWorldInfo.GoodPrefixIdsForMeleeWeapon);
			}
			if (item.CountsAsClass(DamageClass.Ranged))
			{
				PrefixItemFromOptions(item, TenthAnniversaryWorldInfo.GoodPrefixIdsForRangedWeapon);
			}
			if (item.CountsAsClass(DamageClass.Magic))
			{
				PrefixItemFromOptions(item, TenthAnniversaryWorldInfo.GoodPrefixIdsForMagicWeapon);
			}
			if (item.CountsAsClass(DamageClass.Summon))
			{
				PrefixItemFromOptions(item, TenthAnniversaryWorldInfo.GoodPrefixIdsForSummonerWeapon);
			}
		}

		private static void PrefixItemFromOptions(Item item, int[] options)
		{
			int prefix = item.prefix;
			if (!item.Prefix(-3))
			{
				return;
			}
			List<int> list = new List<int>(options);
			while (list.Count > 0)
			{
				int index = WorldGen._genRand.Next(list.Count);
				int num = list[index];
				item.Prefix(num);
				if (item.prefix == num)
				{
					return;
				}
				list.RemoveAt(index);
			}
			item.Prefix(prefix);
		}

		private static void GetVineTop(int i, int j, out int x, out int y)
		{
			x = i;
			y = j;
			Tile tileSafely = Framing.GetTileSafely(x, y);
			if (TileID.Sets.IsVine[tileSafely.TileType])
			{
				while (y > 20 && tileSafely.HasTile && TileID.Sets.IsVine[tileSafely.TileType])
				{
					y--;
					tileSafely = Framing.GetTileSafely(x, y);
				}
			}
		}




	}
}
