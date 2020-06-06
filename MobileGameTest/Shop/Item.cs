using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Graphics;
using MobileGameTest.Data;

namespace MobileGameTest.Shop
{
    public enum ItemType
    {
        Default,
        Skin,
        Item,
        DEFPotion,
        ATCKPotion,
        ProtPotion
    }

    public class Item
    {
        public int cost;
        public string description;
        public Action action;
        public ItemType type;
        public Texture2D txtr;
        public int id;
    }

    public static class ItemBase
    {
        public static Dictionary<int, Item> dict = new Dictionary<int, Item>
        {
            { 20, new Item(){cost = 300, description = "Рыбий король", type = ItemType.Skin, txtr = DataManager.Instance.Skins[1], id = 20} },
            { 1, new Item(){ cost = 50, description = "Зелье защиты:\n начни бой с 5 DEF", type = ItemType.DEFPotion, id = 1} },
            { 2, new Item(){ cost = 75, description = "Зелье охраны:\n первый удар нанесет на 3 меньше DMG", type = ItemType.ProtPotion, id = 2} },
            { 3,  new Item(){ cost = 100, description = "Зелье силы:\n первая атака нанесет на 3 больше DMG", type = ItemType.ATCKPotion, id = 3} }
        };

        public static Dictionary<int, ItemType> idToType = new Dictionary<int, ItemType>
        {
            { 20, ItemType.Skin },
            { 1, ItemType.DEFPotion },
            { 2, ItemType.ProtPotion },
            { 3, ItemType.ATCKPotion }
        };
    }
}