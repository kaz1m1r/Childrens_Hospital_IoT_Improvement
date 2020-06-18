using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary.Logic
{
    public class Class2
    {
        string _apples;

        public Class2(int count)
            : this(Convert.ToString(count))
        {
            
        }

        private Class2(string apples = "iuashd")
        {
            _apples = apples;
        }

        public void MoreBullshit()
        {
            Console.WriteLine("Mooooaaar");
        }
    }
}
