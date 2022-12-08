//using Microsoft.Xna.Framework;
//using Terraria.DataStructures;
//using Terraria.GameContent.Creative;
//using Terraria.ID;
//using Terraria.ModLoader;
//using System;
//using Terraria;
//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using Terraria.GameContent;
//using Terraria.GameContent.UI.BigProgressBar;
//using ascention;
//using System.Collections.Generic;
//using Terraria.Audio;
//using Terraria.GameContent.Bestiary;
//using Terraria.GameContent.ItemDropRules;
//using ascention.Core;
//using static ascention.Core.AssetDirector;
//using ascention.Content.Insight;

//namespace ascention.Core
//{
//    public class FlawlessItem : GlobalNPC
//    {
//        public int normalItem = ItemID.Heart;
//        public int expertItem = ItemID.Heart;
//        public int trophy = ItemID.KingSlimeTrophy;

//        public int playerHit = 0;
//        public int oldHealth = 4000;

//        public override void SetDefaults(NPC npc)
//        {
//            switch (npc.type)
//            {
//                case NPCID.KingSlime:
//                    normalItem = ItemID.RoyalGel;
//                    trophy = ItemID.KingSlimeTrophy;
//                    break;
//                case NPCID.EyeofCthulhu:
//                    normalItem = ItemID.EoCShield;
//                    trophy = ItemID.EyeofCthulhuTrophy;
//                    break;
//                case NPCID.BrainofCthulhu:
//                    normalItem = ItemID.BrainOfConfusion;
//                    trophy = ItemID.BrainofCthulhuTrophy;
//                    break;
//                case NPCID.QueenBee:
//                    normalItem = ItemID.HiveBackpack;
//                    trophy = ItemID.QueenBeeTrophy;
//                    break;
//            }
//        }

//        public override void AI(NPC npc)
//        {
//            Player player = Main.player[npc.target];

//            if (npc.boss && player.statLife < oldHealth)
//            {
//                playerHit++;
//            }

//            if(oldHealth != player.statLife)
//            {
//                oldHealth = player.statLife;
//            }
//            base.AI(npc);
//        }

//        public override void ModifyNPCLoot(NPC npc,NPCLoot npcLoot)
//		{
//            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
//            LeadingConditionRule ExpertRule = new LeadingConditionRule(new Conditions.IsExpert());

//            if (npc.boss && playerHit == 0)
//            {
//                notExpertRule.OnSuccess(ItemDropRule.Common(normalItem));
//                ExpertRule.OnSuccess(ItemDropRule.Common(expertItem));
//                npcLoot.Add(ItemDropRule.Common(trophy));
//            }

//			npcLoot.Add(notExpertRule);
//            npcLoot.Add(ExpertRule);
//		}
//	}
//}
