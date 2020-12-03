using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200000F RID: 15
	public class JobDriver_WPConstructFinishFrame : JobDriver
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003D54 File Offset: 0x00001F54
		private Frame Frame
		{
			get
			{
				return (Frame)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003D80 File Offset: 0x00001F80
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003DB8 File Offset: 0x00001FB8
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			Toil build = new Toil();
			build.initAction = delegate()
			{
				IntVec3 root = build.actor.Position;
				Region region = build.actor.GetRegion(RegionType.Set_Passable);
				bool flag = region == null;
				if (!flag)
				{
					RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null || r.door.Open, delegate(Region r)
					{
						List<Thing> list = r.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
						for (int i = 0; i < list.Count; i++)
						{
							Pawn pawn = list[i] as Pawn;
							float num = Mathf.Clamp01(pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing));
							bool flag2 = num > 0f && pawn.Position.InHorDistOf(root, 15f * num);
							if (flag2)
							{
								pawn.HearClamor(build.actor, ClamorDefOf.Construction);
							}
						}
						return false;
					}, 15, RegionType.Set_Passable);
				}
			};
			build.tickAction = delegate()
			{
				Pawn actor = build.actor;
				Frame frame = this.Frame;
				float num = 1f;
				float value = actor.records.GetValue(RecordDefOf.ThingsConstructed);
				float num2 = value / 20f;
				bool flag = num2 < 1f;
				if (flag)
				{
					num2 = 1f;
				}
				bool flag2 = frame.Stuff != null;
				if (flag2)
				{
					num *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor, null);
				}
				float workToBuild = frame.WorkToBuild;
				bool flag3 = actor.Faction == Faction.OfPlayer && actor.RaceProps.Animal;
				if (flag3)
				{
					bool flag4 = Rand.Value < 1f - Mathf.Pow(num2, num / workToBuild);
					if (flag4)
					{
						frame.FailConstruction(actor);
						this.ReadyForNextToil();
						return;
					}
				}
				bool flag5 = frame.def.entityDefToBuild is TerrainDef;
				if (flag5)
				{
					this.Map.snowGrid.SetDepth(frame.Position, 0f);
				}
				frame.workDone += num;
				bool flag6 = frame.workDone >= workToBuild;
				if (flag6)
				{
					frame.CompleteConstruction(actor);
					this.ReadyForNextToil();
				}
			};
			build.WithEffect(() => ((Frame)build.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).ConstructionEffect, TargetIndex.A);
			build.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			build.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			build.defaultCompleteMode = ToilCompleteMode.Delay;
			build.defaultDuration = 5000;
			build.activeSkill = (() => SkillDefOf.Construction);
			yield return build;
			yield break;
		}

		// Token: 0x0400000B RID: 11
		private const int JobEndInterval = 5000;
	}
}
