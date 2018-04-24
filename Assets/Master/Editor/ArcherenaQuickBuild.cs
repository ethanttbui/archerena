using UnityEditor;

/// <summary>
/// Instead of editing this script, I would recommend to write your own
/// (or copy and change it). Otherwise, your changes will be overwriten when you
/// update project :)
/// </summary>
public class ArcherenaQuickBuild
{
    /// <summary>
    /// Have in mind that if you change it, it might take "a while" 
    /// for the editor to pick up changes 
    /// </summary>
    public static string NetworkRoot = "Assets/Master";

    public static BuildTarget TargetPlatform = BuildTarget.StandaloneWindows;

    /// <summary>
    /// Build with "Development" flag, so that we can see the console if something 
    /// goes wrong
    /// </summary>
    public static BuildOptions BuildOptions = BuildOptions.Development;

    public static string PrevPath = null;

    [MenuItem("Tools/Archerena/Build All", false, 0)]
    public static void BuildGame()
    {
        var path = GetPath();
        if (string.IsNullOrEmpty(path))
            return;

        BuildMasterAndSpawner(path);
        BuildClient(path);
        BuildGameServer(path);
    }

    /// <summary>
    /// Creates a build for master server and spawner
    /// </summary>
    /// <param name="path"></param>
    public static void BuildMasterAndSpawner(string path)
    {
        var masterScenes = new[]
        {
            NetworkRoot+ "/Scenes/MasterAndSpawner.unity"
        };

        BuildPipeline.BuildPlayer(masterScenes, path + "/MasterAndSpawner.exe", TargetPlatform, BuildOptions);
    }

    /// <summary>
    /// Creates a build for client
    /// </summary>
    /// <param name="path"></param>
    public static void BuildClient(string path)
    {
        var clientScenes = new[]
        {
            NetworkRoot+ "/Scenes/Client.unity",
            // Add all the game scenes
            NetworkRoot+ "/Scenes/GameScenes/Main.unity"
        };
        BuildPipeline.BuildPlayer(clientScenes, path + "/Client.exe", TargetPlatform, BuildOptions);
    }

    /// <summary>
    /// Creates a build for game server
    /// </summary>
    /// <param name="path"></param>
    public static void BuildGameServer(string path)
    {
        var gameServerScenes = new[]
        {
            NetworkRoot+"/Scenes/GameServer.unity",
            // Add all the game scenes
            NetworkRoot+"/Scenes/GameScenes/Main.unity"
        };
        BuildPipeline.BuildPlayer(gameServerScenes, path + "/GameServer.exe", TargetPlatform, BuildOptions);
    }

    #region Editor Menu

    [MenuItem("Tools/Msf/Build Master + Spawner", false, 11)]
    public static void BuildMasterAndSpawnerMenu()
    {
        var path = GetPath();
        if (!string.IsNullOrEmpty(path))
        {
            BuildMasterAndSpawner(path);
        }
    }

    [MenuItem("Tools/Msf/Build Client", false, 11)]
    public static void BuildClientMenu()
    {
        var path = GetPath();
        if (!string.IsNullOrEmpty(path))
        {
            BuildClient(path);
        }
    }

    [MenuItem("Tools/Msf/Build Game Server", false, 11)]
    public static void BuildGameServerMenu()
    {
        var path = GetPath();
        if (!string.IsNullOrEmpty(path))
        {
            BuildGameServer(path);
        }
    }

    #endregion

    public static string GetPath()
    {
        var prevPath = EditorPrefs.GetString("msf.buildPath", "");
        string path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");

        if (!string.IsNullOrEmpty(path))
        {
            EditorPrefs.SetString("msf.buildPath", path);
        }
        return path;
    }
}