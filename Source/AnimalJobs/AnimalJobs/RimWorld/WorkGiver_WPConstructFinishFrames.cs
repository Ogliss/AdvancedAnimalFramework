using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200001F RID: 31
	public class WorkGiver_WPConstructFinishFrames : WorkGiver_Scanner
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00005324 File Offset: 0x00003524
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.BuildingFrame);
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005340 File Offset: 0x00003540
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool flag = t.Faction != pawn.Faction;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Frame frame = t as Frame;
				bool flag2 = frame == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = frame.MaterialsNeeded().Count > 0;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = !pawn.CanReserve(frame, 1, -1, null, false);
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = !pawn.CanReserve(t, 1, -1, null, false);
							result = !flag5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000053D8 File Offset: 0x000035D8
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Frame t2 = t as Frame;
			bool flag = !pawn.CanReserveAndReach(t, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false);
			Job result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new Job(WPJobDefOf.WPFinishFrame, t2);
			}
			return result;
		}
	}
}
