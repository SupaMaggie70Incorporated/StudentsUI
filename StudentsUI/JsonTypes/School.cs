using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsUI.JsonTypes
{
    internal struct School
    {
        public string Name = "";
        public List<Student> students = new List<Student>();
        public static School UnknownSchool = new School();
    }
}
