using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice
{
    class Program
    {
        //static DeviceClient deviceClient;
        static string iotHubUri = "EC21.azure-devices.net";
        static string deviceKey = "2lWnKLDADn9lg8RlZpBsK1MQH2a8YA7W6iakbHdUyCc=";
        static string deviceKey2 = "9vsXdWnkEsvIz6/PxMvg1F776/SkB15XNR3cKSwjbyo=";

        private static async Task SendDeviceToCloudMessagesAsync(string _deviceId, DeviceClient deviceClient)
        {
            int messageId = 1;

            while (true)
            {
              var telemetryDataPoint = new
                {
                    messageId = messageId++,
                    deviceId = _deviceId,
                    message = "I am alive",
                    date = DateTime.UtcNow
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("level", (_deviceId == "myFirstDevice" ) ? "critical" : "normal");

                await deviceClient.SendEventAsync(message);
                lock (iotHubUri)
                {
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                }

                await Task.Delay(1000);
            }
        }
        private static async Task ReceiveC2dAsync(DeviceClient deviceClient)
        {
            //Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;
                lock (iotHubUri)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                    Console.ResetColor();
                }

                await deviceClient.CompleteAsync(receivedMessage);

                await Task.Delay(300);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated devices\n");
            DeviceClient deviceClient;
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("myFirstDevice", deviceKey), TransportType.Mqtt);

            DeviceClient deviceClient2;
            deviceClient2 = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("mySecondDevice", deviceKey2), TransportType.Mqtt);


            var tasks = new List<Task>();
            
            tasks.Add(SendDeviceToCloudMessagesAsync("myFirstDevice", deviceClient));
            tasks.Add(ReceiveC2dAsync(deviceClient));

            tasks.Add(SendDeviceToCloudMessagesAsync("mySecondDevice", deviceClient2));
            tasks.Add(ReceiveC2dAsync(deviceClient2));

            Task.WaitAll(tasks.ToArray());
            Console.ReadLine();
        }
    }
}
