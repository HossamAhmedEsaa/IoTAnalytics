﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X2CodingLab.SensorTag.Exceptions
{
    /// <summary>
    /// Thrown when a device that matches certain criteria cannot be retrieved.
    /// </summary>
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException() 
            : base()
        {

        }

        public DeviceNotFoundException(string message)
            : base(message)
        {

        }

        public DeviceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
