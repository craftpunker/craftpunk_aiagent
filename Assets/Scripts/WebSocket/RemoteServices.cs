
using UnityEngine;
using YooAsset;

public class RemoteServices : IRemoteServices
{
    public string RemoteFallbackURL;
    public string RemoteMainURL;

    public RemoteServices(string remoteFallbackURL, string remoteMainURL)
    {
        RemoteFallbackURL = remoteFallbackURL;
        RemoteMainURL = remoteMainURL;
    }


    public string GetRemoteFallbackURL(string fileName)
    {
        return RemoteFallbackURL;
    }

    public string GetRemoteMainURL(string fileName)
    {
        return RemoteMainURL;
    }
}
