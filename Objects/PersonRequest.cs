using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonWebApp.Objects
{
    public class PersonRequest
    {
        public PersonRequestSettings Settings { get; set; }
        public List<Person> Persons { get; set; }
    }

    public enum RequestTypes
    {
        Create = 1,
        Update,
        Delete,
        Exist
    }
}
