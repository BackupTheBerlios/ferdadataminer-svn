using System;
using System.Collections.Generic;

namespace Ferda.Modules.Boxes.LISpMinerTasks.SDCFTask.Quantifiers.Functional.StandardDeviation
{
	class StandardDeviationBoxInfo : Ferda.Modules.Boxes.LISpMinerTasks.SDCFTask.Quantifiers.AbstractSDCFTaskQuantifierBoxInfo
	{
		public const string typeIdentifier =
			"LISpMinerTasks.SDCFTask.Quantifiers.Functional.StandardDeviation";

		protected override string identifier
		{
			get { return typeIdentifier; }
		}

		public override void CreateFunctions(BoxModuleI boxModule, out Ice.Object iceObject, out IFunctions functions)
		{
			StandardDeviationFunctionsI result = new StandardDeviationFunctionsI();
			iceObject = (Ice.Object)result;
			functions = (IFunctions)result;
		}

		public override string[] GetBoxModuleFunctionsIceIds()
		{
			return StandardDeviationFunctionsI.ids__;
		}
	}
}