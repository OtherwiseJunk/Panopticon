using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panopticon.Shared.Models
{
    public class SahmRuleObservation
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public bool IsRecession { get; set; }

        public SahmRuleObservation(DateTime date, double value)
        {
            Date = date;
            Value = value;
            IsRecession = value >= 0.50;
        }
    }
}
