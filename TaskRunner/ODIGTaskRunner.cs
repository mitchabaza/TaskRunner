using System;
using System.Threading;
using Dance;

namespace ScheduledTaskRunner
{
    public class ODIGTaskRunner: TaskRunnerBase
    {
        public ODIGTaskRunner(IJsonSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        public override string RoutingKey => "ODIG";
        protected override  TaskRunResult Run(ScheduledTask task)
        {
            Console.WriteLine($"Veriform is doing some shit y'all: {task}");
            Thread.Sleep(500);
            return new TaskRunResult(false);
        }
    }

 }