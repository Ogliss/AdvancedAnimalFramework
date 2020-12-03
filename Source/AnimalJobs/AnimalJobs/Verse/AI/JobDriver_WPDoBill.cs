using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse.AI
{
	// Token: 0x0200000B RID: 11
	public class JobDriver_WPDoBill : JobDriver
	{
		// Token: 0x06000019 RID: 25 RVA: 0x000033D8 File Offset: 0x000015D8
		public override string GetReport()
		{
			bool flag = this.job.RecipeDef != null;
			string result;
			if (flag)
			{
				result = base.ReportStringProcessed(this.job.RecipeDef.jobString);
			}
			else
			{
				result = base.GetReport();
			}
			return result;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000341C File Offset: 0x0000161C
		public IBillGiver BillGiver
		{
			get
			{
				IBillGiver billGiver = this.job.GetTarget(TargetIndex.A).Thing as IBillGiver;
				bool flag = billGiver == null;
				if (flag)
				{
					throw new InvalidOperationException("DoBill on non-Billgiver.");
				}
				return billGiver;
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003460 File Offset: 0x00001660
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.workLeft, "workLeft", 0f, false);
			Scribe_Values.Look<int>(ref this.billStartTick, "billStartTick", 0, false);
			Scribe_Values.Look<int>(ref this.ticksSpentDoingRecipeWork, "ticksSpentDoingRecipeWork", 0, false);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000034B4 File Offset: 0x000016B4
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo target = this.job.GetTarget(TargetIndex.A);
			Job job = this.job;
			bool flag = !pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.pawn.ReserveAsManyAsPossible(this.job.GetTargetQueue(TargetIndex.B), this.job, 1, -1, null);
				result = true;
			}
			return result;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000351E File Offset: 0x0000171E
		protected override IEnumerable<Toil> MakeNewToils()
		{
			base.AddEndCondition(delegate
			{
				Thing thing = base.GetActor().jobs.curJob.GetTarget(TargetIndex.A).Thing;
				bool flag2 = thing is Building && !thing.Spawned;
				JobCondition result;
				if (flag2)
				{
					result = JobCondition.Incompletable;
				}
				else
				{
					result = JobCondition.Ongoing;
				}
				return result;
			});
			this.FailOnBurningImmobile(TargetIndex.A);
			this.FailOn(delegate()
			{
				IBillGiver billGiver = this.job.GetTarget(TargetIndex.A).Thing as IBillGiver;
				bool flag2 = billGiver != null;
				if (flag2)
				{
					bool deletedOrDereferenced = this.job.bill.DeletedOrDereferenced;
					if (deletedOrDereferenced)
					{
						return true;
					}
					bool flag3 = !billGiver.CurrentlyUsableForBills();
					if (flag3)
					{
						return true;
					}
				}
				return false;
			});
			Toil gotoBillGiver = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			yield return new Toil
			{
				initAction = delegate()
				{
					bool flag2 = this.job.targetQueueB != null && this.job.targetQueueB.Count == 1;
					if (flag2)
					{
						UnfinishedThing unfinishedThing = this.job.targetQueueB[0].Thing as UnfinishedThing;
						bool flag3 = unfinishedThing != null;
						if (flag3)
						{
							unfinishedThing.BoundBill = (this.job.bill as Bill_ProductionWithUft);
						}
					}
				}
			};
			yield return Toils_Jump.JumpIf(gotoBillGiver, () => this.job.GetTargetQueue(TargetIndex.B).NullOrEmpty<LocalTargetInfo>());
			Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B, true);
			yield return extract;
			Toil getToHaulTarget = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
			yield return getToHaulTarget;
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, true, false, true);
			yield return JobDriver_WPDoBill.JumpToCollectNextIntoHandsForBill(getToHaulTarget, TargetIndex.B);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnDestroyedOrNull(TargetIndex.B);
			Toil findPlaceTarget = Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
			yield return findPlaceTarget;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, false, false);
			yield return Toils_Jump.JumpIfHaveTargetInQueue(TargetIndex.B, extract);
			yield return gotoBillGiver;
			yield return Toils_WPRecipe.MakeUnfinishedThingIfNeeded();
			yield return Toils_WPRecipe.DoRecipeWork().FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			yield return Toils_WPRecipe.FinishRecipeAndStartStoringProduct();
			bool flag = !this.job.RecipeDef.products.NullOrEmpty<ThingDefCountClass>() || !this.job.RecipeDef.specialProducts.NullOrEmpty<SpecialProductType>();
			if (flag)
			{
				yield return Toils_Reserve.Reserve((TargetIndex)2, 1, -1, (ReservationLayerDef)null);
				Toil carryToCell = Toils_Haul.CarryHauledThingToCell((TargetIndex)2);
				yield return carryToCell;
				yield return Toils_Haul.PlaceHauledThingInCell((TargetIndex)2, carryToCell, true, false);
				Toil recount = (Toil)(object)new Toil();
				recount.initAction = delegate
				{
					Bill_Production val2 = recount.actor.jobs.curJob.bill as Bill_Production;
					if (val2 != null && val2.repeatMode == BillRepeatModeDefOf.TargetCount)
					{
						this.Map.resourceCounter.UpdateResourceCounts();
					}
				};
				yield return recount;
			}
			yield break;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003530 File Offset: 0x00001730
		private static Toil JumpToCollectNextIntoHandsForBill(Toil gotoGetTargetToil, TargetIndex ind)
		{
			Toil toil = new Toil();
			toil.initAction = delegate()
			{
				Pawn actor = toil.actor;
				bool flag = actor.carryTracker.CarriedThing == null;
				if (flag)
				{
					string str = "JumpToAlsoCollectTargetInQueue run on ";
					Pawn pawn = actor;
					Log.Error(str + ((pawn != null) ? pawn.ToString() : null) + " who is not carrying something.", false);
				}
				else
				{
					bool full = actor.carryTracker.Full;
					if (!full)
					{
						Job curJob = actor.jobs.curJob;
						List<LocalTargetInfo> targetQueue = curJob.GetTargetQueue(ind);
						bool flag2 = targetQueue.NullOrEmpty<LocalTargetInfo>();
						if (!flag2)
						{
							for (int i = 0; i < targetQueue.Count; i++)
							{
								bool flag3 = GenAI.CanUseItemForWork(actor, targetQueue[i].Thing);
								if (flag3)
								{
									bool flag4 = targetQueue[i].Thing.CanStackWith(actor.carryTracker.CarriedThing);
									if (flag4)
									{
										bool flag5 = (float)(actor.Position - targetQueue[i].Thing.Position).LengthHorizontalSquared <= 64f;
										if (flag5)
										{
											int num = (actor.carryTracker.CarriedThing != null) ? actor.carryTracker.CarriedThing.stackCount : 0;
											int num2 = curJob.countQueue[i];
											num2 = Mathf.Min(num2, targetQueue[i].Thing.def.stackLimit - num);
											num2 = Mathf.Min(num2, actor.carryTracker.AvailableStackSpace(targetQueue[i].Thing.def));
											bool flag6 = num2 > 0;
											if (flag6)
											{
												curJob.count = num2;
												curJob.SetTarget(ind, targetQueue[i].Thing);
												List<int> countQueue;
												int index;
												(countQueue = curJob.countQueue)[index = i] = countQueue[index] - num2;
												bool flag7 = curJob.countQueue[i] <= 0;
												if (flag7)
												{
													curJob.countQueue.RemoveAt(i);
													targetQueue.RemoveAt(i);
												}
												actor.jobs.curDriver.JumpToToil(gotoGetTargetToil);
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			};
			return toil;
		}

		// Token: 0x04000002 RID: 2
		public float workLeft;

		// Token: 0x04000003 RID: 3
		public int billStartTick;

		// Token: 0x04000004 RID: 4
		public int ticksSpentDoingRecipeWork;

		// Token: 0x04000005 RID: 5
		public const PathEndMode GotoIngredientPathEndMode = PathEndMode.ClosestTouch;

		// Token: 0x04000006 RID: 6
		public const TargetIndex BillGiverInd = TargetIndex.A;

		// Token: 0x04000007 RID: 7
		public const TargetIndex IngredientInd = TargetIndex.B;

		// Token: 0x04000008 RID: 8
		public const TargetIndex IngredientPlaceCellInd = TargetIndex.C;
	}
}
