using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000005 RID: 5
	public class JobGiver_WPDeconstruct : ThinkNode_JobGiver
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002BFC File Offset: 0x00000DFC
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
				IntVec3 position = pawn.Position;
				Building firstBuilding = position.GetFirstBuilding(pawn.Map);
				List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
				for (int i = 0; i < allBuildingsColonist.Count; i++)
				{
					Building building = allBuildingsColonist[i];
					foreach (Designation designation in pawn.Map.designationManager.AllDesignationsOn(building))
					{
						bool flag2 = building.Faction == pawn.Faction && designation.def == DesignationDefOf.Deconstruct && pawn.CanReserve(building, 1, -1, null, false);
						if (flag2)
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
