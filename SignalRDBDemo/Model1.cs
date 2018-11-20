namespace SignalRDBDemo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class NotificationEntities : DbContext
    {
        public NotificationEntities()
            : base("name=NotificationEntities")
        {
        }

        public virtual DbSet<NotificationList> NotificationLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotificationList>()
                .Property(e => e.ID)
                .IsFixedLength();
        }
    }
}
