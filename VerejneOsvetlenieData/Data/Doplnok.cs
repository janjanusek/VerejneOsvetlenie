﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerejneOsvetlenieData.Data
{
    [SqlClass(TableName = "T_DOPLNKY", DisplayName = "doplnok", TableKey = "Id")]
    public class Doplnok
    {
        [SqlClass(ColumnName = "ID", DisplayName = null)]
        public int Id { get; set; }

        [SqlClass(ColumnName = "TYP_DOPLNKU", DisplayName = "typ doplnku", Length = 1)]
        public char TypDoplnku { get; set; }

        [SqlClass(ColumnName = "POPIS", Length = 500)]
        public string Popis { get; set; }

        [SqlClass(ColumnName = "DATUM_INSTALACIE", DisplayName = "dátum inštalácie", SpecialFormat = "d")]
        public DateTime DatumInstalacie { get; set; }

        [SqlClass(ColumnName = "DATUM_DEMONTAZE", DisplayName = "dátum demontáže", SpecialFormat = "d")]
        public DateTime DatumDemontaze { get; set; }
    }
}
