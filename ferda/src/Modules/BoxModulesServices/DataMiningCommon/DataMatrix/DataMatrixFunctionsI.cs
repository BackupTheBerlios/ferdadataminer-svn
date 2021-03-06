// DataMatrixFunctionsI.cs - functions object for data matrix box module
//
// Author: Tomáš Kuchař <tomas.kuchar@gmail.com>
//
// Copyright (c) 2005 Tomáš Kuchař
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using Ferda.Modules.Boxes.DataMiningCommon.Database;

namespace Ferda.Modules.Boxes.DataMiningCommon.DataMatrix
{
    class DataMatrixFunctionsI : DataMatrixFunctionsDisp_, IFunctions
    {
        /// <summary>
        /// The box module.
        /// </summary>
        protected BoxModuleI boxModule;
        //protected IBoxInfo boxInfo;

        #region IFunctions Members
        /// <summary>
        /// Sets the <see cref="T:Ferda.Modules.BoxModuleI">box module</see>
        /// and the <see cref="T:Ferda.Modules.Boxes.IBoxInfo">box info</see>.
        /// </summary>
        /// <param name="boxModule">The box module.</param>
        /// <param name="boxInfo">The box info.</param>
        public void setBoxModuleInfo(BoxModuleI boxModule, IBoxInfo boxInfo)
        {
            this.boxModule = boxModule;
            //this.boxInfo = boxInfo;
        }
        #endregion

        #region Properties
        public string DataMatrixName
        {
            get
            {
                return this.boxModule.GetPropertyString(DataMatrixBoxInfo.DataMatrixNamePropertyName);
            }
        }

        public string[] PrimaryKeyColumns
        {
            get
            {
                return Ferda.Modules.Helpers.Common.Csv.Csv2Strings(
                    this.boxModule.GetPropertyString(DataMatrixBoxInfo.PrimaryKeyColumnsPropertyName));
            }
        }

        private RecordsCountCache recordsCountCache = new RecordsCountCache();
        public long RecordsCount
        {
            get
            {
                DatabaseInfo databaseInfo = GetDatabaseFunctionsPrx().getDatabaseInfo();
                return recordsCountCache.Value(
                    boxModule.StringIceIdentity,
                    databaseInfo.lastReloadInfo,
                    databaseInfo.odbcConnectionString,
                    DataMatrixName);
            }
        }
        #endregion

        #region Functions
        public override DataMatrixInfo getDataMatrixInfo(Ice.Current __current)
        {
            DatabaseInfo databaseInfo = this.GetDatabaseFunctionsPrx().getDatabaseInfo();

            DataMatrixInfo result = new DataMatrixInfo();

            result.database = databaseInfo;
            result.dataMatrixName = DataMatrixName;
            result.recordsCount = RecordsCount;
            result.primaryKeyColumns = PrimaryKeyColumns;
            result.explainDataMatrix = explain();

            return result;
        }

        private ColumnsNamesCache columnsNamesCache = new ColumnsNamesCache();
        public override string[] getColumnsNames(Ice.Current __current)
        {
            DatabaseInfo databaseInfo = GetDatabaseFunctionsPrx().getDatabaseInfo();
            return columnsNamesCache.Value(
                boxModule.StringIceIdentity,
                databaseInfo.lastReloadInfo,
                databaseInfo.odbcConnectionString,
                DataMatrixName);
        }

        private ExplainDataMatrixStructureCache explainDataMatrixStructureCache = new ExplainDataMatrixStructureCache();
        public override ColumnSchemaInfo[] explain(Ice.Current __current)
        {
            DatabaseInfo databaseInfo = GetDatabaseFunctionsPrx().getDatabaseInfo();
            return explainDataMatrixStructureCache.Value(
                boxModule.StringIceIdentity,
                databaseInfo.lastReloadInfo,
                databaseInfo.odbcConnectionString,
                DataMatrixName);
        }

        #endregion

        #region Sockets
        public DatabaseFunctionsPrx GetDatabaseFunctionsPrx()
        {
            return DatabaseFunctionsPrxHelper.checkedCast(
                SocketConnections.GetObjectPrx(boxModule, "Database")
                );
        }
        #endregion

        private string connectionString
        {
            get
            {
                return GetDatabaseFunctionsPrx().getDatabaseInfo().odbcConnectionString;
            }
        }
    }
}
