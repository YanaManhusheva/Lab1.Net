using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum Position
    {
       Regular,
       PM, 
       DevOps,  
       Developer, 
       Designer 
    }
    public class Employee : Person
    {
        private Position _position;
        public Position Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    throw new ArgumentException("The same position");
                _position = value;
            }
        }
        public List<Project> Projects { get; init; }

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
    
}



