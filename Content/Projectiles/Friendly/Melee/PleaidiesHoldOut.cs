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
using ascention.Content.Insight;
using Terraria.Enums;

namespace ascention.Content.Projectiles.Friendly.Melee
{
    public class PleaidiesHoldOut : ModProjectile
    {
        public override string Texture => MeleeProjectile + Name;

		public SoundStyle swingSound;

		public bool playedSwing = false;

		public double maxAngle = Math.PI;

        public int startDelay;

		public const int FadeInDuration = 7;
		public const int FadeOutDuration = 4;

		public int spriteDir = 1;
		
		double dirIndex;

		double Rotation;

		Vector2 Offset = new Vector2(35, 0);

		public float CollisionWidth => 10f * Projectile.scale;

		public float Timer = 0;

		public float duration
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float swingStyle
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(44);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
			Projectile.idStaticNPCHitCooldown = 2;
            Projectile.tileCollide = false;
            Projectile.scale = 1.5f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.hide = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			//float clampedTimer = Timer / duration;

			Vector2 playerCenter = new Vector2(0);

			Timer += 1;

			if (swingStyle < 4)
			{
                switch (swingStyle)
                {
					case 1:
						maxAngle = Math.PI * 0.8;
						swingSound = SoundID.Item1;
						break;
					case 2:
						maxAngle = Math.PI;
						swingSound = SoundID.Item1;
						Projectile.scale = 2f;
						spriteDir = -1;
						break;
					case 3:
						maxAngle = Math.Tau;
						swingSound = new SoundStyle(ItemSound + "pleaidiesSwingBig");
						Projectile.scale = 3f;
						break;
                }

                if (!playedSwing)
                {
					SoundEngine.PlaySound(swingSound);
					playedSwing = true;
				}

				Rotation = (-maxAngle / 2 * Projectile.spriteDirection) + (Projectile.spriteDirection * maxAngle * Utils.GetLerpValue(0, duration, Timer, clamped: true));
				// Sin: maxAngle * Math.Sin(clampedTimer * Projectile.spriteDirection) + (maxAngle/2 * -Projectile.spriteDirection)
				// Sigmoid: maxAngle / (1 + Math.Exp(-(1/duration/10)*(duration * (clampedTimer - 0.5)))) - (maxAngle/2);

				
				if (Timer >= duration)
				{
					Projectile.Kill();
					return;
				}
				else
				{
					player.heldProj = Projectile.whoAmI;
				}

				if (Math.Sign(Projectile.spriteDirection) == -1)
				{
					dirIndex = 0;
				}
				else
				{
					dirIndex = Math.PI;
				}
			}
            else
            {
				duration = 180;
				Vector2 mouseDir = Main.MouseWorld;

				if (Math.Sign(Projectile.spriteDirection) == -1)
				{
					dirIndex = 0;
				}
				else
				{
					dirIndex = Math.PI;
				}

				if(Timer <= duration / 10 && player.velocity.Y == 0f)
                {
					Projectile.velocity = new Vector2(10 * (float)dirIndex, -20);
                }
                else
                {
					swingSound = new SoundStyle(ItemSound + "pleaidiesCrash");
					Projectile.velocity = new Vector2(0, ModMath.Magnitude(new Vector2(10)));
					if(player.velocity.Y == 0)
                    {
						SoundEngine.PlaySound(swingSound);
						for (int i = 0; i < 6; i++)
                        {
							var entitySource = Projectile.GetSource_FromAI();

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								int type = Projectile.NewProjectile(entitySource, Projectile.Center, new Vector2((-10 / 2) + (10 / 6 * i), -20), ModContent.ProjectileType<Hostile.starShard>(), Projectile.damage / 6, 1f);
								Projectile dart = Main.projectile[type];
								dart.friendly = true;
								dart.hostile = false;
							}
						}
						Projectile.Kill();
                    }
                }

				UpdatePlayer(player);
			}

			if (Projectile.owner == Main.myPlayer)
			{
				playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
				Projectile.Center = Offset.RotatedBy(Rotation + Projectile.velocity.ToRotation()) * Projectile.scale + playerCenter;

				Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt() * spriteDir;

				Projectile.rotation = (float)(Projectile.velocity.ToRotation() + Rotation + (Math.PI - MathHelper.PiOver4 * -Projectile.spriteDirection + dirIndex));
			}

			SetVisualOffsets();
		}

		private void SetVisualOffsets()
		{
			const int HalfSpriteWidth = 38 / 2;
			const int HalfSpriteHeight = 38 / 2;

			int HalfProjWidth = Projectile.width / 2;
			int HalfProjHeight = Projectile.height / 2;

			DrawOriginOffsetX = 0;
			DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
			DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
		}

		public override bool ShouldUpdatePosition()
		{
			return false;
		}

		private void UpdatePlayer(Player player)
		{
			// Multiplayer support here, only run this code if the client running it is the owner of the projectile
			if (Projectile.owner == Main.myPlayer)
			{
				Vector2 diff = Main.MouseWorld - player.Center;
				diff.Normalize();
				Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
				Projectile.netUpdate = true;
			}
			int dir = Projectile.direction;
			player.ChangeDir(dir); // Set player direction to where we are shooting
			player.heldProj = Projectile.whoAmI; // Update player's held projectile
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir);
			player.velocity = Projectile.velocity;
		}

		private void UpdateAim(Vector2 source, float speed, Player player)
		{
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
			if (aim.HasNaNs())
			{
				aim = -Vector2.UnitY;
			}

			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(player.velocity), aim, 1));
			aim *= speed;

			if (aim != player.velocity)
			{
				Projectile.netUpdate = true;
			}
			player.velocity = aim;
		}

		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Player player = Main.player[Projectile.owner];
			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);

			Vector2 start = playerCenter;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * ModMath.Magnitude(Projectile.Size * Projectile.scale);
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Player player = Main.player[Projectile.owner];
			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
			Vector2 normalVel = Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 0.75f;

			Vector2 start = playerCenter;
			Vector2 end = start + normalVel.RotatedBy(Rotation) * ModMath.Magnitude(Projectile.Size * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
		}
    }
}
