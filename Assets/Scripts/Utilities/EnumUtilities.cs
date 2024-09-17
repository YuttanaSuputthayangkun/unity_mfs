#nullable enable

using System;

namespace Utilities
{
    public class EnumUtilities
    {
        public static TEnum? RandomPickEnum<TEnum>() where TEnum: struct
        {
            TEnum[] list = (TEnum[])Enum.GetValues(typeof(TEnum));
            if (list.Length == 0)
            {
                return null;
            }
            
            int randomIndex = UnityEngine.Random.Range(0, list.Length - 1);
            return list[randomIndex];
        }
    }
}