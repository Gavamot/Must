using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace ConsoleApp6
{
    public class Must
    {
        public const int MinSleepMs = 1;
        public readonly int DefaultErrorSleepMs = -1;
        public readonly CancellationToken Token;
        public Action<Exception> OnError { get; set; } = e => { };

        public Must(int defaultErrorSleepMs = 1000, CancellationToken token = default)
        {
            this.DefaultErrorSleepMs = GetErrorSleepMs(defaultErrorSleepMs);
            this.Token = token;
        }

        public Task<T> ExecAttempts<T>(Func<T> func, int attempts, int errorSleepMs = -1) => 
            ExecAttempts(token => func(), attempts, errorSleepMs);

        public Task ExecAttempts(Action action, int attempts, int errorSleepMs = -1) =>
            ExecAttempts(token => action(), attempts, errorSleepMs);

        public Task<T> Exec<T>(Func<T> func, int errorSleepMs = -1) =>
            Exec(token => func(), errorSleepMs);

        public Task Exec(Action action, int errorSleepMs = -1) =>
            Exec(token => action(), errorSleepMs);

        /// <exception cref="AttemptsOverException">Task was canceled</exception>
        /// <exception cref="TaskCanceledException">Task was canceled</exception>
        public async Task<T> ExecAttempts<T>(Func<CancellationToken, T> func, int attempts, int errorSleepMs = -1)
        {
            errorSleepMs = GetErrorSleepMs(errorSleepMs);
            while (true)
            {
                try
                {
                    return func(Token);
                }
                catch (TaskCanceledException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    OnError(e);
                    if (--attempts <= 0)
                    {
                        throw new AttemptsOverException(e);
                    }
                    await Task.Delay(errorSleepMs, Token);
                }
            }
        }

        /// <exception cref="AttemptsOverException">Task was canceled</exception>
        /// <exception cref="TaskCanceledException">Task was canceled</exception>
        public async Task ExecAttempts(Action<CancellationToken> action, int attempts, int errorSleepMs = -1)
        {
            errorSleepMs = GetErrorSleepMs(errorSleepMs);
            while (true)
            {
                try
                {
                    action(Token);
                    break;
                }
                catch (TaskCanceledException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    OnError(e);
                    if (--attempts <= 0)
                    {
                        throw new AttemptsOverException(e);
                    }
                    await Task.Delay(errorSleepMs, Token);
                }
            }
        }

        /// <exception cref="TaskCanceledException">Task was canceled</exception>
        public async Task<T> Exec<T>(Func<CancellationToken, T> func, int errorSleepMs = -1)
        {
            errorSleepMs = GetErrorSleepMs(errorSleepMs);
            while (true)
            {
                try
                {
                    return func(Token);
                }
                catch (TaskCanceledException e)
                {
                    throw;
                }
                catch (Exception e)
                {
                    OnError(e);
                    await Task.Delay(errorSleepMs, Token);
                }
            }
        }

        /// <exception cref="TaskCanceledException">Task was canceled</exception>
        public async Task Exec(Action<CancellationToken> action, int errorSleepMs = -1)
        {
            errorSleepMs = GetErrorSleepMs(errorSleepMs);
            while (true)
            {
                try
                {
                    action(Token);
                    break;
                }
                catch(TaskCanceledException e)
                {
                    throw;
                }
                catch(Exception e)
                {
                    OnError(e);
                    await Task.Delay(errorSleepMs, Token);
                }
            }
        }

        private int GetErrorSleepMs(int errorSleepMs)
        {
            if (errorSleepMs < MinSleepMs)
            {
                if (DefaultErrorSleepMs < MinSleepMs)
                    return MinSleepMs;
                return DefaultErrorSleepMs;
            }
            return errorSleepMs;
        } 
    }
}