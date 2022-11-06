using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobDriver_WPConstructFinishFrame : JobDriver
	{
		private Frame Frame
		{
			get
			{
				return (Frame)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			Pawn pawn = this.pawn;
			LocalTargetInfo targetA = this.job.targetA;
			Job job = this.job;
			return pawn.Reserve(targetA, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			Toil build = new Toil();
			build.initAction = delegate()
			{
				IntVec3 root = build.actor.Position;
				Region region = build.actor.GetRegion(RegionType.Set_Passable);
				if (region != null)
				{
					RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null || r.door.Open, delegate(Region r)
					{
						List<Thing> list = r.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
						for (int i = 0; i < list.Count; i++)
						{
							Pawn pawn = list[i] as Pawn;
							float num = Mathf.Clamp01(pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing));
							if (num > 0f && pawn.Position.InHorDistOf(root, 15f * num))
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
				if (num2 < 1f)
				{
					num2 = 1f;
				}
				if (frame.Stuff != null)
				{
					num *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor, null);
				}
				float workToBuild = frame.WorkToBuild;
				if (actor.Faction == Faction.OfPlayer && actor.RaceProps.Animal)
				{
					if (Rand.Value < 1f - Mathf.Pow(num2, num / workToBuild))
					{
						frame.FailConstruction(actor);
						this.ReadyForNextToil();
						return;
					}
				}
				if (frame.def.entityDefToBuild is TerrainDef)
				{
					this.Map.snowGrid.SetDepth(frame.Position, 0f);
				}
				frame.workDone += num;
				bool flag6 = frame.workDone >= workToBuild;
				if (frame.workDone >= workToBuild)
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

		private const int JobEndInterval = 5000;
	}
}
