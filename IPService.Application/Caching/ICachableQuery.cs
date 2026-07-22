using System;
using System.Collections.Generic;
using System.Text;

namespace IPService.Application.Caching

{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
    }
}
