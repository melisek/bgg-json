using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Web;

namespace BoardGameGeekJsonApi
{
    public static class Cache
    {
        public static MemoryCache Default = new MemoryCache("CustomCache");

        public static string CollectionKey(string username, bool grouped, bool details)
        {
            return "collection:" + (grouped ? "grouped:" : "ungrouped:") + (details ? "detailed:" : "basic:") + username;
        }

        public static string ThingKey(int id)
        {
            return "thing:" + id.ToString();
        }

        public static string PlaysKey(string username, int? gameId = null, DateTime? from = null, DateTime? to = null)
        {
            StringBuilder sb = new StringBuilder("plays:");
            sb.Append(username);

            if (gameId.HasValue)
            {
                sb.Append(';');
                sb.Append(gameId.Value);
            }

            if (from.HasValue)
            {
                sb.Append(';');
                sb.Append(from.Value.Date.ToString("yyyy-MM-dd"));
            }

            if (to.HasValue)
            {
                sb.Append(';');
                sb.Append(to.Value.Date.ToString("yyyy-MM-dd"));
            }

            return sb.ToString();
        }

        public static string LongThingKey(int id)
        {
            return "longthing:" + id.ToString();
        }

        public static string ChallengeKey(int id)
        {
            return "challenge:" + id.ToString();
        }

    }
}