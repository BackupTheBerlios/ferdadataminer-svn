using System;
using System.Collections.Generic;

namespace Ferda.Modules.Boxes.LISpMinerTasks.SDKLTask.Quantifiers.Aggregation.MaxValue
{
	class MaxValueBoxInfo : Ferda.Modules.Boxes.LISpMinerTasks.SDKLTask.Quantifiers.AbstractSDKLTaskQuantifierBoxInfo
	{
		public const string typeIdentifier = 
			"LISpMinerTasks.SDKLTask.Quantifiers.Aggregation.MaxValue";

		protected override string identifier
		{
			get { return typeIdentifier; }
		}

		public override void CreateFunctions(BoxModuleI boxModule, out Ice.Object iceObject, out IFunctions functions)
		{
			MaxValueFunctionsI result = new MaxValueFunctionsI();
			iceObject = (Ice.Object)result;
			functions = (IFunctions)result;
		}

		public override string[] GetBoxModuleFunctionsIceIds()
		{
			return MaxValueFunctionsI.ids__;
		}
	}
}