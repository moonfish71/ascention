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
using ascention.Core;
using static ascention.Core.AssetDirector;
using Terraria.Localization;
using Terraria.Chat;

namespace ascention.Content.NPCs.Bosses.Aster
{
    public sealed partial class Aster : ModNPC
    {
        int loopCounter = 0;
        int phraseUsed = 0;
        bool attackLock = false;
        bool locationSet = false;
        bool chatPhraseSaid = false;
        Vector2 ringCenter;

        #region Phrases
        bool phraseWritten = false;

        enum Letters {_, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z}
        

        float[,] phrase = new float[11,12] {
            {(float)Letters.E, (float)Letters.T, (float)Letters.E, (float)Letters.R, (float)Letters.N, (float)Letters.A, (float)Letters.L, 0, 0, 0, 0, 0} ,
            {(float)Letters.A, (float)Letters.S, (float)Letters.T, (float)Letters.E, (float)Letters.R, 0, 0, 0, 0, 0, 0, 0} ,
            {(float)Letters.T, (float)Letters.H, (float)Letters.E, 0, (float)Letters.S, (float)Letters.T, (float)Letters.A, (float)Letters.R, (float)Letters.S, 0, 0, 0},
            {(float)Letters.A, (float)Letters.S, (float)Letters.C, (float)Letters.E, (float)Letters.N, (float)Letters.D, 0, 0, 0, 0, 0, 0},
            {(float)Letters.B, (float)Letters.E, (float)Letters.H, (float)Letters.O, (float)Letters.L, (float)Letters.D, 0, 0, 0, 0, 0 ,0},
            {(float)Letters.R, (float)Letters.I, (float)Letters.S, (float)Letters.E, 0, 0 ,0, 0, 0, 0, 0, 0} ,
            {(float)Letters.A, (float)Letters.L, (float)Letters.T, (float)Letters.A, (float)Letters.E, 0, 0, 0, 0, 0, 0, 0},
            {(float)Letters.K, (float)Letters.I, (float)Letters.L, (float)Letters.L , 0 ,(float)Letters.O, (float)Letters.L, (float)Letters.O, (float)Letters.T, (float)Letters.H, (float)Letters.O, (float)Letters.N },
            {(float)Letters.L, (float)Letters.E, (float)Letters.T, 0, (float)Letters.M, (float)Letters.E, 0, (float)Letters.I, (float)Letters.N, 0, 0, 0} ,
            {(float)Letters.I, (float)Letters.N, (float)Letters.F, (float)Letters.O, (float)Letters.H, (float)Letters.A, (float)Letters.Z, (float)Letters.A, (float)Letters.R, (float)Letters.D, 0, 0},
            {(float)Letters.A, (float)Letters.L, (float)Letters.L, 0, (float)Letters.I ,(float)Letters.N, (float)Letters.T, (float)Letters.O, 0, (float)Letters.O, (float)Letters.N, (float)Letters.E }
        }; //Lore moment
        #endregion

        #region Aster Voices
        public SoundStyle AsterLine1 = new SoundStyle(AsterVoice + "thesky");
        public SoundStyle AsterLine2 = new SoundStyle(AsterVoice + "refrence");
        public SoundStyle AsterLine3 = new SoundStyle(AsterVoice + "no");
        public SoundStyle AsterLine4 = new SoundStyle(AsterVoice + "issues");
        public SoundStyle AsterLine5 = new SoundStyle(AsterVoice + "ignorant");
        #endregion

        #region helpers
        public void Shoot(Vector2 source, Vector2 target, float speed, int index, int damage, float ai0 = 0, float ai1 = 0, float localAI0 = 0, float localAI1 = 0)
        {
            Vector2 delta = source - target;
            var entitySource = NPC.GetSource_FromAI();
            

            double dirIndex;
            if (delta.X > 0)
            {
                dirIndex = 0;
            }
            else
            {
                dirIndex = Math.PI;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = Projectile.NewProjectile(entitySource, source, -new Vector2(speed, 0).RotatedBy(Math.Atan(delta.Y / delta.X) + dirIndex), index, damage / 2, 1f);
                Projectile dart = Main.projectile[type];
                if(index == ProjectileID.FallingStar)
                {
                    dart.tileCollide = false;
                }
                dart.localAI[0] = localAI0;
                dart.localAI[1] = localAI1;
                dart.ai[0] = ai0;
                dart.ai[1] = ai1;
                dart.friendly = false;
                dart.hostile = true;
            }
        }

        public void Shoot(Vector2 source, float x, float y, int index, int damage)
        {
            var entitySource = NPC.GetSource_FromAI();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int type = Projectile.NewProjectile(entitySource, source, new Vector2(x,y), index, damage / 2, 1f);
                Projectile dart = Main.projectile[type];
                dart.tileCollide = false;
                dart.friendly = false;
                dart.hostile = true;
            }
        }


        int launchDelay = 0; 
        float letterID = 0;
        int letterSelector = 0;

        public void ShootWords(Vector2 source, Vector2 target, int damage)
        {
            Vector2 delta = source - target;
            var entitySource = NPC.GetSource_FromAI();
            double dirIndex;
            if (Math.Sign(delta.X) == 1)
            {
                dirIndex = 0;
            }
            else
            {
                dirIndex = Math.PI;
            }

            if (!phraseWritten)
            {
                NPC.netUpdate = true;
                phraseUsed = Main.rand.Next(0, 10);
                phraseWritten = true;
                NPC.netUpdate = false;
            }

            if (letterSelector < 12)
            {
                if(launchDelay > 10)
                {
                    if(dirIndex == 0)
                    {
                        letterID = phrase[phraseUsed, letterSelector];
                    }
                    else
                    {
                        letterID = phrase[phraseUsed, 11 - letterSelector];
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && letterID != 0)
                    {
                        int type = Projectile.NewProjectile(entitySource, source, -new Vector2(10, 0).RotatedBy(Math.Atan(delta.Y / delta.X) + dirIndex), ModContent.ProjectileType<Projectiles.Hostile.funnyletters>(), damage / 2, 1f);
                        Projectile letter = Main.projectile[type];
                        letter.localAI[1] = letterID;
                    }
                    launchDelay = 0;
                    letterSelector++;
                }
                else
                {
                    launchDelay++;
                }
            }
            else
            {
                attackLock = false;
                phraseWritten = false;
                letterSelector = 0;
            }
            
        }

        public void Move(float speed, float inertia, Vector2 destination)
        {
            Vector2 delta = destination - NPC.Center;

            Vector2 moveTo = delta.SafeNormalize(Vector2.Zero) * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
        }

        public void Move(float speed, Vector2 moveTarget)
        {
            Vector2 delta = moveTarget - NPC.Center;
            Vector2 move = delta;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            NPC.velocity = move;
        }
        public void drawRitualRing(float radius, Vector2 center)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(center + new Vector2(radius, 0).RotatedBy(Main.rand.NextFloat(0, (float)Math.Tau)), 10, 10, DustID.YellowStarDust, 0, 0, 0, Color.Magenta);
            }
        }

        private void playRandomVoiceline()
        {
            NPC.netUpdate = true;
            int seed = Main.rand.Next(0, 5);
            NPC.netUpdate = false;
            switch (seed)
            {
                case 0:
                    SoundEngine.PlaySound(AsterLine1);
                    break;
                case 1:
                    SoundEngine.PlaySound(AsterLine2);
                    break;
                case 2:
                    SoundEngine.PlaySound(AsterLine3);
                    break;
                case 3:
                    SoundEngine.PlaySound(AsterLine4);
                    break;
                case 4:
                    SoundEngine.PlaySound(AsterLine5);
                    break;
            }
        }
        #endregion

        #region Attacks

        public void Circle(Player player, float phase = 1, bool Trid = false)
        {
            int attackFreq = 120;
            if(phase == 1)
            {
                if (Main.expertMode)
                {
                    attackFreq = 90;
                }
                if (Main.getGoodWorld)
                {
                    attackFreq = 45;
                }

                Vector2 moveTarget = player.Center + new Vector2(0,400).RotatedBy(timer/30);
                if (loopCounter <= 3)
                {
                    Move(20f,20f, moveTarget);
                    if (attacktimer >= attackFreq)
                    {
                        for (int i = 0; i < 5; i++) 
                        {
                            Shoot(drawPosition + ModMath.randomArea(NPC.width, NPC.height), player.Center + ModMath.randomArea(NPC.width * 2, NPC.height * 2), 10, ModContent.ProjectileType<Projectiles.Hostile.astralStar>(), NPC.damage / 2);
                        }
                        playRandomVoiceline();
                        attacktimer = 0;
                    }
                    if (timer / 30 >= Math.PI * 2)
                    {
                        timer = 0;
                        loopCounter++;
                    }
                }
                else
                {
                    if(timer / 60 >= 1)
                    {
                        attacktimer = 0;
                        timer = 0;
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(-3, 3));
                        }
                        attackswap = true;
                        loopCounter = 0;
                    }
                }
            }
            else
            {
                if (Main.expertMode)
                {
                    attackFreq = 70;
                }
                if (Main.getGoodWorld)
                {
                    attackFreq = 35;
                }

                Vector2 moveTarget = phase2RingCenter + new Vector2(0, phase2RingRadius - 100).RotatedBy(timer / 15);
                if (loopCounter <= 6)
                {
                    Move(50f, moveTarget);

                    if (attacktimer >= attackFreq)
                    {
                        Shoot(drawPosition + ModMath.randomArea(NPC.width, NPC.height), player.Center, 10, ModContent.ProjectileType<Projectiles.Hostile.astralStar>(), NPC.damage / 2);
                        playRandomVoiceline();
                        if(attacktimer >= attackFreq + 30)
                        {
                            attacktimer = 0;
                        }
                    }
                    if (timer / 15 >= Math.PI * 2)
                    {
                        timer = 0;
                        loopCounter++;
                    }
                }
                else
                {
                    if (timer / 60 >= 1)
                    {
                        attacktimer = 0;
                        timer = 0;
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                        }
                        attackswap = true;
                        loopCounter = 0;
                    }
                }
            }
        }

        public void Falling(Player player, float phase = 1)
        {
            int fallDelay = 90;
            int starCount = 6;
            float starSpread = 35;
            if (phase == 1)
            {
                if (Main.expertMode)
                {
                    starCount = 10;
                    starSpread = 40;
                    fallDelay = 60;
                }
                if (Main.getGoodWorld)
                {
                    starCount = 24;
                    starSpread = 50;
                    fallDelay = 40;
                }
                
                if (loopCounter <= 5)
                {
                    if (timer < fallDelay)
                    {
                        if (timer < fallDelay * 0.8f)
                        {
                            NPC.Center = player.Center + new Vector2(0, -700);
                        }
                        NPC.alpha = 0;
                        NPC.velocity = Vector2.One;
                        NPC.noTileCollide = true;
                    }

                    if (NPC.velocity != Vector2.Zero)
                    {
                        float speed = 20f;
                        NPC.velocity = new Vector2(0, speed);
                        NPC.noTileCollide = false;
                    }
                    else
                    {
                        SoundEngine.PlaySound(AsterCrash);
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                        for (int i = 0; i < starCount; i++)
                        {
                            Shoot(NPC.Center, (-starSpread / 2) + (starSpread / starCount * i), -20, ModContent.ProjectileType<Projectiles.Hostile.starShard>(), NPC.damage);
                            NPC.alpha += 255 / starCount;
                        }
                        playRandomVoiceline();
                        attacktimer++;
                        if (attacktimer > 30)
                        {
                            attacktimer = 0;
                            timer = 0;
                            NPC.alpha += 255 / starCount;

                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(-3, 0) - 5);
                            }
                            loopCounter++;
                        }
                    }
                }
                else
                {
                    NPC.alpha = 0;
                    
                    if (timer == 20)
                    {
                        for (int i = 0; i < starCount + 2; i++)
                        {
                            Shoot(NPC.Center, (-starSpread / 2) + (starSpread / (starCount + 2) * i), -20, ModContent.ProjectileType<Projectiles.Hostile.starShard>(), NPC.damage);

                        }
                        playRandomVoiceline();
                    }

                    if (timer / 60 >= 5)
                    {
                        attacktimer = 0;
                        timer = 0;
                        attackswap = true;
                        loopCounter = 0;
                        NPC.noTileCollide = true;
                    }
                    else if(timer / 60 >= 3)
                    {
                        Dust.NewDust((NPC.Center - new Vector2(NPC.width / 2, NPC.height / 2)), NPC.width / 2, NPC.height / 2, DustID.YellowStarDust, Main.rand.Next(-3, 3), Main.rand.Next(-3, 3), 0, Color.DarkMagenta);
                    }
                    else
                    {
                        Dust.NewDust(drawPosition, NPC.width / 2, NPC.height / 2, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(-10, 10), 0, Color.Magenta);
                        playRandomVoiceline();
                    }
                }
            }
        }

        public void Ritual1(Player player, float phase = 1)
        {
            float radius = 300 + (targets.Count * 50);
            NPC.velocity = Vector2.Zero;
            if (phase == 2)
            {
                parallax = 0.9f;
            }

            if(loopCounter < 6)
            {
                if (attackLock)
                {
                    ShootWords(drawPosition, player.Center, NPC.damage / 2);
                }
                else
                {
                    attacktimer++;
                }

                if (!locationSet)
                {
                    ringCenter = player.Center;
                    locationSet = true;
                }
                else
                {
                    if (phase == 1)
                    {
                        drawRitualRing(radius, ringCenter);
                        Vector2 escapetracker = ringCenter - player.Center;

                        if (player.Distance(ringCenter) > radius)
                        {
                            double dirIndex;
                            if (escapetracker.X < 0)
                            {
                                dirIndex = 0;
                            }
                            else
                            {
                                dirIndex = Math.PI;
                            }
                            player.Center = ringCenter + new Vector2(radius, 0).RotatedBy(Math.Atan(escapetracker.Y / escapetracker.X) + dirIndex);
                        }
                    }

                }

                if(attacktimer > 10 && !attackLock)
                {
                    attackLock = true;
                    foreach(float target in targets)
                    {
                        Player sidePlayer = Main.player[(int)target];

                        if (sidePlayer != player)
                        {
                            Shoot(drawPosition, sidePlayer.Center, 15f, ModContent.ProjectileType<Projectiles.Hostile.astralDart>(), NPC.damage / 6, drawPosition.X, drawPosition.Y, sidePlayer.Center.X * 3, sidePlayer.Center.Y * 3);
                        }
                    }
                    attacktimer = 0;
                    timer = 0;
                    loopCounter++;
                }
                else if(attacktimer == 6)
                {
                    NPC.netUpdate = true;
                    
                    if (phase == 1)
                    {
                        NPC.Center = ringCenter + new Vector2(radius + Main.rand.Next(20, 200), 0).RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi));
                    }
                    else
                    {
                        NPC.Center = phase2RingCenter + new Vector2(phase2RingRadius + Main.rand.Next(20, 200), 0).RotatedBy(Main.rand.NextFloat(0, MathHelper.TwoPi));
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(10, 10) - 5);
                    }
                }
            }
            else if (timer / 60 >= 2)
            {
                attacktimer = 0;
                timer = 0;
                attackswap = true;
                loopCounter = 0;
                locationSet = false;
            }
        }

        public void Gravity(Player player, int phase = 1)
        {
            Core.Systems.CameraSystem system = new Core.Systems.CameraSystem();

            rotationVector = (float)(Math.PI / 2);

            float radius = 700 + (targets.Count * 50);

            int inShots = 24;

            NPC.velocity = Vector2.Zero;

            int blastCount = 8;

            Vector2 ringPos;

            if(!chatPhraseSaid)
            {
                Speak("The sky beckons...");
                chatPhraseSaid = true;
            }

            attacktimer++;

            foreach (float targetID in targets)
            {
                Player target = Main.player[(int)targetID];

                float pull = .15f;

                Vector2 delta = target.Center - (NPC.Center - new Vector2(NPC.width / 2, NPC.height / 2));
                Vector2 grav = Vector2.Normalize(delta) * (pull + Utils.SmoothStep(0, 0.005f, timer / 1200 * 0.005f));

                if (loopCounter < inShots + 4)
                {
                    if (phase == 1)
                    {
                        drawRitualRing(radius, NPC.Center);
                        Vector2 escapetracker = NPC.Center - target.Center;

                        if (player.Distance(NPC.Center) > radius)
                        {
                            double dirIndex;
                            if (escapetracker.X < 0)
                            {
                                dirIndex = 0;
                            }
                            else
                            {
                                dirIndex = Math.PI;
                            }
                            target.Center = NPC.Center + new Vector2(radius, 0).RotatedBy(Math.Atan(escapetracker.Y / escapetracker.X) + dirIndex);
                        }

                        target.velocity.X -= grav.X;
                    }
                    else
                    {
                        NPC.Center = phase2RingCenter;
                        target.velocity -= grav;
                    }
                }
            }

            if (loopCounter < inShots + 4)
            {
                if (loopCounter < inShots)
                {
                    if (attacktimer > 45)
                    {
                        if (phase == 1)
                        {
                            for (int i = 0; i < blastCount; i++)
                            {
                                ringPos = drawPosition + new Vector2(radius, 0).RotatedBy((Math.Tau / blastCount * (i ^ 2)) + (Math.PI / 8 + loopCounter));
                                Shoot(ringPos, drawPosition, 10, ModContent.ProjectileType<Projectiles.Hostile.astralDart>(), NPC.damage / 4, ringPos.X, ringPos.Y, drawPosition.X, drawPosition.Y);
                            }
                        }
                        else
                        {
                            blastCount = 14;
                            for (int i = 0; i < blastCount; i++)
                            {
                                ringPos = drawPosition + new Vector2(phase2RingRadius, 0).RotatedBy((Math.Tau / blastCount * (i ^ 2)) + (Math.PI / 8 + loopCounter));
                                Shoot(ringPos, drawPosition, 10, ModContent.ProjectileType<Projectiles.Hostile.astralDart>(), NPC.damage / 4, ringPos.X, ringPos.Y, drawPosition.X, drawPosition.Y);
                            }
                        }
                        loopCounter++;
                        attacktimer = 0;
                    }
                }
                else if (loopCounter == inShots)
                {
                    if (attacktimer > 60)
                    {
                        attacktimer = 0;
                        loopCounter++;
                    }
                }
                else if (loopCounter < inShots + 3)
                {
                    if (attacktimer > 45)
                    {
                        if (phase == 1)
                        {
                            for (int i = 0; i < blastCount + 10; i++)
                            {
                                ringPos = drawPosition + new Vector2(radius, 0).RotatedBy((Math.Tau / (blastCount + 10) * (i ^ 2)) + (Math.PI / 8 + loopCounter));
                                Shoot(drawPosition, ringPos, 12, ModContent.ProjectileType<Projectiles.Hostile.astralDart>(), NPC.damage / 4, drawPosition.X, drawPosition.Y, ringPos.X * 3, ringPos.Y * 3);
                            }
                        }
                        else
                        {
                            blastCount = 14;
                            for (int i = 0; i < blastCount + 10; i++)
                            {
                                ringPos = drawPosition + new Vector2(phase2RingRadius, 0).RotatedBy((Math.Tau / (blastCount + 10) * (i ^ 2)) + (Math.PI / 8 + loopCounter));
                                Shoot(drawPosition, ringPos, 12, ModContent.ProjectileType<Projectiles.Hostile.astralDart>(), NPC.damage / 4, drawPosition.X, drawPosition.Y, ringPos.X * 3, ringPos.Y * 3);
                            }
                        }
                        loopCounter++;
                        attacktimer = 0;
                    }
                }
                else
                {
                    system.shake += 50;
                    loopCounter++;
                    timer = 0;
                }
            }
            else if(timer/60 >= 8)
            {
                //player.Hitbox = default;
                attacktimer = 0;
                timer = 0;
                attackswap = true;
                loopCounter = 0;
                locationSet = false;
                chatPhraseSaid = false;
            }

        }

        public void Charges(Player player, float phase = 1)
        {
            Vector2 delta = player.Center - (NPC.Center - new Vector2(NPC.width / 2, NPC.height / 2));

            float dirIndex;
            if (delta.X < 0)
            {
                dirIndex = 1;
            }
            else
            {
                dirIndex = -1;
            }

            if (loopCounter < 5)
            {
                attacktimer++;

                Vector2 moveTarget = player.Center + new Vector2(500 * dirIndex, 0);

                if (phase == 2)
                {
                    moveTarget = new Vector2(phase2RingCenter.X, player.Center.Y) + new Vector2(phase2RingRadius * dirIndex, 0);
                }

                if (attacktimer > 200)
                {
                    attackLock = true;
                    attacktimer = 0;
                    timer = 0;
                    loopCounter++;
                    rotationRate = 0.2f;
                }
                else if (attacktimer == 100)
                {
                    NPC.velocity = new Vector2(-25 * dirIndex, 0);
                }
                else if (attacktimer < 100)
                {
                    Move(15f, moveTarget);
                    rotationRate += 0.005f;
                }
            }
            else if (timer / 60 >= 1)
            {
                attacktimer = 0;
                loopCounter = 0;
                attackswap = true;
            }
            else
            {
                NPC.velocity *= 0.001f;
            }
        }

        public void StarRain(Player player, float phase = 1)
        {
            if (timer / 60 >= 1)
            {
                attacktimer = 0;
                attackswap = true;
            }
        }

        public void StarFury(Player player, float phase = 1)
        {
            if (timer / 60 >= 1)
            {
                attacktimer = 0;
                attackswap = true;
            }
        }
        #endregion

        #region Animations

        Vector2 basePos = Vector2.Zero;

        public void Intro()
        {
            attacktimer++;

            if (!Main.dedServ)
            {
                Music = -1;
            }

            if (!attackswap)
            {
                switch (attackphase)
                {
                    case 0:
                        if (NPC.velocity.Y == 0)
                        {
                            NPC.velocity.X = 0;
                            NPC.velocity.Y = -1;
                            rotationRate = 0;
                            basePos = NPC.Center;

                            SoundEngine.PlaySound(AsterCrash);
                            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
  
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(-3, 0) - 5);
                            }
                            
                            NPC.noTileCollide = true;
                            attacktimer = 0;
                            attackswap = true;
                        }
                        break;
                    case 1:

                        if (rotationRate < 0.3f)
                        {
                            rotationRate += 0.0005f;
                        }

                        if(attacktimer >= 300)
                        {
                            NPC.velocity = Vector2.Zero;
                            attacktimer = 0;
                            attackswap = true;
                            basePos = NPC.Center;
                            Speak("Well, we can't have that, can we?");
                        }
                        else if(attacktimer == 60)
                        {
                            Speak("Oh?");
                        }
                        else if(attacktimer == 160)
                        {
                            UpdatePlayers(NPC.Center);
                            if(targets.Count > 1)
                            {
                                Speak("More mortals that don't know the true nature of this world?");
                            }
                            else
                            {
                                Speak("Another mortal that dosen't know the true nature of this world?");
                            }
                        }
                        break;
                    case 2:

                        if (rotationRate < 0.4f)
                        {
                            rotationRate += 0.001f;
                        }

                        if(attacktimer / 15 >= Math.PI * 4)
                        {
                            NPC.velocity = Vector2.Zero;
                            attacktimer = 0;
                            attackswap = true;
                            Speak("Prepare for a lesson you'll never forget!");
                        }
                        else
                        {
                            NPC.Center = basePos + new Vector2(0, 20 * (float)Math.Sin(attacktimer / 30));
                        }
                        break;
                    case 3:

                        if (attacktimer < 2)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-20, 20), Main.rand.Next(-20, 20) - 5);
                            }
                        }
                        else if(attacktimer > 120)
                        {
                            NPC.velocity = Vector2.Zero;
                            attacktimer = 0;
                            attackswap = true;
                        }
                        break;
                }
            }
            else
            {
                attackswap = false;
                attackphase += 1;
                if (attackphase > 3)
                {
                    attackphase = 0;
                    phaseSwap = true;
                }
            }
        }

        float rad = 0;

        public void PhaseTransition()
        {
            attacktimer++;

            if (!Main.dedServ)
            {
                Music = -1;
            }

            if (!attackswap)
            {
                switch (attackphase)
                {
                    case 0:
                        NPC.velocity.X *= 0.99f;
                        NPC.noTileCollide = false;

                        NPC.netUpdate = true;

                        if (NPC.velocity.Y != 0)
                        {
                            if(attacktimer == 15 + Main.rand.Next(10))
                            {
                                glowActive = Main.rand.NextBool();
                                rotationRate = Main.rand.NextFloat(-0.1f, 0.1f);
                            }
                            else if(attacktimer > 30)
                            {
                                attacktimer = 0;
                            }
                        }
                        else
                        {
                            NPC.velocity = Vector2.Zero;
                            glowActive = true;
                            rotationRate = 0;
                            basePos = NPC.Center;

                            SoundEngine.PlaySound(AsterCrash);
                            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);

                            for (int i = 0; i < 20; i++)
                            {
                                Dust.NewDust(drawPosition, NPC.width, NPC.height, DustID.YellowStarDust, Main.rand.Next(-10, 10), Main.rand.Next(-3, 0) - 5);
                            }

                            NPC.noTileCollide = true;
                            attacktimer = 0;
                            timer = 0;
                            attackswap = true;
                        }
                        break;
                    case 1:
                        if(attacktimer > 40)
                        {
                            attacktimer = 0;
                            timer = 0;
                            attackswap = true;
                            basePos = new Vector2(NPC.Center.X, NPC.Center.Y + (NPC.height / 2) - phase2RingRadius);
                            NPC.velocity = new Vector2(0, -(NPC.Center.Y - basePos.Y) / 120);
                        }
                        break;
                    case 2:
                        if (rotationRate < 0.4f)
                        {
                            rotationRate += 0.001f;
                        }
                        if(attacktimer == 120)
                        {
                            NPC.velocity = Vector2.Zero;
                        }
                        else if (attacktimer > 160)
                        {
                            attacktimer = 0;
                            timer = 0;
                            phase2RingCenter = NPC.Center;
                            attackswap = true;
                        }
                        else if(attacktimer == 60)
                        {
                            Speak("Do you think this is a joke?");
                        }
                        break;
                    case 3:
                        parallax = MathHelper.Lerp(1f, 0.91f, rad / phase2RingRadius);
                        drawRitualRing(rad, phase2RingCenter);
                        if(rad < phase2RingRadius)
                        {
                            rad += 4;
                        }
                        else
                        {
                            attacktimer = 0;
                            timer = 0;
                            phase2RingCenter = NPC.Center;
                            attackswap = true;
                        }

                        break;
                }
            }
            else
            {
                attackswap = false;
                attackphase += 1;
                if (attackphase > 3)
                {
                    attackphase = 0;
                    phaseSwap = true;
                }
            }
        }

        private void Speak(string msg)
        {
            var speechType = ModContent.GetInstance<StuffOfLegendsServerConfig>().TalkTextSetting;
            if (speechType == StuffOfLegendsServerConfig.talkInteraction.World || speechType == StuffOfLegendsServerConfig.talkInteraction.Both)
            {
                int c = CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Center.Y, 60, 20), new Color(246, 121, 195), msg, true);
                CombatText combattext = Main.combatText[c];
            }
            if (speechType == StuffOfLegendsServerConfig.talkInteraction.Chat || speechType == StuffOfLegendsServerConfig.talkInteraction.Both)
            {
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(msg), new Color(246, 121, 195));
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(msg, new Color(246, 121, 195));
            }
        }

        #endregion
    }
}
