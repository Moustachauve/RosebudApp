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
    public static class FavoriteRouteAccessor
    {
        const string FAVORITE_ROUTE_FILENAME = "userFavoriteRoutes.json";

        private static List<Route> favoritesCached;

        public static async Task<List<Route>> GetFavoriteRoutes()
        {
            if (favoritesCached == null)
            {
                string path = Path.Combine(Dependency.PathHelper.TempFolderPath, FAVORITE_ROUTE_FILENAME);
                favoritesCached = await LocalFileHelper.GetFromFile<List<Route>>(path);

                if (favoritesCached == null)
                {
                    favoritesCached = new List<Route>();
                }
            }

            return favoritesCached;
        }

        private static async Task SaveFavoriteRoutes()
        {
            string path = Path.Combine(Dependency.PathHelper.TempFolderPath, FAVORITE_ROUTE_FILENAME);
            await LocalFileHelper.SaveToFile(path, favoritesCached);
        }

        public static async Task SetFavoriteForRoute(bool isFavorite, Route route)
        {
            if (isFavorite)
            {
                AddFavoriteRoute(route);
            }
            else
            {
                await RemoveFavoriteRoute(route);
            }
        }

        public static async void AddFavoriteRoute(Route route)
        {
            if (!IsRouteFavorite(route))
            {
                favoritesCached.Add(route);
                await SaveFavoriteRoutes();
            }
        }

        public static async Task RemoveFavoriteRoute(Route route)
        {
            await GetFavoriteRoutes();

            Route routeInList = favoritesCached.FirstOrDefault(r => r.feed_id == route.feed_id && r.route_id == route.route_id);
            favoritesCached.Remove(routeInList);

            await SaveFavoriteRoutes();
        }

        public  static bool IsRouteFavorite(Route route)
        {
            return favoritesCached.Any(r => r.feed_id == route.feed_id && r.route_id == route.route_id);
        }
    }
}
