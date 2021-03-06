﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerejneOsvetlenieData.Data.Interfaces;

namespace VerejneOsvetlenieData.Data
{
    [SqlClass(TableName = "T_DOPLNKY", DisplayName = "doplnok", TableKey = "Id")]
    public class TDoplnok : SqlEntita
    {
        [SqlClass(ColumnName = "ID", DisplayName = null)]
        public int Id { get; set; }

        [SqlClass(ColumnName = "TYP_DOPLNKU", DisplayName = "typ doplnku", Length = 1)]
        public char TypDoplnku { get; set; }

        [SqlClass(ColumnName = "POPIS", Length = 500)]
        public string Popis { get; set; }

        [SqlClass(ColumnName = "CISLO", DisplayName = null)]
        public int Cislo { get; set; }

        [SqlClass(ColumnName = "DATUM_INSTALACIE", DisplayName = "dátum inštalácie", SpecialFormat = "d", IsDate = true)]
        public DateTime DatumInstalacie { get; set; }

        [SqlClass(ColumnName = "DATUM_DEMONTAZE", DisplayName = "dátum demontáže", SpecialFormat = "d", IsDate = true)]
        public string DatumDemontaze { get; set; }

        public TDoplnok()
        {
            Cislo = -1;
            DeleteEnabled = true;
        }

        public override bool Update()
        {
            if (string.IsNullOrEmpty(DatumDemontaze))
                return UseDbMethod(Databaza.UpdateDoplnokStlpu(Cislo, Id, TypDoplnku, Popis, DatumInstalacie));
            return UseDbMethod(Databaza.UpdateDoplnokStlpu(Cislo, Id, TypDoplnku, Popis, DatumInstalacie, DateTime.Parse(DatumDemontaze)));
        }

        public override bool Insert()
        {
            if (string.IsNullOrEmpty(DatumDemontaze))
                return UseDbMethod(Databaza.VlozDoplnokStlpu(Cislo,TypDoplnku, Popis, DatumInstalacie));
            return UseDbMethod(Databaza.VlozDoplnokStlpu(Cislo, TypDoplnku, Popis, DatumInstalacie, DateTime.Parse(DatumDemontaze)));
        }

        public override bool Drop()
        {
            return UseDbMethod(Databaza.ZmazDoplnokStlpu(Id, Cislo));
        }
    }
}
