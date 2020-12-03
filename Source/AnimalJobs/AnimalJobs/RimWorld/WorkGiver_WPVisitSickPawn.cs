using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200002B RID: 43
	public class WorkGiver_WPVisitSickPawn : WorkGiver_Scanner
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00006260 File Offset: 0x00004460
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00006274 File Offset: 0x00004474
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006290 File Offset: 0x00004490
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !InteractionUtility.CanInitiateInteraction(pawn, null);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000062AC File Offset: 0x000044AC
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			return pawn2 != null && SickPawnVisitUtility.CanVisit(pawn, pawn2, JoyCategory.VeryLow);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000062D4 File Offset: 0x000044D4
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = (Pawn)t;
			return new Job(WPJobDefOf.WPVisitSickPawn, pawn2, SickPawnVisitUtility.FindChair(pawn, pawn2))
			{
				ignoreJoyTimeAssignment = true
			};
		}
	}
}
