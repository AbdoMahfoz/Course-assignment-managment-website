using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseAssignmentManagmentWebsite.Models
{
    public class Pair<TFirst, TSecond>
    {
        public Pair() { }
        public Pair(TFirst First, TSecond Second)
        {
            this.First = First;
            this.Second = Second;
        }
        public TFirst First { get; set; }
        public TSecond Second { get; set; }
    }
}