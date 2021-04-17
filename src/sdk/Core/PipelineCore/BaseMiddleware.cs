using System;
using System.Threading.Tasks;

//TODO: Make this into a library
namespace Microsoft.UnifiedRedisPlatform.Core.Pipeline
{
    public abstract class BaseMiddleware
    {
        protected BaseMiddleware(BaseMiddleware next)
        {
            _next = next;
        }

        protected BaseMiddleware() { }

        protected BaseMiddleware _next;

        public virtual T Execute<T>(Func<T> action, ExecutionContext context)
        {
            if (_next != null)
                return _next.Execute(action, context);
            return action();
        }

        public virtual Task<T> ExecuteAsync<T>(Func<Task<T>> action, ExecutionContext context)
        {
            if (_next != null)
                return _next.ExecuteAsync(action, context);
            return action();
        }

        protected virtual bool ShouldSkipExecution(ExecutionContext context) => false;

        public virtual void SetNext(BaseMiddleware next)
        {
            if (_next == null)
                _next = next;
        }

        public virtual void ClearNext()
        {
            _next = null;
        }
    }
}
