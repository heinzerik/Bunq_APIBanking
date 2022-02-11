using System;
using System.Collections.Generic;
using System.Text;

namespace Bunq_APIBanking
{
    public class BankingException : ApplicationException
    {
        public BankingException(string message) : base(message)
        {
            
        }
    }
}
