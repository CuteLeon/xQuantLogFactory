namespace xQuantLogFactory.Model.Factory
{
    public interface ITaskArgumentFactory
    {
        TaskArgument CreateTaskArgument<T>(T source = null) 
            where T : class;
    }
}
