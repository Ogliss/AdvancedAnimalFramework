using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_WPRepair : ThinkNode_JobGiver
	{
		public IEnumerable<Thing> MinionsPotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildingsRepairable.RepairableBuildings(pawn.Faction);
		}

		public bool MinionsShouldSkip(Pawn pawn)
		{
			return pawn.Map.listerBuildingsRepairable.RepairableBuildings(pawn.Faction).Count == 0;
		}

		public override Job TryGiveJob(Pawn pawn)
		{
			Region region = pawn.GetRegion(RegionType.Set_Passable);
			Job result;
			if (region == null) result = null;
			else
			{
				IntVec3 position = pawn.Position;
				Building firstBuilding = position.GetFirstBuilding(pawn.Map);
				List<Thing> list = pawn.Map.listerBuildingsRepairable.RepairableBuildings(pawn.Faction);
				for (int i = 0; i < list.Count; i++)
				{
					Thing thing = list[i];
					if (thing.Faction == pawn.Faction && thing.def.useHitPoints && thing.HitPoints != thing.MaxHitPoints && thing.def.building.repairable && pawn.CanReserve(thing, 1, -1, null, false))
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
