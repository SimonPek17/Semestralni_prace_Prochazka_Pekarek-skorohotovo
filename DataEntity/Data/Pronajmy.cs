using DataEntity.Data.Base;
using DataEntity.Data.Enum;
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
    [Table("Pronajmy")]
    public class Pronajmy : BaseModel
    {
        [Key]
        [Column("ID_Pronajem")]
        public int ID_Pronajem { get; set; }

        [Required(ErrorMessage = "ID Vozidla je povinné pole")]
        [Column("ID_Auto")]
        public int ID_Auto { get; set; }

        [Required(ErrorMessage = "ID Zákazníka je povinné pole")]
        [Column("ID_Zakaznik")]
        public int ID_Zakaznik { get; set; }

        [Required(ErrorMessage = "Datum zahájení pronájmu je povinné pole")]
        [Column(TypeName = "date")]
        public DateTime DatumOd { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Datum ukončení pronájmu je povinné pole")]
        [Column(TypeName = "date")]
        public DateTime DatumDo { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Délka pronájmu je povinné pole")]
        [Range(1, int.MaxValue, ErrorMessage = "Délka pronájmu musí být alespoň 1 den")]
        public int DelkaDni { get; set; }

        [Required(ErrorMessage = "Celková cena je povinné pole")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Cena musí být kladné číslo")]
        public decimal CenaCelkem { get; set; }

        // RELACE: Pronajmy (N) -> (1) Vozidla přes ID_Auto
        [ForeignKey(nameof(ID_Auto))]
        public virtual Vozidla? Vozidlo { get; set; }

        // RELACE: Pronajmy (N) -> (1) Zakaznici přes ID_Zakaznik
        [ForeignKey(nameof(ID_Zakaznik))]
        public virtual Zakaznici? Zakaznik { get; set; }

        // RELACE: Pronajmy (1) -> (N) Pokuty
        public virtual ICollection<Pokuty>? Pokuty { get; set; }

        [NotMapped]
        public Enums.PronajemStav PronajemStav { get; set; }
    }
}
