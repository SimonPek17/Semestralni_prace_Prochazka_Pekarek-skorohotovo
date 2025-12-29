using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity.Data.Enum
{
    public class Enums
    {

        public enum Stav
        {
            [Description("Dostupné")]
            Dostupne = 0,

            [Description("Zapůjčené")]
            Zapujcene = 1,

            [Description("V servisu")]
            Vservisu = 2
        }

        public enum PronajemStav
        {
            [Description("Aktivní")]
            Aktivni = 0,

            [Description("Ukončený")]
            Ukonceny = 1,

            [Description("Opožděný")]
            Opozdeny = 2
        }



    }
}
