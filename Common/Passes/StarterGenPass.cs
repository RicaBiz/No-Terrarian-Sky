using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace TerrarianSky.Common.Passes
{
    internal class StarterGenPass : GenPass
    {
        public StarterGenPass() : base("Starter", 100f) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            
        }
    }
}
