namespace Microsoft.UnifiedRedisPlatform.Core.Pipeline
{
    public interface IPipeline
    {
        IPipeline With(BaseMiddleware middleware);
        BaseMiddleware Create();
    }
}
