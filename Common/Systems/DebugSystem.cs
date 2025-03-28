using TerrarianSky.Common.Passes;
using TerrarianSky.Common.World;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

//namespace TerrarianSky.Common.Systems
//{
//    internal class DebugSystem : ModSystem
//    {

//        private List<GenPass> genPassList = [];
//        private World.World worldCopy;
//        private WorldManager worldManager = new();

//        public static bool JustPressed(Keys key)
//        {
//            return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
//        }
//        public override void PostUpdateWorld() {
//            if (JustPressed(Keys.D1))
//                worldManager.WriteWorlds();
//            //TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
//            //if (JustPressed(Keys.D2))
//            //    CreateWorldCopy();
//            //if (JustPressed(Keys.D3))
//            //    LoadWorldCopy();
//            //if (JustPressed(Keys.D4))
//            //    TestDesertification();
//            //if (JustPressed(Keys.D5))
//            //    GenerateWorld();
//            if (JustPressed(Keys.D6))
//                SaveWorldFile();
//            if (JustPressed(Keys.D7))
//                ReadWorldFile();
//            if (JustPressed(Keys.D2))
//                GenerateWorld();
//            if (JustPressed(Keys.D3))
//                CycleWorld();
//            if (JustPressed(Keys.D4))
//                LoadWorld("2");
//            if (JustPressed(Keys.D5))
//                Main.NewText(Main.worldPathName.ToString());
//            //if (JustPressed(Keys.D6))
//            //    LoadWorld("4");
//            //if (JustPressed(Keys.D7))
//            //    LoadWorld("5");
//            if (JustPressed(Keys.D8))
//                CreateWorldCopy();
//            if (JustPressed(Keys.D9))
//                LoadWorldCopy();
//        }

//        private void CreateWorldCopy()
//        {
//            if (worldCopy == null)
//                worldCopy = new();
//            else
//                worldCopy.SaveWorld();
//        }


//        private void LoadWorldCopy()
//        {
//            worldCopy.LoadWorld();
//        }

//        private void TestMethod(int x, int y) {
//            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);
//            // Code to test placed here:
//            Tile tile = Main.tile[x, y];
//            if (tile.HasTile && tile.TileType == TileID.Dirt)
//                tile.TileType = TileID.Sand;
//        }

//        private void TestDesertification()
//        {
//            Main.NewText("Starting Desert Conversion GenPass");
//            SandConversionGenPass desertification = new();
//            desertification.DoPass();
//            Main.NewText("Desert Conversion Completed");
//        }

//        private void GenerateWorld()
//        {

//            worldManager.MakeNewWorld();
//            //TerrainPassCopy pass = new();
//            //pass.DoPass();

//            //var configuration = WorldGenConfiguration.FromEmbeddedPath("Terraria.GameContent.WorldBuilding.Configuration.json");
//            //var progress = new GenerationProgress();
//            //Terraria.GameContent.Biomes.TerrainPass pass2 = new();
//            //pass2.Apply(progress, configuration.GetPassConfiguration(pass2.Name));

//        }

//        private void CycleWorld()
//        {
//            worldManager.CycleWorlds();
//        }

//        private void LoadWorld(string name)
//        {
//            worldManager.LoadWorld(name);
//        }

//        private void SaveWorldFile()
//        {

//            //Active world file data to create with new path so properties like worldpathname are usable. Then Check out WorldFile.SaveWorld(false)
//            if (worldCopy == null)
//            {
//                Main.NewText("No world save available");
//                return;
//            }
//            Main.NewText("Saving world to file");
//            worldCopy.WriteWorld();
//            Main.NewText("World file created");

//        }

//        private void ReadWorldFile()
//        {
//            if (worldCopy == null)
//            {
//                Main.NewText("No world save available");
//                return;
//            }
//            worldCopy = World.World.ReadWorld(worldCopy.worldName);
//            if (worldCopy == null)
//            {
//                Main.NewText("Wasn't able to read file");
//                return;
//            }
//            worldCopy.LoadWorld();
//        }

//    }
//}
