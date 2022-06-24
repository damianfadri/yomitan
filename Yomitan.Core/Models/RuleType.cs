using System;

namespace Yomitan.Core.Models
{
    [Flags]
    public enum RuleType
    {
        None = 0,
        v1 = 1,
        v5 = 2,
        vs = 4,
        vk = 8,
        vz = 16,
        adji = 32,
        iru = 64,
    }
}
