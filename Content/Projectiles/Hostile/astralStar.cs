using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static ascention.Core.AssetDirector;
using Terraria.ID;
using ascention.Core;

namespace ascention.Content.Projectiles.Hostile
{
    public class astralStar: ModProjectile
    {
        public override string Texture => HostileProjectile + "astralStar";

        Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(HostileProjectile + "astralStar");

        Texture2D speedTex = (Texture2D)ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StarWrath);

        int animTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Star");
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = Projectile.width;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.aiStyle = ProjAIStyleID.FallingStar;
        }

        public override void AI()
        {
            Projectile.Opacity = 255;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle speedSource = new Rectangle(0, 0, (int)speedTex.Size().X, (int)speedTex.Size().Y);

            float speedScale = (float)(2 / (1 + Math.Pow(Math.E, -ModMath.Magnitude(Projectile.velocity) / 4))) - 1;

            Vector2 drawPosition = Projectile.Center + new Vector2(0, 25 * Projectile.scale * speedScale).RotatedBy(Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * 1));

            Main.spriteBatch.Draw(speedTex, drawPosition - Main.screenPosition, speedSource, Color.White * (Projectile.Opacity / 16 * ModMath.Magnitude(Projectile.velocity / 2)), Projectile.velocity.ToRotation() + (MathHelper.PiOver2 * 3), speedTex.Size() / 2, (float)(Projectile.scale * 1f * speedScale + (0.1 * Math.Sin(animTimer / 7 * MathHelper.PiOver2))), SpriteEffects.None, 0f);

            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Rectangle source = new Rectangle(0, 0, 17, 17);

            source.Location = new Point(source.Location.X, (int)tex.Size().Y);
            Vector2 drawPosition = Projectile.Center;
        }
    }
}
