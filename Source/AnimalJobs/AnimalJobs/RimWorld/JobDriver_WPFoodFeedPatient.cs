using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000011 RID: 17
	public class JobDriver_WPFoodFeedPatient : JobDriver
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003E54 File Offset: 0x00002054
		protected Thing Food
		{
			get
			{
				return this.job.targetA.Thing;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00003E78 File Offset: 0x00002078
		protected Pawn Deliveree
		{
			get
			{
				return (Pawn)this.job.targetB.Thing;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003EA0 File Offset: 0x000020A0
		public override string GetReport()
		{
			bool flag = this.job.GetTarget(TargetIndex.A).Thing is Building_NutrientPasteDispenser && this.Deliveree != null;
			string result;
			if (flag)
			{
				result = JobUtility.GetResolvedJobReportRaw(this.job.def.reportString, ThingDefOf.MealNutrientPaste.label, ThingDefOf.MealNutrientPaste, this.Deliveree.LabelShort, this.Deliveree, "", "");
			}
			else
			{
				result = base.GetReport();
			}
			return result;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003F28 File Offset: 0x00002128
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			bool flag = this.pawn == null || this.Deliveree == null || this.job == null || this.Food == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.pawn.Reserve(this.Deliveree, this.job, 1, -1, null, true);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !(base.TargetThingA is Building_NutrientPasteDispenser);
					if (flag3)
					{
						int maxAmountToPickup = FoodUtility.GetMaxAmountToPickup(this.Food, this.pawn, this.job.count);
						bool flag4 = !this.pawn.Reserve(this.Food, this.job, 1, maxAmountToPickup, null, true);
						if (flag4)
						{
							return false;
						}
						this.job.count = maxAmountToPickup;
					}
					bool flag5 = this.pawn.Reserve(this.Deliveree, this.job, 1, -1, null, true);
					result = flag5;
				}
			}
			return result;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004036 File Offset: 0x00002236
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.B);
			this.FailOn(() => !FoodUtility.ShouldBeFedBySomeone(this.Deliveree));
			bool flag = base.TargetThingA is Building_NutrientPasteDispenser;
			if (flag)
			{
				yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell).FailOnForbidden(TargetIndex.A);
				yield return Toils_Ingest.TakeMealFromDispenser(TargetIndex.A, this.pawn);
			}
			else
			{
				yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnForbidden(TargetIndex.A);
				yield return Toils_Ingest.PickupIngestible(TargetIndex.A, this.Deliveree);
			}
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
			yield return Toils_Ingest.ChewIngestible(this.Deliveree, 1.5f, TargetIndex.A, TargetIndex.None).FailOnCannotTouch(TargetIndex.B, PathEndMode.Touch);
			yield return Toils_Ingest.FinalizeIngest(this.Deliveree, TargetIndex.A);
			yield break;
		}

		// Token: 0x0400000D RID: 13
		private const TargetIndex FoodSourceInd = TargetIndex.A;

		// Token: 0x0400000E RID: 14
		private const TargetIndex DelivereeInd = TargetIndex.B;

		// Token: 0x0400000F RID: 15
		private const float FeedDurationMultiplier = 1.5f;
	}
}
