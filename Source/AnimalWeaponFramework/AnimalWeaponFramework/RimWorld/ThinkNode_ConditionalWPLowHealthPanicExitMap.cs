using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000011 RID: 17
	public class ThinkNode_ConditionalWPLowHealthPanicExitMap : ThinkNode_Conditional
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00003C90 File Offset: 0x00001E90
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			ThinkNode_ConditionalWPLowHealthPanicExitMap thinkNode_ConditionalWPLowHealthPanicExitMap = (ThinkNode_ConditionalWPLowHealthPanicExitMap)base.DeepCopy(resolve);
			thinkNode_ConditionalWPLowHealthPanicExitMap.threshold = this.threshold;
			return thinkNode_ConditionalWPLowHealthPanicExitMap;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003CBC File Offset: 0x00001EBC
		protected override bool Satisfied(Pawn pawn)
		{
			bool flag = pawn.health.summaryHealth.SummaryHealthPercent < this.threshold;
			bool result;
			if (flag)
			{
				pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee, null, false, false, null, false);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x04000017 RID: 23
		private float threshold;
	}
}
