using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();
        var mqttClient = mqttFactory.CreateMqttClient();

        var certificate = new X509Certificate2("C:\\Users\\musta\\source\\repos\\ConsoleApp1\\ConsoleApp1\\2021.pfx", "1010");

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("an726pjx0w8v9-ats.iot.eu-north-1.amazonaws.com", 8883)
            .WithTls(new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                Certificates = new[] { certificate }
            })
            .WithClientId("AspClient")
            .Build();

        string topic = "ses";

        mqttClient.UseConnectedHandler(async e =>
        {
            Console.WriteLine("Connected to AWS IoT");
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            Console.WriteLine($"Subscribed to topic: {topic}");
        });

        mqttClient.UseApplicationMessageReceivedHandler(e =>
        {
            Console.WriteLine("Received message: " + Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        });

        await mqttClient.ConnectAsync(options);
        
        while (true)
        {
            Console.WriteLine("Lütfen Mesajınızı Yazın (çıkmak için 'exit' yazın):");
            string mesaj = Console.ReadLine();

            if (mesaj.ToLower() == "exit")
            {
                break;
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(mesaj)
                .WithAtLeastOnceQoS()
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine("Message published");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }
}
