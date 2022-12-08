using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using static ascention.Core.AssetDirector;
using Terraria.ID;
using Terraria.ModLoader;
using ascention.Core;
using System.Collections.Generic;
using System;

namespace ascention.Content.Items
{
	public class Pleaidies : ModItem
	{
        public override string Texture => Melee + Name;

		public int timer;

		public int swings;

		public bool swingCounted;

        public override void SetStaticDefaults()
		{
			QuickItem.SetStatic(this, "Starcutter Scimitar", 1, "Swings 3 times, with the third swing being stronger than the rest\nRight-clicking on the third swing will cause you to fly into the air, and then slam into the ground");
		}

		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Melee;
			Item.width = 44;
			Item.height = 44;
			Item.useTime = 20;
			Item.useAnimation = Item.useTime;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			//Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Melee.PleaidiesHoldOut>();
			Item.shootSpeed = 0.75f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (!Item.channel && !player.channel)
			{
				swings++;
            }
            else
            {
				swings = 4;
            }

            switch (swings)
            {
				case 2:
					damage = (int)(damage * 1.2f);
					break;
				case 3:
					damage = (int)(damage * 1.5f);
					break;
            }

			int projectileID = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			Projectile blade = Main.projectile[projectileID];
			blade.ai[0] = Item.useTime;
			blade.ai[1] = swings;

			if (swings > 2) 
			{
				swings = 0;
			}
			return false;
        }

        public override bool? UseItem(Player player)
        {
			timer = 60;
			return base.UseItem(player);
        }

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 20;
				Item.useAnimation = 20;
				Item.damage = 40;
				Item.channel = true;
				Item.shootSpeed = 5f;
			}
			else
			{
				Item.useTime = 20;
				Item.useAnimation = 20;
				Item.damage = 50;
				Item.channel = false;
				Item.shootSpeed = 0.75f;
			}
			return base.CanUseItem(player);
		}

		public override bool AltFunctionUse(Player player)
		{
			if (swings == 2) 
			{
				return false;
			}

			return false;
		}

		public override void UpdateInventory(Player player)
        {
            if(timer-- <= 0)
            {
				swings = 0;
				timer = 0;
            }
        }

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			(recipe.createItem.ModItem as Pleaidies).timer = 0;
			(recipe.createItem.ModItem as Pleaidies).swings = 0;
			recipe.Register();
		}
	}
}