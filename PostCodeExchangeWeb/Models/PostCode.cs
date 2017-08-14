namespace PostCodeExchangeWeb
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PostCode : DbContext
    {
        public PostCode()
            : base("name=PostCode")
        {
        }

        public virtual DbSet<city> city { get; set; }
        public virtual DbSet<pref> pref { get; set; }
        public virtual DbSet<list> list { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<pref>()
                .Property(e => e.name)
                .IsFixedLength();
        }
    }
}
