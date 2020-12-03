using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200001D RID: 29
	public class WorkGiver_WPConstructDeliverResourcesToBlueprints : WorkGiver_WPConstructDeliverResources
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00005070 File Offset: 0x00003270
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Blueprint);
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005088 File Offset: 0x00003288
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool flag = t.Faction != pawn.Faction;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Blueprint blueprint = t as Blueprint;
				bool flag2 = blueprint == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = WPGenConstruct.FirstBlockingThing(blueprint, pawn) != null;
					if (flag3)
					{
						result = WPGenConstruct.HandleBlockingThingJob(blueprint, pawn, false);
					}
					else
					{
						bool flag4 = !WPGenConstruct.CanConstruct(blueprint, pawn, forced);
						if (flag4)
						{
							result = null;
						}
						else
						{
							bool flag5 = !pawn.CanReserve(blueprint, 1, -1, null, false);
							if (flag5)
							{
								result = null;
							}
							else
							{
								bool flag6 = !pawn.CanReserve(t, 1, -1, null, false);
								if (flag6)
								{
									result = null;
								}
								else
								{
									bool flag7 = WorkGiver_WPConstructDeliverResources.ShouldRemoveExistingFloorFirst(pawn, blueprint);
									if (flag7)
									{
										result = null;
									}
									else
									{
										Job job = base.RemoveExistingFloorJob(pawn, blueprint);
										bool flag8 = job != null;
										if (flag8)
										{
											result = job;
										}
										else
										{
											Job job2 = base.ResourceDeliverJobFor(pawn, blueprint, true);
											bool flag9 = job2 != null;
											if (flag9)
											{
												result = job2;
											}
											else
											{
												bool flag10 = this.def.workType != WorkTypeDefOf.Hauling;
												if (flag10)
												{
													Job job3 = this.NoCostFrameMakeJobFor(pawn, blueprint);
													bool flag11 = job3 != null;
													if (flag11)
													{
														return job3;
													}
												}
												result = null;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000051D4 File Offset: 0x000033D4
		private Job NoCostFrameMakeJobFor(Pawn pawn, IConstructible c)
		{
			bool flag = c is Blueprint_Install;
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = c is Blueprint && c.MaterialsNeeded().Count == 0;
				if (flag2)
				{
					Job job = JobMaker.MakeJob(JobDefOf.PlaceNoCostFrame);
					job.targetA = (Thing)c;
					result = job;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}
	}
}
