using StackExchange.Redis;
using System.Threading.Tasks;

namespace Microsoft.UnifiedRedisPlatform.Core.Database
{
    public partial class UnifiedRedisDatabase
    {
        public IBatch CreateBatch(object asyncState = null) => _primaryDatabase.CreateBatch(asyncState);

        public ITransaction CreateTransaction(object asyncState = null) => _primaryDatabase.CreateTransaction(asyncState);

        public RedisValue DebugObject(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            Execute(() => _primaryDatabase.DebugObject(CreateAppKey(key), flags));

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None) =>
            ExecuteAsync(() => _primaryDatabase.DebugObjectAsync(CreateAppKey(key), flags));

        public bool TryWait(Task task) => _primaryDatabase.TryWait(task);

        public void Wait(Task task) => _primaryDatabase.Wait(task);

        public T Wait<T>(Task<T> task) => _primaryDatabase.Wait(task);

        public void WaitAll(params Task[] tasks) => _primaryDatabase.WaitAll(tasks);
    }
}
