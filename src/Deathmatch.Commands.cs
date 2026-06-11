/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    [Command("guns")]
    public void OnGunsCommand(ICommandContext context)
    {
        var player = context.Sender;
        if (player != null)
            ShowGunsMenu(player);
    }

    [Command("noprimary")]
    public void OnNoprimaryCommand(ICommandContext context)
    {
        var player = context.Sender;
        if (player != null)
        {
            player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(gear_slot_t.GEAR_SLOT_RIFLE);
            player.GetState().GetLoadout().SetNoprimary(true);
        }
    }

    [Command("help")]
    public void OnHelpCommand(ICommandContext context)
    {
        var player = context.Sender;
        player?.PrintHelp();
    }
}
