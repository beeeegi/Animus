using CounterStrikeSharp.API.Core;

namespace Animus;

public class Animus : BasePlugin
{
    public override string ModuleName => "Animus";
    public override string ModuleVersion => "0.0.1";
    public override void Load(bool hotReload)
    {
        Console.WriteLine("Hello World!");
    }
}
