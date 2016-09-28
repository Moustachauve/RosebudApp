using RosebudAppCore.Model;
using RosebudAppCore.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosebudAppCore.DataAccessor
{
    public static class FavoriteFeedAccessor
    {
        const string FAVORITE_FEED_FILENAME = "userFavoriteFeeds.json";

        private static List<Feed> favoritesCached;

        public static async Task<List<Feed>> GetFavoriteFeeds()
        {
            if (favoritesCached == null)
            {
                string path = Path.Combine(Dependency.PathHelper.TempFolderPath, FAVORITE_FEED_FILENAME);
                favoritesCached = await LocalFileHelper.GetFromFile<List<Feed>>(path);

                if (favoritesCached == null)
                {
                    favoritesCached = new List<Feed>();
                }
            }

            return favoritesCached;
        }

        private static async Task SaveFavoriteFeeds()
        {
            string path = Path.Combine(Dependency.PathHelper.TempFolderPath, FAVORITE_FEED_FILENAME);
            await LocalFileHelper.SaveToFile(path, favoritesCached);
        }

        public static async Task SetFavoriteForFeed(bool isFavorite, Feed feed)
        {
            if (isFavorite)
            {
                await AddFavoriteFeed(feed);
            }
            else
            {
                await RemoveFavoriteFeed(feed);
            }
        }

        public static async Task AddFavoriteFeed(Feed feed)
        {
            if (!IsFeedFavorite(feed))
            {
                favoritesCached.Add(feed);
                await SaveFavoriteFeeds();
            }
        }

        public static async Task RemoveFavoriteFeed(Feed feed)
        {
            await GetFavoriteFeeds();

            Feed feedInList = favoritesCached.FirstOrDefault(f => f.feed_id == feed.feed_id);
            favoritesCached.Remove(feedInList);

            await SaveFavoriteFeeds();
        }

        public static bool IsFeedFavorite(Feed feed)
        {
            return favoritesCached.Any(f => f.feed_id == feed.feed_id);
        }
    }
}
