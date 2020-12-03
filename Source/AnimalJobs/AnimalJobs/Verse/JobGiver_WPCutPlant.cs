using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000004 RID: 4
	public class JobGiver_WPCutPlant : ThinkNode_JobGiver
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002ACC File Offset: 0x00000CCC
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
				for (int i = 0; i < 500; i++)
				{
					IntVec3 randomCell = region.RandomCell;
					for (int j = 0; j < 4; j++)
					{
						IntVec3 c = randomCell + GenAdj.CardinalDirections[j];
						Plant plant = c.GetPlant(pawn.Map);
						using (IEnumerator<Designation> enumerator = pawn.Map.designationManager.AllDesignationsOn(plant).GetEnumerator())
						{
							if (enumerator.MoveNext())
							{
								Designation designation = enumerator.Current;
								bool flag2 = designation.def == DesignationDefOf.CutPlant && pawn.CanReserve(plant, 1, -1, null, false);
								if (flag2)
								{
									return new Job(JobDefOf.CutPlant, plant);
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
