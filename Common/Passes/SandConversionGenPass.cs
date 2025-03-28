using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria;

namespace TerrarianSky.Common.Passes
{
    internal class SandConversionGenPass : GenPass
    {
        public SandConversionGenPass() : base("Sand Conversion", 100f) { }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            //progress.Message = "Replacing dirt and stone with Desert";
            this.ReplaceDirtWithSand();
        }

        public void DoPass()
        {
            this.ReplaceDirtWithSand();
        }

        //Replace all terrain dirt with sand and all terrain stone with sandstone blocks and hardened sand blocks
        private void ReplaceDirtWithSand()
        {
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == TileID.Dirt)
                    {
                        tile.TileType = Main.rand.Next(100) switch
                        {
                            > 95 => TileID.HardenedSand,
                            < 4 => TileID.Sandstone,
                            _ => TileID.Sand
                        };
                    }
                    else if (tile.HasTile && tile.TileType == TileID.Stone)
                    {
                        tile.TileType =  TileID.Sandstone;
                    }
                }
            }
        }
    }
}
