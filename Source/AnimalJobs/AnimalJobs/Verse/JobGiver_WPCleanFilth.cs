using System;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000003 RID: 3
	public class JobGiver_WPCleanFilth : ThinkNode_JobGiver
	{
		// Token: 0x06000007 RID: 7 RVA: 0x00002A44 File Offset: 0x00000C44
		protected override Job TryGiveJob(Pawn pawn)
		{
			Predicate<Thing> validator = (Thing t) => t.def.category == ThingCategory.Filth;
			Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Filth), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 10f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
			bool flag = thing == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new Job(JobDefOf.Clean, thing);
			}
			return result;
		}
	}
}
