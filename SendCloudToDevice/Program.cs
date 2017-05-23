using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Devices;


namespace SendCloudToDevice
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=EC21.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fAu/y0Fxf6w6DWLpxZ1aQuuAEUiDeg3jR2bgjqIckaY=";

        private async static Task SendCloudToDeviceMessageAsync(string _deviceId)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Hello device "+ _deviceId));
            await serviceClient.SendAsync(_deviceId, commandMessage);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            while (true)
            {
                Console.WriteLine("Press any key to send a C2D message to myFirstDevice.");
                Console.ReadLine();
                SendCloudToDeviceMessageAsync("myFirstDevice").Wait();
                Console.WriteLine("Press any key to send a C2D message to mySecondDevice.");
                Console.ReadLine();
                SendCloudToDeviceMessageAsync("mySecondDevice").Wait();
            }
           
        }
    }
}
