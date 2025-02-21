using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Logging;
using Serilog.Sinks.File;

namespace Animus;

public class Animus : BasePlugin
{
    public override string ModuleName => "Animus";
    public override string ModuleVersion => "0.0.2";
    public override string ModuleAuthor => "begi.dll";
    private const string PREFIX = "{darkblue}[Animus] ";
    private Dictionary<ulong, int> Killstreaks = [];

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
                string message = $"{PREFIX}{{green}}Synchronization started for{{white}}: {player.PlayerName}";
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
                string message = $"{PREFIX}{{red}}Synchronization lost for{{white}}: {player.PlayerName}";
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
                ulong victimID = victim.SteamID;

                if (attacker is not null && attacker.IsValid)
                {
                    string attackerName = $"{{green}}{attacker.PlayerName}{{white}}";
                    ulong attackerID = attacker.SteamID;

                    string message = $"{PREFIX} {victimName} {{white}}was eliminated by{{white}} {attackerName}{{white}}.";
                    Server.PrintToChatAll(message.ReplaceColorTags());

                    Killstreaks[victimID] = 0;

                    if (!Killstreaks.ContainsKey(attackerID))
                        Killstreaks[attackerID] = 0;

                    Killstreaks[attackerID]++;
                    int streak = Killstreaks[attackerID];

                    // killstreak
                    if (streak == 3 || streak == 5 || streak == 10 || streak == 15)
                    {
                        string streakMessage = $"{PREFIX}{{green}}{attacker.PlayerName}{{white}} is on a {streak} killstreak!";
                        Server.PrintToChatAll(streakMessage.ReplaceColorTags());

                        GiveMedishot(attacker);

                        if (streak == 10) ApplyEffects(attacker, "shake");
                        if (streak == 15) ApplyEffects(attacker, "glow");
                    }
                }
                // if there's no attacker (fall damage, explosion)
                else{ 
                    string message = $"{PREFIX} {victimName} {{white}}has been eliminated.";
                    Server.PrintToChatAll(message.ReplaceColorTags());

                    Killstreaks[victimID] = 0;
                }
            }

            return HookResult.Continue;
        });
    }

    /// <summary>
    /// applies special effects based on killstreaks
    /// </summary>
    /// <param name="player">target</param>
    /// <param name="effect">effect name (string)</param>
    private void ApplyEffects(CCSPlayerController player, string effect)
    {
        if (player is null || !player.IsValid) return;

        switch (effect)
        {
            case "shake":
                player.ExecuteClientCommand("shake"); 
                break;
            case "glow":
                player.ExecuteClientCommand("glow");
                break;
        }
    }

    /// <summary>
    /// gives (weapon_healthshot) to a player.
    /// </summary>
    /// <param name="player">target</param>
    private void GiveMedishot(CCSPlayerController player)
    {
        if (player is null || !player.IsValid) return;

        player.GiveNamedItem("weapon_healthshot");
        string message = $"{PREFIX}{{green}}You received a Health-Shot";
        player.PrintToChat(message.ReplaceColorTags());
    }
}
