using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JustPete.Core
{
	public static class ObjectExtensions
	{
		static readonly JsonSerializerSettings SerializerSettings;

		static ObjectExtensions()
		{
			SerializerSettings = new JsonSerializerSettings 
			{ 
				ContractResolver = new CamelCasePropertyNamesContractResolver() 
			};
		}

		public static string ToJson(this object obj)
		{
			return JsonConvert.SerializeObject(obj, SerializerSettings);
		}
	}
}

