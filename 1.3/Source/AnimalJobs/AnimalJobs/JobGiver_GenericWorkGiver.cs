using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	public class JobGiver_GenericWorkGiver : ThinkNode
	{
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_GenericWorkGiver jobGiver_GenericWorkGiver = (JobGiver_GenericWorkGiver)base.DeepCopy(resolve);
			jobGiver_GenericWorkGiver.workGiverDef = this.workGiverDef;
			return jobGiver_GenericWorkGiver;
		}

		public override float GetPriority(Pawn pawn)
		{
			float result;
			if (pawn.workSettings == null || !pawn.workSettings.EverWork) result = 9f;
			else
			{
				TimeAssignmentDef timeAssignmentDef = (pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything;
				if (timeAssignmentDef == TimeAssignmentDefOf.Anything) result = 5.5f;
				else
				{
					if (timeAssignmentDef == TimeAssignmentDefOf.Work) result = 9f;
					else
					{
						if (timeAssignmentDef == TimeAssignmentDefOf.Sleep) result = 3f;
						else
						{
							if (timeAssignmentDef != TimeAssignmentDefOf.Joy) throw new NotImplementedException(); result = 2f;
						}
					}
				}
			}
			return result;
		}

		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			if (pawn.mindState.priorityWork.IsPrioritized)
			{
				List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkGiver.workType.workGiversByPriority;
				for (int i = 0; i < workGiversByPriority.Count; i++)
				{
					WorkGiver worker = workGiversByPriority[i].Worker;
					Job job = this.GiverTryGiveJobPrioritized(pawn, worker, pawn.mindState.priorityWork.Cell);
					if (job != null)
					{
						job.playerForced = true;
						return new ThinkResult(job, this, new JobTag?(workGiversByPriority[i].tagToGive), false);
					}
				}
				pawn.mindState.priorityWork.Clear();
			}
			int num = -999;
			TargetInfo targetInfo = TargetInfo.Invalid;
			WorkGiver_Scanner workGiver_Scanner = null;
			if (this.workGiverDef != null)
			{
				WorkGiver worker2 = this.workGiverDef.Worker;
				if (worker2.def.priorityInType != num && targetInfo.IsValid)
				{
					return ThinkResult.NoJob;
				}
				if (this.PawnCanUseWorkGiver(pawn, worker2))
				{
					try
					{
						Job job2 = worker2.NonScanJob(pawn);
						if (job2 != null) return new ThinkResult(job2, this, new JobTag?(this.workGiverDef.tagToGive), false);
						WorkGiver_Scanner scanner = worker2 as WorkGiver_Scanner;
						if (scanner != null)
						{
							if (worker2.def.scanThings)
							{
								Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
								IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
								bool prioritized = scanner.Prioritized;
								Thing thing;
								if (prioritized)
								{
									IEnumerable<Thing> enumerable2 = enumerable;
									if (enumerable2 == null)
									{
										enumerable2 = pawn.Map.listerThings.ThingsMatching(scanner.PotentialWorkThingRequest);
									}
									Predicate<Thing> validator = predicate;
									thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, enumerable2, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, (Thing x) => scanner.GetPriority(pawn, x));
								}
								else
								{
									Predicate<Thing> validator2 = predicate;
									bool forceAllowGlobalSearch = enumerable != null;
									thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, scanner.PotentialWorkThingRequest, scanner.PathEndMode, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator2, enumerable, 0, scanner.MaxRegionsToScanBeforeGlobalSearch, forceAllowGlobalSearch, RegionType.Set_Passable, false);
								}
								if (thing != null)
								{
									targetInfo = thing;
									workGiver_Scanner = scanner;
								}
							}
							if (worker2.def.scanCells)
							{
								IntVec3 position = pawn.Position;
								float num2 = 99999f;
								float num3 = float.MinValue;
								bool prioritized2 = scanner.Prioritized;
								foreach (IntVec3 intVec in scanner.PotentialWorkCellsGlobal(pawn))
								{
									bool flag = false;
									float num4 = (float)(intVec - position).LengthHorizontalSquared;
									if (prioritized2)
									{
										if (!intVec.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, intVec, false) && pawn.CanReach(intVec, PathEndMode.Touch, Danger.None))
										{
											float priority = scanner.GetPriority(pawn, intVec);
											if (priority > num3 || (priority == num3 && num4 < num2))
											{
												flag = true;
												num3 = priority;
											}
										}
									}
									else
									{
										if (num4 < num2 && !intVec.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, intVec, false))
										{
											flag = true;
										}
									}
									if (flag)
									{
										targetInfo = new TargetInfo(intVec, pawn.Map, false);
										workGiver_Scanner = scanner;
										num2 = num4;
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						Log.Message(string.Concat(new object[]
						{
							pawn,
							" threw exception DONT WANT LAH in WorkGiver ",
							worker2.def.defName,
							": ",
							ex.ToString()
						}));
					}
					finally
					{
					}
					if (targetInfo.IsValid)
					{
						Job job3;
						if (targetInfo.HasThing)
						{
							job3 = workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing, false);
						}
						else
						{
							job3 = workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell, false);
						}
						if (job3 != null)
						{
							return new ThinkResult(job3, this, new JobTag?(this.workGiverDef.tagToGive), false);
						}
						Log.ErrorOnce(string.Concat(new object[]
						{
							workGiver_Scanner,
							" provided target ",
							targetInfo,
							" but yielded no actual job for pawn ",
							pawn,
							". The CanGiveJob and JobOnX methods may not be synchronized."
						}), 6112651);
					}
					num = worker2.def.priorityInType;
				}
			}
			return ThinkResult.NoJob;
		}

		private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
            if (!pawn.RaceProps.Animal)
            {
				Log.Warning(pawn.Name+" tried to use "+this + " to give " +giver+" but "+ pawn.Name +"is not an animal");
				return false;
            }
			return !giver.ShouldSkip(pawn, false) && giver.MissingRequiredCapacity(pawn) == null;
		}

		private Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
		{
			Job result;
			if (!this.PawnCanUseWorkGiver(pawn, giver))
			{
				result = null;
			}
			else
			{
				try
				{
					Job job = giver.NonScanJob(pawn);
					if (job != null) return job;
					WorkGiver_Scanner scanner = giver as WorkGiver_Scanner;
					if (scanner != null)
					{
						if (giver.def.scanThings)
						{
							Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
							List<Thing> thingList = cell.GetThingList(pawn.Map);
							for (int i = 0; i < thingList.Count; i++)
							{
								Thing thing = thingList[i];
								if (scanner.PotentialWorkThingRequest.Accepts(thing) && predicate(thing))
								{
									Job job2 = scanner.JobOnThing(pawn, thing, false);
									if (job2 != null) job2.workGiverDef = giver.def;
									return job2;
								}
							}
						}
						if (giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell, false))
						{
							Job job3 = scanner.JobOnCell(pawn, cell, false);
							if (job3 != null) job3.workGiverDef = giver.def;
							return job3;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						pawn,
						" threw exception in GiverTryGiveJobTargeted on WorkGiver ",
						giver.def.defName,
						": ",
						ex.ToString()
					}));
				}
				result = null;
			}
			return result;
		}

		public WorkGiverDef workGiverDef;
	}
}
