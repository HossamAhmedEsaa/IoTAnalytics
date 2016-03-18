using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using X2CodingLab.Utils;

namespace X2CodingLab.SensorTag.Sensors
{
    public class HumiditySensor : SensorBase
    {
        public HumiditySensor()
            : base(SensorName.HumiditySensor, SensorTagUuid.UUID_HUM_SERV, SensorTagUuid.UUID_HUM_CONF, SensorTagUuid.UUID_HUM_DATA)
        {
            
        }

        /// <summary>
        /// Calculates the humidity in percent.
        /// </summary>
        /// <param name="sensorData">Complete array of data retrieved from the sensor</param>
        /// <returns></returns>
        public static double CalculateHumidityInPercent(byte[] sensorData)
        {
            Validator.RequiresNotNull(sensorData);

            int hum = BitConverter.ToUInt16(sensorData, 2);

            //cut first two statusbits
            hum = hum - (hum % 4);

            // calculate in percent
            return (-6f) + 125f * (hum / 65535f);
        }
    }
}
