using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using X2CodingLab.SensorTag.Exceptions;
using System.Runtime.InteropServices.WindowsRuntime;
using X2CodingLab.Utils;

namespace X2CodingLab.SensorTag.Sensors
{
     public class LuxometerSensor : SensorBase
     {
        public LuxometerSensor()
            : base(SensorName.LuxometerSensor, SensorTagUuid.UUID_LUX_SERV, SensorTagUuid.UUID_LUX_CONF, SensorTagUuid.UUID_LUX_DATA)
        {
            
        }
         /// <summary>
         /// Calculate the the raw luxometer data
         /// </summary>
        /// <param name="sensorData">Complete array of data retrieved from the sensor</param>
         /// <returns></returns>
        public static double CalculateLuxometer(byte[] sensorData)
        {
            Validator.RequiresNotNull(sensorData);

            var lux = BitConverter.ToUInt16(sensorData, 0);

            var exponent = (lux & 0xF000) >> 12;
            var mantissa = (lux & 0x0FFF);

            var flLux = mantissa * Math.Pow(2, exponent) / 100.0;

            return flLux;
        }
         /// <summary>
         /// Sets the read period of the luxometer
         /// </summary>
         /// <param name="time">Deafult time is 100ms</param>
        public async Task SetReadPeriod(byte time)
        {
            Validator.Requires<DeviceNotInitializedException>(deviceService != null);

            if (time < 10)
                throw new ArgumentOutOfRangeException("time", "Period can't be lower than 100ms");

            GattCharacteristic dataCharacteristic = deviceService.GetCharacteristics(new Guid(SensorTagUuid.UUID_LUX_PERI))[0];

            byte[] data = new byte[] { time };
            GattCommunicationStatus status = await dataCharacteristic.WriteValueAsync(data.AsBuffer());
            if (status == GattCommunicationStatus.Unreachable)
            {
                throw new DeviceUnreachableException(DeviceUnreachableException.DEFAULT_UNREACHABLE_MESSAGE);
            }
        }
    }
}
