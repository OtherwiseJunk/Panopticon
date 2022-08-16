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
        public int Id { get; set; }

        public ulong UserId { get; set; }

        public int TablesFlipped { get; set; }
    }
}
