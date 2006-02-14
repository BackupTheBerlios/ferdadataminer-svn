using System;
using System.Collections.Generic;
using System.Text;

namespace Ferda.Modules.Boxes.DataMiningCommon.Database
{
    public class ExplainDatabaseStructureCache : Ferda.Modules.Helpers.Caching.Cache
    {
        private DataMatrixInfo[] value = new DataMatrixInfo[0];

        /// <summary>
        /// Gets infos about tables in specified data source.
        /// </summary>
        /// <param name="boxIdentity">The box identity.</param>
        /// <param name="lastReloadTime">The last reload time (for force reload of the cached value).</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="acceptableTypesOfTables">The acceptable types of tables.</param>
        /// <returns></returns>
        public DataMatrixInfo[] Value(string boxIdentity, DateTimeT lastReloadTime, string connectionString, string[] acceptableTypesOfTables)
        {
            lock (this)
            {
                Dictionary<string, IComparable> cacheSetting = new Dictionary<string, IComparable>();

                cacheSetting.Add(Database.DatabaseBoxInfo.typeIdentifier + DatabaseBoxInfo.OdbcConnectionStringPropertyName, connectionString);

                //string[] to string (IComparable needed)
                string comparableAcceptableTypesOfTables =
                    Ferda.Modules.Helpers.Common.Others.StringArray2String(acceptableTypesOfTables);
                cacheSetting.Add(Database.DatabaseBoxInfo.typeIdentifier + DatabaseBoxInfo.AcceptableTypesOfTablesPropertyName, comparableAcceptableTypesOfTables);

                if (IsObsolete(lastReloadTime, cacheSetting))
                    value = Ferda.Modules.Helpers.Data.Database.Explain(connectionString, acceptableTypesOfTables, boxIdentity);

                return value;
            }
        }
    }
}