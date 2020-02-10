using System.Collections.Generic;

namespace Otomatik.BlazorBin.Models
{
    public class Request
    {
        public string Method { get; set; }

        public string Body { get; set; }

        public List<KeyValuePair<string, string>> Headers { get; set; }

        public bool IsOpened { get; set; }
    }
}
