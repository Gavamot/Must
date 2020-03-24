using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using MustDo;

namespace MustDo.Test
{
    public class MustTest
    {
        static int step = 0;
        private CancellationTokenSource source;
        [SetUp]
        public void BeforeTest()
        {
            step = 0;
            source = new CancellationTokenSource();
        }

        #region MyRegion Exec_Action

        [Test]
        public async Task Exec_Action()
        {
            var a = new MustDo.Must(100);
            var watch = new Stopwatch();
            int sleep = 60;
            int sleepCount = 5;
            var must = new Must(sleep);
            await must.Exec(token =>
            {
                if (!watch.IsRunning) 
                    watch.Start();
                if(step++ < sleepCount)
                   throw new Exception();
                watch.Stop();
            });
            Console.WriteLine();
            Assert.IsTrue(watch.ElapsedMilliseconds >= sleep * sleepCount);
        }

        [Test, Timeout(2000)]
        public void Exec_ActionToken()
        {
            Assert.Throws<TaskCanceledException>(() =>
            {
                var must = new Must(10, source.Token);
                var t = must.Exec(token => throw new Exception());
                Task.Run(() => source.Cancel());
                try
                {
                    t.Wait();
                }
                catch (AggregateException e)
                {
                    throw (TaskCanceledException) e.InnerException;
                }
               
            });
        }

        [Test, Timeout(2000)]
        public async Task Exec_ActionOnError()
        {
            var log = new List<string>();
            int sleepCount = 2;

            var must = new Must(0);
            must.OnError += e => { log.Add(e.Message); };

            await must.Exec(token =>
            {
                if (step++ < sleepCount)
                    throw new Exception(step.ToString());
            });

            for (int i = 0; i < sleepCount; i++)
            {
                Assert.AreEqual(log[i],(i + 1).ToString());
            }
        }

        #endregion

        #region Exec_Func

        [Test]
        public async Task Exec_Func()
        {
            var watch = new Stopwatch();
            int sleep = 60;
            int sleepCount = 5;
            int value = 1;
            var must = new Must(sleep);
            int res = await must.Exec(token =>
            {
                if (!watch.IsRunning)
                    watch.Start();
                if (step++ < sleepCount)
                    throw new Exception();
                watch.Stop();
                return value;
            }, sleep);
            Assert.AreEqual(value, res);
            Assert.IsTrue(watch.ElapsedMilliseconds >= sleep * sleepCount);
        }

        [Test, Timeout(20000000)]
        public void Exec_FuncToken()
        {
            Assert.Throws<TaskCanceledException>(() =>
            {
                var must = new Must(0, source.Token);
                var t = must.Exec(token =>
                {
                    throw new Exception();
                    return 1;
                });
                Task.Run(() => source.Cancel());
                try
                {
                    t.Wait();
                }
                catch (AggregateException e)
                {
                    throw (TaskCanceledException)e.InnerException;
                }
            });
        }

        [Test, Timeout(2000)]
        public async Task Exec_FuncOnError()
        {
            var log = new List<string>();
            int sleepCount = 2;
            var must = new Must(0, source.Token);
            must.OnError += e => { log.Add(e.Message); };

            await must.Exec(token =>
            {
                if (step++ < sleepCount)
                    throw new Exception(step.ToString());
                return 1;
            }, 1);

            for (int i = 0; i < sleepCount; i++)
            {
                Assert.AreEqual(log[i], (i + 1).ToString());
            }
        }

        #endregion

        #region ExecAttempts_Func

        [Test]
        public async Task ExecAttempts_Func()
        {
            var watch = new Stopwatch();
            int sleep = 60;
            int sleepCount = 5;
            int value = 1;
            var must = new Must(sleep);
            int res = await must.ExecAttempts(token =>
            {
                if (!watch.IsRunning)
                    watch.Start();
                if (step++ < sleepCount)
                    throw new Exception();
                watch.Stop();
                return value;
            }, 100, sleep);
            Assert.AreEqual(value, res);
            Assert.IsTrue(watch.ElapsedMilliseconds >= sleep * sleepCount);
        }

        [Test]
        public async Task ExecAttempts_FuncThrowsAttemptsOverException()
        {
            Assert.Throws<AttemptsOverException>(() =>
            {
                int sleep = 60;
                int sleepCount = 5;
                int value = 1;
                var must = new Must(sleep);
                try
                {
                    int res = must.ExecAttempts(token =>
                    {
                        if (step++ < sleepCount)
                            throw new Exception();
                        return value;
                    }, sleepCount - 1, sleep).Result;
                }
                catch (AggregateException e)
                {
                    throw (AttemptsOverException)e.InnerException;
                }
            });
        }

        [Test, Timeout(20000000)]
        public void ExecAttempts_FuncToken()
        {
            Assert.Throws<TaskCanceledException>(() =>
            {
                var must = new Must(0, source.Token);
                var t = must.ExecAttempts(token =>
                {
                    throw new Exception();
                    return 1;
                }, int.MaxValue);
                Task.Run(() => source.Cancel());
                try
                {
                    t.Wait();
                }
                catch (AggregateException e)
                {
                    throw (TaskCanceledException)e.InnerException;
                }
            });
        }

        [Test, Timeout(2000)]
        public async Task ExecAttempts_FuncOnError()
        {
            var log = new List<string>();
            int sleepCount = 2;
            var must = new Must(0, source.Token);
            must.OnError += e => { log.Add(e.Message); };

            await must.Exec(token =>
            {
                if (step++ < sleepCount)
                    throw new Exception(step.ToString());
                return 1;
            }, 1);

            for (int i = 0; i < sleepCount; i++)
            {
                Assert.AreEqual(log[i], (i + 1).ToString());
            }
        }

        #endregion

        #region ExecAttempts_Action

        [Test]
        public async Task ExecAttempts_Action()
        {
            var watch = new Stopwatch();
            int sleep = 60;
            int sleepCount = 5;
            int value = 1;
            var must = new Must(sleep);
            await must.ExecAttempts(token =>
            {
                if (!watch.IsRunning)
                    watch.Start();
                if (step++ < sleepCount)
                    throw new Exception();
                watch.Stop();
            }, 100, sleep);
            Assert.IsTrue(watch.ElapsedMilliseconds >= sleep * sleepCount);
        }

        [Test]
        public Task ExecAttempts_ActionThrowsAttemptsOverException()
        {
            Assert.Throws<AttemptsOverException>(() =>
            {
                int sleep = 60;
                int sleepCount = 5;
                var must = new Must(sleep);
                try
                {
                    must.ExecAttempts(token =>
                    {
                        if (step++ < sleepCount)
                            throw new Exception();
                    }, sleepCount - 1, sleep).Wait();
                }
                catch (AggregateException e)
                {
                    throw (AttemptsOverException)e.InnerException;
                }
            });
            return Task.CompletedTask;
        }

        [Test, Timeout(20000000)]
        public void ExecAttempts_ActionToken()
        {
            Assert.Throws<TaskCanceledException>(() =>
            {
                var must = new Must(0, source.Token);
                var t = must.ExecAttempts(token =>
                {
                    throw new Exception();
                }, int.MaxValue);
                Task.Run(() => source.Cancel());
                try
                {
                    t.Wait();
                }
                catch (AggregateException e)
                {
                    throw (TaskCanceledException)e.InnerException;
                }
            });
        }

        [Test, Timeout(2000)]
        public async Task ExecAttempts_ActionOnError()
        {
            var log = new List<string>();
            int sleepCount = 2;
            var must = new Must(0, source.Token);
            must.OnError += e => { log.Add(e.Message); };

            await must.Exec(token =>
            {
                if (step++ < sleepCount)
                    throw new Exception(step.ToString());
            }, 1);

            for (int i = 0; i < sleepCount; i++)
            {
                Assert.AreEqual(log[i], (i + 1).ToString());
            }
        }

        #endregion

    }
}
