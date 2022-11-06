using System;
using Verse;
using Verse.AI;

namespace AnimalJobs
{
    public class ThinkNode_WPChancePerHour_Constant : ThinkNode_WPChancePerHour
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_WPChancePerHour_Constant thinkNode_WPChancePerHour_Constant = (ThinkNode_WPChancePerHour_Constant)base.DeepCopy(resolve);
            thinkNode_WPChancePerHour_Constant.mtbHours = this.mtbHours;
            thinkNode_WPChancePerHour_Constant.mtbDays = this.mtbDays;
            return thinkNode_WPChancePerHour_Constant;
        }

        protected override float MtbHours(Pawn Pawn)
        {
            if (this.mtbDays > 0f)
            {
                return this.mtbDays * 24f;
            }
            return this.mtbHours;
        }

        private float mtbHours = -1f;

        private float mtbDays = -1f;
    }
}
