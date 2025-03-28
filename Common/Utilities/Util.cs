using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using static TerrarianSky.Common.World.World;

namespace TerrarianSky.Common.Utilities
{
    internal static class Util
    {

        public static ChestData CopyOfChestData(Chest chest)
        {
            return new(CopyOfList<Item>(chest.item, CopyOfItem), chest.eatingAnimationTime, chest.frame, chest.frameCounter,
                chest.name, chest.x, chest.y);
        }
        //public static List<Item> CopyOfItemList(ICollection<Item> items)
        //{
        //    var list = new List<Item>();
        //    foreach (Item item in items)
        //        list.Add(CopyOfItem(item));
        //    return list;
        //}
        public static List<T> CopyOfList<T>(ICollection list, Func<T, T> singleCopyMethod)
        {
            var copy = new List<T>();
            foreach (T node in list)
                copy.Add(singleCopyMethod(node));
            return copy;
        }

        public static Item CopyOfItem(Item item)
        {
            if (item == null) return null;
            Item copy = new(item.type, item.stack, item.prefix)
            {
                Hitbox = new Rectangle(item.getRect().X, item.getRect().Y, item.getRect().Width, item.getRect().Height)
            };
            return copy;
        }

        public static NPC CopyOfNPC(NPC npc)
        {
            if (npc == null) return null;
            NPC copy = new();
            copy.SetDefaults(npc.type);
            copy.active = npc.active;
            copy.Hitbox = new Rectangle(npc.getRect().X, npc.getRect().Y, npc.getRect().Width, npc.getRect().Height);
            return copy;
        }

        public static Projectile CopyOfProjectile(Projectile projectile)
        {
            if (projectile == null) return null;
            Projectile copy = new();
            copy.SetDefaults(projectile.type);
            copy.active = projectile.active;
            copy.direction = projectile.direction;
            copy.velocity = projectile.velocity;
            copy.Hitbox = new Rectangle(projectile.getRect().X, projectile.getRect().Y, projectile.getRect().Width, projectile.getRect().Height);
            return copy;
        }

        public static Rain CopyOfRain(Rain rain)
        {
            if (rain == null) return null;
            Rain copy = new()
            {
                active = rain.active,
                alpha = rain.alpha,
                position = new(rain.position.X, rain.position.Y),
                velocity = new(rain.velocity.X, rain.velocity.Y),
                rotation = rain.rotation,
                scale = rain.scale,
                type = rain.type,
                waterStyle = rain.waterStyle
            };
            return copy;
        }

        public static Sign CopyOfSign(Sign sign)
        {
            if (sign == null) return null;
            Sign copy = new()
            {
                text = sign.text,
                x = sign.x,
                y = sign.y
            };
            return copy;
        }

        public static void ReplaceTile(Tile tileTo, TileData tileFrom)
        {
            tileTo.TileType = tileFrom.TileTypeData.Type;
            tileTo.WallType = tileFrom.WallTypeData.Type;
            tileTo.Get<TileWallWireStateData>() = tileFrom.TileWallWireStateData;
            tileTo.Get<TileWallBrightnessInvisibilityData>() = tileFrom.TileWallBrightnessInvisibilityData;
            tileTo.Get<LiquidData>() = tileFrom.LiquidData;
        }

        public static void ReplaceChest(Chest chest, ChestData chestData)
        {
            chest.item = CopyOfList<Item>(chestData.Items, CopyOfItem).ToArray();
            chest.eatingAnimationTime = chestData.EatingAnimationTime;
            chest.frame = chestData.Frame;
            chest.frameCounter = chestData.FrameCounter;
            chest.name = chestData.Name;
            chest.x = chestData.X;
            chest.y = chestData.Y;
        }

        public static WorldFileData CreateMetadata(string name)
	    {
		    WorldFileData worldFileData = new WorldFileData("D:\\Documenti\\My Games\\Terraria\\tModLoader\\ModSources\\AllDesert\\Saves\\" + name, false);
		    if (Main.autoGenFileLocation != null && Main.autoGenFileLocation != "") {
			    worldFileData = new WorldFileData(Main.autoGenFileLocation, false);
			    Main.autoGenFileLocation = null;
		    }

		    worldFileData.Name = name;
		    worldFileData.GameMode = Main.GameMode;
		    worldFileData.CreationTime = DateTime.Now;
		    worldFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
		    worldFileData.SetFavorite(favorite: false);
		    worldFileData.WorldGeneratorVersion = 1198295875585uL;
		    worldFileData.UniqueId = Guid.NewGuid();
		    if (Main.DefaultSeed == "")
			    worldFileData.SetSeedToRandom();
		    else
			    worldFileData.SetSeed(Main.DefaultSeed);

		    return worldFileData;
	    }

    }
}
