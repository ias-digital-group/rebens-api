using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Models
{
    public class CloudinaryModel
    {
        public string public_id { get; set; }
        public string version { get; set; }
        public string signature { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
        public string resource_type { get; set; }
        public DateTime created_at { get; set; }
        public long bytes { get; set; }
        public string type { get; set; }
        public string etag { get; set; }
        public string placeholder { get; set; }
        public string url { get; set; }
        public string secure_url { get; set; }
        public string access_mode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
