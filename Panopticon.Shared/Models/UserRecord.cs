using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panopticon.Shared.Models
{
    public class UserRecord
    {
		[Key]
		public ulong UserId { get; set; }
		public double LibcraftCoinBalance { get; set; }
		public int TablesFlipped { get; set; }
		public bool TimeOut { get; set; }
		public DateTime LastTimePosted { get; set; }
	}
}
