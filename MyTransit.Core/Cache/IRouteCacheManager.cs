using System;
using System.Collections.Generic;
using MyTransit.Core.Model;
using System.Threading.Tasks;

namespace MyTransit.Core.Cache
{
	public interface IRouteCacheManager
	{
		Task<List<Route>> GetAllRoutes(int feedId);
		Task SaveAllRoutes(int feedId, List<Route> routes);

		Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date);
		Task SaveRouteDetails(int feedId, string routeId, DateTime date, RouteDetails routeDetails);
	}
}

