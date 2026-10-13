using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json.Linq;

namespace apiRESTCheckUsuarioTADS.Models
{
    public class clsApiStatus
    {
        public bool statusExec { get; set; }
        public string msg { get; set; }
        public int ban { get; set; }
        public JObject datos { get; set; }
    }
}