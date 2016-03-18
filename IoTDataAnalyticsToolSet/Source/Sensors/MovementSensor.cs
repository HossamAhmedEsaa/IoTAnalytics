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
   public class MovementSensor:SensorBase
    {
        private GyroscopeAxis gyroscopeAxis;
        public MovementSensor()
            : base(SensorName.MovementSensor, SensorTagUuid.UUID_MOV_SERV, SensorTagUuid.UUID_MOV_CONF, SensorTagUuid.UUID_MOV_DATA)
        {
            
        }
        /// <summary>
        /// Extracts the values of the 3 axis from the raw data of the sensor,
        /// </summary>
        /// <param name="sensorData">Complete array of data retrieved from the sensor</param>
        /// <returns>Array of doubles with the size of 3</returns>
        public static double[] CalculateAccelerometerCoordinates(byte[] sensorData)
        {
            Validator.RequiresNotNull(sensorData, "sensorData");
            return new double[] { BitConverter.ToInt16(sensorData, 6) / 4096.0, BitConverter.ToInt16(sensorData, 8) / 4096.0, BitConverter.ToInt16(sensorData, 10) / 4096.0 };
        }

        /// <summary>
        /// Sets the period the sensor reads data. Default is 1s. Lower limit is 100ms.
        /// </summary>
        /// <param name="time">Period in 10 ms.</param>
        /// <exception cref="DeviceUnreachableException">Thrown if it wasn't possible to communicate with the device.</exception>
        /// <exception cref="DeviceNotInitializedException">Thrown if sensor has not been initialized successfully.</exception>
        public async Task SetAccelerometerReadPeriod(byte time)
        {
            Validator.Requires<DeviceNotInitializedException>(deviceService != null);

            if (time < 10)
                throw new ArgumentOutOfRangeException("time", "Period can't be lower than 100ms");

            GattCharacteristic dataCharacteristic = deviceService.GetCharacteristics(new Guid(SensorTagUuid.UUID_MOV_PERI))[0];

            byte[] data = new byte[] { time };
            GattCommunicationStatus status = await dataCharacteristic.WriteValueAsync(data.AsBuffer());
            if (status == GattCommunicationStatus.Unreachable)
            {
                throw new DeviceUnreachableException(DeviceUnreachableException.DEFAULT_UNREACHABLE_MESSAGE);
            }
        }
        /// <summary>
        /// Returns the GyroscopeAxis used to enable the sensor
        /// </summary>
        public GyroscopeAxis GyroscopeAxis
        {
            get { return gyroscopeAxis; }
        }

        /// <summary>
        /// Calculates the value of the different gyroscope axis and scales it.
        /// </summary>
        /// <param name="data">Complete array of data retrieved from the sensor</param>
        /// <param name="axis">Specifies the axis the gyroscope was configured to read</param>
        /// <returns>Array of float with values in order of the GyroscopeAxis enum</returns>
        public static float[] CalculateGyroscopeAxisValue(byte[] data, GyroscopeAxis axis)
        {
            switch (axis)
            {
                case GyroscopeAxis.X:
                case GyroscopeAxis.Y:
                case GyroscopeAxis.Z:
                    return new float[] { BitConverter.ToInt16(data, 0) * (500f / 65536f) };
                case GyroscopeAxis.XY:
                case GyroscopeAxis.XZ:
                case GyroscopeAxis.YZ:
                    return new float[] { BitConverter.ToInt16(data, 0) * (500f / 65536f), 
                        BitConverter.ToInt16(data, 2) * (500f / 65536f) };
                case GyroscopeAxis.XYZ:
                    return new float[] { BitConverter.ToInt16(data, 0)/128.0f, 
                        BitConverter.ToInt16(data, 2) * 128.0f, 
                        BitConverter.ToInt16(data, 4) * 128.0f };
                default:
                    return new float[] { 0, 0, 0 };
            }
        }

        /// <summary>
        /// Enables the sensor with the specified axis
        /// </summary>
        /// <param name="gyroscopeAxis">axis you want to record</param>
        /// <returns></returns>
        /// <exception cref="DeviceUnreachableException">Thrown if it wasn't possible to communicate with the device.</exception>
        /// <exception cref="DeviceNotInitializedException">Thrown if sensor has not been initialized successfully.</exception>
        public async Task EnableSensor(GyroscopeAxis gyroscopeAxis)
        {
            Validator.Requires<DeviceNotInitializedException>(deviceService != null);
            this.gyroscopeAxis = gyroscopeAxis;
            await base.EnableSensor(new byte[] { (byte)gyroscopeAxis });
        }

        /// <summary>
        /// Enables the sensor to read all axis
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DeviceUnreachableException">Thrown if it wasn't possible to communicate with the device.</exception>
        /// <exception cref="DeviceNotInitializedException">Thrown if sensor has not been initialized successfully.</exception>
        public override async Task EnableSensor()
        {
            await EnableSensor(GyroscopeAxis.XYZ);
        }
        /// <summary>
        /// Extracts the three axis from the raw sensor data and scales it.
        /// </summary>
        /// <param name="sensorData">Complete array of data retrieved from the sensor</param>
        /// <returns></returns>
        public static float[] CalculateMagnetometerCoordinates(byte[] sensorData)
        {
            Validator.RequiresNotNull(sensorData);
            return new float[] { BitConverter.ToInt16(sensorData, 12) * 4912.0f / 32768.0f, 
                BitConverter.ToInt16(sensorData, 14) * 4912.0f / 32768.0f, 
                BitConverter.ToInt16(sensorData, 16) * 4912.0f / 32768.0f};
        }

        /// <summary>
        /// Sets the period the sensor reads data. Default is 1s. Lower limit is 100ms.
        /// </summary>
        /// <param name="time">Period in 10 ms</param>
        /// <exception cref="DeviceUnreachableException">Thrown if it wasn't possible to communicate with the device.</exception>
        /// <exception cref="DeviceNotInitializedException">Thrown if sensor has not been initialized successfully.</exception>
        public async Task SetMagnetorReadPeriod(byte time)
        {
            Validator.Requires<DeviceNotInitializedException>(deviceService != null);

            if (time < 10)
                throw new ArgumentOutOfRangeException("time", "Period can't be lower than 100ms");

            GattCharacteristic dataCharacteristic = deviceService.GetCharacteristics(new Guid(SensorTagUuid.UUID_MOV_PERI))[0];

            byte[] data = new byte[] { time };
            GattCommunicationStatus status = await dataCharacteristic.WriteValueAsync(data.AsBuffer());
            if (status == GattCommunicationStatus.Unreachable)
            {
                throw new DeviceUnreachableException(DeviceUnreachableException.DEFAULT_UNREACHABLE_MESSAGE);
            }
        }

    }
    public enum GyroscopeAxis
    {
        X = 1,
        Y = 2,
        XY = 3,
        Z = 4,
        XZ = 5,
        YZ = 6,
        XYZ = 7
    }


}
