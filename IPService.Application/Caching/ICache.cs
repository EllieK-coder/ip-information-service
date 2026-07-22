namespace WorldRank.Application.Caching;

public interface ICache
{
    bool TryGet<T>(string key, out T? value); //"try to get the value for this key; return true if found, and hand back the value." (<T> = works for any type — players, wallets, whatever.)
    void Set<T>(string key, T value, TimeSpan ttl); //"store this value under this key for this long (ttl = time to live)."
    void Remove(string key); // "delete this key."
}