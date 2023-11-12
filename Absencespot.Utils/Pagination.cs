using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Utils
{
    public class Pagination<T>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages{ get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
