using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using Ferda.Modules.Boxes.DataMiningCommon.Database;

namespace Ferda.Modules.Helpers.Data
{
    /// <summary>
    /// This class contains static methods for working with databases.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Tests if it is possible to create <see cref="T:System.Data.Odbc.OdbcConnection"/> 
        /// with <c>odbcConnectionString</c>.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for the test.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <exception cref="T:Ferda.Modules.BadParamsError"/>
        public static void TestConnectionString(string odbcConnectionString, string boxIdentity)
        {
            //throws Ferda.Modules.Boxes.DataMiningCommon.Database.BadConnectionStringError
            Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);
        }

        /// <summary>
        /// Gets names of publishable 
        /// (<see cref="M:Ferda.Modules.Helpers.Data.Database.IsTableTypePublishable(System.String,System.String[])"/>) 
        /// tables in database given by <c>odbcConnectionString</c>.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for test.</param>
        /// <param name="acceptableTypesOfTables">The acceptable types of the tables. Iff <c>null</c> than system and temporary tables are not accepted.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <returns>Array of names of tables in current DB.</returns>
        /// <exception cref="T:Ferda.Modules.BadParamsError"/>
        public static string[] GetTables(string odbcConnectionString, string[] acceptableTypesOfTables, string boxIdentity)
        {
            //get connection
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            //get schema
            DataTable dataTable = conn.GetSchema("TABLES");

            //result variable
            List<string> dataMatrixNames = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                //only publishable tables and views are added to result
                if (IsTableTypePublishable(row["TABLE_TYPE"].ToString(), acceptableTypesOfTables))
                    dataMatrixNames.Add(row["TABLE_NAME"].ToString());
            }
            return dataMatrixNames.ToArray();
        }

        /// <summary>
        /// Gets all possible types of data tables.
        /// </summary>
        /// <returns>Array of strings: ALIAS, TABLE, SYNONYM, SYSTEM TABLE, VIEW,
        /// GLOBAL TEMPORARY, LOCAL TEMPORARY, EXTERNAL TABLE, SYSTEM VIEW.</returns>
        public static string[] GetAllPossibleTableTypes()
        {
            return new string[] { "ALIAS", "TABLE", "SYNONYM", "SYSTEM TABLE", "VIEW",
                "GLOBAL TEMPORARY", "LOCAL TEMPORARY", "EXTERNAL TABLE", "SYSTEM VIEW"};
        }

        /// <summary>
        /// Tests if table type is publishable i.e. the table (or view) is not system or temporary.
        /// </summary>
        /// <param name="tableType">A table type.</param>
        /// <param name="publishableTypesOfTables">The publishable types of tables. Iff <c>null</c> than system and temporry tables are not publishable.</param>
        /// <returns>
        /// False iff table (or view) is not acceptable.
        /// </returns>
        /// <remarks>
        /// There are many types of tables: ALIAS, TABLE, SYNONYM, SYSTEM TABLE, VIEW,
        /// GLOBAL TEMPORARY, LOCAL TEMPORARY, EXTERNAL TABLE, or SYSTEM VIEW.
        /// Iff <c>publishableTypesOfTables</c> is <c>null</c> than
        /// TEMPORARY and SYSTEM tables are not publishable.
        /// </remarks>
        public static bool IsTableTypePublishable(string tableType, string[] publishableTypesOfTables)
        {
            //Iff publishableTypesOfTables == null than system and temporary tables are not publishable.
            if (publishableTypesOfTables == null || publishableTypesOfTables.Length == 0)
                publishableTypesOfTables =
                    new string[] { "TABLE", "VIEW", "ALIAS", "SYNONYM", "EXTERNAL TABLE" };

            //temporary and system tables (or views) are not publishable
            foreach (string publishableType in publishableTypesOfTables)
            {
                if (0 == String.Compare(tableType, publishableType, true))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the connection info. (some fields of <see cref="T:System.Data.Odbc.OdbcConnection"/>)
        /// If specified <c>odbcConnectionString</c> doesn`t work empty strings are returned in
        /// result fields.
        /// </summary>
        /// <param name="odbcConnectionString">The ODBC connection string.</param>
        /// <param name="boxIdentity">The box identity.</param>
        /// <returns>
        /// <see cref="Ferda.Modules.Boxes.DataMiningCommon.Database.ConnectionInfo"/>
        /// </returns>
        public static ConnectionInfo GetConnectionInfo(string odbcConnectionString, string boxIdentity)
        {
            ConnectionInfo result = new ConnectionInfo();
            try
            {
                OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);
                result.databaseName = conn.Database;
                result.dataSource = conn.DataSource;
                result.driver = conn.Driver;
                try
                {
                    result.serverVersion = conn.ServerVersion;
                }
                catch (InvalidOperationException)
                {
                    result.serverVersion = String.Empty;
                }
            }
            catch (Ferda.Modules.BadParamsError ex)
            {
                if (ex.restrictionType != Ferda.Modules.restrictionTypeEnum.DbConnectionString)
                    throw ex;
                result.databaseName = String.Empty;
                result.dataSource = String.Empty;
                result.driver = String.Empty;
                result.serverVersion = String.Empty;
            }
            return result;
        }

        /// <summary>
        /// Gets information about publishable (<see cref="M:Ferda.Modules.Helpers.Data.Database.IsTableTypePublishable(System.String)"/>) tables in database given by <c>odbcConnectionString</c>.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for test.</param>
        /// <param name="acceptableTypesOfTables">The acceptable types of the tables. Iff <c>null</c> than system and temporary tables are not accepted.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <returns>
        /// Array of <see cref="T:Ferda.Modules.Boxes.DataMiningCommon.Database.DataMatrixInfo"/>.
        /// </returns>
        /// <exception cref="T:Ferda.Modules.BadParamsError"/>
        public static DataMatrixSchemaInfo[] Explain(string odbcConnectionString, string[] acceptableTypesOfTables, string boxIdentity)
        {
            //get connection
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            //get schema
            DataTable schema = conn.GetSchema("TABLES");

            //prepare OdbcCommand for "SELECT COUNT(1) FROM ..." query
            OdbcCommand odbcCommand = new OdbcCommand();
            odbcCommand.Connection = conn;

            //result variable
            List<DataMatrixSchemaInfo> result = new List<DataMatrixSchemaInfo>();

            foreach (DataRow row in schema.Rows)
            {
                //only publishable tables or views are added to result
                if (IsTableTypePublishable(row["TABLE_TYPE"].ToString(), acceptableTypesOfTables))
                {
                    DataMatrixSchemaInfo dataMatrixSchemaInfo = new DataMatrixSchemaInfo();
                    dataMatrixSchemaInfo.name = row["TABLE_NAME"].ToString();
                    dataMatrixSchemaInfo.type = row["TABLE_TYPE"].ToString();
                    dataMatrixSchemaInfo.remarks = row["REMARKS"].ToString();

                    //complete OdbcCommand and execute
                    odbcCommand.CommandText = "SELECT COUNT(1) FROM " + "`" + dataMatrixSchemaInfo.name + "`";
                    dataMatrixSchemaInfo.rowCount = Convert.ToInt32(odbcCommand.ExecuteScalar());

                    result.Add(dataMatrixSchemaInfo);
                }
            }
            return result.ToArray();
        }
    }
}