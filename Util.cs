using System;

namespace stackoverazure
{
    public static class Util
    {
        public static string GetSetting(string key)
        {
            return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }
    }
}
