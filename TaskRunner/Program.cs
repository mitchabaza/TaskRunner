using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dance;
using Newtonsoft.Json;
using RabbitMQ.Client;
using JsonSerializer = ScheduledTaskRunner.JsonSerializer;

namespace ScheduledTaskRunner
{
    public class Program
    {
        static void Main(string[] args)
        {
          
            ConsumeMessages();
        }

        private static void ConsumeMessages()
        {
            var taskRunner = new ODIGTaskRunner(new JsonSerializer());
            taskRunner.ConfigureQueue(); 
            PublishMessages(1);
            var ts = new CancellationTokenSource();
            Task task = new Task(() => taskRunner.Start(ts.Token));
            // Run the task.
            task.Start();
            Console.WriteLine("Hit any key to exit");
            Console.ReadKey();
            Console.WriteLine("Cancelling thread...");
            ts.Cancel();
        }

        private static void PublishMessages(int count)
        {
            var factory = new ConnectionFactory();

            using (IConnection connection = factory.CreateConnection())
            {
                var encoding = new UTF8Encoding();
                using (IModel model = connection.CreateModel())
                {
                    
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    var tasks = new ScheduledTask(1231, Guid.NewGuid().ToString(), DateTime.UtcNow.AddMinutes(-i * 5),"ENT_REPORTS");
                    //    string str = JsonConvert.SerializeObject(tasks);
                    //    byte[] message = encoding.GetBytes(str);
                    //    model.BasicPublish("ScheduledTasksExchange", "ENT_REPORTS", null, message);

                    //}
                    for (int i = 0; i < count; i++)
                    {
                        var tasks = new ScheduledTask(1231, Guid.NewGuid().ToString(), DateTime.UtcNow.AddMinutes(-i*5),
                            "ODIG");
                        string str = JsonConvert.SerializeObject(tasks);
                        byte[] message = encoding.GetBytes(str);
                        model.BasicPublish("ScheduledTasksExchange", "ODIG", null, message);
                    }
                }
            }
        }
    }
}