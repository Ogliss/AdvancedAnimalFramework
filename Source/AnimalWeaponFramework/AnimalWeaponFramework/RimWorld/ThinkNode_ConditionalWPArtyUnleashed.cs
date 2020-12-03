using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200000F RID: 15
	public class ThinkNode_ConditionalWPArtyUnleashed : ThinkNode_Conditional
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00003A8C File Offset: 0x00001C8C
		protected override bool Satisfied(Pawn pawn)
		{
			bool flag = !pawn.Spawned || pawn.Downed || pawn.Dead || pawn.Faction == null || pawn.playerSettings.Master == null || pawn.playerSettings == null || !pawn.playerSettings.RespectsMaster;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = pawn.playerSettings.Master.playerSettings.animalsReleased && pawn.training.HasLearned(TrainableDefOf.Release);
				result = flag2;
			}
			return result;
		}
	}
}
