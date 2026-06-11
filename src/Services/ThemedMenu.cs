/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Ian Lucas. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using SwiftlyS2.Core.Menus.OptionsBase;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Menus;
using SwiftlyS2.Shared.Players;

namespace Deathmatch;

// Single source of truth for SwiftlyS2 menu styling. Construct from ISwiftlyCore
// and build every menu through this so the theme lives in exactly one place.
public class ThemedMenu(ISwiftlyCore core)
{
    // jsDelivr CDN mirror of the Pisex cs2-menus SVG button icons (edge-cached,
    // proper cache headers — much faster first paint than raw.githubusercontent).
    private const string MenuBtnUrl =
        "https://cdn.jsdelivr.net/gh/Pisex/cs2-menus@main/menu_buttons/site";

    // One <img> tag. Native SVG size (no width/height).
    private static string Img(string name) => $"<img src='{MenuBtnUrl}/{name}.svg'/>";

    // The image "footer" row (goes in the comment slot).
    private static readonly string FooterButtons =
        $"{Img("w")} {Img("s")} {Img("empty")} {Img("f")}";

    // Select icon appended only to the row the player is currently navigating.
    private static readonly string SelectIcon = " " + Img("e");

    private readonly ISwiftlyCore _core = core;

    public IMenuBuilderAPI CreateBuilder(string title)
    {
        var builder = _core.MenusAPI.CreateBuilder();
        // Note: design setters return the builder, not the design — so capture
        // the design and call line-by-line (cannot chain SetX().SetY()).
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

    public ButtonMenuOption SelectableOption(string text)
    {
        var opt = new ButtonMenuOption(text);
        opt.AfterFormat += (_, args) =>
        {
            var menu = _core.MenusAPI.GetCurrentMenu(args.Player);
            if (menu != null && ReferenceEquals(menu.GetCurrentOption(args.Player), args.Option))
                args.CustomText += SelectIcon;
        };
        return opt;
    }

    public void CloseCurrentMenu(IPlayer player)
    {
        var cur = _core.MenusAPI.GetCurrentMenu(player);
        if (cur != null)
            _core.MenusAPI.CloseMenuForPlayer(player, cur);
    }
}
