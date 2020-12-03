using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000006 RID: 6
	public class JobGiver_WPFlick : ThinkNode_JobGiver
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002D24 File Offset: 0x00000F24
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
						Thing firstBuilding = c.GetFirstBuilding(pawn.Map);
						using (IEnumerator<Designation> enumerator = pawn.Map.designationManager.AllDesignationsOn(firstBuilding).GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								Designation designation = enumerator.Current;
								bool flag2 = designation.def == DesignationDefOf.Flick && pawn.CanReserve(firstBuilding, 1, -1, null, false);
								if (flag2)
								{
									return new Job(JobDefOf.Flick, firstBuilding);
								}
								return null;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}
	}
}
