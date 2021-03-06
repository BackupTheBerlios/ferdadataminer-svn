using System;
using System.Collections.Generic;

namespace Ferda.Modules.Boxes.LISpMinerTasks.SDCFTask.Quantifiers.Aggregation.SumOfValues
{
	class SumOfValuesBoxInfo : Ferda.Modules.Boxes.LISpMinerTasks.SDCFTask.Quantifiers.AbstractSDCFTaskQuantifierBoxInfo
	{
		public const string typeIdentifier =
			"LISpMinerTasks.SDCFTask.Quantifiers.Aggregation.SumOfValues";

		protected override string identifier
		{
			get { return typeIdentifier; }
		}

		public override void CreateFunctions(BoxModuleI boxModule, out Ice.Object iceObject, out IFunctions functions)
		{
			SumOfValuesFunctionsI result = new SumOfValuesFunctionsI();
			iceObject = (Ice.Object)result;
			functions = (IFunctions)result;
		}

		public override string[] GetBoxModuleFunctionsIceIds()
		{
			return SumOfValuesFunctionsI.ids__;
		}
	}
}