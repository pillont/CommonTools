using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pillont.CommonTools.Core.Parallel
{
    public class SingleThreadUpdater
    {
        private CancellationTokenSource Source { get; set; }
        private SemaphoreSlim SourceSemaphore { get; } = new SemaphoreSlim(1, 1);

        public async Task UpdateAsync(Action action)
        {
            await UpdateAsync(() =>
            {
                action?.Invoke();
                return Task.CompletedTask;
            });
        }

        public async Task UpdateAsync(Func<Task> action)
        {
            await SourceSemaphore.WaitForAsync(() =>
            {
                Source?.Dispose();
                Source = new CancellationTokenSource();
            });

            await Task.Run(cancellationToken: Source.Token, action: async () =>
                                                                                await action());
        }
    }
}