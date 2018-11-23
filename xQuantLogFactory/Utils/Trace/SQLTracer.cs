namespace xQuantLogFactory.Utils.Trace
{
    /// <summary>
    /// SQL追踪器
    /// </summary>
    public class SQLTracer : ITracer
    {
        /// <summary>
        /// 输出SQL日志 (由EF调用)
        /// </summary>
        /// <param name="info"></param>
        public void WriteLine(string info)
        {
            System.Diagnostics.Debug.Print(info);
        }

        [System.Obsolete]
        public void WriteLine(string info, params object[] values)
        {
        }
    }
}
