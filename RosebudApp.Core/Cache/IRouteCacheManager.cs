﻿using System;
using System.Collections.Generic;
using RosebudAppCore.Model;
using System.Threading.Tasks;

namespace RosebudAppCore.Cache
{
	public interface IRouteCacheManager
	{
		Task<List<Route>> GetAllRoutes(int feedId);
		Task SaveAllRoutes(int feedId, List<Route> routes);

        Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date);
        Task SaveRouteDetails(int feedId, string routeId, DateTime date, RouteDetails routeDetails);

        Task<List<Stop>> GetRouteStops(int feedId, string routeId, DateTime date);
        Task SaveRouteStops(int feedId, string routeId, DateTime date, List<Stop> routeStops);
    }
}

