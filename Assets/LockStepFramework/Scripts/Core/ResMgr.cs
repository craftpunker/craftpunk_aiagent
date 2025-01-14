#if _CLIENTLOGIC_
using YooAsset;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;
using Battle;
using SimpleJSON;
using UnityEngine.Networking;

public class ResMgr : MonoSingleton<ResMgr>
{
    private ResourcePackage package;
#if UNITY_EDITOR
    public EPlayMode playMode = EPlayMode.EditorSimulateMode;

#elif UNITY_WEBGL
    public EPlayMode playMode = EPlayMode.WebPlayMode;
#elif UNITY_STANDALONE_WIN
    public EPlayMode playMode = EPlayMode.OfflinePlayMode;
#endif


    public Transform World;

    private Dictionary<string, SpriteAtlas> dicAtlas = new Dictionary<string, SpriteAtlas>();
    private string tablePath = "Assets/Prefabs/Json";

    public void Init(Action callBack)
    {
        YooAssets.Initialize();
        package = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(package);

        StartCoroutine(InitializeYooAsset(playMode, callBack));
    }

    private IEnumerator InitializeYooAsset(EPlayMode playMode, Action callBack = null)
    {
        InitializationOperation initializationOperation = null;

        if (playMode == EPlayMode.EditorSimulateMode)// 
        {
            //：EDefaultBuildPipeline.RawFileBuildPipeline
            var buildPipeline = EDefaultBuildPipeline.ScriptableBuildPipeline;
            var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(buildPipeline, "DefaultPackage");
            var editorFileSystem = FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystem;
            yield return package.InitializeAsync(initParameters);

            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            var operation1 = package.UpdatePackageManifestAsync(operation.PackageVersion);
            yield return operation1;
        }
        else if (playMode == EPlayMode.OfflinePlayMode)// 
        {
            var buildinFileSystem = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystem;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("！");
            else
            {
                Debug.Log(initOperation.Error);
                Debug.LogError($"：{initOperation.Error}");
            }

            var operation = package.RequestPackageVersionAsync();

            yield return operation;
            var operation1 = package.UpdatePackageManifestAsync(operation.PackageVersion);

            yield return operation1;
        }
        else if (playMode == EPlayMode.HostPlayMode)// 
        {
            string defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
            string fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var cacheFileSystem = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystem = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystem;
            initParameters.CacheFileSystemParameters = cacheFileSystem;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("！");
            else
                Debug.LogError($"：{initOperation.Error}");

            var operation = package.RequestPackageVersionAsync();

            yield return operation;
            var operation1 = package.UpdatePackageManifestAsync(operation.PackageVersion);

            yield return operation1;
        }
        else if (playMode == EPlayMode.WebPlayMode) //web 
        {
            var webFileSystem = FileSystemParameters.CreateDefaultWebFileSystemParameters();
            var initParameters = new WebPlayModeParameters();
            initParameters.WebFileSystemParameters = webFileSystem;
            var initOperation = package.InitializeAsync(initParameters);

            yield return initOperation;
            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("！");
            else
                Debug.LogError($"：{initOperation.Error}");

            string version = "";
            if (AppConfig.Platform == "webgl")
            {
                var operation = package.RequestPackageVersionAsync();

                yield return operation;

                version = operation.PackageVersion;
            }
            else
            {
                string streamPath = "https://1252945775256932403.discordsays.com/.proxy/StreamingAssets/yoo/DefaultPackage/PackageManifest_DefaultPackage.version";

                using (UnityWebRequest webRequest = UnityWebRequest.Get(streamPath))
                {
                    yield return webRequest.SendWebRequest();
                    if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log(webRequest.error);
                        yield break;
                    }
                    else
                    {
                        version = webRequest.downloadHandler.text;
                        Debug.Log(version);
                    }
                }
            }


            var operation1 = package.UpdatePackageManifestAsync(version);
            yield return operation1;

            if (operation1.Status == EOperationStatus.Succeed)
                Debug.Log("UpdatePackageManifestAsync！");
            else
                Debug.LogError($"UpdatePackageManifestAsync：{operation1.Error}");
        }

        GameObject goPool = new GameObject();
        goPool.name = "GameObjPool";
        goPool.AddComponent<GameObjectPool>();

        World = GameObject.Find("World").transform;

        // ：location
        AllAssetsHandle handle = package.LoadAllAssetsAsync<TextAsset>("GlobalConf");
        handle.Completed += (ao) =>
        {
            foreach (var assetObj in handle.AllAssetObjects)
            {
                TextAsset textAsset = assetObj as TextAsset;
                JSONNode data = JSONNode.Parse(textAsset.text);
                GameData.instance.TableJsonDict.Add(textAsset.name, data);
            }

            callBack?.Invoke();
        };
    }


    public void LoadAssetAsync<T>(string name, Action<T> callBack) where T : UnityEngine.Object {

        AssetHandle aoHandle = package.LoadAssetAsync<T>(name);

        aoHandle.Completed += (ao) =>
        {
            callBack(ao.AssetObject as T);
        };
    }

    public void LoadGameObjectAsync(string name, Action<GameObject> callBack)
    {
        GameObject go = null;
        go = GameObjectPool.instance.PopInstance(name);
        if (go != null)
        {
            go.transform.parent = World.transform;
            callBack?.Invoke(go);
            return;
        }

        AssetHandle aoHandle = package.LoadAssetAsync<GameObject>(name);
        aoHandle.Completed += (ao) =>
        {
            go = GameObject.Instantiate(ao.AssetObject as GameObject);
            go.name = name;
            go.transform.parent = World.transform;
            callBack?.Invoke(go);
        };
    }

    public void LoadMaterialAsync(string name, Action<Material> callBack) {

        Material mat;
        AssetHandle aoHandle = package.LoadAssetAsync<Material>(name);
        aoHandle.Completed += (ao) =>
        {
            mat = ao.AssetObject as Material;
            callBack?.Invoke(mat);
        };
    }

    //YooAsset LoadAssetSync ，Thread 。
    //public GameObject LoadGameObject(string name)
    //{
    //    GameObject go = null;
    //    go = GameObjectPool.instance.PopInstance(name);
    //    if (go != null)
    //    {
    //        go.transform.parent = World.transform;
    //        return go;
    //    }

    //    var handle = package.LoadAssetSync<GameObject>(name);
    //    go = GameObject.Instantiate(handle.AssetObject as GameObject);
    //    go.name = name;
    //    return go;
    //}

    public void LoadSpriteAsync(string atlasName, string spriteName, Action<Sprite> callBack)
    {
        if (dicAtlas.TryGetValue(atlasName, out SpriteAtlas spriteAtlas))
        {
            var sprite = spriteAtlas.GetSprite(spriteName);

            callBack(sprite);
            return;
        }

        var handle = package.LoadAssetAsync<SpriteAtlas>(atlasName);
        handle.Completed += (ao) =>
        {
            var spriteAtlas = handle.AssetObject as SpriteAtlas;
            if (!dicAtlas.ContainsKey(atlasName))
            {
                dicAtlas.Add(atlasName, spriteAtlas);
            }
            var sprite = spriteAtlas.GetSprite(spriteName);
            callBack(sprite);
        };

    }

    public void LoadAudioClipAsync(string name, Action<AudioClip> callBack)
    {
        AssetHandle ahandle = package.LoadAssetAsync<AudioClip>(name);
        ahandle.Completed += (ao) =>
        {
            AudioClip audioClip = ao.AssetObject as AudioClip;
            callBack(audioClip);
        };
    }

    public void ReleaseGameObject(GameObject go)
    {
        GameObjectPool.instance.PushInstance(go);
    }
}
#endif