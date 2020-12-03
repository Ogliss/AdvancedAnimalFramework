using System;

namespace Verse.AI
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

		// Token: 0x0600002B RID: 43 RVA: 0x00003820 File Offset: 0x00001A20
		protected override float MtbHours(Pawn Pawn)
		{
			return this.mtbHours;
		}

		// Token: 0x04000009 RID: 9
		private float mtbHours = 1f;
	}
}
