using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000013 RID: 19
	public class ThinkNode_ConditionalWPNotOfPlayerFaction : ThinkNode_Conditional
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00003D84 File Offset: 0x00001F84
		protected override bool Satisfied(Pawn pawn)
		{
			bool flag = !pawn.Spawned || pawn.Downed || pawn.Dead;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = pawn.Faction != Faction.OfPlayer;
				result = flag2;
			}
			return result;
		}
	}
}
