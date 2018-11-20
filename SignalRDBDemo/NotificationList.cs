namespace SignalRDBDemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RepoTable")]
    public partial class NotificationList
    {
        [StringLength(10)]
        public string ID { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public DateTime? DateStamp { get; set; }
    }
}
