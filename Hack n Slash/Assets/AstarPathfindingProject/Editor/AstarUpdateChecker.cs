using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding
{
    /// <summary>Handles update checking for the A* Pathfinding Project</summary>
    [InitializeOnLoad]
    public static class AstarUpdateChecker
    {
        /// <summary>Used for downloading new version information</summary>
        static UnityWebRequest updateCheckDownload;

        static System.DateTime _lastUpdateCheck;
        static bool _lastUpdateCheckRead;

        static System.Version _latestVersion;

        static System.Version _latestBetaVersion;

        /// <summary>Description of the latest update of the A* Pathfinding Project</summary>
        static string _latestVersionDescription;

        static bool hasParsedServerMessage;

        /// <summary>Number of days between update checks</summary>
        const double updateCheckRate = 1F;

        /// <summary>URL to the version file containing the latest version number.</summary>
        const string updateURL = "https://www.arongranberg.com/astar/version.php"; // Updated to HTTPS

        /// <summary>Last time an update check was made</summary>
        public static System.DateTime lastUpdateCheck
        {
            get
            {
                try
                {
                    // Reading from EditorPrefs is relatively slow, avoid it
                    if (_lastUpdateCheckRead) return _lastUpdateCheck;

                    _lastUpdateCheck = System.DateTime.Parse(EditorPrefs.GetString("AstarLastUpdateCheck", "1/1/1971 00:00:01"), System.Globalization.CultureInfo.InvariantCulture);
                    _lastUpdateCheckRead = true;
                }
                catch (System.FormatException)
                {
                    lastUpdateCheck = System.DateTime.UtcNow;
                    Debug.LogWarning("Invalid DateTime string encountered when loading from preferences");
                }
                return _lastUpdateCheck;
            }
            private set
            {
                _lastUpdateCheck = value;
                EditorPrefs.SetString("AstarLastUpdateCheck", _lastUpdateCheck.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        /// <summary>Latest version of the A* Pathfinding Project</summary>
        public static System.Version latestVersion
        {
            get
            {
                RefreshServerMessage();
                return _latestVersion ?? AstarPath.Version;
            }
            private set
            {
                _latestVersion = value;
            }
        }

        /// <summary>Latest beta version of the A* Pathfinding Project</summary>
        public static System.Version latestBetaVersion
        {
            get
            {
                RefreshServerMessage();
                return _latestBetaVersion ?? AstarPath.Version;
            }
            private set
            {
                _latestBetaVersion = value;
            }
        }

        /// <summary>Summary of the latest update</summary>
        public static string latestVersionDescription
        {
            get
            {
                RefreshServerMessage();
                return _latestVersionDescription ?? "";
            }
            private set
            {
                _latestVersionDescription = value;
            }
        }

        /// <summary>
        /// Holds various URLs and text for the editor.
        /// This info can be updated when a check for new versions is done to ensure that there are no invalid links.
        /// </summary>
        static Dictionary<string, string> astarServerData = new Dictionary<string, string> {
            { "URL:modifiers", "https://www.arongranberg.com/astar/docs/modifiers.php" }, // Updated to HTTPS
            { "URL:astarpro", "https://arongranberg.com/unity/a-pathfinding/astarpro/" }, // Updated to HTTPS
            { "URL:documentation", "https://arongranberg.com/astar/docs/" }, // Updated to HTTPS
            { "URL:findoutmore", "https://arongranberg.com/astar" }, // Updated to HTTPS
            { "URL:download", "https://arongranberg.com/unity/a-pathfinding/download" }, // Updated to HTTPS
            { "URL:changelog", "https://arongranberg.com/astar/docs/changelog.php" }, // Updated to HTTPS
            { "URL:tags", "https://arongranberg.com/astar/docs/tags.php" }, // Updated to HTTPS
            { "URL:homepage", "https://arongranberg.com/astar/" } // Updated to HTTPS
        };

        static AstarUpdateChecker()
        {
            // Add a callback so that we can parse the message when it has been downloaded
            EditorApplication.update += UpdateCheckLoop;
            EditorBase.getDocumentationURL = () => GetURL("documentation");
        }


        static void RefreshServerMessage()
        {
            if (!hasParsedServerMessage)
            {
                var serverMessage = EditorPrefs.GetString("AstarServerMessage");

                if (!string.IsNullOrEmpty(serverMessage))
                {
                    ParseServerMessage(serverMessage);
                    ShowUpdateWindowIfRelevant();
                }
            }
        }

        public static string GetURL(string tag)
        {
            RefreshServerMessage();
            string url;
            astarServerData.TryGetValue("URL:" + tag, out url);
            return url ?? "";
        }

        /// <summary>Initiate a check for updates now, regardless of when the last check was done</summary>
        public static void CheckForUpdatesNow()
        {
            lastUpdateCheck = System.DateTime.UtcNow.AddDays(-5);

            // Remove the callback if it already exists
            EditorApplication.update -= UpdateCheckLoop;

            // Add a callback so that we can parse the message when it has been downloaded
            EditorApplication.update += UpdateCheckLoop;
        }

        /// <summary>
        /// Checking for updates...
        /// Should be called from EditorApplication.update
        /// </summary>
        static void UpdateCheckLoop()
        {
            // Go on until the update check has been completed
            if (!CheckForUpdates())
            {
                EditorApplication.update -= UpdateCheckLoop;
            }
        }

        /// <summary>
        /// Checks for updates if there was some time since last check.
        /// It must be called repeatedly to ensure that the result is processed.
        /// Returns: True if an update check is progressing (UnityWebRequest request)
        /// </summary>
        static bool CheckForUpdates()
        {
            if (updateCheckDownload != null && updateCheckDownload.isDone)
            {
                if (!string.IsNullOrEmpty(updateCheckDownload.error))
                {
                    Debug.LogWarning("There was an error checking for updates to the A* Pathfinding Project\n" +
                        "Error: " + updateCheckDownload.error);
                    updateCheckDownload = null;
                    return false;
                }
                UpdateCheckCompleted(updateCheckDownload.downloadHandler.text);
                updateCheckDownload.Dispose();
                updateCheckDownload = null;
            }

            // Check if it is time to check for updates
            // Check for updates a bit earlier if we are in play mode or have the AstarPath object in the scene
            // as then the collected statistics will be a bit more accurate
            var offsetMinutes = (Application.isPlaying && Time.time > 60) || AstarPath.active != null ? -20 : 20;
            var minutesUntilUpdate = lastUpdateCheck.AddDays(updateCheckRate).AddMinutes(offsetMinutes).Subtract(System.DateTime.UtcNow).TotalMinutes;
            if (minutesUntilUpdate < 0)
            {
                DownloadVersionInfo();
            }

            return updateCheckDownload != null || minutesUntilUpdate < 10;
        }

        static void DownloadVersionInfo()
        {
            var script = AstarPath.active != null ? AstarPath.active : GameObject.FindObjectOfType(typeof(AstarPath)) as AstarPath;

            if (script != null)
            {
                script.ConfigureReferencesInternal();
                if ((!Application.isPlaying && (script.data.graphs == null || script.data.graphs.Length == 0)) || script.data.graphs == null)
                {
                    script.data.DeserializeGraphs();
                }
            }

            bool mecanim = GameObject.FindObjectOfType(typeof(Animator)) != null;
            string query = updateURL +
                           "?v=" + AstarPath.Version +
                           "&pro=0" +
                           "&check=" + updateCheckRate + "&distr=" + AstarPath.Distribution +
                           "&unitypro=" + (Application.HasProLicense() ? "1" : "0") +
                           "&inscene=" + (script != null ? "1" : "0") +
                           "&targetplatform=" + EditorUserBuildSettings.activeBuildTarget +
                           "&devplatform=" + Application.platform +
                           "&mecanim=" + (mecanim ? "1" : "0") +
                           "&hasNavmesh=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "NavMeshGraph") ? 1 : 0) +
                           "&hasPoint=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "PointGraph") ? 1 : 0) +
                           "&hasGrid=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "GridGraph") ? 1 : 0) +
                           "&hasLayered=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "LayerGridGraph") ? 1 : 0) +
                           "&hasRecast=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "RecastGraph") ? 1 : 0) +
                           "&hasGrid=" + (script != null && script.data.graphs.Any(g => g.GetType().Name == "GridGraph") ? 1 : 0) +
                           "&hasCustom=" + (script != null && script.data.graphs.Any(g => g != null && !g.GetType().FullName.Contains("Pathfinding.")) ? 1 : 0) +
                           "&graphCount=" + (script != null ? script.data.graphs.Count(g => g != null) : 0) +
                           "&unityversion=" + Application.unityVersion +
                           "&branch=" + AstarPath.Branch;

            updateCheckDownload = UnityWebRequest.Get(query); // Updated to use UnityWebRequest
            updateCheckDownload.SendWebRequest(); // Updated to use SendWebRequest
            lastUpdateCheck = System.DateTime.UtcNow;
        }

        /// <summary>Handles the data from the update page</summary>
        static void UpdateCheckCompleted(string result)
        {
            EditorPrefs.SetString("AstarServerMessage", result);
            ParseServerMessage(result);
            ShowUpdateWindowIfRelevant();
        }

        static void ParseServerMessage(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                return;
            }

            hasParsedServerMessage = true;

            string[] splits = result.Split('|');
            latestVersionDescription = splits.Length > 1 ? splits[1] : "";

            if (splits.Length > 4)
            {
                // First 4 are just compatibility fields
                var fields = splits.Skip(4).ToArray();

                // Take all pairs of fields
                for (int i = 0; i < (fields.Length / 2) * 2; i += 2)
                {
                    string key = fields[i];
                    string val = fields[i + 1];
                    astarServerData[key] = val;
                }
            }

            try
            {
                latestVersion = new System.Version(astarServerData["VERSION:branch"]);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Could not parse version\n" + ex);
            }

            try
            {
                latestBetaVersion = new System.Version(astarServerData["VERSION:beta"]);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Could not parse version\n" + ex);
            }
        }

        static void ShowUpdateWindowIfRelevant()
        {
            // Your logic here for showing update window if relevant
        }
    }
}
