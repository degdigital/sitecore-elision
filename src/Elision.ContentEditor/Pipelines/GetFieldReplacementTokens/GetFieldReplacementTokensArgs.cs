using System.Collections.Generic;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;

namespace Elision.ContentEditor.Pipelines.GetFieldReplacementTokens
{
	public class GetFieldReplacementTokensArgs : PipelineArgs
	{
		public RenderFieldArgs RenderFieldArgs { get; set; }
		public readonly Dictionary<string, object> ReplacementValues = new Dictionary<string, object>();

		public GetFieldReplacementTokensArgs(RenderFieldArgs renderFieldArgs)
		{
			RenderFieldArgs = renderFieldArgs;
		}
	}
}