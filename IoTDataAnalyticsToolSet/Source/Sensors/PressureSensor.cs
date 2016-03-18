using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using X2CodingLab.SensorTag.Exceptions;
using X2CodingLab.Utils;

namespace X2CodingLab.SensorTag.Sensors
{
    public class PressureSensor : SensorBase
    {
        private int[] calibrationData = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// Returns the calibration data read from the sensor after EnableSensor() was called. 
        /// </summary>
        public int[] CalibrationData
        {
            get { return calibrationData; }
        }

        public PressureSensor()
            : base(SensorName.PressureSensor, SensorTagUuid.UUID_BAR_SERV, SensorTagUuid.UUID_BAR_CONF, SensorTagUuid.UUID_BAR_DATA)
        {
            
        }

        /// <summary>
        /// Calculates the pressure from the raw sensor data.
        /// </summary>
        /// <param name="sensorData"></param>
        /// <returns>Pressure in pascal</returns>
        public static double CalculatePressure(byte[] sensorData)
        {
            Validator.RequiresNotNull(sensorData, "sensorData");
            
            float Calculateed_barometerData =( (sensorData[5] << 16) + (sensorData[4] << 8) + sensorData[3] )/ 100.0f;

            return Calculateed_barometerData;
        }

      
    }
}
