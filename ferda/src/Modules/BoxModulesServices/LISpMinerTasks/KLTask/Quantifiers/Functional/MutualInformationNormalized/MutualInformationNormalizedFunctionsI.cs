using System;
using Ferda.Modules.Boxes.LISpMinerTasks.AbstractQuantifier;
using Ferda.Modules.Quantifiers;
using System.Collections.Generic;

namespace Ferda.Modules.Boxes.LISpMinerTasks.KLTask.Quantifiers.Functional.MutualInformationNormalized
{
	class MutualInformationNormalizedFunctionsI : AbstractKLTaskQuantifierFunctions
	{
		protected override ContingencyTable.QuantifierValue<TwoDimensionalContingencyTable> valueFunctionDelegate
		{
			get {
				return TwoDimensionalContingencyTable.MutualInformationNormalizedValue;			
			}
		}
	}
}