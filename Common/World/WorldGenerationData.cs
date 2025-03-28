using TerrarianSky.Common.Passes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace TerrarianSky.Common.World
{
    internal class WorldGenerationData
    {
        public string Type { get; private set; }
        public List<string> AbledPasses { get; private set; }
        public List<GenPass> GenPasses { get; private set; }
        public int PassIndex { get; private set; }
        public WorldGenerationData(string type, List<GenPass> genPasses, int passIndex, List<string> abledPasses)
        {
            Type = type;
            AbledPasses = abledPasses;
            GenPasses = genPasses;
            PassIndex = passIndex;
        }

        public static byte WorldTypeAmount = 1;

        public static WorldGenerationData DesertWorld
        { get {
                return new("DesertWorld", [new SandConversionGenPass()], 10,
                    ["Reset", "Terrain", "Dunes", "Ocean Sand", "Sand Patches", "Tunnels", "Small Holes", "Surface Caves", "Full Desert",
                    "Webs", "Gems", "Gravitating Sand", "Oasis", "Remove Water From Sand", "Smooth World", "Statues", "Buried Chests", "Surface Chests",
                    "Water Chests", "Quick Cleanup", "Pots", "Traps", "Piles", "Spawn Point", "Planting Trees", "Herbs", "Weeds", "Random Gems", "Larva",
                    "Settle Liquids Again", "Cactus, Palm Trees, & Coral", "Tile Cleanup", "Water Plants", "Stalac", "Remove Broken Traps", "Final Cleanup", "Sand Conversion"]);
            }
        }

        //public static WorldGenerationData DesertWorld { get {
        //        return new("DesertWorld", [new SandConversionGenPass("Sand Conversion", 100f)], 10,
        //            ["Terrain", "Dunes", "Ocean Sand", "Sand Patches", /*"Tunnels", "Small Holes", "Surface Caves", "Full Desert",
        //            "Webs", "Gems", "Gravitating Sand", "Oasis", "Remove Water From Sand", */"Smooth World", /*"Statues", "Buried Chests", "Surface Chests",
        //            "Water Chests", "Quick Cleanup", "Pots", "Traps", "Piles", "Spawn Point", "Planting Trees", "Herbs", "Weeds", "Random Gems", "Larva",
        //            "Settle Liquids Again", "Cactus, Palm Trees, & Coral", "Tile Cleanup", "Water Plants", "Stalac", "Remove Broken Traps", "Final Cleanup",*/ "Sand Conversion"]); 
        //} }

        public static WorldGenerationData VanillaWorld { get {
                return new("VanillaWorld", [new SandConversionGenPass()], 10,
                    ["Reset", "Terrain", "Dunes", "Ocean Sand", "Sand Patches", "Tunnels", "Small Holes", "Surface Caves", "Full Desert",
                    "Webs", "Gems", "Gravitating Sand", "Oasis", "Remove Water From Sand", "Smooth World", "Statues", "Buried Chests", "Surface Chests",
                    "Water Chests", "Quick Cleanup", "Pots", "Traps", "Piles", "Spawn Point", "Planting Trees", "Herbs", "Weeds", "Random Gems", "Larva",
                    "Settle Liquids Again", "Cactus, Palm Trees, & Coral", "Tile Cleanup", "Water Plants", "Stalac", "Remove Broken Traps", "Sand Conversion"]); 
        } }

        public static WorldGenerationData RandomGenerationData()
        {
            Random rand = new();
            return rand.Next(WorldGenerationData.WorldTypeAmount) switch
            {
                0 => DesertWorld,
                _ => VanillaWorld
            };
        }
    }

}
