using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000003 RID: 3
	public class JobGiver_WPAIFightAnyone : JobGiver_AIDefendPawn
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002334 File Offset: 0x00000534
		protected override Pawn GetDefendee(Pawn pawn)
		{
			return pawn;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002348 File Offset: 0x00000548
		protected override float GetFlagRadius(Pawn pawn)
		{
			bool flag = pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps != null;
			float result;
			if (flag)
			{
				float num = pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range + 15f;
				result = num;
			}
			else
			{
				float num2 = 50f;
				result = num2;
			}
			return result;
		}
	}
}
