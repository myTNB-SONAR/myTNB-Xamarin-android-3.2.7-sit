﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace myTNB_Android.Src.ManageCards.Request
{
    public class RemoveRegisteredCardRequest
    {
        [JsonProperty("apiKeyID")]
        public string ApiKeyId { get; set; }

        [JsonProperty("registeredCardId")]
        public string RegisteredCardId { get; set; }
    }
}