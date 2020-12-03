using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200001A RID: 26
	public class JobGiver_WPVisitSickPawn : JoyGiver
	{
		// Token: 0x0600006E RID: 110 RVA: 0x0000463C File Offset: 0x0000283C
		public override Job TryGiveJob(Pawn pawn)
		{
			bool flag = !InteractionUtility.CanInitiateInteraction(pawn, null);
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Pawn pawn2 = SickPawnVisitUtility.FindRandomSickPawn(pawn, JoyCategory.Low);
				bool flag2 = pawn2 == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = new Job(WPJobDefOf.WPVisitSickPawn, pawn2, SickPawnVisitUtility.FindChair(pawn, pawn2));
				}
			}
			return result;
		}
	}
}
