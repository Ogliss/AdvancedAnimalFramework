using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	// Token: 0x02000025 RID: 37
	public class WorkGiver_WPGrowerSow : WorkGiver_WPGrower
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00005A5C File Offset: 0x00003C5C
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            if (!pawn.RaceProps.Animal)
            {
				return true;
            }
            return base.ShouldSkip(pawn, forced);
        }

        // Token: 0x060000A9 RID: 169 RVA: 0x00005A6F File Offset: 0x00003C6F
        public static void Reset()
		{
			WorkGiver_WPGrowerSow.CantSowCavePlantBecauseOfLightTrans = "CantSowCavePlantBecauseOfLight".Translate();
			WorkGiver_WPGrowerSow.CantSowCavePlantBecauseUnroofedTrans = "CantSowCavePlantBecauseUnroofed".Translate();
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005A9C File Offset: 0x00003C9C
		protected override bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
		{
			bool flag = !settable.CanAcceptSowNow();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Zone_Growing zone_Growing = settable as Zone_Growing;
				bool flag2 = zone_Growing != null;
				IntVec3 c;
				if (flag2)
				{
					bool flag3 = !zone_Growing.allowSow;
					if (flag3)
					{
						return false;
					}
					c = zone_Growing.Cells[0];
				}
				else
				{
					c = ((Thing)settable).Position;
				}
				WorkGiver_WPGrower.wantedPlantDef = WorkGiver_WPGrower.CalculateWantedPlantDef(c, pawn.Map);
				result = (WorkGiver_WPGrower.wantedPlantDef != null);
			}
			return result;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005B20 File Offset: 0x00003D20
		public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
		{
			Map map = pawn.Map;
			bool flag = c.IsForbidden(pawn);
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = !PlantUtility.GrowthSeasonNow(c, map, false);
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = WorkGiver_WPGrower.wantedPlantDef == null;
					if (flag3)
					{
						WorkGiver_WPGrower.wantedPlantDef = WorkGiver_WPGrower.CalculateWantedPlantDef(c, map);
						bool flag4 = WorkGiver_WPGrower.wantedPlantDef == null;
						if (flag4)
						{
							return null;
						}
					}
					List<Thing> thingList = c.GetThingList(map);
					bool flag5 = false;
					for (int i = 0; i < thingList.Count; i++)
					{
						Thing thing = thingList[i];
						bool flag6 = thing.def == WorkGiver_WPGrower.wantedPlantDef;
						if (flag6)
						{
							return null;
						}
						bool flag7 = (thing is Blueprint || thing is Frame) && thing.Faction == pawn.Faction;
						if (flag7)
						{
							flag5 = true;
						}
					}
					bool flag8 = flag5;
					if (flag8)
					{
						Thing edifice = c.GetEdifice(map);
						bool flag9 = edifice == null || edifice.def.fertility < 0f;
						if (flag9)
						{
							return null;
						}
					}
					bool cavePlant = WorkGiver_WPGrower.wantedPlantDef.plant.cavePlant;
					if (cavePlant)
					{
						bool flag10 = !c.Roofed(map);
						if (flag10)
						{
							JobFailReason.Is(WorkGiver_WPGrowerSow.CantSowCavePlantBecauseUnroofedTrans, null);
							return null;
						}
						bool flag11 = map.glowGrid.GameGlowAt(c, true) > 0f;
						if (flag11)
						{
							JobFailReason.Is(WorkGiver_WPGrowerSow.CantSowCavePlantBecauseOfLightTrans, null);
							return null;
						}
					}
					Plant plant = c.GetPlant(map);
					bool flag12 = plant != null && plant.def.plant.blockAdjacentSow;
					if (flag12)
					{
						bool flag13 = !pawn.CanReserve(plant, 1, -1, null, false) || plant.IsForbidden(pawn);
						if (flag13)
						{
							result = null;
						}
						else
						{
							result = new Job(JobDefOf.CutPlant, plant);
						}
					}
					else
					{
						Thing thing2 = PlantUtility.AdjacentSowBlocker(WorkGiver_WPGrower.wantedPlantDef, c, map);
						bool flag14 = thing2 != null;
						if (flag14)
						{
							Plant plant2 = thing2 as Plant;
							bool flag15 = plant2 != null && pawn.CanReserve(plant2, 1, -1, null, false) && !plant2.IsForbidden(pawn);
							if (flag15)
							{
								IPlantToGrowSettable plantToGrowSettable = plant2.Position.GetPlantToGrowSettable(plant2.Map);
								bool flag16 = plantToGrowSettable == null || plantToGrowSettable.GetPlantDefToGrow() != plant2.def;
								if (flag16)
								{
									return new Job(JobDefOf.CutPlant, plant2);
								}
							}
							result = null;
						}
						else
						{
							bool flag17 = WorkGiver_WPGrower.wantedPlantDef.plant.sowMinSkill > 0 && pawn.skills != null && pawn.skills.GetSkill(SkillDefOf.Plants).Level < WorkGiver_WPGrower.wantedPlantDef.plant.sowMinSkill;
							if (flag17)
							{
								result = null;
							}
							else
							{
								int j = 0;
								while (j < thingList.Count)
								{
									Thing thing3 = thingList[j];
									bool blockPlanting = thing3.def.BlocksPlanting(false);
									if (blockPlanting)
									{
										bool flag18 = !pawn.CanReserve(thing3, 1, -1, null, false);
										if (flag18)
										{
											return null;
										}
										bool flag19 = thing3.def.category == ThingCategory.Plant;
										if (flag19)
										{
											bool flag20 = !thing3.IsForbidden(pawn);
											if (flag20)
											{
												return new Job(JobDefOf.CutPlant, thing3);
											}
											return null;
										}
										else
										{
											bool everHaulable = thing3.def.EverHaulable;
											if (everHaulable)
											{
												return HaulAIUtility.HaulAsideJobFor(pawn, thing3);
											}
											return null;
										}
									}
									else
									{
										j++;
									}
								}
								bool flag21 = !WorkGiver_WPGrower.wantedPlantDef.CanEverPlantAt(c, map) || !PlantUtility.GrowthSeasonNow(c, map, false) || !pawn.CanReserve(c, 1, -1, null, false);
								if (flag21)
								{
									result = null;
								}
								else
								{
									result = new Job(WPJobDefOf.WPSow, c)
									{
										plantDefToSow = WorkGiver_WPGrower.wantedPlantDef
									};
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04000022 RID: 34
		protected static string CantSowCavePlantBecauseOfLightTrans;

		// Token: 0x04000023 RID: 35
		protected static string CantSowCavePlantBecauseUnroofedTrans;
	}
}
