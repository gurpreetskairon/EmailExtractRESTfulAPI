using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace EmailExtractRESTfulAPI.Models
{
    [DataContract]
    public class GstInfo
    {
        [DataMember(Name = "costCentre")]
        public string CostCentre { get; set; }

        [DataMember(Name = "total")]
        public double Total { get; set; }

        [DataMember(Name = "gst")]
        public double Gst { get; set; }
    }
}