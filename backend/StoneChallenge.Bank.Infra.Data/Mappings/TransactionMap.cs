using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoneChallenge.Bank.Domain.Models;
using System;

namespace StoneChallenge.Bank.Infra.Data.Mappings
{
    public class TransactionMap : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.TransactionId)
                .ValueGeneratedOnAdd()
                .HasColumnType("text")
                .HasColumnName("Id");

            builder.Property(t => t.TransactionType)
                .HasConversion(
                    v => v.ToString(),
                    v => (TransactionType)Enum.Parse(typeof(TransactionType), v))
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(t => t.Date)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(t => t.Value)
                .HasColumnType("double")
                .IsRequired();

            builder.Property(t => t.Note)
                .HasColumnType("varchar(255)");

            builder.HasOne(t => t.Account)
                   .WithMany(a => a.Transactions)
                   .HasForeignKey(t => t.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Transaction");
        }
    }
}
