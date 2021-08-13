using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Quarry;
using RBB_Code;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AnimalJobs
{
	// Token: 0x0200002F RID: 47
	[StaticConstructorOnStartup]
	internal static class HarmonyPatches
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x000080E4 File Offset: 0x000062E4
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("rimworld.walkingproblem.animaljobspatch");
			MethodInfo original = AccessTools.Method(typeof(GenConstruct), "CanConstruct", new Type[] { typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool) }, null);
			MethodInfo original2 = AccessTools.Method(typeof(Toils_Haul), "JumpToCarryToNextContainerIfPossible", null, null);
			MethodInfo original3 = AccessTools.Method(typeof(Bill), "PawnAllowedToStartAnew", null, null);
			MethodInfo methodInfo = AccessTools.Method(typeof(Toils_Recipe), "DoRecipeWork", null, null);
			MethodInfo original4 = AccessTools.Method(typeof(Frame), "CompleteConstruction", null, null);
			MethodInfo methodInfo2 = AccessTools.Method(typeof(Thing), "Destroy", null, null);
			MethodInfo methodInfo3 = AccessTools.Method(typeof(FoodUtility), "TryFindBestFoodSourceFor", null, null);
			MethodInfo methodInfo4 = AccessTools.Method(typeof(Corpse), "ButcherProducts", null, null);
			MethodInfo original5 = AccessTools.Method(typeof(GenConstruct), "HandleBlockingThingJob", null, null);
			HarmonyMethod prefix = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CanConstruct_Prefix"));
			HarmonyMethod prefix2 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("JumpToCarryToNextContainerIfPossible_Prefix"));
			HarmonyMethod prefix3 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("PawnAllowedToStartAnew_Prefix"));
			HarmonyMethod harmonyMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DoRecipeWork_Prefix"));
			HarmonyMethod prefix4 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CompleteConstruction_Prefix"));
			HarmonyMethod harmonyMethod2 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("Destroy_Prefix"));
			HarmonyMethod harmonyMethod3 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("TryFindBestFoodSourceFor_Prefix"));
			HarmonyMethod harmonyMethod4 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("ButcherProducts_Prefix"));
			HarmonyMethod prefix5 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("HandleBlockingThingJob_Prefix"));


			bool Fishin = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "firefoxpdm.RainbeausFishing");
            if (Fishin)
            {
				Fishing = DefDatabase<WorkTypeDef>.GetNamed("Fishing");
                if (Fishing != null)
				{
					FishingPatch(harmony);
				}
			}
			harmony.Patch(original, prefix, null, null, null);
			harmony.Patch(original2, prefix2, null, null, null);
			harmony.Patch(original3, prefix3, null, null, null);
			harmony.Patch(original4, prefix4, null, null, null);
			harmony.Patch(original5, prefix5, null, null, null);
		}
		public static WorkTypeDef Fishing = null;

		public static void FishingPatch(Harmony harmony)
        {

			MethodInfo method3 = AccessTools.TypeByName("RBB_Code.JobDriver_CatchFish").GetMethod("MakeNewToils", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo method4 = typeof(HarmonyPatches).GetMethod("JobDriver_CatchFish_Prefix");
			bool flag3 = method3 == null;
			if (method3 != null)
			{
				harmony.Patch(method3, prefix: new HarmonyMethod(method4));
			}

			MethodInfo method5 = AccessTools.TypeByName("Quarry.WorkGiver_MineQuarry").GetMethod("JobOnThing");
			MethodInfo method6 = typeof(HarmonyPatches).GetMethod("WorkGiver_MineQuarry_JobOnThing_Prefix");
			bool flag4 = method5 == null;
			if (method5 != null)
			{
				harmony.Patch(method5, prefix: new HarmonyMethod(method6));
			}

		}
		// Token: 0x060000EA RID: 234 RVA: 0x00008318 File Offset: 0x00006518
		public static void CanConstruct_Prefix(ref bool __result, Thing t, Pawn p, bool checkSkills = true, bool forced = false)
		{
			bool flag = !p.kindDef.RaceProps.Humanlike;
			if (flag)
			{
				bool flag2 = GenConstruct.FirstBlockingThing(t, p) != null;
				if (flag2)
				{
					__result = false;
				}
				LocalTargetInfo target = t;
				PathEndMode peMode = PathEndMode.Touch;
				Danger maxDanger = (!forced) ? p.NormalMaxDanger() : Danger.Deadly;
				bool flag3 = !p.CanReserveAndReach(target, peMode, maxDanger, 1, -1, null, forced);
				if (flag3)
				{
					__result = false;
				}
				bool flag4 = t.IsBurning();
				if (flag4)
				{
					__result = false;
				}
				__result = true;
			}
			__result = false;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000083A0 File Offset: 0x000065A0
		public static void HandleBlockingThingJob_Prefix(ref Job __result, Thing constructible, Pawn worker, bool forced = false)
		{
			Thing thing = GenConstruct.FirstBlockingThing(constructible, worker);
			bool flag = thing == null;
			if (flag)
			{
				__result = null;
			}
			bool flag2 = thing.def.category == ThingCategory.Plant && worker.RaceProps.Animal;
			if (flag2)
			{
				bool flag3 = worker.CanReserveAndReach(thing, PathEndMode.ClosestTouch, worker.NormalMaxDanger(), 1, -1, null, forced);
				if (flag3)
				{
					__result = new Job(JobDefOf.CutPlant, thing);
				}
			}
			else
			{
				bool flag4 = thing.def.category == ThingCategory.Item && worker.RaceProps.Animal;
				if (flag4)
				{
					bool everHaulable = thing.def.EverHaulable;
					if (everHaulable)
					{
						__result = HaulAIUtility.HaulAsideJobFor(worker, thing);
					}
					Log.ErrorOnce(string.Concat(new object[]
					{
						"Never haulable ",
						thing,
						" blocking ",
						constructible.ToStringSafe<Thing>(),
						" at ",
						constructible.Position
					}), 6429262);
				}
				else
				{
					bool flag5 = thing.def.category == ThingCategory.Building && worker.RaceProps.Animal;
					if (flag5)
					{
						bool flag6 = worker.CanReserveAndReach(thing, PathEndMode.Touch, worker.NormalMaxDanger(), 1, -1, null, forced);
						if (flag6)
						{
							__result = new Job(WPJobDefOf.WPDeconstruct, thing)
							{
								ignoreDesignations = true
							};
						}
					}
				}
			}
			__result = null;
		}

		public static void JumpToCarryToNextContainerIfPossible_Prefix(ref Toil __result, Toil carryToContainerToil, TargetIndex primaryTargetInd)
		{
			Toil toil = new Toil();
			toil.initAction = delegate ()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				if (actor.carryTracker.CarriedThing != null)
				{
					LocalTargetInfo target;
					if (curJob.targetQueueB != null && curJob.targetQueueB.Count > 0 && actor.RaceProps.Animal)
					{
						target = curJob.GetTarget(primaryTargetInd);
						Thing primaryTarget2 = target.Thing;
						bool hasSpareItems2 = actor.carryTracker.CarriedThing.stackCount > GenConstruct.AmountNeededByOf((IConstructible)(object)(IConstructible)primaryTarget2, actor.carryTracker.CarriedThing.def);
						Predicate<Thing> predicate = (Thing th) => ReservationUtility.CanReserve(actor, primaryTarget2, 1, -1, (ReservationLayerDef)null, false) && GenCollection.Any<ThingDefCountClass>(((IConstructible)th).MaterialsNeeded(), (Predicate<ThingDefCountClass>)((ThingDefCountClass need) => need.thingDef == actor.carryTracker.CarriedThing.def)) && ((th == primaryTarget2) | hasSpareItems2);
						Thing nextTarget2 = GenClosest.ClosestThing_Global_Reachable(actor.Position, actor.Map, curJob.targetQueueB.Select(delegate (LocalTargetInfo targ)
						{
							//IL_0000: Unknown result type (might be due to invalid IL or missing references)
							//IL_0001: Unknown result type (might be due to invalid IL or missing references)
							LocalTargetInfo val2 = targ;
							return val2.Thing;
						}), (PathEndMode)2, TraverseParms.For(actor, (Danger)3, (TraverseMode)0, false), 99999f, predicate, (Func<Thing, float>)null);
						if (nextTarget2 != null)
						{
							curJob.targetQueueB.RemoveAll((LocalTargetInfo targ) => targ.Thing == nextTarget2);
							curJob.targetB = nextTarget2;
							actor.jobs.curDriver.JumpToToil(carryToContainerToil);
						}
					}
					if (curJob.targetQueueB != null && curJob.targetQueueB.Count > 0 && !actor.RaceProps.Animal)
					{
						target = curJob.GetTarget(primaryTargetInd);
						Thing primaryTarget = target.Thing;
						bool hasSpareItems = actor.carryTracker.CarriedThing.stackCount > GenConstruct.AmountNeededByOf((IConstructible)(object)(IConstructible)primaryTarget, actor.carryTracker.CarriedThing.def);
						Predicate<Thing> predicate2 = (Thing th) => GenConstruct.CanConstruct(th, actor, false, false) && GenCollection.Any<ThingDefCountClass>(((IConstructible)th).MaterialsNeeded(), (Predicate<ThingDefCountClass>)((ThingDefCountClass need) => need.thingDef == actor.carryTracker.CarriedThing.def)) && ((th == primaryTarget) | hasSpareItems);
						Thing nextTarget = GenClosest.ClosestThing_Global_Reachable(actor.Position, actor.Map, curJob.targetQueueB.Select(delegate (LocalTargetInfo targ)
						{
							//IL_0000: Unknown result type (might be due to invalid IL or missing references)
							//IL_0001: Unknown result type (might be due to invalid IL or missing references)
							LocalTargetInfo val = targ;
							return val.Thing;
						}), (PathEndMode)2, TraverseParms.For(actor, (Danger)3, (TraverseMode)0, false), 99999f, predicate2, (Func<Thing, float>)null);
						if (nextTarget != null)
						{
							curJob.targetQueueB.RemoveAll((LocalTargetInfo targ) => targ.Thing == nextTarget);
							curJob.targetB = nextTarget;
							actor.jobs.curDriver.JumpToToil(carryToContainerToil);
						}
					}
				}
			};
			__result = toil;
		}
		// Token: 0x060000ED RID: 237 RVA: 0x00008550 File Offset: 0x00006750
		public static void PawnAllowedToStartAnew_Prefix(ref bool __result, Pawn p)
		{
			bool animal = p.RaceProps.Animal;
			if (animal)
			{
				__result = true;
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008574 File Offset: 0x00006774
		public static void DoRecipeWork_Prefix(ref Toil __result)
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Log.Message("init delegated.");
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				bool animal = actor.RaceProps.Animal;
				if (animal)
				{
					Log.Message("I am in.");
					JobDriver curDriver = actor.jobs.curDriver;
					JobDriver_WPDoBill jobDriver_WPDoBill = curDriver as JobDriver_WPDoBill;
					UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					jobDriver_WPDoBill.workLeft = curJob.bill.recipe.WorkAmountTotal(unfinishedThing.def);
					bool flag = unfinishedThing != null;
					if (flag)
					{
						unfinishedThing.workLeft = jobDriver_WPDoBill.workLeft;
					}
					jobDriver_WPDoBill.billStartTick = Find.TickManager.TicksGame;
					jobDriver_WPDoBill.ticksSpentDoingRecipeWork = 0;
					curJob.bill.Notify_DoBillStarted(actor);
				}
				else
				{
					Log.Message("I am here instead.");
					JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor.jobs.curDriver;
					UnfinishedThing unfinishedThing2 = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					bool flag2 = unfinishedThing2 != null && unfinishedThing2.Initialized;
					if (flag2)
					{
						jobDriver_DoBill.workLeft = unfinishedThing2.workLeft;
					}
					else
					{
						jobDriver_DoBill.workLeft = curJob.bill.recipe.WorkAmountTotal(unfinishedThing2?.Stuff);
						bool flag3 = unfinishedThing2 != null;
						if (flag3)
						{
							unfinishedThing2.workLeft = jobDriver_DoBill.workLeft;
						}
					}
					jobDriver_DoBill.billStartTick = Find.TickManager.TicksGame;
					jobDriver_DoBill.ticksSpentDoingRecipeWork = 0;
					curJob.bill.Notify_DoBillStarted(actor);
				}
			};
			toil.tickAction = delegate()
			{
				Log.Message("tick delegated.");
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				JobDriver curDriver = actor.jobs.curDriver;
				bool animal = actor.RaceProps.Animal;
				if (animal)
				{
					JobDriver_WPDoBill jobDriver_WPDoBill = curDriver as JobDriver_WPDoBill;
					UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					bool flag = unfinishedThing != null && unfinishedThing.Destroyed;
					if (flag)
					{
						actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
						return;
					}
					jobDriver_WPDoBill.ticksSpentDoingRecipeWork++;
					curJob.bill.Notify_PawnDidWork(actor);
					IBillGiverWithTickAction billGiverWithTickAction = toil.actor.CurJob.GetTarget(TargetIndex.A).Thing as IBillGiverWithTickAction;
					bool flag2 = billGiverWithTickAction != null;
					if (flag2)
					{
						billGiverWithTickAction.UsedThisTick();
					}
					float num = (curJob.RecipeDef.workSpeedStat != null) ? actor.GetStatValue(curJob.RecipeDef.workSpeedStat, true) : 1f;
					Building_WorkTable building_WorkTable = jobDriver_WPDoBill.BillGiver as Building_WorkTable;
					bool flag3 = building_WorkTable != null;
					if (flag3)
					{
						num *= building_WorkTable.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor, true);
					}
					bool fastCrafting = DebugSettings.fastCrafting;
					if (fastCrafting)
					{
						num *= 30f;
					}
					jobDriver_WPDoBill.workLeft -= num;
					bool flag4 = unfinishedThing != null;
					if (flag4)
					{
						unfinishedThing.workLeft = jobDriver_WPDoBill.workLeft;
					}
					actor.GainComfortFromCellIfPossible(false);
					bool flag5 = jobDriver_WPDoBill.workLeft <= 0f;
					if (flag5)
					{
						jobDriver_WPDoBill.ReadyForNextToil();
					}
					bool usesUnfinishedThing = curJob.bill.recipe.UsesUnfinishedThing;
					if (usesUnfinishedThing)
					{
						int num2 = Find.TickManager.TicksGame - jobDriver_WPDoBill.billStartTick;
						bool flag6 = num2 >= 3000 && num2 % 1000 == 0;
						if (flag6)
						{
							actor.jobs.CheckForJobOverride();
						}
					}
				}
				bool flag7 = !actor.RaceProps.Animal;
				if (flag7)
				{
					JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor.jobs.curDriver;
					UnfinishedThing unfinishedThing2 = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
					bool flag8 = unfinishedThing2 != null && unfinishedThing2.Destroyed;
					if (flag8)
					{
						actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
					}
					else
					{
						jobDriver_DoBill.ticksSpentDoingRecipeWork++;
						curJob.bill.Notify_PawnDidWork(actor);
						IBillGiverWithTickAction billGiverWithTickAction2 = toil.actor.CurJob.GetTarget(TargetIndex.A).Thing as IBillGiverWithTickAction;
						bool flag9 = billGiverWithTickAction2 != null;
						if (flag9)
						{
							billGiverWithTickAction2.UsedThisTick();
						}
						bool flag10 = curJob.RecipeDef.workSkill != null && curJob.RecipeDef.UsesUnfinishedThing;
						if (flag10)
						{
							actor.skills.GetSkill(curJob.RecipeDef.workSkill).Learn(0.11f * curJob.RecipeDef.workSkillLearnFactor, false);
						}
						float num3 = (curJob.RecipeDef.workSpeedStat != null) ? actor.GetStatValue(curJob.RecipeDef.workSpeedStat, true) : 1f;
						Building_WorkTable building_WorkTable2 = jobDriver_DoBill.BillGiver as Building_WorkTable;
						bool flag11 = building_WorkTable2 != null;
						if (flag11)
						{
							num3 *= building_WorkTable2.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor, true);
						}
						bool fastCrafting2 = DebugSettings.fastCrafting;
						if (fastCrafting2)
						{
							num3 *= 30f;
						}
						jobDriver_DoBill.workLeft -= num3;
						bool flag12 = unfinishedThing2 != null;
						if (flag12)
						{
							unfinishedThing2.workLeft = jobDriver_DoBill.workLeft;
						}
						actor.GainComfortFromCellIfPossible(false);
						bool flag13 = jobDriver_DoBill.workLeft <= 0f;
						if (flag13)
						{
							jobDriver_DoBill.ReadyForNextToil();
						}
						bool usesUnfinishedThing2 = curJob.bill.recipe.UsesUnfinishedThing;
						if (usesUnfinishedThing2)
						{
							int num4 = Find.TickManager.TicksGame - jobDriver_DoBill.billStartTick;
							bool flag14 = num4 >= 3000 && num4 % 1000 == 0;
							if (flag14)
							{
								actor.jobs.CheckForJobOverride();
							}
						}
					}
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.WithEffect(() => toil.actor.CurJob.bill.recipe.effectWorking, TargetIndex.A);
			toil.PlaySustainerOrSound(() => toil.actor.CurJob.bill.recipe.soundWorking);
			toil.WithProgressBar(TargetIndex.A, delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.CurJob;
				UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
				bool animal = actor.RaceProps.Animal;
				float result;
				if (animal)
				{
					result = 1f - ((JobDriver_WPDoBill)actor.jobs.curDriver).workLeft / curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff);
				}
				else
				{
					result = 1f - ((JobDriver_DoBill)actor.jobs.curDriver).workLeft / curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff);
				}
				return result;
			}, false, -0.5f);
			toil.FailOn(() => toil.actor.CurJob.bill.suspended);
			__result = toil;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008640 File Offset: 0x00006840
		public static bool CompleteConstruction_Prefix(Frame __instance, Pawn worker)
		{
			bool flag = !worker.RaceProps.Humanlike;
			bool result;
			if (flag)
			{
				Thing thing = __instance.holdingOwner.Take(__instance);
				__instance.resourceContainer.ClearAndDestroyContents(DestroyMode.Vanish);
				Map map = worker.Map;
				__instance.Destroy(DestroyMode.Vanish);
				bool flag2 = __instance.GetStatValue(StatDefOf.WorkToBuild, true) > 150f && __instance.def.entityDefToBuild is ThingDef && ((ThingDef)__instance.def.entityDefToBuild).category == ThingCategory.Building;
				if (flag2)
				{
					SoundDefOf.Building_Complete.PlayOneShot(new TargetInfo(thing.Position, map, false));
				}
				ThingDef thingDef = __instance.def.entityDefToBuild as ThingDef;
				Thing thing2 = null;
				bool flag3 = thingDef != null;
				if (flag3)
				{
					thing2 = ThingMaker.MakeThing(thingDef, thing.Stuff);
					thing2.SetFactionDirect(thing.Faction);
					CompQuality compQuality = thing2.TryGetComp<CompQuality>();
					bool flag4 = compQuality != null;
					if (flag4)
					{
						int relevantSkillLevel = 1;
						QualityCategory q = QualityUtility.GenerateQualityCreatedByPawn(relevantSkillLevel, false);
						compQuality.SetQuality(q, ArtGenerationContext.Colony);
						QualityUtility.SendCraftNotification(thing2, worker);
					}
					CompArt compArt = thing2.TryGetComp<CompArt>();
					bool flag5 = compArt != null;
					if (flag5)
					{
						bool flag6 = compQuality == null;
						if (flag6)
						{
							compArt.InitializeArt(ArtGenerationContext.Colony);
						}
						compArt.JustCreatedBy(worker);
					}
					thing2.HitPoints = Mathf.CeilToInt((float)__instance.HitPoints / (float)thing.MaxHitPoints * (float)thing2.MaxHitPoints);
					GenSpawn.Spawn(thing2, thing.Position, worker.Map, thing.Rotation, WipeMode.Vanish, false);
				}
				else
				{
					map.terrainGrid.SetTerrain(thing.Position, (TerrainDef)__instance.def.entityDefToBuild);
					FilthMaker.RemoveAllFilth(thing.Position, map);
				}
				worker.records.Increment(RecordDefOf.ThingsConstructed);
				bool flag7 = thing2 != null && thing2.GetStatValue(StatDefOf.WorkToBuild, true) >= 9500f;
				if (flag7)
				{
					TaleRecorder.RecordTale(TaleDefOf.CompletedLongConstructionProject, new object[]
					{
						worker,
						thing2.def
					});
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00008874 File Offset: 0x00006A74
		public static void Destroy_Prefix(Thing __instance, DestroyMode mode = DestroyMode.Vanish)
		{
			bool flag = !Thing.allowDestroyNonDestroyable && !__instance.def.destroyable;
			if (!flag)
			{
				bool destroyed = __instance.Destroyed;
				if (!destroyed)
				{
					bool spawned = __instance.Spawned;
					Map map = __instance.Map;
					bool spawned2 = __instance.Spawned;
					if (spawned2)
					{
						__instance.DeSpawn(mode);
					}
					sbyte value = Traverse.Create(typeof(Thing)).Field("mapIndexOrState").GetValue<sbyte>();
					bool discardOnDestroyed = __instance.def.DiscardOnDestroyed;
					if (discardOnDestroyed)
					{
						__instance.Discard(false);
					}
					CompExplosive compExplosive = __instance.TryGetComp<CompExplosive>();
					bool flag2 = compExplosive != null && compExplosive.destroyedThroughDetonation;
					bool flag3 = spawned && !flag2;
					if (flag3)
					{
						GenLeaving.DoLeavingsFor(__instance, map, mode, null);
					}
					bool flag4 = __instance.holdingOwner != null;
					if (flag4)
					{
						__instance.holdingOwner.Notify_ContainedItemDestroyed(__instance);
					}
					List<Map> maps = Find.Maps;
					for (int i = 0; i < maps.Count; i++)
					{
						bool flag5 = __instance.def.category == ThingCategory.Mote;
						if (flag5)
						{
							return;
						}
						bool flag6 = __instance.def.category != ThingCategory.Mote;
						if (flag6)
						{
							maps[i].reservationManager.ReleaseAllForTarget(__instance);
							maps[i].physicalInteractionReservationManager.ReleaseAllForTarget(__instance);
							IAttackTarget attackTarget = __instance as IAttackTarget;
							bool flag7 = attackTarget != null;
							if (flag7)
							{
								maps[i].attackTargetReservationManager.ReleaseAllForTarget(attackTarget);
							}
							maps[i].designationManager.RemoveAllDesignationsOn(__instance, false);
						}
					}
					bool flag8 = !(__instance is Pawn);
					if (flag8)
					{
						__instance.stackCount = 0;
					}
				}
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00008A58 File Offset: 0x00006C58
		public static void TryFindBestFoodSourceFor_Prefix(ref bool __result, Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false, bool allowHarvest = false)
		{
			bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
			bool animal = getter.RaceProps.Animal;
			bool flag2 = !eater.IsTeetotaler();
			Thing thing = null;
			if (canUseInventory)
			{
				bool flag3 = animal;
				if (flag3)
				{
					thing = FoodUtility.BestFoodInInventory(getter, eater, FoodPreferability.MealAwful, FoodPreferability.MealLavish, 0f, false);
				}
			}
			bool flag4 = getter == eater;
			ThingDef thingDef;
			Thing thing2 = FoodUtility.BestFoodSourceOnMap(getter, eater, desperate, out thingDef, FoodPreferability.MealLavish, flag4, flag2, allowCorpse, true, canRefillDispenser, allowForbidden, allowSociallyImproper, allowHarvest, false);
			bool flag5 = thing == null && thing2 == null;
			if (flag5)
			{
				bool flag6 = canUseInventory && animal;
				if (flag6)
				{
					FoodPreferability minFoodPref = FoodPreferability.DesperateOnly;
					bool allowDrug = flag2;
					thing = FoodUtility.BestFoodInInventory(getter, eater, minFoodPref, FoodPreferability.MealLavish, 0f, allowDrug);
					bool flag7 = thing != null;
					if (flag7)
					{
						foodSource = thing;
						foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
						__result = true;
					}
				}
				foodSource = null;
				foodDef = null;
				__result = false;
			}
			bool flag8 = thing == null && thing2 != null;
			if (flag8)
			{
				foodSource = thing2;
				foodDef = thingDef;
				__result = true;
			}
			ThingDef finalIngestibleDef = FoodUtility.GetFinalIngestibleDef(thing, false);
			bool flag9 = thing2 == null;
			if (flag9)
			{
				foodSource = thing;
				foodDef = finalIngestibleDef;
				__result = true;
			}
			float num = FoodUtility.FoodOptimality(eater, thing2, thingDef, (float)(getter.Position - thing2.Position).LengthManhattan, false);
			float num2 = FoodUtility.FoodOptimality(eater, thing, finalIngestibleDef, 0f, false);
			num2 -= 32f;
			bool flag10 = num > num2;
			if (flag10)
			{
				foodSource = thing2;
				foodDef = thingDef;
				__result = true;
			}
			foodSource = thing;
			foodDef = FoodUtility.GetFinalIngestibleDef(foodSource, false);
			__result = true;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00008C04 File Offset: 0x00006E04
		public static bool ButcherProducts_Prefix(Pawn butcher, float efficiency, ref IEnumerable<Thing> __result, Corpse __instance)
		{
			if (butcher.RaceProps.Animal)
			{
				__result = ((Func<IEnumerable<Thing>>)delegate
				{
					Log.Message("butcherproduct patched");
					Pawn innerPawn = __instance.InnerPawn;
					return ((Thing)innerPawn).ButcherProducts(butcher, efficiency);
				})();
				return true;
			}
			return true;
		}

		public static bool WorkGiver_MineQuarry_JobOnThing_Prefix(Pawn pawn, Thing t, bool forced, ref Job __result)
		{
			if (pawn.RaceProps.Animal)
			{
				__result = WorkGiver_MineQuarry_MakeNewToils(pawn, t, forced);
				return false;
			}

			return true;
		}

		public static Job WorkGiver_MineQuarry_MakeNewToils(Pawn pawn, Thing t, bool forced)
		{
			Building_Quarry quarry = t as Building_Quarry;
			/*
			if (!pawn.workSettings.WorkIsActive(QuarryDefOf.QuarryMining))
			{
				return null;
			}
			*/

			// Make sure a permitted quarry is found, and that it has resources, and does not have too many workers
			if (quarry == null || quarry.IsForbidden(pawn) || quarry.Depleted)
			{
				return null;
			}

			if (!quarry.Unowned && !quarry.AssignedPawns.Contains(pawn))
			{
				return null;
			}

			// Find a cell within the middle of the quarry to mine at
			IntVec3 cell = IntVec3.Invalid;
			CellRect rect = quarry.OccupiedRect().ContractedBy(quarry.WallThickness);
			foreach (IntVec3 c in rect.Cells.InRandomOrder())
			{
				if (pawn.Map.reservationManager.CanReserve(pawn, c, 1))
				{
					cell = c;
					break;
				}
			}
			// If a cell wasn't found, fail
			if (!cell.IsValid)
			{
				return null;
			}

			return new Job(QuarryDefOf.QRY_MineQuarry, cell);
		}

		public static bool JobDriver_CatchFish_Prefix(JobDriver __instance, ref IEnumerable<Toil> __result)
		{
			JobDriver_CatchFish catchFish = __instance as JobDriver_CatchFish;
			if (__instance.pawn.RaceProps.Animal && catchFish != null) 
			{
				__result = JobDriver_CatchFish_MakeNewToils(__instance, __instance.job.targetA.Thing);
				return false;
			}

			return true;
		}

		public static IEnumerable<Toil> JobDriver_CatchFish_MakeNewToils(JobDriver __instance, Thing ___TargetThingA)
		{
			JobDriver_CatchFish catchFish = __instance as JobDriver_CatchFish;
			int fishingDuration = 2000;
			Building_FishingSpot fishingSpot = ___TargetThingA as Building_FishingSpot;
			Passion passion = Passion.None;
			float skillGainFactor = 0f;
			catchFish.AddEndCondition(delegate
			{
				Thing thing = catchFish.pawn.jobs.curJob.GetTarget(catchFish.fishingSpotIndex).Thing;
				if (thing is Building && !thing.Spawned)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			catchFish.FailOnBurningImmobile(catchFish.fishingSpotIndex);
			catchFish.rotateToFace = TargetIndex.B;
			yield return Toils_Reserve.Reserve(catchFish.fishingSpotIndex, 1, -1, null);
			float fishingSkillLevel = 0f;

			if (catchFish.pawn.skills != null)
			{
				fishingSkillLevel = catchFish.pawn.skills.AverageOfRelevantSkillsFor(Fishing);
			}
			else
			{
				fishingSkillLevel = 1f;
			}
			float num = fishingSkillLevel / 20f;
			fishingDuration = (int)(2000f * (1.5f - num));
			fishingDuration = (int)((float)fishingDuration / (Controller.Settings.fishingEfficiency / 100f));
			yield return Toils_Goto.GotoThing(catchFish.fishingSpotIndex, fishingSpot.InteractionCell).FailOnDespawnedOrNull(catchFish.fishingSpotIndex);
			Toil toil = new Toil
			{
				initAction = delegate ()
				{
					ThingDef def;
					if (fishingSpot.Rotation == Rot4.North)
					{
						def = ThingDef.Named("Mote_FishingRodNorth");
					}
					else if (fishingSpot.Rotation == Rot4.East)
					{
						def = ThingDef.Named("Mote_FishingRodEast");
					}
					else if (fishingSpot.Rotation == Rot4.South)
					{
						def = ThingDef.Named("Mote_FishingRodSouth");
					}
					else
					{
						def = ThingDef.Named("Mote_FishingRodWest");
					}
					catchFish.fishingRodMote = (Mote)ThingMaker.MakeThing(def, null);
					catchFish.fishingRodMote.exactPosition = fishingSpot.fishingSpotCell.ToVector3Shifted();
					catchFish.fishingRodMote.Scale = 1f;
					GenSpawn.Spawn(catchFish.fishingRodMote, fishingSpot.fishingSpotCell, fishingSpot.Map, WipeMode.Vanish);
					WorkTypeDef fishing = Fishing;
					if (catchFish.pawn.skills != null)
					{
						passion = catchFish.pawn.skills.MaxPassionOfRelevantSkillsFor(fishing);
					}
					else
					{
						passion = Passion.None;

					}
					if (passion == Passion.None)
					{
						skillGainFactor = 0.3f;
						return;
					}
					if (passion == Passion.Minor)
					{
						skillGainFactor = 1f;
						return;
					}
					skillGainFactor = 1.5f;
				},
				tickAction = delegate ()
				{
					if (catchFish.pawn.skills != null)
					{
						catchFish.pawn.skills.Learn(SkillDefOf.Animals, 0.01f * skillGainFactor, false);
					}
					if (catchFish.ticksLeftThisToil == 1)
					{
						if (catchFish.fishingRodMote != null)
						{
							catchFish.fishingRodMote.Destroy(DestroyMode.Vanish);
						}
						List<Thing> list = fishingSpot.Map.thingGrid.ThingsListAt(fishingSpot.fishingSpotCell);
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].def.defName.Contains("Mote_Fishing"))
							{
								list[i].DeSpawn(DestroyMode.Vanish);
							}
						}
					}
				},
				defaultDuration = fishingDuration,
				defaultCompleteMode = ToilCompleteMode.Delay
			};
			yield return toil.WithProgressBarToilDelay(catchFish.fishingSpotIndex, false, -0.5f);
			Toil toil2 = new Toil
			{
				initAction = delegate ()
				{
					Job curJob = catchFish.pawn.jobs.curJob;
					Thing thing = null;
					float num2 = Rand.Value;
					TerrainDef terrainDef = fishingSpot.Map.terrainGrid.TerrainAt(fishingSpot.fishingSpotCell);
					if (terrainDef.defName.Equals("Marsh") || fishingSpot.Map.Biome.defName.Contains("Swamp"))
					{
						num2 -= 0.0025f;
					}
					if ((double)num2 < 0.0025)
					{
						FleckMaker.ThrowMetaIcon(catchFish.pawn.Position, fishingSpot.Map, FleckDefOf.IncapIcon);
						catchFish.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
						string str;
						if (fishingSpot.Map.Biome.defName.Contains("Tropical") && !terrainDef.defName.Contains("Ocean") && (double)Rand.Value < 0.33)
						{
							str = catchFish.pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.TragedyPiranhaText".Translate();
							Find.LetterStack.ReceiveLetter("RBB.TragedyTitle".Translate(), str, LetterDefOf.NegativeEvent, catchFish.pawn, null, null, null, null);
							int num3 = Rand.Range(3, 6);
							for (int i = 0; i < num3; i++)
							{
								catchFish.pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, (float)Rand.Range(1, 4), 0f, -1f, catchFish.pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
							}
							return;
						}
						if (terrainDef.defName.Contains("Ocean") && (double)Rand.Value < 0.33)
						{
							str = catchFish.pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.TragedyJellyfishText".Translate();
							Find.LetterStack.ReceiveLetter("RBB.TragedyTitle".Translate(), str, LetterDefOf.NegativeEvent, catchFish.pawn, null, null, null, null);
							catchFish.pawn.TakeDamage(new DamageInfo(DamageDefOf.Burn, (float)Rand.Range(3, 8), 0f, -1f, catchFish.pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
							return;
						}
						str = catchFish.pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.TragedyText".Translate();
						Find.LetterStack.ReceiveLetter("RBB.TragedyTitle".Translate(), str, LetterDefOf.NegativeEvent, catchFish.pawn, null, null, null, null);
						catchFish.pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, (float)Rand.Range(3, 8), 0f, -1f, catchFish.pawn, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null));
						return;
					}
					else
					{
						float num4 = 0.2f + fishingSkillLevel / 40f;
						num4 *= Controller.Settings.fishingEfficiency / 100f;
						if (fishingSpot.Map.Biome == BiomeDef.Named("AridShrubland") || fishingSpot.Map.Biome.defName.Contains("DesertArchi") || fishingSpot.Map.Biome.defName.Contains("Boreal"))
						{
							num4 -= 0.05f;
						}
						else if (fishingSpot.Map.Biome.defName.Contains("Oasis"))
						{
							num4 -= 0.075f;
						}
						else if (fishingSpot.Map.Biome.defName.Contains("ColdBog") || fishingSpot.Map.Biome == BiomeDef.Named("Desert") || fishingSpot.Map.Biome.defName.Contains("Tundra"))
						{
							num4 -= 0.1f;
						}
						else if (fishingSpot.Map.Biome.defName.Contains("Permafrost") || fishingSpot.Map.Biome.defName == "RRP_TemperateDesert")
						{
							num4 -= 0.125f;
						}
						else if (fishingSpot.Map.Biome == BiomeDef.Named("ExtremeDesert") || fishingSpot.Map.Biome == BiomeDef.Named("IceSheet") || fishingSpot.Map.Biome == BiomeDef.Named("SeaIce"))
						{
							num4 -= 0.15f;
						}
						if (Rand.Value > num4)
						{
							string text = catchFish.pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.CaughtNothing".Translate();
							if (Rand.Value < 0.75f)
							{
								FleckMaker.ThrowMetaIcon(catchFish.pawn.Position, fishingSpot.Map, FleckDefOf.IncapIcon);
								catchFish.pawn.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
								if (Controller.Settings.failureLevel >= 1f)
								{
									if (Controller.Settings.failureLevel < 2f)
									{
										Messages.Message(text, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.SilentInput, true);
										return;
									}
									if (Controller.Settings.failureLevel < 3f)
									{
										Messages.Message(text, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.NegativeEvent, true);
										return;
									}
									Find.LetterStack.ReceiveLetter("RBB.CaughtNothingTitle".Translate(), text, LetterDefOf.NegativeEvent, catchFish.pawn, null, null, null, null);
								}
								return;
							}
							float value = Rand.Value;
							float value2 = Rand.Value;
							if (value < 0.75f)
							{
								if (value2 < 0.5f)
								{
									thing = GenSpawn.Spawn(ThingDefOf.WoodLog, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
								}
								else
								{
									thing = GenSpawn.Spawn(ThingDefOf.Cloth, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
								}
							}
							else if (value2 < 0.4f)
							{
								thing = GenSpawn.Spawn(ThingDefOf.WoodLog, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else if (value2 < 0.7f)
							{
								thing = GenSpawn.Spawn(ThingDefOf.Cloth, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else if (value2 < 0.8f)
							{
								thing = GenSpawn.Spawn(ThingDefOf.Steel, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else if (value2 < 0.85f)
							{
								thing = GenSpawn.Spawn(ThingDef.Named("WoolMuffalo"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else if (value2 < 0.9f)
							{
								thing = GenSpawn.Spawn(ThingDef.Named("WoolMegasloth"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else if (value2 < 0.975f)
							{
								thing = GenSpawn.Spawn(ThingDef.Named("WoolCamel"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else if (value2 < 0.97f)
							{
								thing = GenSpawn.Spawn(ThingDef.Named("WoolAlpaca"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							else
							{
								thing = GenSpawn.Spawn(ThingDef.Named("Synthread"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
							}
							if (Rand.Value < 0.75f)
							{
								fishingSpot.fishStock--;
							}
							text = catchFish.pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.SnaggedJunk".Translate() + thing.def.label + ".";
							if (Controller.Settings.junkLevel >= 1f)
							{
								if (Controller.Settings.junkLevel < 2f)
								{
									Messages.Message(text, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.SilentInput, true);
								}
								else if (Controller.Settings.junkLevel < 3f)
								{
									Messages.Message(text, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.NegativeEvent, true);
								}
								else
								{
									Find.LetterStack.ReceiveLetter("RBB.SnaggedJunkTitle".Translate(), text, LetterDefOf.NegativeEvent, catchFish.pawn, null, null, null, null);
								}
							}
						}
						else
						{
							float num5 = fishingSkillLevel * 0.0002f;
							if (terrainDef.defName.Contains("Deep"))
							{
								num5 += 0.001f;
							}
							if (Rand.Value <= num5)
							{
								float value3 = Rand.Value;
								float value4 = Rand.Value;
								if (value3 < 0.75f)
								{
									thing = GenSpawn.Spawn(ThingDefOf.Silver, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
									thing.stackCount = Rand.RangeInclusive(5, 50);
								}
								else if (value4 < 0.6f)
								{
									thing = GenSpawn.Spawn(ThingDefOf.Silver, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
									thing.stackCount = Rand.RangeInclusive(10, 100);
								}
								else if (value4 < 0.9f)
								{
									thing = GenSpawn.Spawn(ThingDefOf.Gold, catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
									thing.stackCount = Rand.RangeInclusive(10, 100);
								}
								else if (value4 < 0.96f)
								{
									thing = GenSpawn.Spawn(ThingDef.Named("Gun_ChargeRifle"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
								}
								else if (value4 < 0.98f)
								{
									thing = GenSpawn.Spawn(ThingDef.Named("SimpleProstheticArm"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
								}
								else
								{
									thing = GenSpawn.Spawn(ThingDef.Named("SimpleProstheticLeg"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
								}
								string text2 = string.Concat(new object[]
								{
									catchFish.pawn.Name.ToStringShort.CapitalizeFirst() + "RBB.SunkenTreasureText".Translate(),
									thing.stackCount,
									" ",
									thing.def.label,
									"."
								});
								if (Controller.Settings.treasureLevel >= 1f)
								{
									if (Controller.Settings.treasureLevel < 2f)
									{
										Messages.Message(text2, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.SilentInput, true);
									}
									else if (Controller.Settings.treasureLevel < 3f)
									{
										Messages.Message(text2, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.PositiveEvent, true);
									}
									else
									{
										Find.LetterStack.ReceiveLetter("RBB.SunkenTreasureTitle".Translate(), text2, LetterDefOf.PositiveEvent, catchFish.pawn, null, null, null, null);
									}
								}
							}
							else
							{
								float num6 = Rand.Value + num4;
								string text3 = catchFish.pawn.Name.ToStringShort.CapitalizeFirst();
								if (num6 >= 0.4f && num6 < 0.9f)
								{
									thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
									text3 += "RBB.TinyFish".Translate();
								}
								else if (terrainDef.defName == "Marsh" && num6 >= 0.9f && num6 < 1.15f)
								{
									thing = GenSpawn.Spawn(ThingDef.Named("Eel"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
									text3 += "RBB.Eel".Translate();
								}
								else if (num6 >= 1.15f && num6 < 1.4f)
								{
									thing = GenSpawn.Spawn(ThingDef.Named("Eel"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
									text3 += "RBB.Eel".Translate();
								}
								else
								{
									float value5 = Rand.Value;
									if (terrainDef.defName.Contains("Ocean"))
									{
										if (terrainDef.defName.Contains("Deep") && (double)value5 > 0.7)
										{
											if ((double)value5 > 0.85)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Squid"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Squid".Translate();
											}
											else
											{
												thing = GenSpawn.Spawn(ThingDef.Named("SeaCucumber"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.SeaCucumber".Translate();
											}
										}
										else if (fishingSpot.Map.Biome.defName == "IceSheet" || fishingSpot.Map.Biome.defName == "SeaIce")
										{
											if ((double)value5 < 0.025)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Salmon".Translate();
											}
											else if ((double)value5 < 0.125)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Jellyfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Jellyfish".Translate();
											}
											else if ((double)value5 < 0.475)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.TinyFish".Translate();
											}
											else if ((double)value5 > 0.975)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Squid"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Squid".Translate();
											}
											else if ((double)value5 > 0.95)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("SeaCucumber"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.SeaCucumber".Translate();
											}
											else
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Fish".Translate();
											}
										}
										else if (fishingSpot.Map.Biome.defName.Contains("Tundra") || fishingSpot.Map.Biome.defName.Contains("Permafrost") || fishingSpot.Map.Biome.defName.Contains("Boreal") || fishingSpot.Map.Biome.defName.Contains("ColdBog"))
										{
											if ((double)value5 < 0.15)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Salmon".Translate();
											}
											else if ((double)value5 < 0.25)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Jellyfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Jellyfish".Translate();
											}
											else if ((double)value5 < 0.35)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.TinyFish".Translate();
											}
											else if ((double)value5 < 0.4)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Bass".Translate();
											}
											else if ((double)value5 > 0.95)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Squid"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Squid".Translate();
											}
											else if ((double)value5 > 0.9)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("SeaCucumber"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.SeaCucumber".Translate();
											}
											else
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Fish".Translate();
											}
										}
										else if ((fishingSpot.Map.Biome.defName.Contains("Temperate") && fishingSpot.Map.Biome.defName != "RRP_TemperateDesert") || fishingSpot.Map.Biome.defName.Contains("Steppes") || fishingSpot.Map.Biome.defName.Contains("Grassland") || fishingSpot.Map.Biome.defName.Contains("Savanna"))
										{
											if ((double)value5 < 0.05)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Salmon".Translate();
											}
											else if ((double)value5 < 0.15)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Jellyfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Jellyfish".Translate();
											}
											else if ((double)value5 < 0.25)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Pufferfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Pufferfish".Translate();
											}
											else if ((double)value5 < 0.45)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Bass".Translate();
											}
											else if ((double)value5 > 0.95)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Squid"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Squid".Translate();
											}
											else if ((double)value5 > 0.9)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("SeaCucumber"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.SeaCucumber".Translate();
											}
											else
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Fish".Translate();
											}
										}
										else if (fishingSpot.Map.Biome.defName.Contains("Tropical") || fishingSpot.Map.Biome.defName == "AridShrubland" || fishingSpot.Map.Biome.defName.Contains("Desert") || fishingSpot.Map.Biome.defName.Contains("Oasis"))
										{
											if ((double)value5 < 0.1)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Jellyfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Jellyfish".Translate();
											}
											else if ((double)value5 < 0.25)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Pufferfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Pufferfish".Translate();
											}
											else if ((double)value5 < 0.45)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Bass".Translate();
											}
											else if ((double)value5 > 0.95)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Squid"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Squid".Translate();
											}
											else if ((double)value5 > 0.9)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("SeaCucumber"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.SeaCucumber".Translate();
											}
											else
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Fish".Translate();
											}
										}
										else if ((double)value5 < 0.1)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Jellyfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Jellyfish".Translate();
										}
										else if ((double)value5 < 0.2)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Pufferfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Pufferfish".Translate();
										}
										else if ((double)value5 < 0.3)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.TinyFish".Translate();
										}
										else if ((double)value5 < 0.4)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else if ((double)value5 > 0.95)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Squid"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Squid".Translate();
										}
										else if ((double)value5 > 0.9)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("SeaCucumber"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.SeaCucumber".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName.Contains("Tundra") || fishingSpot.Map.Biome.defName.Contains("Permafrost"))
									{
										if ((double)value5 < 0.5)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Salmon".Translate();
										}
										else if ((double)value5 < 0.7)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else if ((double)value5 < 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Catfish".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName.Contains("Boreal") || fishingSpot.Map.Biome.defName.Contains("ColdBog"))
									{
										if ((double)value5 < 0.02)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.1)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.5)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Salmon".Translate();
										}
										else if ((double)value5 < 0.7)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else if ((double)value5 < 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Catfish".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if ((fishingSpot.Map.Biome.defName.Contains("Temperate") && fishingSpot.Map.Biome.defName != "RRP_TemperateDesert") || fishingSpot.Map.Biome.defName.Contains("Steppes") || fishingSpot.Map.Biome.defName.Contains("Grassland") || fishingSpot.Map.Biome.defName.Contains("Savanna"))
									{
										if (fishingSpot.Map.Biome.defName.Contains("Swamp"))
										{
											if ((double)value5 < 0.02)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Sturgeon".Translate();
											}
											else if ((double)value5 < 0.1)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Sturgeon".Translate();
											}
											else if ((double)value5 < 0.2)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Salmon".Translate();
											}
											else if ((double)value5 < 0.45)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Bass".Translate();
											}
											else if ((double)value5 > 0.75)
											{
												thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Catfish".Translate();
											}
											else
											{
												thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
												text3 += "RBB.Fish".Translate();
											}
										}
										else if ((double)value5 < 0.04)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.2)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.3)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Salmon".Translate();
										}
										else if ((double)value5 < 0.6)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else if ((double)value5 > 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Catfish".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName.Contains("Tropical"))
									{
										if ((double)value5 < 0.01)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.05)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.15)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Arapaima"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Arapaima".Translate();
										}
										else if ((double)value5 < 0.45)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Piranha"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Piranha".Translate();
										}
										else if ((double)value5 < 0.65)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else if ((double)value5 > 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Catfish".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName == "AridShrubland" || fishingSpot.Map.Biome.defName.Contains("DesertArchi"))
									{
										if ((double)value5 < 0.02)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.1)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.2)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.TinyFish".Translate();
										}
										else if ((double)value5 < 0.7)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else if ((double)value5 > 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Catfish".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName == "Desert" || fishingSpot.Map.Biome.defName.Contains("Oasis"))
									{
										if ((double)value5 < 0.01)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.05)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Sturgeon".Translate();
										}
										else if ((double)value5 < 0.3)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.TinyFish".Translate();
										}
										else if ((double)value5 < 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName == "ExtremeDesert" || fishingSpot.Map.Biome.defName == "RRP_TemperateDesert")
									{
										if ((double)value5 < 0.5)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.TinyFish".Translate();
										}
										else if ((double)value5 < 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if (fishingSpot.Map.Biome.defName == "IceSheet" || fishingSpot.Map.Biome.defName == "SeaIce")
									{
										if ((double)value5 < 0.15)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Salmon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Salmon".Translate();
										}
										else if ((double)value5 < 0.65)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.TinyFish".Translate();
										}
										else if ((double)value5 < 0.75)
										{
											thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Bass".Translate();
										}
										else
										{
											thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
											text3 += "RBB.Fish".Translate();
										}
									}
									else if ((double)value5 < 0.01)
									{
										thing = GenSpawn.Spawn(ThingDef.Named("SturgeonCaviar"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
										text3 += "RBB.Sturgeon".Translate();
									}
									else if ((double)value5 < 0.05)
									{
										thing = GenSpawn.Spawn(ThingDef.Named("Sturgeon"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
										text3 += "RBB.Sturgeon".Translate();
									}
									else if ((double)value5 < 0.2)
									{
										thing = GenSpawn.Spawn(ThingDef.Named("RawFishTiny"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
										text3 += "RBB.TinyFish".Translate();
									}
									else if ((double)value5 < 0.7)
									{
										thing = GenSpawn.Spawn(ThingDef.Named("Bass"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
										text3 += "RBB.Bass".Translate();
									}
									else if ((double)value5 > 0.75)
									{
										thing = GenSpawn.Spawn(ThingDef.Named("Catfish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
										text3 += "RBB.Catfish".Translate();
									}
									else
									{
										thing = GenSpawn.Spawn(ThingDef.Named("RawFish"), catchFish.pawn.Position, fishingSpot.Map, WipeMode.Vanish);
										text3 += "RBB.Fish".Translate();
									}
								}
								fishingSpot.fishStock--;
								if (Controller.Settings.successLevel >= 1f)
								{
									if (Controller.Settings.successLevel < 2f)
									{
										Messages.Message(text3, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.SilentInput, true);
									}
									else if (Controller.Settings.successLevel < 3f)
									{
										Messages.Message(text3, new TargetInfo(catchFish.pawn.Position, fishingSpot.Map, false), MessageTypeDefOf.PositiveEvent, true);
									}
									else
									{
										Find.LetterStack.ReceiveLetter("RBB.FishingSuccessTitle".Translate(), text3, LetterDefOf.PositiveEvent, catchFish.pawn, null, null, null, null);
									}
								}
							}
						}
						catchFish.pawn.carryTracker.TryStartCarry(thing, thing.stackCount, true);
						catchFish.pawn.carryTracker.TryDropCarriedThing(catchFish.pawn.Position, ThingPlaceMode.Near, out thing, null);
						if (!Controller.Settings.fishersHaul.Equals(true))
						{
							catchFish.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
							return;
						}
						IntVec3 c;
						if (StoreUtility.TryFindBestBetterStoreCellFor(thing, catchFish.pawn, fishingSpot.Map, StoragePriority.Unstored, catchFish.pawn.Faction, out c, true))
						{
							catchFish.pawn.Reserve(thing, catchFish.job, 1, -1, null, true);
							catchFish.pawn.Reserve(c, catchFish.job, 1, -1, null, true);
							catchFish.pawn.CurJob.SetTarget(TargetIndex.B, c);
							catchFish.pawn.CurJob.SetTarget(TargetIndex.A, thing);
							catchFish.pawn.CurJob.count = 1;
							catchFish.pawn.CurJob.haulMode = HaulMode.ToCellStorage;
							return;
						}
						catchFish.pawn.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
						return;
					}
				}
			};
			yield return toil2;
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, false, false);
			Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.B);
			yield return carryToCell;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.B, carryToCell, true, false);
			yield return Toils_Reserve.Release(catchFish.fishingSpotIndex);
			yield break;
		}
		
	}
}
