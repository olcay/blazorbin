using System;
using System.Collections.Generic;

namespace Otomatik.BlazorBin.Common
{
    public class Request
    {
        public Guid Id { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }

        public string QueryString { get; set; }

        public List<KeyValuePair<string, string>> Headers { get; set; }

        public bool IsOpened { get; set; }

        public DateTime ReceivedOn { get; set; }
    }
}
