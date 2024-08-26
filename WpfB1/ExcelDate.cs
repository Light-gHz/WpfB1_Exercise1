using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfB1
{
    internal class ExcelDate
    {
        public class Account
        {
            [Key]
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public decimal IncomingBalanceActive { get; set; }
            public decimal IncomingBalancePassive { get; set; }
        }

        public class Turnover
        {
            [Key]
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public decimal DebitTurnover { get; set; }
            public decimal CreditTurnover { get; set; }
        }

        public class Balance
        {
            [Key]
            public int Id { get; set; }
            public string AccountNumber { get; set; }
            public decimal OutgoingBalanceActive { get; set; }
            public decimal OutgoingBalancePassive { get; set; }
        }
    }
}
