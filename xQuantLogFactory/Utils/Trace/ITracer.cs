namespace xQuantLogFactory.Utils.Trace
{

    /// <summary>
    /// 追踪器接口
    /// </summary>
    public interface ITracer
    {
        void WriteLine(string info);

        void WriteLine(string info, params object[] values);
    }
}
