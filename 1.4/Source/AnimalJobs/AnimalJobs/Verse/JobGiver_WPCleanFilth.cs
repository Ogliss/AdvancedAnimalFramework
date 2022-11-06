using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_WPCleanFilth : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			Predicate<Thing> validator = (Thing t) => t.def.category == ThingCategory.Filth;
			Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Filth), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 10f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
			return thing == null ? null : new Job(JobDefOf.Clean, thing);
		}
	}
}
