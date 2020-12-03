using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000012 RID: 18
	public class ThinkNode_ConditionalWPNotFollowingMaster : ThinkNode_Conditional
	{
		// Token: 0x0600003D RID: 61 RVA: 0x00003D0C File Offset: 0x00001F0C
		protected override bool Satisfied(Pawn pawn)
		{
			return ThinkNode_ConditionalWPNotFollowingMaster.NotFollowingMaster(pawn);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003D24 File Offset: 0x00001F24
		public static bool NotFollowingMaster(Pawn pawn)
		{
			bool flag = !pawn.Spawned || pawn.playerSettings == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Pawn respectedMaster = pawn.playerSettings.RespectedMaster;
				bool flag2 = respectedMaster == null || !pawn.playerSettings.Master.Drafted;
				result = flag2;
			}
			return result;
		}
	}
}
