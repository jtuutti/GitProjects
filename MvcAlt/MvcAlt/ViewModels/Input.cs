using System;
using System.Globalization;

namespace MvcAlt.ViewModels
{
    public class Input
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "({0}) {1} {2}, age: {3}", ID, FirstName, LastName, Age);
        }
    }
}
