using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ascention.Core;
using static ascention.Core.AssetDirector;
using ascention.Content.Particles.Satellite;
using Terraria.DataStructures;
using Terraria.ID;

namespace ascention.Content.Items.Testing
{
    public class OrbitRod : ModItem
    {
        public override string Texture => TestingItem + Name;

        public override void SetStaticDefaults()
        {
            QuickItem.SetStatic(this, "Orbit Rod", 1, "Item for testing Orbital Mechanics\nIf you see this in a playthrough, @ me lol");
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = 2;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 0.1f;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ParticleManager.AddParticle(new Stell(position, Vector2.One));
            return true;
        }
    }
}
