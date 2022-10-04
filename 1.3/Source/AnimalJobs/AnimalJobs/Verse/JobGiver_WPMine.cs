using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_WPMine : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			Region region = pawn.GetRegion(RegionType.Set_Passable);
			Job result;
			if (region == null) result = null;
			else
			{
				for (int i = 0; i < 40; i++)
				{
					IntVec3 randomCell = region.RandomCell;
					for (int j = 0; j < 4; j++)
					{
						IntVec3 c = randomCell + GenAdj.CardinalDirections[j];
						Building edifice = c.GetEdifice(pawn.Map);
						if (edifice != null && edifice.def != ThingDefOf.CollapsedRocks && pawn.CanReserve(edifice, 1, -1, null, false))
						{
							return new Job(JobDefOf.Mine, edifice);
						}
					}
				}
				result = null;
			}
			return result;
		}
	}
}
