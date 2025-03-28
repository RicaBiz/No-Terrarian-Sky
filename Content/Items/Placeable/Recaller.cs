using TerrarianSky.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrarianSky.Content.Items.Placeable
{
    internal class Recaller : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Recaller>(), 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddIngredient(ItemID.GoldOre, 5)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 15)
                .AddIngredient(ItemID.PlatinumOre, 5)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
