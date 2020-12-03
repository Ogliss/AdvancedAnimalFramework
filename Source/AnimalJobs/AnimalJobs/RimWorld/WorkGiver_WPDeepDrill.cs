using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000021 RID: 33
	public class WorkGiver_WPDeepDrill : WorkGiver_Scanner
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00005460 File Offset: 0x00003660
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ThingDefOf.DeepDrill);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000547C File Offset: 0x0000367C
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005490 File Offset: 0x00003690
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000054A4 File Offset: 0x000036A4
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.DeepDrill).Cast<Thing>();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000054D0 File Offset: 0x000036D0
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
			for (int i = 0; i < allBuildingsColonist.Count; i++)
			{
				bool flag = allBuildingsColonist[i].def == ThingDefOf.DeepDrill;
				if (flag)
				{
					CompPowerTrader comp = allBuildingsColonist[i].GetComp<CompPowerTrader>();
					bool flag2 = comp == null || comp.PowerOn;
					if (flag2)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005550 File Offset: 0x00003750
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool flag = t.Faction != pawn.Faction;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Building building = t as Building;
				bool flag2 = building == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = building.IsForbidden(pawn);
					if (flag3)
					{
						result = false;
					}
					else
					{
						LocalTargetInfo target = building;
						bool flag4 = !pawn.CanReserve(target, 1, -1, null, forced);
						if (flag4)
						{
							result = false;
						}
						else
						{
							CompDeepDrill compDeepDrill = building.TryGetComp<CompDeepDrill>();
							result = (compDeepDrill.CanDrillNow() && !building.IsBurning());
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000055E8 File Offset: 0x000037E8
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(WPJobDefOf.WPOperateDeepDrill, t, 1500, true);
		}
	}
}
