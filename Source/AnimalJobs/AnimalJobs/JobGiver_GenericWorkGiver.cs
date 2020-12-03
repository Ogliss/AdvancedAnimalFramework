using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	// Token: 0x02000002 RID: 2
	public class JobGiver_GenericWorkGiver : ThinkNode
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_GenericWorkGiver jobGiver_GenericWorkGiver = (JobGiver_GenericWorkGiver)base.DeepCopy(resolve);
			jobGiver_GenericWorkGiver.workGiverDef = this.workGiverDef;
			return jobGiver_GenericWorkGiver;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000207C File Offset: 0x0000027C
		public override float GetPriority(Pawn pawn)
		{
			bool flag = pawn.workSettings == null || !pawn.workSettings.EverWork;
			float result;
			if (flag)
			{
				result = 9f;
			}
			else
			{
				TimeAssignmentDef timeAssignmentDef = (pawn.timetable != null) ? pawn.timetable.CurrentAssignment : TimeAssignmentDefOf.Anything;
				bool flag2 = timeAssignmentDef == TimeAssignmentDefOf.Anything;
				if (flag2)
				{
					result = 5.5f;
				}
				else
				{
					bool flag3 = timeAssignmentDef == TimeAssignmentDefOf.Work;
					if (flag3)
					{
						result = 9f;
					}
					else
					{
						bool flag4 = timeAssignmentDef == TimeAssignmentDefOf.Sleep;
						if (flag4)
						{
							result = 3f;
						}
						else
						{
							bool flag5 = timeAssignmentDef == TimeAssignmentDefOf.Joy;
							if (!flag5)
							{
								throw new NotImplementedException();
							}
							result = 2f;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000212C File Offset: 0x0000032C
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			bool isPrioritized = pawn.mindState.priorityWork.IsPrioritized;
			if (isPrioritized)
			{
				List<WorkGiverDef> workGiversByPriority = pawn.mindState.priorityWork.WorkGiver.workType.workGiversByPriority;
				for (int i = 0; i < workGiversByPriority.Count; i++)
				{
					WorkGiver worker = workGiversByPriority[i].Worker;
					Job job = this.GiverTryGiveJobPrioritized(pawn, worker, pawn.mindState.priorityWork.Cell);
					bool flag = job != null;
					if (flag)
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
			bool flag2 = this.workGiverDef != null;
			if (flag2)
			{
				WorkGiver worker2 = this.workGiverDef.Worker;
				bool flag3 = worker2.def.priorityInType != num && targetInfo.IsValid;
				if (flag3)
				{
					return ThinkResult.NoJob;
				}
				bool flag4 = this.PawnCanUseWorkGiver(pawn, worker2);
				if (flag4)
				{
					try
					{
						Job job2 = worker2.NonScanJob(pawn);
						bool flag5 = job2 != null;
						if (flag5)
						{
							return new ThinkResult(job2, this, new JobTag?(this.workGiverDef.tagToGive), false);
						}
						WorkGiver_Scanner scanner = worker2 as WorkGiver_Scanner;
						bool flag6 = scanner != null;
						if (flag6)
						{
							bool scanThings = worker2.def.scanThings;
							if (scanThings)
							{
								Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
								IEnumerable<Thing> enumerable = scanner.PotentialWorkThingsGlobal(pawn);
								bool prioritized = scanner.Prioritized;
								Thing thing;
								if (prioritized)
								{
									IEnumerable<Thing> enumerable2 = enumerable;
									bool flag7 = enumerable2 == null;
									if (flag7)
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
								bool flag8 = thing != null;
								if (flag8)
								{
									targetInfo = thing;
									workGiver_Scanner = scanner;
								}
							}
							bool scanCells = worker2.def.scanCells;
							if (scanCells)
							{
								IntVec3 position = pawn.Position;
								float num2 = 99999f;
								float num3 = float.MinValue;
								bool prioritized2 = scanner.Prioritized;
								foreach (IntVec3 intVec in scanner.PotentialWorkCellsGlobal(pawn))
								{
									bool flag9 = false;
									float num4 = (float)(intVec - position).LengthHorizontalSquared;
									bool flag10 = prioritized2;
									if (flag10)
									{
										bool flag11 = !intVec.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, intVec, false) && pawn.CanReach(intVec, PathEndMode.Touch, Danger.None);
										if (flag11)
										{
											float priority = scanner.GetPriority(pawn, intVec);
											bool flag12 = priority > num3 || (priority == num3 && num4 < num2);
											if (flag12)
											{
												flag9 = true;
												num3 = priority;
											}
										}
									}
									else
									{
										bool flag13 = num4 < num2 && !intVec.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, intVec, false);
										if (flag13)
										{
											flag9 = true;
										}
									}
									bool flag14 = flag9;
									if (flag14)
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
						}), false);
					}
					finally
					{
					}
					bool isValid = targetInfo.IsValid;
					if (isValid)
					{
						bool hasThing = targetInfo.HasThing;
						Job job3;
						if (hasThing)
						{
							job3 = workGiver_Scanner.JobOnThing(pawn, targetInfo.Thing, false);
						}
						else
						{
							job3 = workGiver_Scanner.JobOnCell(pawn, targetInfo.Cell, false);
						}
						bool flag15 = job3 != null;
						if (flag15)
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
						}), 6112651, false);
					}
					num = worker2.def.priorityInType;
				}
			}
			return ThinkResult.NoJob;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000027C4 File Offset: 0x000009C4
		private bool PawnCanUseWorkGiver(Pawn pawn, WorkGiver giver)
		{
			return !giver.ShouldSkip(pawn, false) && giver.MissingRequiredCapacity(pawn) == null;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000027F0 File Offset: 0x000009F0
		private Job GiverTryGiveJobPrioritized(Pawn pawn, WorkGiver giver, IntVec3 cell)
		{
			bool flag = !this.PawnCanUseWorkGiver(pawn, giver);
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				try
				{
					Job job = giver.NonScanJob(pawn);
					bool flag2 = job != null;
					if (flag2)
					{
						return job;
					}
					WorkGiver_Scanner scanner = giver as WorkGiver_Scanner;
					bool flag3 = scanner != null;
					if (flag3)
					{
						bool scanThings = giver.def.scanThings;
						if (scanThings)
						{
							Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && scanner.HasJobOnThing(pawn, t, false);
							List<Thing> thingList = cell.GetThingList(pawn.Map);
							for (int i = 0; i < thingList.Count; i++)
							{
								Thing thing = thingList[i];
								bool flag4 = scanner.PotentialWorkThingRequest.Accepts(thing) && predicate(thing);
								if (flag4)
								{
									Job job2 = scanner.JobOnThing(pawn, thing, false);
									bool flag5 = job2 != null;
									if (flag5)
									{
										job2.workGiverDef = giver.def;
									}
									return job2;
								}
							}
						}
						bool flag6 = giver.def.scanCells && !cell.IsForbidden(pawn) && scanner.HasJobOnCell(pawn, cell, false);
						if (flag6)
						{
							Job job3 = scanner.JobOnCell(pawn, cell, false);
							bool flag7 = job3 != null;
							if (flag7)
							{
								job3.workGiverDef = giver.def;
							}
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
					}), false);
				}
				result = null;
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		public WorkGiverDef workGiverDef;
	}
}
