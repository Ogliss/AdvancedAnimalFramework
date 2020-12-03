using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200001E RID: 30
	public class WorkGiver_WPConstructDeliverResourcesToFrames : WorkGiver_WPConstructDeliverResources
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00005240 File Offset: 0x00003440
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.BuildingFrame);
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000525C File Offset: 0x0000345C
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
				Frame frame = t as Frame;
				bool flag2 = frame == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = WPGenConstruct.FirstBlockingThing(frame, pawn) != null;
					if (flag3)
					{
						result = WPGenConstruct.HandleBlockingThingJob(frame, pawn, false);
					}
					else
					{
						bool flag4 = !WPGenConstruct.CanConstruct(frame, pawn, forced);
						if (flag4)
						{
							result = null;
						}
						else
						{
							bool flag5 = !pawn.CanReserve(t, 1, -1, null, false);
							if (flag5)
							{
								result = null;
							}
							else
							{
								bool flag6 = !pawn.CanReserveAndReach(t, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced);
								if (flag6)
								{
									result = null;
								}
								else
								{
									result = base.ResourceDeliverJobFor(pawn, frame, true);
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
