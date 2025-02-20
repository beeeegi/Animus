using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Logging;

namespace Animus;

public class Animus : BasePlugin
{
    public override string ModuleName => "Animus";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "begi.dll";
    private const string PREFIX = "{darkblue}[Animus] ";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("####################################################");
        Logger.LogInformation("#                                                  #");
        Logger.LogInformation("#         Animus System Initialization...          #");
        Logger.LogInformation("#                                                  #");
        Logger.LogInformation($"#  Version: {ModuleVersion,-37}  #");
        Logger.LogInformation($"#  Author: {ModuleAuthor,-38}  #");
        Logger.LogInformation("#                                                  #");
        Logger.LogInformation("#  Warning: Desynchronization may occur.           #");
        Logger.LogInformation("#                                                  #");
        Logger.LogInformation("####################################################");

        // hooking player connect event
        RegisterEventHandler<EventPlayerConnectFull>((ev, info) =>
        {
            var player = ev.Userid;
            if (player is not null && player.IsValid && !player.IsBot)
            {
                string message = $"{PREFIX}{{green}}Synchronization Started{{white}}: {player.PlayerName}";
                Server.PrintToChatAll(message.ReplaceColorTags());
            }

            return HookResult.Continue;
        });

        // hooking player disconnect event
        RegisterEventHandler<EventPlayerDisconnect>((ev, info) =>
        {
            var player = ev.Userid;
            if (player is not null && player.IsValid && !player.IsBot)
            {
                string message = $"{PREFIX}{{red}}Synchronization Lost{{white}}: {player.PlayerName}";
                Server.PrintToChatAll(message.ReplaceColorTags());
            }

            return HookResult.Continue;
        });

        // hooking player death event
        RegisterEventHandler<EventPlayerDeath>((ev, info) =>
        {
            var victim = ev.Userid;
            var attacker = ev.Attacker;

            if (victim is not null && victim.IsValid)
            {
                string victimName = $"{{red}}{victim.PlayerName}{{white}}";
                string attackerName = attacker is not null && attacker.IsValid
                    ? $"{{green}}{attacker.PlayerName}{{white}}"
                    : "{{grey}}Unknown{{white}}";

                string message = $"{PREFIX} {victimName} {{white}}was eliminated by{{white}} {attackerName}{{white}}.";
                Server.PrintToChatAll(message.ReplaceColorTags());
            }

            return HookResult.Continue;
        });
    }
}
