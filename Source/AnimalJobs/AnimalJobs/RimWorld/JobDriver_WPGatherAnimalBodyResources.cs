using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000012 RID: 18
	public abstract class JobDriver_WPGatherAnimalBodyResources : JobDriver
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000042 RID: 66
		protected abstract float WorkTotal { get; }

		// Token: 0x06000043 RID: 67
		protected abstract CompHasGatherableBodyResource GetComp(Pawn animal);

		// Token: 0x06000044 RID: 68 RVA: 0x0000405F File Offset: 0x0000225F
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.gatherProgress, "gatherProgress", 0f, false);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004080 File Offset: 0x00002280
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo target = this.job.GetTarget(TargetIndex.A);
			Job job = this.job;
			return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000040B9 File Offset: 0x000022B9
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnDowned(TargetIndex.A);
			this.FailOnNotCasualInterruptible(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil wait = new Toil();
			wait.initAction = delegate()
			{
				Pawn actor = wait.actor;
				Pawn pawn = (Pawn)wait.actor.CurJob.GetTarget(TargetIndex.A).Thing;
				actor.pather.StopDead();
				PawnUtility.ForceWait(pawn, 15000, null, true);
			};
			wait.tickAction = delegate()
			{
				Pawn actor = wait.actor;
				this.gatherProgress += actor.GetStatValue(StatDefOf.AnimalGatherSpeed, true);
				bool flag = this.gatherProgress >= this.WorkTotal;
				if (flag)
				{
					this.GetComp((Pawn)((Thing)this.job.GetTarget(TargetIndex.A))).Gathered(this.pawn);
					actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				}
			};
			wait.AddFinishAction(delegate
			{
				Pawn pawn = (Pawn)wait.actor.CurJob.GetTarget(TargetIndex.A).Thing;
				bool flag = pawn.jobs.curJob.def == JobDefOf.Wait_MaintainPosture;
				if (flag)
				{
					pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
				}
			});
			wait.FailOnDespawnedOrNull(TargetIndex.A);
			wait.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			wait.AddEndCondition(delegate
			{
				bool flag = !this.GetComp((Pawn)((Thing)this.job.GetTarget(TargetIndex.A))).ActiveAndFull;
				JobCondition result;
				if (flag)
				{
					result = JobCondition.Incompletable;
				}
				else
				{
					result = JobCondition.Ongoing;
				}
				return result;
			});
			wait.defaultCompleteMode = ToilCompleteMode.Never;
			wait.WithProgressBar(TargetIndex.A, () => this.gatherProgress / this.WorkTotal, false, -0.5f);
			yield return wait;
			yield break;
		}

		// Token: 0x04000010 RID: 16
		private float gatherProgress;

		// Token: 0x04000011 RID: 17
		protected const TargetIndex AnimalInd = TargetIndex.A;
	}
}
