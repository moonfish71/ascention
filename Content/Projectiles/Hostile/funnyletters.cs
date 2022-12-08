using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static ascention.Core.AssetDirector;

namespace ascention.Content.Projectiles.Hostile
{
    public class funnyletters : ModProjectile
    {
        public override string Texture => HostileProjectile + "funnyletters";

        Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(HostileProjectile + "funnyletters");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("funny words");
            Main.projFrames[Projectile.type] = 27;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = Projectile.width;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public float timer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        public float letter
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void AI()
        {
            timer++;
            Projectile.frame = (int)letter;
            Projectile.velocity.Y += 0.01f * (float)Math.Sin(timer / 20);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }
    }
}
