// DerivedColumnFunctionsI.cs - functions object for derived column box module
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
using Ferda.Modules.Boxes.DataMiningCommon.DataMatrix;
using Ferda.Modules.Boxes.DataMiningCommon.Column;

namespace Ferda.Modules.Boxes.DataMiningCommon.DerivedColumn
{
    class DerivedColumnFunctionsI : DerivedColumnFunctionsDisp_, IFunctions
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
        protected string columnSelectExpression
        {
            get
            {
                return this.boxModule.GetPropertyString("Formula");
            }
        }

        #region Cache: StatisticsInfo
        private class statisticsInfoCache : Ferda.Modules.Helpers.Caching.Cache
        {
            private StatisticsInfo value;
            public StatisticsInfo Value(string boxIdentity, DateTimeT lastReloadTime, string connectionString, string dataMatrixName, long dataMatrixRecordsCount, string columnSelectExpression, ValueSubTypeEnum columnValueSubType)
            {
                lock (this)
                {
                    Dictionary<string, IComparable> cacheSetting = new Dictionary<string, IComparable>();
                    cacheSetting.Add(Database.DatabaseBoxInfo.typeIdentifier + Database.DatabaseBoxInfo.OdbcConnectionStringPropertyName, connectionString);
                    cacheSetting.Add(DataMatrix.DataMatrixBoxInfo.typeIdentifier + DataMatrix.DataMatrixBoxInfo.DataMatrixNamePropertyName, dataMatrixName);
                    cacheSetting.Add(DataMatrix.DataMatrixBoxInfo.typeIdentifier + DataMatrix.DataMatrixBoxInfo.RecordCountPropertyName, dataMatrixRecordsCount);
                    cacheSetting.Add(Column.ColumnBoxInfo.typeIdentifier + Column.ColumnBoxInfo.ColumnSelectExpressionPropertyName, columnSelectExpression);
                    if (IsObsolete(lastReloadTime, cacheSetting))
                    {
                        value = new StatisticsInfo();
                        value = Ferda.Modules.Helpers.Data.Column.GetStatistics(connectionString, dataMatrixName, columnSelectExpression, columnValueSubType, boxIdentity);
                    }
                    if (value == null)
                        value = new StatisticsInfo();
                    return value;
                }
            }
        }
        private statisticsInfoCache statisticsStructCached = new statisticsInfoCache();
        #endregion
        protected StatisticsInfo getStatisticsStruct(DataMatrixInfo dataMatrixInfo)
        {
            return statisticsStructCached.Value(boxModule.StringIceIdentity, dataMatrixInfo.database.lastReloadInfo, dataMatrixInfo.database.odbcConnectionString, dataMatrixInfo.dataMatrixName, dataMatrixInfo.recordsCount, columnSelectExpression, columnValueSubType);
        }
        protected StatisticsInfo getStatisticsStruct()
        {
            DataMatrixInfo dataMatrixInfo = getDataMatrixFunctionsPrx().getDataMatrixInfo();
            return getStatisticsStruct(dataMatrixInfo);
        }

        protected ValueSubTypeEnum columnValueSubType
        {
            get
            {
                return (ValueSubTypeEnum)(Enum.Parse(typeof(ValueSubTypeEnum), this.boxModule.GetPropertyString("ValueSubType")));
            }
        }
        #endregion

        #region Functions
        public override ColumnInfo getColumnInfo(Ice.Current __current)
        {
            DataMatrixInfo dataMatrixInfo = this.getDataMatrixFunctionsPrx().getDataMatrixInfo();
            string connectionString = dataMatrixInfo.database.odbcConnectionString;
            string dataMatrixName = dataMatrixInfo.dataMatrixName;
            string columnSelectExpression = this.columnSelectExpression;
            Ferda.Modules.Helpers.Data.Column.TestColumnSelectExpression(
                connectionString,
                dataMatrixName,
                columnSelectExpression,
                boxModule.StringIceIdentity);
            ColumnInfo result = new ColumnInfo();
            result.statistics = getStatisticsStruct(dataMatrixInfo);
            result.dataMatrix = dataMatrixInfo;
            result.columnSelectExpression = columnSelectExpression;
            result.columnSubType = columnValueSubType;
            result.columnType = ColumnTypeEnum.Derived;
            return result;
        }

        public override string[] getDistinctValues(Ice.Current __current)
        {
            DataMatrixInfo dataMatrixInfo = getDataMatrixFunctionsPrx().getDataMatrixInfo();
            return Helpers.Data.Column.GetDistinctsStringSeq(dataMatrixInfo.database.odbcConnectionString, dataMatrixInfo.dataMatrixName, columnSelectExpression, boxModule.StringIceIdentity);
        }
        #endregion

        #region Sockets
        private DataMatrixFunctionsPrx getDataMatrixFunctionsPrx()
        {
            return DataMatrixFunctionsPrxHelper.checkedCast(
                SocketConnections.GetObjectPrx(boxModule, "DataMatrix")
            );
        }
        #endregion

        #region Actions
        public bool TestColumnSelectExpressionAction()
        {
            try
            {
                DataMatrixInfo dataMatrixInfo = getDataMatrixFunctionsPrx().getDataMatrixInfo();
                Ferda.Modules.Helpers.Data.Column.TestColumnSelectExpression(
                    dataMatrixInfo.database.odbcConnectionString,
                    dataMatrixInfo.dataMatrixName,
                    columnSelectExpression,
                    boxModule.StringIceIdentity);
                return true;
            }
            catch (Ferda.Modules.BoxRuntimeError) { }
            return false;
        }
        #endregion

        #region BoxInfo
        public StatisticsInfo GetStatistics()
        {
            try
            {
                return getStatisticsStruct();
            }
            catch (Ferda.Modules.BoxRuntimeError)
            {
                return new StatisticsInfo();
            }

        }
        #endregion
    }
}