using System.Collections.Generic;

namespace ScenarioModel
{
    public interface IOrder
    {
        List<string> ResourceFileNames { get; }
    }
}