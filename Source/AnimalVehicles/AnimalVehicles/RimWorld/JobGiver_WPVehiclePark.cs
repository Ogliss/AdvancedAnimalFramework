using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000006 RID: 6
	public class JobGiver_WPVehiclePark : ThinkNode
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000026F4 File Offset: 0x000008F4
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			bool drafted = pawn.playerSettings.Master.Drafted;
			ThinkResult result;
			if (drafted)
			{
				result = ThinkResult.NoJob;
			}
			else
			{
				bool flag = pawn.mindState.IsIdle && !pawn.InBed();
				if (flag)
				{
					Job job = new Job(JobDefOf.Wait);
					result = new ThinkResult(job, this, null, false);
				}
				else
				{
					result = ThinkResult.NoJob;
				}
			}
			return result;
		}
	}
}
