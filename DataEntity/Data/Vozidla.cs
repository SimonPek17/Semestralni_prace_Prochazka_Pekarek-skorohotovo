using DataEntity.Data.Base;
using DataEntity.Data.Enum;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataEntity.Data
{

    [AddINotifyPropertyChangedInterface()]
    [Table("Vozidla")]
    public class Vozidla : BaseModel
    {
        [Key]
        [Column("ID_Auto")]
        public int ID_Auto { get; set; }

        [Required(ErrorMessage = "Státní poznávací značka je povinné pole")]
        [StringLength(10, ErrorMessage = "Maximální délka je 10 znaků")]
        public string SPZ { get; set; } = string.Empty;

        [Required(ErrorMessage = "Značka vozidla je povinné pole")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Znacka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model vozidla je povinné pole")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rok výroby je povinné pole")]
        [Range(1900, 3000, ErrorMessage = "Rok výroby musí být platná hodnota")]
        public int RokVyroby { get; set; }

        [Required(ErrorMessage = "Stav tachometru je povinné pole")]
        [Range(0, int.MaxValue, ErrorMessage = "Stav tachometru musí být nezáporné číslo")]
        public int StavTachometru { get; set; }

        [Required(ErrorMessage = "Denní sazba je povinné pole")]
        [Column(TypeName = "decimal(10, 2)")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Denní sazba musí být kladné číslo")]
        public decimal DenniSazba { get; set; }

        [Required(ErrorMessage = "Stav vozidla je povinné pole")]
        [StringLength(20)]
        public Enums.Stav Stav { get; set; }

        // RELACE: Vozidla (1) -> (N) Pronajmy
        public virtual ICollection<Pronajmy>? Pronajmy { get; set; }

    }
}
