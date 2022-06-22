using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public abstract class Person
    {
        public string Name { get; init; }
        public string Surname { get; init;  }

        public override bool Equals(object obj)
        {
            if (obj is Person person)
                return Name == person.Name;
            return false;
        }

        public override int GetHashCode() => Name.GetHashCode();
        
    }
}
