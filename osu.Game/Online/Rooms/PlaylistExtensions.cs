// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Humanizer.Localisation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Mods;
using osu.Game.Utils;

namespace osu.Game.Online.Rooms
{
    public static class PlaylistExtensions
    {
        /// <summary>
        /// Returns all historical/expired items from the <paramref name="playlist"/>, in the order in which they were played.
        /// </summary>
        public static IEnumerable<PlaylistItem> GetHistoricalItems(this IEnumerable<PlaylistItem> playlist)
            => playlist.Where(item => item.Expired).OrderBy(item => item.PlayedAt);

        /// <summary>
        /// Returns all non-expired items from the <paramref name="playlist"/>, in the order in which they are to be played.
        /// </summary>
        public static IEnumerable<PlaylistItem> GetUpcomingItems(this IEnumerable<PlaylistItem> playlist)
            => playlist.Where(item => !item.Expired).OrderBy(item => item.PlaylistOrder);

        /// <summary>
        /// Returns the first non-expired <see cref="PlaylistItem"/> in playlist order from the supplied <paramref name="playlist"/>,
        /// or the last-played <see cref="PlaylistItem"/> if all items are expired,
        /// or <see langword="null"/> if <paramref name="playlist"/> was empty.
        /// </summary>
        public static PlaylistItem? GetCurrentItem(this ICollection<PlaylistItem> playlist)
        {
            if (playlist.Count == 0)
                return null;

            return playlist.All(item => item.Expired)
                ? GetHistoricalItems(playlist).Last()
                : GetUpcomingItems(playlist).First();
        }

        /// <summary>
        /// Returns the total duration from the <see cref="PlaylistItem"/> in playlist order from the supplied <paramref name="playlist"/>,
        /// </summary>
        public static string GetTotalDuration(this BindableList<PlaylistItem> playlist) =>
            playlist.Select(p =>
            {
                IEnumerable<Mod> modList = [];
                if (p.RequiredMods.Length > 0)
                {
                    var ruleset = p.Beatmap.Ruleset.CreateInstance();
                    modList = p.RequiredMods.Select(mod => mod.ToMod(ruleset));
                }
                double rate = ModUtils.CalculateRateWithMods(modList);
                return p.Beatmap.Length / rate;
            }).Sum().Milliseconds().Humanize(minUnit: TimeUnit.Second, maxUnit: TimeUnit.Hour, precision: 2);
    }
}
