using StoreManager.DTO.Interfaces;

namespace StoreManager.Extensions
{
    public static class ParameterHelper
    {
        public static IEnumerable<KeyValuePair<string, object>> GetParameters(this IDto dto, params string[] ignoredParameters)
        {
            Dictionary<string,object> parameters = new();
            foreach (var prop in dto!.GetType().GetProperties())
            {
                if (!ignoredParameters.Contains(prop.Name))
                {
                    parameters.Add(prop.Name, prop.GetValue(dto)!);
                }
            }

            return parameters;
        }
    }
}