﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Db;
using VerejneOsvetlenieData.Annotations;
using VerejneOsvetlenieData.Data.Tables;

namespace VerejneOsvetlenieData.Data.Interfaces
{
    public abstract class SqlEntita : INotifyPropertyChanged
    {
        protected Databaza Databaza;
        public abstract bool Update();
        public abstract bool Insert();
        public abstract bool Drop();

        [SqlClass(DisplayName = null)]
        public string ErrorMessage { get; protected set; }
        [SqlClass(DisplayName = null)]
        public bool DeleteEnabled { get; set; }

        protected SqlEntita()
        {
            Databaza = new Databaza();
            DeleteEnabled = false;
        }

        /// <summary>
        /// entita sa načíta podľa svojho id
        /// </summary>
        /// <param name="paIdEntity"></param>
        /// <returns></returns>
        public virtual bool SelectPodlaId(object paIdEntity)
        {
            var atribut = SqlClassAttribute.ExtractSqlClassAttribute(this);
            var db = new Databaza();
            var stlpce = string.Join(", ", this.GetType().GetProperties()
                .Where(p => SqlClassAttribute.ExtractSqlClassAttribute(p)?.IsColumn == true
                && SqlClassAttribute.ExtractSqlClassAttribute(p)?.IsReference == false)
                .Select(p => SqlClassAttribute.ExtractSqlClassAttribute(p)?.ColumnName));
            var iterator =
                db.SpecialSelect(
                    $"select {stlpce} from {atribut.TableName} where {atribut.TableKey} = {paIdEntity}");
            var enumerator = iterator.GetEnumerator();
            if (enumerator.MoveNext())
            {
                MapujRiadokSelectu(enumerator.Current, this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Funguje to iba na selecty bez aliasov alebo alias musí byť ako názov stĺpca
        /// </summary>
        /// <returns></returns>
        public static SqlEntita MapujRiadokSelectu<TData>(Dictionary<string, object> paRiadok, TData paExistujucaEntita = null) where TData : SqlEntita
        {
            var entita = paExistujucaEntita ?? System.Activator.CreateInstance<TData>();
            var props = entita.GetType().GetProperties();
            foreach (var propertyInfo in props)
            {
                var atribut = SqlClassAttribute.ExtractSqlClassAttribute(propertyInfo);
                if (atribut != null && atribut.IsReference == false && paRiadok.ContainsKey(atribut.ColumnName))
                {
                    if (!string.IsNullOrEmpty(paRiadok[atribut.ColumnName]?.ToString()))
                    {
                        var hodnota = System.Convert.ChangeType(paRiadok[atribut.ColumnName], propertyInfo.PropertyType);
                        propertyInfo.SetValue(entita, hodnota);
                    }
                }
            }
            return entita;
        }

        public virtual IVystup GetSelectOnTableData()
        {
            var atribut = SqlClassAttribute.ExtractSqlClassAttribute(this);
            var stlpce = string.Join(", ", this.GetType().GetProperties()
                .Where(p => SqlClassAttribute.ExtractSqlClassAttribute(p)?.IsColumn == true
                            && SqlClassAttribute.ExtractSqlClassAttribute(p)?.IsReference == false)
                .Select(p => SqlClassAttribute.ExtractSqlClassAttribute(p)?.ColumnName));

            var select = $"select distinct {stlpce} from {atribut.TableName}";// a0 {string.Join(" ", tabulky.Select(t => $"join {t} a{++poradieAlias} on(a0.{atribut.TableKey} = a{poradieTab}.{kluce[poradieTab]})"))}";
            var vystup = new VystupSelect(select) { KlucovyStlpec = atribut.TableKey };
            return vystup;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool UseDbMethod(Vysledok v)
        {
            ErrorMessage = v.Popis;
            return !v.JeChyba;
        }
    }
}
