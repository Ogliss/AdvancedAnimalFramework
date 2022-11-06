using System;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	// Token: 0x0200000C RID: 12
	public abstract class ThinkNode_WPChancePerHour : ThinkNode_Priority
	{
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            if (Find.TickManager.TicksGame < this.GetLastTryTick(pawn) + 2500)
            {
                return ThinkResult.NoJob;
            }
            this.SetLastTryTick(pawn, Find.TickManager.TicksGame);
            float num = this.MtbHours(pawn);
            if (num <= 0f)
            {
                return ThinkResult.NoJob;
            }
            Rand.PushState();
            int salt = Gen.HashCombineInt(base.UniqueSaveKey, 26504059);
            Rand.Seed = pawn.RandSeedForHour(salt);
            bool flag = Rand.MTBEventOccurs(num, 2500f, 2500f);
            Rand.PopState();
            if (flag)
            {
                return base.TryIssueJobPackage(pawn, jobParams);
            }
            return ThinkResult.NoJob;

		}

		// Token: 0x06000026 RID: 38
		protected abstract float MtbHours(Pawn pawn);

		// Token: 0x06000027 RID: 39 RVA: 0x00003798 File Offset: 0x00001998
		private int GetLastTryTick(Pawn pawn)
		{
			int num;
			bool flag = pawn.mindState.thinkData.TryGetValue(base.UniqueSaveKey, out num);
			int result;
			if (flag)
			{
				result = num;
			}
			else
			{
				result = -99999;
			}
			return result;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000037D0 File Offset: 0x000019D0
		private void SetLastTryTick(Pawn pawn, int val)
		{
			pawn.mindState.thinkData[base.UniqueSaveKey] = val;
		}
	}
}
