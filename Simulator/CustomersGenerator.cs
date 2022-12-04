using Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class CustomersGenerator
    {
        int counter;
        Random r = new Random();

        public Customer GenerateCustomer()
        {
            return new Customer() { Id = counter += 1, OrderingTime = 5, AvailableMoney = GenerateMoney()};
        }


        private int GenerateMoney()
        {
            return r.Next(55,80);
        }

    }
}
