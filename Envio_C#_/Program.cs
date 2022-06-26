using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Envio_C__
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

                IModel channel1 = SetupChannel(connection, nomeFila);
                // IModel channel2 = CreateChannel(connection, nomeFila);
                // IModel channel3 = CreateChannel(connection, nomeFila);

                BuildPublisher(channel1, "Produtor A", DateTime.Now.ToString());
                // BuildPublisher(channel2, "Produtor B", "B");
                // BuildPublisher(channel3, "Produtor C", "C");

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

        public static void BuildPublisher(IModel channel, string publisher, string message)
        {
            Task.Run(() =>
            {

                //IBasicProperties props = channel.CreateBasicProperties();

                //Dura de dez a vinte segundos na fila e é especificado por mensagem
                //props.Expiration = new Random().Next(10000, 20000).ToString();

                while (true)
                {
                    Random r = new Random();
                    string mensagem = $"{publisher} - Enviou {message}";
                    byte[] bytes = Encoding.UTF8.GetBytes(mensagem);

                    channel.BasicPublish(
                        body: bytes,
                        routingKey: "",
                        basicProperties: null,
                        exchange: "envio_para_A_B_C"
                    );

                    Console.WriteLine(mensagem);
                    Thread.Sleep(1000);
                }
            });
        }
    }
}
