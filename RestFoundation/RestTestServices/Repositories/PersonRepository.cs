using System;
using System.Collections.Generic;
using RestTestContracts.Resources;

namespace RestTestServices.Repositories
{
    public class PersonRepository
    {
        private readonly List<Person> people = new List<Person>
        {
            new Person { Name = "John", Age = 51, Values = new[] { "Manager", "old" }, TimeStamp = DateTime.Now.AddDays(-55) },
            new Person { Name = "Mike", Age = 16, Values = new string[0], TimeStamp = DateTime.Now },
            new Person { Name = "Beth", Age = 32, Values = new[] { "Secretary" }, TimeStamp = DateTime.Now.AddYears(-1) },
            new Person { Name = "Saul", Age = 62, TimeStamp = DateTime.Now.AddMonths(-2) }
        };

        public IList<Person> GetAll()
        {
            return new List<Person>(people);
        }
    }
}
