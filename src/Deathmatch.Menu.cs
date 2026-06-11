/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Deathmatch;

public partial class Deathmatch
{
    private ThemedMenu Menu => _menu ??= new ThemedMenu(Core);
    private ThemedMenu? _menu;

    public void ShowGunsMenu(IPlayer player)
    {
        var mode = Rules.GetCurrentMode();
        if (mode == null)
            return;

        var builder = Menu.CreateBuilder("Weapons");

        foreach (var weapon in mode.GetWeapons())
        {
            var captured = weapon;
            var opt = Menu.SelectableOption($"<font color='lightgreen'>{captured.Aliases[0]}</font>");
            opt.Click += (_, args) =>
            {
                Core.Scheduler.NextTick(() =>
                {
                    Menu.CloseCurrentMenu(args.Player);
                    args.Player.RequestWeapon(captured);
                });
                return ValueTask.CompletedTask;
            };
            builder.AddOption(opt);
        }

        if (mode.HasPrimary)
        {
            var noprimary = Menu.SelectableOption("<font color='orange'>noprimary</font>");
            noprimary.Click += (_, args) =>
            {
                Core.Scheduler.NextTick(() =>
                {
                    Menu.CloseCurrentMenu(args.Player);
                    args.Player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(
                        gear_slot_t.GEAR_SLOT_RIFLE
                    );
                    args.Player.GetState().GetLoadout().SetNoprimary(true);
                });
                return ValueTask.CompletedTask;
            };
            builder.AddOption(noprimary);
        }

        var close = Menu.SelectableOption("<font color='grey'>Close</font>");
        close.Click += (_, args) =>
        {
            Core.Scheduler.NextTick(() => Menu.CloseCurrentMenu(args.Player));
            return ValueTask.CompletedTask;
        };
        builder.AddOption(close);

        Core.MenusAPI.OpenMenuForPlayer(player, builder.Build());
    }
}
