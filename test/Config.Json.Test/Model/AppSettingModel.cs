using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Configuration.Json.Test.Model
{
    public class AppSettingModel
    {
        public string RoomDefaultTemperature { get; set; }

        public SpaceshipSettingModel SpaceshipSetting { get; set; }
    }
}
