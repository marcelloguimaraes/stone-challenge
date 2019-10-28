using System;

namespace StoneChallenge.Bank.Domain.Models
{
    public class Customer
    {
        public string CustomerId { get; set; }
        public string Cpf { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual Account Account { get; set; }

        protected Customer() { }

        public Customer(string customerId, string cpf, string name, DateTime birthDate)
        {
            CustomerId = customerId;
            Cpf = cpf;
            Name = name;
            BirthDate = birthDate;
        }
    }
}
