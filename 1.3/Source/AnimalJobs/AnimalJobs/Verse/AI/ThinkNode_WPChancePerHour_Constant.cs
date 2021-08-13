using System;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
	// Token: 0x0200000D RID: 13
	public class ThinkNode_WPChancePerHour_Constant : ThinkNode_WPChancePerHour
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000037F4 File Offset: 0x000019F4
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			ThinkNode_WPChancePerHour_Constant thinkNode_WPChancePerHour_Constant = (ThinkNode_WPChancePerHour_Constant)base.DeepCopy(resolve);
			thinkNode_WPChancePerHour_Constant.mtbHours = this.mtbHours;
			return thinkNode_WPChancePerHour_Constant;
		}

		protected override float MtbHours(Pawn pawn)
		{
			return this.mtbHours;
		}

        // Token: 0x04000009 RID: 9
        private float mtbHours = 1f;
	}
}
