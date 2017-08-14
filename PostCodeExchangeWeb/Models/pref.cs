namespace PostCodeExchangeWeb
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pref")]
    public partial class pref
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int prefcd { get; set; }

        [StringLength(10)]
        public string name { get; set; }
    }
}
