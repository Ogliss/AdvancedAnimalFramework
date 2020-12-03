using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000015 RID: 21
	public class JobDriver_WPPlantSow : JobDriver
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00004164 File Offset: 0x00002364
		private Plant Plant
		{
			get
			{
				return (Plant)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000418F File Offset: 0x0000238F
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.sowWorkDone, "sowWorkDone", 0f, false);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000041B0 File Offset: 0x000023B0
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000041E8 File Offset: 0x000023E8
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOn(() => PlantUtility.AdjacentSowBlocker(this.job.plantDefToSow, this.TargetA.Cell, this.Map) != null).FailOn(() => !this.job.plantDefToSow.CanEverPlantAt(this.TargetLocA, this.Map));
			Toil sowToil = new Toil();
			sowToil.initAction = delegate()
			{
				this.TargetThingA = GenSpawn.Spawn(this.job.plantDefToSow, this.TargetLocA, this.Map, WipeMode.Vanish);
				this.pawn.Reserve(this.TargetThingA, sowToil.actor.CurJob, 1, -1, null, true);
				Plant plant = (Plant)this.TargetThingA;
				plant.Growth = 0f;
				plant.sown = true;
			};
			sowToil.tickAction = delegate()
			{
				Pawn actor = sowToil.actor;
				float num = 1f;
				Plant plant = this.Plant;
				bool flag = plant.LifeStage > PlantLifeStage.Sowing;
				if (flag)
				{
					Log.Error(this?.ToString() + " getting sowing work while not in Sowing life stage.", false);
				}
				this.sowWorkDone += num;
				bool flag2 = this.sowWorkDone >= plant.def.plant.sowWork;
				if (flag2)
				{
					plant.Growth = 0.05f;
					this.Map.mapDrawer.MapMeshDirty(plant.Position, MapMeshFlag.Things);
					actor.records.Increment(RecordDefOf.PlantsSown);
					this.ReadyForNextToil();
				}
			};
			sowToil.defaultCompleteMode = ToilCompleteMode.Never;
			sowToil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			sowToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			sowToil.WithEffect(EffecterDefOf.Sow, TargetIndex.A);
			sowToil.WithProgressBar(TargetIndex.A, () => this.sowWorkDone / this.Plant.def.plant.sowWork, true, -0.5f);
			sowToil.PlaySustainerOrSound(() => SoundDefOf.Interact_Sow);
			sowToil.AddFinishAction(delegate
			{
				bool flag = this.TargetThingA != null;
				if (flag)
				{
					Plant plant = (Plant)sowToil.actor.CurJob.GetTarget(TargetIndex.A).Thing;
					bool flag2 = this.sowWorkDone < plant.def.plant.sowWork && !this.TargetThingA.Destroyed;
					if (flag2)
					{
						this.TargetThingA.Destroy(DestroyMode.Vanish);
					}
				}
			});
			yield return sowToil;
			yield break;
		}

		// Token: 0x04000012 RID: 18
		private float sowWorkDone;
	}
}
