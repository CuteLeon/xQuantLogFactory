using static BatchHost.Model.FixedValue;

namespace BatchHost.Model
{
    public static class Extension
    {
        /// <summary>
        /// 获取一个此单位时间内分钟数
        /// </summary>
        /// <param name="timeUtil"></param>
        /// <returns></returns>
        public static int GetMinutes(this TimeUnits timeUtil)
        {
            switch (timeUtil)
            {
                case TimeUnits.Day:
                    return 24 * 60;
                case TimeUnits.Hour:
                    return 60;
                case TimeUnits.Minute:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
