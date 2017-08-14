namespace PostCodeExchangeWeb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("city")]
    public partial class city
    {
        public int prefcd { get; set; }

        [StringLength(50)]
        public string pref { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int citycd { get; set; }

        [Column("city")]
        [StringLength(50)]
        public string city1 { get; set; }

        [StringLength(50)]
        public string ryaku { get; set; }
    }
}
