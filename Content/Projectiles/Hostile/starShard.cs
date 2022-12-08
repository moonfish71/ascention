using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using ascention;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ascention.Core;
using static ascention.Core.AssetDirector;

namespace ascention.Content.Projectiles.Hostile
{
    public class starShard : ModProjectile
    {
        public override string Texture => PlaceholderTx;

        Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(PlaceholderTx);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Shard");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = Projectile.width;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.shouldFallThrough = true;
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
            Projectile.velocity.Y += 0.5f;
            ParticleManager.AddParticle(new starDust(Projectile.Center, Vector2.Zero));
        }

        public override void PostDraw(Color lightColor)
        {
            Rectangle source = new Rectangle(0, 0, (int)tex.Size().X, (int)tex.Size().Y / Main.projFrames[Projectile.type]);
            Vector2 drawPosition = Projectile.Center;
            Main.EntitySpriteDraw(tex, drawPosition - Main.screenPosition, source, Color.White * Projectile.Opacity, Projectile.rotation, new Vector2(tex.Size().X / 2, tex.Size().Y / 2), Projectile.scale, SpriteEffects.None, 1);
        }
    }
    public class starDust : Particle
    {
        private int lifeTimer = 0;
        private int MaxLifeTimer;

        public starDust(Vector2 pos, Vector2 vel) : base(pos, vel, 1f, PlaceholderTx)
        {
            MaxLifeTimer = 60;
            drawLighted = false;
        }

        public override void Update()
        {
            base.Update();
            lifeTimer++;

            if(lifeTimer > MaxLifeTimer)
                kill = true;
        }

        public override void Draw()
        {
            float alphMult = 1f;
            float alpha = 1f;

            if (lifeTimer < 100)
                alpha = lifeTimer / 100f;
            if (lifeTimer > MaxLifeTimer - 100)
                alpha = (MaxLifeTimer - lifeTimer) / 100f;

            scale = alpha;

            drawColor *= alpha * alphMult;
            drawPosition = Center;
            base.Draw();
        }
    }
}
