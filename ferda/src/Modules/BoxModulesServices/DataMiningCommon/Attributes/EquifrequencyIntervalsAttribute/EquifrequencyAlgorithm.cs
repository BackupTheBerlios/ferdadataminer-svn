// EquifrequencyAlgorithm.cs - equifrequency algorithm (generates split points for equifrequency intervals)
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
using System.Text;
using System.Data;
using System.Collections;
using Ferda.Modules.Helpers.Common;

namespace Ferda.Modules.Boxes.DataMiningCommon.Attributes.EquifrequencyIntervalsAttribute
{
	/// <summary>
	/// Algorithm for equidistant attribute.
	/// </summary>
    public static class EquidistantAlgorithm
	{
        /// <summary>
        /// Generates the the attribute.
        /// </summary>
        /// <param name="domainType">Type of the domain.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="countOfCategories">The count of categories.</param>
        /// <param name="columnInfo">The column info.</param>
        /// <param name="boxIdentity">The box identity.</param>
        /// <returns>Generated attribute.</returns>
		public static GeneratedAttribute Generate(
			AttributeDomainEnum domainType,
			double from,
			double to,
			long countOfCategories,
			Ferda.Modules.Boxes.DataMiningCommon.Column.ColumnInfo columnInfo,
			string boxIdentity)
		{
			Ferda.Modules.Helpers.Data.Column.ValueType simpleColumnType = Ferda.Modules.Helpers.Data.Column.GetColumnValueTypeByValueSubType(columnInfo.columnSubType);
			if (simpleColumnType != Ferda.Modules.Helpers.Data.Column.ValueType.Integral
				&& simpleColumnType != Ferda.Modules.Helpers.Data.Column.ValueType.Floating)
				throw Ferda.Modules.Exceptions.BadParamsError(null, boxIdentity, simpleColumnType.ToString(), restrictionTypeEnum.DbColumnDataType);

			//dataArray.Add(new Data(currentValue, currentCount));
			ArrayList dataArray = new ArrayList();
			DataTable frequencies = null;
			float startValue = Convert.ToSingle(columnInfo.statistics.ValueMin);
			float endValue = Convert.ToSingle(columnInfo.statistics.ValueMax);
			switch (domainType)
			{
				case AttributeDomainEnum.WholeDomain:
					frequencies = Ferda.Modules.Helpers.Data.Column.GetDistinctsAndFrequencies(
                        columnInfo.dataMatrix.database.odbcConnectionString,
						columnInfo.dataMatrix.dataMatrixName,
						columnInfo.columnSelectExpression,
						boxIdentity);
					from = to = 0;
					break;
				case AttributeDomainEnum.SubDomainValueBounds:
                    frequencies = Ferda.Modules.Helpers.Data.Column.GetDistinctsAndFrequencies(
                        columnInfo.dataMatrix.database.odbcConnectionString,
						columnInfo.dataMatrix.dataMatrixName,
						columnInfo.columnSelectExpression,
						columnInfo.columnSelectExpression + ">=" + from + " AND " + columnInfo.columnSelectExpression + "<=" + to,
						boxIdentity);
					startValue = (float)from;
					endValue = (float)to;
					from = to = 0;
					break;
				case AttributeDomainEnum.SubDomainNumberOfValuesBounds:
                    frequencies = Ferda.Modules.Helpers.Data.Column.GetDistinctsAndFrequencies(
                        columnInfo.dataMatrix.database.odbcConnectionString,
						columnInfo.dataMatrix.dataMatrixName,
						columnInfo.columnSelectExpression,
						boxIdentity);
					break;
				default:
					throw Ferda.Modules.Exceptions.SwitchCaseNotImplementedError(domainType);
			}

			if (from < 0)
			{
                throw Ferda.Modules.Exceptions.BadValueError(null, boxIdentity, null, new string[] { "From" }, restrictionTypeEnum.Minimum);
			}
			if (to < 0)
			{
                throw Ferda.Modules.Exceptions.BadValueError(null, boxIdentity, null, new string[] { "To" }, restrictionTypeEnum.Minimum);
			}
			int fromUi = (int)from;
			int toUi = (int)to;
			int frequenciesCount = frequencies.Rows.Count;
			if (frequenciesCount <= from + to)
				return new GeneratedAttribute();
			object item;

			switch (simpleColumnType)
			{
				case Ferda.Modules.Helpers.Data.Column.ValueType.Floating:
					for (int i = fromUi; i < (frequenciesCount - toUi); i++)
					{
						item = frequencies.Rows[i][Ferda.Modules.Helpers.Data.Column.SelectDistincts];
						if (item == System.DBNull.Value)
							continue;
                        dataArray.Add(new EquifrequencyIntervalGenerator.Data(Convert.ToSingle(item), Convert.ToInt32(frequencies.Rows[i][Ferda.Modules.Helpers.Data.Column.SelectFrequency])));
					}
					break;
				case Ferda.Modules.Helpers.Data.Column.ValueType.Integral:
					for (int i = fromUi; i < (frequenciesCount - toUi); i++)
					{
                        item = frequencies.Rows[i][Ferda.Modules.Helpers.Data.Column.SelectDistincts];
						if (item == System.DBNull.Value)
							continue;
                        dataArray.Add(new EquifrequencyIntervalGenerator.Data(Convert.ToInt64(item), Convert.ToInt32(frequencies.Rows[i][Ferda.Modules.Helpers.Data.Column.SelectFrequency])));
					}
					break;
				default:
					throw Ferda.Modules.Exceptions.BadParamsError(null, boxIdentity, simpleColumnType.ToString(), restrictionTypeEnum.DbColumnDataType);
			}
			object[] splitPoints = null;
			try
			{
				splitPoints = EquifrequencyIntervalGenerator.GenerateIntervals((int)countOfCategories, dataArray);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				if (ex.ParamName == "intervals")
				{
					throw Ferda.Modules.Exceptions.BadValueError(ex, boxIdentity, null, new string[] { "CountOfCategories" }, restrictionTypeEnum.Other);
				}
				else
					throw ex;
			}
			if (splitPoints == null)
				return new GeneratedAttribute();

			CategoriesStruct categoriesStruct = new CategoriesStruct();

			string categoryName;
			List<SelectString> categoriesNames = new List<SelectString>();
			SelectString categoriesNamesItem;
			bool computeCategoriesNames = (splitPoints.Length + 1 < Ferda.Modules.Boxes.DataMiningCommon.Attributes.AbstractAttributeConstants.MaxLengthOfCategoriesNamesSelectStringArray);
			switch (simpleColumnType)
			{
				case Ferda.Modules.Helpers.Data.Column.ValueType.Floating:
					categoriesStruct.floatIntervals = new FloatIntervalCategorySeq();
					float beginValueFl = startValue;
					float nextValueFl;
					FloatIntervalStruct intervalFlTemplate = new FloatIntervalStruct();
					intervalFlTemplate.leftBoundType = BoundaryEnum.Sharp;
					intervalFlTemplate.rightBoundType = BoundaryEnum.Round;
					FloatIntervalStruct intervalFl;
					for (int i = 0; i < splitPoints.Length + 1; i++)
					{
						if (i == splitPoints.Length)
						{
							nextValueFl = endValue;
							intervalFlTemplate.rightBoundType = BoundaryEnum.Sharp;
						}
						else
							nextValueFl = (float)splitPoints[i];
						intervalFl = intervalFlTemplate;
						intervalFl.leftBound = beginValueFl;
						intervalFl.rightBound = nextValueFl;
						categoryName = Constants.LeftClosedInterval + beginValueFl.ToString() + Constants.SeparatorInterval + nextValueFl.ToString();
						categoryName += (intervalFl.rightBoundType == BoundaryEnum.Sharp)? Constants.RightClosedInterval : Constants.RightOpenedInterval;

						if (computeCategoriesNames)
						{
							categoriesNamesItem = new SelectString();
							categoriesNamesItem.name = categoryName;
							categoriesNamesItem.label = categoryName;
							categoriesNames.Add(categoriesNamesItem);
						}

						categoriesStruct.floatIntervals.Add(
							categoryName,
							new FloatIntervalStruct[] { intervalFl });
						beginValueFl = nextValueFl;
					}

					return new GeneratedAttribute(
						categoriesStruct,
						null,
						categoriesStruct.floatIntervals.Count,
						categoriesNames.ToArray());
				case Ferda.Modules.Helpers.Data.Column.ValueType.Integral:
					categoriesStruct.longIntervals = new LongIntervalCategorySeq();
					long beginValueLn = (long)startValue;
					long nextValueLn;
					LongIntervalStruct intervalLnTemplate = new LongIntervalStruct();
					intervalLnTemplate.leftBoundType = BoundaryEnum.Sharp;
					intervalLnTemplate.rightBoundType = BoundaryEnum.Round;
					LongIntervalStruct intervalLn;
					for (int i = 0; i < splitPoints.Length + 1; i++)
					{
						if (i == splitPoints.Length)
						{
							nextValueLn = (long)endValue;
							intervalLnTemplate.rightBoundType = BoundaryEnum.Sharp;
						}
						else
							nextValueLn = (long)splitPoints[i];
						intervalLn = intervalLnTemplate;
						intervalLn.leftBound = beginValueLn;
						intervalLn.rightBound = nextValueLn;
						categoryName = Constants.LeftClosedInterval + beginValueLn.ToString() + Constants.SeparatorInterval + nextValueLn.ToString();
						categoryName += (intervalLn.rightBoundType == BoundaryEnum.Sharp)? Constants.RightClosedInterval : Constants.RightOpenedInterval;

						if (computeCategoriesNames)
						{
							categoriesNamesItem = new SelectString();
							categoriesNamesItem.name = categoryName;
							categoriesNamesItem.label = categoryName;
							categoriesNames.Add(categoriesNamesItem);
						}

						categoriesStruct.longIntervals.Add(
							categoryName,
							new LongIntervalStruct[] { intervalLn });
						beginValueLn = nextValueLn;
					}

					return new GeneratedAttribute(
						categoriesStruct,
						null,
						categoriesStruct.longIntervals.Count,
						categoriesNames.ToArray());
				default:
					throw Ferda.Modules.Exceptions.BadParamsError(null, boxIdentity, simpleColumnType.ToString(), restrictionTypeEnum.DbColumnDataType);
			}
		}
	}
}
