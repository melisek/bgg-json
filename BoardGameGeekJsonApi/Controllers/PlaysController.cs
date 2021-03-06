﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace BoardGameGeekJsonApi.Controllers
{
    public class PlaysController : ApiController
    {
        [Route("plays/{username}/{gameId}")]
        [CacheOutput(ClientTimeSpan = 30)]
        public async Task<List<PlayItem>> Get(string username, int? gameId = null)
        {
            var cachedResult = Cache.Default.Get(Cache.PlaysKey(username, gameId)) as List<PlayItem>;
            if (cachedResult != null)
            {
                Debug.WriteLine("Served Plays from Cache.");
                return cachedResult;
            }

            var client = new BoardGameGeekClient();

            var plays = await client.LoadPlays(username, 1, null, null, gameId);
            var gameIds = new HashSet<int>(plays.Items.Select(g => g.GameId));
            var gameDetails = await client.ParallelLoadGames(gameIds);
            var gameDetailsById = gameDetails.ToDictionary(g => g.GameId);

            var response = (from p in plays.Items
                            orderby p.PlayDate descending
                            let g = gameDetailsById[p.GameId]
                            select new PlayItem
                            {
                                GameId = p.GameId,
                                Name = p.Name,
                                Image = g.Image,
                                Thumbnail = g.Thumbnail,
                                PlayDate = p.PlayDate,
                                NumPlays = p.NumPlays,
                                Comments = p.Comments,
                                Players = p.Players
                            }).ToList();
            Cache.Default.Set(Cache.PlaysKey(username), response, DateTimeOffset.Now.AddSeconds(15));

            return response;
        }

        [Route("mostplayed/{username}")]
        [CacheOutput(ClientTimeSpan = 30)]
        public async Task<List<PlayItem>> GetMostPlayedGames(string username, DateTime? from = null, DateTime? to = null)
        {
            var cachedResult = Cache.Default.Get(Cache.PlaysKey(username, null, from, to)) as List<PlayItem>;
            if (cachedResult != null)
            {
                Debug.WriteLine("Served Plays from Cache.");
                return cachedResult;
            }

            var client = new BoardGameGeekClient();

            var plays = await client.LoadMostPlayedGames(username, 1, from, to);
            var gameIds = new HashSet<int>(plays.Items.Select(g => g.GameId));
            var gameDetails = await client.ParallelLoadGames(gameIds);
            var gameDetailsById = gameDetails.ToDictionary(g => g.GameId);

            var response = (from p in plays.Items
                            orderby p.PlayDate descending
                            let g = gameDetailsById[p.GameId]
                            select new PlayItem
                            {
                                GameId = p.GameId,
                                Name = p.Name,
                                Image = g.Image,
                                Thumbnail = g.Thumbnail,
                                PlayDate = p.PlayDate,
                                NumPlays = p.NumPlays,
                                Comments = p.Comments,
                                Players = p.Players
                            }).ToList();
            Cache.Default.Set(Cache.PlaysKey(username), response, DateTimeOffset.Now.AddSeconds(15));

            return response;
        }

    }
}