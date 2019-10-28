using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StoneChallenge.Bank.Domain.Models;
using StoneChallenge.Bank.Infra.Data.Mappings;

namespace StoneChallenge.Bank.Infra.Data.Context
{
    public class BankContext : IdentityDbContext
    {
        private readonly IHostingEnvironment _env;

        public BankContext(DbContextOptions<BankContext> options,
                           IHostingEnvironment env) : base(options)
        {
            _env = env;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionMap())
                        .ApplyConfiguration(new AccountMap())
                        .ApplyConfiguration(new CustomerMap());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //// get the configuration from the app settings
            var config = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            //define the database to use
            optionsBuilder.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }
    }
}
