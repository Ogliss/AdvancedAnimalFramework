using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_WPFlick : ThinkNode_JobGiver
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
                        if (c.InBounds(pawn.Map))
						{
							Thing firstBuilding = c.GetFirstBuilding(pawn.Map);
							using (IEnumerator<Designation> enumerator = pawn.Map.designationManager.AllDesignationsOn(firstBuilding).GetEnumerator())
							{
								if (enumerator.MoveNext())
								{
									Designation designation = enumerator.Current;
									if (designation.def == DesignationDefOf.Flick && pawn.CanReserve(firstBuilding, 1, -1, null, false))
									{
										return new Job(JobDefOf.Flick, firstBuilding);
									}
									return null;
								}
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
