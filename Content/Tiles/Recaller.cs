using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TerrarianSky.Common.Systems;

namespace TerrarianSky.Content.Tiles
{
    internal class Recaller : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            DustType = DustID.Adamantite;
            AddMapEntry(Color.Navy);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);
        }

        public override bool RightClick(int i, int j)
        {
            Dust.QuickBox(new Vector2(i, j) * 16, new Vector2(i + 1, j + 1) * 16, 2, Color.YellowGreen, null);

            ModContent.GetInstance<RecallerUISystem>().ShowRecallerUI();

            return true;
        }

    }
}
