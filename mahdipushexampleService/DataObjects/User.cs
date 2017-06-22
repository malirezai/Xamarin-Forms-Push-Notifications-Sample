using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mahdipushexampleService.DataObjects
{
    public class User : EntityData
    {
        public string Name { get; set; }
        public string DeviceToken { get; set; }
        public string DevicePlatform { get; set; }
        public string NHRegistrationID { get; set; }
        public string UserID { get; set; }
    }
}