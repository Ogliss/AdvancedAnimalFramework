using System;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000007 RID: 7
	public class JobGiver_WPMine : ThinkNode_JobGiver
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002E50 File Offset: 0x00001050
		protected override Job TryGiveJob(Pawn pawn)
		{
			Region region = pawn.GetRegion(RegionType.Set_Passable);
			bool flag = region == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < 40; i++)
				{
					IntVec3 randomCell = region.RandomCell;
					for (int j = 0; j < 4; j++)
					{
						IntVec3 c = randomCell + GenAdj.CardinalDirections[j];
						Building edifice = c.GetEdifice(pawn.Map);
						bool flag2 = edifice != null && edifice.def != ThingDefOf.CollapsedRocks && pawn.CanReserve(edifice, 1, -1, null, false);
						if (flag2)
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
