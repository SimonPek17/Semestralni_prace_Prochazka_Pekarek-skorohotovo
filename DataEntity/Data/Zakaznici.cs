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
    [Table("Zakaznici")]
    public class Zakaznici : BaseModel
    {
        [Key]
        [Column("ID_Zakaznik")]
        public int ID_Zakaznik { get; set; }

        [Required(ErrorMessage = "Jméno je povinné pole")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Jmeno { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné pole")]
        [StringLength(50, ErrorMessage = "Maximální délka je 50 znaků")]
        public string Prijmeni { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum narození je povinné pole")]
        [Column(TypeName = "date")]
        public DateTime DatumNarozeni { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Adresa je povinné pole")]
        [StringLength(100, ErrorMessage = "Maximální délka je 100 znaků")]
        public string Adresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonní číslo je povinné pole")]
        [StringLength(20, ErrorMessage = "Maximální délka je 20 znaků")]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail je povinné pole")]
        [StringLength(100, ErrorMessage = "Maximální délka je 100 znaků")]
        [EmailAddress(ErrorMessage = "Zadejte platnou e-mailovou adresu")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Počet pronájmů je povinné pole")]
        [Range(0, int.MaxValue, ErrorMessage = "Počet pronájmů nesmí být záporný")]
        public int PocetPronajmu { get; set; } = 0;

        // RELACE: Zakaznici (1) -> (N) Pronajmy
        public virtual ICollection<Pronajmy>? Pronajmy { get; set; }
    }
}
