using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Library.Helpers
{
    public class JwtSection
    {
        public string? Issuer { get; set; }
        public string ?Audience { get; set; }
        public string ?Key { get; set; }
    }
}
