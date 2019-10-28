using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoneChallenge.Bank.Domain.Models;
using System;

namespace StoneChallenge.Bank.Infra.Data.Mappings
{
    public class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.CustomerId);

            builder.Property(c => c.CustomerId)
                .HasColumnType("varchar(255)")
                .HasColumnName("Id");

            builder.Property(c => c.Cpf)
                .HasColumnType("varchar(11)")
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.BirthDate)
                .HasColumnType("datetime")
                .IsRequired();

            builder.ToTable("Customer");

            //builder.HasData(
            //    new Customer(
            //        customerId: "b6f5a05b-a466-4bcc-977a-507e9745a227",
            //        cpf: "16400206752",
            //        name: "Marcello",
            //        birthDate: new DateTime(1995, 3, 16)),
            //    new Customer(
            //        customerId: "b84385d7-6b8c-459e-bf29-db46857fd40c",
            //        cpf: "56645114585",
            //        name: "Thiago",
            //        birthDate: new DateTime(1990, 5, 20)));

            //builder.HasOne(c => c.Account)
            //       .WithOne(a => a.Customer)
            //       .HasForeignKey<Account>(s => s.CustomerId);
        }

    }
}
