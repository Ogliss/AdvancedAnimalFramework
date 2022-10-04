using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_WPDeconstruct : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			Region region = pawn.GetRegion(RegionType.Set_Passable);
			Job result;
			if (region == null) result = null;
			else
			{
				IntVec3 position = pawn.Position;
				Building firstBuilding = position.GetFirstBuilding(pawn.Map);
				List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
				for (int i = 0; i < allBuildingsColonist.Count; i++)
				{
					Building building = allBuildingsColonist[i];
					foreach (Designation designation in pawn.Map.designationManager.AllDesignationsOn(building))
					{
						if (building.Faction == pawn.Faction && designation.def == DesignationDefOf.Deconstruct && pawn.CanReserve(building, 1, -1, null, false))
						{
							return new Job(WPJobDefOf.WPDeconstruct, building);
						}
					}
				}
				result = null;
			}
			return result;
		}
	}
}
