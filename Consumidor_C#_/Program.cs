using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumidor_C__
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            {
                string nomeFila = "mensagens";

                IModel channel = SetupChannel(connection, nomeFila);

                BuildAndRunWorker(channel, nomeFila + "_A", "Consumidor A:");
                BuildAndRunWorker(channel, nomeFila + "_B", "Consumidor B:");
                BuildAndRunWorker(channel, nomeFila + "_C", "Consumidor C:");
                
                Console.ReadKey();
            }
        }

        public static IModel SetupChannel(IConnection connection, string queue)
        {
            IModel channel = connection.CreateModel();

            Dictionary<string, object> args = new Dictionary<string, object>();

            //REMOVER CADA COMENTÁRIO E TESTAR

            //Dura vinte segundos na fila e é especificado por fila, ou seja, toda a mensagem na fila
            //vai durar quarenta segundos
            args.Add("x-message-ttl", 40000);

            //A fila é apagada depois de 20 segundos de ociosidade
            // args.Add("x-expires", 20000);

            //A fila aceita no máximo 10 mensagens
            // args.Add("x-max-length", 10);

            //Criando várias filas
            string queue_A = queue + "_A";
            channel.QueueDeclare(queue: queue_A,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: args);

            string queue_B = queue + "_B";
            channel.QueueDeclare(queue: queue_B,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: args);

            string queue_C = queue + "_C";
            channel.QueueDeclare(queue: queue_C,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: args);

            channel.ExchangeDeclare("envio_para_A_B_C", ExchangeType.Fanout);

            channel.QueueBind(queue_A, "envio_para_A_B_C", "", null);
            channel.QueueBind(queue_B, "envio_para_A_B_C", "", null);
            channel.QueueBind(queue_C, "envio_para_A_B_C", "", null);

            return channel;
        }

        public static void BuildAndRunWorker(IModel channel, string queue, string consumerName)
        {
            Task.Run(() =>
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, message) =>
                {
                    var body = message.Body.ToArray();
                    var text = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"{consumerName} {text}");
                };
                channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);
                Console.ReadLine();
            });
        }
    }
}
