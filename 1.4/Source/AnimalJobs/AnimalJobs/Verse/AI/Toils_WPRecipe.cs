using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public static class Toils_WPRecipe
	{
		public static Toil MakeUnfinishedThingIfNeeded()
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				if (curJob.RecipeDef.UsesUnfinishedThing)
				{
					if (!(curJob.GetTarget(TargetIndex.B).Thing is UnfinishedThing))
					{
						List<Thing> list = Toils_WPRecipe.CalculateIngredients(curJob, actor);
						string str = "ingredient list in toil_wprecipe";
						List<Thing> list2 = list;
					//	Log.Message(str + (list2?.ToString()));
						Thing thing = Toils_WPRecipe.CalculateDominantIngredient(curJob, list);
						for (int i = 0; i < list.Count; i++)
						{
							Thing thing2 = list[i];
							string str2 = "ingredient in toil_wprecipe";
							Thing thing3 = list[i];
						//	Log.Message(str2 + (thing3?.ToString()));
							actor.Map.designationManager.RemoveAllDesignationsOn(thing2, false);
							if (thing2.Spawned) thing2.DeSpawn(DestroyMode.Vanish);
						}
						ThingDef stuff = (!curJob.RecipeDef.unfinishedThingDef.MadeFromStuff) ? null : thing.def;
						UnfinishedThing unfinishedThing = (UnfinishedThing)ThingMaker.MakeThing(curJob.RecipeDef.unfinishedThingDef, stuff);
						unfinishedThing.Creator = actor;
						unfinishedThing.BoundBill = (Bill_ProductionWithUft)curJob.bill;
						unfinishedThing.ingredients = list;
						CompColorable compColorable = unfinishedThing.TryGetComp<CompColorable>();
						bool flag3 = compColorable != null;
						if (flag3) compColorable.SetColor(thing.DrawColor);
						GenSpawn.Spawn(unfinishedThing, curJob.GetTarget(TargetIndex.A).Cell, actor.Map, WipeMode.Vanish);
						curJob.SetTarget(TargetIndex.B, unfinishedThing);
						actor.Reserve(unfinishedThing, curJob, 1, -1, null, true);
					}
				}
			};
			return toil;
		}

		public static Toil DoRecipeWork()
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				JobDriver_WPDoBill jobDriver_WPDoBill = (JobDriver_WPDoBill)actor.jobs.curDriver;
				UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
				if (unfinishedThing != null && unfinishedThing.Initialized) jobDriver_WPDoBill.workLeft = unfinishedThing.workLeft;
				else
				{
					jobDriver_WPDoBill.workLeft = curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff);
					if (unfinishedThing != null) unfinishedThing.workLeft = jobDriver_WPDoBill.workLeft;
				}
				jobDriver_WPDoBill.billStartTick = Find.TickManager.TicksGame;
				jobDriver_WPDoBill.ticksSpentDoingRecipeWork = 0;
				curJob.bill.Notify_DoBillStarted(actor);
			};
			toil.tickAction = delegate()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				JobDriver_WPDoBill jobDriver_WPDoBill = (JobDriver_WPDoBill)actor.jobs.curDriver;
				UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
				if (unfinishedThing != null && unfinishedThing.Destroyed) actor.jobs.EndCurrentJob(JobCondition.Incompletable, true, true);
				else
				{
					jobDriver_WPDoBill.ticksSpentDoingRecipeWork++;
					curJob.bill.Notify_PawnDidWork(actor);
					IBillGiverWithTickAction billGiverWithTickAction = toil.actor.CurJob.GetTarget(TargetIndex.A).Thing as IBillGiverWithTickAction;
					if (billGiverWithTickAction != null) billGiverWithTickAction.UsedThisTick();
					float num = (curJob.RecipeDef.workSpeedStat != null) ? actor.GetStatValue(curJob.RecipeDef.workSpeedStat, true) : 1f;
					Building_WorkTable building_WorkTable = jobDriver_WPDoBill.BillGiver as Building_WorkTable;
					if (building_WorkTable != null) num *= building_WorkTable.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor, true);
					if (DebugSettings.fastCrafting) num *= 30f;
					jobDriver_WPDoBill.workLeft -= num;
					if (unfinishedThing != null) unfinishedThing.workLeft = jobDriver_WPDoBill.workLeft;
					actor.GainComfortFromCellIfPossible(false);
					if (jobDriver_WPDoBill.workLeft <= 0f) jobDriver_WPDoBill.ReadyForNextToil();
					if (curJob.bill.recipe.UsesUnfinishedThing)
					{
						int num2 = Find.TickManager.TicksGame - jobDriver_WPDoBill.billStartTick;
						if (num2 >= 3000 && num2 % 1000 == 0) actor.jobs.CheckForJobOverride();
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
				return 1f - ((JobDriver_WPDoBill)actor.jobs.curDriver).workLeft / curJob.bill.recipe.WorkAmountTotal(unfinishedThing?.Stuff);
			}, false, -0.5f);
			toil.FailOn(() => toil.actor.CurJob.bill.suspended);
			return toil;
		}

		public static Toil FinishRecipeAndStartStoringProduct()
		{
			Log.Message("start finish recipe, store product.");
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
			//	Log.Message("in finishing");
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				string str = "curJob";
				Job job = curJob;
				string str2 = job?.ToString();
				string str3 = " actor ";
				Pawn pawn = actor;
				Log.Message(str + str2 + str3 + (pawn?.ToString()));
				JobDriver_WPDoBill jobDriver_WPDoBill = (JobDriver_WPDoBill)actor.jobs.curDriver;
				List<Thing> list = Toils_WPRecipe.CalculateIngredients(curJob, actor);
				for (int i = 0; i < list.Count; i++)
				{
					string str4 = "toils_wprecipe: ingredient[i] ";
					Thing thing = list[i];
				//	Log.Message(str4 + (thing?.ToString()));
				}
				Thing thing2 = Toils_WPRecipe.CalculateDominantIngredient(curJob, list);
				string[] array = new string[8];
				array[0] = "finishing: curJob.RecipeDef ";
				int num = 1;
				RecipeDef recipeDef = curJob.RecipeDef;
				array[num] = (recipeDef?.ToString());
				array[2] = " actor ";
				int num2 = 3;
				Pawn pawn2 = actor;
				array[num2] = (pawn2?.ToString());
				array[4] = " ingredients ";
				int num3 = 5;
				List<Thing> list2 = list;
				array[num3] = list2?.ToString();
				array[6] = " dominantIngredient ";
				int num4 = 7;
				Thing thing3 = thing2;
				array[num4] = thing3?.ToString();
				Log.Message(string.Concat(array));
				List<Thing> list3 = WPGenRecipe.MakeRecipeProducts(curJob.RecipeDef, actor, list, thing2).ToList<Thing>();
				Toils_WPRecipe.ConsumeIngredients(list, curJob.RecipeDef, actor.Map);
				curJob.bill.Notify_IterationCompleted(actor, list);
				RecordsUtility.Notify_BillDone(actor, list3);
				UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
				bool flag = list3.Count == 0;
				if (flag)
				{
					actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				}
				else
				{
					bool flag2 = curJob.bill.GetStoreMode() == BillStoreModeDefOf.DropOnFloor;
					if (flag2)
					{
						for (int j = 0; j < list3.Count; j++)
						{
							bool flag3 = !GenPlace.TryPlaceThing(list3[j], actor.Position, actor.Map, ThingPlaceMode.Near, null, null, default);
							if (flag3)
							{
								Log.Error(string.Concat(new object[]
								{
									actor,
									" could not drop recipe product ",
									list3[j],
									" near ",
									actor.Position
								}));
							}
						}
						actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
					}
					else
					{
						bool flag4 = list3.Count > 1;
						if (flag4)
						{
							for (int k = 1; k < list3.Count; k++)
							{
								bool flag5 = !GenPlace.TryPlaceThing(list3[k], actor.Position, actor.Map, ThingPlaceMode.Near, null, null, default);
								if (flag5)
								{
									Log.Error(string.Concat(new object[]
									{
										actor,
										" could not drop recipe product ",
										list3[k],
										" near ",
										actor.Position
									}));
								}
							}
						}
						list3[0].SetPositionDirect(actor.Position);
						IntVec3 c;
						bool flag6 = StoreUtility.TryFindBestBetterStoreCellFor(list3[0], actor, actor.Map, StoragePriority.Unstored, actor.Faction, out c, true);
						if (flag6)
						{
							actor.carryTracker.TryStartCarry(list3[0]);
							curJob.targetB = c;
							curJob.targetA = list3[0];
							curJob.count = 99999;
						}
						else
						{
							bool flag7 = !GenPlace.TryPlaceThing(list3[0], actor.Position, actor.Map, ThingPlaceMode.Near, null, null, default);
							if (flag7)
							{
								Log.Error(string.Concat(new object[]
								{
									"Bill doer could not drop product ",
									list3[0],
									" near ",
									actor.Position
								}));
							}
							actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
						}
					}
				}
			};
			return toil;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000039A4 File Offset: 0x00001BA4
		private static List<Thing> CalculateIngredients(Job job, Pawn actor)
		{
			UnfinishedThing unfinishedThing = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
			bool flag = unfinishedThing != null;
			List<Thing> result;
			if (flag)
			{
				List<Thing> ingredients = unfinishedThing.ingredients;
				job.RecipeDef.Worker.ConsumeIngredient(unfinishedThing, job.RecipeDef, actor.Map);
				job.placedThings = null;
				result = ingredients;
			}
			else
			{
				List<Thing> list = new List<Thing>();
				bool flag2 = job.placedThings != null;
				if (flag2)
				{
					for (int i = 0; i < job.placedThings.Count; i++)
					{
						bool flag3 = job.placedThings[i].Count <= 0;
						if (flag3)
						{
							Log.Error(string.Concat(new object[]
							{
								"PlacedThing ",
								job.placedThings[i],
								" with count ",
								job.placedThings[i].Count,
								" for job ",
								job
							}));
						}
						else
						{
							bool flag4 = job.placedThings[i].Count < job.placedThings[i].thing.stackCount;
							Thing thing;
							if (flag4)
							{
								thing = job.placedThings[i].thing.SplitOff(job.placedThings[i].Count);
							}
							else
							{
								thing = job.placedThings[i].thing;
							}
							job.placedThings[i].Count = 0;
							bool flag5 = list.Contains(thing);
							if (flag5)
							{
								string str = "Tried to add ingredient from job placed targets twice: ";
								Thing thing2 = thing;
								Log.Error(str + (thing2?.ToString()));
							}
							else
							{
								list.Add(thing);
								IStrippable strippable = thing as IStrippable;
								bool flag6 = strippable != null;
								if (flag6)
								{
									strippable.Strip();
								}
							}
						}
					}
				}
				job.placedThings = null;
				result = list;
			}
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003BBC File Offset: 0x00001DBC
		private static Thing CalculateDominantIngredient(Job job, List<Thing> ingredients)
		{
			UnfinishedThing uft = job.GetTarget(TargetIndex.B).Thing as UnfinishedThing;
			bool flag = uft != null && uft.def.MadeFromStuff;
			Thing result;
			if (flag)
			{
				result = uft.ingredients.First((Thing ing) => ing.def == uft.Stuff);
			}
			else
			{
				bool flag2 = ingredients.NullOrEmpty<Thing>();
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool productHasIngredientStuff = job.RecipeDef.productHasIngredientStuff;
					if (productHasIngredientStuff)
					{
						result = ingredients[0];
					}
					else
					{
						bool flag3 = job.RecipeDef.products.Any((ThingDefCountClass x) => x.thingDef.MadeFromStuff);
						if (flag3)
						{
							result = (from x in ingredients
							where x.def.IsStuff
							select x).RandomElementByWeight((Thing x) => (float)x.stackCount);
						}
						else
						{
							result = ingredients.RandomElementByWeight((Thing x) => (float)x.stackCount);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003D0C File Offset: 0x00001F0C
		private static void ConsumeIngredients(List<Thing> ingredients, RecipeDef recipe, Map map)
		{
			for (int i = 0; i < ingredients.Count; i++)
			{
				recipe.Worker.ConsumeIngredient(ingredients[i], recipe, map);
				Log.Message("destroying ingredient");
			}
		}

		// Token: 0x0400000A RID: 10
		private const int LongCraftingProjectThreshold = 20000;
	}
}
