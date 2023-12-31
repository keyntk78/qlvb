﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Notification
{
    public class NotificationModel
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }


    public class GoogleNotification
    {
        public class DataPayload
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("body")]
            public string Body { get; set; }
        }
        [JsonProperty("priority")]
        public string Priority { get; set; } = "high";
        [JsonProperty("data")]
        public DataPayload Data { get; set; }
        [JsonProperty("notification")]
        public DataPayload Notification { get; set; }
    }

    public class ResponseModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class FcmNotificationSetting
    {
        public string SenderId { get; set; }
        public string ServerKey { get; set; }
    }
}
