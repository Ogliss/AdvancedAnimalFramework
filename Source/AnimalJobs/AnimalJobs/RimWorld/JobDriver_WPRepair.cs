using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000017 RID: 23
	public class JobDriver_WPRepair : JobDriver
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000042DC File Offset: 0x000024DC
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004314 File Offset: 0x00002514
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil repair = new Toil();
			repair.initAction = delegate()
			{
				this.ticksToNextRepair = 80f;
			};
			repair.tickAction = delegate()
			{
				Pawn actor = base.CurToil.actor;
				float statValue = actor.GetStatValue(StatDefOf.ConstructionSpeed, true);
				this.ticksToNextRepair -= statValue;
				bool flag = this.ticksToNextRepair <= 0f;
				if (flag)
				{
					this.ticksToNextRepair += 20f;
					Thing targetThingA = base.TargetThingA;
					int hitPoints = targetThingA.HitPoints;
					targetThingA.HitPoints = hitPoints + 1;
					base.TargetThingA.HitPoints = Mathf.Min(base.TargetThingA.HitPoints, base.TargetThingA.MaxHitPoints);
					base.Map.listerBuildingsRepairable.Notify_BuildingRepaired((Building)base.TargetThingA);
					bool flag2 = base.TargetThingA.HitPoints == base.TargetThingA.MaxHitPoints;
					if (flag2)
					{
						actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
					}
				}
			};
			repair.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			repair.WithEffect(base.TargetThingA.def.repairEffect, TargetIndex.A);
			repair.defaultCompleteMode = ToilCompleteMode.Never;
			yield return repair;
			yield break;
		}

		// Token: 0x04000015 RID: 21
		private const float WarmupTicks = 80f;

		// Token: 0x04000016 RID: 22
		private const float TicksBetweenRepairs = 20f;

		// Token: 0x04000017 RID: 23
		protected float ticksToNextRepair;
	}
}
