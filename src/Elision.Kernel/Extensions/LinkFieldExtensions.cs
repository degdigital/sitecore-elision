using Sitecore.Data;
using Sitecore.Data.Fields;

namespace Elision
{
	public static class LinkFieldExtensions
	{
		public static bool HasValue(this LinkField field)
		{
			return (field != null) && (!string.IsNullOrWhiteSpace(field.Url) || !ID.IsNullOrEmpty(field.TargetID));
		}
	}
}
