using System;
using Ferda.Modules.Boxes.LISpMinerTasks.AbstractQuantifier;
using Ferda.Modules.Quantifiers;
using System.Collections.Generic;

namespace Ferda.Modules.Boxes.LISpMinerTasks.KLTask.Quantifiers.Functional.ConditionalEntropy
{
	class ConditionalEntropyFunctionsI : AbstractKLTaskQuantifierFunctionsWithDirection
	{
		protected override ContingencyTable.QuantifierValue<TwoDimensionalContingencyTable> valueFunctionDelegate
		{
			get
			{
				switch (Direction)
				{
					case DirectionEnum.ColumnsOnRows:
						return TwoDimensionalContingencyTable.InformationDependenceCRValue;
					case DirectionEnum.RowsOnColumns:
						return TwoDimensionalContingencyTable.InformationDependenceRCValue;
					default:
						throw Ferda.Modules.Exceptions.SwitchCaseNotImplementedError(Direction);
				}
			}
		}
	}
}