﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VerejneOsvetlenie {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ResourceVystupy {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceVystupy() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VerejneOsvetlenie.ResourceVystupy", typeof(ResourceVystupy).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select id_ulice, nazov, mesto
        ///from s_ulica u
        ///group by id_ulice, nazov, mesto
        ///having
        ///(
        ///select count(id_lampy) 
        ///from s_stlp ss
        ///left join s_lampa_na_stlpe ls using(cislo)
        ///where ls.stav = &apos;N&apos; and ls.datum_demontaze is null
        ///and ss.id_ulice = u.id_ulice
        ///group by ss.id_ulice ) / (
        ///select count(*) 
        ///from s_stlp ss
        ///left join s_lampa_na_stlpe ls using(cislo)
        ///where ls.datum_demontaze is null
        ///and ss.id_ulice = u.id_ulice
        ///group by ss.id_ulice) &gt; 0.15;.
        /// </summary>
        public static string vystup_4 {
            get {
                return ResourceManager.GetString("vystup_4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select cislo, svietivost from(
        ///
        ///select cislo, sum(svietivost_lampy(id_lampy)) svietivost , row_number() over(order by sum(svietivost_lampy(id_lampy)) asc) dr
        ///from s_stlp
        ///join s_lampa_na_stlpe using(cislo)
        ///where datum_demontaze is null and stav = &apos;S&apos;
        ///group by cislo
        ///) tb
        ///where (dr / (select count(*) from s_stlp)* 100 ) &lt;= 10;
        ///.
        /// </summary>
        public static string vystup_7 {
            get {
                return ResourceManager.GetString("vystup_7", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select id_typu, svietivost from s_lampa_na_stlpe
        ///join s_lampa using(id_typu)
        ///where id_lampy = (
        ///  select id_lampy from s_obsluha_lampy
        ///  join s_servis using(id_sluzby)
        ///  group by id_lampy
        ///  having count(id_lampy) = (
        ///    select max(count(id_lampy)) from s_obsluha_lampy
        ///    join s_servis using(id_sluzby)
        ///    group by id_lampy 
        ///  )
        ///);.
        /// </summary>
        public static string vystup_D2 {
            get {
                return ResourceManager.GetString("vystup_D2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select cislo, id_ulice, poradie from s_stlp ss 
        ///where not exists(
        /// select &apos;x&apos; from s_info si
        ///  where si.datum &gt; add_months(sysdate, -12) and
        ///  ss.cislo = si.cislo
        ///);.
        /// </summary>
        public static string vystup_D5 {
            get {
                return ResourceManager.GetString("vystup_D5", resourceCulture);
            }
        }
    }
}
