namespace PostCodeExchangeWeb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("list")]
    public partial class list
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int? prefcd { get; set; }

        [StringLength(50)]
        public string pref { get; set; }

        public int? citycd { get; set; }

        [StringLength(50)]
        public string city { get; set; }

        public int? towncd { get; set; }

        [StringLength(50)]
        public string town { get; set; }

        [StringLength(50)]
        public string touri { get; set; }

        [StringLength(50)]
        public string choume { get; set; }

        [StringLength(50)]
        public string postcode { get; set; }
    }
}
