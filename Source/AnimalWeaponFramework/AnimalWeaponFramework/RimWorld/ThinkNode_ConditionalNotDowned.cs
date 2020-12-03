using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x0200000E RID: 14
	public class ThinkNode_ConditionalNotDowned : ThinkNode_Conditional
	{
		// Token: 0x06000033 RID: 51 RVA: 0x00003A60 File Offset: 0x00001C60
		protected override bool Satisfied(Pawn pawn)
		{
			bool downed = pawn.Downed;
			return !downed;
		}
	}
}
