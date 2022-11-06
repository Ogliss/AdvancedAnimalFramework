using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	// Token: 0x02000022 RID: 34
	public class WorkGiver_WPFeedPatient : WorkGiver_Scanner
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000095 RID: 149 RVA: 0x0000561C File Offset: 0x0000381C
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
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

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x06000096 RID: 150 RVA: 0x00005638 File Offset: 0x00003838
        public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000564C File Offset: 0x0000384C
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005660 File Offset: 0x00003860
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.mapPawns.SpawnedHungryPawns;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005684 File Offset: 0x00003884
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			bool flag = pawn == null || pawn.CurJob != null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = pawn2 == null || pawn2 == pawn;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.def.feedHumanlikesOnly && !pawn2.RaceProps.Humanlike;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.def.feedAnimalsOnly && !pawn2.RaceProps.Animal;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = pawn2.needs.food == null || pawn2.needs.food.CurLevelPercentage > pawn2.needs.food.PercentageThreshHungry + 0.02f;
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = !FeedPatientUtility.ShouldBeFed(pawn2);
								if (flag6)
								{
									result = false;
								}
								else
								{
									LocalTargetInfo target = t;
									bool flag7 = !pawn.CanReserve(target, 1, -1, null, forced);
									if (flag7)
									{
										result = false;
									}
									else
									{
										bool flag8 = !pawn.CanReserve(target, 1, -1, null, false);
										if (flag8)
										{
											result = false;
										}
										else
										{
											bool flag9 = pawn2.needs.food.CurLevelPercentage < pawn2.needs.food.PercentageThreshHungry + 0.02f && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
											result = flag9;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000580C File Offset: 0x00003A0C
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = (Pawn)t;
			Predicate<Thing> predicate = (Thing t3) => t3.def.category == ThingCategory.Item && t3.IngestibleNow && pawn.RaceProps.CanEverEat(t3) && pawn.CanReserve(t3, 1, -1, null, false) && !t3.IsForbidden(pawn.Faction) && !t3.IsNotFresh() && t3.def.IsNutritionGivingIngestible;
			bool flag = predicate == null;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableAlways), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 10f, predicate, null, 0, -1, false, RegionType.Set_Passable, false);
				bool flag2 = thing == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = predicate != null && thing != null && pawn2 != null;
					if (flag3)
					{
						ThingDef def = thing.def;
						float nutrition = FoodUtility.GetNutrition(pawn2, thing, def);
						result = new Job(WPJobDefOf.WPFeedPatient)
						{
							targetA = thing,
							targetB = pawn2,
							count = FoodUtility.WillIngestStackCountOf(pawn2, def, nutrition)
						};
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}
	}
}
