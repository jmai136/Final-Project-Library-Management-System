using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project___Library_Management_System.Actors
{
    public class Reader
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Reader(string username, string firstName, string lastName)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
