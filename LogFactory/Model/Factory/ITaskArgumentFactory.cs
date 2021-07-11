namespace LogFactory.Model.Factory
{
    /// <summary>
    /// 任务工厂结构
    /// </summary>
    public interface ITaskArgumentFactory
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        TaskArgument CreateTaskArgument<T>(T source = null)
            where T : class;
    }
}
