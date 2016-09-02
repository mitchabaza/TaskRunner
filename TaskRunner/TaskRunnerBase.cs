using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dance;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.MessagePatterns;

namespace ScheduledTaskRunner
{
    public abstract class TaskRunnerBase
    {
        const string ScheduledTasksExchange = "ScheduledTasksExchange";
        private readonly IJsonSerializer _jsonSerializer;

        protected TaskRunnerBase(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public void Start(CancellationToken token)
        {
            var factory = new ConnectionFactory();

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                     model.BasicQos(0, 1, false);
                    EventingBasicConsumer consumer = new EventingBasicConsumer(model);
                    consumer.Received += (sender, args) =>
                    {
                        var message = Encoding.UTF8.GetString(args.Body);
                        Console.WriteLine($"Receiving a message {DateTime.Now.ToLongTimeString()}");
                        var task = _jsonSerializer.Deserialize<ScheduledTask>(message);
                        if (Run(task).Success)
                        {
                            model.BasicAck(args.DeliveryTag, true);
                        }
                        else
                        {
                            model.BasicNack(args.DeliveryTag, false,false);
                        }
                    };
                    //start the consumer
                    model.BasicConsume(QueueName, false, consumer);

                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            //simulate cleanup
                            Thread.Sleep(2000);
                            Console.WriteLine("Bugging out");
                            return;
                        }
                    }
                }
            }
        }


        private string QueueName => $"{RoutingKey}Queue";

        public void ConfigureQueue()
        {
            var factory = new ConnectionFactory();

            using (IConnection connection = factory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    model.ExchangeDelete("Dlx");
                    model.ExchangeDeclare("Dlx", "fanout",true);
                    model.QueueDeclare("Dlx", true, false, false,
                        new Dictionary<string, object>() {{"x-dead-letter-exchange", ScheduledTasksExchange}, {"x-message-ttl",60000} });
                    model.QueueBind("Dlx", "Dlx",string.Empty);
                    model.QueueDelete(QueueName);
                    model.QueueDeclare(QueueName, true, false, false,
                        new Dictionary<string, object>
                        {
                            
                            {"x-dead-letter-exchange", "Dlx"}
                        });
                    model.ExchangeDeclare(ScheduledTasksExchange, "direct", true);
                    model.QueueBind(QueueName, ScheduledTasksExchange, RoutingKey, null);
                }
            }
        }

        public abstract string RoutingKey { get; }
        protected abstract TaskRunResult Run(ScheduledTask task);
    }
}