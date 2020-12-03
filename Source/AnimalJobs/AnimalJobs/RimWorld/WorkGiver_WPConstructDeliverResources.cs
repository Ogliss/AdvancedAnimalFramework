using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200001C RID: 28
	public abstract class WorkGiver_WPConstructDeliverResources : WorkGiver_Scanner
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00004754 File Offset: 0x00002954
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004767 File Offset: 0x00002967
		public static void ResetStaticData()
		{
			WorkGiver_WPConstructDeliverResources.MissingMaterialsTranslated = "MissingMaterials".Translate();
			WorkGiver_WPConstructDeliverResources.ForbiddenLowerTranslated = "ForbiddenLower".Translate();
			WorkGiver_WPConstructDeliverResources.NoPathTranslated = "NoPath".Translate();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000047A8 File Offset: 0x000029A8
		private static bool ResourceValidator(Pawn pawn, ThingDefCountClass need, Thing th)
		{
			return th.def == need.thingDef && !th.IsForbidden(pawn) && pawn.CanReserve(th, 1, -1, null, false);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000047E4 File Offset: 0x000029E4
		protected Job ResourceDeliverJobFor(Pawn pawn, IConstructible c, bool canRemoveExistingFloorUnderNearbyNeeders = true)
		{
			Blueprint_Install blueprint_Install = c as Blueprint_Install;
			bool flag = blueprint_Install != null;
			Job result;
			if (flag)
			{
				result = this.InstallJob(pawn, blueprint_Install);
			}
			else
			{
				bool flag2 = false;
				ThingDefCountClass thingDefCountClass = null;
				List<ThingDefCountClass> list = c.MaterialsNeeded();
				int count = list.Count;
				int i = 0;
				while (i < count)
				{
					ThingDefCountClass need = list[i];
					bool flag3 = !pawn.Map.itemAvailability.ThingsAvailableAnywhere(need, pawn);
					if (flag3)
					{
						flag2 = true;
						thingDefCountClass = need;
						break;
					}
					Thing foundRes = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(need.thingDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, (Thing r) => WorkGiver_WPConstructDeliverResources.ResourceValidator(pawn, need, r), null, 0, -1, false, RegionType.Set_Passable, false);
					bool flag4 = foundRes != null;
					if (flag4)
					{
						int resTotalAvailable;
						this.FindAvailableNearbyResources(foundRes, pawn, out resTotalAvailable);
						int num;
						Job job;
						HashSet<Thing> hashSet = this.FindNearbyNeeders(pawn, need, c, resTotalAvailable, canRemoveExistingFloorUnderNearbyNeeders, out num, out job);
						bool flag5 = job != null;
						if (flag5)
						{
							return job;
						}
						hashSet.Add((Thing)c);
						Thing thing = hashSet.MinBy((Thing nee) => IntVec3Utility.ManhattanDistanceFlat(foundRes.Position, nee.Position));
						hashSet.Remove(thing);
						int num2 = 0;
						int j = 0;
						do
						{
							num2 += WorkGiver_WPConstructDeliverResources.resourcesAvailable[j].stackCount;
							j++;
						}
						while (num2 < num && j < WorkGiver_WPConstructDeliverResources.resourcesAvailable.Count);
						WorkGiver_WPConstructDeliverResources.resourcesAvailable.RemoveRange(j, WorkGiver_WPConstructDeliverResources.resourcesAvailable.Count - j);
						WorkGiver_WPConstructDeliverResources.resourcesAvailable.Remove(foundRes);
						Job job2 = JobMaker.MakeJob(JobDefOf.HaulToContainer);
						job2.targetA = foundRes;
						job2.targetQueueA = new List<LocalTargetInfo>();
						for (j = 0; j < WorkGiver_WPConstructDeliverResources.resourcesAvailable.Count; j++)
						{
							job2.targetQueueA.Add(WorkGiver_WPConstructDeliverResources.resourcesAvailable[j]);
						}
						job2.targetB = thing;
						bool flag6 = hashSet.Count > 0;
						if (flag6)
						{
							job2.targetQueueB = new List<LocalTargetInfo>();
							foreach (Thing t in hashSet)
							{
								job2.targetQueueB.Add(t);
							}
						}
						job2.targetC = (Thing)c;
						job2.count = num;
						job2.haulMode = HaulMode.ToContainer;
						return job2;
					}
					else
					{
						flag2 = true;
						thingDefCountClass = need;
						i++;
					}
				}
				bool flag7 = flag2;
				if (flag7)
				{
					JobFailReason.Is(string.Format("{0}: {1}", WorkGiver_WPConstructDeliverResources.MissingMaterialsTranslated, thingDefCountClass.thingDef.label), null);
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004B80 File Offset: 0x00002D80
		private void FindAvailableNearbyResources(Thing firstFoundResource, Pawn pawn, out int resTotalAvailable)
		{
			int num = Mathf.Min(firstFoundResource.def.stackLimit, pawn.carryTracker.MaxStackSpaceEver(firstFoundResource.def));
			resTotalAvailable = 0;
			WorkGiver_WPConstructDeliverResources.resourcesAvailable.Clear();
			WorkGiver_WPConstructDeliverResources.resourcesAvailable.Add(firstFoundResource);
			resTotalAvailable += firstFoundResource.stackCount;
			bool flag = resTotalAvailable < num;
			if (flag)
			{
				foreach (Thing thing in GenRadial.RadialDistinctThingsAround(firstFoundResource.Position, firstFoundResource.Map, 5f, false))
				{
					bool flag2 = resTotalAvailable >= num;
					if (flag2)
					{
						break;
					}
					bool flag3 = thing.def == firstFoundResource.def && GenAI.CanUseItemForWork(pawn, thing);
					if (flag3)
					{
						WorkGiver_WPConstructDeliverResources.resourcesAvailable.Add(thing);
						resTotalAvailable += thing.stackCount;
					}
				}
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004C7C File Offset: 0x00002E7C
		private HashSet<Thing> FindNearbyNeeders(Pawn pawn, ThingDefCountClass need, IConstructible c, int resTotalAvailable, bool canRemoveExistingFloorUnderNearbyNeeders, out int neededTotal, out Job jobToMakeNeederAvailable)
		{
			neededTotal = need.count;
			HashSet<Thing> hashSet = new HashSet<Thing>();
			Thing thing = (Thing)c;
			foreach (Thing thing2 in GenRadial.RadialDistinctThingsAround(thing.Position, thing.Map, 8f, true))
			{
				bool flag = neededTotal >= resTotalAvailable;
				if (flag)
				{
					break;
				}
				bool flag2 = this.IsNewValidNearbyNeeder(thing2, hashSet, c, pawn);
				if (flag2)
				{
					Blueprint blueprint = thing2 as Blueprint;
					bool flag3 = blueprint == null || !WorkGiver_WPConstructDeliverResources.ShouldRemoveExistingFloorFirst(pawn, blueprint);
					if (flag3)
					{
						int num = GenConstruct.AmountNeededByOf((IConstructible)thing2, need.thingDef);
						bool flag4 = num > 0;
						if (flag4)
						{
							hashSet.Add(thing2);
							neededTotal += num;
						}
					}
				}
			}
			Blueprint blueprint2 = c as Blueprint;
			bool flag5 = blueprint2 != null && blueprint2.def.entityDefToBuild is TerrainDef && canRemoveExistingFloorUnderNearbyNeeders && neededTotal < resTotalAvailable;
			if (flag5)
			{
				foreach (Thing thing3 in GenRadial.RadialDistinctThingsAround(thing.Position, thing.Map, 3f, false))
				{
					bool flag6 = this.IsNewValidNearbyNeeder(thing3, hashSet, c, pawn);
					if (flag6)
					{
						Blueprint blueprint3 = thing3 as Blueprint;
						bool flag7 = blueprint3 != null;
						if (flag7)
						{
							Job job = this.RemoveExistingFloorJob(pawn, blueprint3);
							bool flag8 = job != null;
							if (flag8)
							{
								jobToMakeNeederAvailable = job;
								return hashSet;
							}
						}
					}
				}
			}
			jobToMakeNeederAvailable = null;
			return hashSet;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004E50 File Offset: 0x00003050
		private bool IsNewValidNearbyNeeder(Thing t, HashSet<Thing> nearbyNeeders, IConstructible constructible, Pawn pawn)
		{
			return t is IConstructible && t != constructible && !(t is Blueprint_Install) && t.Faction == pawn.Faction && !t.IsForbidden(pawn) && !nearbyNeeders.Contains(t) && GenConstruct.CanConstruct(t, pawn, false, false);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004EA8 File Offset: 0x000030A8
		protected static bool ShouldRemoveExistingFloorFirst(Pawn pawn, Blueprint blue)
		{
			return blue.def.entityDefToBuild is TerrainDef && pawn.Map.terrainGrid.CanRemoveTopLayerAt(blue.Position);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004EE8 File Offset: 0x000030E8
		protected Job RemoveExistingFloorJob(Pawn pawn, Blueprint blue)
		{
			bool flag = !WorkGiver_WPConstructDeliverResources.ShouldRemoveExistingFloorFirst(pawn, blue);
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = !pawn.CanReserve(blue.Position, 1, -1, ReservationLayerDefOf.Floor, false);
				if (flag2)
				{
					result = null;
				}
				else
				{
					Job job = JobMaker.MakeJob(JobDefOf.RemoveFloor, blue.Position);
					job.ignoreDesignations = true;
					result = job;
				}
			}
			return result;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004F50 File Offset: 0x00003150
		private Job InstallJob(Pawn pawn, Blueprint_Install install)
		{
			Thing miniToInstallOrBuildingToReinstall = install.MiniToInstallOrBuildingToReinstall;
			bool flag = miniToInstallOrBuildingToReinstall.IsForbidden(pawn);
			Job result;
			if (flag)
			{
				JobFailReason.Is(WorkGiver_WPConstructDeliverResources.ForbiddenLowerTranslated, null);
				result = null;
			}
			else
			{
				bool flag2 = !pawn.CanReach(miniToInstallOrBuildingToReinstall, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), false, TraverseMode.ByPawn);
				if (flag2)
				{
					JobFailReason.Is(WorkGiver_WPConstructDeliverResources.NoPathTranslated, null);
					result = null;
				}
				else
				{
					bool flag3 = !pawn.CanReserve(miniToInstallOrBuildingToReinstall, 1, -1, null, false);
					if (flag3)
					{
						Pawn pawn2 = pawn.Map.reservationManager.FirstRespectedReserver(miniToInstallOrBuildingToReinstall, pawn);
						bool flag4 = pawn2 != null;
						if (flag4)
						{
							JobFailReason.Is("ReservedBy".Translate(pawn2.LabelShort, pawn2), null);
						}
						result = null;
					}
					else
					{
						Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer);
						job.targetA = miniToInstallOrBuildingToReinstall;
						job.targetB = install;
						job.count = 1;
						job.haulMode = HaulMode.ToContainer;
						result = job;
					}
				}
			}
			return result;
		}

		// Token: 0x0400001B RID: 27
		private static List<Thing> resourcesAvailable = new List<Thing>();

		// Token: 0x0400001C RID: 28
		private const float MultiPickupRadius = 5f;

		// Token: 0x0400001D RID: 29
		private const float NearbyConstructScanRadius = 8f;

		// Token: 0x0400001E RID: 30
		private static string MissingMaterialsTranslated;

		// Token: 0x0400001F RID: 31
		private static string ForbiddenLowerTranslated;

		// Token: 0x04000020 RID: 32
		private static string NoPathTranslated;
	}
}
