using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TerrarianSky.Common.World;

namespace TerrarianSky.Common.Systems
{
    internal class WorldManagerSystem : ModSystem
    {

        public static WorldManager worldManager = new();

        public override void OnWorldLoad()
        {
            worldManager.UpdateCreatedWorldNames();
        }
    }
}
