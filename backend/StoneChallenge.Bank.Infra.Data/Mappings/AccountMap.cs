using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoneChallenge.Bank.Domain.Models;

namespace StoneChallenge.Bank.Infra.Data.Mappings
{
    public class AccountMap : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.AccountId);

            builder.Property(a => a.AccountId)
                .ValueGeneratedOnAdd()
                .HasColumnType("text")
                .HasColumnName("Id");

            builder.Property(c => c.AccountNumber)
                .HasColumnType("integer(6)")
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(c => c.Agency)
                .HasColumnType("integer(4)")
                .HasMaxLength(4)
                .IsRequired();

            builder.Property(c => c.Balance)
                .HasColumnType("double")
                .IsRequired();

            builder.Property(c => c.CustomerId)
                .HasColumnType("text")
                .IsRequired();

            builder.Property(c => c.UserId)
                .HasColumnType("text")
                .IsRequired();

            builder.HasOne(a => a.Customer)
                   .WithOne(c => c.Account)
                   .HasForeignKey<Account>(a => a.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            //builder.Ignore(a => a.BankFee);

            //builder.HasData(new Account("47a77e3a-4db7-44bc-aa01-bcab6b58e191",
            //                            348710,
            //                            3032,
            //                            "b6f5a05b-a466-4bcc-977a-507e9745a227"){ Balance = 100.0 },
            //                new Account("d3067022-5fab-4a42-9c80-6dab8b231a21",
            //                265445,
            //                3031,
            //                "b84385d7-6b8c-459e-bf29-db46857fd40c"){ Balance = 100.0 });

            builder.ToTable("Account");
        }
    }
}
