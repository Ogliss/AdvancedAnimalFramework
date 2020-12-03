using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace Verse
{
	// Token: 0x02000009 RID: 9
	public class JobGiver_WPRepair : ThinkNode_JobGiver
	{
		// Token: 0x06000013 RID: 19 RVA: 0x000030AC File Offset: 0x000012AC
		public IEnumerable<Thing> MinionsPotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildingsRepairable.RepairableBuildings(pawn.Faction);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000030D4 File Offset: 0x000012D4
		public bool MinionsShouldSkip(Pawn pawn)
		{
			return pawn.Map.listerBuildingsRepairable.RepairableBuildings(pawn.Faction).Count == 0;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003104 File Offset: 0x00001304
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
				List<Thing> list = pawn.Map.listerBuildingsRepairable.RepairableBuildings(pawn.Faction);
				for (int i = 0; i < list.Count; i++)
				{
					Thing thing = list[i];
					bool flag2 = thing.Faction == pawn.Faction && thing.def.useHitPoints && thing.HitPoints != thing.MaxHitPoints && thing.def.building.repairable && pawn.CanReserve(thing, 1, -1, null, false);
					if (flag2)
					{
						return new Job(WPJobDefOf.WPRepair, thing);
					}
				}
				result = null;
			}
			return result;
		}
	}
}
