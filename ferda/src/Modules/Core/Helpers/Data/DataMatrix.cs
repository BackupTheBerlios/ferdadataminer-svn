using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Odbc;
using Ferda.Modules.Boxes.DataMiningCommon.DataMatrix;

namespace Ferda.Modules.Helpers.Data
{
    /// <summary>
    /// This class contains static methods for working with data matrixes in databases.
    /// </summary>
    public static class DataMatrix
    {
        /// <summary>
        /// Tests if <c>dataMatrixName</c> is contained in database given by <c>odbcConnectionString</c>.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for connecting to database.</param>
        /// <param name="dataMatrixName">An unescaped name of data matrix for the test.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <exception cref="T:Ferda.Modules.BadParamsError"/>
        public static void TestDataMatrixExists(string odbcConnectionString, string dataMatrixName, string boxIdentity)
        {
            //throws exception if odbcConnectionString is wrong
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            try
            {
                dataMatrixName = SqlSecurity.SafeSqlObjectName(dataMatrixName);

                //try query "SELECT * FROM dataMatrixName WHERE 0"
                OdbcCommand command = new OdbcCommand("SELECT * FROM `" + dataMatrixName + "` WHERE 0", conn);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //throws exception if dataMatrixName is wrong
                //there is not data matrix (dataMatrixName) in given database
                throw Ferda.Modules.Exceptions.BadParamsError(ex, boxIdentity, "Cold not found data matrix in database!", Ferda.Modules.restrictionTypeEnum.DbTable);
            }
        }

        /// <summary>
        /// Tests if values in <c>primaryKeyColumns</c> are distinct.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for connecting to databse.</param>
        /// <param name="dataMatrixName">An unescaped name of data matrix.</param>
        /// <param name="primaryKeyColumns">An array of unescaped names of column(s) for test.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <exception cref="T:Ferda.Modules.BadParamsError">Thrown if <c>odbcConnectionString</c> or <c>dataMatrixName</c> or <c>primaryKeyColumns</c> parameter is wrong.</exception>
        public static void TestValuesInEnteredPrimaryKeyColumnsAreNotUniqueError(string odbcConnectionString, string dataMatrixName, string[] primaryKeyColumns, string boxIdentity)
        {
            //throws exception if odbcConnectionString is wrong
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            bool result = false;
            if (primaryKeyColumns != null && primaryKeyColumns.Length > 0)
            {
                //make a sequence "`PkColumn1`, `PkColumn2`, `PkColumn3`, ..."
                string subQuery = "";
                foreach (string columnName in primaryKeyColumns)
                {
                    string columnNameTmp = "`" + SqlSecurity.SafeSqlObjectName(columnName) + "`";
                    subQuery = (string.IsNullOrEmpty(subQuery)) ? columnNameTmp : subQuery + ", " + columnNameTmp;
                }

                //create SQL query " ... GROUP BY PkColumns HAVING COUNT(1) > 1"
                dataMatrixName = SqlSecurity.SafeSqlObjectName(dataMatrixName);
                string mySelectQuery = "SELECT " + subQuery + " FROM `" + dataMatrixName + "` GROUP BY " + subQuery + " HAVING COUNT(1) > 1";
                OdbcDataAdapter myDataAdapter = new OdbcDataAdapter(mySelectQuery, conn);
                DataSet myDataSet = new DataSet();

                //test if SQL query returned any rows
                try
                {
                    myDataAdapter.Fill(myDataSet);
                    result = !(myDataSet.Tables[0].Rows.Count > 1);
                }
                catch (OdbcException)
                {
                    //throws exception if dataMatrixName is wrong
                    TestDataMatrixExists(odbcConnectionString, dataMatrixName, boxIdentity);

                    //if columns are not in dataMatrix
                    //result = false;
                }
            }
            if (!result)
            {
                throw Ferda.Modules.Exceptions.BadParamsError(null, boxIdentity, "Values in entered primary key columns are not unique!", Ferda.Modules.restrictionTypeEnum.DbPrimaryKey);
            }
        }

        /// <summary>
        /// Gets number of records/rows in data matrix.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for connecting DB.</param>
        /// <param name="dataMatrixName">An unescaped name of data matrix.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <returns>Number of records/rows in data matrix.</returns>
        /// <exception cref="T:Ferda.Modules.BadParamsError">Thrown if <c>odbcConnectionString</c> or <c>dataMatrixName</c> parameter is wrong.</exception>
        public static long GetRecordsCount(string odbcConnectionString, string dataMatrixName, string boxIdentity)
        {
            //throws exception if odbcConnectionString is wrong
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            //execute query "SELECT COUNT(1) FROM dataMatrixName"
            dataMatrixName = SqlSecurity.SafeSqlObjectName(dataMatrixName);
            OdbcCommand command = new OdbcCommand(
                    "SELECT COUNT(1) FROM `" + dataMatrixName + "`",
                    conn);
            try
            {
                return Convert.ToInt64(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                //throws exception if dataMatrixName is wrong
                TestDataMatrixExists(odbcConnectionString, dataMatrixName, boxIdentity);

                //or other reason for exception
                throw Ferda.Modules.Exceptions.BadParamsUnexpectedReasonError(ex, boxIdentity);
            }
        }

        /// <summary>
        /// Get information about columns in current data matrix.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for connecting DB.</param>
        /// <param name="dataMatrixName">An unescaped name of data matrix.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <returns>An array of <see cref="T:Ferda.Modules.Boxes.DataMiningCommon.DataMatrix.ColumnInfo"/>.</returns>
        /// <remarks><see cref="T:Ferda.Modules.Boxes.DataMiningCommon.DataMatrix.ColumnInfo"/> is similar to result of <see cref="M:System.Data.Odbc.OdbcDataReader.GetSchemaTable"/>.</remarks>
        /// <exception cref="T:Ferda.Modules.BadParamsError">Thrown if <c>odbcConnectionString</c> or <c>dataMatrixName</c> parameter is wrong.</exception>
        public static ColumnSchemaInfo[] Explain(string odbcConnectionString, string dataMatrixName, string boxIdentity)
        {
            //throws exception if odbcConnectionString is wrong
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            List<ColumnSchemaInfo> result = new List<ColumnSchemaInfo>();
            ColumnSchemaInfo columnSchemaInfo;

            //create SQL (empty ... WHERE 0) query over dataMatrixName (this select given data matrix)
            dataMatrixName = SqlSecurity.SafeSqlObjectName(dataMatrixName);
            OdbcCommand odbcCommand = new OdbcCommand("SELECT * FROM `" + dataMatrixName + "` WHERE 0", conn);

            try
            {
                //see documentation for System.Data.Odbc.OdbcDataReader.GetSchemaTable()
                foreach (DataRow row in odbcCommand.ExecuteReader().GetSchemaTable().Rows)
                {
                    columnSchemaInfo = new ColumnSchemaInfo();
                    columnSchemaInfo.name = row["ColumnName"].ToString();
                    columnSchemaInfo.columnOrdinal = Convert.ToInt32(row["ColumnOrdinal"]);
                    columnSchemaInfo.columnSize = Convert.ToInt32(row["ColumnSize"]);
                    columnSchemaInfo.numericPrecision = Convert.ToInt32(row["NumericPrecision"]);
                    columnSchemaInfo.numericScale = Convert.ToInt32(row["NumericScale"]);
                    columnSchemaInfo.providerType = Convert.ToInt32(row["ProviderType"]);
                    columnSchemaInfo.dataType = row["DataType"].ToString();
                    columnSchemaInfo.isLong = Convert.ToBoolean(row["IsLong"]);
                    columnSchemaInfo.allowDBNull = Convert.ToBoolean(row["AllowDBNull"]);
                    columnSchemaInfo.isReadOnly = Convert.ToBoolean(row["IsReadOnly"]);
                    columnSchemaInfo.isRowVersion = Convert.ToBoolean(row["IsRowVersion"]);
                    columnSchemaInfo.isUnique = Convert.ToBoolean(row["IsUnique"]);
                    columnSchemaInfo.isKey = Convert.ToBoolean(row["IsKey"]);
                    columnSchemaInfo.isAutoIncrement = Convert.ToBoolean(row["IsAutoIncrement"]);
                    result.Add(columnSchemaInfo);
                }
            }
            catch (Exception ex)
            {
                //throws exception if dataMatrixName is wrong
                TestDataMatrixExists(odbcConnectionString, dataMatrixName, boxIdentity);

                //or other reason for exception
                throw Ferda.Modules.Exceptions.BadParamsUnexpectedReasonError(ex, boxIdentity);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get names of columns in data matrix.
        /// </summary>
        /// <param name="odbcConnectionString">An ODBC connection string for connecting DB.</param>
        /// <param name="dataMatrixName">An unescaped name of data matrix.</param>
        /// <param name="boxIdentity">An identity of BoxModule.</param>
        /// <returns>An array of names of columns in data matrix.</returns>
        /// <exception cref="T:Ferda.Modules.BadParamsError">Thrown if <c>odbcConnectionString</c> or <c>dataMatrixName</c> parameter is wrong.</exception>
        public static string[] GetColumns(string odbcConnectionString, string dataMatrixName, string boxIdentity)
        {
            //throws exception if odbcConnectionString is wrong
            OdbcConnection conn = Ferda.Modules.Helpers.Data.OdbcConnections.GetConnection(odbcConnectionString, boxIdentity);

            List<string> result = new List<string>();

            //create SQL (empty ... WHERE 0) query over dataMatrixName
            dataMatrixName = SqlSecurity.SafeSqlObjectName(dataMatrixName);
            OdbcCommand odbcCommand = new OdbcCommand("SELECT * FROM `" + dataMatrixName + "` WHERE 0", conn);

            try
            {
                //see documentation of System.Data.Odbc.OdbcDataReader.GetSchemaTable()
                foreach (DataRow row in odbcCommand.ExecuteReader().GetSchemaTable().Rows)
                {
                    result.Add(row["ColumnName"].ToString());
                }
            }
            catch (Exception ex)
            {
                //throws exception if dataMatrixName is wrong
                TestDataMatrixExists(odbcConnectionString, dataMatrixName, boxIdentity);

                //or other reason for exception
                throw Ferda.Modules.Exceptions.BadParamsUnexpectedReasonError(ex, boxIdentity);
            }
            return result.ToArray();
        }
    }
}