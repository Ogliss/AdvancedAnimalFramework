using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000020 RID: 32
	public class WorkGiver_WPDeconstruct : WorkGiver_WPRemoveBuilding
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00005428 File Offset: 0x00003628
		protected override DesignationDef Designation
		{
			get
			{
				return DesignationDefOf.Deconstruct;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00005440 File Offset: 0x00003640
		protected override JobDef WPRemoveBuildingJob
		{
			get
			{
				return WPJobDefOf.WPDeconstruct;
			}
		}
	}
}
