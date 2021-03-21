using System.Linq;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Pipeline
{
    public class PipelineManager : IPipeline
    {
        private readonly List<BaseMiddleware> _middlewares = new List<BaseMiddleware>();
        private BaseMiddleware _lastMiddleware = null;

        public PipelineManager() { }
        
        public BaseMiddleware Create()
        {
            With(new ExecutionMiddleware());
            return _middlewares.First();
        }

        public IPipeline With(BaseMiddleware middleware)
        {
            if (_lastMiddleware != null)
            {
                _lastMiddleware.ClearNext();
                _lastMiddleware.SetNext(middleware);
            }
            _lastMiddleware = middleware;
            _middlewares.Add(middleware);
            return this;
        }
    }
}
