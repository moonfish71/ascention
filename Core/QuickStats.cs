using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.GameContent.Creative;

namespace ascention.Core
{
    public class QuickItem
    {
        static readonly int[] empty = { 0};

        public static void SetStatic(ModItem i, string name, int sacrifices, string tooltip = "", bool isStaff = false)
        {
            i.DisplayName.SetDefault(name);
            i.Tooltip.SetDefault(tooltip);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[i.Item.type] = sacrifices;
            Item.staff[i.Item.type] = true;
        }

        public static void SetBlock(ModItem i, int w, int h, int tile, bool consumable = true, int placeStyle = -1, int rarity = ItemRarityID.White)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.createTile = tile;
            i.Item.useTurn = true;
            i.Item.autoReuse = true;
            i.Item.useAnimation = 15;
            i.Item.useTime = 10;
            i.Item.maxStack = 999;
            i.Item.useStyle = ItemUseStyleID.Swing;
            i.Item.consumable = consumable;
            i.Item.rare = rarity;

            if (placeStyle != -1)
                i.Item.placeStyle = placeStyle;
        }

        public static void SetWall(ModItem i, int w, int h, int wall, bool consumable = true)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.createWall = wall;
            i.Item.useTurn = true;
            i.Item.autoReuse = true;
            i.Item.useAnimation = 15;
            i.Item.useTime = 10;
            i.Item.maxStack = 999;
            i.Item.useStyle = ItemUseStyleID.Swing;
            i.Item.consumable = consumable;
        }

        public static void SetStaff(ModItem i, int w, int h, int shoot, int use, int damage, int mana, float speed = 5f, float kB = 2f, int rarity = 1)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.useAnimation = use;
            i.Item.useTime = use;
            i.Item.shootSpeed = speed;
            i.Item.knockBack = kB;
            i.Item.damage = damage;
            i.Item.shoot = shoot;
            i.Item.rare = rarity;
            i.Item.mana = mana;
            i.Item.noMelee = true;
            i.Item.useStyle = ItemUseStyleID.Swing;
            i.Item.DamageType = DamageClass.Magic;
        }

        public static void SetMinion(ModItem i, int w, int h, int shoot, int damage, int mana, int rarity = 1)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.useAnimation = 16;
            i.Item.useTime = 16;
            i.Item.shootSpeed = 0f;
            i.Item.knockBack = 1f;
            i.Item.damage = damage;
            i.Item.shoot = shoot;
            i.Item.rare = rarity;
            i.Item.mana = mana;
            i.Item.UseSound = SoundID.Item44;

            i.Item.noMelee = true;
            i.Item.useStyle = ItemUseStyleID.Swing;
            i.Item.DamageType = DamageClass.Summon;
        }

        public static void SetCritter(ModItem i, int w, int h, int npcType, int rarity = 0, int b = 5)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.useAnimation = 16;
            i.Item.useTime = 16;
            i.Item.damage = 0;
            i.Item.rare = rarity;
            i.Item.maxStack = 999;
            i.Item.noUseGraphic = true;
            i.Item.noMelee = false;
            i.Item.useStyle = ItemUseStyleID.Swing;
            i.Item.bait = b;
            i.Item.makeNPC = (short)npcType;
            i.Item.autoReuse = true;
            i.Item.consumable = true;
        }

        public static void SetMaterial(ModItem i, int w, int h, int rarity = 0, int maxStack = 999, bool consumable = false)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.useAnimation = 16;
            i.Item.useTime = 16;
            i.Item.damage = 0;
            i.Item.rare = rarity;
            i.Item.maxStack = maxStack;
            i.Item.noMelee = false;
            i.Item.useStyle = ItemUseStyleID.Swing;
            i.Item.autoReuse = true;
            i.Item.consumable = consumable;
        }

        public static void SetFishingRod(ModItem i, int w, int h, int fish, int shoot, float shootSpeed, int rarity = 0)
        {
            i.Item.width = w;
            i.Item.height = h;
            i.Item.useAnimation = 16;
            i.Item.useTime = 16;
            i.Item.damage = 0;
            i.Item.knockBack = 0f;
            i.Item.rare = rarity;
            i.Item.maxStack = 1;
            i.Item.shootSpeed = shootSpeed;
            i.Item.shoot = shoot;
            i.Item.noMelee = true;
            i.Item.fishingPole = fish;
            i.Item.autoReuse = true;
            i.Item.consumable = false;
        }

        public static void SetLightPet(ModItem i, int w, int h, int rarity = 0)
        {
            i.Item.CloneDefaults(ItemID.ShadowOrb);
            i.Item.width = w;
            i.Item.height = h;
            i.Item.rare = rarity;
        }
    }
    public class QuickNPC
    {
        public static void SetStatic(ModNPC n, string name, int frames = 1, bool boss = false)
        {
            n.DisplayName.SetDefault(name);
            Main.npcFrameCount[n.NPC.type] = frames;

            if (boss)
            {
                NPCID.Sets.BossBestiaryPriority.Add(n.NPC.type);
            }
            NPCID.Sets.MPAllowedEnemies[n.NPC.type] = true;
        }

        public static void SetDefaultsHostile(ModNPC n, int health = 25, int damage = 0, int defense = 0, int w = 1, int h = 1, float kbR = 0, bool noGrav = false, bool noCollide = false, int aiStyle = -1 )
        {
            n.NPC.lifeMax = health;
            n.NPC.damage = damage;
            n.NPC.defense = defense;
            n.NPC.width = w;
            n.NPC.height = h;
            n.NPC.knockBackResist = kbR;
            n.NPC.noGravity = noGrav;
            n.NPC.noTileCollide = noCollide;
            n.NPC.aiStyle = aiStyle;
        }
    }
    public class QuickProjectile
    {
        static readonly int[] empty = { 0 };

    }
    public class QuickTile
    {
        static readonly int[] empty = { 0 };

    }
}
