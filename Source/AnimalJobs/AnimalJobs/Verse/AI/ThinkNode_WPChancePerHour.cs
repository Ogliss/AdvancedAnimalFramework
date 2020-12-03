using System;

namespace Verse.AI
{
	// Token: 0x0200000C RID: 12
	public abstract class ThinkNode_WPChancePerHour : ThinkNode_Priority
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000036D4 File Offset: 0x000018D4
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			bool flag = Find.TickManager.TicksGame < this.GetLastTryTick(pawn) + 10;
			ThinkResult result;
			if (flag)
			{
				result = ThinkResult.NoJob;
			}
			else
			{
				this.SetLastTryTick(pawn, Find.TickManager.TicksGame);
				float num = this.MtbHours(pawn);
				bool flag2 = num <= 0f;
				if (flag2)
				{
					result = ThinkResult.NoJob;
				}
				else
				{
					Rand.PushState();
					int salt = Gen.HashCombineInt(base.UniqueSaveKey, 26504059);
					Rand.Seed = pawn.RandSeedForHour(salt);
					bool flag3 = Rand.MTBEventOccurs(num, 250f, 250f);
					Rand.PopState();
					bool flag4 = flag3;
					if (flag4)
					{
						result = base.TryIssueJobPackage(pawn, jobParams);
					}
					else
					{
						result = ThinkResult.NoJob;
					}
				}
			}
			return result;
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
