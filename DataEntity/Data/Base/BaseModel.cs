using System;                                     // DateTime, výjimky (ArgumentException, NotSupportedException).
using System.Collections.Generic;                 // List<ValidationResult> pro shromažďování chyb.
using System.ComponentModel;                      // IDataErrorInfo – rozhraní pro validaci do WPF bindingů.
using System.ComponentModel.DataAnnotations;      // DataAnnotations – Required, StringLength, Range, Timestamp.
using System.Linq;                                // First() – použito při čtení první validační chyby.
using System.Linq.Expressions;                    // (Nepoužito; zůstává kvůli shodě s originálem.)
using System.Runtime.CompilerServices;            // (Nepoužito; zůstává.)
using System.Text;                                // (Nepoužito; zůstává.)
using System.Threading.Tasks;                     // (Nepoužito; zůstává.)

namespace DataEntity.Data.Base
{
    // Abstraktní základ pro entity (nelze přímo instanciovat).
    // Implementuje IDataErrorInfo, takže WPF umí vyčíst chybové hlášky pro jednotlivé vlastnosti.
    public abstract class BaseModel : IDataErrorInfo
    {

        #region "validace"                        
        // Skupina členů souvisejících s validací přes IDataErrorInfo.

        // IDataErrorInfo.Error – globální chybové hlášení objektu; zde záměrně „nepodporováno“
        // (WPF typicky používá indexer níže pro validaci konkrétní vlastnosti).
        string IDataErrorInfo.Error
        {
            get
            {
                //return null; // původní tiché chování
                throw new NotSupportedException("IDataErrorInfo.Error is not supported, use IDataErrorInfo.this[propertyName] instead.");
            }
        }

        // IDataErrorInfo.this[propertyName] – vrací text chyby pro konkrétní vlastnost.
        // WPF volá tuto indexerovou vlastnost při validaci bindingu (ValidatesOnDataErrors=True).
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                // return null; // původně bez chyby
                return OnValidate(propertyName);   // Přesměrujeme do vlastní metody, která použije DataAnnotations.
            }
        }

        // Metoda, která vyhodnotí validaci jedné vlastnosti podle jejích DataAnnotations atributů.
        protected virtual string OnValidate(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))                        // Ochrana – jméno vlastnosti musí dávat smysl.
                throw new ArgumentException("Invalid property name", propertyName);

            string error = string.Empty;                                   // Prázdný řetězec = bez chyby.

            var prop = GetType().GetProperty(propertyName);
            if (prop is null)
                return string.Empty; // neznámá vlastnost → žádná chyba pro WPF                    // a vyzvedneme její hodnotu z objektu.

            var results = new List<ValidationResult>(1);                   // Kolekce pro zachycení (první) validační chyby.
            var value = prop.GetValue(this, null); // Hodnota vlastnosti, kterou validujeme.

            var context = new ValidationContext(this, null, null)
            {        // Kontext říká: validujeme tento objekt…
                MemberName = propertyName                                  // …a tuto konkrétní vlastnost.
            };

            var result = Validator.TryValidateProperty(value, context, results); // Spustí kontrolu Required/Range/StringLength…

            if (!result)
            {
                var validationResult = results.FirstOrDefault();            // vezmeme první chybu bezpečně
                // zajistíme, že nepřiřadíme null do non-nullable stringu
                error = validationResult?.ErrorMessage ?? "Neznámá chyba validace.";
                // a vrátíme její text (čte ho WPF).
            }

            return error;                                                  // Bez chyby → prázdný string.
        }

        /// <summary>Vrátí všechny validační výsledky pro celý objekt (všechny vlastnosti).</summary>
        protected IReadOnlyList<ValidationResult> ValidateAll()
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, validateAllProperties: true);
            return results;
        }


        /// <summary>Počet chyb dle DataAnnotations.</summary>
        public int ErrorsCount => ValidateAll().Count;

        /// <summary>True, pokud objekt nemá žádné validační chyby.</summary>
        public bool IsValid => ErrorsCount == 0;

        #endregion

        [Timestamp]                                                        // EF: označí sloupec jako rowversion (concurrency token).
        public byte[] RowVersion { get; set; } = null!;                           // Slouží k detekci kolize zápisu.
        // FIX: EF ji naplní z DB (rowversion), kompilátor už nebude varovat

        public DateTime DatumVytvoreni { get; set; } = DateTime.Now;
        // Audit – kdy byla instance vytvořena.
    }
}