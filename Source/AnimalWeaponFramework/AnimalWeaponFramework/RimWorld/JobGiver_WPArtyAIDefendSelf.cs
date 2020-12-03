using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000004 RID: 4
	public class JobGiver_WPArtyAIDefendSelf : JobGiver_WPArtyAIDefendPawn
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000023B8 File Offset: 0x000005B8
		protected override Pawn GetDefendee(Pawn pawn)
		{
			return pawn;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000023CC File Offset: 0x000005CC
		protected override float GetFlagRadius(Pawn pawn)
		{
			return pawn.equipment.PrimaryEq.verbTracker.PrimaryVerb.verbProps.range;
		}
	}
}
