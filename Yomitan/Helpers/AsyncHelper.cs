using System.Threading;
using System.Threading.Tasks;

namespace Yomitan.Helpers
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None,
                TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static async Task<TResult> RunAsync<TResult>(this Task<TResult> func)
        {
            return await _myTaskFactory.StartNew(async () => await func).Unwrap();
        }

        public static async Task RunAsync(this Task func)
        {
            await _myTaskFactory.StartNew(async () => await func).Unwrap();
        }

        public static TResult RunSync<TResult>(this Task<TResult> func)
        {
            return _myTaskFactory.StartNew(async () => await func).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(this Task func)
        {
            _myTaskFactory.StartNew(async () => await func).Unwrap().GetAwaiter().GetResult();
        }
    }
}
