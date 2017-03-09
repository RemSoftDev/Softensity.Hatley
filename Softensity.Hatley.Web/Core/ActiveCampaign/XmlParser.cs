using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Xml.Serialization;

namespace Softensity.Hatley.Web.Core.ActiveCampaign
{
    [XmlInclude(typeof(SubscribersListResult))]
    public abstract class Result
    {
        [XmlElement(ElementName = "result_code")]
        public int ResultCode { get; set; }
        [XmlElement(ElementName = "result_message")]
        public string ResultMessage { get; set; }
        [XmlElement(ElementName = "result_output")]
        public string ResultOutputType { get; set; }
    }

    [XmlRoot("account_get")]
    public class CheckingResult : Result
    {
        [XmlElement(ElementName = "account")]
        public string Account { get; set; }
    }

    [XmlRoot("subscriber_list")]
    public class SubscribersListResult : Result
    {
        [XmlElement(ElementName = "row")]
        public List<Subscriber> Subscribers { get; set; }
    }

    public class Subscriber
    {
        [XmlElement(ElementName = "id")]
        public int Id { get; set; }
        [XmlElement(ElementName = "email")]
        public string Email { get; set; }
        [XmlElement(ElementName = "phone")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "first_name")]
        public string Firstname { get; set; }
        [XmlElement(ElementName = "last_name")]
        public string Lastname { get; set; }
        [XmlElement(ElementName = "sdate")]
        public string SubscriptionDate { get; set; }
        [XmlElement(ElementName = "listid")]
        public int ListId { get; set; }

    }
}