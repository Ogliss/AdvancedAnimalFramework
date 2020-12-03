using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000004 RID: 4
	public class JobGiver_WalkblemSelfDestruct : ThinkNode_JobGiver
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002390 File Offset: 0x00000590
		protected override Job TryGiveJob(Pawn pawn)
		{
			float radius = 1.5f;
			GenExplosion.DoExplosion(pawn.Position, pawn.Map, radius, DamageDefOf.Bomb, pawn, 5000000, 1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, true, null, null);
			return null;
		}
	}
}
