using System;
using Verse;

namespace RimWorld
{
	// Token: 0x0200002A RID: 42
	public class WorkGiver_WPTendOther : WorkGiver_WPTend
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x00006230 File Offset: 0x00004430
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return base.HasJobOnThing(pawn, t, forced) && pawn != t;
		}
	}
}
