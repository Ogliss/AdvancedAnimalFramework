﻿using Verse;
using Verse.AI;
using RimWorld;

namespace AnimalWeapons
{
    // Token: 0x02000014 RID: 20
    public class ThinkNode_ConditionalWPVehicleCaravan : ThinkNode_Conditional
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00003DD4 File Offset: 0x00001FD4
		public override bool Satisfied(Pawn pawn)
		{
			return pawn.GetTraderCaravanRole() == TraderCaravanRole.Carrier && pawn.Faction != Faction.OfPlayer;
		}
	}
}
