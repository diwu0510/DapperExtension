using System;
using System.Collections.Concurrent;

namespace HZC.Data.Dapper.Reflections
{
    public class EntityInfoContainer
    {
        private static readonly ConcurrentDictionary<string, CustomEntityInfo> Cache =
            new ConcurrentDictionary<string, CustomEntityInfo>();

        public static CustomEntityInfo Get(Type type)
        {
            if (Cache.TryGetValue(type.FullName ?? throw new InvalidOperationException(), out var info))
            {
                return info;
            }
            info = new CustomEntityInfo(type);
            Cache.TryAdd(type.FullName, info);
            return info;
        }
    }
}
