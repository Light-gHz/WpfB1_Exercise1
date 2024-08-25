using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfB1
{
    internal class Line
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string LatinString { get; set; }
        public string CyrillicString { get; set; }
        public int EvenNumber { get; set; }
        public double DecimalNumber { get; set; }
    }
}
