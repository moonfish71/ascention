using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace ascention.Content.Insight
{
    public class InsightPlayer : ModPlayer
    {
        public int maxInsight;
        public int insight = 0;
        public int insightState;
        public float maxQuotient;

        const int absMaxInsight = 1500;

        public float FMthreshold;
        public float dullThreshold;
        public float sharpThreshold;
        public float maddenedThreshold;
        public float insaneThreshold;

        public int healHurt;

        #region Shortcuts
        public const int feebleMinded = 0;
        public const int dull = 1;
        public const int average = 2;
        public const int sharp = 3;
        public const int maddened = 4;
        public const int insane = 5;
        #endregion

        public bool returnToBase;

        private int returnTimer = 0;

        public override void ResetEffects()
        {
            maxInsight = 1000;
            FMthreshold = 0.05f;
            dullThreshold = 0.15f;
            sharpThreshold = 0.4f;
            maddenedThreshold = 0.5f;
            insaneThreshold = 0.9f;
            returnToBase = true;

            healHurt = 0;

            maxQuotient = maxInsight/absMaxInsight;

            returnTimer++;

            if (insight > maxInsight)
            {
                insight = maxInsight;
            }
            else if(insight < 0)
            {
                insight = 0;
            }

            if (returnToBase && returnTimer > 1)
            {
                if(insight < (maxInsight * dullThreshold) + 50) 
                {
                    insight++;
                    returnTimer = 0;
                }
                else if (insight > (maxInsight * sharpThreshold) - 50 && insightState != insane)
                {
                    insight--;
                    returnTimer = 0;
                }
            }
        }

        public override void PostUpdate()
        {
            if (insight <= maxInsight * FMthreshold)
            {
                insightState = feebleMinded;
                Player.accRunSpeed *= 0.5f;
                Player.dpsDamage /= 2;
                Player.statDefense *= 2;
            }
            else if (insight <= maxInsight * dullThreshold)
            {
                insightState = dull;
                Player.accRunSpeed *= 0.7f;
                Player.dpsDamage *= 3 / 4;
                Player.statDefense *= 13 / 10;
            }
            else if (insight >= maxInsight * sharpThreshold)
            {
                insightState = sharp;
                Player.accRunSpeed *= 1.3f;
                Player.dpsDamage *= 5 / 4;
                Player.statDefense *= 7 / 10;
            }
            else if (insight >= maxInsight * maddenedThreshold)
            {
                insightState = maddened;
                Player.accRunSpeed *= 1.6f;
                Player.dpsDamage *= 6 / 4;
                Player.statDefense *= 5 / 10;
            }
            else if (insight >= maxInsight * insaneThreshold)
            {
                insightState = insane;
                Player.accRunSpeed *= 1.2f;
                Player.dpsDamage *= 6 / 4;
                Player.statDefense *= 2 / 10;
                healHurt = 16;
            }
            else
            {
                insightState = average;
            }
        }

        public override void clientClone(ModPlayer clientClone)
        {
            InsightPlayer clone = clientClone as InsightPlayer;
            clone.insight = insight;
            clone.maxInsight = maxInsight;
            clone.insightState = insightState;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)ascentionMessageType.InsightPlayerSyncPlayer);
            packet.Write((byte)Player.whoAmI);
            packet.Write(maxInsight);
            packet.Write(insight);
            packet.Write(insightState);
            packet.Send(toWho,fromWho);
        }
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            InsightPlayer clone = clientPlayer as InsightPlayer;
            if (clone.insight != insight)
            {
                var packet = Mod.GetPacket();
                packet.Write((byte)ascentionMessageType.InsightChanged);
                packet.Write((byte)Player.whoAmI);
                packet.Write(insight);
                packet.Send();
            }
            if(clone.insightState != insightState)
            {
                var packet = Mod.GetPacket();
                packet.Write((byte)ascentionMessageType.InsightStatusChanged);
                packet.Write(((byte)Player.whoAmI));
                packet.Write(insightState);
                packet.Send();
            }
            if (clone.maxInsight != maxInsight)
            {
                var packet = Mod.GetPacket();
                packet.Write((byte)ascentionMessageType.MaxInsightChanged);
                packet.Write(((byte)Player.whoAmI));
                packet.Write(maxInsight);
                packet.Send();
            }
        }

        public override void UpdateBadLifeRegen()
        {
            if(healHurt > 0)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= healHurt;
            }
        }

        public override void UpdateDead()
        {
            insight = (int)(maxInsight * 0.275f);
            returnTimer = 0;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["MaxInsight"] = maxInsight;
        }

        public override void LoadData(TagCompound tag)
        {
            maxInsight = tag.GetInt("MaxInsight");
        }
    }
}
