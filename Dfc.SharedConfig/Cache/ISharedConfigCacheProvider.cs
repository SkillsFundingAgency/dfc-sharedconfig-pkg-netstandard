namespace Dfc.SharedConfig.Contracts
{
    public interface ISharedConfigCacheProvider
    {
        string GetConfig(string key);

        void SetConfig(string key, object value);
    }
}