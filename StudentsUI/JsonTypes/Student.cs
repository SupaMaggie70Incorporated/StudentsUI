using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentsUI.JsonTypes
{
    internal struct Student
    {
        public int Id = -1;
        public string FirstName = "";
        public string LastName = "";
        public string School = "";
        public string Discord = "";
        public string[] DeadNames = new string[] {};
        public School school;
        public string currentDeadName = "";
    }
}
