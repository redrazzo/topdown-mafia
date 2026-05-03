using System.Collections.Generic;
using System.Linq;
using MafiaTopDown.Gameplay.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace MafiaTopDown.Editor
{
    public static class LaunchContentAudit
    {
        private const int MinimumMissionCount = 12;
        private const int MinimumDistrictCount = 4;
        private const int MinimumActivityCount = 6;
        private const int MinimumInteriorCount = 10;

        [MenuItem("MafiaTopDown/QA/Run Launch Content Audit")]
        public static void RunLaunchContentAuditFromMenu()
        {
            RunAudit(exitBatchMode: false);
        }

        public static void RunBatchLaunchContentAudit()
        {
            var passed = RunAudit(exitBatchMode: true);
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(passed ? 0 : 1);
            }
        }

        private static bool RunAudit(bool exitBatchMode)
        {
            var failures = new List<string>();
            var campaign = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            if (campaign == null)
            {
                failures.Add("CampaignDatabase.asset is missing.");
                return Finish(failures, exitBatchMode);
            }

            RequireCount(failures, "missions", campaign.Missions, MinimumMissionCount);
            RequireCount(failures, "chapters", campaign.Chapters, 3);
            RequireCount(failures, "districts", campaign.Districts, MinimumDistrictCount);
            RequireCount(failures, "side activities", campaign.Activities, MinimumActivityCount);
            RequireCount(failures, "interior portals", campaign.InteriorPortals, MinimumInteriorCount);

            var buildScenePaths = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            var interiorScenePaths = buildScenePaths
                .Where(path => System.IO.Path.GetFileNameWithoutExtension(path).StartsWith("Interior_", System.StringComparison.Ordinal))
                .ToArray();
            if (interiorScenePaths.Length < MinimumInteriorCount)
            {
                failures.Add("Build settings include " + interiorScenePaths.Length + " interior scenes; expected at least " + MinimumInteriorCount + ".");
            }

            foreach (var districtAsset in campaign.Districts.Where(district => district != null))
            {
                var district = districtAsset.ToDefinition();
                if (district.LandmarkIds.Count == 0)
                {
                    failures.Add("District " + district.Id + " has no landmark ids.");
                }

                if (district.ActivityIds.Count == 0)
                {
                    failures.Add("District " + district.Id + " has no activity ids.");
                }

                if (!buildScenePaths.Any(path => path.EndsWith("/" + district.SceneName + ".unity", System.StringComparison.Ordinal)))
                {
                    failures.Add("District " + district.Id + " scene is not in build settings: " + district.SceneName + ".");
                }
            }

            foreach (var portalAsset in campaign.InteriorPortals.Where(portal => portal != null))
            {
                var portal = portalAsset.ToDefinition();
                var interiorPath = "Assets/Game/Scenes/" + portal.InteriorSceneName + ".unity";
                var exteriorPath = "Assets/Game/Scenes/" + portal.ExteriorSceneName + ".unity";
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(interiorPath) == null)
                {
                    failures.Add("Portal " + portal.Id + " targets missing interior scene " + interiorPath + ".");
                }

                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(exteriorPath) == null)
                {
                    failures.Add("Portal " + portal.Id + " targets missing exterior scene " + exteriorPath + ".");
                }

                if (!buildScenePaths.Contains(interiorPath))
                {
                    failures.Add("Portal " + portal.Id + " interior scene is not in build settings: " + interiorPath + ".");
                }
            }

            Debug.Log(
                "Launch content audit coverage: missions=" + campaign.Missions.Length +
                ", chapters=" + campaign.Chapters.Length +
                ", districts=" + campaign.Districts.Length +
                ", activities=" + campaign.Activities.Length +
                ", portals=" + campaign.InteriorPortals.Length +
                ", buildInteriors=" + interiorScenePaths.Length + ".");

            return Finish(failures, exitBatchMode);
        }

        private static void RequireCount<T>(ICollection<string> failures, string label, T[] values, int minimum) where T : UnityEngine.Object
        {
            var count = values == null ? 0 : values.Count(value => value != null);
            if (count < minimum)
            {
                failures.Add("Campaign database has " + count + " " + label + "; expected at least " + minimum + ".");
            }
        }

        private static bool Finish(IReadOnlyCollection<string> failures, bool exitBatchMode)
        {
            if (failures.Count == 0)
            {
                Debug.Log("Launch content audit passed.");
                return true;
            }

            foreach (var failure in failures)
            {
                Debug.LogError("Launch content audit failed: " + failure);
            }

            if (exitBatchMode && Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }

            return false;
        }
    }
}
