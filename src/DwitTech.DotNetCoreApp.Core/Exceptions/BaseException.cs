using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Core.Exceptions
{
    internal abstract class BaseException
    {
        public string? Message { get; set; }
        public Exception? Exception { get; set; }

    }
}
