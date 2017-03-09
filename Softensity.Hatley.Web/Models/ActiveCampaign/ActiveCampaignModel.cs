﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Softensity.Hatley.Web.Models.ActiveCampaign
{
    public class ActiveCampaignModel
    {
        [DisplayName("Api Key:")]
        [Required(ErrorMessage = "Enter Api Key.")]
        [MaxLength(100)]
        public string ApiKey { get; set; }

        [DisplayName("Api URL:")]
        [Url]
        [Required(ErrorMessage = "Enter Api URL.")]
        [MaxLength(100)]
        public string ApiUrl { get; set; }
    }
}