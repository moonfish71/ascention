using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static ascention.Core.AssetDirector;
using Terraria.ID;
using ascention.Core;
using Terraria.DataStructures;

namespace ascention.Content.Projectiles.Hostile
{
    public class madnessBubble : ascentionProjectile
    {
        public override string Texture => HostileProjectile + "madnessBubble";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Madness Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = Projectile.width;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.aiStyle = -1;
        }

        public ref float maxTimer => ref Projectile.ai[0];

        public ref float startParallax => ref Projectile.ai[1];

        public override void Init()
        {
            drawShaded = false;
            tex = (Texture2D)ModContent.Request<Texture2D>(HostileProjectile + "madnessBubble");
        }

        float timer = 0;

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {
            timer++;
            if(parallax >= 1f)
            {
                parallax += (1f - startParallax) / maxTimer * (timer / maxTimer);
            }
        }
    }
}
