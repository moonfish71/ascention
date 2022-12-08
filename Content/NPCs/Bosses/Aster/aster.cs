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
using Terraria.Graphics.Effects;
using Terraria.GameContent.ItemDropRules;
using ascention.Core;
using static ascention.Core.AssetDirector;
using ascention.Content.Insight;
using System.IO;
using ascention.Content.Projectiles.Hostile;

namespace ascention.Content.NPCs.Bosses.Aster
{
    [AutoloadBossHead]
    public sealed partial class Aster : ModNPC
    {
        public override string Texture => AsterTex + Name;

        public SoundStyle AsterCrash = new SoundStyle(ItemSound + "pleaidiesCrash");

        public ref float timer => ref NPC.ai[0];
        public ref float phase => ref NPC.ai[1];
        public ref float attacktimer => ref NPC.ai[2];
        public ref float attackphase => ref NPC.ai[3];

        public bool phaseSwap = false;

        private bool timerOn = false;
        bool introSet = false;

        int randAttackIndex = 0;

        Vector2 phase2RingCenter = Vector2.Zero;
        float phase2RingRadius = 16 * 40;

        public float starRadiusMult = 1;
        bool starsSpawned = false;
        public float rotationVector = 0f;

        public Vector2 ZrotationVector = Vector2.Zero;

        List<float> targets = new List<float>();
        public void UpdatePlayers(Vector2 locus ,float radius = 2500)
        {
            if (Main.netMode != 1)
            {
                int originalCount = targets.Count;
                targets.Clear();
                for (int k = 0; k < 255; k++)
                {
                    if (Main.player[k].active && ModMath.Magnitude(locus - Main.player[k].Center) < radius)
                    {
                        targets.Add(k);
                    }
                }
                if (Main.netMode == 2 && targets.Count != originalCount)
                {
                    ModPacket netMessage = GetPacket(AsterMessageType.TargetList);
                    netMessage.Write(targets.Count);
                    foreach (int target in targets)
                    {
                        netMessage.Write(target);
                    }
                    netMessage.Send();
                }
            }
        }

        public override void SetStaticDefaults() => QuickNPC.SetStatic(this, "Fell Aster", 2, true);

        public override void SetDefaults()
        {
            NPC.lifeMax = 6000;
            NPC.damage = 80;
            NPC.defense = 5;
            NPC.width = 92;
            NPC.height = 96;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Meteor,
				new FlavorTextBestiaryInfoElement("A colonial organism infected by a corruptive infohazard. Once it's species' empire streched far across the stars, but now they have become a great plague that assimilates all who are unfortunate enough to meet them.")
            });;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public void GetRandomTarget()
        {
            NPC.target = Main.rand.Next(targets.Count);
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player player = Main.player[NPC.target];
            Vector2 delta = player.Center - NPC.Center;

            if (timerOn)
            {
                timer++;
            }

            if (Main.getGoodWorld)
            {
                NPC.scale = 0.6f;
            }

            Visuals();
            if (player.dead || Main.dayTime)
            {
                player.GetModPlayer<InsightPlayer>().returnToBase = true;
                NPC.velocity.Y -= 0.1f;
                NPC.EncourageDespawn(10);
                NPC.localAI[0] = 1;
                NPC.alpha -= (int)0.1;
                return;
            }
            else
            {
                //NPC.localAI[0] = 2;
                player.GetModPlayer<InsightPlayer>().returnToBase = false;

                if (NPC.localAI[0] <= 1)
                {
                    if (!introSet)
                    {
                        NPC.Center = player.Center + new Vector2(900, -900);
                        NPC.velocity = new Vector2(-15, 15);
                        NPC.noTileCollide = false;
                        NPC.dontTakeDamage = true;
                        NPC.damage = 300;
                        introSet = true;
                    }
                    if (!Main.dedServ)
                    {
                        Music = -1;
                    }
                    NPC.localAI[0] = 2;
                }
                else
                {
                    timer++;

                    if(phaseSwap == false)
                    {
                        switch (phase)
                        {
                            case 0:
                                NPC.dontTakeDamage = true;
                                if (!Main.dedServ)
                                {
                                    Music = -1;
                                }

                                SpawnStars();

                                Intro();
                                break;
                            case 1:
                                rotationRate = 0.2f;
                                UpdatePlayers(NPC.Center);
                                Phase1(player, delta);
                                break;
                            case 2:
                                NPC.dontTakeDamage = true;
                                if (!Main.dedServ)
                                {
                                    Music = -1;
                                }
                                PhaseTransition();
                                break;
                            case 3:
                                UpdatePlayers(phase2RingCenter, phase2RingRadius);
                                player.GetModPlayer<noGravPlayer>().noGravActive = true;
                                Phase2(player, delta);
                                break;
                        }
                    }
                    else
                    {
                        phaseSwap = false;
                        phase += 1;
                    }

                    if (parallax <= 0.9f || parallax >= 1.1f)
                    {
                        NPC.dontTakeDamage = true;
                        NPC.damage = 0;
                    }
                    else if(phase != 0 && phase != 2)
                    {
                        NPC.dontTakeDamage = false;
                        NPC.damage = 80;
                    }
                }
            }
            
        }

        bool attackswap = false;
        public void Phase1(Player player, Vector2 delta)
        {
            attacktimer++;


            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Revalation");
            }

            if (!attackswap)
            {
                switch (attackphase)
                {
                    case 0:
                        Circle(player);
                        break;
                    case 1:
                        Falling(player);
                        break;
                    case 2:
                        switch (randAttackIndex)
                        {
                            case 0:
                                Ritual1(player);
                                break;
                            case 1:
                                Gravity(player);
                                break;
                        }
                        break;
                    case 3:
                        Charges(player);
                        break;
                    case 4:
                        switch (randAttackIndex)
                        {
                            case 0:
                                Ritual1(player);
                                break;
                            case 1:
                                Gravity(player);
                                break;
                        }
                        break;
                }
            }
            else
            {
                if((attackphase == 1 || attackphase == 4) && ((Main.expertMode || Main.masterMode) | Main.getGoodWorld))
                {
                    NPC.netUpdate = true;
                    randAttackIndex = Main.rand.Next(2);
                    randAttackIndex = 1;
                }
                attackswap = false;
                attackphase += 1;
                if (attackphase > 4)
                {
                    attackphase = 0;
                }
            }

            if (NPC.life <= NPC.lifeMax * 0.6f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                phaseSwap = true;
                attackphase = 0;
                NPC.noTileCollide = false;
                phase2RingCenter = NPC.Center;
                NPC.velocity.Y = 5;
                NPC.netUpdate = true;
                Speak("Agh!");
            }
        }

        public void Phase2(Player player, Vector2 delta)
        {
            attacktimer++;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Madness");
            }

            if (!attackswap)
            {
                switch (attackphase)
                {
                    case 0:
                        Circle(player, 2);
                        break;
                    case 1:
                        Charges(player,2);
                        break;
                    case 2:
                        switch (randAttackIndex)
                        {
                            case 0:
                                Ritual1(player, 2);
                                break;
                            case 1:
                                Gravity(player, 2);
                                break;
                        }
                        break;
                }
            }
            else
            {
                if (attackphase == 2 && ((Main.expertMode || Main.masterMode) | Main.getGoodWorld))
                {
                    randAttackIndex = Main.rand.Next(2);
                }
                attackswap = false;
                attackphase += 1;
                if (attackphase > 2)
                {
                    attackphase = 0;
                }
            }

            drawRitualRing(phase2RingRadius, phase2RingCenter);
            Vector2 escapetracker = phase2RingCenter - player.Center;

            if (player.Distance(phase2RingCenter) > phase2RingRadius)
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
                player.Center = phase2RingCenter + new Vector2(phase2RingRadius, 0).RotatedBy(Math.Atan(escapetracker.Y / escapetracker.X) + dirIndex);
            }

            // Remove player gravity

            // Rapidly fly through rifts while firing stars

            // Falling attacks

            // Circle attack from phase one

            // Horizontal charges, but above the player and while raining falling stars

            // Falling attacks

            // Chanting again, but it's blasts of energy from the background

            // Loop
        }

        public override void OnKill()
        {
            Player player = Main.player[NPC.target];

            player.GetModPlayer<noGravPlayer>().noGravActive = false;
            base.OnKill();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A " + name;
        }

        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            Vector2 delta = NPC.Center - player.Center;

            if (delta.Y < -40 || phase == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SpawnStars()
        {
            if (starsSpawned)
            {
                return;
            }

            starsSpawned = true;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Console.Write("Stars spawned");

            int count = 5;
            var source = NPC.GetSource_FromAI();

            for (int i = 0; i < count; i++)
            {
                int index = Projectile.NewProjectile(source, NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Hostile.astralStarOrbiting>(), 0, 0);
                Projectile star = Main.projectile[index];

                if (star.ModProjectile is astralStarOrbiting Star)
                {
                    Star.baseNPC = NPC.whoAmI;
                    Star.rotationIndex = i;
                }

                if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: index);
                }
            }
        }

        private ModPacket GetPacket(AsterMessageType type)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)ascentionMessageType.Aster);
            packet.Write(NPC.whoAmI);
            packet.Write((byte)type);
            return packet;
        }

        public void HandlePacket(BinaryReader reader)
        {
            AsterMessageType type = (AsterMessageType)reader.ReadByte();
            if (type == AsterMessageType.TargetList)
            {
                int numTargets = reader.ReadInt32();
                targets.Clear();
                for (int k = 0; k < numTargets; k++)
                {
                    targets.Add(reader.ReadInt32());
                }
            }
        }
    }

    internal enum AsterMessageType : byte
    {
        TargetList
    }
}
