using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000016 RID: 22
	public abstract class JobDriver_WPRemoveBuilding : JobDriver
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00004204 File Offset: 0x00002404
		protected Thing Target
		{
			get
			{
				return this.job.targetA.Thing;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00004228 File Offset: 0x00002428
		protected Building Building
		{
			get
			{
				return (Building)this.Target.GetInnerIfMinified();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000055 RID: 85
		protected abstract DesignationDef Designation { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000056 RID: 86
		protected abstract int TotalNeededWork { get; }

		// Token: 0x06000057 RID: 87 RVA: 0x0000424A File Offset: 0x0000244A
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.workLeft, "workLeft", 0f, false);
			Scribe_Values.Look<float>(ref this.totalNeededWork, "totalNeededWork", 0f, false);
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004284 File Offset: 0x00002484
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo target = this.Target;
			Job job = this.job;
			return pawn.Reserve(target, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000042BC File Offset: 0x000024BC
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnThingMissingDesignation(TargetIndex.A, this.Designation);
			this.FailOnForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil doWork = new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			doWork.initAction = delegate()
			{
				this.totalNeededWork = (float)this.TotalNeededWork;
				this.workLeft = this.totalNeededWork;
			};
			doWork.tickAction = delegate()
			{
				bool flag = this.pawn.GetStatValue(StatDefOf.ConstructionSpeed, true) >= 1f;
				float num;
				if (flag)
				{
					num = this.pawn.GetStatValue(StatDefOf.ConstructionSpeed, true);
				}
				else
				{
					num = 1f;
				}
				this.workLeft -= num;
				this.TickAction();
				bool flag2 = this.workLeft <= 0f;
				if (flag2)
				{
					doWork.actor.jobs.curDriver.ReadyForNextToil();
				}
			};
			doWork.defaultCompleteMode = ToilCompleteMode.Never;
			doWork.WithProgressBar(TargetIndex.A, () => 1f - this.workLeft / this.totalNeededWork, false, -0.5f);
			yield return doWork;
			yield return new Toil
			{
				initAction = delegate()
				{
					this.FinishedRemoving();
					this.Map.designationManager.RemoveAllDesignationsOn(this.Target, false);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
			yield break;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000042CC File Offset: 0x000024CC
		protected virtual void FinishedRemoving()
		{
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000042CF File Offset: 0x000024CF
		protected virtual void TickAction()
		{
		}

		// Token: 0x04000013 RID: 19
		private float workLeft;

		// Token: 0x04000014 RID: 20
		private float totalNeededWork;
	}
}
