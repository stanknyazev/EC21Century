using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;

namespace CreateDeviceIdentity
{
    class Program
    {
        static RegistryManager registryManager;
        static string connectionString = "HostName=EC21.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fAu/y0Fxf6w6DWLpxZ1aQuuAEUiDeg3jR2bgjqIckaY=";

        private static async Task AddDeviceAsync(string deviceId)
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }

        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync("myFirstDevice").Wait();
            Console.ReadLine();
            AddDeviceAsync("mySecondDevice").Wait();
            Console.ReadLine();
        }
    }
}
