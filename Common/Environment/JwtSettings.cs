using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Environment
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public double Lifetime { get; set; }
        public string CookieName { get; set; }
    }
}
