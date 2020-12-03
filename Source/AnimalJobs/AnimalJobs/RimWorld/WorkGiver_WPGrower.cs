using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000024 RID: 36
	public abstract class WorkGiver_WPGrower : WorkGiver_Scanner
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x000059E4 File Offset: 0x00003BE4
		public override bool AllowUnreachable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000059F8 File Offset: 0x00003BF8
		protected virtual bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
		{
			return true;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005A0B File Offset: 0x00003C0B
		public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
		{
			Danger maxDanger = pawn.NormalMaxDanger();
			List<Building> bList = pawn.Map.listerBuildings.allBuildingsColonist;
			int num;
			for (int i = 0; i < bList.Count; i = num + 1)
			{
				Building_PlantGrower building_PlantGrower = bList[i] as Building_PlantGrower;
				bool flag = building_PlantGrower != null && this.ExtraRequirements(building_PlantGrower, pawn) && !building_PlantGrower.IsForbidden(pawn) && pawn.CanReach(building_PlantGrower, PathEndMode.OnCell, maxDanger, false, TraverseMode.ByPawn) && !building_PlantGrower.IsBurning();
				if (flag)
				{
					foreach (IntVec3 intVec in building_PlantGrower.OccupiedRect())
					{
						yield return intVec;
					}
					WorkGiver_WPGrower.wantedPlantDef = null;
				}
				building_PlantGrower = null;
				num = i;
			}
			WorkGiver_WPGrower.wantedPlantDef = null;
			List<Zone> zonesList = pawn.Map.zoneManager.AllZones;
			for (int j = 0; j < zonesList.Count; j = num + 1)
			{
				Zone_Growing growZone = zonesList[j] as Zone_Growing;
				bool flag2 = growZone != null;
				if (flag2)
				{
					bool flag3 = growZone.cells.Count == 0;
					if (flag3)
					{
						string str = "Grow zone has 0 cells: ";
						Zone_Growing zone_Growing = growZone;
						Log.ErrorOnce(str + (zone_Growing?.ToString()), -563487, false);
					}
					else
					{
						bool flag4 = this.ExtraRequirements(growZone, pawn);
						if (flag4)
						{
							bool flag5 = !growZone.ContainsStaticFire;
							if (flag5)
							{
								bool flag6 = pawn.CanReach(growZone.Cells[0], PathEndMode.OnCell, maxDanger, false, TraverseMode.ByPawn);
								if (flag6)
								{
									for (int k = 0; k < growZone.cells.Count; k = num + 1)
									{
										yield return growZone.cells[k];
										num = k;
									}
									WorkGiver_WPGrower.wantedPlantDef = null;
								}
							}
						}
					}
				}
				growZone = null;
				num = j;
			}
			WorkGiver_WPGrower.wantedPlantDef = null;
			yield break;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005A24 File Offset: 0x00003C24
		public static ThingDef CalculateWantedPlantDef(IntVec3 c, Map map)
		{
			IPlantToGrowSettable plantToGrowSettable = c.GetPlantToGrowSettable(map);
			bool flag = plantToGrowSettable == null;
			ThingDef result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = plantToGrowSettable.GetPlantDefToGrow();
			}
			return result;
		}

		// Token: 0x04000021 RID: 33
		protected static ThingDef wantedPlantDef;
	}
}
