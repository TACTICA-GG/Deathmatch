/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Core.Menus.OptionsBase;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace Deathmatch;

public partial class Deathmatch
{
    // Base URL of the SVG button icons (Pisex cs2-menus assets).
    private const string MenuBtnUrl =
        "https://raw.githubusercontent.com/Pisex/cs2-menus/refs/heads/main/menu_buttons/site";

    // One <img> tag. Native SVG size (no width/height).
    private static string Img(string name) => $"<img src='{MenuBtnUrl}/{name}.svg'/>";

    // The image "footer" row (goes in the comment slot).
    private static readonly string FooterButtons =
        $"{Img("w")} {Img("s")} {Img("empty")} {Img("f")}";

    // Select icon appended only to the row the player is currently navigating.
    private static readonly string SelectIcon = " " + Img("e");

    private IMenuBuilderAPI CreateThemedBuilder(string title)
    {
        var builder = Core.MenusAPI.CreateBuilder();
        var design = builder.Design;
        design.SetMenuTitle(title);
        design.SetMenuTitleVisible(true);
        design.SetMenuTitleItemCountVisible(true);
        design.SetMenuFooterVisible(false);
        design.SetCommentVisible(true);
        design.SetDefaultComment(FooterButtons);
        design.SetNavigationMarkerColor("#FFFFFF");
        design.SetVisualGuideLineColor("#FFFFFF");
        design.SetDisabledColor("#808080");
        design.SetMaxVisibleItems(3);
        return builder;
    }

    private ButtonMenuOption SelectableOption(string text)
    {
        var opt = new ButtonMenuOption(text);
        opt.AfterFormat += (_, args) =>
        {
            var menu = Core.MenusAPI.GetCurrentMenu(args.Player);
            if (menu != null && ReferenceEquals(menu.GetCurrentOption(args.Player), args.Option))
                args.CustomText += SelectIcon;
        };
        return opt;
    }

    private static void CloseCurrentMenu(IPlayer player)
    {
        var cur = Runtime.Core.MenusAPI.GetCurrentMenu(player);
        if (cur != null)
            Runtime.Core.MenusAPI.CloseMenuForPlayer(player, cur);
    }

    public void ShowGunsMenu(IPlayer player)
    {
        var mode = Rules.GetCurrentMode();
        if (mode == null)
            return;

        var builder = CreateThemedBuilder("Weapons");

        foreach (var weapon in mode.GetWeapons())
        {
            var captured = weapon;
            var opt = SelectableOption($"<font color='lightgreen'>{captured.Aliases[0]}</font>");
            opt.Click += (_, args) =>
            {
                Core.Scheduler.NextTick(() =>
                {
                    CloseCurrentMenu(args.Player);
                    args.Player.RequestWeapon(captured);
                });
                return ValueTask.CompletedTask;
            };
            builder.AddOption(opt);
        }

        if (mode.HasPrimary)
        {
            var noprimary = SelectableOption("<font color='orange'>noprimary</font>");
            noprimary.Click += (_, args) =>
            {
                Core.Scheduler.NextTick(() =>
                {
                    CloseCurrentMenu(args.Player);
                    args.Player.PlayerPawn?.WeaponServices?.RemoveWeaponBySlot(
                        SwiftlyS2.Shared.SchemaDefinitions.gear_slot_t.GEAR_SLOT_RIFLE
                    );
                    args.Player.GetState().GetLoadout().SetNoprimary(true);
                });
                return ValueTask.CompletedTask;
            };
            builder.AddOption(noprimary);
        }

        var close = SelectableOption("<font color='grey'>Close</font>");
        close.Click += (_, args) =>
        {
            Core.Scheduler.NextTick(() => CloseCurrentMenu(args.Player));
            return ValueTask.CompletedTask;
        };
        builder.AddOption(close);

        Core.MenusAPI.OpenMenuForPlayer(player, builder.Build());
    }
}
