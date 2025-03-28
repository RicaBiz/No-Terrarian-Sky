using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Terraria;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace TerrarianSky.Common.World
{
    internal class WorldManager
    {
        public List<World> Worlds { get; set; } = [];
        public World CurrentWorld { get; set; }
        public List<string> CreatedWorldNames { get; set; } = [];
        private WorldGenerator WorldGenerator { get; }

        public WorldManager()
        {
            WorldGenerator = new WorldGenerator();
        }

        public void MakeNewWorld()
        {
            if (CreatedWorldNames.Count == 0)
            {
                CreatedWorldNames = ReadCreatedWorldNames();
            }
            var worldGenerationData = WorldGenerationData.RandomGenerationData();
            Main.NewText("Creating new world of " + worldGenerationData.Type + " type");

            SaveCurrentWorld();
            CurrentWorld = WorldGenerator.GenerateWorld((CreatedWorldNames.Count + 1).ToString(), worldGenerationData);
            
            Worlds.Add(CurrentWorld);
            CreatedWorldNames.Add(CurrentWorld.worldName);
            
			//Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld);
            Main.NewText(worldGenerationData.Type + " type world created");
        }

        public void LoadWorld(string worldName)
        {
            if (CreatedWorldNames.Count == 0)
            {
                CreatedWorldNames = ReadCreatedWorldNames();
            }
            Main.NewText("Loading World: " + worldName);

            //Check if World has ever been created
            if (!CreatedWorldNames.Contains(worldName))
            {
                Main.NewText("No world named " + worldName + " found");
                return;
            }
            SaveCurrentWorld();
            //Check if world is already in running world list
            if (!Worlds.Exists(world => world.worldName == worldName))
                Worlds.Add(World.ReadWorld(worldName));
            CurrentWorld = Worlds.Find(world => world.worldName == worldName);
            CurrentWorld.LoadWorld();

            Main.NewText("Loading of " + worldName + " completed");
        }

        public void CycleWorlds()
        {
            if (CreatedWorldNames.Count == 0)
            {
                CreatedWorldNames = ReadCreatedWorldNames();
            }
            if (CreatedWorldNames.Contains(Main.worldName))
            {
                var index = CreatedWorldNames.IndexOf(Main.worldName);
                index = index + 1 < CreatedWorldNames.Count ? index + 1 : 0;
                LoadWorld(CreatedWorldNames.ElementAt(index));
            }
        }

        private void SaveCurrentWorld()
        {
            if (CurrentWorld == null)
                CurrentWorld = new();
            else
                CurrentWorld.SaveWorld();
            if (!Worlds.Exists(world => world.worldName == CurrentWorld.worldName))
                Worlds.Add(CurrentWorld);
            if (!CreatedWorldNames.Contains(CurrentWorld.worldName))
                CreatedWorldNames.Add(CurrentWorld.worldName);
        }

        public void WriteWorlds()
        {
            foreach (World world in Worlds)
            {
                //to do: load world before writing to file. add save directory.
                world.WriteWorld();
            }
            SaveCreatedWorldNames();
        }
        
        private static readonly JsonSerializerOptions options = new() { WriteIndented = true, IncludeFields = true };
        private static readonly string namesFileName = "_CreatedWorldNames.json";
        private void SaveCreatedWorldNames()
        {
            if (Main.worldPathName == null)
                return;
            string savesPath = Main.worldPathName.Remove(Main.worldPathName.LastIndexOf('.'));

            //Write only the names that aren't written yet
            var writtenWorldNames = ReadCreatedWorldNames();
            List<string> namesToWrite = new(CreatedWorldNames);
            namesToWrite.RemoveAll(name => writtenWorldNames.Contains(name));

            string jsonString = JsonSerializer.Serialize(namesToWrite, options);
            File.WriteAllText(savesPath + namesFileName, jsonString);
        }

        public void UpdateCreatedWorldNames()
        {
            CreatedWorldNames = ReadCreatedWorldNames();
        }

        private List<string> ReadCreatedWorldNames()
        {
            if (Main.worldPathName == null)
                return [];
            string savesPath = Main.worldPathName.Remove(Main.worldPathName.LastIndexOf('.'));

            if (!File.Exists(savesPath + namesFileName))
                return [];
            string jsonString = File.ReadAllText(savesPath + namesFileName);
            return JsonSerializer.Deserialize<List<string>>(jsonString, options);
        }
    }
}
