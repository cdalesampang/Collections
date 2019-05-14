using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Data.Utilities
{
    internal static class QueryableExt
    {
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null)
                return query;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}
