using DataEntity.Data.Base;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity.Data
{

    [AddINotifyPropertyChangedInterface()]
    [Table("Pokuty")]
    public class Pokuty : BaseModel
    {

        [Key]
        [Column("ID_Pokuta")]
        public int ID_Pokuta { get; set; }

        [Required(ErrorMessage = "Vazba na pronájem je povinná")]
        [Column("ID_Pronajem")]
        public int ID_Pronajem { get; set; }

        [Required(ErrorMessage = "Datum udělení pokuty je povinné pole")]
        [Column(TypeName = "date")]
        public DateTime DatumPokuty { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Částka pokuty je povinné pole")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Částka musí být kladné číslo")]
        public decimal Castka { get; set; }

        [Required(ErrorMessage = "Důvod pokuty je povinné pole")]
        [StringLength(255, ErrorMessage = "Maximální délka důvodu je 255 znaků")]
        public string Duvod { get; set; } = string.Empty;

        // RELACE: Pokuty (N) -> (1) Pronajmy přes ID_Pronajem
        [ForeignKey(nameof(ID_Pronajem))]
        public virtual Pronajmy? Pronajem { get; set; }
    }
}
