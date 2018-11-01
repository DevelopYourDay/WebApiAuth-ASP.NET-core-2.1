using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAuth.Models
{
    public class TokenBlackList
    {
        public int Id { get; set; }
        public string token { get; set; }
    }
}
