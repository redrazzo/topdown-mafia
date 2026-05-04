#nullable enable annotations

using MafiaTopDown.Gameplay.Runtime.Camera;
using MafiaTopDown.Gameplay.Runtime.Combat;
using MafiaTopDown.Gameplay.Runtime.Data;
using MafiaTopDown.Gameplay.Runtime.Interaction;
using MafiaTopDown.Gameplay.Runtime.Law;
using MafiaTopDown.Gameplay.Runtime.Missions;
using MafiaTopDown.Gameplay.Runtime.Player;
using MafiaTopDown.Gameplay.Runtime.Progression;
using MafiaTopDown.Gameplay.Runtime.Scenes;
using MafiaTopDown.Gameplay.Runtime.Story;
using MafiaTopDown.Gameplay.Runtime.UI;
using MafiaTopDown.Gameplay.Runtime.Vehicles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace MafiaTopDown.Editor
{
    public static class PrototypeSceneBuilder
    {
        private sealed class ExteriorRuntimeBundle
        {
            public TopDownPlayerController PlayerController = null!;
            public CombatHealth PlayerHealth = null!;
            public PlayerCombatController PlayerCombatController = null!;
            public HeatSystemController HeatSystemController = null!;
            public PoliceResponseController PoliceResponseController = null!;
            public ActivityProgressionController ActivityProgressionController = null!;
            public SaveGameFileService SaveGameFileService = null!;
            public CampaignProgressionController CampaignProgressionController = null!;
            public SceneTransitionController SceneTransitionController = null!;
            public DialogueController DialogueController = null!;
        }

        private sealed class InteriorRuntimeBundle
        {
            public TopDownPlayerController PlayerController = null!;
            public CombatHealth PlayerHealth = null!;
            public HeatSystemController HeatSystemController = null!;
            public SaveGameFileService SaveGameFileService = null!;
            public CampaignProgressionController CampaignProgressionController = null!;
            public SceneTransitionController SceneTransitionController = null!;
            public DialogueController DialogueController = null!;
        }

        public static void RunBatchPrototypePopulate()
        {
            AssetDatabase.Refresh();
            PrototypeDataBuilder.CreatePrototypeDataAssets();
            BuildBootScene();
            BuildDistrictScene();
            BuildBusinessCoreScene();
            BuildOldQuarterScene();
            BuildRailYardScene();
            BuildBusinessInteriorScene();
            BuildOldQuarterInteriorScene();
            BuildRailYardInteriorScene();
            BuildDocksWarehouseInteriorScene();
            BuildBusinessPrintShopInteriorScene();
            BuildOldQuarterTenementInteriorScene();
            BuildRailYardDispatchInteriorScene();
            BuildDocksSocialClubInteriorScene();
            BuildBusinessUnionHallInteriorScene();
            BuildOldQuarterDinerInteriorScene();
            BuildRailYardLockerInteriorScene();
            BuildInteriorScene();
            ConfigureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        [MenuItem("MafiaTopDown/Setup/Populate Prototype Scenes")]
        public static void BuildPrototypeScenesFromMenu()
        {
            RunBatchPrototypePopulate();
        }

        private static void BuildBootScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/Boot.unity");
            ClearScene(scene);

            CreateBootTitleDiorama();

            var bootRoot = new GameObject("BootFlow");
            var saveGameFileService = bootRoot.AddComponent<SaveGameFileService>();
            SetStringValue(saveGameFileService, "defaultSceneName", "District_01");
            SetStringValue(saveGameFileService, "defaultSpawnPointId", "PickupSpawn");
            var bootFlowController = bootRoot.AddComponent<BootFlowController>();
            SetStringValue(bootFlowController, "firstPlayableSpawnPoint", "PickupSpawn");
            SetObjectReference(bootFlowController, "saveGameFileService", saveGameFileService);

            EditorSceneManager.SaveScene(scene);
        }

        private static void CreateBootTitleDiorama()
        {
            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/MenuWetAsphalt.mat", new Color(0.045f, 0.052f, 0.058f), 0.9f, 0.02f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/MenuRainSidewalk.mat", new Color(0.19f, 0.18f, 0.16f), 0.42f, 0f);
            var brick = GetOrCreateMaterial("Assets/Game/Materials/MenuSootBrick.mat", new Color(0.17f, 0.075f, 0.055f), 0.24f, 0f);
            var roof = GetOrCreateMaterial("Assets/Game/Materials/MenuTarRoof.mat", new Color(0.035f, 0.037f, 0.04f), 0.26f, 0f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/MenuOiledMetal.mat", new Color(0.16f, 0.17f, 0.17f), 0.72f, 0.65f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/MenuAgedBrass.mat", new Color(0.62f, 0.42f, 0.16f), 0.72f, 0.22f);
            var glow = GetOrCreateMaterial("Assets/Game/Materials/MenuWindowGlow.mat", new Color(1f, 0.70f, 0.32f), 0.88f, 0.04f, new Color(1f, 0.54f, 0.18f) * 3.6f);
            var neon = GetOrCreateMaterial("Assets/Game/Materials/MenuRedNeon.mat", new Color(0.88f, 0.22f, 0.11f), 0.8f, 0.08f, new Color(1f, 0.14f, 0.05f) * 3.4f);
            var water = GetOrCreateMaterial("Assets/Game/Materials/MenuBlackHarborWater.mat", new Color(0.03f, 0.055f, 0.065f), 0.95f, 0.02f);
            var wood = GetOrCreateMaterial("Assets/Game/Materials/MenuWetWood.mat", new Color(0.28f, 0.17f, 0.095f), 0.46f, 0f);
            var rain = GetOrCreateMaterial("Assets/Game/Materials/MenuRainStreak.mat", new Color(0.34f, 0.42f, 0.46f, 0.55f), 0.58f, 0f);

            ApplyExteriorAtmosphere(
                new Color(0.065f, 0.072f, 0.078f),
                new Color(0.035f, 0.044f, 0.050f),
                0.016f,
                new Color(0.82f, 0.68f, 0.52f),
                0.55f,
                Quaternion.Euler(34f, -42f, 0f));

            var root = new GameObject("BootTitleDiorama_ReadyAssetHarbor");

            CreateVisualPrimitive(PrimitiveType.Cube, "BootHarborWaterPlane", new Vector3(16f, -0.04f, 6.5f), new Vector3(22f, 0.04f, 33f), water).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootMainWetStreet", new Vector3(-1.5f, 0.02f, 0f), new Vector3(13.5f, 0.04f, 35f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootLeftSidewalk", new Vector3(-9.2f, 0.11f, 0f), new Vector3(4.2f, 0.18f, 35f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootRightPierWalk", new Vector3(6.7f, 0.12f, 0f), new Vector3(3.0f, 0.2f, 35f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootHarborPierFace", new Vector3(8.4f, 0.38f, 0f), new Vector3(0.55f, 0.72f, 35f), wood).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWetCrossStreet", new Vector3(-1.5f, 0.03f, 4.4f), new Vector3(28f, 0.035f, 6.2f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootStreetGoldCenterLineA", new Vector3(-1.5f, 0.065f, -9f), new Vector3(0.28f, 0.025f, 4.2f), brass).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootStreetGoldCenterLineB", new Vector3(-1.5f, 0.065f, 2.5f), new Vector3(0.28f, 0.025f, 4.2f), brass).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootStreetGoldCenterLineC", new Vector3(-1.5f, 0.065f, 13.8f), new Vector3(0.28f, 0.025f, 4.2f), brass).transform.SetParent(root.transform, true);

            EnvironmentArtCatalog.CreateDocksBuilding(DocksBuildingRole.CornerShop, "BootReadyMorettiCafe", new Vector3(-12.1f, 0.05f, -10.4f), new Vector3(0f, 90f, 0f), new Vector3(2.35f, 2.55f, 2.35f), root.transform);
            EnvironmentArtCatalog.CreateDocksBuilding(DocksBuildingRole.Warehouse, "BootReadyWarehouseNorth", new Vector3(-12.45f, 0.05f, 3.2f), new Vector3(0f, 90f, 0f), new Vector3(2.8f, 2.7f, 2.55f), root.transform);
            EnvironmentArtCatalog.CreateDocksBuilding(DocksBuildingRole.OfficeBlock, "BootReadyHarborOffice", new Vector3(-12.35f, 0.05f, 14.1f), new Vector3(0f, 90f, 0f), new Vector3(2.45f, 2.65f, 2.45f), root.transform);
            EnvironmentArtCatalog.CreateDocksBuilding(DocksBuildingRole.RowHouse, "BootReadyForegroundTenement", new Vector3(4.6f, 0.05f, 17.5f), new Vector3(0f, 180f, 0f), new Vector3(2.8f, 2.45f, 2.4f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "detail-tank", "BootReadyFuelTank", new Vector3(10.6f, 0.08f, -8.8f), Vector3.zero, new Vector3(2.2f, 2.2f, 2.2f), root.transform);
            CreateLowProfileStreetLamp("BootReadyWorkLight", new Vector3(4.3f, 0.08f, -12.5f), root.transform);
            CreateLowProfileStreetLamp("BootReadyStreetLightLeft", new Vector3(-5.6f, 0.08f, -6.6f), root.transform);
            CreateLowProfileStreetLamp("BootReadyStreetLightRight", new Vector3(4.9f, 0.08f, 7.8f), root.transform);

            CreateVisualPrimitive(PrimitiveType.Cube, "BootHeroLeftBrickFacade", new Vector3(-12.9f, 3.15f, -4.8f), new Vector3(1.25f, 6.3f, 13.5f), brick).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootHeroLeftRoofCap", new Vector3(-12.9f, 6.55f, -4.8f), new Vector3(1.45f, 0.32f, 13.9f), roof).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootHeroRightWarehouseWall", new Vector3(9.4f, 2.85f, 2.8f), new Vector3(1.25f, 5.7f, 16.2f), brick).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootHeroRightRoofCap", new Vector3(9.4f, 5.92f, 2.8f), new Vector3(1.45f, 0.32f, 16.7f), roof).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootBackAlleyBlock", new Vector3(1.2f, 3.8f, 18.2f), new Vector3(12.5f, 7.6f, 1.25f), brick).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootBackAlleyRoof", new Vector3(1.2f, 7.75f, 18.2f), new Vector3(12.9f, 0.35f, 1.45f), roof).transform.SetParent(root.transform, true);

            for (var floor = 0; floor < 4; floor += 1)
            {
                var y = 1.45f + (floor * 1.22f);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootRightBrickCourse_" + floor, new Vector3(8.70f, y, 2.8f), new Vector3(0.07f, 0.055f, 15.4f), roof).transform.SetParent(root.transform, true);
            }

            for (var bay = 0; bay < 5; bay += 1)
            {
                var z = -4.9f + (bay * 2.55f);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWindowFrameA_" + bay, new Vector3(8.67f, 3.15f, z), new Vector3(0.07f, 0.78f, 0.08f), metal).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWindowFrameB_" + bay, new Vector3(8.67f, 3.15f, z + 0.56f), new Vector3(0.07f, 0.78f, 0.08f), metal).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWindowSill_" + bay, new Vector3(8.66f, 2.74f, z + 0.28f), new Vector3(0.12f, 0.08f, 0.88f), brass).transform.SetParent(root.transform, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseGarageShutter", new Vector3(8.64f, 1.22f, -4.1f), new Vector3(0.12f, 1.72f, 2.55f), metal).transform.SetParent(root.transform, true);
            for (var stripe = 0; stripe < 5; stripe += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseShutterSlat_" + stripe, new Vector3(8.56f, 0.62f + (stripe * 0.27f), -4.1f), new Vector3(0.08f, 0.035f, 2.42f), brass).transform.SetParent(root.transform, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseFireEscapeDeckA", new Vector3(8.50f, 3.62f, -1.0f), new Vector3(0.42f, 0.08f, 2.1f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseFireEscapeDeckB", new Vector3(8.50f, 4.64f, 1.2f), new Vector3(0.42f, 0.08f, 2.1f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseFireEscapeRailA", new Vector3(8.31f, 3.94f, -1.0f), new Vector3(0.055f, 0.58f, 2.1f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseFireEscapeRailB", new Vector3(8.31f, 4.96f, 1.2f), new Vector3(0.055f, 0.58f, 2.1f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseLadderA", new Vector3(8.24f, 4.22f, 0.08f), new Vector3(0.055f, 1.5f, 0.055f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseLadderB", new Vector3(8.24f, 4.22f, 0.42f), new Vector3(0.055f, 1.5f, 0.055f), metal).transform.SetParent(root.transform, true);

            CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWarehouseSignBacker", new Vector3(8.54f, 2.26f, 4.95f), new Vector3(0.12f, 0.72f, 2.85f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWarehouseSignStripe", new Vector3(8.46f, 2.42f, 4.95f), new Vector3(0.08f, 0.10f, 2.45f), neon).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWarehouseServiceGlow", new Vector3(8.45f, 1.18f, 5.95f), new Vector3(0.08f, 1.15f, 0.62f), glow).transform.SetParent(root.transform, true);

            for (var index = 0; index < 7; index += 1)
            {
                var z = -10.2f + (index * 2.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootLeftWindowGlow_" + index, new Vector3(-12.22f, 3.2f + ((index % 2) * 1.55f), z), new Vector3(0.08f, 0.62f, 0.55f), glow).transform.SetParent(root.transform, true);
            }

            for (var index = 0; index < 8; index += 1)
            {
                var z = -5.1f + (index * 1.85f);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootRightWarehouseWindow_" + index, new Vector3(8.74f, 3.05f + ((index % 3) * 0.9f), z), new Vector3(0.08f, 0.44f, 0.42f), glow).transform.SetParent(root.transform, true);
            }

            for (var index = 0; index < 6; index += 1)
            {
                var x = -4.4f + (index * 1.85f);
                CreateVisualPrimitive(PrimitiveType.Cube, "BootBackAlleyWindow_" + index, new Vector3(x, 4.2f + ((index % 2) * 1.3f), 17.52f), new Vector3(0.46f, 0.58f, 0.08f), glow).transform.SetParent(root.transform, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "BootForegroundCafeAwning", new Vector3(-12.12f, 2.15f, -9.25f), new Vector3(1.35f, 0.18f, 2.35f), brass).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootForegroundWarehouseAwning", new Vector3(8.55f, 2.25f, 2.6f), new Vector3(1.7f, 0.18f, 3.2f), brass).transform.SetParent(root.transform, true);

            CreateVisualPrimitive(PrimitiveType.Cube, "BootCafeSignBacker", new Vector3(-12.32f, 3.1f, -10.4f), new Vector3(0.16f, 0.78f, 3.25f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootCafeSignNeon", new Vector3(-12.15f, 3.16f, -10.4f), new Vector3(0.08f, 0.26f, 2.65f), neon).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootCafeDoorGlow", new Vector3(-12.05f, 1.2f, -9.15f), new Vector3(0.08f, 0.96f, 0.58f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWarehouseLoadingGlow", new Vector3(-12.02f, 1.35f, 3.2f), new Vector3(0.08f, 1.7f, 1.35f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootBillboardBacker", new Vector3(2.2f, 4.8f, 13.8f), new Vector3(7.2f, 2.1f, 0.18f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootBillboardWarmFace", new Vector3(2.2f, 4.86f, 13.66f), new Vector3(6.65f, 1.55f, 0.08f), brass).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootBillboardTitleStripe", new Vector3(2.2f, 5.24f, 13.56f), new Vector3(5.8f, 0.12f, 0.08f), neon).transform.SetParent(root.transform, true);

            CreateStaticVehicle("BootMenuSedan", new Vector3(0.8f, 0.72f, -4.6f), new Vector3(1.72f, 0.9f, 3.85f), new Color(0.055f, 0.052f, 0.048f), roof, glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootSedanWetReflection", new Vector3(0.8f, 0.075f, -4.6f), new Vector3(2.4f, 0.018f, 4.8f), water).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootSedanHeadlightConeLeft", new Vector3(0.38f, 0.11f, -8.9f), new Vector3(0.35f, 0.018f, 5.2f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootSedanHeadlightConeRight", new Vector3(1.22f, 0.11f, -8.9f), new Vector3(0.35f, 0.018f, 5.2f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootWetStreetReflectionLong", new Vector3(4.2f, 0.078f, -1.8f), new Vector3(0.42f, 0.018f, 13.5f), water).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootRightNeonReflection", new Vector3(6.8f, 0.082f, 2.6f), new Vector3(0.34f, 0.018f, 8.2f), neon).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootSteamVentA", new Vector3(-5.1f, 0.52f, -3.7f), new Vector3(0.24f, 0.88f, 0.24f), rain).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "BootSteamVentB", new Vector3(-4.85f, 0.78f, -3.45f), new Vector3(0.16f, 1.18f, 0.16f), rain).transform.SetParent(root.transform, true);

            CreateBootRainField(root.transform, rain);
            CreateBootMenuLight("BootCafeWarmPool", root.transform, new Vector3(-4.8f, 3.0f, -9.2f), new Color(1f, 0.58f, 0.22f), 8.2f, 3.4f);
            CreateBootMenuLight("BootSedanHeadlampPool", root.transform, new Vector3(0.8f, 1.1f, -6.8f), new Color(1f, 0.76f, 0.44f), 7.4f, 2.5f);
            CreateBootMenuLight("BootHarborSodiumPool", root.transform, new Vector3(5.0f, 3.4f, 7.8f), new Color(1f, 0.66f, 0.30f), 9.0f, 3.1f);

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(-4.85f, 2.95f, -13.2f);
            cameraObject.transform.rotation = Quaternion.Euler(9.5f, 23.0f, 0f);
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.010f, 0.012f, 0.014f);
            camera.fieldOfView = 49f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 120f;
        }

        private static void CreateBootRainField(Transform root, Material rainMaterial)
        {
            for (var index = 0; index < 84; index += 1)
            {
                var x = -14f + (StableNoise(index, 3) * 30f);
                var y = 3.0f + (StableNoise(index, 7) * 7.5f);
                var z = -16f + (StableNoise(index, 11) * 34f);
                var streak = CreateVisualPrimitive(
                    PrimitiveType.Cube,
                    "BootRainStreak_" + index,
                    new Vector3(x, y, z),
                    new Vector3(0.025f, 0.72f + (StableNoise(index, 19) * 0.7f), 0.025f),
                    rainMaterial);
                streak.transform.SetParent(root, true);
                streak.transform.rotation = Quaternion.Euler(0f, 0f, -12f);
            }
        }

        private static void CreateBootMenuLight(string name, Transform root, Vector3 position, Color color, float range, float intensity)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(root, true);
            lightObject.transform.position = position;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = range;
            light.intensity = intensity;
            light.shadows = LightShadows.None;
        }

        private static void BuildDistrictScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/District_01.unity");
            ClearScene(scene);
            var missionAsset = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/AQuietFavor.asset");
            var pierNightWatchMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/PierNightWatch.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var lucaBriefing = AssetDatabase.LoadAssetAtPath<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/LucaBriefing.asset");
            var dockCourierActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/DockCourier.asset");
            var ledgerRunActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/LedgerRun.asset");
            var chopDeliveryActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/ChopDelivery.asset");
            var docksideMuscleActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/DocksideMuscle.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.09f, 0.105f, 0.125f), 0.86f, 0.02f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/Sidewalk.mat", new Color(0.24f, 0.23f, 0.21f), 0.34f, 0f);
            var lanePaint = GetOrCreateMaterial("Assets/Game/Materials/LanePaint.mat", new Color(0.9f, 0.76f, 0.34f), 0.65f, 0f);
            var brick = GetOrCreateMaterial("Assets/Game/Materials/Brick.mat", new Color(0.2f, 0.11f, 0.1f), 0.18f, 0f);
            var warmDoor = GetOrCreateMaterial("Assets/Game/Materials/WarmDoor.mat", new Color(0.62f, 0.49f, 0.28f), 0.28f, 0f);
            var sedanPaint = GetOrCreateMaterial("Assets/Game/Materials/SedanPaint.mat", new Color(0.37f, 0.07f, 0.08f), 0.8f, 0.1f);
            var awning = GetOrCreateMaterial("Assets/Game/Materials/Awning.mat", new Color(0.1f, 0.12f, 0.14f), 0.18f, 0f);
            var playerCoat = GetOrCreateMaterial("Assets/Game/Materials/PlayerCoat.mat", new Color(0.16f, 0.145f, 0.12f), 0.32f, 0f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.20f, 0.13f, 0.08f), 0.24f, 0f);
            var roof = GetOrCreateMaterial("Assets/Game/Materials/Roof.mat", new Color(0.055f, 0.06f, 0.07f), 0.22f, 0f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(1f, 0.78f, 0.42f), 0.86f, 0.08f, new Color(1f, 0.68f, 0.28f) * 3.2f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var officeSign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var water = GetOrCreateMaterial("Assets/Game/Materials/DockWater.mat", new Color(0.07f, 0.13f, 0.17f), 0.92f, 0.08f, new Color(0.08f, 0.16f, 0.2f) * 0.25f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);
            var stone = GetOrCreateMaterial("Assets/Game/Materials/Stone.mat", new Color(0.28f, 0.27f, 0.24f), 0.22f, 0f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);

            var light = new GameObject("Sun");
            var lightComponent = light.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.color = new Color(1f, 0.88f, 0.73f);
            lightComponent.intensity = 0.72f;
            lightComponent.shadows = LightShadows.Soft;
            lightComponent.shadowStrength = 0.22f;
            light.transform.rotation = Quaternion.Euler(62f, -38f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.14f, 0.145f, 0.155f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.09f, 0.105f, 0.13f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.009f;
            CreateNoirPostProcessVolume("District_01", new Color(0.92f, 0.96f, 1f), 0.26f, 21f, -4f, 1.4f, 0.14f);

            CreateSteamReadyDocksCityPass(asphalt, brick, roof, metal, windowGlow, crateWood, officeSign, sidewalk, puddle, water, stone, brass, lanePaint);

            var openingSpawn = new Vector3(-14.8f, 1f, -22.5f);
            var player = CreateNoirActor("Player", openingSpawn, playerCoat, null, false);
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            var playerController = player.AddComponent<TopDownPlayerController>();
            var interactionController = player.AddComponent<PlayerInteractionController>();
            var playerHealth = player.AddComponent<CombatHealth>();
            var playerCombatController = player.AddComponent<PlayerCombatController>();
            SetObjectReference(playerController, "characterController", characterController);
            SetObjectReference(interactionController, "playerController", playerController);
            SetStringValue(playerHealth, "displayName", "Tommy");
            SetIntValue(playerHealth, "maximumHealth", 3);
            SetBoolValue(playerHealth, "destroyOnDeath", false);
            SetBoolValue(playerHealth, "reloadSceneOnDeath", true);

            CreateSpawnPoint("DefaultSpawnPoint", "DefaultSpawn", openingSpawn);
            CreateSpawnPoint("PickupSpawnPoint", "PickupSpawn", openingSpawn);
            CreateSpawnPoint("DriveSpawnPoint", "DriveSpawn", new Vector3(-14.4f, 1f, -14.0f));
            CreateSpawnPoint("ExteriorReturnSpawnPoint", "ExteriorReturn", new Vector3(0f, 1f, 12.5f));
            CreateSpawnPoint("PierExitSpawnPoint", "PierExitSpawn", new Vector3(-3.2f, 1f, 9.8f));
            CreateSpawnPoint("FromBusinessCoreGatePoint", "FromBusinessCoreGate", new Vector3(0f, 1f, -12.8f));
            CreateSpawnPoint("FromRailYardGatePoint", "FromRailYardGate", new Vector3(6.8f, 1f, -1.6f));
            CreateSpawnPoint("FromWarehouseInteriorPoint", "FromWarehouseInterior", new Vector3(-5.15f, 1f, -9.9f));
            CreateSpawnPoint("FromSocialClubInteriorPoint", "FromSocialClubInterior", new Vector3(5.15f, 1f, 5.4f));

            var campaignSystems = new GameObject("CampaignSystems");
            var saveGameFileService = campaignSystems.AddComponent<SaveGameFileService>();
            SetStringValue(saveGameFileService, "defaultSceneName", "District_01");
            SetStringValue(saveGameFileService, "defaultSpawnPointId", "PickupSpawn");
            var heatSystemController = campaignSystems.AddComponent<HeatSystemController>();
            SetObjectReference(heatSystemController, "saveGameFileService", saveGameFileService);
            var activityProgressionController = campaignSystems.AddComponent<ActivityProgressionController>();
            SetObjectReference(activityProgressionController, "campaignDatabase", campaignDatabase);
            SetObjectReference(activityProgressionController, "saveGameFileService", saveGameFileService);
            var campaignProgressionController = campaignSystems.AddComponent<CampaignProgressionController>();
            SetObjectReference(campaignProgressionController, "campaignDatabase", campaignDatabase);
            SetObjectReference(campaignProgressionController, "saveGameFileService", saveGameFileService);
            var sceneBootstrapper = campaignSystems.AddComponent<CampaignSceneBootstrapper>();
            SetObjectReference(sceneBootstrapper, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(sceneBootstrapper, "bootstrapChapter", chapterAsset);
            SetObjectReference(sceneBootstrapper, "bootstrapMission", missionAsset);
            var sceneSpawnController = campaignSystems.AddComponent<SceneSpawnController>();
            SetObjectReference(sceneSpawnController, "saveGameFileService", saveGameFileService);
            SetObjectReference(sceneSpawnController, "playerController", playerController);
            SetStringValue(sceneSpawnController, "defaultSpawnPointId", "DefaultSpawn");
            var dialogueController = campaignSystems.AddComponent<DialogueController>();
            SetObjectReference(dialogueController, "playerController", playerController);

            var cameraRoot = new GameObject("CameraRig");
            var camera = cameraRoot.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.28f, 0.29f, 0.33f);
            camera.orthographic = false;
            camera.fieldOfView = 38f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 140f;
            var tunedOpeningCameraOffset = new Vector3(0.2f, 4.85f, -4.2f);
            cameraRoot.transform.position = player.transform.position + tunedOpeningCameraOffset;
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            ConfigureNoirCamera(camera, topDownCamera, tunedOpeningCameraOffset, 58f, 8f, 4.4f, 8.2f);

            var sceneTransitionControllerObject = new GameObject("SceneTransitionController");
            var sceneTransitionController = sceneTransitionControllerObject.AddComponent<SceneTransitionController>();
            SetObjectReference(sceneTransitionController, "saveGameFileService", saveGameFileService);

            var missionSystem = CreateSceneObjectiveSystem("MissionSystem");

            var hudObject = new GameObject("ObjectiveHud");
            var hud = hudObject.AddComponent<ObjectiveHudController>();
            SetObjectReference(hud, "objectiveSystem", missionSystem);
            SetObjectReference(hud, "saveGameFileService", saveGameFileService);
            SetObjectReference(hud, "campaignDatabase", campaignDatabase);
            var statusHudObject = new GameObject("StatusHud");
            var statusHud = statusHudObject.AddComponent<StatusHudController>();
            SetObjectReference(statusHud, "playerHealth", playerHealth);
            SetObjectReference(statusHud, "heatSystemController", heatSystemController);
            var activityHudObject = new GameObject("ActivityHud");
            var activityHud = activityHudObject.AddComponent<ActivityHudController>();
            SetObjectReference(activityHud, "activityProgressionController", activityProgressionController);
            var pauseMenuObject = new GameObject("PauseMenu");
            var pauseMenu = pauseMenuObject.AddComponent<PauseMenuController>();
            SetObjectReference(pauseMenu, "playerController", playerController);
            SetObjectReference(pauseMenu, "dialogueController", dialogueController);

            var docksMissionRoot = new GameObject("AQuietFavorDistrictRoot");
            var luca = CreateNoirActor("LucaContact", new Vector3(-16.4f, 1f, -21.25f), contactCoat, docksMissionRoot.transform);
            var lucaInteractable = luca.AddComponent<DialogueInteractable>();
            SetObjectReference(lucaInteractable, "dialogueController", dialogueController);
            SetObjectReference(lucaInteractable, "dialogueSequence", lucaBriefing);
            SetStringValue(lucaInteractable, "promptText", "Talk to Luca");

            var car = GameObject.CreatePrimitive(PrimitiveType.Cube);
            car.name = "Sedan";
            car.transform.SetParent(docksMissionRoot.transform, false);
            car.transform.position = new Vector3(-12.95f, 0.75f, -23.75f);
            car.transform.localScale = new Vector3(2.2f, 1f, 4.6f);
            AssignMaterial(car, sedanPaint);
            HideRenderer(car);
            var carBody = car.AddComponent<Rigidbody>();
            carBody.useGravity = false;
            carBody.mass = 1200f;
            carBody.interpolation = RigidbodyInterpolation.Interpolate;
            carBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            carBody.constraints = RigidbodyConstraints.FreezePositionY |
                RigidbodyConstraints.FreezeRotationX |
                RigidbodyConstraints.FreezeRotationZ;
            var seatController = car.AddComponent<VehicleSeatController>();
            var vehicleDriver = car.AddComponent<SimpleVehicleDriver>();
            var driverAnchor = new GameObject("DriverAnchor").transform;
            driverAnchor.SetParent(car.transform, false);
            driverAnchor.localPosition = new Vector3(0f, 0.2f, 0f);
            var exitAnchor = new GameObject("ExitAnchor").transform;
            exitAnchor.SetParent(car.transform, false);
            exitAnchor.localPosition = new Vector3(1.6f, 0f, 0f);
            SetObjectReference(seatController, "driverAnchor", driverAnchor);
            SetObjectReference(seatController, "exitAnchor", exitAnchor);
            SetObjectReference(vehicleDriver, "seatController", seatController);
            SetObjectReference(seatController, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(seatController, "missionAsset", missionAsset);
            SetStringValue(seatController, "missionStageIdOnEnter", "pickup");
            BuildNoirVehicleVisual(car, "Sedan", car.transform.position, car.transform.localScale, sedanPaint, roof, brass, windowGlow);

            var backOfficeRoot = new GameObject("BackOfficeReadyAssetRoot");
            PlaceDistrictBuilding(
                DistrictArtStyle.Docks,
                DistrictBuildingRole.HeroLandmark,
                "ReadyAsset_BackOfficeWarehouse",
                new Vector3(0f, 0.08f, 20f),
                new Vector3(0f, 0f, 0f),
                new Vector3(3.2f, 3.2f, 3.2f),
                brick,
                roof,
                windowGlow,
                backOfficeRoot.transform);
            CreateColliderBlock("BackOfficeBuildingCollision", new Vector3(0f, 2.8f, 20f), new Vector3(12f, 5.6f, 8f));

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "DoorAwning",
                new Vector3(0f, 2.35f, 15.6f),
                new Vector3(4f, 0.3f, 1.4f),
                awning);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "OfficeSign",
                new Vector3(0f, 3.8f, 15.8f),
                new Vector3(3.8f, 0.65f, 0.2f),
                officeSign);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "OfficeSignTrim",
                new Vector3(0f, 3.8f, 15.68f),
                new Vector3(4.15f, 0.82f, 0.08f),
                brass);

            var signLightObject = new GameObject("OfficeSignLight");
            signLightObject.transform.position = new Vector3(0f, 3.25f, 15f);
            var signLight = signLightObject.AddComponent<Light>();
            signLight.type = LightType.Point;
            signLight.range = 7f;
            signLight.intensity = 1.8f;
            signLight.color = new Color(1f, 0.77f, 0.52f);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "LeftCrateStack",
                new Vector3(-3.1f, 0.6f, 15.8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                crateWood);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "RightCrateStack",
                new Vector3(3.1f, 0.6f, 15.8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                crateWood);

            CreateTravelGate(
                "ToBusinessCoreGate",
                new Vector3(0f, 1.05f, -16.4f),
                new Vector3(6.6f, 2.1f, 0.4f),
                officeSign,
                "Drive toward the business core",
                "business-core",
                "District_BusinessCore_01",
                "FromDocksGate",
                sceneTransitionController,
                saveGameFileService);

            CreateTravelGate(
                "ToRailYardGate",
                new Vector3(6.95f, 1.05f, -1.8f),
                new Vector3(0.55f, 2.1f, 5.2f),
                officeSign,
                "Cut through to the rail yard",
                "rail-yard",
                "District_RailYard_01",
                "FromDocksGate",
                sceneTransitionController,
                saveGameFileService);

            CreateStaticVehicle(
                "ParkedCoupe",
                new Vector3(4.9f, 0.7f, 8.6f),
                new Vector3(1.7f, 0.95f, 3.8f),
                new Color(0.08f, 0.08f, 0.09f),
                roof,
                windowGlow);

            CreateStaticVehicle(
                "HarborTruck",
                new Vector3(-8.8f, 0.75f, -6.5f),
                new Vector3(2.3f, 1.2f, 5.2f),
                new Color(0.2f, 0.23f, 0.24f),
                roof,
                windowGlow);

            CreateEnemyGuard(
                "DockGuard",
                new Vector3(-2.6f, 1f, 12.4f),
                enemyCoat,
                playerController,
                playerHealth);

            CreateEnemyGuard(
                "OfficeWatchman",
                new Vector3(2.8f, 1f, 14.6f),
                enemyCoat,
                playerController,
                playerHealth);

            SetObjectReference(playerCombatController, "playerController", playerController);
            SetObjectReference(playerCombatController, "combatHealth", playerHealth);
            SetObjectReference(playerCombatController, "heatSystemController", heatSystemController);

            var arrivalTrigger = new GameObject("ArrivalTrigger");
            arrivalTrigger.transform.SetParent(docksMissionRoot.transform, false);
            arrivalTrigger.transform.position = new Vector3(0f, 1f, 11f);
            var arrivalCollider = arrivalTrigger.AddComponent<BoxCollider>();
            arrivalCollider.isTrigger = true;
            arrivalCollider.size = new Vector3(10f, 2f, 6f);
            var arrivalZone = arrivalTrigger.AddComponent<ObjectiveTriggerZone>();
            SetObjectReference(arrivalZone, "objectiveSystem", missionSystem);
            SetStringValue(arrivalZone, "requiredObjectiveId", "objective-drive");
            SetObjectReference(arrivalZone, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(arrivalZone, "missionAsset", missionAsset);
            SetStringValue(arrivalZone, "missionStageIdToComplete", "drive");

            var door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "OfficeDoor";
            door.transform.SetParent(docksMissionRoot.transform, false);
            door.transform.position = new Vector3(0f, 1f, 15.4f);
            door.transform.localScale = new Vector3(2.2f, 2.2f, 0.4f);
            AssignMaterial(door, warmDoor);
            var doorInteractable = door.AddComponent<SceneDoorInteractable>();
            SetObjectReference(doorInteractable, "sceneTransitionController", sceneTransitionController);
            SetStringValue(doorInteractable, "promptText", "Enter the back office");
            SetStringValue(doorInteractable, "sourceScene", "District_01");
            SetStringValue(doorInteractable, "targetScene", "Interior_BackOffice_01");
            SetStringValue(doorInteractable, "spawnPointId", "InteriorSpawn");
            SetObjectReference(doorInteractable, "objectiveSystem", missionSystem);
            SetStringValue(doorInteractable, "requiredObjectiveId", "objective-enter-office");

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "ColdStorageApron",
                new Vector3(-5.82f, 0.145f, -9.9f),
                new Vector3(1.9f, 0.08f, 2.25f),
                sidewalk);
            CreateFreeRoamSceneDoor(
                "ColdStorageWarehouseDoor",
                new Vector3(-6.72f, 1.1f, -9.9f),
                new Vector3(0.35f, 2.2f, 1.85f),
                officeSign,
                "Enter cold-storage warehouse",
                "District_01",
                "Interior_Docks_Warehouse_01",
                "WarehouseInteriorSpawn",
                sceneTransitionController);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "HarborSocialClubApron",
                new Vector3(5.82f, 0.145f, 5.4f),
                new Vector3(1.9f, 0.08f, 2.25f),
                sidewalk);
            CreateFreeRoamSceneDoor(
                "HarborSocialClubDoor",
                new Vector3(6.72f, 1.1f, 5.4f),
                new Vector3(0.35f, 2.2f, 1.85f),
                officeSign,
                "Enter harbor social club",
                "District_01",
                "Interior_Docks_SocialClub_01",
                "SocialClubInteriorSpawn",
                sceneTransitionController);

            var pierNightWatchRoot = CreateObjectiveActionMissionRoot(
                "PierNightWatchMissionRoot",
                "PierWatchMarker",
                new Vector3(-11.8f, 1f, 3.8f),
                new Vector3(1.2f, 1.2f, 1.2f),
                crateWood,
                "Inspect the marked crate",
                missionSystem,
                "check-pier",
                campaignProgressionController,
                pierNightWatchMission,
                "check-pier");
            CreateMissionTravelGate(
                pierNightWatchRoot.transform,
                "PierNightWatchExit",
                new Vector3(5.8f, 1.1f, -13.8f),
                new Vector3(2.4f, 2.2f, 0.4f),
                officeSign,
                "Clear the harbor",
                "Interior_RailYard_Garage_01",
                "GarageInteriorSpawn",
                "leave-harbor",
                sceneTransitionController,
                saveGameFileService,
                missionSystem,
                campaignProgressionController,
                pierNightWatchMission,
                "leave-harbor");

            var docksMissionDirectorObject = new GameObject("DocksMissionDirector");
            var docksMissionDirector = docksMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                docksMissionDirector,
                saveGameFileService,
                missionSystem,
                (missionAsset, docksMissionRoot, new[]
                {
                    ("objective-drive", "Reach the back office", "Drive the sedan to the family office.", "objective-enter-office"),
                    ("objective-enter-office", "Enter the back office", "Walk to the door and head inside.", null)
                }),
                (pierNightWatchMission, pierNightWatchRoot, new[]
                {
                    ("check-pier", "Check the pier", "Inspect the marked crate on the late pier.", "leave-harbor"),
                    ("leave-harbor", "Leave the harbor", "Get clear before the next watch rolls in.", null)
                }));

            var dockCourierStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dockCourierStart.name = "DockCourierStart";
            dockCourierStart.transform.position = new Vector3(-4.6f, 1f, -8.6f);
            dockCourierStart.transform.localScale = new Vector3(1.4f, 1.6f, 1.4f);
            AssignMaterial(dockCourierStart, crateWood);
            var dockCourierStartInteractable = dockCourierStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(dockCourierStartInteractable, "activityAsset", dockCourierActivity);
            SetObjectReference(dockCourierStartInteractable, "activityProgressionController", activityProgressionController);
            SetStringValue(dockCourierStartInteractable, "promptText", "Take dock courier job");

            var ledgerRunComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ledgerRunComplete.name = "LedgerRunComplete";
            ledgerRunComplete.transform.position = new Vector3(2.8f, 1f, 14.8f);
            ledgerRunComplete.transform.localScale = new Vector3(1.2f, 1.5f, 1.2f);
            AssignMaterial(ledgerRunComplete, brass);
            var ledgerRunCompleteInteractable = ledgerRunComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(ledgerRunCompleteInteractable, "activityAsset", ledgerRunActivity);
            SetObjectReference(ledgerRunCompleteInteractable, "activityProgressionController", activityProgressionController);
            SetStringValue(ledgerRunCompleteInteractable, "promptText", "Drop off the ledger run");

            var chopDeliveryComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chopDeliveryComplete.name = "ChopDeliveryComplete";
            chopDeliveryComplete.transform.position = new Vector3(-10.8f, 0.8f, -7.8f);
            chopDeliveryComplete.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            AssignMaterial(chopDeliveryComplete, metal);
            var chopDeliveryCompleteInteractable = chopDeliveryComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(chopDeliveryCompleteInteractable, "activityAsset", chopDeliveryActivity);
            SetObjectReference(chopDeliveryCompleteInteractable, "activityProgressionController", activityProgressionController);
            SetStringValue(chopDeliveryCompleteInteractable, "promptText", "Drop the chopped cargo");

            var docksideMuscleStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            docksideMuscleStart.name = "DocksideMuscleStart";
            docksideMuscleStart.transform.position = new Vector3(5.6f, 1f, 5.4f);
            docksideMuscleStart.transform.localScale = new Vector3(1.35f, 1.55f, 1.35f);
            AssignMaterial(docksideMuscleStart, officeSign);
            var docksideMuscleStartInteractable = docksideMuscleStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(docksideMuscleStartInteractable, "activityAsset", docksideMuscleActivity);
            SetObjectReference(docksideMuscleStartInteractable, "activityProgressionController", activityProgressionController);
            SetStringValue(docksideMuscleStartInteractable, "promptText", "Take dockside muscle job");

            var docksideMuscleComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            docksideMuscleComplete.name = "DocksideMuscleComplete";
            docksideMuscleComplete.transform.position = new Vector3(-12.4f, 1f, 6.8f);
            docksideMuscleComplete.transform.localScale = new Vector3(1.2f, 1.4f, 1.2f);
            AssignMaterial(docksideMuscleComplete, brass);
            var docksideMuscleCompleteInteractable = docksideMuscleComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(docksideMuscleCompleteInteractable, "activityAsset", docksideMuscleActivity);
            SetObjectReference(docksideMuscleCompleteInteractable, "activityProgressionController", activityProgressionController);
            SetStringValue(docksideMuscleCompleteInteractable, "promptText", "Collect the dockside envelope");

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildInteriorScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/Interior_BackOffice_01.unity");
            ClearScene(scene);
            var missionAsset = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/AQuietFavor.asset");
            var bloodLedgerMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/BloodLedger.asset");
            var lastRunMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/LastRun.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var vincentHandoff = AssetDatabase.LoadAssetAtPath<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/VincentHandoff.asset");

            EnsureFolder("Assets/Game/Materials");
            var officeFloor = GetOrCreateMaterial("Assets/Game/Materials/OfficeFloor.mat", new Color(0.17f, 0.15f, 0.13f), 0.34f, 0f);
            var officeWall = GetOrCreateMaterial("Assets/Game/Materials/OfficeWall.mat", new Color(0.27f, 0.22f, 0.18f), 0.08f, 0f);
            var officeTrim = GetOrCreateMaterial("Assets/Game/Materials/OfficeTrim.mat", new Color(0.56f, 0.43f, 0.26f), 0.26f, 0f);
            var ledgerAccent = GetOrCreateMaterial("Assets/Game/Materials/LedgerAccent.mat", new Color(0.34f, 0.47f, 0.19f), 0.22f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.20f, 0.13f, 0.08f), 0.24f, 0f);
            var wallpaper = GetOrCreateMaterial("Assets/Game/Materials/OfficeWallpaper.mat", new Color(0.24f, 0.2f, 0.15f), 0.05f, 0f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);
            var carpet = GetOrCreateMaterial("Assets/Game/Materials/OfficeCarpet.mat", new Color(0.32f, 0.08f, 0.07f), 0.14f, 0f);

            var light = new GameObject("InteriorLight");
            var lightComponent = light.AddComponent<Light>();
            lightComponent.type = LightType.Point;
            lightComponent.range = 24f;
            lightComponent.intensity = 8f;
            lightComponent.color = new Color(1f, 0.86f, 0.68f);
            light.transform.position = new Vector3(0f, 4f, -1f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "OfficeFloor";
            floor.transform.localScale = new Vector3(2.2f, 1f, 2.2f);
            AssignMaterial(floor, officeFloor);

            CreatePrimitive(
                PrimitiveType.Cube,
                "Ceiling",
                new Vector3(0f, 5.2f, 0f),
                new Vector3(10f, 0.25f, 17f),
                officeTrim);

            CreatePrimitive(
                PrimitiveType.Cube,
                "BackWall",
                new Vector3(0f, 2.5f, 8.5f),
                new Vector3(10f, 5f, 0.5f),
                officeWall);

            CreatePrimitive(
                PrimitiveType.Cube,
                "WallpaperPanel",
                new Vector3(0f, 2.7f, 8.15f),
                new Vector3(8.2f, 3.8f, 0.08f),
                wallpaper);

            CreatePrimitive(
                PrimitiveType.Cube,
                "FrontWall",
                new Vector3(0f, 2.5f, -8.5f),
                new Vector3(10f, 5f, 0.5f),
                officeWall);

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeftWall",
                new Vector3(-8.5f, 2.5f, 0f),
                new Vector3(0.5f, 5f, 17f),
                officeWall);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RightWall",
                new Vector3(8.5f, 2.5f, 0f),
                new Vector3(0.5f, 5f, 17f),
                officeWall);

            CreatePrimitive(
                PrimitiveType.Cube,
                "WindowStrip",
                new Vector3(-8.2f, 3.2f, 0f),
                new Vector3(0.15f, 1.3f, 6f),
                ledgerAccent);

            CreatePrimitive(
                PrimitiveType.Cube,
                "OfficeRug",
                new Vector3(0f, 0.08f, 1.2f),
                new Vector3(5.8f, 0.02f, 6.2f),
                carpet);

            var player = CreateNoirActor("InteriorPlayer", new Vector3(0f, 1f, -5.5f), officeTrim, null, false);
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            var playerController = player.AddComponent<TopDownPlayerController>();
            var interactionController = player.AddComponent<PlayerInteractionController>();
            var playerHealth = player.AddComponent<CombatHealth>();
            var playerCombatController = player.AddComponent<PlayerCombatController>();
            SetObjectReference(playerController, "characterController", characterController);
            SetObjectReference(interactionController, "playerController", playerController);
            SetStringValue(playerHealth, "displayName", "Tommy");
            SetIntValue(playerHealth, "maximumHealth", 3);
            SetBoolValue(playerHealth, "destroyOnDeath", false);
            SetBoolValue(playerHealth, "reloadSceneOnDeath", true);

            CreateSpawnPoint("InteriorDefaultSpawnPoint", "DefaultSpawn", new Vector3(0f, 1f, -5.5f));
            CreateSpawnPoint("InteriorSpawnPoint", "InteriorSpawn", new Vector3(0f, 1f, -5.5f));
            CreateSpawnPoint("LedgerDeskSpawnPoint", "LedgerDeskSpawn", new Vector3(0f, 1f, -0.8f));
            CreateSpawnPoint("BackOfficeExitSpawnPoint", "BackOfficeExitSpawn", new Vector3(0f, 1f, -4.4f));
            CreateSpawnPoint("BackOfficeFinalExitSpawnPoint", "BackOfficeFinalExitSpawn", new Vector3(-2.2f, 1f, -4.4f));

            var campaignSystems = new GameObject("CampaignSystems");
            var saveGameFileService = campaignSystems.AddComponent<SaveGameFileService>();
            SetStringValue(saveGameFileService, "defaultSceneName", "Interior_BackOffice_01");
            SetStringValue(saveGameFileService, "defaultSpawnPointId", "InteriorSpawn");
            var heatSystemController = campaignSystems.AddComponent<HeatSystemController>();
            SetObjectReference(heatSystemController, "saveGameFileService", saveGameFileService);
            var campaignProgressionController = campaignSystems.AddComponent<CampaignProgressionController>();
            SetObjectReference(campaignProgressionController, "campaignDatabase", campaignDatabase);
            SetObjectReference(campaignProgressionController, "saveGameFileService", saveGameFileService);
            var sceneBootstrapper = campaignSystems.AddComponent<CampaignSceneBootstrapper>();
            SetObjectReference(sceneBootstrapper, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(sceneBootstrapper, "bootstrapChapter", chapterAsset);
            SetObjectReference(sceneBootstrapper, "bootstrapMission", missionAsset);
            var sceneSpawnController = campaignSystems.AddComponent<SceneSpawnController>();
            SetObjectReference(sceneSpawnController, "saveGameFileService", saveGameFileService);
            SetObjectReference(sceneSpawnController, "playerController", playerController);
            SetStringValue(sceneSpawnController, "defaultSpawnPointId", "InteriorSpawn");
            var dialogueController = campaignSystems.AddComponent<DialogueController>();
            SetObjectReference(dialogueController, "playerController", playerController);

            var cameraRoot = new GameObject("InteriorCameraRig");
            var camera = cameraRoot.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.11f, 0.1f, 0.09f);
            camera.orthographic = false;
            camera.fieldOfView = 46f;
            cameraRoot.transform.position = player.transform.position + new Vector3(0f, 10.8f, -6.8f);
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            ConfigureNoirCamera(camera, topDownCamera, new Vector3(0f, 10.8f, -6.8f), 60f, 0f, 8.5f, 15f);
            CreateNoirPostProcessVolume("Interior_BackOffice_01", new Color(1f, 0.78f, 0.55f), -0.28f, 36f, -12f, 1.6f, 0.4f);

            var transitionRoot = new GameObject("SceneTransitionController");
            var transitionController = transitionRoot.AddComponent<SceneTransitionController>();
            SetObjectReference(transitionController, "saveGameFileService", saveGameFileService);

            var missionSystem = CreateSceneObjectiveSystem("InteriorMissionSystem");

            var hudObject = new GameObject("InteriorObjectiveHud");
            var hud = hudObject.AddComponent<ObjectiveHudController>();
            SetObjectReference(hud, "objectiveSystem", missionSystem);
            SetObjectReference(hud, "saveGameFileService", saveGameFileService);
            SetObjectReference(hud, "campaignDatabase", campaignDatabase);
            var statusHudObject = new GameObject("InteriorStatusHud");
            var statusHud = statusHudObject.AddComponent<StatusHudController>();
            SetObjectReference(statusHud, "playerHealth", playerHealth);
            SetObjectReference(statusHud, "heatSystemController", heatSystemController);
            var pauseMenuObject = new GameObject("InteriorPauseMenu");
            var pauseMenu = pauseMenuObject.AddComponent<PauseMenuController>();
            SetObjectReference(pauseMenu, "playerController", playerController);
            SetObjectReference(pauseMenu, "dialogueController", dialogueController);

            var desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            desk.name = "LedgerDesk";
            desk.transform.position = new Vector3(0f, 1f, 0f);
            desk.transform.localScale = new Vector3(3.5f, 2f, 1.8f);
            AssignMaterial(desk, officeTrim);

            CreatePrimitive(
                PrimitiveType.Cube,
                "DeskLampStem",
                new Vector3(1.2f, 1.95f, -0.15f),
                new Vector3(0.12f, 0.7f, 0.12f),
                brass);

            CreatePrimitive(
                PrimitiveType.Cube,
                "DeskLampShade",
                new Vector3(1.2f, 2.45f, -0.05f),
                new Vector3(0.55f, 0.22f, 0.55f),
                ledgerAccent);

            var deskLampLight = new GameObject("DeskLampLight");
            deskLampLight.transform.position = new Vector3(1.2f, 2.2f, 0f);
            var deskLampPoint = deskLampLight.AddComponent<Light>();
            deskLampPoint.type = LightType.Point;
            deskLampPoint.range = 5.5f;
            deskLampPoint.intensity = 1.7f;
            deskLampPoint.color = new Color(1f, 0.82f, 0.58f);

            CreatePrimitive(
                PrimitiveType.Cube,
                "BackShelf",
                new Vector3(4.8f, 2f, 4.2f),
                new Vector3(1.2f, 3.4f, 2.2f),
                crateWood);

            CreatePrimitive(
                PrimitiveType.Cube,
                "VisitorChairLeft",
                new Vector3(-2.2f, 0.6f, 2.5f),
                new Vector3(0.8f, 1.2f, 0.8f),
                officeWall);

            CreatePrimitive(
                PrimitiveType.Cube,
                "VisitorChairRight",
                new Vector3(2.2f, 0.6f, 2.5f),
                new Vector3(0.8f, 1.2f, 0.8f),
                officeWall);

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeatherSofa",
                new Vector3(-4.9f, 0.95f, -1.6f),
                new Vector3(2.3f, 1.6f, 1f),
                officeTrim);

            CreatePrimitive(
                PrimitiveType.Cube,
                "CoffeeTable",
                new Vector3(-4.9f, 0.55f, 0.7f),
                new Vector3(1.7f, 0.35f, 0.9f),
                crateWood);

            CreatePrimitive(
                PrimitiveType.Cube,
                "PortraitFrame",
                new Vector3(4.2f, 3.2f, 8.12f),
                new Vector3(1.8f, 1.2f, 0.08f),
                brass);

            var backOfficeMissionRoot = new GameObject("AQuietFavorInteriorRoot");

            var backOfficeGuard = CreateEnemyGuard(
                "BackOfficeGuard",
                new Vector3(4.8f, 1f, 1.6f),
                enemyCoat,
                playerController,
                playerHealth,
                dialogueController,
                backOfficeMissionRoot.transform);

            var vincent = CreateNoirActor("VincentContact", new Vector3(0f, 1f, 3.7f), contactCoat, backOfficeMissionRoot.transform);
            var vincentInteractable = vincent.AddComponent<DialogueInteractable>();
            SetObjectReference(vincentInteractable, "dialogueController", dialogueController);
            SetObjectReference(vincentInteractable, "dialogueSequence", vincentHandoff);
            SetObjectReference(vincentInteractable, "objectiveSystem", missionSystem);
            SetStringValue(vincentInteractable, "promptText", "Talk to Vincent");
            SetStringValue(vincentInteractable, "requiredObjectiveId", "objective-handoff");
            SetObjectReference(vincentInteractable, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(vincentInteractable, "missionAsset", missionAsset);
            SetStringValue(vincentInteractable, "missionStageIdToComplete", "handoff");

            var ledgerDrop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ledgerDrop.name = "LedgerDrop";
            ledgerDrop.transform.position = new Vector3(0f, 1f, 1.8f);
            ledgerDrop.transform.localScale = new Vector3(1f, 1f, 1f);
            AssignMaterial(ledgerDrop, ledgerAccent);
            SetObjectReference(playerCombatController, "playerController", playerController);
            SetObjectReference(playerCombatController, "combatHealth", playerHealth);
            SetObjectReference(playerCombatController, "heatSystemController", heatSystemController);

            var exitDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            exitDoor.name = "ExitDoor";
            exitDoor.transform.SetParent(backOfficeMissionRoot.transform, false);
            exitDoor.transform.position = new Vector3(0f, 1f, 6f);
            exitDoor.transform.localScale = new Vector3(2f, 2f, 0.5f);
            AssignMaterial(exitDoor, officeTrim);
            var exitInteractable = exitDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(exitInteractable, "sceneTransitionController", transitionController);
            SetStringValue(exitInteractable, "promptText", "Leave the office");
            SetStringValue(exitInteractable, "sourceScene", "Interior_BackOffice_01");
            SetStringValue(exitInteractable, "targetScene", "District_01");
            SetStringValue(exitInteractable, "spawnPointId", "ExteriorReturn");
            SetObjectReference(exitInteractable, "objectiveSystem", missionSystem);
            SetStringValue(exitInteractable, "requiredObjectiveId", "objective-leave");
            SetObjectReference(exitInteractable, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(exitInteractable, "missionAsset", missionAsset);
            SetStringValue(exitInteractable, "missionStageIdToComplete", "leave");

            var bloodLedgerRoot = CreateObjectiveActionMissionRoot(
                "BloodLedgerMissionRoot",
                "BloodLedgerMarker",
                new Vector3(0.4f, 1.1f, 1.6f),
                new Vector3(1.1f, 1.1f, 1.1f),
                ledgerAccent,
                "Read the marked ledger",
                missionSystem,
                "read-ledger",
                campaignProgressionController,
                bloodLedgerMission,
                "read-ledger");
            CreateMissionExitDoor(
                bloodLedgerRoot.transform,
                "BloodLedgerExitDoor",
                new Vector3(-2.2f, 1f, 6f),
                new Vector3(1.4f, 2f, 0.45f),
                officeTrim,
                "Leave the office quietly",
                "Interior_BackOffice_01",
                "District_BusinessCore_01",
                "BusinessCoreStart",
                "leave-office",
                transitionController,
                missionSystem,
                campaignProgressionController,
                bloodLedgerMission,
                "leave-office");

            var lastRunRoot = CreateObjectiveActionMissionRoot(
                "LastRunMissionRoot",
                "FinalOrdersMarker",
                new Vector3(-0.8f, 1.1f, 3.4f),
                new Vector3(1.1f, 1.1f, 1.1f),
                brass,
                "Hear Vincent's orders",
                missionSystem,
                "hear-orders",
                campaignProgressionController,
                lastRunMission,
                "hear-orders");
            CreateMissionExitDoor(
                lastRunRoot.transform,
                "LastRunExitDoor",
                new Vector3(2.2f, 1f, 6f),
                new Vector3(1.4f, 2f, 0.45f),
                officeTrim,
                "Step back into the harbor night",
                "Interior_BackOffice_01",
                "District_01",
                "ExteriorReturn",
                "leave-for-harbor",
                transitionController,
                missionSystem,
                campaignProgressionController,
                lastRunMission,
                "leave-for-harbor");

            var interiorMissionDirectorObject = new GameObject("InteriorMissionDirector");
            var interiorMissionDirector = interiorMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                interiorMissionDirector,
                saveGameFileService,
                missionSystem,
                (missionAsset, backOfficeMissionRoot, new[]
                {
                    ("objective-handoff", "Hand over the ledger", "Deliver the ledger to Vincent.", "objective-leave"),
                    ("objective-leave", "Leave the office", "Get out clean and head to the next job.", null)
                }),
                (bloodLedgerMission, bloodLedgerRoot, new[]
                {
                    ("read-ledger", "Read the ledger", "Review the marked ledger before anyone notices.", "leave-office"),
                    ("leave-office", "Leave the office", "Walk out with the new names in your head.", null)
                }),
                (lastRunMission, lastRunRoot, new[]
                {
                    ("hear-orders", "Hear the orders", "Take Vincent's final order in the back office.", "leave-for-harbor"),
                    ("leave-for-harbor", "Leave for the harbor", "Step out knowing there is no clean road left.", null)
                }));

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildBusinessInteriorScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/Interior_BusinessBookkeeper_01.unity");
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var bookkeeperDialogue = AssetDatabase.LoadAssetAtPath<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/BookkeeperWarning.asset");
            var belloriBooksMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/BelloriBooks.asset");

            var officeFloor = GetOrCreateMaterial("Assets/Game/Materials/OfficeFloor.mat", new Color(0.17f, 0.15f, 0.13f), 0.34f, 0f);
            var officeWall = GetOrCreateMaterial("Assets/Game/Materials/OfficeWall.mat", new Color(0.27f, 0.22f, 0.18f), 0.08f, 0f);
            var officeTrim = GetOrCreateMaterial("Assets/Game/Materials/OfficeTrim.mat", new Color(0.56f, 0.43f, 0.26f), 0.26f, 0f);
            var wallpaper = GetOrCreateMaterial("Assets/Game/Materials/OfficeWallpaper.mat", new Color(0.24f, 0.2f, 0.15f), 0.05f, 0f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);
            var ledgerAccent = GetOrCreateMaterial("Assets/Game/Materials/LedgerAccent.mat", new Color(0.34f, 0.47f, 0.19f), 0.22f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.20f, 0.13f, 0.08f), 0.24f, 0f);

            var runtime = CreateFreeRoamInteriorRuntime(
                "Interior_BusinessBookkeeper_01",
                "BusinessInteriorSpawn",
                new Vector3(0f, 1f, -4.8f),
                new Color(0.1f, 0.09f, 0.08f),
                new Vector3(0f, 13f, -8.5f),
                campaignDatabase,
                chapterAsset);
            var missionSystem = CreateSceneObjectiveSystem("BusinessInteriorMissionSystem");
            CreateSceneObjectiveHud("BusinessInteriorObjectiveHud", missionSystem, runtime.SaveGameFileService, campaignDatabase);

            CreateSpawnPoint("BusinessInteriorDefaultSpawnPoint", "DefaultSpawn", new Vector3(0f, 1f, -4.8f));
            CreateSpawnPoint("BusinessInteriorSpawnPoint", "BusinessInteriorSpawn", new Vector3(0f, 1f, -4.8f));
            CreateSpawnPoint("BusinessBookstoreExitSpawnPoint", "BusinessBookstoreExitSpawn", new Vector3(0f, 1f, -4.2f));

            var light = new GameObject("BusinessInteriorLight");
            var pointLight = light.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 18f;
            pointLight.intensity = 7f;
            pointLight.color = new Color(1f, 0.86f, 0.68f);
            light.transform.position = new Vector3(0f, 4.2f, 0f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "BusinessInteriorFloor";
            floor.transform.localScale = new Vector3(1.7f, 1f, 1.5f);
            AssignMaterial(floor, officeFloor);

            CreatePrimitive(PrimitiveType.Cube, "BusinessCeiling", new Vector3(0f, 5.2f, 0f), new Vector3(7.8f, 0.25f, 7f), officeTrim);
            CreatePrimitive(PrimitiveType.Cube, "BusinessBackWall", new Vector3(0f, 2.5f, 7f), new Vector3(7.8f, 5f, 0.45f), officeWall);
            CreatePrimitive(PrimitiveType.Cube, "BusinessFrontWall", new Vector3(0f, 2.5f, -7f), new Vector3(7.8f, 5f, 0.45f), officeWall);
            CreatePrimitive(PrimitiveType.Cube, "BusinessLeftWall", new Vector3(-6.8f, 2.5f, 0f), new Vector3(0.45f, 5f, 14f), officeWall);
            CreatePrimitive(PrimitiveType.Cube, "BusinessRightWall", new Vector3(6.8f, 2.5f, 0f), new Vector3(0.45f, 5f, 14f), officeWall);
            CreatePrimitive(PrimitiveType.Cube, "BusinessWallpaperPanel", new Vector3(0f, 2.7f, 6.65f), new Vector3(6.1f, 3.7f, 0.08f), wallpaper);
            CreatePrimitive(PrimitiveType.Cube, "BusinessLedgerDesk", new Vector3(0f, 1f, 1.8f), new Vector3(3.3f, 1.8f, 1.5f), officeTrim);
            CreatePrimitive(PrimitiveType.Cube, "BusinessBackShelves", new Vector3(4.8f, 2f, 2.6f), new Vector3(1.2f, 3.3f, 5.2f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "BusinessVisitorChair", new Vector3(-2.2f, 0.6f, -0.5f), new Vector3(0.9f, 1.1f, 0.9f), officeWall);
            CreatePrimitive(PrimitiveType.Cube, "BusinessVisitorChairTwo", new Vector3(2.2f, 0.6f, -0.5f), new Vector3(0.9f, 1.1f, 0.9f), officeWall);
            CreatePrimitive(PrimitiveType.Cube, "BusinessLedgers", new Vector3(0.7f, 2f, 1.9f), new Vector3(0.85f, 0.22f, 0.55f), ledgerAccent);
            CreatePrimitive(PrimitiveType.Cylinder, "BusinessChandelier", new Vector3(0f, 4.3f, -0.6f), new Vector3(0.35f, 0.1f, 0.35f), brass);

            var bookkeeper = CreateNoirActor("NicoBelloriInterior", new Vector3(0f, 1f, 3.9f), contactCoat);
            var bookkeeperInteractable = bookkeeper.AddComponent<DialogueInteractable>();
            SetObjectReference(bookkeeperInteractable, "dialogueController", runtime.DialogueController);
            SetObjectReference(bookkeeperInteractable, "dialogueSequence", bookkeeperDialogue);
            SetStringValue(bookkeeperInteractable, "promptText", "Talk to Nico");

            var belloriBooksRoot = CreateObjectiveActionMissionRoot(
                "BelloriBooksMissionRoot",
                "BelloriLedgerShelf",
                new Vector3(4.6f, 1.1f, 2.4f),
                new Vector3(1.1f, 1.1f, 1.1f),
                ledgerAccent,
                "Inspect the hidden ledgers",
                missionSystem,
                "inspect-ledgers",
                runtime.CampaignProgressionController,
                belloriBooksMission,
                "inspect-ledgers");
            CreateMissionExitDoor(
                belloriBooksRoot.transform,
                "BelloriBooksExitDoor",
                new Vector3(0f, 1f, -5.8f),
                new Vector3(1.8f, 2.2f, 0.45f),
                officeTrim,
                "Leave Bellori Books",
                "Interior_BusinessBookkeeper_01",
                "Interior_OldQuarter_Chapel_01",
                "ChapelInteriorSpawn",
                "leave-bookstore",
                runtime.SceneTransitionController,
                missionSystem,
                runtime.CampaignProgressionController,
                belloriBooksMission,
                "leave-bookstore");

            var exitDoor = CreatePrimitive(
                PrimitiveType.Cube,
                "BusinessInteriorExitDoor",
                new Vector3(0f, 1f, -5.8f),
                new Vector3(1.8f, 2.2f, 0.45f),
                officeTrim);
            var exitInteractable = exitDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(exitInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(exitInteractable, "promptText", "Back to the square");
            SetStringValue(exitInteractable, "sourceScene", "Interior_BusinessBookkeeper_01");
            SetStringValue(exitInteractable, "targetScene", "District_BusinessCore_01");
            SetStringValue(exitInteractable, "spawnPointId", "FromBookkeeperInterior");

            var businessInteriorMissionDirectorObject = new GameObject("BusinessInteriorMissionDirector");
            var businessInteriorMissionDirector = businessInteriorMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                businessInteriorMissionDirector,
                runtime.SaveGameFileService,
                missionSystem,
                (belloriBooksMission, belloriBooksRoot, new[]
                {
                    ("inspect-ledgers", "Inspect the ledgers", "Search Nico's hidden books for the missing names.", "leave-bookstore"),
                    ("leave-bookstore", "Leave the bookstore", "Get back out before the pages disappear.", null)
                }));

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildOldQuarterInteriorScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/Interior_OldQuarter_Chapel_01.unity");
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var saintVeraSilenceMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/SaintVeraSilence.asset");

            var churchStone = GetOrCreateMaterial("Assets/Game/Materials/ChurchStone.mat", new Color(0.36f, 0.34f, 0.29f), 0.18f, 0f);
            var officeTrim = GetOrCreateMaterial("Assets/Game/Materials/OfficeTrim.mat", new Color(0.56f, 0.43f, 0.26f), 0.26f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var laundry = GetOrCreateMaterial("Assets/Game/Materials/LaundryCloth.mat", new Color(0.67f, 0.63f, 0.58f), 0.12f, 0f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);
            var ledgerAccent = GetOrCreateMaterial("Assets/Game/Materials/LedgerAccent.mat", new Color(0.34f, 0.47f, 0.19f), 0.22f, 0f);

            var runtime = CreateFreeRoamInteriorRuntime(
                "Interior_OldQuarter_Chapel_01",
                "ChapelInteriorSpawn",
                new Vector3(0f, 1f, -5.2f),
                new Color(0.09f, 0.08f, 0.07f),
                new Vector3(0f, 13f, -8.2f),
                campaignDatabase,
                chapterAsset);
            var missionSystem = CreateSceneObjectiveSystem("ChapelInteriorMissionSystem");
            CreateSceneObjectiveHud("ChapelInteriorObjectiveHud", missionSystem, runtime.SaveGameFileService, campaignDatabase);

            CreateSpawnPoint("ChapelInteriorDefaultSpawnPoint", "DefaultSpawn", new Vector3(0f, 1f, -5.2f));
            CreateSpawnPoint("ChapelInteriorSpawnPoint", "ChapelInteriorSpawn", new Vector3(0f, 1f, -5.2f));
            CreateSpawnPoint("ChapelExitSpawnPoint", "ChapelExitSpawn", new Vector3(0f, 1f, -4.4f));

            var light = new GameObject("ChapelInteriorLight");
            var pointLight = light.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 20f;
            pointLight.intensity = 6.5f;
            pointLight.color = new Color(1f, 0.84f, 0.62f);
            light.transform.position = new Vector3(0f, 4.4f, 0f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "ChapelInteriorFloor";
            floor.transform.localScale = new Vector3(1.8f, 1f, 1.8f);
            AssignMaterial(floor, churchStone);

            CreatePrimitive(PrimitiveType.Cube, "ChapelCeiling", new Vector3(0f, 5.5f, 0f), new Vector3(8.5f, 0.25f, 9f), officeTrim);
            CreatePrimitive(PrimitiveType.Cube, "ChapelBackWall", new Vector3(0f, 2.7f, 8.5f), new Vector3(8.5f, 5.4f, 0.45f), churchStone);
            CreatePrimitive(PrimitiveType.Cube, "ChapelFrontWall", new Vector3(0f, 2.7f, -8.5f), new Vector3(8.5f, 5.4f, 0.45f), churchStone);
            CreatePrimitive(PrimitiveType.Cube, "ChapelLeftWall", new Vector3(-8.1f, 2.7f, 0f), new Vector3(0.45f, 5.4f, 17f), churchStone);
            CreatePrimitive(PrimitiveType.Cube, "ChapelRightWall", new Vector3(8.1f, 2.7f, 0f), new Vector3(0.45f, 5.4f, 17f), churchStone);
            CreatePrimitive(PrimitiveType.Cube, "ChapelAltar", new Vector3(0f, 1.1f, 5.8f), new Vector3(2.4f, 2f, 1.1f), officeTrim);
            CreatePrimitive(PrimitiveType.Cube, "ChapelCloth", new Vector3(0f, 2.15f, 5.4f), new Vector3(2.5f, 0.12f, 0.8f), laundry);
            CreatePrimitive(PrimitiveType.Cube, "PewLeftA", new Vector3(-2.2f, 0.7f, 1.5f), new Vector3(1.8f, 1f, 0.8f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "PewRightA", new Vector3(2.2f, 0.7f, 1.5f), new Vector3(1.8f, 1f, 0.8f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "PewLeftB", new Vector3(-2.2f, 0.7f, -1.4f), new Vector3(1.8f, 1f, 0.8f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "PewRightB", new Vector3(2.2f, 0.7f, -1.4f), new Vector3(1.8f, 1f, 0.8f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "Confessional", new Vector3(-5.4f, 1.6f, 4.4f), new Vector3(1.5f, 2.8f, 1.7f), crateWood);
            CreatePrimitive(PrimitiveType.Cylinder, "CandleStand", new Vector3(4.8f, 1f, 4.8f), new Vector3(0.35f, 0.9f, 0.35f), brass);
            CreatePrimitive(PrimitiveType.Cube, "PrayerCandles", new Vector3(4.8f, 1.9f, 4.8f), new Vector3(0.7f, 0.15f, 0.7f), ledgerAccent);

            var saintVeraSilenceRoot = CreateObjectiveActionMissionRoot(
                "SaintVeraSilenceMissionRoot",
                "VestryMarker",
                new Vector3(-5.4f, 1.1f, 4.4f),
                new Vector3(1.1f, 1.1f, 1.1f),
                ledgerAccent,
                "Search the vestry",
                missionSystem,
                "search-vestry",
                runtime.CampaignProgressionController,
                saintVeraSilenceMission,
                "search-vestry");
            CreateMissionExitDoor(
                saintVeraSilenceRoot.transform,
                "SaintVeraSilenceExitDoor",
                new Vector3(0f, 1f, -6.8f),
                new Vector3(1.8f, 2.2f, 0.45f),
                officeTrim,
                "Leave the chapel",
                "Interior_OldQuarter_Chapel_01",
                "District_01",
                "ExteriorReturn",
                "leave-chapel",
                runtime.SceneTransitionController,
                missionSystem,
                runtime.CampaignProgressionController,
                saintVeraSilenceMission,
                "leave-chapel");

            var exitDoor = CreatePrimitive(
                PrimitiveType.Cube,
                "ChapelInteriorExitDoor",
                new Vector3(0f, 1f, -6.8f),
                new Vector3(1.8f, 2.2f, 0.45f),
                officeTrim);
            var exitInteractable = exitDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(exitInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(exitInteractable, "promptText", "Back to Saint Vera Yard");
            SetStringValue(exitInteractable, "sourceScene", "Interior_OldQuarter_Chapel_01");
            SetStringValue(exitInteractable, "targetScene", "District_OldQuarter_01");
            SetStringValue(exitInteractable, "spawnPointId", "FromChapelInterior");

            var chapelInteriorMissionDirectorObject = new GameObject("ChapelInteriorMissionDirector");
            var chapelInteriorMissionDirector = chapelInteriorMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                chapelInteriorMissionDirector,
                runtime.SaveGameFileService,
                missionSystem,
                (saintVeraSilenceMission, saintVeraSilenceRoot, new[]
                {
                    ("search-vestry", "Search the vestry", "Find the priest's side-book before the chapel wakes up.", "leave-chapel"),
                    ("leave-chapel", "Leave the chapel", "Slip back out before the bells start talking.", null)
                }));

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildRailYardInteriorScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/Interior_RailYard_Garage_01.unity");
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var ironlineLedgerMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/IronlineLedger.asset");

            var officeFloor = GetOrCreateMaterial("Assets/Game/Materials/OfficeFloor.mat", new Color(0.17f, 0.15f, 0.13f), 0.34f, 0f);
            var rustMetal = GetOrCreateMaterial("Assets/Game/Materials/RustMetal.mat", new Color(0.34f, 0.22f, 0.17f), 0.58f, 0.45f);
            var tankMetal = GetOrCreateMaterial("Assets/Game/Materials/TankMetal.mat", new Color(0.22f, 0.26f, 0.28f), 0.72f, 0.62f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var ledgerAccent = GetOrCreateMaterial("Assets/Game/Materials/LedgerAccent.mat", new Color(0.34f, 0.47f, 0.19f), 0.22f, 0f);

            var runtime = CreateFreeRoamInteriorRuntime(
                "Interior_RailYard_Garage_01",
                "GarageInteriorSpawn",
                new Vector3(0f, 1f, -5.2f),
                new Color(0.08f, 0.09f, 0.1f),
                new Vector3(0f, 13f, -8.5f),
                campaignDatabase,
                chapterAsset);
            var missionSystem = CreateSceneObjectiveSystem("GarageInteriorMissionSystem");
            CreateSceneObjectiveHud("GarageInteriorObjectiveHud", missionSystem, runtime.SaveGameFileService, campaignDatabase);

            CreateSpawnPoint("GarageInteriorDefaultSpawnPoint", "DefaultSpawn", new Vector3(0f, 1f, -5.2f));
            CreateSpawnPoint("GarageInteriorSpawnPoint", "GarageInteriorSpawn", new Vector3(0f, 1f, -5.2f));
            CreateSpawnPoint("GarageExitSpawnPoint", "GarageExitSpawn", new Vector3(0f, 1f, -4.4f));

            var light = new GameObject("GarageInteriorLight");
            var pointLight = light.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 20f;
            pointLight.intensity = 6.2f;
            pointLight.color = new Color(1f, 0.8f, 0.58f);
            light.transform.position = new Vector3(0f, 4.6f, 0f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "GarageInteriorFloor";
            floor.transform.localScale = new Vector3(1.9f, 1f, 1.8f);
            AssignMaterial(floor, officeFloor);

            CreatePrimitive(PrimitiveType.Cube, "GarageCeiling", new Vector3(0f, 5.6f, 0f), new Vector3(9f, 0.25f, 8.6f), metal);
            CreatePrimitive(PrimitiveType.Cube, "GarageBackWall", new Vector3(0f, 2.8f, 8f), new Vector3(9f, 5.4f, 0.45f), rustMetal);
            CreatePrimitive(PrimitiveType.Cube, "GarageFrontWall", new Vector3(0f, 2.8f, -8f), new Vector3(9f, 5.4f, 0.45f), rustMetal);
            CreatePrimitive(PrimitiveType.Cube, "GarageLeftWall", new Vector3(-8.6f, 2.8f, 0f), new Vector3(0.45f, 5.4f, 16f), rustMetal);
            CreatePrimitive(PrimitiveType.Cube, "GarageRightWall", new Vector3(8.6f, 2.8f, 0f), new Vector3(0.45f, 5.4f, 16f), rustMetal);
            CreatePrimitive(PrimitiveType.Cube, "GarageDesk", new Vector3(4.8f, 1f, 2.4f), new Vector3(2.4f, 1.8f, 1.3f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "GarageToolWall", new Vector3(-5.4f, 2f, 3f), new Vector3(1.2f, 3.2f, 5.8f), tankMetal);
            CreatePrimitive(PrimitiveType.Cube, "GaragePartsCrates", new Vector3(-2.2f, 0.8f, -1.6f), new Vector3(2.8f, 1.2f, 1.5f), crateWood);
            CreatePrimitive(PrimitiveType.Cube, "GarageBlueprints", new Vector3(5.2f, 2f, 2.4f), new Vector3(0.9f, 0.12f, 0.7f), ledgerAccent);
            CreatePrimitive(PrimitiveType.Cube, "GarageLocker", new Vector3(6.8f, 1.8f, -3.2f), new Vector3(1.1f, 3.2f, 1.1f), tankMetal);

            var ironlineLedgerRoot = CreateObjectiveActionMissionRoot(
                "IronlineLedgerMissionRoot",
                "GarageLedgerMarker",
                new Vector3(5.2f, 1.1f, 2.4f),
                new Vector3(1.1f, 1.1f, 1.1f),
                ledgerAccent,
                "Inspect the routing ledger",
                missionSystem,
                "inspect-garage-ledger",
                runtime.CampaignProgressionController,
                ironlineLedgerMission,
                "inspect-garage-ledger");
            CreateMissionExitDoor(
                ironlineLedgerRoot.transform,
                "IronlineLedgerExitDoor",
                new Vector3(0f, 1f, -6.8f),
                new Vector3(1.8f, 2.2f, 0.45f),
                metal,
                "Leave the garage office",
                "Interior_RailYard_Garage_01",
                "Interior_BackOffice_01",
                "LedgerDeskSpawn",
                "leave-garage",
                runtime.SceneTransitionController,
                missionSystem,
                runtime.CampaignProgressionController,
                ironlineLedgerMission,
                "leave-garage");

            var exitDoor = CreatePrimitive(
                PrimitiveType.Cube,
                "GarageInteriorExitDoor",
                new Vector3(0f, 1f, -6.8f),
                new Vector3(1.8f, 2.2f, 0.45f),
                metal);
            var exitInteractable = exitDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(exitInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(exitInteractable, "promptText", "Back to the yard");
            SetStringValue(exitInteractable, "sourceScene", "Interior_RailYard_Garage_01");
            SetStringValue(exitInteractable, "targetScene", "District_RailYard_01");
            SetStringValue(exitInteractable, "spawnPointId", "FromGarageInterior");

            var garageInteriorMissionDirectorObject = new GameObject("GarageInteriorMissionDirector");
            var garageInteriorMissionDirector = garageInteriorMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                garageInteriorMissionDirector,
                runtime.SaveGameFileService,
                missionSystem,
                (ironlineLedgerMission, ironlineLedgerRoot, new[]
                {
                    ("inspect-garage-ledger", "Inspect the garage ledger", "Read the routing ledger before the dawn crews arrive.", "leave-garage"),
                    ("leave-garage", "Leave the garage", "Get the numbers out before the office wakes up.", null)
                }));

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildDocksWarehouseInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_Docks_Warehouse_01.unity",
                "Interior_Docks_Warehouse_01",
                "WarehouseInteriorSpawn",
                "District_01",
                "FromWarehouseInterior",
                "Leave the cold-storage warehouse",
                "Warehouse",
                new Color(0.07f, 0.085f, 0.095f),
                new Color(0.12f, 0.14f, 0.14f),
                new Color(0.19f, 0.22f, 0.22f),
                new Color(0.34f, 0.24f, 0.16f),
                new Color(0.63f, 0.47f, 0.21f));
        }

        private static void BuildBusinessPrintShopInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_Business_PrintShop_01.unity",
                "Interior_Business_PrintShop_01",
                "PrintShopInteriorSpawn",
                "District_BusinessCore_01",
                "FromPrintShopInterior",
                "Leave the union print shop",
                "PrintShop",
                new Color(0.09f, 0.085f, 0.075f),
                new Color(0.16f, 0.135f, 0.115f),
                new Color(0.28f, 0.24f, 0.2f),
                new Color(0.48f, 0.36f, 0.2f),
                new Color(0.16f, 0.18f, 0.2f));
        }

        private static void BuildOldQuarterTenementInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_OldQuarter_Tenement_01.unity",
                "Interior_OldQuarter_Tenement_01",
                "TenementInteriorSpawn",
                "District_OldQuarter_01",
                "FromTenementInterior",
                "Leave the Sava tenement",
                "Tenement",
                new Color(0.105f, 0.085f, 0.075f),
                new Color(0.19f, 0.145f, 0.11f),
                new Color(0.29f, 0.22f, 0.17f),
                new Color(0.42f, 0.26f, 0.16f),
                new Color(0.72f, 0.58f, 0.38f));
        }

        private static void BuildRailYardDispatchInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_RailYard_Dispatch_01.unity",
                "Interior_RailYard_Dispatch_01",
                "DispatchInteriorSpawn",
                "District_RailYard_01",
                "FromDispatchInterior",
                "Leave the dispatch office",
                "Dispatch",
                new Color(0.07f, 0.08f, 0.09f),
                new Color(0.12f, 0.12f, 0.115f),
                new Color(0.18f, 0.2f, 0.2f),
                new Color(0.28f, 0.19f, 0.14f),
                new Color(0.82f, 0.58f, 0.24f));
        }

        private static void BuildDocksSocialClubInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_Docks_SocialClub_01.unity",
                "Interior_Docks_SocialClub_01",
                "SocialClubInteriorSpawn",
                "District_01",
                "FromSocialClubInterior",
                "Leave the harbor social club",
                "SocialClub",
                new Color(0.075f, 0.055f, 0.05f),
                new Color(0.16f, 0.105f, 0.08f),
                new Color(0.24f, 0.13f, 0.095f),
                new Color(0.42f, 0.25f, 0.13f),
                new Color(0.72f, 0.45f, 0.2f));
        }

        private static void BuildBusinessUnionHallInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_Business_UnionHall_01.unity",
                "Interior_Business_UnionHall_01",
                "UnionHallInteriorSpawn",
                "District_BusinessCore_01",
                "FromUnionHallInterior",
                "Leave the union hall",
                "UnionHall",
                new Color(0.08f, 0.075f, 0.07f),
                new Color(0.15f, 0.135f, 0.11f),
                new Color(0.24f, 0.21f, 0.17f),
                new Color(0.45f, 0.34f, 0.18f),
                new Color(0.18f, 0.2f, 0.22f));
        }

        private static void BuildOldQuarterDinerInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_OldQuarter_Diner_01.unity",
                "Interior_OldQuarter_Diner_01",
                "DinerInteriorSpawn",
                "District_OldQuarter_01",
                "FromDinerInterior",
                "Leave Cafe Sava",
                "Diner",
                new Color(0.09f, 0.07f, 0.06f),
                new Color(0.18f, 0.14f, 0.105f),
                new Color(0.28f, 0.19f, 0.15f),
                new Color(0.56f, 0.37f, 0.18f),
                new Color(0.74f, 0.58f, 0.38f));
        }

        private static void BuildRailYardLockerInteriorScene()
        {
            BuildSupplementalInteriorScene(
                "Assets/Game/Scenes/Interior_RailYard_LockerRoom_01.unity",
                "Interior_RailYard_LockerRoom_01",
                "LockerInteriorSpawn",
                "District_RailYard_01",
                "FromLockerInterior",
                "Leave the locker room",
                "LockerRoom",
                new Color(0.055f, 0.065f, 0.075f),
                new Color(0.105f, 0.11f, 0.105f),
                new Color(0.15f, 0.17f, 0.17f),
                new Color(0.26f, 0.2f, 0.14f),
                new Color(0.76f, 0.5f, 0.22f));
        }

        private static void BuildSupplementalInteriorScene(
            string scenePath,
            string sceneName,
            string spawnPointId,
            string exteriorSceneName,
            string exteriorSpawnPointId,
            string exitPrompt,
            string themeKey,
            Color backgroundColor,
            Color floorColor,
            Color wallColor,
            Color trimColor,
            Color accentColor)
        {
            var scene = OpenOrCreateScene(scenePath);
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");

            var floorMaterial = GetOrCreateMaterial("Assets/Game/Materials/" + themeKey + "InteriorFloor.mat", floorColor, 0.34f, 0f);
            var wallMaterial = GetOrCreateMaterial("Assets/Game/Materials/" + themeKey + "InteriorWall.mat", wallColor, 0.12f, 0f);
            var trimMaterial = GetOrCreateMaterial("Assets/Game/Materials/" + themeKey + "InteriorTrim.mat", trimColor, 0.28f, 0f);
            var accentMaterial = GetOrCreateMaterial("Assets/Game/Materials/" + themeKey + "InteriorAccent.mat", accentColor, 0.7f, 0.25f);
            var wood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var paper = GetOrCreateMaterial("Assets/Game/Materials/PaperStack.mat", new Color(0.72f, 0.67f, 0.55f), 0.08f, 0f);
            var glow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(1f, 0.78f, 0.42f), 0.86f, 0.08f, new Color(1f, 0.68f, 0.28f) * 3.2f);
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.20f, 0.13f, 0.08f), 0.24f, 0f);

            var runtime = CreateFreeRoamInteriorRuntime(
                sceneName,
                spawnPointId,
                new Vector3(0f, 1f, -5.25f),
                backgroundColor,
                new Vector3(0f, 12.4f, -8.2f),
                campaignDatabase,
                chapterAsset);

            CreateSpawnPoint(themeKey + "DefaultSpawnPoint", "DefaultSpawn", new Vector3(0f, 1f, -5.25f));
            CreateSpawnPoint(themeKey + "InteriorSpawnPoint", spawnPointId, new Vector3(0f, 1f, -5.25f));
            CreateSpawnPoint(themeKey + "ExitSpawnPoint", "ExitSpawn", new Vector3(0f, 1f, -4.55f));

            var keyLight = new GameObject(themeKey + "InteriorKeyLight");
            var pointLight = keyLight.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 20f;
            pointLight.intensity = 6.8f;
            pointLight.color = new Color(1f, 0.82f, 0.58f);
            keyLight.transform.position = new Vector3(0f, 4.4f, -0.2f);

            var fillLight = new GameObject(themeKey + "InteriorWindowFill");
            var fill = fillLight.AddComponent<Light>();
            fill.type = LightType.Point;
            fill.range = 11f;
            fill.intensity = 2.1f;
            fill.color = new Color(0.55f, 0.68f, 0.86f);
            fillLight.transform.position = new Vector3(-5.6f, 2.8f, 3.2f);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = themeKey + "Floor";
            floor.transform.localScale = new Vector3(2.1f, 1f, 1.8f);
            AssignMaterial(floor, floorMaterial);

            CreatePrimitive(PrimitiveType.Cube, themeKey + "Ceiling", new Vector3(0f, 5.35f, 0f), new Vector3(9.6f, 0.25f, 8.6f), trimMaterial);
            CreatePrimitive(PrimitiveType.Cube, themeKey + "BackWall", new Vector3(0f, 2.65f, 8.3f), new Vector3(9.6f, 5.3f, 0.45f), wallMaterial);
            CreatePrimitive(PrimitiveType.Cube, themeKey + "FrontWall", new Vector3(0f, 2.65f, -8.3f), new Vector3(9.6f, 5.3f, 0.45f), wallMaterial);
            CreatePrimitive(PrimitiveType.Cube, themeKey + "LeftWall", new Vector3(-8.6f, 2.65f, 0f), new Vector3(0.45f, 5.3f, 16.6f), wallMaterial);
            CreatePrimitive(PrimitiveType.Cube, themeKey + "RightWall", new Vector3(8.6f, 2.65f, 0f), new Vector3(0.45f, 5.3f, 16.6f), wallMaterial);
            CreateVisualPrimitive(PrimitiveType.Cube, themeKey + "BackWallpaperPanel", new Vector3(0f, 2.75f, 8.02f), new Vector3(7.8f, 3.4f, 0.08f), trimMaterial);
            CreateVisualPrimitive(PrimitiveType.Cube, themeKey + "WindowGlowLeft", new Vector3(-8.32f, 3.05f, 2.8f), new Vector3(0.08f, 1.45f, 3.4f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, themeKey + "WindowGlowRight", new Vector3(8.32f, 3.05f, -2.6f), new Vector3(0.08f, 1.45f, 3.2f), glow);

            CreateSupplementalInteriorDressing(themeKey, trimMaterial, accentMaterial, wood, metal, paper, glow, contactCoat);

            var exitDoor = CreatePrimitive(
                PrimitiveType.Cube,
                themeKey + "ExitDoor",
                new Vector3(0f, 1f, -6.95f),
                new Vector3(1.9f, 2.2f, 0.45f),
                trimMaterial);
            var exitInteractable = exitDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(exitInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(exitInteractable, "promptText", exitPrompt);
            SetStringValue(exitInteractable, "sourceScene", sceneName);
            SetStringValue(exitInteractable, "targetScene", exteriorSceneName);
            SetStringValue(exitInteractable, "spawnPointId", exteriorSpawnPointId);

            EditorSceneManager.SaveScene(scene);
        }

        private static void CreateSupplementalInteriorDressing(
            string themeKey,
            Material trim,
            Material accent,
            Material wood,
            Material metal,
            Material paper,
            Material glow,
            Material contactCoat)
        {
            switch (themeKey)
            {
                case "Warehouse":
                    CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseLoadingDesk", new Vector3(-3.4f, 0.95f, 2.8f), new Vector3(2.4f, 1.6f, 1.35f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseScalePlatform", new Vector3(2.8f, 0.35f, 1.8f), new Vector3(2.2f, 0.28f, 1.8f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseManifestStack", new Vector3(-3.0f, 1.82f, 2.8f), new Vector3(0.9f, 0.24f, 0.6f), paper);
                    CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseIceRoomGlow", new Vector3(0f, 2.4f, 7.92f), new Vector3(4.8f, 0.36f, 0.08f), glow);
                    for (var index = 0; index < 8; index += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseCrateStack_" + index, new Vector3(-5.4f + ((index % 2) * 1.1f), 0.55f + ((index / 2) * 0.34f), -1.8f + (index * 0.7f)), new Vector3(0.88f, 0.82f, 0.78f), wood);
                    }

                    CreateNoirActor("WarehouseClerk", new Vector3(3.4f, 1f, 4.2f), contactCoat, null, false);
                    break;

                case "PrintShop":
                    CreateVisualPrimitive(PrimitiveType.Cube, "PrintPressBed", new Vector3(0f, 0.9f, 1.8f), new Vector3(4.2f, 1.2f, 2.1f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cylinder, "PrintPressRollerA", new Vector3(-1.2f, 1.65f, 1.8f), new Vector3(0.38f, 0.9f, 0.38f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cylinder, "PrintPressRollerB", new Vector3(1.2f, 1.65f, 1.8f), new Vector3(0.38f, 0.9f, 0.38f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cube, "UnionFlyerTable", new Vector3(-4.8f, 0.8f, -0.8f), new Vector3(2.2f, 1.15f, 1.45f), wood);
                    for (var stack = 0; stack < 6; stack += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cube, "PrintPaperStack_" + stack, new Vector3(-5.0f + (stack * 0.34f), 1.45f + (stack * 0.03f), -0.8f), new Vector3(0.55f, 0.08f, 0.72f), paper);
                    }

                    CreateVisualPrimitive(PrimitiveType.Cube, "InkCabinet", new Vector3(5.1f, 1.4f, 3.4f), new Vector3(1.5f, 2.4f, 1.2f), trim);
                    CreateNoirActor("PrintShopForeman", new Vector3(4.2f, 1f, -2.4f), contactCoat, null, false);
                    break;

                case "Tenement":
                    CreateVisualPrimitive(PrimitiveType.Cube, "TenementKitchenTable", new Vector3(-2.8f, 0.78f, 1.8f), new Vector3(2.1f, 1f, 1.35f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "TenementBed", new Vector3(4.4f, 0.55f, 2.8f), new Vector3(2.6f, 0.72f, 1.8f), trim);
                    CreateVisualPrimitive(PrimitiveType.Cube, "TenementBlanket", new Vector3(4.4f, 0.98f, 2.8f), new Vector3(2.35f, 0.12f, 1.55f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cube, "TenementRadiator", new Vector3(-7.9f, 0.95f, -1.6f), new Vector3(0.22f, 1.3f, 2.3f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, "TenementSaintCard", new Vector3(0f, 2.8f, 8.0f), new Vector3(1.1f, 1.4f, 0.08f), accent);
                    for (var chair = 0; chair < 4; chair += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cube, "TenementChair_" + chair, new Vector3(-4.2f + (chair * 0.9f), 0.55f, 0.2f), new Vector3(0.55f, 0.9f, 0.55f), wood);
                    }

                    CreateNoirActor("TenementNeighbor", new Vector3(-5.1f, 1f, 4.4f), contactCoat, null, false);
                    break;

                case "SocialClub":
                    CreateVisualPrimitive(PrimitiveType.Cube, "SocialClubBar", new Vector3(-4.5f, 0.95f, 2.4f), new Vector3(1.4f, 1.6f, 5.2f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "SocialClubBackBarMirror", new Vector3(-7.95f, 2.6f, 2.4f), new Vector3(0.08f, 2.2f, 4.8f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, "SocialClubCardTable", new Vector3(1.6f, 0.78f, 1.8f), new Vector3(2.2f, 0.95f, 2.2f), trim);
                    CreateVisualPrimitive(PrimitiveType.Cube, "SocialClubStage", new Vector3(4.8f, 0.42f, 5.1f), new Vector3(3.2f, 0.35f, 1.7f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cube, "SocialClubSafe", new Vector3(5.8f, 1.15f, -3.8f), new Vector3(1.1f, 1.8f, 1.1f), metal);
                    for (var chair = 0; chair < 6; chair += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cylinder, "SocialClubStool_" + chair, new Vector3(-2.5f, 0.65f, -1.9f + (chair * 0.85f)), new Vector3(0.28f, 0.45f, 0.28f), accent);
                    }

                    CreateNoirActor("SocialClubBoss", new Vector3(2.8f, 1f, -2.6f), contactCoat, null, false);
                    break;

                case "UnionHall":
                    CreateVisualPrimitive(PrimitiveType.Cube, "UnionHallPodium", new Vector3(0f, 0.95f, 5.2f), new Vector3(1.5f, 1.7f, 1f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "UnionHallBanner", new Vector3(0f, 3.35f, 7.96f), new Vector3(5.8f, 1f, 0.08f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cube, "UnionHallNoticeBoard", new Vector3(-7.95f, 2.6f, 0.8f), new Vector3(0.08f, 2.7f, 4.4f), paper);
                    for (var row = 0; row < 4; row += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cube, "UnionHallBench_" + row, new Vector3(-3.6f + (row * 2.4f), 0.62f, -0.8f), new Vector3(1.7f, 0.72f, 0.45f), wood);
                        CreateVisualPrimitive(PrimitiveType.Cube, "UnionHallBackrest_" + row, new Vector3(-3.6f + (row * 2.4f), 1.02f, -1.1f), new Vector3(1.7f, 0.55f, 0.18f), wood);
                    }

                    CreateVisualPrimitive(PrimitiveType.Cube, "UnionHallDuesBox", new Vector3(4.8f, 1f, 4.4f), new Vector3(1.2f, 1.4f, 1f), metal);
                    CreateNoirActor("UnionHallSteward", new Vector3(-3.8f, 1f, 4.3f), contactCoat, null, false);
                    break;

                case "Diner":
                    CreateVisualPrimitive(PrimitiveType.Cube, "DinerCounter", new Vector3(-3.8f, 0.9f, 1.8f), new Vector3(1.4f, 1.35f, 5.6f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DinerCoffeeUrn", new Vector3(-4.2f, 1.85f, 3.2f), new Vector3(0.75f, 0.85f, 0.75f), metal);
                    for (var stool = 0; stool < 5; stool += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cylinder, "DinerStool_" + stool, new Vector3(-1.9f, 0.65f, -1.4f + (stool * 1.05f)), new Vector3(0.28f, 0.45f, 0.28f), accent);
                    }

                    CreateVisualPrimitive(PrimitiveType.Cube, "DinerBackBooth", new Vector3(4.4f, 0.75f, 4.6f), new Vector3(3.2f, 1f, 1.3f), trim);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DinerNeonMenu", new Vector3(0f, 3.25f, 7.96f), new Vector3(4.4f, 0.85f, 0.08f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DinerPieCase", new Vector3(2.4f, 1.05f, -3.8f), new Vector3(1.8f, 1.2f, 1f), paper);
                    CreateNoirActor("DinerOwner", new Vector3(-5.4f, 1f, 4.4f), contactCoat, null, false);
                    break;

                case "LockerRoom":
                    for (var locker = 0; locker < 5; locker += 1)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cube, "LockerRoomLocker_" + locker, new Vector3(-7.6f, 1.65f, -4.2f + (locker * 1.2f)), new Vector3(0.82f, 2.7f, 0.8f), metal);
                    }

                    CreateVisualPrimitive(PrimitiveType.Cube, "LockerRoomToolBench", new Vector3(1.8f, 0.82f, 2.5f), new Vector3(4.2f, 1f, 1.4f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "LockerRoomWeaponCrate", new Vector3(5.1f, 0.65f, -3.2f), new Vector3(1.5f, 1.1f, 1.2f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cylinder, "LockerRoomStove", new Vector3(-2.6f, 1.05f, 4.8f), new Vector3(0.55f, 1f, 0.55f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cylinder, "LockerRoomStovePipe", new Vector3(-2.6f, 3.1f, 4.8f), new Vector3(0.18f, 2.4f, 0.18f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, "LockerRoomShiftBoard", new Vector3(0f, 2.9f, 7.96f), new Vector3(4.8f, 2.1f, 0.08f), paper);
                    CreateNoirActor("LockerRoomMechanic", new Vector3(3.7f, 1f, 4.1f), contactCoat, null, false);
                    break;

                default:
                    CreateVisualPrimitive(PrimitiveType.Cube, "DispatchMapTable", new Vector3(-1.2f, 0.9f, 2.3f), new Vector3(3.2f, 1.2f, 1.8f), wood);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DispatchRouteMap", new Vector3(-1.2f, 1.58f, 2.3f), new Vector3(2.65f, 0.08f, 1.3f), paper);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DispatchSignalBoard", new Vector3(0f, 2.9f, 7.96f), new Vector3(5.4f, 2.2f, 0.08f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DispatchAmberSignal", new Vector3(-1.8f, 3.2f, 7.9f), new Vector3(0.52f, 0.52f, 0.05f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DispatchRedSignal", new Vector3(0f, 3.2f, 7.9f), new Vector3(0.52f, 0.52f, 0.05f), accent);
                    CreateVisualPrimitive(PrimitiveType.Cube, "DispatchLockerRow", new Vector3(5.6f, 1.7f, -1.4f), new Vector3(1.4f, 3.1f, 3.4f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cylinder, "DispatchStovePipe", new Vector3(-6.5f, 2.8f, -2.6f), new Vector3(0.22f, 2.8f, 0.22f), metal);
                    CreateNoirActor("DispatchOperator", new Vector3(3.5f, 1f, 3.8f), contactCoat, null, false);
                    break;
            }
        }

        private static void BuildBusinessCoreScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/District_BusinessCore_01.unity");
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var unionDueMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/UnionDue.asset");
            var bookkeeperDialogue = AssetDatabase.LoadAssetAtPath<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/BookkeeperWarning.asset");
            var dockCourierActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/DockCourier.asset");
            var ledgerRunActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/LedgerRun.asset");
            var unionGetawayActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/UnionGetaway.asset");
            var unionCrackdownMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/UnionCrackdown.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.09f, 0.105f, 0.125f), 0.86f, 0.02f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/Sidewalk.mat", new Color(0.24f, 0.23f, 0.21f), 0.34f, 0f);
            var stone = GetOrCreateMaterial("Assets/Game/Materials/Stone.mat", new Color(0.22f, 0.22f, 0.2f), 0.32f, 0f);
            var brick = GetOrCreateMaterial("Assets/Game/Materials/Brick.mat", new Color(0.21f, 0.13f, 0.12f), 0.18f, 0f);
            var lanePaint = GetOrCreateMaterial("Assets/Game/Materials/LanePaint.mat", new Color(0.9f, 0.76f, 0.34f), 0.65f, 0f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(1f, 0.78f, 0.42f), 0.86f, 0.08f, new Color(1f, 0.68f, 0.28f) * 3.2f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var policeCoat = GetOrCreateMaterial("Assets/Game/Materials/PoliceCoat.mat", new Color(0.08f, 0.13f, 0.19f), 0.18f, 0f);
            var officeSign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);

            ApplyExteriorAtmosphere(new Color(0.16f, 0.15f, 0.16f), new Color(0.09f, 0.1f, 0.12f), 0.014f, new Color(1f, 0.82f, 0.58f), 0.9f, Quaternion.Euler(38f, -24f, 0f));

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "BusinessGround";
            ground.transform.localScale = new Vector3(4f, 1f, 4f);
            AssignMaterial(ground, asphalt);

            CreatePrimitive(PrimitiveType.Cube, "BusinessRoad", new Vector3(0f, 0.05f, 0f), new Vector3(10f, 0.1f, 36f), asphalt);
            CreatePrimitive(PrimitiveType.Cube, "BusinessLeftWalk", new Vector3(-6.2f, 0.1f, 0f), new Vector3(3f, 0.2f, 36f), sidewalk);
            CreatePrimitive(PrimitiveType.Cube, "BusinessRightWalk", new Vector3(6.2f, 0.1f, 0f), new Vector3(3f, 0.2f, 36f), sidewalk);
            CreateColliderBlock("BankFacadeCollision", new Vector3(-10.85f, 3.4f, 6.6f), new Vector3(3.7f, 6.8f, 13f));
            CreateColliderBlock("StoneRowCollision", new Vector3(10.85f, 3.2f, 0f), new Vector3(3.7f, 6.4f, 33f));
            CreateColliderBlock("BrickCornerCollision", new Vector3(-10.95f, 2.7f, -11f), new Vector3(3.65f, 5.4f, 8.5f));
            CreateVisualPrimitive(PrimitiveType.Cube, "SquarePlaza", new Vector3(-2.7f, 0.08f, 13.5f), new Vector3(5.5f, 0.04f, 6f), stone);
            CreateVisualPrimitive(PrimitiveType.Cube, "StatueBase", new Vector3(-2.7f, 0.8f, 13.5f), new Vector3(1.2f, 1.2f, 1.2f), stone);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "StatueColumn", new Vector3(-2.7f, 2.3f, 13.5f), new Vector3(0.25f, 1.2f, 0.25f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "StreetcarRailA", new Vector3(-1.7f, 0.075f, 0f), new Vector3(0.08f, 0.01f, 36f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "StreetcarRailB", new Vector3(1.7f, 0.075f, 0f), new Vector3(0.08f, 0.01f, 36f), metal);

            for (var index = 0; index < 6; index += 1)
            {
                CreateVisualPrimitive(
                    PrimitiveType.Cube,
                    "BusinessLaneMarker_" + index,
                    new Vector3(0f, 0.11f, -12f + (index * 5f)),
                    new Vector3(0.35f, 0.02f, 1.8f),
                    lanePaint);
            }

            for (var index = 0; index < 5; index += 1)
            {
                var z = -12f + (index * 6f);
                CreateVisualPrimitive(PrimitiveType.Cube, "BusinessWindowL_" + index, new Vector3(-8.2f, 3.2f, z), new Vector3(1.5f, 1.8f, 0.12f), windowGlow);
                CreateVisualPrimitive(PrimitiveType.Cube, "BusinessWindowR_" + index, new Vector3(8.3f, 3.2f, z + 1.4f), new Vector3(1.5f, 1.8f, 0.12f), windowGlow);
            }

            CreateLampPost(new Vector3(-4.8f, 0f, -6f), metal, windowGlow);
            CreateLampPost(new Vector3(4.8f, 0f, 0f), metal, windowGlow);
            CreateLampPost(new Vector3(-4.8f, 0f, 9f), metal, windowGlow);
            CreateLampPost(new Vector3(4.8f, 0f, 16f), metal, windowGlow);
            CreateProductionBusinessCorePass(stone, brick, metal, windowGlow, officeSign, sidewalk, puddle, brass);
            CreateReadyAssetUrbanCanyonPass(DistrictArtStyle.BusinessCore, "Business", stone, stone, metal, windowGlow, officeSign, sidewalk, puddle, brass);
            CreateNoirStreetDressing("Business", metal, windowGlow, crateWood, officeSign, sidewalk, puddle, brass);
            CreateNoirSpawnComposition("BusinessStart", new Vector3(0f, 0f, -15f), metal, windowGlow, crateWood, officeSign, sidewalk, puddle, brass, "UNION SQ");
            CreateFinishedCityBlockPass("Business", asphalt, stone, stone, metal, windowGlow, officeSign, sidewalk, puddle, brass);
            CreateDistrictRooftopDetailPass("Business", -10.6f, 10.6f, metal, windowGlow, stone, officeSign, brass);
            CreateBusinessCoreIdentityPass(metal, windowGlow, stone, officeSign, brass);
            CreateKenneyDistrictAssetPass("Business");
            CreateMafiaDistrictProductionPass(DistrictArtStyle.BusinessCore, "Business", asphalt, stone, stone, metal, windowGlow, officeSign, sidewalk, puddle, brass);

            var runtime = CreateFreeRoamDistrictRuntime(
                "District_BusinessCore_01",
                "BusinessCoreStart",
                new Vector3(-1.4f, 1f, -10.4f),
                new Color(0.16f, 0.17f, 0.21f),
                new Vector3(0f, 17.6f, -7.8f),
                campaignDatabase,
                chapterAsset);
            ConfigurePoliceResponse(
                runtime.PoliceResponseController,
                CreatePoliceResponder("BusinessWatchOfficer", new Vector3(-4.8f, 1f, -2f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("BusinessPatrolOfficerA", new Vector3(4.8f, 1f, 2.4f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("BusinessPatrolOfficerB", new Vector3(-2.2f, 1f, 9.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("BusinessHuntOfficerA", new Vector3(6.2f, 1f, -8.2f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("BusinessHuntOfficerB", new Vector3(-5.9f, 1f, 14.3f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("BusinessHuntOfficerC", new Vector3(1.6f, 1f, 17.2f), policeCoat, runtime.PlayerController, runtime.PlayerHealth));

            var missionSystem = CreateSceneObjectiveSystem("BusinessCoreObjectiveSystem");
            CreateSceneObjectiveHud("BusinessCoreObjectiveHud", missionSystem, runtime.SaveGameFileService, campaignDatabase);

            CreateSpawnPoint("BusinessCoreStartPoint", "BusinessCoreStart", new Vector3(0f, 1f, -15f));
            CreateSpawnPoint("BusinessSquareExitSpawnPoint", "BusinessSquareExitSpawn", new Vector3(-1.4f, 1f, 11.8f));
            CreateSpawnPoint("BusinessDowntownExitSpawnPoint", "BusinessDowntownExitSpawn", new Vector3(7.8f, 1f, 4.2f));
            CreateSpawnPoint("FromDocksGatePoint", "FromDocksGate", new Vector3(0f, 1f, -14f));
            CreateSpawnPoint("FromOldQuarterGatePoint", "FromOldQuarterGate", new Vector3(12f, 1f, 0f));
            CreateSpawnPoint("FromBookkeeperInteriorPoint", "FromBookkeeperInterior", new Vector3(5.4f, 1f, -3.2f));
            CreateSpawnPoint("FromPrintShopInteriorPoint", "FromPrintShopInterior", new Vector3(-5.35f, 1f, -10.6f));
            CreateSpawnPoint("FromUnionHallInteriorPoint", "FromUnionHallInterior", new Vector3(5.35f, 1f, 8.4f));

            CreateTravelGate(
                "ToDocksGate",
                new Vector3(0f, 1.1f, -16.8f),
                new Vector3(6.4f, 2.2f, 0.4f),
                officeSign,
                "Drive back to the harbor",
                "docks",
                "District_01",
                "FromBusinessCoreGate",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService);

            CreateTravelGate(
                "ToOldQuarterGate",
                new Vector3(6.95f, 1.1f, 0f),
                new Vector3(0.55f, 2.2f, 6.4f),
                officeSign,
                "Head into the old quarter",
                "old-quarter",
                "District_OldQuarter_01",
                "FromBusinessCoreGate",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService,
                missionSystem,
                "leave-square",
                runtime.CampaignProgressionController,
                unionDueMission,
                "leave-square");

            CreatePrimitive(
                PrimitiveType.Cube,
                "BelloriBooksApron",
                new Vector3(6.55f, 0.14f, -3.2f),
                new Vector3(1.7f, 0.08f, 2.1f),
                sidewalk);

            var businessInteriorDoor = CreatePrimitive(
                PrimitiveType.Cube,
                "BelloriBooksDoor",
                new Vector3(7.55f, 1.1f, -3.2f),
                new Vector3(0.35f, 2.2f, 1.8f),
                officeSign);
            var businessInteriorInteractable = businessInteriorDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(businessInteriorInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(businessInteriorInteractable, "promptText", "Enter Bellori Books");
            SetStringValue(businessInteriorInteractable, "sourceScene", "District_BusinessCore_01");
            SetStringValue(businessInteriorInteractable, "targetScene", "Interior_BusinessBookkeeper_01");
            SetStringValue(businessInteriorInteractable, "spawnPointId", "BusinessInteriorSpawn");

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "PrintShopApron",
                new Vector3(-5.85f, 0.14f, -10.6f),
                new Vector3(1.8f, 0.08f, 2.2f),
                sidewalk);
            CreateFreeRoamSceneDoor(
                "PrintShopDoor",
                new Vector3(-6.85f, 1.1f, -10.6f),
                new Vector3(0.35f, 2.2f, 1.85f),
                officeSign,
                "Enter union print shop",
                "District_BusinessCore_01",
                "Interior_Business_PrintShop_01",
                "PrintShopInteriorSpawn",
                runtime.SceneTransitionController);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "UnionHallApron",
                new Vector3(6.55f, 0.145f, 8.4f),
                new Vector3(1.75f, 0.08f, 2.2f),
                sidewalk);
            CreateFreeRoamSceneDoor(
                "UnionHallDoor",
                new Vector3(7.55f, 1.1f, 8.4f),
                new Vector3(0.35f, 2.2f, 1.85f),
                officeSign,
                "Enter union hall",
                "District_BusinessCore_01",
                "Interior_Business_UnionHall_01",
                "UnionHallInteriorSpawn",
                runtime.SceneTransitionController);

            CreateStaticVehicle("BusinessSedan", new Vector3(3.8f, 0.72f, 6.5f), new Vector3(1.8f, 0.95f, 4f), new Color(0.12f, 0.12f, 0.14f), metal, windowGlow);
            CreateEnemyGuard("BusinessWatchman", new Vector3(4.2f, 1f, 11.5f), enemyCoat, runtime.PlayerController, runtime.PlayerHealth);

            var unionMissionRoot = new GameObject("UnionDueMissionRoot");
            var bookkeeper = CreateNoirActor("BookkeeperContact", new Vector3(-1.6f, 1f, 13.8f), brass, unionMissionRoot.transform);
            var bookkeeperInteractable = bookkeeper.AddComponent<DialogueInteractable>();
            SetObjectReference(bookkeeperInteractable, "dialogueController", runtime.DialogueController);
            SetObjectReference(bookkeeperInteractable, "dialogueSequence", bookkeeperDialogue);
            SetObjectReference(bookkeeperInteractable, "objectiveSystem", missionSystem);
            SetStringValue(bookkeeperInteractable, "promptText", "Talk to Nico");
            SetStringValue(bookkeeperInteractable, "requiredObjectiveId", "meet-bookkeeper");
            SetObjectReference(bookkeeperInteractable, "campaignProgressionController", runtime.CampaignProgressionController);
            SetObjectReference(bookkeeperInteractable, "missionAsset", unionDueMission);
            SetStringValue(bookkeeperInteractable, "missionStageIdToComplete", "meet-bookkeeper");

            var unionCrackdownRoot = CreateObjectiveActionMissionRoot(
                "UnionCrackdownMissionRoot",
                "UnionSurveyMarker",
                new Vector3(-2.7f, 1.1f, 13.5f),
                new Vector3(1.1f, 1.1f, 1.1f),
                brass,
                "Survey Union Square",
                missionSystem,
                "survey-square",
                runtime.CampaignProgressionController,
                unionCrackdownMission,
                "survey-square");
            CreateMissionTravelGate(
                unionCrackdownRoot.transform,
                "UnionCrackdownExit",
                new Vector3(13.6f, 1.1f, 4.2f),
                new Vector3(0.4f, 2.2f, 3.6f),
                officeSign,
                "Get out of the square",
                "District_OldQuarter_01",
                "OldQuarterStart",
                "leave-downtown",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService,
                missionSystem,
                runtime.CampaignProgressionController,
                unionCrackdownMission,
                "leave-downtown");

            var businessMissionDirectorObject = new GameObject("BusinessMissionDirector");
            var businessMissionDirector = businessMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                businessMissionDirector,
                runtime.SaveGameFileService,
                missionSystem,
                (unionDueMission, unionMissionRoot, new[]
                {
                    ("meet-bookkeeper", "Meet the bookkeeper", "Find Nico Bellori in Union Square.", "leave-square"),
                    ("leave-square", "Leave the square", "Get clear through the old quarter before the patrols close in.", null)
                }),
                (unionCrackdownMission, unionCrackdownRoot, new[]
                {
                    ("survey-square", "Survey the square", "Mark the storefront that folds when the pressure starts.", "leave-downtown"),
                    ("leave-downtown", "Leave downtown", "Disappear before the square turns hostile.", null)
                }));

            var dockCourierComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dockCourierComplete.name = "DockCourierComplete";
            dockCourierComplete.transform.position = new Vector3(-2.4f, 1f, 12.2f);
            dockCourierComplete.transform.localScale = new Vector3(1.2f, 1.5f, 1.2f);
            AssignMaterial(dockCourierComplete, brass);
            var dockCourierCompleteInteractable = dockCourierComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(dockCourierCompleteInteractable, "activityAsset", dockCourierActivity);
            SetObjectReference(dockCourierCompleteInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(dockCourierCompleteInteractable, "promptText", "Deliver the courier satchel");

            var ledgerRunStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ledgerRunStart.name = "LedgerRunStart";
            ledgerRunStart.transform.position = new Vector3(5.2f, 1f, -4.4f);
            ledgerRunStart.transform.localScale = new Vector3(1.4f, 1.6f, 1.4f);
            AssignMaterial(ledgerRunStart, officeSign);
            var ledgerRunStartInteractable = ledgerRunStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(ledgerRunStartInteractable, "activityAsset", ledgerRunActivity);
            SetObjectReference(ledgerRunStartInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(ledgerRunStartInteractable, "promptText", "Take ledger run");

            var unionGetawayStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unionGetawayStart.name = "UnionGetawayStart";
            unionGetawayStart.transform.position = new Vector3(5.6f, 1f, 8.4f);
            unionGetawayStart.transform.localScale = new Vector3(1.35f, 1.55f, 1.35f);
            AssignMaterial(unionGetawayStart, officeSign);
            var unionGetawayStartInteractable = unionGetawayStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(unionGetawayStartInteractable, "activityAsset", unionGetawayActivity);
            SetObjectReference(unionGetawayStartInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(unionGetawayStartInteractable, "promptText", "Take union getaway contract");

            var unionGetawayComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            unionGetawayComplete.name = "UnionGetawayComplete";
            unionGetawayComplete.transform.position = new Vector3(-1.2f, 1f, -14.0f);
            unionGetawayComplete.transform.localScale = new Vector3(1.15f, 1.45f, 1.15f);
            AssignMaterial(unionGetawayComplete, brass);
            var unionGetawayCompleteInteractable = unionGetawayComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(unionGetawayCompleteInteractable, "activityAsset", unionGetawayActivity);
            SetObjectReference(unionGetawayCompleteInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(unionGetawayCompleteInteractable, "promptText", "Finish the getaway handoff");

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildOldQuarterScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/District_OldQuarter_01.unity");
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var chapelDebtMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/ChapelDebt.asset");
            var debtorDialogue = AssetDatabase.LoadAssetAtPath<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/DebtorThreat.asset");
            var lookoutActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/LookoutRun.asset");
            var chapelCollectionActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/ChapelCollection.asset");
            var chapelAshMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/ChapelAsh.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.085f, 0.1f, 0.12f), 0.86f, 0.02f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/Sidewalk.mat", new Color(0.22f, 0.21f, 0.19f), 0.32f, 0f);
            var tenement = GetOrCreateMaterial("Assets/Game/Materials/TenementBrick.mat", new Color(0.22f, 0.12f, 0.1f), 0.18f, 0f);
            var churchStone = GetOrCreateMaterial("Assets/Game/Materials/ChurchStone.mat", new Color(0.27f, 0.26f, 0.23f), 0.28f, 0f);
            var laundry = GetOrCreateMaterial("Assets/Game/Materials/LaundryCloth.mat", new Color(0.67f, 0.63f, 0.58f), 0.12f, 0f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(1f, 0.78f, 0.42f), 0.86f, 0.08f, new Color(1f, 0.68f, 0.28f) * 3.2f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var policeCoat = GetOrCreateMaterial("Assets/Game/Materials/PoliceCoat.mat", new Color(0.08f, 0.13f, 0.19f), 0.18f, 0f);
            var sign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);

            ApplyExteriorAtmosphere(new Color(0.15f, 0.13f, 0.12f), new Color(0.095f, 0.085f, 0.08f), 0.015f, new Color(1f, 0.78f, 0.52f), 0.88f, Quaternion.Euler(32f, -34f, 0f));

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "QuarterGround";
            ground.transform.localScale = new Vector3(3.6f, 1f, 3.6f);
            AssignMaterial(ground, asphalt);

            CreatePrimitive(PrimitiveType.Cube, "QuarterRoad", new Vector3(0f, 0.05f, 0f), new Vector3(8.4f, 0.1f, 32f), asphalt);
            CreatePrimitive(PrimitiveType.Cube, "QuarterLeftWalk", new Vector3(-5.5f, 0.1f, 0f), new Vector3(2.6f, 0.2f, 32f), sidewalk);
            CreatePrimitive(PrimitiveType.Cube, "QuarterRightWalk", new Vector3(5.5f, 0.1f, 0f), new Vector3(2.6f, 0.2f, 32f), sidewalk);
            CreateColliderBlock("TenementRowLeftCollision", new Vector3(-10.2f, 3f, 0f), new Vector3(3.05f, 6f, 30f));
            CreateColliderBlock("TenementRowRightCollision", new Vector3(10.2f, 3f, 0f), new Vector3(3.05f, 6f, 30f));
            CreateColliderBlock("ChurchBodyCollision", new Vector3(-10.4f, 4.2f, 11.5f), new Vector3(3.5f, 8.4f, 9.4f));
            CreateColliderBlock("ChurchTowerCollision", new Vector3(-10.4f, 7.3f, 16.2f), new Vector3(1.8f, 14f, 2.2f));
            CreateVisualPrimitive(PrimitiveType.Cube, "NarrowAlley", new Vector3(0f, 0.06f, -9.4f), new Vector3(3.4f, 0.04f, 7f), sidewalk);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -10f + (index * 5.8f);
                CreateVisualPrimitive(PrimitiveType.Cube, "QuarterWindowL_" + index, new Vector3(-7.2f, 3.1f, z), new Vector3(1.2f, 1.6f, 0.1f), windowGlow);
                CreateVisualPrimitive(PrimitiveType.Cube, "QuarterWindowR_" + index, new Vector3(7.2f, 3.1f, z + 1.1f), new Vector3(1.2f, 1.6f, 0.1f), windowGlow);
                CreateVisualPrimitive(PrimitiveType.Cube, "LaundryLine_" + index, new Vector3(0f, 5.6f, z + 2f), new Vector3(9.6f, 0.05f, 0.05f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, "LaundryCloth_" + index, new Vector3(-1.8f + (index * 0.7f), 5.25f, z + 2f), new Vector3(0.42f, 0.6f, 0.02f), laundry);
            }

            CreateLampPost(new Vector3(-4.2f, 0f, -4f), metal, windowGlow);
            CreateLampPost(new Vector3(4.2f, 0f, 4f), metal, windowGlow);
            CreateLampPost(new Vector3(-4.2f, 0f, 12f), metal, windowGlow);
            CreateProductionOldQuarterPass(tenement, churchStone, metal, windowGlow, laundry, sign, sidewalk, puddle, brass);
            CreateReadyAssetUrbanCanyonPass(DistrictArtStyle.OldQuarter, "Quarter", tenement, churchStone, metal, windowGlow, sign, sidewalk, puddle, brass);
            CreateNoirStreetDressing("Quarter", metal, windowGlow, crateWood, sign, sidewalk, puddle, brass);
            CreateNoirSpawnComposition("QuarterStart", new Vector3(0f, 0f, -12.8f), metal, windowGlow, crateWood, sign, sidewalk, puddle, brass, "ST VERA");
            CreateFinishedCityBlockPass("Quarter", asphalt, tenement, churchStone, metal, windowGlow, sign, sidewalk, puddle, brass);
            CreateDistrictRooftopDetailPass("Quarter", -10.6f, 10.6f, metal, windowGlow, tenement, sign, brass);
            CreateOldQuarterIdentityPass(metal, windowGlow, churchStone, tenement, laundry, brass);
            CreateKenneyDistrictAssetPass("OldQuarter");
            CreateMafiaDistrictProductionPass(DistrictArtStyle.OldQuarter, "Quarter", asphalt, tenement, churchStone, metal, windowGlow, sign, sidewalk, puddle, brass);

            var runtime = CreateFreeRoamDistrictRuntime(
                "District_OldQuarter_01",
                "OldQuarterStart",
                new Vector3(-1.2f, 1f, -9.8f),
                new Color(0.14f, 0.15f, 0.18f),
                new Vector3(0f, 17.2f, -7.4f),
                campaignDatabase,
                chapterAsset);
            ConfigurePoliceResponse(
                runtime.PoliceResponseController,
                CreatePoliceResponder("QuarterWatchOfficer", new Vector3(4.2f, 1f, -4.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("QuarterPatrolOfficerA", new Vector3(-1.6f, 1f, 4.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("QuarterPatrolOfficerB", new Vector3(5.2f, 1f, 10.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("QuarterHuntOfficerA", new Vector3(-4.8f, 1f, 11.5f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("QuarterHuntOfficerB", new Vector3(6.4f, 1f, -8.5f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("QuarterHuntOfficerC", new Vector3(0.5f, 1f, 14.5f), policeCoat, runtime.PlayerController, runtime.PlayerHealth));

            var missionSystem = CreateSceneObjectiveSystem("OldQuarterObjectiveSystem");
            CreateSceneObjectiveHud("OldQuarterObjectiveHud", missionSystem, runtime.SaveGameFileService, campaignDatabase);

            CreateSpawnPoint("OldQuarterStartPoint", "OldQuarterStart", new Vector3(-1.2f, 1f, -9.8f));
            CreateSpawnPoint("OldQuarterMessageSpawnPoint", "OldQuarterMessageSpawn", new Vector3(-2.8f, 1f, 6.8f));
            CreateSpawnPoint("OldQuarterExitSpawnPoint", "OldQuarterExitSpawn", new Vector3(8.2f, 1f, -2.6f));
            CreateSpawnPoint("FromBusinessCoreGatePoint", "FromBusinessCoreGate", new Vector3(12.2f, 1f, 0f));
            CreateSpawnPoint("FromChapelInteriorPoint", "FromChapelInterior", new Vector3(-5.2f, 1f, 8.4f));
            CreateSpawnPoint("FromTenementInteriorPoint", "FromTenementInterior", new Vector3(5.35f, 1f, -2.2f));
            CreateSpawnPoint("FromDinerInteriorPoint", "FromDinerInterior", new Vector3(5.35f, 1f, 6.4f));

            CreateTravelGate(
                "ToBusinessCoreGate",
                new Vector3(13.4f, 1.1f, 0f),
                new Vector3(0.4f, 2.2f, 6f),
                sign,
                "Return to the business core",
                "business-core",
                "District_BusinessCore_01",
                "FromOldQuarterGate",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService);

            CreateTravelGate(
                "ToRailYardGate",
                new Vector3(-6.95f, 1.1f, 6.5f),
                new Vector3(0.55f, 2.2f, 6.2f),
                sign,
                "Cut through to the rail yard",
                "rail-yard",
                "District_RailYard_01",
                "FromOldQuarterGate",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService,
                missionSystem,
                "push-message",
                runtime.CampaignProgressionController,
                chapelDebtMission,
                "push-message");

            CreatePrimitive(
                PrimitiveType.Cube,
                "SaintVeraApron",
                new Vector3(-5.4f, 0.14f, 8.4f),
                new Vector3(1.9f, 0.08f, 2.2f),
                sidewalk);

            var chapelInteriorDoor = CreatePrimitive(
                PrimitiveType.Cube,
                "SaintVeraDoor",
                new Vector3(-6.25f, 1.1f, 8.4f),
                new Vector3(0.35f, 2.2f, 1.9f),
                sign);
            var chapelInteriorInteractable = chapelInteriorDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(chapelInteriorInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(chapelInteriorInteractable, "promptText", "Enter Saint Vera");
            SetStringValue(chapelInteriorInteractable, "sourceScene", "District_OldQuarter_01");
            SetStringValue(chapelInteriorInteractable, "targetScene", "Interior_OldQuarter_Chapel_01");
            SetStringValue(chapelInteriorInteractable, "spawnPointId", "ChapelInteriorSpawn");

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "TenementApron",
                new Vector3(5.85f, 0.145f, -2.2f),
                new Vector3(1.75f, 0.08f, 2.05f),
                sidewalk);
            CreateFreeRoamSceneDoor(
                "SavaTenementDoor",
                new Vector3(6.85f, 1.1f, -2.2f),
                new Vector3(0.35f, 2.2f, 1.75f),
                sign,
                "Enter Sava tenement",
                "District_OldQuarter_01",
                "Interior_OldQuarter_Tenement_01",
                "TenementInteriorSpawn",
                runtime.SceneTransitionController);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "CafeSavaApron",
                new Vector3(5.85f, 0.145f, 6.4f),
                new Vector3(1.75f, 0.08f, 2.05f),
                sidewalk);
            CreateFreeRoamSceneDoor(
                "CafeSavaDoor",
                new Vector3(6.85f, 1.1f, 6.4f),
                new Vector3(0.35f, 2.2f, 1.75f),
                sign,
                "Enter Cafe Sava",
                "District_OldQuarter_01",
                "Interior_OldQuarter_Diner_01",
                "DinerInteriorSpawn",
                runtime.SceneTransitionController);

            CreateStaticVehicle("QuarterCoupe", new Vector3(-2.9f, 0.72f, 7.4f), new Vector3(1.7f, 0.92f, 3.8f), new Color(0.19f, 0.07f, 0.05f), metal, windowGlow);
            CreateEnemyGuard("QuarterCollector", new Vector3(2.6f, 1f, 9.2f), enemyCoat, runtime.PlayerController, runtime.PlayerHealth);

            var chapelMissionRoot = new GameObject("ChapelDebtMissionRoot");
            var debtor = CreateNoirActor("DebtorContact", new Vector3(-5.8f, 1f, 10.2f), sign, chapelMissionRoot.transform);
            var debtorInteractable = debtor.AddComponent<DialogueInteractable>();
            SetObjectReference(debtorInteractable, "dialogueController", runtime.DialogueController);
            SetObjectReference(debtorInteractable, "dialogueSequence", debtorDialogue);
            SetObjectReference(debtorInteractable, "objectiveSystem", missionSystem);
            SetStringValue(debtorInteractable, "promptText", "Talk to Pietro");
            SetStringValue(debtorInteractable, "requiredObjectiveId", "find-debtor");
            SetObjectReference(debtorInteractable, "campaignProgressionController", runtime.CampaignProgressionController);
            SetObjectReference(debtorInteractable, "missionAsset", chapelDebtMission);
            SetStringValue(debtorInteractable, "missionStageIdToComplete", "find-debtor");

            var prayerCard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prayerCard.name = "PrayerCardMarker";
            prayerCard.transform.SetParent(chapelMissionRoot.transform, false);
            prayerCard.transform.position = new Vector3(-8f, 1.1f, 14.2f);
            prayerCard.transform.localScale = new Vector3(0.7f, 0.1f, 0.9f);
            AssignMaterial(prayerCard, laundry);
            var chapelMissionDirectorObject = new GameObject("OldQuarterMissionDirector");
            var oldQuarterMissionDirector = chapelMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                oldQuarterMissionDirector,
                runtime.SaveGameFileService,
                missionSystem,
                (chapelDebtMission, chapelMissionRoot, new[]
                {
                    ("find-debtor", "Find the debtor", "Track Pietro through the old quarter and make contact.", "push-message"),
                    ("push-message", "Push the message", "Head toward the rail yard after making the threat stick.", null)
                }),
                (chapelAshMission, CreateChapelAshMissionRoot(runtime, missionSystem, chapelAshMission, sign, laundry), new[]
                {
                    ("investigate-courtyard", "Investigate the courtyard", "Search the Saint Vera yard for proof in the ash.", "leave-quarter"),
                    ("leave-quarter", "Leave the quarter", "Get clear before the block closes around you.", null)
                }));

            var lookoutStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lookoutStart.name = "LookoutRunStart";
            lookoutStart.transform.position = new Vector3(-8.4f, 1f, -2.4f);
            lookoutStart.transform.localScale = new Vector3(1.3f, 1.5f, 1.3f);
            AssignMaterial(lookoutStart, sign);
            var lookoutStartInteractable = lookoutStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(lookoutStartInteractable, "activityAsset", lookoutActivity);
            SetObjectReference(lookoutStartInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(lookoutStartInteractable, "promptText", "Take church lookout");

            var lookoutComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lookoutComplete.name = "LookoutRunComplete";
            lookoutComplete.transform.position = new Vector3(6.4f, 1f, 13.6f);
            lookoutComplete.transform.localScale = new Vector3(1.2f, 1.4f, 1.2f);
            AssignMaterial(lookoutComplete, laundry);
            var lookoutCompleteInteractable = lookoutComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(lookoutCompleteInteractable, "activityAsset", lookoutActivity);
            SetObjectReference(lookoutCompleteInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(lookoutCompleteInteractable, "promptText", "Report the church lookout");

            var chapelCollectionStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chapelCollectionStart.name = "ChapelCollectionStart";
            chapelCollectionStart.transform.position = new Vector3(5.6f, 1f, 6.4f);
            chapelCollectionStart.transform.localScale = new Vector3(1.3f, 1.5f, 1.3f);
            AssignMaterial(chapelCollectionStart, sign);
            var chapelCollectionStartInteractable = chapelCollectionStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(chapelCollectionStartInteractable, "activityAsset", chapelCollectionActivity);
            SetObjectReference(chapelCollectionStartInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(chapelCollectionStartInteractable, "promptText", "Take chapel collection");

            var chapelCollectionComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chapelCollectionComplete.name = "ChapelCollectionComplete";
            chapelCollectionComplete.transform.position = new Vector3(-5.8f, 1f, 13.2f);
            chapelCollectionComplete.transform.localScale = new Vector3(1.15f, 1.4f, 1.15f);
            AssignMaterial(chapelCollectionComplete, brass);
            var chapelCollectionCompleteInteractable = chapelCollectionComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(chapelCollectionCompleteInteractable, "activityAsset", chapelCollectionActivity);
            SetObjectReference(chapelCollectionCompleteInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(chapelCollectionCompleteInteractable, "promptText", "Deliver the chapel collection");

            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildRailYardScene()
        {
            var scene = OpenOrCreateScene("Assets/Game/Scenes/District_RailYard_01.unity");
            ClearScene(scene);
            var campaignDatabase = AssetDatabase.LoadAssetAtPath<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");
            var chapterAsset = AssetDatabase.LoadAssetAtPath<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var yardHeatMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/YardHeat.asset");
            var chopDeliveryActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/ChopDelivery.asset");
            var garagePrepActivity = AssetDatabase.LoadAssetAtPath<ActivityDefinitionAsset>("Assets/Game/Data/Activities/GaragePrep.asset");
            var yardBetrayalMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/YardBetrayal.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.08f, 0.095f, 0.11f), 0.86f, 0.02f);
            var gravel = GetOrCreateMaterial("Assets/Game/Materials/Gravel.mat", new Color(0.18f, 0.18f, 0.17f), 0.26f, 0f);
            var rustMetal = GetOrCreateMaterial("Assets/Game/Materials/RustMetal.mat", new Color(0.24f, 0.15f, 0.11f), 0.62f, 0.45f);
            var tankMetal = GetOrCreateMaterial("Assets/Game/Materials/TankMetal.mat", new Color(0.14f, 0.17f, 0.19f), 0.78f, 0.62f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(1f, 0.78f, 0.42f), 0.86f, 0.08f, new Color(1f, 0.68f, 0.28f) * 3.2f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var policeCoat = GetOrCreateMaterial("Assets/Game/Materials/PoliceCoat.mat", new Color(0.08f, 0.13f, 0.19f), 0.18f, 0f);
            var sign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);

            ApplyExteriorAtmosphere(new Color(0.13f, 0.14f, 0.15f), new Color(0.075f, 0.085f, 0.095f), 0.017f, new Color(0.92f, 0.78f, 0.58f), 0.82f, Quaternion.Euler(28f, -40f, 0f));

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "RailGround";
            ground.transform.localScale = new Vector3(4f, 1f, 4f);
            AssignMaterial(ground, gravel);

            CreatePrimitive(PrimitiveType.Cube, "RailRoad", new Vector3(0f, 0.05f, -8.5f), new Vector3(8.8f, 0.1f, 16f), asphalt);
            CreateVisualPrimitive(PrimitiveType.Cube, "TrackA", new Vector3(-2.2f, 0.07f, 8f), new Vector3(0.08f, 0.02f, 26f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "TrackB", new Vector3(2.2f, 0.07f, 8f), new Vector3(0.08f, 0.02f, 26f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "Sleepers", new Vector3(0f, 0.05f, 8f), new Vector3(5.4f, 0.02f, 26f), rustMetal);
            CreateColliderBlock("GarageHallCollision", new Vector3(-10.4f, 2.9f, -2f), new Vector3(4.1f, 5.8f, 16f));
            CreateVisualPrimitive(PrimitiveType.Cube, "TrainCar", new Vector3(2.2f, 1.55f, 10.5f), new Vector3(2.8f, 3f, 8.2f), rustMetal);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "FuelTankA", new Vector3(9.6f, 2.2f, -2.5f), new Vector3(1.3f, 2.2f, 1.3f), tankMetal);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "FuelTankB", new Vector3(9.6f, 2.2f, 4.2f), new Vector3(1.1f, 2.2f, 1.1f), tankMetal);

            for (var index = 0; index < 4; index += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "GarageWindow_" + index, new Vector3(-7.5f, 2.8f, -9f + (index * 5f)), new Vector3(1.6f, 1.2f, 0.1f), windowGlow);
            }

            CreateLampPost(new Vector3(-4.6f, 0f, -11f), metal, windowGlow);
            CreateLampPost(new Vector3(4.6f, 0f, -4f), metal, windowGlow);
            CreateLampPost(new Vector3(4.6f, 0f, 11f), metal, windowGlow);
            CreateProductionRailYardPass(rustMetal, tankMetal, metal, windowGlow, gravel, puddle, brass);
            CreateReadyAssetUrbanCanyonPass(DistrictArtStyle.RailYard, "Rail", rustMetal, tankMetal, metal, windowGlow, sign, gravel, puddle, brass);
            CreateNoirStreetDressing("Rail", metal, windowGlow, crateWood, sign, gravel, puddle, brass);
            CreateNoirSpawnComposition("RailStart", new Vector3(0f, 0f, -13.5f), metal, windowGlow, crateWood, sign, gravel, puddle, brass, "IRONLINE");
            CreateFinishedCityBlockPass("Rail", asphalt, rustMetal, tankMetal, metal, windowGlow, sign, gravel, puddle, brass);
            CreateDistrictRooftopDetailPass("Rail", -10.6f, 10.6f, metal, windowGlow, rustMetal, sign, brass);
            CreateRailYardIdentityPass(metal, windowGlow, rustMetal, tankMetal, crateWood, brass);
            CreateKenneyDistrictAssetPass("RailYard");
            CreateMafiaDistrictProductionPass(DistrictArtStyle.RailYard, "Rail", asphalt, rustMetal, tankMetal, metal, windowGlow, sign, gravel, puddle, brass);

            var runtime = CreateFreeRoamDistrictRuntime(
                "District_RailYard_01",
                "RailYardStart",
                new Vector3(1.2f, 1f, -9.6f),
                new Color(0.13f, 0.14f, 0.17f),
                new Vector3(0f, 17.2f, -7.4f),
                campaignDatabase,
                chapterAsset);
            ConfigurePoliceResponse(
                runtime.PoliceResponseController,
                CreatePoliceResponder("RailWatchOfficer", new Vector3(-2.8f, 1f, -7.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("RailPatrolOfficerA", new Vector3(4.6f, 1f, -2.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("RailPatrolOfficerB", new Vector3(2.4f, 1f, 6.8f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("RailHuntOfficerA", new Vector3(-6.4f, 1f, -1.2f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("RailHuntOfficerB", new Vector3(6.1f, 1f, 9.4f), policeCoat, runtime.PlayerController, runtime.PlayerHealth),
                CreatePoliceResponder("RailHuntOfficerC", new Vector3(-1.2f, 1f, 12.2f), policeCoat, runtime.PlayerController, runtime.PlayerHealth));

            var missionSystem = CreateSceneObjectiveSystem("RailYardObjectiveSystem");
            CreateSceneObjectiveHud("RailYardObjectiveHud", missionSystem, runtime.SaveGameFileService, campaignDatabase);

            CreateSpawnPoint("RailYardStartPoint", "RailYardStart", new Vector3(1.2f, 1f, -9.6f));
            CreateSpawnPoint("RailYardWatchmenSpawnPoint", "RailYardWatchmenSpawn", new Vector3(2.8f, 1f, -2.4f));
            CreateSpawnPoint("RailYardBetrayalSpawnPoint", "RailYardBetrayalSpawn", new Vector3(-1.6f, 1f, 8.4f));
            CreateSpawnPoint("RailYardExitSpawnPoint", "RailYardExitSpawn", new Vector3(4.8f, 1f, -10.8f));
            CreateSpawnPoint("FromDocksGatePoint", "FromDocksGate", new Vector3(0f, 1f, -12.2f));
            CreateSpawnPoint("FromOldQuarterGatePoint", "FromOldQuarterGate", new Vector3(-2.6f, 1f, -12.2f));
            CreateSpawnPoint("FromGarageInteriorPoint", "FromGarageInterior", new Vector3(-5.2f, 1f, -6.8f));
            CreateSpawnPoint("FromDispatchInteriorPoint", "FromDispatchInterior", new Vector3(-5.15f, 1f, 9.2f));
            CreateSpawnPoint("FromLockerInteriorPoint", "FromLockerInterior", new Vector3(5.35f, 1f, -1.4f));

            CreateTravelGate(
                "ToDocksFromRailGate",
                new Vector3(0f, 1.1f, -15.2f),
                new Vector3(6f, 2.2f, 0.4f),
                sign,
                "Head back to the harbor",
                "docks",
                "District_01",
                "FromRailYardGate",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService);

            CreatePrimitive(
                PrimitiveType.Cube,
                "IronlineOfficeApron",
                new Vector3(-5.55f, 0.14f, -6.8f),
                new Vector3(1.9f, 0.08f, 2.2f),
                asphalt);

            var garageInteriorDoor = CreatePrimitive(
                PrimitiveType.Cube,
                "IronlineOfficeDoor",
                new Vector3(-6.85f, 1.1f, -6.8f),
                new Vector3(0.35f, 2.2f, 1.9f),
                sign);
            var garageInteriorInteractable = garageInteriorDoor.AddComponent<SceneDoorInteractable>();
            SetObjectReference(garageInteriorInteractable, "sceneTransitionController", runtime.SceneTransitionController);
            SetStringValue(garageInteriorInteractable, "promptText", "Enter garage office");
            SetStringValue(garageInteriorInteractable, "sourceScene", "District_RailYard_01");
            SetStringValue(garageInteriorInteractable, "targetScene", "Interior_RailYard_Garage_01");
            SetStringValue(garageInteriorInteractable, "spawnPointId", "GarageInteriorSpawn");

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "DispatchApron",
                new Vector3(-5.75f, 0.145f, 9.2f),
                new Vector3(1.9f, 0.08f, 2.15f),
                gravel);
            CreateFreeRoamSceneDoor(
                "DispatchOfficeDoor",
                new Vector3(-6.85f, 1.1f, 9.2f),
                new Vector3(0.35f, 2.2f, 1.85f),
                sign,
                "Enter dispatch office",
                "District_RailYard_01",
                "Interior_RailYard_Dispatch_01",
                "DispatchInteriorSpawn",
                runtime.SceneTransitionController);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "LockerRoomApron",
                new Vector3(5.75f, 0.145f, -1.4f),
                new Vector3(1.9f, 0.08f, 2.15f),
                gravel);
            CreateFreeRoamSceneDoor(
                "LockerRoomDoor",
                new Vector3(6.85f, 1.1f, -1.4f),
                new Vector3(0.35f, 2.2f, 1.85f),
                sign,
                "Enter yard locker room",
                "District_RailYard_01",
                "Interior_RailYard_LockerRoom_01",
                "LockerInteriorSpawn",
                runtime.SceneTransitionController);

            var yardMissionRoot = new GameObject("YardHeatMissionRoot");
            var yardGuardA = CreateEnemyGuard("RailYardGuardA", new Vector3(-3.1f, 1f, -2.4f), enemyCoat, runtime.PlayerController, runtime.PlayerHealth, null, yardMissionRoot.transform);
            var yardGuardB = CreateEnemyGuard("RailYardGuardB", new Vector3(3.6f, 1f, 7.8f), enemyCoat, runtime.PlayerController, runtime.PlayerHealth, null, yardMissionRoot.transform);

            var garageTrigger = new GameObject("GarageEntryTrigger");
            garageTrigger.transform.SetParent(yardMissionRoot.transform, false);
            garageTrigger.transform.position = new Vector3(-6.5f, 1f, -2f);
            var garageCollider = garageTrigger.AddComponent<BoxCollider>();
            garageCollider.isTrigger = true;
            garageCollider.size = new Vector3(5.2f, 2f, 9f);
            var garageZone = garageTrigger.AddComponent<ObjectiveTriggerZone>();
            SetObjectReference(garageZone, "objectiveSystem", missionSystem);
            SetStringValue(garageZone, "requiredObjectiveId", "check-garage");
            SetObjectReference(garageZone, "campaignProgressionController", runtime.CampaignProgressionController);
            SetObjectReference(garageZone, "missionAsset", yardHeatMission);
            SetStringValue(garageZone, "missionStageIdToComplete", "check-garage");

            var cleanupObjective = new GameObject("ClearWatchmenObjective");
            cleanupObjective.transform.SetParent(yardMissionRoot.transform, false);
            var eliminateTargets = cleanupObjective.AddComponent<EliminateTargetsObjective>();
            SetObjectReference(eliminateTargets, "objectiveSystem", missionSystem);
            SetStringValue(eliminateTargets, "requiredObjectiveId", "clear-watchmen");
            SetObjectReference(eliminateTargets, "campaignProgressionController", runtime.CampaignProgressionController);
            SetObjectReference(eliminateTargets, "missionAsset", yardHeatMission);
            SetStringValue(eliminateTargets, "missionStageIdToComplete", "clear-watchmen");
            ConfigureEliminateTargets(eliminateTargets, yardGuardA, yardGuardB);

            var railYardMissionDirectorObject = new GameObject("RailYardMissionDirector");
            var railYardMissionDirector = railYardMissionDirectorObject.AddComponent<CampaignMissionSceneDirector>();
            ConfigureMissionSceneDirector(
                railYardMissionDirector,
                runtime.SaveGameFileService,
                missionSystem,
                (yardHeatMission, yardMissionRoot, new[]
                {
                    ("check-garage", "Check the garage", "Reach the Ironline garage and inspect the takeover point.", "clear-watchmen"),
                    ("clear-watchmen", "Clear the watchmen", "Take down the yard crew holding the garage.", null)
                }),
                (yardBetrayalMission, CreateYardBetrayalMissionRoot(runtime, missionSystem, yardBetrayalMission, sign, rustMetal), new[]
                {
                    ("inspect-yard", "Inspect the yard handoff", "Reach the betrayal marker in the yard.", "leave-yard"),
                    ("leave-yard", "Leave the yard", "Get out before the trains and traitors close around you.", null)
                }));

            CreateStaticVehicle("GarageTruck", new Vector3(-5.2f, 0.8f, -8.4f), new Vector3(2.4f, 1.2f, 5f), new Color(0.16f, 0.18f, 0.19f), tankMetal, windowGlow);

            var chopDeliveryStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chopDeliveryStart.name = "ChopDeliveryStart";
            chopDeliveryStart.transform.position = new Vector3(5.8f, 1f, -8.6f);
            chopDeliveryStart.transform.localScale = new Vector3(1.4f, 1.5f, 1.4f);
            AssignMaterial(chopDeliveryStart, sign);
            var chopDeliveryStartInteractable = chopDeliveryStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(chopDeliveryStartInteractable, "activityAsset", chopDeliveryActivity);
            SetObjectReference(chopDeliveryStartInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(chopDeliveryStartInteractable, "promptText", "Take chop delivery");

            var garagePrepStart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            garagePrepStart.name = "GaragePrepStart";
            garagePrepStart.transform.position = new Vector3(5.7f, 1f, -1.4f);
            garagePrepStart.transform.localScale = new Vector3(1.35f, 1.5f, 1.35f);
            AssignMaterial(garagePrepStart, sign);
            var garagePrepStartInteractable = garagePrepStart.AddComponent<ActivityStartInteractable>();
            SetObjectReference(garagePrepStartInteractable, "activityAsset", garagePrepActivity);
            SetObjectReference(garagePrepStartInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(garagePrepStartInteractable, "promptText", "Take garage prep job");

            var garagePrepComplete = GameObject.CreatePrimitive(PrimitiveType.Cube);
            garagePrepComplete.name = "GaragePrepComplete";
            garagePrepComplete.transform.position = new Vector3(-4.9f, 1f, -8.4f);
            garagePrepComplete.transform.localScale = new Vector3(1.15f, 1.4f, 1.15f);
            AssignMaterial(garagePrepComplete, brass);
            var garagePrepCompleteInteractable = garagePrepComplete.AddComponent<ActivityCompleteInteractable>();
            SetObjectReference(garagePrepCompleteInteractable, "activityAsset", garagePrepActivity);
            SetObjectReference(garagePrepCompleteInteractable, "activityProgressionController", runtime.ActivityProgressionController);
            SetStringValue(garagePrepCompleteInteractable, "promptText", "Finish garage prep");

            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Game/Scenes/Boot.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/District_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/District_BusinessCore_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/District_OldQuarter_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/District_RailYard_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_BusinessBookkeeper_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_Business_PrintShop_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_OldQuarter_Chapel_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_OldQuarter_Tenement_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_RailYard_Garage_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_RailYard_Dispatch_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_Docks_Warehouse_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_Docks_SocialClub_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_Business_UnionHall_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_OldQuarter_Diner_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_RailYard_LockerRoom_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_BackOffice_01.unity", true)
            };
        }

        private static void EnsureFolder(string assetPath)
        {
            var parts = assetPath.Split('/');
            var current = parts[0];

            for (var index = 1; index < parts.Length; index += 1)
            {
                var next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private static void ClearScene(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static Scene OpenOrCreateScene(string path)
        {
            if (System.IO.File.Exists(path))
            {
                return EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                EnsureFolder(directory.Replace('\\', '/'));
            }

            EditorSceneManager.SaveScene(scene, path);
            return scene;
        }

        private static void ConfigureObjectives(SimpleObjectiveSystem system, params (string Id, string Title, string Condition, string NextId)[] steps)
        {
            var serializedObject = new SerializedObject(system);
            serializedObject.FindProperty("startingObjectiveId").stringValue = steps[0].Id;

            var stepsProperty = serializedObject.FindProperty("steps");
            stepsProperty.arraySize = steps.Length;

            for (var index = 0; index < steps.Length; index += 1)
            {
                var element = stepsProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("Id").stringValue = steps[index].Id;
                element.FindPropertyRelative("Title").stringValue = steps[index].Title;
                element.FindPropertyRelative("SuccessCondition").stringValue = steps[index].Condition;
                element.FindPropertyRelative("NextObjectiveId").stringValue = steps[index].NextId ?? string.Empty;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetStringValue(UnityEngine.Object target, string propertyName, string value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector3Value(UnityEngine.Object target, string propertyName, Vector3 value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).vector3Value = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloatValue(UnityEngine.Object target, string propertyName, float value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetColorValue(UnityEngine.Object target, string propertyName, Color value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).colorValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetIntValue(UnityEngine.Object target, string propertyName, int value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBoolValue(UnityEngine.Object target, string propertyName, bool value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray(SerializedProperty property, UnityEngine.Object[] values)
        {
            property.arraySize = values.Length;
            for (var index = 0; index < values.Length; index += 1)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
        }

        private static SimpleObjectiveSystem CreateSceneObjectiveSystem(string name)
        {
            var objectiveRoot = new GameObject(name);
            return objectiveRoot.AddComponent<SimpleObjectiveSystem>();
        }

        private static void CreateSceneObjectiveHud(
            string name,
            SimpleObjectiveSystem objectiveSystem,
            SaveGameFileService? saveGameFileService = null,
            CampaignDatabaseAsset? campaignDatabase = null)
        {
            var hudRoot = new GameObject(name);
            var hud = hudRoot.AddComponent<ObjectiveHudController>();
            SetObjectReference(hud, "objectiveSystem", objectiveSystem);
            if (saveGameFileService != null)
            {
                SetObjectReference(hud, "saveGameFileService", saveGameFileService);
            }

            if (campaignDatabase != null)
            {
                SetObjectReference(hud, "campaignDatabase", campaignDatabase);
            }
        }

        private static void ConfigureMissionSceneDirector(
            CampaignMissionSceneDirector director,
            SaveGameFileService saveGameFileService,
            SimpleObjectiveSystem objectiveSystem,
            params (MissionDefinitionAsset MissionAsset, GameObject Root, (string Id, string Title, string Condition, string? NextId)[] Steps)[] bindings)
        {
            var serializedObject = new SerializedObject(director);
            serializedObject.FindProperty("saveGameFileService").objectReferenceValue = saveGameFileService;
            serializedObject.FindProperty("objectiveSystem").objectReferenceValue = objectiveSystem;

            var bindingsProperty = serializedObject.FindProperty("missionBindings");
            bindingsProperty.arraySize = bindings.Length;

            for (var bindingIndex = 0; bindingIndex < bindings.Length; bindingIndex += 1)
            {
                var bindingProperty = bindingsProperty.GetArrayElementAtIndex(bindingIndex);
                bindingProperty.FindPropertyRelative("MissionAsset").objectReferenceValue = bindings[bindingIndex].MissionAsset;
                bindingProperty.FindPropertyRelative("Root").objectReferenceValue = bindings[bindingIndex].Root;

                var stepsProperty = bindingProperty.FindPropertyRelative("ObjectiveSteps");
                stepsProperty.arraySize = bindings[bindingIndex].Steps.Length;

                for (var stepIndex = 0; stepIndex < bindings[bindingIndex].Steps.Length; stepIndex += 1)
                {
                    var stepProperty = stepsProperty.GetArrayElementAtIndex(stepIndex);
                    stepProperty.FindPropertyRelative("Id").stringValue = bindings[bindingIndex].Steps[stepIndex].Id;
                    stepProperty.FindPropertyRelative("Title").stringValue = bindings[bindingIndex].Steps[stepIndex].Title;
                    stepProperty.FindPropertyRelative("SuccessCondition").stringValue = bindings[bindingIndex].Steps[stepIndex].Condition;
                    stepProperty.FindPropertyRelative("NextObjectiveId").stringValue = bindings[bindingIndex].Steps[stepIndex].NextId ?? string.Empty;
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureEliminateTargets(EliminateTargetsObjective objective, params CombatHealth[] targets)
        {
            var serializedObject = new SerializedObject(objective);
            var targetsProperty = serializedObject.FindProperty("targets");
            targetsProperty.arraySize = targets.Length;

            for (var index = 0; index < targets.Length; index += 1)
            {
                targetsProperty.GetArrayElementAtIndex(index).objectReferenceValue = targets[index];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static InteriorRuntimeBundle CreateFreeRoamInteriorRuntime(
            string sceneName,
            string defaultSpawnPointId,
            Vector3 playerPosition,
            Color backgroundColor,
            Vector3 cameraOffset,
            CampaignDatabaseAsset campaignDatabase,
            CampaignChapterAsset bootstrapChapter)
        {
            var player = CreateNoirActor(
                "InteriorPlayer",
                playerPosition,
                GetOrCreateMaterial("Assets/Game/Materials/PlayerCoat.mat", new Color(0.16f, 0.145f, 0.12f), 0.32f, 0f),
                null,
                false);
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            var playerController = player.AddComponent<TopDownPlayerController>();
            var interactionController = player.AddComponent<PlayerInteractionController>();
            var playerHealth = player.AddComponent<CombatHealth>();
            var playerCombatController = player.AddComponent<PlayerCombatController>();
            SetObjectReference(playerController, "characterController", characterController);
            SetObjectReference(interactionController, "playerController", playerController);
            SetStringValue(playerHealth, "displayName", "Tommy");
            SetIntValue(playerHealth, "maximumHealth", 3);
            SetBoolValue(playerHealth, "destroyOnDeath", false);
            SetBoolValue(playerHealth, "reloadSceneOnDeath", true);

            var systems = new GameObject("InteriorSystems");
            var saveGameFileService = systems.AddComponent<SaveGameFileService>();
            SetStringValue(saveGameFileService, "defaultSceneName", sceneName);
            SetStringValue(saveGameFileService, "defaultSpawnPointId", defaultSpawnPointId);
            var heatSystemController = systems.AddComponent<HeatSystemController>();
            SetObjectReference(heatSystemController, "saveGameFileService", saveGameFileService);
            var campaignProgressionController = systems.AddComponent<CampaignProgressionController>();
            SetObjectReference(campaignProgressionController, "campaignDatabase", campaignDatabase);
            SetObjectReference(campaignProgressionController, "saveGameFileService", saveGameFileService);
            var sceneBootstrapper = systems.AddComponent<CampaignSceneBootstrapper>();
            SetObjectReference(sceneBootstrapper, "campaignProgressionController", campaignProgressionController);
            if (bootstrapChapter != null)
            {
                SetObjectReference(sceneBootstrapper, "bootstrapChapter", bootstrapChapter);
            }

            var sceneSpawnController = systems.AddComponent<SceneSpawnController>();
            SetObjectReference(sceneSpawnController, "saveGameFileService", saveGameFileService);
            SetObjectReference(sceneSpawnController, "playerController", playerController);
            SetStringValue(sceneSpawnController, "defaultSpawnPointId", defaultSpawnPointId);
            var dialogueController = systems.AddComponent<DialogueController>();
            SetObjectReference(dialogueController, "playerController", playerController);

            var cameraRoot = new GameObject("InteriorCameraRig");
            var camera = cameraRoot.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = backgroundColor;
            camera.orthographic = false;
            camera.fieldOfView = 46f;
            var tunedInteriorCameraOffset = new Vector3(cameraOffset.x, Mathf.Min(cameraOffset.y, 10.8f), -6.8f);
            cameraRoot.transform.position = playerPosition + tunedInteriorCameraOffset;
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            ConfigureNoirCamera(camera, topDownCamera, tunedInteriorCameraOffset, 60f, 0f, 8.5f, 15f);
            CreateNoirPostProcessVolume(sceneName, new Color(1f, 0.78f, 0.55f), -0.28f, 36f, -12f, 1.6f, 0.4f);

            var transitionRoot = new GameObject("SceneTransitionController");
            var transitionController = transitionRoot.AddComponent<SceneTransitionController>();
            SetObjectReference(transitionController, "saveGameFileService", saveGameFileService);

            var statusHudObject = new GameObject("InteriorStatusHud");
            var statusHud = statusHudObject.AddComponent<StatusHudController>();
            SetObjectReference(statusHud, "playerHealth", playerHealth);
            SetObjectReference(statusHud, "heatSystemController", heatSystemController);
            var pauseMenuObject = new GameObject("InteriorPauseMenu");
            var pauseMenu = pauseMenuObject.AddComponent<PauseMenuController>();
            SetObjectReference(pauseMenu, "playerController", playerController);
            SetObjectReference(pauseMenu, "dialogueController", dialogueController);

            SetObjectReference(playerCombatController, "playerController", playerController);
            SetObjectReference(playerCombatController, "combatHealth", playerHealth);
            SetObjectReference(playerCombatController, "heatSystemController", heatSystemController);

            return new InteriorRuntimeBundle
            {
                PlayerController = playerController,
                PlayerHealth = playerHealth,
                HeatSystemController = heatSystemController,
                SaveGameFileService = saveGameFileService,
                CampaignProgressionController = campaignProgressionController,
                SceneTransitionController = transitionController,
                DialogueController = dialogueController
            };
        }

        private static ExteriorRuntimeBundle CreateFreeRoamDistrictRuntime(
            string sceneName,
            string defaultSpawnPointId,
            Vector3 playerPosition,
            Color backgroundColor,
            Vector3 cameraOffset,
            CampaignDatabaseAsset campaignDatabase,
            CampaignChapterAsset bootstrapChapter)
        {
            var player = CreateNoirActor(
                "Player",
                playerPosition,
                GetOrCreateMaterial("Assets/Game/Materials/PlayerCoat.mat", new Color(0.16f, 0.145f, 0.12f), 0.32f, 0f),
                null,
                false);
            var characterController = player.AddComponent<CharacterController>();
            characterController.height = 1.8f;
            characterController.radius = 0.35f;
            var playerController = player.AddComponent<TopDownPlayerController>();
            var interactionController = player.AddComponent<PlayerInteractionController>();
            var playerHealth = player.AddComponent<CombatHealth>();
            var playerCombatController = player.AddComponent<PlayerCombatController>();
            SetObjectReference(playerController, "characterController", characterController);
            SetObjectReference(interactionController, "playerController", playerController);
            SetStringValue(playerHealth, "displayName", "Tommy");
            SetIntValue(playerHealth, "maximumHealth", 3);
            SetBoolValue(playerHealth, "destroyOnDeath", false);
            SetBoolValue(playerHealth, "reloadSceneOnDeath", true);

            var systems = new GameObject("DistrictSystems");
            var saveGameFileService = systems.AddComponent<SaveGameFileService>();
            SetStringValue(saveGameFileService, "defaultSceneName", sceneName);
            SetStringValue(saveGameFileService, "defaultSpawnPointId", defaultSpawnPointId);
            var heatSystemController = systems.AddComponent<HeatSystemController>();
            SetObjectReference(heatSystemController, "saveGameFileService", saveGameFileService);
            var policeResponseController = systems.AddComponent<PoliceResponseController>();
            SetObjectReference(policeResponseController, "heatSystemController", heatSystemController);
            var activityProgressionController = systems.AddComponent<ActivityProgressionController>();
            SetObjectReference(activityProgressionController, "campaignDatabase", campaignDatabase);
            SetObjectReference(activityProgressionController, "saveGameFileService", saveGameFileService);
            var campaignProgressionController = systems.AddComponent<CampaignProgressionController>();
            SetObjectReference(campaignProgressionController, "campaignDatabase", campaignDatabase);
            SetObjectReference(campaignProgressionController, "saveGameFileService", saveGameFileService);
            var sceneBootstrapper = systems.AddComponent<CampaignSceneBootstrapper>();
            SetObjectReference(sceneBootstrapper, "campaignProgressionController", campaignProgressionController);
            if (bootstrapChapter != null)
            {
                SetObjectReference(sceneBootstrapper, "bootstrapChapter", bootstrapChapter);
            }

            var sceneSpawnController = systems.AddComponent<SceneSpawnController>();
            SetObjectReference(sceneSpawnController, "saveGameFileService", saveGameFileService);
            SetObjectReference(sceneSpawnController, "playerController", playerController);
            SetStringValue(sceneSpawnController, "defaultSpawnPointId", defaultSpawnPointId);
            var dialogueController = systems.AddComponent<DialogueController>();
            SetObjectReference(dialogueController, "playerController", playerController);

            var cameraRoot = new GameObject("CameraRig");
            var camera = cameraRoot.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = backgroundColor;
            camera.orthographic = false;
            camera.fieldOfView = 36f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 160f;
            var tunedDistrictCameraOffset = new Vector3(cameraOffset.x - 0.1f, Mathf.Clamp(cameraOffset.y - 6.8f, 10.4f, 12.4f), Mathf.Clamp(cameraOffset.z + 0.15f, -7.2f, -5.8f));
            cameraRoot.transform.position = playerPosition + tunedDistrictCameraOffset;
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            ConfigureNoirCamera(camera, topDownCamera, tunedDistrictCameraOffset, 52f, -2f, 8.6f, 14.2f);
            CreateNoirPostProcessVolume(sceneName, ResolveDistrictColorFilter(sceneName), 0.08f, 28f, -8f, 1.1f, 0.22f);

            var transitionRoot = new GameObject("SceneTransitionController");
            var transitionController = transitionRoot.AddComponent<SceneTransitionController>();
            SetObjectReference(transitionController, "saveGameFileService", saveGameFileService);

            var statusHudObject = new GameObject("StatusHud");
            var statusHud = statusHudObject.AddComponent<StatusHudController>();
            SetObjectReference(statusHud, "playerHealth", playerHealth);
            SetObjectReference(statusHud, "heatSystemController", heatSystemController);
            SetObjectReference(statusHud, "policeResponseController", policeResponseController);
            var activityHudObject = new GameObject("ActivityHud");
            var activityHud = activityHudObject.AddComponent<ActivityHudController>();
            SetObjectReference(activityHud, "activityProgressionController", activityProgressionController);
            var pauseMenuObject = new GameObject("PauseMenu");
            var pauseMenu = pauseMenuObject.AddComponent<PauseMenuController>();
            SetObjectReference(pauseMenu, "playerController", playerController);
            SetObjectReference(pauseMenu, "dialogueController", dialogueController);

            SetObjectReference(playerCombatController, "playerController", playerController);
            SetObjectReference(playerCombatController, "combatHealth", playerHealth);
            SetObjectReference(playerCombatController, "heatSystemController", heatSystemController);

            return new ExteriorRuntimeBundle
            {
                PlayerController = playerController,
                PlayerHealth = playerHealth,
                PlayerCombatController = playerCombatController,
                HeatSystemController = heatSystemController,
                PoliceResponseController = policeResponseController,
                ActivityProgressionController = activityProgressionController,
                SaveGameFileService = saveGameFileService,
                CampaignProgressionController = campaignProgressionController,
                SceneTransitionController = transitionController,
                DialogueController = dialogueController
            };
        }

        private static Color ResolveDistrictColorFilter(string sceneName)
        {
            if (sceneName.Contains("BusinessCore", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Color(1f, 0.9f, 0.72f);
            }

            if (sceneName.Contains("OldQuarter", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Color(1f, 0.82f, 0.68f);
            }

            if (sceneName.Contains("RailYard", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Color(0.76f, 0.86f, 1f);
            }

            return new Color(0.78f, 0.86f, 1f);
        }

        private static void ConfigureNoirCamera(
            Camera camera,
            TopDownCameraController topDownCamera,
            Vector3 offset,
            float pitch,
            float yaw,
            float minimumHeight,
            float maximumHeight)
        {
            camera.allowHDR = true;
            camera.allowMSAA = true;

            var additionalCameraData = camera.GetComponent<UniversalAdditionalCameraData>();
            if (additionalCameraData == null)
            {
                additionalCameraData = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }

            additionalCameraData.renderPostProcessing = true;
            additionalCameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
            additionalCameraData.dithering = false;
            additionalCameraData.stopNaN = true;

            // Keep gameplay readable: the noir look comes from authored lighting and geometry,
            // not screen-wide pixel/grain overlays.
            SetVector3Value(topDownCamera, "offset", offset);
            SetFloatValue(topDownCamera, "minimumHeight", minimumHeight);
            SetFloatValue(topDownCamera, "maximumHeight", maximumHeight);
            SetFloatValue(topDownCamera, "pitch", pitch);
            SetFloatValue(topDownCamera, "yaw", yaw);
        }

        private static void CreateNoirPostProcessVolume(
            string sceneName,
            Color colorFilter,
            float postExposure,
            float contrast,
            float saturation,
            float bloomIntensity,
            float vignetteIntensity)
        {
            var profile = GetOrCreateNoirVolumeProfile("Assets/Game/Data/Volumes/" + sceneName + "_NoirGrade.asset");
            var colorAdjustments = GetOrAddVolumeComponent<ColorAdjustments>(profile);
            SetVolumeParameter(colorAdjustments.postExposure, postExposure);
            SetVolumeParameter(colorAdjustments.contrast, contrast);
            SetVolumeParameter(colorAdjustments.saturation, saturation);
            SetVolumeParameter(colorAdjustments.colorFilter, colorFilter);

            var tonemapping = GetOrAddVolumeComponent<Tonemapping>(profile);
            SetVolumeParameter(tonemapping.mode, TonemappingMode.ACES);

            var bloom = GetOrAddVolumeComponent<Bloom>(profile);
            SetVolumeParameter(bloom.threshold, 0.52f);
            SetVolumeParameter(bloom.intensity, bloomIntensity);
            SetVolumeParameter(bloom.scatter, 0.74f);
            SetVolumeParameter(bloom.tint, new Color(1f, 0.72f, 0.38f));

            var vignette = GetOrAddVolumeComponent<Vignette>(profile);
            SetVolumeParameter(vignette.intensity, Mathf.Min(vignetteIntensity, 0.16f));
            SetVolumeParameter(vignette.smoothness, 0.66f);

            var grain = GetOrAddVolumeComponent<FilmGrain>(profile);
            grain.active = false;
            SetVolumeParameter(grain.type, FilmGrainLookup.Thin1);
            SetVolumeParameter(grain.intensity, 0f);
            SetVolumeParameter(grain.response, 0.72f);

            EditorUtility.SetDirty(profile);

            var volumeObject = new GameObject(sceneName + "_NoirGradeVolume");
            var volume = volumeObject.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 50f;
            volume.sharedProfile = profile;
        }

        private static VolumeProfile GetOrCreateNoirVolumeProfile(string assetPath)
        {
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(assetPath);
            if (profile != null)
            {
                return profile;
            }

            var directory = System.IO.Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                EnsureFolder(directory.Replace('\\', '/'));
            }

            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            profile.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            AssetDatabase.CreateAsset(profile, assetPath);
            return profile;
        }

        private static T GetOrAddVolumeComponent<T>(VolumeProfile profile)
            where T : VolumeComponent
        {
            if (!profile.TryGet<T>(out var component))
            {
                component = profile.Add<T>(true);
            }

            component.active = true;
            return component;
        }

        private static void SetVolumeParameter<T>(VolumeParameter<T> parameter, T value)
        {
            parameter.overrideState = true;
            parameter.value = value;
        }

        private static GameObject CreatePrimitive(PrimitiveType primitiveType, string name, Vector3 position, Vector3 scale, Material material)
        {
            var gameObject = GameObject.CreatePrimitive(primitiveType);
            gameObject.name = name;
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;
            AssignMaterial(gameObject, material);
            return gameObject;
        }

        private static void AssignMaterial(GameObject gameObject, Material material)
        {
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static void HideRenderer(GameObject gameObject)
        {
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        private static GameObject CreateVisualPrimitive(PrimitiveType primitiveType, string name, Vector3 position, Vector3 scale, Material material)
        {
            var gameObject = CreatePrimitive(primitiveType, name, position, scale, material);
            var collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }

            return gameObject;
        }

        private static GameObject CreateColliderBlock(string name, Vector3 position, Vector3 scale)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = name;
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;

            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                UnityEngine.Object.DestroyImmediate(renderer);
            }

            return gameObject;
        }

        private static void CreateKenneyDistrictAssetPass(string districtKey)
        {
            var root = new GameObject("KenneyAssetDressing_" + districtKey);

            switch (districtKey)
            {
                case "Docks":
                    CreateKenneyModel("CityKitIndustrial", "detail-tank", "KenneyDockFuelTank", new Vector3(-13.5f, 0.04f, 13.2f), Vector3.zero, new Vector3(1.35f, 1.35f, 1.35f), root.transform);
                    CreateKenneyModel("CityKitRoads", "construction-barrier", "KenneyDockBarrierA", new Vector3(3.35f, 0.08f, -13.5f), Vector3.zero, new Vector3(1.25f, 1.25f, 1.25f), root.transform);
                    CreateKenneyModel("CityKitRoads", "construction-cone", "KenneyDockConeA", new Vector3(4.6f, 0.08f, -12.8f), Vector3.zero, new Vector3(1.05f, 1.05f, 1.05f), root.transform);
                    CreateKenneyModel("CityKitRoads", "construction-light", "KenneyDockWorkLight", new Vector3(-4.8f, 0.08f, 15.6f), new Vector3(0f, 45f, 0f), new Vector3(1.15f, 1.15f, 1.15f), root.transform);
                    CreateKenneyModel("CityKitRoads", "light-curved", "KenneyDockStreetLightA", new Vector3(-4.85f, 0.08f, -8.9f), new Vector3(0f, 90f, 0f), new Vector3(1.25f, 1.25f, 1.25f), root.transform);
                    CreateKenneyModel("CityKitRoads", "light-curved", "KenneyDockStreetLightB", new Vector3(4.85f, 0.08f, 8.6f), new Vector3(0f, -90f, 0f), new Vector3(1.25f, 1.25f, 1.25f), root.transform);
                    break;

                case "Business":
                    CreateKenneyModel("CityKitCommercial", "building-skyscraper-a", "KenneyBusinessTowerA", new Vector3(-10.5f, 0.08f, 0.8f), new Vector3(0f, 90f, 0f), new Vector3(2.2f, 2.35f, 2.2f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "building-d", "KenneyBusinessBlockA", new Vector3(10.2f, 0.08f, -9.2f), new Vector3(0f, -90f, 0f), new Vector3(2.45f, 2.3f, 2.45f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "building-h", "KenneyBusinessBlockB", new Vector3(10.2f, 0.08f, 6.8f), new Vector3(0f, -90f, 0f), new Vector3(2.3f, 2.25f, 2.3f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "building-skyscraper-c", "KenneyBusinessTowerB", new Vector3(-11.9f, 0.08f, 12.4f), new Vector3(0f, 90f, 0f), new Vector3(1.95f, 2.1f, 1.95f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "building-k", "KenneyBusinessCornerHotel", new Vector3(11.4f, 0.08f, 15.2f), new Vector3(0f, -90f, 0f), new Vector3(2.05f, 2f, 2.05f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "low-detail-building-wide-a", "KenneyBusinessBackgroundBlock", new Vector3(-12.2f, 0.08f, -13.8f), new Vector3(0f, 90f, 0f), new Vector3(2.3f, 2.1f, 2.3f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "detail-awning-wide", "KenneyBelloriAwning", new Vector3(6.95f, 1.85f, -3.2f), new Vector3(0f, -90f, 0f), new Vector3(2.2f, 2.2f, 2.2f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "detail-parasol-a", "KenneySquareParasolA", new Vector3(-3.9f, 0.08f, 12.2f), Vector3.zero, new Vector3(1.65f, 1.65f, 1.65f), root.transform);
                    CreateKenneyModel("CityKitRoads", "light-square-double", "KenneyBusinessLightA", new Vector3(-4.5f, 0.08f, -6.2f), Vector3.zero, new Vector3(1.8f, 1.8f, 1.8f), root.transform);
                    CreateKenneyModel("CityKitRoads", "sign-highway-detailed", "KenneyBusinessWayfinding", new Vector3(4.7f, 0.08f, -15f), new Vector3(0f, 180f, 0f), new Vector3(1.65f, 1.65f, 1.65f), root.transform);
                    break;

                case "OldQuarter":
                    CreateKenneyModel("CityKitSuburban", "building-type-g", "KenneyQuarterTenementA", new Vector3(-9.1f, 0.08f, -4.4f), new Vector3(0f, 90f, 0f), new Vector3(2.35f, 2.2f, 2.35f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "building-type-r", "KenneyQuarterTenementB", new Vector3(9.1f, 0.08f, 5.4f), new Vector3(0f, -90f, 0f), new Vector3(2.35f, 2.2f, 2.35f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "building-type-l", "KenneyQuarterRowHouseA", new Vector3(-9.6f, 0.08f, 10.8f), new Vector3(0f, 90f, 0f), new Vector3(2.15f, 2.05f, 2.15f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "building-type-q", "KenneyQuarterRowHouseB", new Vector3(9.8f, 0.08f, -10.4f), new Vector3(0f, -90f, 0f), new Vector3(2.12f, 2.02f, 2.12f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "fence-1x4", "KenneyChapelFenceA", new Vector3(-5.8f, 0.08f, 12.2f), new Vector3(0f, 90f, 0f), new Vector3(2f, 2f, 2f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "fence-1x3", "KenneyChapelFenceB", new Vector3(-5.8f, 0.08f, 14.6f), new Vector3(0f, 90f, 0f), new Vector3(2f, 2f, 2f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "planter", "KenneyQuarterPlanterA", new Vector3(4.5f, 0.08f, -6.4f), Vector3.zero, new Vector3(1.9f, 1.9f, 1.9f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "planter", "KenneyQuarterPlanterB", new Vector3(-4.8f, 0.08f, 7.6f), Vector3.zero, new Vector3(1.75f, 1.75f, 1.75f), root.transform);
                    CreateKenneyModel("CityKitSuburban", "tree-small", "KenneyQuarterTreeA", new Vector3(6.2f, 0.08f, -9.8f), Vector3.zero, new Vector3(1.9f, 1.9f, 1.9f), root.transform);
                    CreateKenneyModel("CityKitCommercial", "detail-awning", "KenneyQuarterShopAwning", new Vector3(6.95f, 1.75f, -2.2f), new Vector3(0f, -90f, 0f), new Vector3(2f, 2f, 2f), root.transform);
                    CreateKenneyModel("CityKitRoads", "light-square", "KenneyQuarterLightA", new Vector3(-4.2f, 0.08f, -4.2f), Vector3.zero, new Vector3(1.75f, 1.75f, 1.75f), root.transform);
                    break;

                case "RailYard":
                    CreateKenneyModel("CityKitIndustrial", "building-a", "KenneyRailFactoryA", new Vector3(-9.7f, 0.08f, -3.4f), new Vector3(0f, 90f, 0f), new Vector3(2.6f, 2.35f, 2.6f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "building-q", "KenneyRailFactoryB", new Vector3(9.6f, 0.08f, 3.8f), new Vector3(0f, -90f, 0f), new Vector3(2.55f, 2.35f, 2.55f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "building-p", "KenneyRailFoundry", new Vector3(-10.8f, 0.08f, 12.4f), new Vector3(0f, 90f, 0f), new Vector3(2.25f, 2.15f, 2.25f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "building-t", "KenneyRailWarehouse", new Vector3(10.8f, 0.08f, -11.2f), new Vector3(0f, -90f, 0f), new Vector3(2.2f, 2.05f, 2.2f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "chimney-large", "KenneyRailChimneyA", new Vector3(-8.8f, 0.08f, 7.8f), Vector3.zero, new Vector3(2.1f, 2.1f, 2.1f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "chimney-medium", "KenneyRailChimneyB", new Vector3(8.2f, 0.08f, 8.9f), Vector3.zero, new Vector3(2f, 2f, 2f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "detail-tank", "KenneyRailTankA", new Vector3(8.6f, 0.08f, -5.8f), Vector3.zero, new Vector3(2.2f, 2.2f, 2.2f), root.transform);
                    CreateKenneyModel("CityKitIndustrial", "detail-tank", "KenneyRailTankB", new Vector3(-8.4f, 0.08f, -9.4f), Vector3.zero, new Vector3(1.85f, 1.85f, 1.85f), root.transform);
                    CreateKenneyModel("CityKitRoads", "construction-barrier", "KenneyRailBarrierA", new Vector3(5.2f, 0.08f, -11.8f), Vector3.zero, new Vector3(2.1f, 2.1f, 2.1f), root.transform);
                    CreateKenneyModel("CityKitRoads", "light-curved-double", "KenneyRailLightA", new Vector3(-4.7f, 0.08f, -10.8f), new Vector3(0f, 90f, 0f), new Vector3(1.85f, 1.85f, 1.85f), root.transform);
                    break;
            }
        }

        private static GameObject? CreateKenneyModel(
            string packName,
            string modelName,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform? parent = null)
        {
            if (packName == "CityKitRoads" && modelName.Contains("light", System.StringComparison.OrdinalIgnoreCase))
            {
                return CreateLowProfileStreetLamp(instanceName, position, parent);
            }

            var assetPath = "Assets/ThirdParty/Kenney/" + packName + "/Models/FBX format/" + modelName + ".fbx";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (asset == null)
            {
                return null;
            }

            var instance = PrefabUtility.InstantiatePrefab(asset) as GameObject;
            if (instance == null)
            {
                instance = UnityEngine.Object.Instantiate(asset);
            }

            instance.name = instanceName;
            if (parent != null)
            {
                instance.transform.SetParent(parent, true);
            }

            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(rotationEuler);
            instance.transform.localScale = scale;
            RemoveCollidersRecursive(instance);
            EnvironmentArtCatalog.ApplyNoirMaterials(instance, assetPath);

            return instance;
        }

        private static GameObject CreateLowProfileStreetLamp(string name, Vector3 basePosition, Transform? parent)
        {
            var root = new GameObject(name);
            if (parent != null)
            {
                root.transform.SetParent(parent, true);
            }

            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.035f, 0.039f, 0.041f), 0.3f, 0f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.42f, 0.29f, 0.12f), 0.28f, 0f);
            var glow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(0.62f, 0.42f, 0.24f), 0.2f, 0f);

            CreateVisualPrimitive(PrimitiveType.Cylinder, name + "_PavementBase", basePosition + new Vector3(0f, 0.16f, 0f), new Vector3(0.22f, 0.045f, 0.22f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_Shade", basePosition + new Vector3(0f, 0.48f, 0.12f), new Vector3(0.34f, 0.10f, 0.3f), brass).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_WarmGlass", basePosition + new Vector3(0f, 0.38f, 0.12f), new Vector3(0.24f, 0.1f, 0.22f), glow).transform.SetParent(root.transform, true);

            var lightObject = new GameObject(name + "_PointLight");
            lightObject.transform.SetParent(root.transform, true);
            lightObject.transform.position = basePosition + new Vector3(0f, 0.48f, 0.12f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 2.8f;
            light.intensity = 0.5f;
            light.color = new Color(1f, 0.6f, 0.32f);
            light.shadows = LightShadows.None;

            return root;
        }

        private static void RemoveCollidersRecursive(GameObject gameObject)
        {
            var colliders = gameObject.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }
        }

        private static GameObject CreateNoirActor(string name, Vector3 position, Material coatMaterial, Transform? parent = null, bool addInteractionCollider = true)
        {
            var root = new GameObject(name);
            root.transform.position = position;
            if (parent != null)
            {
                root.transform.SetParent(parent, true);
            }

            var hat = GetOrCreateMaterial("Assets/Game/Materials/FedoraFelt.mat", new Color(0.045f, 0.04f, 0.035f), 0.36f, 0f);
            var skin = GetOrCreateMaterial("Assets/Game/Materials/FaceWarm.mat", new Color(0.30f, 0.21f, 0.15f), 0.22f, 0f);
            var shirt = GetOrCreateMaterial("Assets/Game/Materials/ShirtIvory.mat", new Color(0.24f, 0.21f, 0.16f), 0.24f, 0f);
            var shadow = GetOrCreateMaterial("Assets/Game/Materials/ActorShadow.mat", new Color(0.005f, 0.006f, 0.008f, 0.72f), 0.04f, 0f);
            var ground = new Vector3(position.x, position.y - 1f, position.z);

            CreateVisualPrimitive(PrimitiveType.Cylinder, name + "_Shadow", ground + new Vector3(0.06f, 0.035f, -0.06f), new Vector3(0.38f, 0.02f, 0.56f), shadow).transform.SetParent(root.transform, true);
            var readyActor = CreateReadyActorModel(name, ground, root.transform, coatMaterial, skin, shirt);
            if (readyActor != null)
            {
                CreateVisualPrimitive(PrimitiveType.Sphere, name + "_FaceRead", ground + new Vector3(0f, 1.34f, -0.08f), new Vector3(0.23f, 0.2f, 0.2f), skin).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cylinder, name + "_HatBrim", ground + new Vector3(0f, 1.42f, 0f), new Vector3(0.34f, 0.035f, 0.34f), hat).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cylinder, name + "_HatCrown", ground + new Vector3(0f, 1.53f, 0f), new Vector3(0.22f, 0.12f, 0.22f), hat).transform.SetParent(root.transform, true);

                if (addInteractionCollider)
                {
                    var capsule = root.AddComponent<CapsuleCollider>();
                    capsule.height = 1.8f;
                    capsule.radius = 0.42f;
                    capsule.center = new Vector3(0f, 0.9f, 0f);
                }

                return root;
            }

            CreateVisualPrimitive(PrimitiveType.Cube, name + "_LeftShoe", ground + new Vector3(-0.09f, 0.15f, 0.04f), new Vector3(0.08f, 0.16f, 0.18f), hat).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_RightShoe", ground + new Vector3(0.09f, 0.15f, 0.04f), new Vector3(0.08f, 0.16f, 0.18f), hat).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_LeftTrouser", ground + new Vector3(-0.08f, 0.38f, 0.02f), new Vector3(0.08f, 0.45f, 0.11f), coatMaterial).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_RightTrouser", ground + new Vector3(0.08f, 0.38f, 0.02f), new Vector3(0.08f, 0.45f, 0.11f), coatMaterial).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_LongCoat", ground + new Vector3(0f, 0.7f, -0.02f), new Vector3(0.28f, 0.72f, 0.2f), coatMaterial).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_CoatLapels", ground + new Vector3(0f, 0.93f, -0.13f), new Vector3(0.16f, 0.25f, 0.04f), shirt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_ShoulderLine", ground + new Vector3(0f, 0.99f, -0.01f), new Vector3(0.42f, 0.08f, 0.2f), coatMaterial).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_LeftSleeve", ground + new Vector3(-0.24f, 0.73f, -0.01f), new Vector3(0.08f, 0.47f, 0.09f), coatMaterial).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_RightSleeve", ground + new Vector3(0.24f, 0.73f, -0.01f), new Vector3(0.08f, 0.47f, 0.09f), coatMaterial).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_Head", ground + new Vector3(0f, 1.18f, -0.01f), new Vector3(0.16f, 0.16f, 0.15f), skin).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_FedoraBrim", ground + new Vector3(0f, 1.27f, -0.03f), new Vector3(0.25f, 0.028f, 0.18f), hat).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_FedoraCrown", ground + new Vector3(0f, 1.34f, -0.01f), new Vector3(0.15f, 0.075f, 0.13f), hat).transform.SetParent(root.transform, true);

            if (addInteractionCollider)
            {
                var capsule = root.AddComponent<CapsuleCollider>();
                capsule.height = 1.8f;
                capsule.radius = 0.42f;
                capsule.center = new Vector3(0f, 0.9f, 0f);
            }

            return root;
        }

        private static GameObject? CreateReadyActorModel(
            string actorName,
            Vector3 groundPosition,
            Transform parent,
            Material coatMaterial,
            Material skinMaterial,
            Material shirtMaterial)
        {
            // The imported blocky kit reads worse than the authored noir silhouette at gameplay scale.
            var useBlockyPlaceholderMesh = false;
            if (!useBlockyPlaceholderMesh)
            {
                return null;
            }

            var modelName = ResolveReadyActorModel(actorName);
            var readyActor = EnvironmentArtCatalog.CreateKenneyFallback(
                "BlockyCharacters",
                modelName,
                actorName + "_ReadyActorMesh",
                groundPosition,
                Vector3.zero,
                new Vector3(0.68f, 0.68f, 0.68f),
                parent);
            if (readyActor == null)
            {
                return null;
            }

            ApplyNoirActorMaterials(readyActor, coatMaterial, skinMaterial, shirtMaterial);
            readyActor.transform.localRotation = Quaternion.identity;
            return readyActor;
        }

        private static void ApplyNoirActorMaterials(
            GameObject actorModel,
            Material coatMaterial,
            Material skinMaterial,
            Material shirtMaterial)
        {
            var renderers = actorModel.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                var sourceMaterials = renderer.sharedMaterials;
                if (sourceMaterials.Length == 0)
                {
                    renderer.sharedMaterial = coatMaterial;
                    continue;
                }

                var replacements = new Material[sourceMaterials.Length];
                for (var index = 0; index < sourceMaterials.Length; index += 1)
                {
                    replacements[index] = coatMaterial;
                }

                renderer.sharedMaterials = replacements;
            }
        }

        private static Material ResolveNoirActorMaterial(
            Material? sourceMaterial,
            Material coatMaterial,
            Material skinMaterial,
            Material shirtMaterial)
        {
            if (sourceMaterial == null)
            {
                return coatMaterial;
            }

            var sourceName = sourceMaterial.name;
            if (sourceName.Contains("skin", System.StringComparison.OrdinalIgnoreCase) ||
                sourceName.Contains("face", System.StringComparison.OrdinalIgnoreCase) ||
                sourceName.Contains("head", System.StringComparison.OrdinalIgnoreCase))
            {
                return skinMaterial;
            }

            if (sourceName.Contains("shirt", System.StringComparison.OrdinalIgnoreCase) ||
                sourceName.Contains("collar", System.StringComparison.OrdinalIgnoreCase))
            {
                return shirtMaterial;
            }

            return coatMaterial;
        }

        private static Color ExtractMaterialColor(Material material)
        {
            if (material.HasProperty("_BaseColor"))
            {
                return material.GetColor("_BaseColor");
            }

            return material.HasProperty("_Color")
                ? material.GetColor("_Color")
                : Color.black;
        }

        private static string ResolveReadyActorModel(string actorName)
        {
            if (actorName.Contains("Luca", System.StringComparison.OrdinalIgnoreCase) ||
                actorName.Contains("Vincent", System.StringComparison.OrdinalIgnoreCase) ||
                actorName.Contains("Contact", System.StringComparison.OrdinalIgnoreCase))
            {
                return "character-c";
            }

            if (actorName.Contains("Watch", System.StringComparison.OrdinalIgnoreCase) ||
                actorName.Contains("Enemy", System.StringComparison.OrdinalIgnoreCase) ||
                actorName.Contains("Guard", System.StringComparison.OrdinalIgnoreCase))
            {
                return "character-f";
            }

            return "character-a";
        }

        private static void CreateNoirStreetDressing(
            string prefix,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material groundAccent,
            Material puddle,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftCurbShadow", new Vector3(-4.55f, 0.13f, 0f), new Vector3(0.18f, 0.04f, 31f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightCurbShadow", new Vector3(4.55f, 0.13f, 0f), new Vector3(0.18f, 0.04f, 31f), sign);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -12f + (index * 6f);
                var leftX = -6.55f - (index % 2 * 0.45f);
                var rightX = 6.65f + (index % 2 * 0.4f);

                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "PosterBoardL_" + index, new Vector3(leftX, 1.45f, z), new Vector3(0.12f, 1.05f, 1.1f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "PosterTrimL_" + index, new Vector3(leftX + 0.02f, 1.98f, z), new Vector3(0.14f, 0.08f, 1.16f), brass);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SignBoxR_" + index, new Vector3(rightX, 2.2f, z + 2.6f), new Vector3(0.16f, 0.56f, 1.1f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SignWarmTrimR_" + index, new Vector3(rightX - 0.02f, 2.48f, z + 2.6f), new Vector3(0.055f, 0.055f, 0.62f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "Awning_" + index, new Vector3(rightX, 2.05f, z + 0.7f), new Vector3(1.25f, 0.18f, 1.5f), sign);
            }

            for (var index = 0; index < 6; index += 1)
            {
                var z = -13f + (index * 5f);
                var x = index % 2 == 0 ? -3.25f : 3.15f;
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WetStreetGlint_" + index, new Vector3(x, 0.125f, z), new Vector3(1.25f, 0.015f, 0.55f), puddle);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "NewsStandBase", new Vector3(-5.75f, 0.75f, -7.6f), new Vector3(1.25f, 1.1f, 1.1f), wood);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "NewsStandRoof", new Vector3(-5.75f, 1.45f, -7.6f), new Vector3(1.55f, 0.18f, 1.35f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "Payphone", new Vector3(5.65f, 1.25f, -6.2f), new Vector3(0.55f, 1.8f, 0.45f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "PayphoneGlow", new Vector3(5.65f, 1.85f, -6.47f), new Vector3(0.28f, 0.28f, 0.04f), glow);

            for (var index = 0; index < 4; index += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "Barrel_" + index, new Vector3(-6.75f + (index * 0.55f), 0.65f, 6.8f + (index % 2 * 0.55f)), new Vector3(0.28f, 0.6f, 0.28f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "Crate_" + index, new Vector3(6.2f + (index % 2 * 0.55f), 0.55f, 8.8f + (index * 0.55f)), new Vector3(0.8f, 0.8f, 0.8f), wood);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CigaretteKiosk", new Vector3(5.9f, 0.72f, 13.8f), new Vector3(1.15f, 1.1f, 0.9f), groundAccent);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "KioskSign", new Vector3(5.9f, 1.55f, 13.8f), new Vector3(1.28f, 0.22f, 1f), brass);
            CreateSteamVent(prefix + "SteamVentA", new Vector3(-3.8f, 0.12f, 11.2f), metal, glow);
            CreateSteamVent(prefix + "SteamVentB", new Vector3(3.5f, 0.12f, -10.6f), metal, glow);
        }

        private static void CreateProductionDockOpeningPass(
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("ProductionArt_Docks_ReadyAssetBlock");

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.Warehouse,
                "QuaterniusDockWarehouse_ColdStorage",
                new Vector3(-9.55f, 0.08f, -8.9f),
                new Vector3(0f, 90f, 0f),
                new Vector3(2.55f, 2.55f, 2.55f),
                facade,
                roof,
                glow);

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.CornerShop,
                "QuaterniusDockCornerShop_MorettiCafe",
                new Vector3(-9.2f, 0.08f, -15.4f),
                new Vector3(0f, 90f, 0f),
                new Vector3(2.35f, 2.35f, 2.35f),
                facade,
                roof,
                glow);

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.OfficeBlock,
                "QuaterniusDockOffice_Block",
                new Vector3(-9.65f, 0.08f, 3.2f),
                new Vector3(0f, 90f, 0f),
                new Vector3(2.45f, 2.45f, 2.45f),
                facade,
                roof,
                glow);

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.RowHouse,
                "QuaterniusDockRightRowHouse_A",
                new Vector3(9.4f, 0.08f, -11.2f),
                new Vector3(0f, -90f, 0f),
                new Vector3(2.35f, 2.35f, 2.35f),
                facade,
                roof,
                glow);

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.NarrowTenement,
                "QuaterniusDockRightTenement_B",
                new Vector3(9.2f, 0.08f, -3.6f),
                new Vector3(0f, -90f, 0f),
                new Vector3(2.45f, 2.45f, 2.45f),
                facade,
                roof,
                glow);

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.Warehouse,
                "QuaterniusDockWarehouse_LoadingDoors",
                new Vector3(9.65f, 0.08f, 7.6f),
                new Vector3(0f, -90f, 0f),
                new Vector3(2.65f, 2.65f, 2.65f),
                facade,
                roof,
                glow);

            PlaceDocksBuilding(
                root.transform,
                DocksBuildingRole.CornerShop,
                "QuaterniusDockRightCornerShop_News",
                new Vector3(9.35f, 0.08f, 15.4f),
                new Vector3(0f, -90f, 0f),
                new Vector3(2.35f, 2.35f, 2.35f),
                facade,
                roof,
                glow);

            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningLeftContactShadow", new Vector3(-9.2f, 0.142f, -6.2f), new Vector3(4.7f, 0.025f, 22.4f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningRightContactShadow", new Vector3(9.2f, 0.142f, 2.6f), new Vector3(4.7f, 0.025f, 27.8f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningSpawnSidewalkClearance", new Vector3(-4.85f, 0.158f, -14.25f), new Vector3(1.85f, 0.035f, 2.9f), sidewalk);
            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningSpawnWetReflection", new Vector3(-3.08f, 0.166f, -14.05f), new Vector3(1.8f, 0.018f, 0.52f), puddle);
            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningCafeSignBacker", new Vector3(-5.92f, 3.15f, -14.3f), new Vector3(0.14f, 0.62f, 2.75f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningCafeSignWarmEdge", new Vector3(-5.78f, 3.22f, -14.3f), new Vector3(0.08f, 0.18f, 2.35f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningCafeDoorGlow", new Vector3(-5.63f, 1.38f, -13.15f), new Vector3(0.07f, 0.9f, 0.55f), glow);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -15.7f + (index * 4.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningFireEscapeDeck_" + index, new Vector3(-6.55f, 3.35f, z), new Vector3(0.72f, 0.075f, 1.52f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningFireEscapeRail_" + index, new Vector3(-6.18f, 3.65f, z), new Vector3(0.06f, 0.48f, 1.52f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningLoadingBay_" + index, new Vector3(6.18f, 1.22f, z + 7.6f), new Vector3(0.12f, 1.68f, 1.18f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningLoadingBayLamp_" + index, new Vector3(6.02f, 2.25f, z + 7.6f), new Vector3(0.06f, 0.18f, 0.58f), glow);
            }

            for (var stack = 0; stack < 6; stack += 1)
            {
                var z = -12.9f + (stack * 1.15f);
                CreateVisualPrimitive(PrimitiveType.Cube, "DockOpeningCrateStack_" + stack, new Vector3(-5.85f, 0.48f + (stack % 2 * 0.34f), z), new Vector3(0.72f, 0.68f, 0.62f), wood);
            }

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "detail-tank", "DockOpeningFuelTankReadyAsset", new Vector3(-13.35f, 0.08f, 8.6f), Vector3.zero, new Vector3(1.55f, 1.55f, 1.55f), root.transform);
            CreateLowProfileStreetLamp("DockOpeningWorkLightReadyAsset", new Vector3(4.8f, 0.08f, -14.5f), root.transform);
        }

        private static void CreateProductionBusinessCorePass(
            Material stone,
            Material brick,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("ProductionArt_BusinessCore_ReadyAssetBlocks");

            PlaceDistrictBuilding(DistrictArtStyle.BusinessCore, DistrictBuildingRole.HeroLandmark, "ReadyAssetBusiness_CityTrustBank", new Vector3(-9.85f, 0.08f, 6.8f), new Vector3(0f, 90f, 0f), new Vector3(2.85f, 2.85f, 2.85f), stone, stone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.BusinessCore, DistrictBuildingRole.CornerShop, "ReadyAssetBusiness_BelloriBooks", new Vector3(9.35f, 0.08f, -3.2f), new Vector3(0f, -90f, 0f), new Vector3(2.45f, 2.45f, 2.45f), brick, stone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.BusinessCore, DistrictBuildingRole.OfficeBlock, "ReadyAssetBusiness_UnionOfficeNorth", new Vector3(9.65f, 0.08f, 7.2f), new Vector3(0f, -90f, 0f), new Vector3(2.7f, 2.7f, 2.7f), stone, stone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.BusinessCore, DistrictBuildingRole.OfficeBlock, "ReadyAssetBusiness_InsuranceTower", new Vector3(9.6f, 0.08f, -12.1f), new Vector3(0f, -90f, 0f), new Vector3(2.65f, 2.65f, 2.65f), stone, stone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.BusinessCore, DistrictBuildingRole.RowHouse, "ReadyAssetBusiness_PrinterBlock", new Vector3(-9.55f, 0.08f, -10.6f), new Vector3(0f, 90f, 0f), new Vector3(2.35f, 2.35f, 2.35f), brick, stone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.BusinessCore, DistrictBuildingRole.CornerShop, "ReadyAssetBusiness_TheatreCorner", new Vector3(9.45f, 0.08f, 15.2f), new Vector3(0f, -90f, 0f), new Vector3(2.45f, 2.45f, 2.45f), brick, stone, glow, root.transform);

            CreateVisualPrimitive(PrimitiveType.Cube, "BusinessReadyAssetBankSteps", new Vector3(-5.95f, 0.24f, 6.8f), new Vector3(1.25f, 0.18f, 6.8f), sidewalk);
            CreateVisualPrimitive(PrimitiveType.Cube, "BusinessReadyAssetBankBrassHeader", new Vector3(-6.38f, 3.75f, 6.8f), new Vector3(0.12f, 0.28f, 5.2f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "BusinessReadyAssetTheatreGlow", new Vector3(6.15f, 2.75f, 15.2f), new Vector3(0.08f, 0.42f, 4.4f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "BusinessReadyAssetTheatreCanopy", new Vector3(6.35f, 2.35f, 15.2f), new Vector3(1.15f, 0.22f, 4.65f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "BusinessReadyAssetWetSquareReflection", new Vector3(-2.25f, 0.145f, 13.4f), new Vector3(4.6f, 0.018f, 2.2f), puddle);

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-awning-wide", "BusinessReadyAssetBookstoreAwning", new Vector3(6.55f, 1.8f, -3.2f), new Vector3(0f, -90f, 0f), new Vector3(1.8f, 1.8f, 1.8f), root.transform);
            CreateLowProfileStreetLamp("BusinessReadyAssetUnionSquareStreetLight", new Vector3(3.9f, 0.08f, 12.6f), root.transform);
        }

        private static void CreateProductionOldQuarterPass(
            Material tenement,
            Material churchStone,
            Material metal,
            Material glow,
            Material laundry,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("ProductionArt_OldQuarter_ReadyAssetBlocks");

            PlaceDistrictBuilding(DistrictArtStyle.OldQuarter, DistrictBuildingRole.HeroLandmark, "ReadyAssetOldQuarter_SaintVeraFront", new Vector3(-9.25f, 0.08f, 11.4f), new Vector3(0f, 90f, 0f), new Vector3(2.75f, 2.75f, 2.75f), churchStone, churchStone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.OldQuarter, DistrictBuildingRole.NarrowTenement, "ReadyAssetOldQuarter_LeftTenementA", new Vector3(-9.35f, 0.08f, -8.2f), new Vector3(0f, 90f, 0f), new Vector3(2.55f, 2.55f, 2.55f), tenement, churchStone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.OldQuarter, DistrictBuildingRole.RowHouse, "ReadyAssetOldQuarter_LeftRowB", new Vector3(-9.2f, 0.08f, 0.2f), new Vector3(0f, 90f, 0f), new Vector3(2.4f, 2.4f, 2.4f), tenement, churchStone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.OldQuarter, DistrictBuildingRole.NarrowTenement, "ReadyAssetOldQuarter_RightTenementA", new Vector3(9.25f, 0.08f, -7.4f), new Vector3(0f, -90f, 0f), new Vector3(2.5f, 2.5f, 2.5f), tenement, churchStone, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.OldQuarter, DistrictBuildingRole.RowHouse, "ReadyAssetOldQuarter_RightRowB", new Vector3(9.1f, 0.08f, 4.8f), new Vector3(0f, -90f, 0f), new Vector3(2.35f, 2.35f, 2.35f), tenement, churchStone, glow, root.transform);

            CreateVisualPrimitive(PrimitiveType.Cube, "OldQuarterReadyAssetChurchTowerSilhouette", new Vector3(-7.05f, 6.9f, 15.65f), new Vector3(1.05f, 8.8f, 1.05f), churchStone);
            CreateVisualPrimitive(PrimitiveType.Cube, "OldQuarterReadyAssetChurchSpire", new Vector3(-7.05f, 11.75f, 15.65f), new Vector3(0.45f, 1.6f, 0.45f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "OldQuarterReadyAssetChapelWarmDoor", new Vector3(-6.25f, 1.22f, 8.4f), new Vector3(0.08f, 1.7f, 1.05f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "OldQuarterReadyAssetWetAlley", new Vector3(0f, 0.142f, -9.4f), new Vector3(3.0f, 0.018f, 5.4f), puddle);

            for (var line = 0; line < 4; line += 1)
            {
                var z = -10.5f + (line * 5.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, "OldQuarterReadyAssetLaundryLine_" + line, new Vector3(0f, 5.65f, z), new Vector3(9.2f, 0.05f, 0.05f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, "OldQuarterReadyAssetLaundrySheet_" + line, new Vector3(-1.8f + (line * 0.9f), 5.25f, z), new Vector3(0.48f, 0.72f, 0.03f), laundry);
            }

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitSuburban", "fence-1x4", "OldQuarterReadyAssetChapelFenceLeft", new Vector3(-5.55f, 0.08f, 12.25f), new Vector3(0f, 90f, 0f), new Vector3(1.25f, 1.25f, 1.25f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitSuburban", "planter", "OldQuarterReadyAssetStoopedPlanter", new Vector3(5.2f, 0.08f, -5.8f), Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), root.transform);
        }

        private static void CreateProductionRailYardPass(
            Material rustMetal,
            Material tankMetal,
            Material metal,
            Material glow,
            Material gravel,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("ProductionArt_RailYard_ReadyAssetBlocks");

            PlaceDistrictBuilding(DistrictArtStyle.RailYard, DistrictBuildingRole.HeroLandmark, "ReadyAssetRail_IronlineGarage", new Vector3(-9.65f, 0.08f, -2.4f), new Vector3(0f, 90f, 0f), new Vector3(3.05f, 3.05f, 3.05f), tankMetal, rustMetal, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.RailYard, DistrictBuildingRole.Warehouse, "ReadyAssetRail_FoundryShedNorth", new Vector3(9.55f, 0.08f, 4.8f), new Vector3(0f, -90f, 0f), new Vector3(2.75f, 2.75f, 2.75f), rustMetal, tankMetal, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.RailYard, DistrictBuildingRole.Warehouse, "ReadyAssetRail_FreightOffice", new Vector3(9.45f, 0.08f, -8.4f), new Vector3(0f, -90f, 0f), new Vector3(2.35f, 2.35f, 2.35f), tankMetal, rustMetal, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.RailYard, DistrictBuildingRole.CornerShop, "ReadyAssetRail_DispatchCorner", new Vector3(-9.35f, 0.08f, 9.2f), new Vector3(0f, 90f, 0f), new Vector3(2.35f, 2.35f, 2.35f), tankMetal, rustMetal, glow, root.transform);

            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetBoxcarBody", new Vector3(1.9f, 1.35f, 10.5f), new Vector3(2.45f, 2.55f, 7.6f), rustMetal);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetBoxcarRoof", new Vector3(1.9f, 2.75f, 10.5f), new Vector3(2.2f, 0.25f, 7.2f), tankMetal);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetBoxcarDoorA", new Vector3(0.64f, 1.45f, 9.2f), new Vector3(0.07f, 1.8f, 1.5f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetWetTrackReflection", new Vector3(0f, 0.142f, 8f), new Vector3(4.2f, 0.018f, 14.8f), puddle);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetGarageLitHeader", new Vector3(-6.45f, 3.1f, -6.8f), new Vector3(0.08f, 0.28f, 3.4f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetCoalPileShadow", new Vector3(6.2f, 0.18f, 10.8f), new Vector3(2.4f, 0.18f, 2.2f), gravel);

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "detail-tank", "RailReadyAssetFuelTankA", new Vector3(11.8f, 0.08f, -1.8f), Vector3.zero, new Vector3(1.8f, 1.8f, 1.8f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "chimney-large", "RailReadyAssetSmokeStack", new Vector3(-11.6f, 0.08f, 4.8f), Vector3.zero, new Vector3(1.7f, 1.7f, 1.7f), root.transform);
            CreateLowProfileStreetLamp("RailReadyAssetWorkLight", new Vector3(4.6f, 0.08f, -9.8f), root.transform);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailReadyAssetYardHook", new Vector3(-4.2f, 5.2f, 8.2f), new Vector3(0.38f, 0.34f, 0.38f), brass);
        }

        private static void CreateReadyAssetUrbanCanyonPass(
            DistrictArtStyle district,
            string prefix,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("ProductionArt_" + prefix + "_UrbanCanyonReadyAssets");
            CreateReadyRoadTilePass(prefix, root.transform);
            var leftRoles = new[]
            {
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.OfficeBlock
            };
            var rightRoles = new[]
            {
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.CornerShop
            };

            for (var index = 0; index < 8; index += 1)
            {
                var z = -18.5f + (index * 5.35f);
                var leftX = -13.65f - ((index % 2) * 0.85f);
                var rightX = 13.65f + (((index + 1) % 2) * 0.85f);
                var leftScale = new Vector3(1.85f + ((index % 3) * 0.18f), 2.2f + ((index % 2) * 0.22f), 1.85f + ((index % 2) * 0.15f));
                var rightScale = new Vector3(1.95f + (((index + 1) % 3) * 0.18f), 2.1f + (((index + 1) % 2) * 0.24f), 1.9f + (((index + 1) % 2) * 0.15f));

                PlaceDistrictBuilding(
                    district,
                    leftRoles[index % leftRoles.Length],
                    prefix + "CanyonLeftReadyAsset_" + index,
                    new Vector3(leftX, 0.08f, z),
                    new Vector3(0f, 90f, 0f),
                    leftScale,
                    facade,
                    roof,
                    glow,
                    root.transform);

                PlaceDistrictBuilding(
                    district,
                    rightRoles[index % rightRoles.Length],
                    prefix + "CanyonRightReadyAsset_" + index,
                    new Vector3(rightX, 0.08f, z + 1.9f),
                    new Vector3(0f, -90f, 0f),
                    rightScale,
                    facade,
                    roof,
                    glow,
                    root.transform);

                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonLeftFacadeDepth_" + index, new Vector3(-8.05f, 2.85f, z), new Vector3(0.16f, 5.05f, 2.35f), facade);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonRightFacadeDepth_" + index, new Vector3(8.05f, 2.85f, z + 1.9f), new Vector3(0.16f, 5.05f, 2.35f), facade);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonLeftWarmWindow_" + index, new Vector3(-7.93f, 3.25f, z - 0.5f), new Vector3(0.06f, 0.7f, 0.48f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonRightWarmWindow_" + index, new Vector3(7.93f, 3.25f, z + 2.45f), new Vector3(0.06f, 0.7f, 0.48f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonLeftStorefront_" + index, new Vector3(-7.92f, 1.35f, z + 0.65f), new Vector3(0.07f, 1.55f, 0.95f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonRightStorefront_" + index, new Vector3(7.92f, 1.35f, z + 1.15f), new Vector3(0.07f, 1.55f, 0.95f), sign);

                if (index % 2 == 0)
                {
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonFireEscapeDeckL_" + index, new Vector3(-7.3f, 4.35f, z), new Vector3(0.92f, 0.08f, 1.85f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonFireEscapeRailL_" + index, new Vector3(-6.86f, 4.67f, z), new Vector3(0.06f, 0.5f, 1.85f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonFireEscapeDeckR_" + index, new Vector3(7.3f, 4.35f, z + 1.9f), new Vector3(0.92f, 0.08f, 1.85f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonFireEscapeRailR_" + index, new Vector3(6.86f, 4.67f, z + 1.9f), new Vector3(0.06f, 0.5f, 1.85f), metal);
                }
                else
                {
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonAwningLeft_" + index, new Vector3(-6.92f, 2.32f, z + 0.65f), new Vector3(1.25f, 0.14f, 1.25f), brass);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonAwningRight_" + index, new Vector3(6.92f, 2.32f, z + 1.15f), new Vector3(1.25f, 0.14f, 1.25f), brass);
                }
            }

            for (var index = 0; index < 5; index += 1)
            {
                var z = -15.2f + (index * 8.1f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonSideStreetWet_" + index, new Vector3(0f, 0.168f, z), new Vector3(28.5f, 0.018f, 1.25f), puddle);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonSidewalkSeamL_" + index, new Vector3(-5.9f, 0.205f, z), new Vector3(2.45f, 0.024f, 0.055f), sidewalk);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CanyonSidewalkSeamR_" + index, new Vector3(5.9f, 0.205f, z + 1.6f), new Vector3(2.45f, 0.024f, 0.055f), sidewalk);
            }

            CreateLowProfileStreetLamp(prefix + "CanyonReadyStreetLightA", new Vector3(-5.25f, 0.08f, -16.2f), root.transform);
            CreateLowProfileStreetLamp(prefix + "CanyonReadyStreetLightB", new Vector3(5.25f, 0.08f, -5.8f), root.transform);
            CreateLowProfileStreetLamp(prefix + "CanyonReadyStreetLightC", new Vector3(-5.25f, 0.08f, 5.9f), root.transform);
            CreateLowProfileStreetLamp(prefix + "CanyonReadyStreetLightD", new Vector3(5.25f, 0.08f, 16.1f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "trashcan", prefix + "CanyonReadyTrashCan", new Vector3(-5.95f, 0.08f, -9.8f), Vector3.zero, new Vector3(1.15f, 1.15f, 1.15f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "mailbox", prefix + "CanyonReadyMailbox", new Vector3(5.95f, 0.08f, 8.8f), Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f), root.transform);
        }

        private static void CreateReadyRoadTilePass(string prefix, Transform root)
        {
            for (var index = 0; index < 7; index += 1)
            {
                var z = -17.4f + (index * 5.8f);
                EnvironmentArtCatalog.CreateKenneyFallback(
                    "CityKitRoads",
                    "road-straight",
                    prefix + "ReadyRoadStraight_" + index,
                    new Vector3(0f, 0.132f, z),
                    Vector3.zero,
                    new Vector3(2.85f, 1f, 2.85f),
                    root);
            }

            for (var index = 0; index < 3; index += 1)
            {
                var z = -11.6f + (index * 11.6f);
                EnvironmentArtCatalog.CreateKenneyFallback(
                    "CityKitRoads",
                    "road-intersection-line",
                    prefix + "ReadyRoadIntersection_" + index,
                    new Vector3(0f, 0.136f, z),
                    Vector3.zero,
                    new Vector3(2.92f, 1f, 2.92f),
                    root);
                EnvironmentArtCatalog.CreateKenneyFallback(
                    "CityKitRoads",
                    "road-side",
                    prefix + "ReadySideStreetLeft_" + index,
                    new Vector3(-7.1f, 0.134f, z),
                    new Vector3(0f, 90f, 0f),
                    new Vector3(2.2f, 1f, 2.2f),
                    root);
                EnvironmentArtCatalog.CreateKenneyFallback(
                    "CityKitRoads",
                    "road-side",
                    prefix + "ReadySideStreetRight_" + index,
                    new Vector3(7.1f, 0.134f, z + 1.5f),
                    new Vector3(0f, -90f, 0f),
                    new Vector3(2.2f, 1f, 2.2f),
                    root);
            }
        }

        private static void PlaceDocksBuilding(
            Transform root,
            DocksBuildingRole role,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Material facade,
            Material roof,
            Material glow)
        {
            var instance = EnvironmentArtCatalog.CreateDocksBuilding(role, instanceName, position, rotationEuler, scale, root);
            if (instance != null)
            {
                AddRendererBoundsCollider(instance, instanceName + "_BuildingEnvelope", 0.88f, 0.92f);
                return;
            }

            CreateFallbackFacade(instanceName, position, rotationEuler.y < 0f ? -1f : 1f, facade, roof, glow);
        }

        private static void PlaceDistrictBuilding(
            DistrictArtStyle district,
            DistrictBuildingRole role,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Material facade,
            Material roof,
            Material glow,
            Transform root)
        {
            var instance = EnvironmentArtCatalog.CreateDistrictBuilding(district, role, instanceName, position, rotationEuler, scale, root);
            if (instance != null)
            {
                AddRendererBoundsCollider(instance, instanceName + "_BuildingEnvelope", 0.88f, 0.92f);
                return;
            }

            CreateFallbackFacade(instanceName, position, rotationEuler.y < 0f ? -1f : 1f, facade, roof, glow);
        }

        private static void AddRendererBoundsCollider(GameObject instance, string colliderName, float horizontalScale, float depthScale)
        {
            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return;
            }

            var bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index += 1)
            {
                bounds.Encapsulate(renderers[index].bounds);
            }

            var colliderObject = new GameObject(colliderName);
            colliderObject.transform.SetParent(instance.transform, true);
            colliderObject.transform.position = bounds.center;
            colliderObject.transform.rotation = Quaternion.identity;
            var collider = colliderObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(
                Mathf.Max(0.8f, bounds.size.x * horizontalScale),
                Mathf.Max(1.4f, bounds.size.y),
                Mathf.Max(0.8f, bounds.size.z * depthScale));
        }

        private static void CreateFallbackFacade(string name, Vector3 position, float facingSign, Material facade, Material roof, Material glow)
        {
            var x = position.x;
            var z = position.z;
            var frontX = x - (facingSign * 1.72f);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_FallbackMass", new Vector3(x, 2.6f, z), new Vector3(3.2f, 5.2f, 4.8f), facade);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_FallbackRoof", new Vector3(x, 5.45f, z), new Vector3(3.5f, 0.34f, 5.05f), roof);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "_FallbackDoor", new Vector3(frontX, 1.05f, z - 1.1f), new Vector3(0.08f, 1.55f, 0.72f), roof);
            CreateColliderBlock(name + "_FallbackCollider", new Vector3(x, 2.6f, z), new Vector3(3.0f, 5.2f, 4.6f));

            for (var row = 0; row < 3; row += 1)
            {
                for (var col = 0; col < 2; col += 1)
                {
                    CreateVisualPrimitive(
                        PrimitiveType.Cube,
                        name + "_FallbackWindow_" + row + "_" + col,
                        new Vector3(frontX, 2.05f + (row * 0.88f), z - 0.9f + (col * 1.8f)),
                        new Vector3(0.075f, 0.44f, 0.48f),
                        glow);
                }
            }
        }

        private static void CreateFinishedCityBlockPass(
            string prefix,
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            for (var index = 0; index < 4; index += 1)
            {
                var z = -12.8f + (index * 8.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftAlleyMouth_" + index, new Vector3(-6.95f, 0.145f, z), new Vector3(2.15f, 0.035f, 1.1f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightAlleyMouth_" + index, new Vector3(6.95f, 0.145f, z + 2.15f), new Vector3(2.15f, 0.035f, 1.1f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftAlleyWetEdge_" + index, new Vector3(-5.35f, 0.15f, z), new Vector3(0.42f, 0.018f, 1.06f), puddle);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightAlleyWetEdge_" + index, new Vector3(5.35f, 0.15f, z + 2.15f), new Vector3(0.42f, 0.018f, 1.06f), puddle);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrosswalkSliceA_" + index, new Vector3(-1.85f, 0.145f, z + 0.55f), new Vector3(1.55f, 0.025f, 0.12f), brass);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrosswalkSliceB_" + index, new Vector3(1.85f, 0.145f, z + 0.55f), new Vector3(1.55f, 0.025f, 0.12f), brass);
            }

            for (var block = 0; block < 2; block += 1)
            {
                var z = block == 0 ? -5.6f : 8.6f;
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetAsphalt_" + block, new Vector3(0f, 0.126f, z), new Vector3(31.5f, 0.032f, 4.4f), asphalt);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetNearCurb_" + block, new Vector3(0f, 0.152f, z - 2.34f), new Vector3(31.5f, 0.035f, 0.16f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetFarCurb_" + block, new Vector3(0f, 0.152f, z + 2.34f), new Vector3(31.5f, 0.035f, 0.16f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetLeftWalk_" + block, new Vector3(-12.4f, 0.154f, z), new Vector3(3.8f, 0.036f, 4.8f), sidewalk);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetRightWalk_" + block, new Vector3(12.4f, 0.154f, z), new Vector3(3.8f, 0.036f, 4.8f), sidewalk);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetWetMiddle_" + block, new Vector3(block == 0 ? -2.4f : 2.2f, 0.158f, z - 0.55f), new Vector3(3.2f, 0.018f, 0.48f), puddle);

                for (var stripe = 0; stripe < 4; stripe += 1)
                {
                    CreateVisualPrimitive(
                        PrimitiveType.Cube,
                        prefix + "IntersectionStripe_" + block + "_" + stripe,
                        new Vector3(-3.0f + (stripe * 2.0f), 0.165f, z - 2.02f),
                        new Vector3(0.95f, 0.018f, 0.18f),
                        brass);
                }
            }

            for (var index = 0; index < 8; index += 1)
            {
                var z = -15.4f + (index * 4.35f);
                var leftZ = z;
                var rightZ = z + 1.35f;

                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftStorefrontBay_" + index, new Vector3(-7.64f, 1.58f, leftZ), new Vector3(0.14f, 2.35f, 2.34f), facade);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightStorefrontBay_" + index, new Vector3(7.64f, 1.58f, rightZ), new Vector3(0.14f, 2.35f, 2.34f), facade);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftWindowPocket_" + index, new Vector3(-7.55f, 1.86f, leftZ - 0.48f), new Vector3(0.055f, 0.72f, 0.55f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftDoorPocket_" + index, new Vector3(-7.54f, 1.02f, leftZ + 0.58f), new Vector3(0.06f, 1.42f, 0.45f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightWindowPocket_" + index, new Vector3(7.55f, 1.86f, rightZ + 0.48f), new Vector3(0.055f, 0.72f, 0.55f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightDoorPocket_" + index, new Vector3(7.54f, 1.02f, rightZ - 0.58f), new Vector3(0.06f, 1.42f, 0.45f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftAwningRib_" + index, new Vector3(-6.72f, 2.42f, leftZ), new Vector3(1.55f, 0.16f, 1.62f), roof);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightAwningRib_" + index, new Vector3(6.72f, 2.42f, rightZ), new Vector3(1.55f, 0.16f, 1.62f), roof);

                if (index % 2 == 0)
                {
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HangingSignLeft_" + index, new Vector3(-6.74f, 2.92f, leftZ + 0.92f), new Vector3(0.9f, 0.44f, 0.08f), sign);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HangingSignRight_" + index, new Vector3(6.74f, 2.92f, rightZ - 0.92f), new Vector3(0.9f, 0.44f, 0.08f), sign);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SignBulbLeft_" + index, new Vector3(-6.28f, 2.95f, leftZ + 0.92f), new Vector3(0.12f, 0.12f, 0.055f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SignBulbRight_" + index, new Vector3(6.28f, 2.95f, rightZ - 0.92f), new Vector3(0.12f, 0.12f, 0.055f), glow);
                }
                else
                {
                    CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "DownspoutLeft_" + index, new Vector3(-7.36f, 1.75f, leftZ + 1.08f), new Vector3(0.055f, 1.62f, 0.055f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "DownspoutRight_" + index, new Vector3(7.36f, 1.75f, rightZ - 1.08f), new Vector3(0.055f, 1.62f, 0.055f), metal);
                }

                for (var level = 0; level < 3; level += 1)
                {
                    var y = 2.82f + (level * 0.86f);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftUpperWindowA_" + index + "_" + level, new Vector3(-7.52f, y, leftZ - 0.82f), new Vector3(0.06f, 0.46f, 0.36f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftUpperWindowB_" + index + "_" + level, new Vector3(-7.52f, y, leftZ + 0.18f), new Vector3(0.06f, 0.46f, 0.36f), level == 1 ? sign : glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightUpperWindowA_" + index + "_" + level, new Vector3(7.52f, y, rightZ + 0.82f), new Vector3(0.06f, 0.46f, 0.36f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightUpperWindowB_" + index + "_" + level, new Vector3(7.52f, y, rightZ - 0.18f), new Vector3(0.06f, 0.46f, 0.36f), level == 1 ? sign : glow);
                }

                if (index % 3 == 0)
                {
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftFireEscapeDeck_" + index, new Vector3(-6.95f, 3.28f, leftZ - 0.28f), new Vector3(0.82f, 0.07f, 1.85f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftFireEscapeRail_" + index, new Vector3(-6.55f, 3.55f, leftZ - 0.28f), new Vector3(0.055f, 0.46f, 1.85f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightFireEscapeDeck_" + index, new Vector3(6.95f, 3.28f, rightZ + 0.28f), new Vector3(0.82f, 0.07f, 1.85f), metal);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightFireEscapeRail_" + index, new Vector3(6.55f, 3.55f, rightZ + 0.28f), new Vector3(0.055f, 0.46f, 1.85f), metal);
                }
            }

            for (var index = 0; index < 6; index += 1)
            {
                var z = -14.6f + (index * 5.7f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftRoofPatch_" + index, new Vector3(-10.35f, 6.94f, z), new Vector3(3.2f, 0.03f, 1.35f), roof);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightRoofPatch_" + index, new Vector3(10.35f, 6.94f, z + 1.7f), new Vector3(3.2f, 0.03f, 1.35f), roof);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftParapetBreak_" + index, new Vector3(-7.94f, 6.98f, z), new Vector3(0.18f, 0.16f, 1.45f), brass);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightParapetBreak_" + index, new Vector3(7.94f, 6.98f, z + 1.7f), new Vector3(0.18f, 0.16f, 1.45f), brass);
                CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "RoofPipeLeft_" + index, new Vector3(-11.45f, 7.28f, z - 0.8f), new Vector3(0.16f, 0.42f, 0.16f), metal);
                CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "RoofPipeRight_" + index, new Vector3(11.45f, 7.28f, z + 0.9f), new Vector3(0.16f, 0.42f, 0.16f), metal);
            }

            for (var cover = 0; cover < 4; cover += 1)
            {
                var z = -13.2f + (cover * 8.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ManholeCover_" + cover, new Vector3(cover % 2 == 0 ? -1.45f : 1.35f, 0.162f, z), new Vector3(0.46f, 0.02f, 0.46f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WornLanePaint_" + cover, new Vector3(0f, 0.164f, z + 2.2f), new Vector3(0.22f, 0.018f, 1.4f), brass);
            }
        }

        private static void CreateMafiaDistrictProductionPass(
            DistrictArtStyle district,
            string prefix,
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("LaunchArt_" + prefix + "_FinishedDistrictBlock");
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OuterNeighborhoodAsphalt", new Vector3(0f, 0.042f, -2f), new Vector3(78f, 0.035f, 100f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WestServiceLane", new Vector3(-17.4f, 0.096f, 1f), new Vector3(5.2f, 0.035f, 50f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "EastServiceLane", new Vector3(17.4f, 0.096f, 1f), new Vector3(5.2f, 0.035f, 50f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WestBackWalk", new Vector3(-20.9f, 0.13f, 1f), new Vector3(1.4f, 0.08f, 48f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "EastBackWalk", new Vector3(20.9f, 0.13f, 1f), new Vector3(1.4f, 0.08f, 48f), sidewalk).transform.SetParent(root.transform, true);
            CreateOpenCityGridPass(district, prefix, root.transform, asphalt, facade, roof, metal, glow, sign, sidewalk, puddle, brass);

            var leftRoles = new[]
            {
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.Warehouse
            };
            var rightRoles = new[]
            {
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.NarrowTenement
            };

            for (var index = 0; index < 12; index += 1)
            {
                var z = -25.5f + (index * 4.85f);
                var leftX = -12.2f - ((index % 3) * 0.65f);
                var rightX = 12.2f + (((index + 1) % 3) * 0.65f);
                var frontScale = new Vector3(2.05f + ((index % 2) * 0.22f), 2.35f + ((index % 4) * 0.18f), 2.05f + (((index + 1) % 2) * 0.14f));
                var backScale = new Vector3(1.7f + ((index % 3) * 0.12f), 1.9f + ((index % 2) * 0.2f), 1.75f + (((index + 1) % 3) * 0.12f));

                PlaceDistrictBuilding(district, leftRoles[index % leftRoles.Length], prefix + "FinishedFrontLeft_" + index, new Vector3(leftX, 0.08f, z), new Vector3(0f, 90f, 0f), frontScale, facade, roof, glow, root.transform);
                PlaceDistrictBuilding(district, rightRoles[index % rightRoles.Length], prefix + "FinishedFrontRight_" + index, new Vector3(rightX, 0.08f, z + 2.05f), new Vector3(0f, -90f, 0f), frontScale, facade, roof, glow, root.transform);
                PlaceDistrictBuilding(district, leftRoles[(index + 2) % leftRoles.Length], prefix + "FinishedBackLeft_" + index, new Vector3(-21.8f, 0.04f, z + 1.2f), new Vector3(0f, 90f, 0f), backScale, facade, roof, glow, root.transform);
                PlaceDistrictBuilding(district, rightRoles[(index + 3) % rightRoles.Length], prefix + "FinishedBackRight_" + index, new Vector3(21.8f, 0.04f, z - 0.7f), new Vector3(0f, -90f, 0f), backScale, facade, roof, glow, root.transform);

                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "FrontLeftShadowCut_" + index, new Vector3(-8.05f, 0.18f, z + 1.1f), new Vector3(1.35f, 0.035f, 2.8f), puddle).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "FrontRightShadowCut_" + index, new Vector3(8.05f, 0.18f, z + 0.8f), new Vector3(1.35f, 0.035f, 2.8f), puddle).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ShopSignLeft_" + index, new Vector3(-7.2f, 2.75f, z + 0.7f), new Vector3(1.05f, 0.34f, 0.08f), sign).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ShopSignRight_" + index, new Vector3(7.2f, 2.75f, z + 1.2f), new Vector3(1.05f, 0.34f, 0.08f), sign).transform.SetParent(root.transform, true);

                if (index % 3 == 0)
                {
                    EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-awning-wide", prefix + "KitAwningLeft_" + index, new Vector3(-7.15f, 1.62f, z + 0.15f), new Vector3(0f, 90f, 0f), new Vector3(1.55f, 1.55f, 1.55f), root.transform);
                    EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-awning-wide", prefix + "KitAwningRight_" + index, new Vector3(7.15f, 1.62f, z + 1.65f), new Vector3(0f, -90f, 0f), new Vector3(1.55f, 1.55f, 1.55f), root.transform);
                }
                else if (index % 3 == 1)
                {
                    CreateLowProfileStreetLamp(prefix + "KitLampLeft_" + index, new Vector3(-5.1f, 0.08f, z + 0.4f), root.transform);
                    CreateLowProfileStreetLamp(prefix + "KitLampRight_" + index, new Vector3(5.1f, 0.08f, z + 1.6f), root.transform);
                }
            }

            for (var block = 0; block < 5; block += 1)
            {
                var z = -20f + (block * 10f);
                EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "road-crossroad-line", prefix + "KitCrossroad_" + block, new Vector3(0f, 0.138f, z), Vector3.zero, new Vector3(3.25f, 1f, 3.25f), root.transform);
                EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "road-side", prefix + "KitWestSideRoad_" + block, new Vector3(-9.7f, 0.136f, z), new Vector3(0f, 90f, 0f), new Vector3(2.35f, 1f, 2.35f), root.transform);
                EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "road-side", prefix + "KitEastSideRoad_" + block, new Vector3(9.7f, 0.136f, z + 1.7f), new Vector3(0f, -90f, 0f), new Vector3(2.35f, 1f, 2.35f), root.transform);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrossStreetPool_" + block, new Vector3(block % 2 == 0 ? -2.7f : 2.5f, 0.17f, z - 1.4f), new Vector3(3.1f, 0.02f, 0.55f), puddle).transform.SetParent(root.transform, true);
            }

            CreateSouthNeighborhoodFill(district, prefix, root.transform, asphalt, facade, roof, metal, glow, sign, sidewalk, puddle, brass);
            CreateDistrictLightPools(prefix, root.transform, glow, brass);

            CreateStaticVehicle(prefix + "ParkedPeriodSedanA", new Vector3(-4.1f, 0.72f, -18.3f), new Vector3(1.8f, 0.95f, 4.1f), new Color(0.055f, 0.058f, 0.064f), roof, glow);
            CreateStaticVehicle(prefix + "ParkedPeriodSedanB", new Vector3(4.1f, 0.72f, 12.2f), new Vector3(1.75f, 0.92f, 3.95f), new Color(0.18f, 0.06f, 0.055f), roof, glow);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ReadableNorthBlockerShadow", new Vector3(0f, 0.18f, 26.2f), new Vector3(30f, 0.05f, 1.2f), sign).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ReadableSouthBlockerShadow", new Vector3(0f, 0.18f, -27.8f), new Vector3(30f, 0.05f, 1.2f), sign).transform.SetParent(root.transform, true);

            if (district == DistrictArtStyle.Docks)
            {
                CreateDockProductionSignature(prefix, root.transform, metal, glow, roof, sign, sidewalk, puddle, brass);
            }
            else if (district == DistrictArtStyle.BusinessCore)
            {
                CreateBusinessProductionSignature(prefix, root.transform, metal, glow, roof, sign, sidewalk, puddle, brass);
            }
            else if (district == DistrictArtStyle.OldQuarter)
            {
                CreateOldQuarterProductionSignature(prefix, root.transform, metal, glow, roof, sign, sidewalk, puddle, brass);
            }
            else
            {
                CreateRailProductionSignature(prefix, root.transform, metal, glow, roof, sign, sidewalk, puddle, brass);
            }
        }

        private static void CreateOpenCityGridPass(
            DistrictArtStyle district,
            string prefix,
            Transform root,
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var northSouthRoads = new[] { -18f, 0f, 18f };
            var crossStreets = new[] { -42f, -28f, -14f, 0f, 14f, 28f, 42f };

            for (var roadIndex = 0; roadIndex < northSouthRoads.Length; roadIndex += 1)
            {
                var x = northSouthRoads[roadIndex];
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridNorthSouthRoad_" + roadIndex, new Vector3(x, 0.128f, -2f), new Vector3(5.9f, 0.036f, 92f), asphalt).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridNorthSouthLeftCurb_" + roadIndex, new Vector3(x - 4.15f, 0.168f, -2f), new Vector3(1.35f, 0.08f, 89f), sidewalk).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridNorthSouthRightCurb_" + roadIndex, new Vector3(x + 4.15f, 0.168f, -2f), new Vector3(1.35f, 0.08f, 89f), sidewalk).transform.SetParent(root, true);

                for (var stripe = 0; stripe < 10; stripe += 1)
                {
                    var z = -39.5f + (stripe * 8.1f);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridLaneStripe_" + roadIndex + "_" + stripe, new Vector3(x, 0.181f, z), new Vector3(0.28f, 0.018f, 2.2f), brass).transform.SetParent(root, true);
                }
            }

            for (var streetIndex = 0; streetIndex < crossStreets.Length; streetIndex += 1)
            {
                var z = crossStreets[streetIndex];
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridEastWestRoad_" + streetIndex, new Vector3(0f, 0.132f, z), new Vector3(72f, 0.036f, 5.7f), asphalt).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridEastWestNorthCurb_" + streetIndex, new Vector3(0f, 0.17f, z + 4.05f), new Vector3(69f, 0.08f, 1.28f), sidewalk).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridEastWestSouthCurb_" + streetIndex, new Vector3(0f, 0.17f, z - 4.05f), new Vector3(69f, 0.08f, 1.28f), sidewalk).transform.SetParent(root, true);

                for (var crossing = 0; crossing < northSouthRoads.Length; crossing += 1)
                {
                    var x = northSouthRoads[crossing];
                    EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "road-crossroad-line", prefix + "GridCrossroad_" + streetIndex + "_" + crossing, new Vector3(x, 0.186f, z), Vector3.zero, new Vector3(2.4f, 1f, 2.4f), root);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridCrosswalkA_" + streetIndex + "_" + crossing, new Vector3(x - 2.65f, 0.194f, z), new Vector3(0.32f, 0.018f, 4.65f), brass).transform.SetParent(root, true);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "GridCrosswalkB_" + streetIndex + "_" + crossing, new Vector3(x + 2.65f, 0.194f, z), new Vector3(0.32f, 0.018f, 4.65f), brass).transform.SetParent(root, true);
                }
            }

            var buildingRoles = new[]
            {
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.Warehouse
            };
            var lotXs = new[] { -31f, -10.4f, 10.4f, 31f };
            var lotZs = new[] { -49f, -35f, -21f, -7f, 7f, 21f, 35f, 49f };

            for (var zIndex = 0; zIndex < lotZs.Length; zIndex += 1)
            {
                for (var xIndex = 0; xIndex < lotXs.Length; xIndex += 1)
                {
                    var x = lotXs[xIndex];
                    var z = lotZs[zIndex] + (((xIndex + zIndex) % 2 == 0) ? 0.9f : -0.75f);
                    var innerBlock = Mathf.Abs(x) < 12f;
                    var frontageScale = innerBlock
                        ? new Vector3(1.45f + ((zIndex % 2) * 0.2f), 1.9f + ((zIndex % 3) * 0.18f), 1.6f)
                        : new Vector3(2.1f + ((zIndex % 2) * 0.22f), 2.55f + ((xIndex % 3) * 0.22f), 2.25f);
                    var facing = x < 0f ? 90f : -90f;
                    var role = buildingRoles[(xIndex + zIndex) % buildingRoles.Length];

                    PlaceDistrictBuilding(district, role, prefix + "OpenGridBlock_" + xIndex + "_" + zIndex, new Vector3(x, 0.08f, z), new Vector3(0f, facing, 0f), frontageScale, facade, roof, glow, root);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OpenGridLotShadow_" + xIndex + "_" + zIndex, new Vector3(x + (x < 0f ? 2.8f : -2.8f), 0.19f, z + 0.85f), new Vector3(1.8f, 0.02f, 2.8f), puddle).transform.SetParent(root, true);

                    if ((xIndex + zIndex) % 3 == 0)
                    {
                        CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OpenGridStoreSign_" + xIndex + "_" + zIndex, new Vector3(x + (x < 0f ? 2.1f : -2.1f), 2.85f, z - 0.55f), new Vector3(1.1f, 0.34f, 0.08f), sign).transform.SetParent(root, true);
                    }
                }
            }

            for (var alley = 0; alley < 7; alley += 1)
            {
                var z = -42f + (alley * 14f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OpenGridAlleyWetCutWest_" + alley, new Vector3(-24.5f, 0.19f, z), new Vector3(4.6f, 0.02f, 0.82f), puddle).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OpenGridAlleyWetCutEast_" + alley, new Vector3(24.5f, 0.19f, z + 1.7f), new Vector3(4.6f, 0.02f, 0.82f), puddle).transform.SetParent(root, true);
            }

            CreateOpenGridHeroLandmark(district, prefix, root, facade, roof, metal, glow, sign, sidewalk, puddle, brass);

            var lightCorners = new[]
            {
                new Vector3(-22.15f, 0f, -28f),
                new Vector3(22.15f, 0f, -14f),
                new Vector3(-4.15f, 0f, 0f),
                new Vector3(4.15f, 0f, 14f),
                new Vector3(-22.15f, 0f, 28f),
                new Vector3(22.15f, 0f, 42f)
            };

            for (var index = 0; index < lightCorners.Length; index += 1)
            {
                CreateDistrictPointLight(prefix + "OpenGridCornerLight_" + index, root, lightCorners[index], glow, brass);
                CreateLowProfileStreetLamp(prefix + "OpenGridKitLamp_" + index, lightCorners[index] + new Vector3(0f, 0.08f, 0f), root);
            }

            CreateStaticVehicle(prefix + "OpenGridParkedSedanWest", new Vector3(-18f, 0.72f, -33.5f), new Vector3(1.78f, 0.92f, 4.05f), new Color(0.045f, 0.048f, 0.052f), roof, glow);
            CreateStaticVehicle(prefix + "OpenGridParkedSedanEast", new Vector3(18f, 0.72f, 17.5f), new Vector3(1.78f, 0.92f, 4.05f), new Color(0.16f, 0.045f, 0.04f), roof, glow);
            CreateStaticVehicle(prefix + "OpenGridParkedSedanNorth", new Vector3(5.4f, 0.72f, 35.8f), new Vector3(1.72f, 0.9f, 3.95f), new Color(0.07f, 0.066f, 0.058f), roof, glow);

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OpenGridDistrictMarquee", new Vector3(-3.2f, 3.35f, -28.35f), new Vector3(4.8f, 0.48f, 0.1f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "OpenGridDistrictMarqueeGlow", new Vector3(-3.2f, 3.37f, -28.28f), new Vector3(3.9f, 0.16f, 0.08f), glow).transform.SetParent(root, true);
        }

        private static void CreateOpenGridHeroLandmark(
            DistrictArtStyle district,
            string prefix,
            Transform root,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            if (district == DistrictArtStyle.Docks)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroHarborWaterSlip", new Vector3(-38.2f, 0.205f, -8f), new Vector3(7.4f, 0.03f, 58f), puddle).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroHarborPierA", new Vector3(-33.2f, 0.29f, -25f), new Vector3(9.2f, 0.18f, 1.15f), sidewalk).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroHarborPierB", new Vector3(-33.2f, 0.29f, 2.5f), new Vector3(9.2f, 0.18f, 1.15f), sidewalk).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroCraneMast", new Vector3(-29.4f, 4.15f, -21.5f), new Vector3(0.55f, 8.1f, 0.55f), metal).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroCraneBoom", new Vector3(-33.6f, 7.9f, -21.5f), new Vector3(8.4f, 0.32f, 0.32f), brass).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroCraneCabGlow", new Vector3(-30.2f, 6.5f, -20.9f), new Vector3(1.1f, 0.72f, 0.1f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroDockGateGlow", new Vector3(-5.9f, 2.65f, -31.4f), new Vector3(0.12f, 0.9f, 2.8f), glow).transform.SetParent(root, true);
                return;
            }

            if (district == DistrictArtStyle.BusinessCore)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroBankPlaza", new Vector3(-29.8f, 0.24f, -24.4f), new Vector3(11.5f, 0.12f, 9.2f), sidewalk).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroBankMass", new Vector3(-31f, 3.25f, -24.4f), new Vector3(6.2f, 6.5f, 6.9f), facade).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroBankRoofCap", new Vector3(-31f, 6.75f, -24.4f), new Vector3(6.9f, 0.46f, 7.4f), roof).transform.SetParent(root, true);
                for (var column = 0; column < 4; column += 1)
                {
                    CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "HeroBankColumn_" + column, new Vector3(-27.4f, 2.2f, -27.2f + (column * 1.85f)), new Vector3(0.34f, 2.35f, 0.34f), brass).transform.SetParent(root, true);
                }

                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroBankWarmWindows", new Vector3(-27.85f, 4.15f, -24.4f), new Vector3(0.12f, 1.8f, 4.6f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroBankMarquee", new Vector3(-27.7f, 2.95f, -24.4f), new Vector3(0.12f, 0.55f, 3.8f), sign).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroBankStreetClockGlow", new Vector3(-5.6f, 3.2f, -31.2f), new Vector3(0.12f, 0.7f, 0.7f), glow).transform.SetParent(root, true);
                return;
            }

            if (district == DistrictArtStyle.OldQuarter)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelCourtyard", new Vector3(-30.4f, 0.24f, -24.8f), new Vector3(10.4f, 0.1f, 10.8f), sidewalk).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelNave", new Vector3(-31.5f, 2.7f, -24.8f), new Vector3(5.1f, 5.4f, 8.2f), facade).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelRoof", new Vector3(-31.5f, 5.55f, -24.8f), new Vector3(5.6f, 0.55f, 8.8f), roof).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelTower", new Vector3(-27.8f, 5.2f, -28.1f), new Vector3(2.1f, 10.4f, 2.1f), facade).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "HeroChapelSteeple", new Vector3(-27.8f, 11.0f, -28.1f), new Vector3(1.25f, 3.4f, 1.25f), roof).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelRoseWindow", new Vector3(-26.72f, 5.6f, -28.1f), new Vector3(0.1f, 1.0f, 1.0f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelStreetCross", new Vector3(-26.62f, 12.55f, -28.1f), new Vector3(0.1f, 1.2f, 0.2f), brass).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroChapelStreetGlow", new Vector3(-5.6f, 3.35f, -31.2f), new Vector3(0.12f, 1.0f, 0.72f), glow).transform.SetParent(root, true);
                return;
            }

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroRailLoadingYard", new Vector3(-30.2f, 0.22f, -23.8f), new Vector3(12.4f, 0.08f, 11.4f), sidewalk).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroRailGantryTop", new Vector3(-30.2f, 5.85f, -23.8f), new Vector3(10.8f, 0.42f, 0.48f), brass).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroRailGantryLegA", new Vector3(-35.2f, 3.1f, -23.8f), new Vector3(0.42f, 6.2f, 0.42f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroRailGantryLegB", new Vector3(-25.2f, 3.1f, -23.8f), new Vector3(0.42f, 6.2f, 0.42f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "HeroRailTankA", new Vector3(-33.8f, 1.6f, -28.3f), new Vector3(2.0f, 1.65f, 2.0f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "HeroRailTankB", new Vector3(-28.1f, 1.6f, -28.3f), new Vector3(2.0f, 1.65f, 2.0f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "HeroRailStackA", new Vector3(-35.6f, 4.2f, -19.2f), new Vector3(0.62f, 8.4f, 0.62f), roof).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "HeroRailStackB", new Vector3(-27.3f, 3.7f, -18.7f), new Vector3(0.54f, 7.4f, 0.54f), roof).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroRailYardLampGlow", new Vector3(-29.7f, 4.2f, -23.35f), new Vector3(1.7f, 0.12f, 0.12f), glow).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "HeroRailStreetSignalGlow", new Vector3(-5.8f, 3.35f, -31.2f), new Vector3(0.12f, 0.85f, 1.4f), glow).transform.SetParent(root, true);
        }

        private static void CreateSouthNeighborhoodFill(
            DistrictArtStyle district,
            string prefix,
            Transform root,
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthBlockAsphaltExtension", new Vector3(0f, 0.1f, -35.4f), new Vector3(42f, 0.04f, 17.8f), asphalt).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthCrosswalkNorth", new Vector3(0f, 0.176f, -28.8f), new Vector3(8.8f, 0.02f, 0.34f), brass).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthCrosswalkSouth", new Vector3(0f, 0.176f, -36.8f), new Vector3(8.8f, 0.02f, 0.34f), brass).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthLeftWalk", new Vector3(-6.25f, 0.16f, -35.4f), new Vector3(2.2f, 0.08f, 17.4f), sidewalk).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthRightWalk", new Vector3(6.25f, 0.16f, -35.4f), new Vector3(2.2f, 0.08f, 17.4f), sidewalk).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthWetStreetSheetA", new Vector3(-2.35f, 0.18f, -32.4f), new Vector3(3.4f, 0.018f, 1.2f), puddle).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthWetStreetSheetB", new Vector3(2.8f, 0.18f, -39.2f), new Vector3(3.1f, 0.018f, 1.05f), puddle).transform.SetParent(root, true);

            PlaceDistrictBuilding(district, DistrictBuildingRole.Warehouse, prefix + "SouthAnchorLeftWarehouse", new Vector3(-12.7f, 0.08f, -33.6f), new Vector3(0f, 90f, 0f), new Vector3(2.65f, 2.55f, 2.35f), facade, roof, glow, root);
            PlaceDistrictBuilding(district, DistrictBuildingRole.CornerShop, prefix + "SouthAnchorRightCorner", new Vector3(12.5f, 0.08f, -35.3f), new Vector3(0f, -90f, 0f), new Vector3(2.35f, 2.4f, 2.2f), facade, roof, glow, root);
            PlaceDistrictBuilding(district, DistrictBuildingRole.NarrowTenement, prefix + "SouthBackLeftTenement", new Vector3(-21.4f, 0.04f, -37.8f), new Vector3(0f, 90f, 0f), new Vector3(1.9f, 2.15f, 1.8f), facade, roof, glow, root);
            PlaceDistrictBuilding(district, DistrictBuildingRole.OfficeBlock, prefix + "SouthBackRightOffice", new Vector3(21.4f, 0.04f, -32.6f), new Vector3(0f, -90f, 0f), new Vector3(1.9f, 2.2f, 1.9f), facade, roof, glow, root);

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "road-crossroad-line", prefix + "SouthKitCrossroadPrimary", new Vector3(0f, 0.14f, -32.5f), Vector3.zero, new Vector3(3.15f, 1f, 3.15f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "road-crossroad-line", prefix + "SouthKitCrossroadDeep", new Vector3(0f, 0.14f, -40f), Vector3.zero, new Vector3(2.75f, 1f, 2.75f), root);
            CreateLowProfileStreetLamp(prefix + "SouthReadyLampLeft", new Vector3(-5.3f, 0.08f, -31.6f), root);
            CreateLowProfileStreetLamp(prefix + "SouthReadyLampRight", new Vector3(5.3f, 0.08f, -38.6f), root);

            CreateStaticVehicle(prefix + "SouthParkedLongSedan", new Vector3(-2.8f, 0.72f, -35.8f), new Vector3(1.82f, 0.92f, 4.2f), new Color(0.08f, 0.075f, 0.07f), roof, glow);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthStackedCratesA", new Vector3(8.3f, 0.58f, -31.7f), new Vector3(1.1f, 1.1f, 1.1f), facade).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthStackedCratesB", new Vector3(9.15f, 0.48f, -32.75f), new Vector3(0.84f, 0.84f, 0.84f), facade).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthStorefrontSignLeft", new Vector3(-7.1f, 2.75f, -33.7f), new Vector3(1.2f, 0.36f, 0.09f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "SouthStorefrontSignRight", new Vector3(7.1f, 2.75f, -35.4f), new Vector3(1.2f, 0.36f, 0.09f), sign).transform.SetParent(root, true);
        }

        private static void CreateDistrictLightPools(string prefix, Transform root, Material glow, Material brass)
        {
            var points = new[]
            {
                new Vector3(-4.9f, 0f, -30.8f),
                new Vector3(4.9f, 0f, -24.1f),
                new Vector3(-4.8f, 0f, -12.6f),
                new Vector3(4.8f, 0f, -2.4f),
                new Vector3(-4.8f, 0f, 8.8f),
                new Vector3(4.8f, 0f, 18.2f)
            };

            for (var index = 0; index < points.Length; index += 1)
            {
                CreateDistrictPointLight(prefix + "WarmStreetPool_" + index, root, points[index], glow, brass);
            }
        }

        private static void CreateDistrictPointLight(string name, Transform root, Vector3 basePosition, Material glow, Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cylinder, name + "GlowPool", basePosition + new Vector3(0f, 0.18f, 0f), new Vector3(1.18f, 0.014f, 1.18f), glow).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "LampHead", basePosition + new Vector3(0f, 1.72f, 0.15f), new Vector3(0.28f, 0.14f, 0.28f), brass).transform.SetParent(root, true);

            var lightObject = new GameObject(name + "PointLight");
            lightObject.transform.SetParent(root, true);
            lightObject.transform.position = basePosition + new Vector3(0f, 1.55f, 0.15f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.69f, 0.34f);
            light.range = 3.8f;
            light.intensity = 0.72f;
            light.shadows = LightShadows.None;
        }

        private static void CreateDockProductionSignature(
            string prefix,
            Transform root,
            Material metal,
            Material glow,
            Material roof,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LongWaterPlane", new Vector3(-25f, -0.1f, 0f), new Vector3(8f, 0.04f, 58f), puddle).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "TimberPierRun", new Vector3(-20.8f, 0.18f, -5f), new Vector3(2.8f, 0.18f, 34f), sidewalk).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CargoCraneArm", new Vector3(-17.4f, 6.8f, 8f), new Vector3(0.28f, 0.28f, 10f), brass).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "CargoCraneMast", new Vector3(-17.4f, 3.4f, 8f), new Vector3(0.28f, 3.4f, 0.28f), metal).transform.SetParent(root, true);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "detail-tank", prefix + "HarborFuelTankSignatureA", new Vector3(-18.9f, 0.08f, 18.2f), Vector3.zero, new Vector3(2.6f, 2.6f, 2.6f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "construction-barrier", prefix + "HarborGateBarricade", new Vector3(-5.4f, 0.08f, -21.8f), Vector3.zero, new Vector3(1.7f, 1.7f, 1.7f), root);
            CreateStaticVehicle(prefix + "HarborServiceTruck", new Vector3(-4.6f, 0.76f, 19.4f), new Vector3(2.25f, 1.15f, 5.2f), new Color(0.16f, 0.17f, 0.16f), roof, glow);
        }

        private static void CreateBusinessProductionSignature(
            string prefix,
            Transform root,
            Material metal,
            Material glow,
            Material roof,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "building-skyscraper-b", prefix + "SkylineTowerA", new Vector3(-22.6f, 0.04f, 7.8f), new Vector3(0f, 90f, 0f), new Vector3(2.4f, 2.7f, 2.4f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "building-skyscraper-d", prefix + "SkylineTowerB", new Vector3(22.8f, 0.04f, -8.4f), new Vector3(0f, -90f, 0f), new Vector3(2.2f, 2.55f, 2.2f), root);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "UnionSquareStoneField", new Vector3(-2.8f, 0.18f, 17.4f), new Vector3(8.4f, 0.04f, 7.4f), sidewalk).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "UnionSquareFountain", new Vector3(-2.8f, 0.58f, 17.4f), new Vector3(1.25f, 0.24f, 1.25f), brass).transform.SetParent(root, true);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-parasol-a", prefix + "SquareParasolSignatureA", new Vector3(-5.7f, 0.08f, 15.2f), Vector3.zero, new Vector3(1.65f, 1.65f, 1.65f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "sign-highway-detailed", prefix + "DowntownWayfindingSignature", new Vector3(5.5f, 0.08f, -19.6f), new Vector3(0f, 180f, 0f), new Vector3(1.8f, 1.8f, 1.8f), root);
        }

        private static void CreateOldQuarterProductionSignature(
            string prefix,
            Transform root,
            Material metal,
            Material glow,
            Material roof,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitSuburban", "building-type-q", prefix + "ChapelMassReadyAsset", new Vector3(-18.8f, 0.06f, 15.4f), new Vector3(0f, 90f, 0f), new Vector3(2.9f, 2.8f, 2.9f), root);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "ChapelBellTower", new Vector3(-16.6f, 6.2f, 18.8f), new Vector3(0.65f, 3.5f, 0.65f), roof).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ChapelCross", new Vector3(-16.6f, 10.1f, 18.8f), new Vector3(0.18f, 1.15f, 0.18f), brass).transform.SetParent(root, true);
            for (var line = 0; line < 5; line += 1)
            {
                var z = -15.5f + (line * 7.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LaundrySheetFinished_" + line, new Vector3(-1.7f + (line * 0.55f), 5.35f, z), new Vector3(0.55f, 0.65f, 0.035f), sign).transform.SetParent(root, true);
            }
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitSuburban", "tree-small", prefix + "QuarterCourtyardTree", new Vector3(4.8f, 0.08f, 19.2f), Vector3.zero, new Vector3(2f, 2f, 2f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitSuburban", "fence-1x4", prefix + "ChapelFenceSignature", new Vector3(-6.2f, 0.08f, 17.2f), new Vector3(0f, 90f, 0f), new Vector3(2f, 2f, 2f), root);
        }

        private static void CreateRailProductionSignature(
            string prefix,
            Transform root,
            Material metal,
            Material glow,
            Material roof,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            for (var track = 0; track < 3; track += 1)
            {
                var x = -3.4f + (track * 3.4f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "FinishedRailA_" + track, new Vector3(x - 0.42f, 0.16f, 10f), new Vector3(0.08f, 0.035f, 34f), metal).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "FinishedRailB_" + track, new Vector3(x + 0.42f, 0.16f, 10f), new Vector3(0.08f, 0.035f, 34f), metal).transform.SetParent(root, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "BoxcarSignatureA", new Vector3(1.6f, 1.15f, 15.8f), new Vector3(2.3f, 2.15f, 8.2f), roof).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "BoxcarSignatureDoor", new Vector3(1.6f, 1.15f, 11.9f), new Vector3(1.7f, 1.55f, 0.08f), sign).transform.SetParent(root, true);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "building-r", prefix + "FoundrySignatureAsset", new Vector3(-21.4f, 0.08f, 12.4f), new Vector3(0f, 90f, 0f), new Vector3(2.8f, 2.65f, 2.8f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "chimney-large", prefix + "FoundryStackSignatureA", new Vector3(-18.8f, 0.08f, 6.2f), Vector3.zero, new Vector3(2.25f, 2.25f, 2.25f), root);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitIndustrial", "detail-tank", prefix + "RailTankSignatureA", new Vector3(18.6f, 0.08f, -14.6f), Vector3.zero, new Vector3(2.4f, 2.4f, 2.4f), root);
            CreateLowProfileStreetLamp(prefix + "YardWorkLightSignature", new Vector3(4.8f, 0.08f, -20.2f), root);
        }

        private static void CreateDistrictRooftopDetailPass(
            string prefix,
            float leftRoofX,
            float rightRoofX,
            Material metal,
            Material glow,
            Material roofDetail,
            Material sign,
            Material brass)
        {
            for (var index = 0; index < 5; index += 1)
            {
                var z = -13.5f + (index * 6.2f);
                var leftName = prefix + "LeftRoof";
                var rightName = prefix + "RightRoof";

                CreateVisualPrimitive(PrimitiveType.Cube, leftName + "TarSeam_" + index, new Vector3(leftRoofX, 6.9f, z), new Vector3(4.6f, 0.025f, 0.06f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, rightName + "TarSeam_" + index, new Vector3(rightRoofX, 6.9f, z + 1.6f), new Vector3(4.6f, 0.025f, 0.06f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, leftName + "VentBox_" + index, new Vector3(leftRoofX + 0.9f, 7.18f, z + 1.2f), new Vector3(0.72f, 0.48f, 0.62f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, rightName + "VentBox_" + index, new Vector3(rightRoofX - 0.75f, 7.18f, z - 0.7f), new Vector3(0.72f, 0.48f, 0.62f), metal);
                CreateVisualPrimitive(PrimitiveType.Cylinder, leftName + "SteamCap_" + index, new Vector3(leftRoofX - 1.25f, 7.32f, z - 1.1f), new Vector3(0.22f, 0.34f, 0.22f), brass);
                CreateVisualPrimitive(PrimitiveType.Cylinder, rightName + "SteamCap_" + index, new Vector3(rightRoofX + 1.15f, 7.32f, z + 0.9f), new Vector3(0.22f, 0.34f, 0.22f), brass);

                if (index % 2 == 0)
                {
                    CreateVisualPrimitive(PrimitiveType.Cube, leftName + "SkylightGlow_" + index, new Vector3(leftRoofX - 0.6f, 7.02f, z + 2.2f), new Vector3(1.1f, 0.035f, 0.82f), glow);
                    CreateVisualPrimitive(PrimitiveType.Cube, rightName + "SkylightFrame_" + index, new Vector3(rightRoofX + 0.45f, 7.04f, z + 2.5f), new Vector3(1.25f, 0.05f, 0.95f), roofDetail);
                }
            }

            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "LeftWaterTowerTank", new Vector3(leftRoofX + 1.25f, 8.12f, 12.8f), new Vector3(0.92f, 0.8f, 0.92f), roofDetail);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "LeftWaterTowerLegs", new Vector3(leftRoofX + 1.25f, 7.45f, 12.8f), new Vector3(0.66f, 0.52f, 0.66f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightStairwellBulkhead", new Vector3(rightRoofX - 1.15f, 7.42f, -14.2f), new Vector3(1.5f, 1.05f, 1.75f), roofDetail);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightBulkheadDoor", new Vector3(rightRoofX - 1.15f, 7.5f, -13.28f), new Vector3(0.78f, 0.62f, 0.08f), sign);
        }

        private static void CreateNoirSpawnComposition(
            string prefix,
            Vector3 anchor,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material masonry,
            Material puddle,
            Material brass,
            string marqueeText)
        {
            var streetZ = anchor.z + 3.8f;
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "ForegroundWetEdge", anchor + new Vector3(0f, 0.095f, -8.6f), new Vector3(10.8f, 0.018f, 0.18f), puddle);
            CreateColliderBlock(prefix + "LeftFacadeCollision", anchor + new Vector3(-7.25f, 2.2f, -1.2f), new Vector3(2.4f, 4.4f, 7.4f));
            CreateColliderBlock(prefix + "RightFacadeCollision", anchor + new Vector3(7.15f, 2.4f, 1.2f), new Vector3(2.2f, 4.8f, 8.6f));
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftAwning", anchor + new Vector3(-5.85f, 2.35f, 1.1f), new Vector3(1.15f, 0.24f, 2.9f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightAwning", anchor + new Vector3(5.85f, 2.45f, 4.4f), new Vector3(1.15f, 0.24f, 3.2f), sign);

            for (var index = 0; index < 4; index += 1)
            {
                var z = anchor.z - 2.8f + (index * 1.85f);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftWindowFrame_" + index, new Vector3(-5.98f, 3.0f, z), new Vector3(0.11f, 0.9f, 1.0f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "LeftWindowGlow_" + index, new Vector3(-6.04f, 3.0f, z), new Vector3(0.045f, 0.52f, 0.48f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightWindowFrame_" + index, new Vector3(5.98f, 3.2f, z + 0.7f), new Vector3(0.11f, 0.86f, 1.04f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RightWindowGlow_" + index, new Vector3(6.04f, 3.2f, z + 0.7f), new Vector3(0.045f, 0.5f, 0.5f), glow);
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WetReflection_" + index, new Vector3(index % 2 == 0 ? -1.8f : 2.0f, 0.13f, z + 0.45f), new Vector3(1.6f, 0.018f, 0.42f), puddle);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "MarqueeBack", anchor + new Vector3(0f, 3.85f, streetZ), new Vector3(5.8f, 0.42f, 0.28f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "MarqueeGlow", anchor + new Vector3(0f, 3.86f, streetZ - 0.18f), new Vector3(5.2f, 0.16f, 0.08f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "MarqueeBulbLeft", anchor + new Vector3(-2.55f, 3.9f, streetZ - 0.26f), new Vector3(0.22f, 0.22f, 0.06f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "MarqueeBulbRight", anchor + new Vector3(2.55f, 3.9f, streetZ - 0.26f), new Vector3(0.22f, 0.22f, 0.06f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "MarqueeTextHintA", anchor + new Vector3(-1.15f, 4.18f, streetZ - 0.38f), new Vector3(1.2f, 0.08f, 0.06f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "MarqueeTextHintB", anchor + new Vector3(0.65f, 4.18f, streetZ - 0.38f), new Vector3(1.65f, 0.08f, 0.06f), brass);

            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrosswalkA", anchor + new Vector3(0f, 0.135f, 1.7f), new Vector3(7.2f, 0.035f, 0.22f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CrosswalkB", anchor + new Vector3(0f, 0.135f, 2.45f), new Vector3(7.2f, 0.035f, 0.22f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "CafeTable", anchor + new Vector3(-5.3f, 0.72f, 4.7f), new Vector3(0.85f, 0.18f, 0.85f), wood);
            CreateVisualPrimitive(PrimitiveType.Cylinder, prefix + "CafeLamp", anchor + new Vector3(-5.3f, 1.32f, 4.7f), new Vector3(0.18f, 0.45f, 0.18f), glow);
            CreateStaticVehicle(prefix + "ParkedCoupe", anchor + new Vector3(2.85f, 0.72f, 5.7f), new Vector3(1.65f, 0.92f, 3.55f), new Color(0.08f, 0.08f, 0.1f), metal, glow);

            var lightObject = new GameObject(prefix + "MarqueeLight");
            lightObject.transform.position = anchor + new Vector3(0f, 3.6f, streetZ - 0.6f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 7.25f;
            light.intensity = 2.15f;
            light.color = new Color(1f, 0.66f, 0.28f);
        }

        private static void CreateDockStartDetailPass(
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material masonry,
            Material puddle,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, "StartLeftStorefrontBase", new Vector3(-7.58f, 1.16f, -11.4f), new Vector3(0.28f, 2.25f, 5.8f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartLeftStorefrontTrimA", new Vector3(-7.38f, 2.32f, -11.4f), new Vector3(0.14f, 0.16f, 5.55f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartLeftStorefrontTrimB", new Vector3(-7.38f, 0.42f, -11.4f), new Vector3(0.14f, 0.16f, 5.55f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartCafeAwning", new Vector3(-5.72f, 2.08f, -10.1f), new Vector3(2.05f, 0.22f, 2.4f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartCafeAwningLip", new Vector3(-4.7f, 1.86f, -10.1f), new Vector3(0.16f, 0.18f, 2.4f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartCafeDoor", new Vector3(-5.08f, 1.02f, -12.8f), new Vector3(0.14f, 1.8f, 0.78f), wood);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartCafeDoorGlow", new Vector3(-4.98f, 1.28f, -12.8f), new Vector3(0.045f, 0.62f, 0.42f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartDoorStepA", new Vector3(-4.72f, 0.23f, -12.8f), new Vector3(0.92f, 0.12f, 1.1f), masonry);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartDoorStepB", new Vector3(-4.44f, 0.32f, -12.8f), new Vector3(0.58f, 0.12f, 0.82f), masonry);

            for (var level = 0; level < 3; level += 1)
            {
                var y = 2.65f + (level * 0.78f);
                CreateVisualPrimitive(PrimitiveType.Cube, "StartFireEscapeRail_" + level, new Vector3(-5.58f, y, -14.1f), new Vector3(0.08f, 0.07f, 2.8f), metal);
                CreateVisualPrimitive(PrimitiveType.Cube, "StartFireEscapeDeck_" + level, new Vector3(-5.72f, y - 0.22f, -14.1f), new Vector3(0.7f, 0.08f, 2.6f), metal);
            }

            for (var rung = 0; rung < 7; rung += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "StartFireEscapeLadder_" + rung, new Vector3(-5.48f, 1.72f + (rung * 0.34f), -12.9f), new Vector3(0.07f, 0.06f, 0.76f), metal);
            }

            for (var seam = 0; seam < 5; seam += 1)
            {
                var z = -16.8f + (seam * 2.4f);
                CreateVisualPrimitive(PrimitiveType.Cube, "StartSidewalkSeamL_" + seam, new Vector3(-6.2f, 0.205f, z), new Vector3(2.72f, 0.024f, 0.045f), sign);
                CreateVisualPrimitive(PrimitiveType.Cube, "StartSidewalkSeamR_" + seam, new Vector3(6.2f, 0.205f, z + 0.8f), new Vector3(2.72f, 0.024f, 0.045f), sign);
            }

            CreateStaticVehicle("StartParkedBlackSedan", new Vector3(3.25f, 0.72f, -11.15f), new Vector3(1.42f, 0.86f, 3.25f), new Color(0.055f, 0.058f, 0.064f), metal, glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartSedanRoadShadow", new Vector3(3.25f, 0.122f, -11.15f), new Vector3(1.62f, 0.018f, 3.55f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartWetGutterLeft", new Vector3(-4.72f, 0.13f, -11.6f), new Vector3(0.38f, 0.018f, 6.6f), puddle);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartWetGutterRight", new Vector3(4.72f, 0.13f, -12.2f), new Vector3(0.38f, 0.018f, 4.2f), puddle);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "StartTrashCanA", new Vector3(-5.48f, 0.56f, -8.35f), new Vector3(0.32f, 0.52f, 0.32f), metal);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "StartTrashCanB", new Vector3(-5.95f, 0.52f, -8.1f), new Vector3(0.28f, 0.46f, 0.28f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartNewspaperBundle", new Vector3(-5.28f, 0.36f, -9.15f), new Vector3(0.72f, 0.22f, 0.48f), masonry);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartShopSignBack", new Vector3(-5.12f, 2.65f, -10.1f), new Vector3(0.12f, 0.62f, 1.82f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartShopSignWarmLine", new Vector3(-5.04f, 2.78f, -10.1f), new Vector3(0.045f, 0.08f, 1.42f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartShopSignLetterHintA", new Vector3(-5.0f, 3.03f, -10.55f), new Vector3(0.045f, 0.42f, 0.06f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartShopSignLetterHintB", new Vector3(-5.0f, 3.03f, -10.18f), new Vector3(0.045f, 0.42f, 0.06f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "StartShopSignLetterHintC", new Vector3(-5.0f, 3.03f, -9.8f), new Vector3(0.045f, 0.42f, 0.06f), brass);
        }

        private static void CreateSteamReadyDocksCityPass(
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material water,
            Material stone,
            Material brass,
            Material lanePaint)
        {
            var root = new GameObject("SteamDocks_AuthoredCitySlice");

            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_CityGround", new Vector3(0f, 0.0f, 0f), new Vector3(56f, 0.08f, 78f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_HarborWater", new Vector3(-31f, -0.12f, 0f), new Vector3(11f, 0.08f, 78f), water).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_DockBoardwalk", new Vector3(-23.4f, 0.16f, 0f), new Vector3(4.4f, 0.18f, 74f), stone).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_HarborRail", new Vector3(-25.7f, 0.76f, 0f), new Vector3(0.16f, 1.1f, 72f), metal).transform.SetParent(root.transform, true);

            var northSouthRoads = new[] { -15f, 0f, 15f };
            foreach (var x in northSouthRoads)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_NS_Asphalt_" + x, new Vector3(x, 0.12f, 0f), new Vector3(6.8f, 0.08f, 72f), asphalt).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_NS_WestSidewalk_" + x, new Vector3(x - 4.7f, 0.22f, 0f), new Vector3(2.2f, 0.12f, 72f), sidewalk).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_NS_EastSidewalk_" + x, new Vector3(x + 4.7f, 0.22f, 0f), new Vector3(2.2f, 0.12f, 72f), sidewalk).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_NS_WestCurb_" + x, new Vector3(x - 3.58f, 0.32f, 0f), new Vector3(0.22f, 0.22f, 72f), stone).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_NS_EastCurb_" + x, new Vector3(x + 3.58f, 0.32f, 0f), new Vector3(0.22f, 0.22f, 72f), stone).transform.SetParent(root.transform, true);
            }

            var eastWestRoads = new[] { -24f, -8f, 10f, 27f };
            foreach (var z in eastWestRoads)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_EW_Asphalt_" + z, new Vector3(0f, 0.13f, z), new Vector3(48f, 0.08f, 6.4f), asphalt).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_EW_NorthCurb_" + z, new Vector3(0f, 0.33f, z + 3.35f), new Vector3(48f, 0.22f, 0.22f), stone).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_EW_SouthCurb_" + z, new Vector3(0f, 0.33f, z - 3.35f), new Vector3(48f, 0.22f, 0.22f), stone).transform.SetParent(root.transform, true);
            }

            for (var stripe = 0; stripe < 12; stripe += 1)
            {
                var z = -32f + (stripe * 5.6f);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_MainLaneStripe_" + stripe, new Vector3(0.9f, 0.18f, z), new Vector3(0.18f, 0.025f, 1.65f), lanePaint).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_WestLaneStripe_" + stripe, new Vector3(-14.1f, 0.18f, z + 1.5f), new Vector3(0.18f, 0.025f, 1.45f), lanePaint).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_EastLaneStripe_" + stripe, new Vector3(15.9f, 0.18f, z - 0.8f), new Vector3(0.18f, 0.025f, 1.45f), lanePaint).transform.SetParent(root.transform, true);
            }

            for (var cross = 0; cross < eastWestRoads.Length; cross += 1)
            {
                var z = eastWestRoads[cross];
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_CrosswalkA_" + cross, new Vector3(0f, 0.19f, z - 2.15f), new Vector3(8.5f, 0.025f, 0.22f), brass).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_CrosswalkB_" + cross, new Vector3(0f, 0.19f, z - 1.45f), new Vector3(8.5f, 0.025f, 0.22f), brass).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_CrossStreetWetRead_" + cross, new Vector3(-1.4f, 0.17f, z + 1.4f), new Vector3(14.5f, 0.018f, 0.75f), puddle).transform.SetParent(root.transform, true);
            }

            var westRoles = new[]
            {
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.Warehouse
            };
            var eastRoles = new[]
            {
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.OfficeBlock
            };

            for (var index = 0; index < westRoles.Length; index += 1)
            {
                var z = -31f + (index * 10.2f);
                PlaceDistrictBuilding(DistrictArtStyle.Docks, westRoles[index], "SteamDocks_WaterfrontBlock_" + index, new Vector3(-20.2f, 0.08f, z), new Vector3(0f, 90f, 0f), new Vector3(2.45f, 2.65f + ((index % 2) * 0.25f), 2.35f), facade, roof, glow, root.transform);
                PlaceDistrictBuilding(DistrictArtStyle.Docks, eastRoles[index], "SteamDocks_CentralWestFrontage_" + index, new Vector3(-5.1f, 0.08f, z + 2.2f), new Vector3(0f, 90f, 0f), new Vector3(2.72f, 2.82f + ((index % 3) * 0.2f), 2.45f), facade, roof, glow, root.transform);
                PlaceDistrictBuilding(DistrictArtStyle.Docks, westRoles[(index + 2) % westRoles.Length], "SteamDocks_CentralEastFrontage_" + index, new Vector3(5.1f, 0.08f, z - 1.4f), new Vector3(0f, -90f, 0f), new Vector3(2.72f, 2.82f + (((index + 1) % 3) * 0.2f), 2.45f), facade, roof, glow, root.transform);
                PlaceDistrictBuilding(DistrictArtStyle.Docks, eastRoles[(index + 3) % eastRoles.Length], "SteamDocks_BacklotFrontage_" + index, new Vector3(20.2f, 0.08f, z + 3.5f), new Vector3(0f, -90f, 0f), new Vector3(2.4f, 2.55f + ((index % 2) * 0.24f), 2.3f), facade, roof, glow, root.transform);
            }

            CreateSteamDocksOpeningCanyon(root.transform, facade, roof, metal, glow, wood, sign, puddle, brass);

            CreateColliderBlock("SteamDocks_WestHarborBoundary", new Vector3(-27.2f, 1.4f, 0f), new Vector3(0.8f, 2.8f, 78f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("SteamDocks_EastCityBoundary", new Vector3(27.2f, 1.4f, 0f), new Vector3(0.8f, 2.8f, 78f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("SteamDocks_SouthCityBoundary", new Vector3(0f, 1.4f, -38.3f), new Vector3(56f, 2.8f, 0.8f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("SteamDocks_NorthCityBoundary", new Vector3(0f, 1.4f, 38.3f), new Vector3(56f, 2.8f, 0.8f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("SteamDocks_WaterfrontWarehouseMass", new Vector3(-20.8f, 2.5f, 4f), new Vector3(3.0f, 5.0f, 61f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("SteamDocks_BacklotWarehouseMass", new Vector3(20.8f, 2.5f, 3f), new Vector3(3.0f, 5.0f, 61f)).transform.SetParent(root.transform, true);

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-awning-wide", "SteamDocks_MorettiAwningReadyAsset", new Vector3(-7.45f, 2.05f, -22.8f), new Vector3(0f, 90f, 0f), new Vector3(1.75f, 1.75f, 1.75f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-overhang-wide", "SteamDocks_BackOfficeCanopyReadyAsset", new Vector3(0f, 2.35f, 15.6f), new Vector3(0f, 0f, 0f), new Vector3(1.95f, 1.95f, 1.95f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "construction-barrier", "SteamDocks_RoadworkBarrierA", new Vector3(9.2f, 0.22f, -13.4f), new Vector3(0f, 20f, 0f), new Vector3(1.3f, 1.3f, 1.3f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "construction-light", "SteamDocks_RoadworkLightA", new Vector3(10.1f, 0.22f, -12.4f), new Vector3(0f, 42f, 0f), new Vector3(1.25f, 1.25f, 1.25f), root.transform);

            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_BackOfficeMarquee", new Vector3(0f, 3.6f, 15.7f), new Vector3(5.8f, 0.5f, 0.22f), sign).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_BackOfficeWarmLetters", new Vector3(0f, 3.72f, 15.52f), new Vector3(4.85f, 0.13f, 0.08f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_MarketSign", new Vector3(-7.3f, 3.15f, -23.1f), new Vector3(0.14f, 0.68f, 3.7f), sign).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_MarketSignGlow", new Vector3(-7.15f, 3.17f, -23.1f), new Vector3(0.055f, 0.18f, 3.15f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_HarborCraneMast", new Vector3(-24.2f, 3.4f, -9.5f), new Vector3(0.55f, 6.8f, 0.55f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_HarborCraneBoom", new Vector3(-20.8f, 6.25f, -9.5f), new Vector3(6.8f, 0.24f, 0.24f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_CraneHook", new Vector3(-18.1f, 3.75f, -9.5f), new Vector3(0.28f, 0.34f, 0.28f), brass).transform.SetParent(root.transform, true);

            for (var index = 0; index < 12; index += 1)
            {
                var z = -32f + (index * 6f);
                CreatePrimitive(PrimitiveType.Cylinder, "SteamDocks_HarborBollard_" + index, new Vector3(-25.1f, 0.55f, z), new Vector3(0.24f, 0.8f, 0.24f), metal).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_WarehouseCrate_" + index, new Vector3(-18.3f + ((index % 3) * 0.55f), 0.58f + ((index % 2) * 0.24f), z + 1.5f), new Vector3(0.86f, 0.72f, 0.72f), wood).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_WetGutter_" + index, new Vector3(index % 2 == 0 ? -3.2f : 3.2f, 0.17f, z + 2.4f), new Vector3(0.42f, 0.018f, 2.4f), puddle).transform.SetParent(root.transform, true);
            }

            CreateStaticVehicle("SteamDocks_ParkedTaxiForeground", new Vector3(-2.85f, 0.72f, -30.4f), new Vector3(1.7f, 0.9f, 3.85f), new Color(0.13f, 0.095f, 0.036f), roof, glow);
            CreateStaticVehicle("SteamDocks_ParkedDeliveryTruck", new Vector3(-14.9f, 0.78f, -2.5f), new Vector3(2.25f, 1.15f, 5.0f), new Color(0.1f, 0.11f, 0.105f), roof, glow);
            CreateStaticVehicle("SteamDocks_ParkedMaroonSedan", new Vector3(15.1f, 0.72f, 12.8f), new Vector3(1.72f, 0.9f, 3.85f), new Color(0.18f, 0.045f, 0.04f), roof, glow);
            CreateStaticVehicle("SteamDocks_ParkedPoliceWatch", new Vector3(14.9f, 0.72f, -25.8f), new Vector3(1.78f, 0.92f, 4.05f), new Color(0.04f, 0.048f, 0.058f), roof, glow);

            var lampPositions = new[]
            {
                new Vector3(-4.7f, 0.1f, -26.2f),
                new Vector3(4.7f, 0.1f, -19.8f),
                new Vector3(-4.7f, 0.1f, -6.2f),
                new Vector3(4.7f, 0.1f, 5.6f),
                new Vector3(-4.7f, 0.1f, 16.2f),
                new Vector3(4.7f, 0.1f, 25.5f),
                new Vector3(-18.8f, 0.1f, -17.3f),
                new Vector3(18.8f, 0.1f, 21.4f)
            };

            for (var index = 0; index < lampPositions.Length; index += 1)
            {
                CreateLowProfileStreetLamp("SteamDocks_StreetLamp_" + index, lampPositions[index], root.transform);
            }

            var keyLight = new GameObject("SteamDocks_WarmMarketKey");
            keyLight.transform.SetParent(root.transform, true);
            keyLight.transform.position = new Vector3(-4.2f, 4.5f, -22.5f);
            var light = keyLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 14f;
            light.intensity = 2.3f;
            light.color = new Color(1f, 0.66f, 0.32f);
        }

        private static void CreateSteamDocksOpeningCanyon(
            Transform root,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material puddle,
            Material brass)
        {
            const float laneCenterX = -15f;
            var leftRoles = new[]
            {
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.OfficeBlock
            };
            var rightRoles = new[]
            {
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.NarrowTenement
            };

            for (var index = 0; index < leftRoles.Length; index += 1)
            {
                var z = -31f + (index * 7.2f);
                PlaceDistrictBuilding(
                    DistrictArtStyle.Docks,
                    leftRoles[index],
                    "SteamDocks_OpeningLeftPlayableFacade_" + index,
                    new Vector3(laneCenterX - 4.35f, 0.08f, z),
                    new Vector3(0f, 90f, 0f),
                    new Vector3(3.05f, 3.15f + ((index % 2) * 0.22f), 2.72f),
                    facade,
                    roof,
                    glow,
                    root);
                PlaceDistrictBuilding(
                    DistrictArtStyle.Docks,
                    rightRoles[index],
                    "SteamDocks_OpeningRightPlayableFacade_" + index,
                    new Vector3(laneCenterX + 4.35f, 0.08f, z + 2.4f),
                    new Vector3(0f, -90f, 0f),
                    new Vector3(3.0f, 3.08f + (((index + 1) % 2) * 0.24f), 2.7f),
                    facade,
                    roof,
                    glow,
                    root);
            }

            CreateColliderBlock("SteamDocks_OpeningLeftStreetWallCollider", new Vector3(laneCenterX - 3.65f, 2.35f, -20.6f), new Vector3(0.45f, 4.7f, 27f)).transform.SetParent(root, true);
            CreateColliderBlock("SteamDocks_OpeningRightStreetWallCollider", new Vector3(laneCenterX + 3.65f, 2.35f, -19.4f), new Vector3(0.45f, 4.7f, 27f)).transform.SetParent(root, true);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -31f + (index * 6f);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningLeftCloseFacade_" + index, new Vector3(laneCenterX - 3.55f, 2.25f, z), new Vector3(0.34f, 4.5f, 5.2f), facade).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningRightCloseFacade_" + index, new Vector3(laneCenterX + 3.55f, 2.2f, z + 1.5f), new Vector3(0.34f, 4.4f, 5.2f), facade).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningLeftCornice_" + index, new Vector3(laneCenterX - 3.22f, 4.6f, z), new Vector3(0.42f, 0.28f, 5.45f), roof).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningRightCornice_" + index, new Vector3(laneCenterX + 3.22f, 4.52f, z + 1.5f), new Vector3(0.42f, 0.28f, 5.45f), roof).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningLeftLitWindowsA_" + index, new Vector3(laneCenterX - 3.2f, 2.65f, z - 1.35f), new Vector3(0.16f, 0.7f, 1.12f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningLeftLitWindowsB_" + index, new Vector3(laneCenterX - 3.2f, 3.35f, z + 1.35f), new Vector3(0.16f, 0.6f, 1.02f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningRightLitWindowsA_" + index, new Vector3(laneCenterX + 3.2f, 2.6f, z + 0.35f), new Vector3(0.16f, 0.66f, 1.1f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningRightLitWindowsB_" + index, new Vector3(laneCenterX + 3.2f, 3.22f, z + 2.55f), new Vector3(0.16f, 0.58f, 1.0f), glow).transform.SetParent(root, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningRightNeonGarageBlade", new Vector3(laneCenterX + 3.0f, 2.85f, -22.9f), new Vector3(0.2f, 0.42f, 3.6f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningRightNeonGarageGlow", new Vector3(laneCenterX + 2.86f, 2.88f, -22.9f), new Vector3(0.08f, 0.16f, 3.0f), glow).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningLeftClubCanopy", new Vector3(laneCenterX - 2.98f, 1.85f, -25.6f), new Vector3(0.32f, 0.18f, 3.2f), sign).transform.SetParent(root, true);

            EnvironmentArtCatalog.CreateKenneyFallback(
                "CityKitCommercial",
                "detail-awning-wide",
                "SteamDocks_OpeningMorettiAwning",
                new Vector3(laneCenterX - 2.85f, 1.82f, -23.25f),
                new Vector3(0f, 90f, 0f),
                new Vector3(1.9f, 1.9f, 1.9f),
                root);
            EnvironmentArtCatalog.CreateKenneyFallback(
                "CityKitRoads",
                "mailbox",
                "SteamDocks_OpeningMailbox",
                new Vector3(laneCenterX - 2.75f, 0.22f, -20.2f),
                new Vector3(0f, 95f, 0f),
                new Vector3(1.08f, 1.08f, 1.08f),
                root);
            EnvironmentArtCatalog.CreateKenneyFallback(
                "CityKitRoads",
                "trashcan",
                "SteamDocks_OpeningTrashCan",
                new Vector3(laneCenterX + 2.75f, 0.22f, -17.65f),
                new Vector3(0f, -35f, 0f),
                new Vector3(1.12f, 1.12f, 1.12f),
                root);
            EnvironmentArtCatalog.CreateKenneyFallback(
                "CityKitRoads",
                "construction-barrier",
                "SteamDocks_OpeningCurbBarrier",
                new Vector3(laneCenterX + 2.7f, 0.22f, -28.8f),
                new Vector3(0f, -12f, 0f),
                new Vector3(1.38f, 1.38f, 1.38f),
                root);

            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningCafeBladeSign", new Vector3(laneCenterX - 2.72f, 3.1f, -22.7f), new Vector3(0.12f, 0.68f, 2.65f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningCafeBladeGlow", new Vector3(laneCenterX - 2.62f, 3.12f, -22.7f), new Vector3(0.045f, 0.16f, 2.22f), glow).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningGarageSign", new Vector3(laneCenterX + 2.72f, 3.0f, -15.4f), new Vector3(0.12f, 0.62f, 2.55f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningGarageSignGlow", new Vector3(laneCenterX + 2.62f, 3.02f, -15.4f), new Vector3(0.045f, 0.14f, 2.1f), glow).transform.SetParent(root, true);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -29.5f + (index * 5.2f);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningFireEscapeLeft_" + index, new Vector3(laneCenterX - 2.72f, 3.1f, z), new Vector3(0.12f, 0.08f, 1.8f), metal).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningFireEscapeRailLeft_" + index, new Vector3(laneCenterX - 2.63f, 3.42f, z), new Vector3(0.06f, 0.52f, 1.8f), metal).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningWindowGlowLeft_" + index, new Vector3(laneCenterX - 2.68f, 2.72f, z + 1.45f), new Vector3(0.055f, 0.52f, 0.82f), glow).transform.SetParent(root, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningWindowGlowRight_" + index, new Vector3(laneCenterX + 2.68f, 2.62f, z + 2.2f), new Vector3(0.055f, 0.46f, 0.78f), glow).transform.SetParent(root, true);
            }

            for (var index = 0; index < 8; index += 1)
            {
                var z = -31.8f + (index * 4.6f);
                var x = laneCenterX + (index % 2 == 0 ? -2.85f : 2.85f);
                CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningWetPatch_" + index, new Vector3(x, 0.205f, z), new Vector3(0.82f, 0.014f, 1.85f), puddle).transform.SetParent(root, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningStackedCrateA", new Vector3(laneCenterX - 2.55f, 0.55f, -26.7f), new Vector3(0.82f, 0.72f, 0.74f), wood).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "SteamDocks_OpeningStackedCrateB", new Vector3(laneCenterX - 2.52f, 1.18f, -26.68f), new Vector3(0.72f, 0.62f, 0.64f), wood).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "SteamDocks_OpeningCornerBollardA", new Vector3(laneCenterX - 2.8f, 0.55f, -24.25f), new Vector3(0.18f, 0.68f, 0.18f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "SteamDocks_OpeningCornerBollardB", new Vector3(laneCenterX + 2.8f, 0.55f, -24.25f), new Vector3(0.18f, 0.68f, 0.18f), metal).transform.SetParent(root, true);

            CreateLowProfileStreetLamp("SteamDocks_OpeningHeroLampLeft", new Vector3(laneCenterX - 2.95f, 0.1f, -23.6f), root);
            CreateLowProfileStreetLamp("SteamDocks_OpeningHeroLampRight", new Vector3(laneCenterX + 2.95f, 0.1f, -16.6f), root);

            var practicalLight = new GameObject("SteamDocks_OpeningStorefrontPractical");
            practicalLight.transform.SetParent(root, true);
            practicalLight.transform.position = new Vector3(laneCenterX - 2.55f, 3.05f, -22.9f);
            var light = practicalLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 8f;
            light.intensity = 2.1f;
            light.color = new Color(1f, 0.68f, 0.32f);

            CreateStaticVehicle("SteamDocks_OpeningParkedDeliveryTruck", new Vector3(laneCenterX - 2.9f, 0.78f, -12.2f), new Vector3(2.15f, 1.08f, 4.75f), new Color(0.09f, 0.105f, 0.11f), roof, glow);
            CreateStaticVehicle("SteamDocks_OpeningParkedMaroonCoupe", new Vector3(laneCenterX + 2.95f, 0.72f, -27.9f), new Vector3(1.64f, 0.88f, 3.7f), new Color(0.17f, 0.045f, 0.038f), roof, brass);
        }

        private static void CreateAuthoredDocksVerticalSlicePass(
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("Authoring_DocksVerticalSlice_ProductionBlock");

            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceReadableWetAsphalt", new Vector3(0f, 0.215f, 0.5f), new Vector3(8.4f, 0.07f, 54f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceHarborSideStreet", new Vector3(-8.2f, 0.205f, -12.8f), new Vector3(8.2f, 0.055f, 4.8f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceWarehouseSideStreet", new Vector3(8.2f, 0.205f, 6.8f), new Vector3(8.2f, 0.055f, 4.8f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceWestCurbStone", new Vector3(-4.18f, 0.31f, 0.5f), new Vector3(0.28f, 0.24f, 54f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceEastCurbStone", new Vector3(4.18f, 0.31f, 0.5f), new Vector3(0.28f, 0.24f, 54f), sidewalk).transform.SetParent(root.transform, true);

            for (var index = 0; index < 9; index += 1)
            {
                var z = -21f + (index * 5.3f);
                CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceBrokenLanePaint_" + index, new Vector3(0f, 0.262f, z), new Vector3(0.26f, 0.018f, 1.9f), brass).transform.SetParent(root.transform, true);
            }

            for (var index = 0; index < 4; index += 1)
            {
                var z = -17f + (index * 12f);
                CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceCrossStreetWetRead_" + index, new Vector3(0f, 0.255f, z), new Vector3(18.5f, 0.018f, 0.82f), puddle).transform.SetParent(root.transform, true);
            }

            var leftRoles = new[]
            {
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.RowHouse
            };
            var rightRoles = new[]
            {
                DistrictBuildingRole.Warehouse,
                DistrictBuildingRole.RowHouse,
                DistrictBuildingRole.CornerShop,
                DistrictBuildingRole.OfficeBlock,
                DistrictBuildingRole.NarrowTenement,
                DistrictBuildingRole.Warehouse
            };

            for (var index = 0; index < leftRoles.Length; index += 1)
            {
                var z = -21.5f + (index * 8f);
                PlaceDistrictBuilding(
                    DistrictArtStyle.Docks,
                    leftRoles[index],
                    "DocksSliceLeftAssetFacade_" + index,
                    new Vector3(-9.25f - ((index % 2) * 0.45f), 0.08f, z),
                    new Vector3(0f, 90f, 0f),
                    new Vector3(2.35f, 2.55f + ((index % 3) * 0.18f), 2.25f),
                    facade,
                    roof,
                    glow,
                    root.transform);
                PlaceDistrictBuilding(
                    DistrictArtStyle.Docks,
                    rightRoles[index],
                    "DocksSliceRightAssetFacade_" + index,
                    new Vector3(9.25f + (((index + 1) % 2) * 0.45f), 0.08f, z + 2.6f),
                    new Vector3(0f, -90f, 0f),
                    new Vector3(2.3f, 2.45f + (((index + 1) % 3) * 0.18f), 2.2f),
                    facade,
                    roof,
                    glow,
                    root.transform);
            }

            CreateColliderBlock("DocksSliceWestBuildingContinuousCollider", new Vector3(-8.15f, 2.9f, 0.5f), new Vector3(1.15f, 5.8f, 53f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("DocksSliceEastBuildingContinuousCollider", new Vector3(8.15f, 2.9f, 1.5f), new Vector3(1.15f, 5.8f, 53f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("DocksSliceHarborEdgeCollider", new Vector3(-14.15f, 1.3f, 1f), new Vector3(0.7f, 2.6f, 55f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("DocksSliceBackLotCollider", new Vector3(14.15f, 1.3f, 1f), new Vector3(0.7f, 2.6f, 55f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("DocksSliceSouthSoftBoundary", new Vector3(0f, 1.4f, -27.2f), new Vector3(28f, 2.8f, 0.75f)).transform.SetParent(root.transform, true);
            CreateColliderBlock("DocksSliceNorthSoftBoundary", new Vector3(0f, 1.4f, 31.4f), new Vector3(28f, 2.8f, 0.75f)).transform.SetParent(root.transform, true);

            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceWestPavement", new Vector3(-5.65f, 0.24f, 1f), new Vector3(2.6f, 0.08f, 54f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceEastPavement", new Vector3(5.65f, 0.24f, 1f), new Vector3(2.6f, 0.08f, 54f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceWetMainLane", new Vector3(-1.6f, 0.235f, -1.5f), new Vector3(0.45f, 0.018f, 30f), puddle).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceHarborWaterRead", new Vector3(-17.25f, 0.015f, 1f), new Vector3(5.4f, 0.018f, 56f), puddle).transform.SetParent(root.transform, true);

            CreateLowProfileStreetLamp("DocksSliceLampSouthWest", new Vector3(-5.1f, 0.08f, -18.8f), root.transform);
            CreateLowProfileStreetLamp("DocksSliceLampCafe", new Vector3(5.15f, 0.08f, -10.2f), root.transform);
            CreateLowProfileStreetLamp("DocksSliceLampWarehouse", new Vector3(-5.1f, 0.08f, 4.5f), root.transform);
            CreateLowProfileStreetLamp("DocksSliceLampOffice", new Vector3(5.15f, 0.08f, 18.5f), root.transform);

            CreateStaticVehicle("DocksSliceParkedDeliveryTruck", new Vector3(-3.85f, 0.76f, -20.4f), new Vector3(2.05f, 1.08f, 4.55f), new Color(0.09f, 0.105f, 0.11f), roof, glow);
            CreateStaticVehicle("DocksSliceParkedBlackSedan", new Vector3(3.85f, 0.72f, -2.8f), new Vector3(1.65f, 0.9f, 3.75f), new Color(0.055f, 0.052f, 0.048f), roof, glow);
            CreateStaticVehicle("DocksSliceParkedMaroonSedan", new Vector3(3.95f, 0.72f, 18.2f), new Vector3(1.7f, 0.92f, 3.85f), new Color(0.22f, 0.055f, 0.05f), roof, glow);

            for (var index = 0; index < 6; index += 1)
            {
                var z = -18.5f + (index * 7.1f);
                CreatePrimitive(PrimitiveType.Cube, "DocksSliceCrateCollisionStack_" + index, new Vector3(-6.1f, 0.58f, z), new Vector3(0.95f, 0.95f, 0.9f), wood).transform.SetParent(root.transform, true);
                CreatePrimitive(PrimitiveType.Cylinder, "DocksSliceSteelBollard_" + index, new Vector3(-13.35f, 0.58f, z + 1.9f), new Vector3(0.26f, 0.9f, 0.26f), metal).transform.SetParent(root.transform, true);
            }

            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-awning-wide", "DocksSliceMorettiAwning", new Vector3(-6.85f, 1.86f, -14.2f), new Vector3(0f, 90f, 0f), new Vector3(1.75f, 1.75f, 1.75f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitCommercial", "detail-overhang-wide", "DocksSliceWarehouseOverhang", new Vector3(6.85f, 2.05f, 8.6f), new Vector3(0f, -90f, 0f), new Vector3(1.7f, 1.7f, 1.7f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "construction-barrier", "DocksSliceRoadworksBarrierA", new Vector3(4.85f, 0.18f, -20.6f), new Vector3(0f, 15f, 0f), new Vector3(1.35f, 1.35f, 1.35f), root.transform);
            EnvironmentArtCatalog.CreateKenneyFallback("CityKitRoads", "construction-light", "DocksSliceRoadworksLightA", new Vector3(5.4f, 0.18f, -19.35f), new Vector3(0f, 45f, 0f), new Vector3(1.25f, 1.25f, 1.25f), root.transform);

            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceNeonCafeSignBack", new Vector3(-6.98f, 3.05f, -14.15f), new Vector3(0.16f, 0.72f, 2.9f), sign).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceNeonCafeSignGlow", new Vector3(-6.88f, 3.08f, -14.15f), new Vector3(0.07f, 0.18f, 2.35f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceWarehouseLoadingDoor", new Vector3(7.02f, 1.2f, 8.6f), new Vector3(0.12f, 1.85f, 1.85f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceOfficeHeroSign", new Vector3(7.02f, 3.45f, 21.5f), new Vector3(0.14f, 0.62f, 3.6f), sign).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "DocksSliceOfficeHeroSignGlow", new Vector3(6.92f, 3.48f, 21.5f), new Vector3(0.06f, 0.18f, 3.1f), glow).transform.SetParent(root.transform, true);

            var sliceKeyLight = new GameObject("DocksSliceWarmStreetKey");
            sliceKeyLight.transform.SetParent(root.transform, true);
            sliceKeyLight.transform.position = new Vector3(-2.3f, 5.8f, -13.4f);
            var light = sliceKeyLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 13f;
            light.intensity = 2.6f;
            light.color = new Color(1f, 0.68f, 0.32f);
        }

        private static void CreateDocksFirstScreenPolishPass(
            Material asphalt,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material sidewalk,
            Material puddle,
            Material brass)
        {
            var root = new GameObject("Authoring_DocksFirstScreen_NoMoreBlockout");

            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenWetAsphaltUnifier", new Vector3(0f, 0.305f, -8.3f), new Vector3(7.65f, 0.045f, 19.4f), asphalt).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenWetCenterReflection", new Vector3(-1.25f, 0.335f, -8.6f), new Vector3(0.32f, 0.018f, 8.6f), puddle).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenLeftCurbRun", new Vector3(-4.18f, 0.42f, -8.4f), new Vector3(0.22f, 0.24f, 19.8f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenRightCurbRun", new Vector3(4.18f, 0.42f, -8.4f), new Vector3(0.22f, 0.24f, 19.8f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenLeftSidewalkStone", new Vector3(-5.75f, 0.37f, -8.4f), new Vector3(2.95f, 0.12f, 19.6f), sidewalk).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenRightSidewalkStone", new Vector3(5.75f, 0.37f, -8.4f), new Vector3(2.95f, 0.12f, 19.6f), sidewalk).transform.SetParent(root.transform, true);

            PlaceDistrictBuilding(DistrictArtStyle.Docks, DistrictBuildingRole.CornerShop, "FirstScreenReadyMorettiMarketAsset", new Vector3(-8.45f, 0.08f, -13.7f), new Vector3(0f, 90f, 0f), new Vector3(2.15f, 2.35f, 2.05f), facade, roof, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.Docks, DistrictBuildingRole.NarrowTenement, "FirstScreenReadyPierRoomsAsset", new Vector3(-8.35f, 0.08f, -4.6f), new Vector3(0f, 90f, 0f), new Vector3(2.05f, 2.25f, 2.0f), facade, roof, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.Docks, DistrictBuildingRole.Warehouse, "FirstScreenReadyColdStorageAsset", new Vector3(8.55f, 0.08f, -11.1f), new Vector3(0f, -90f, 0f), new Vector3(2.2f, 2.35f, 2.15f), facade, roof, glow, root.transform);
            PlaceDistrictBuilding(DistrictArtStyle.Docks, DistrictBuildingRole.OfficeBlock, "FirstScreenReadyMorettiOfficeAsset", new Vector3(8.45f, 0.08f, -1.1f), new Vector3(0f, -90f, 0f), new Vector3(2.15f, 2.4f, 2.1f), facade, roof, glow, root.transform);

            CreateDocksFacadeSlice(root.transform, "MorettiMarket", -7.18f, -13.6f, 8.2f, 5.8f, facade, roof, metal, glow, wood, sign, brass, true);
            CreateDocksFacadeSlice(root.transform, "PierRooms", -7.08f, -4.5f, 7.6f, 6.4f, facade, roof, metal, glow, wood, sign, brass, true);
            CreateDocksFacadeSlice(root.transform, "ColdStorage", 7.18f, -11.2f, 9.4f, 5.9f, facade, roof, metal, glow, wood, sign, brass, false);
            CreateDocksFacadeSlice(root.transform, "MorettiOffice", 7.1f, -1.2f, 9.0f, 6.8f, facade, roof, metal, glow, wood, sign, brass, false);

            for (var index = 0; index < 7; index += 1)
            {
                var z = -16.5f + (index * 2.8f);
                CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenSidewalkSlabLeft_" + index, new Vector3(-5.75f, 0.455f, z), new Vector3(2.55f, 0.022f, 0.055f), metal).transform.SetParent(root.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenSidewalkSlabRight_" + index, new Vector3(5.75f, 0.455f, z + 0.9f), new Vector3(2.55f, 0.022f, 0.055f), metal).transform.SetParent(root.transform, true);
            }

            for (var stripe = 0; stripe < 4; stripe += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenBrokenLaneStripe_" + stripe, new Vector3(0.95f, 0.36f, -15.4f + (stripe * 4.6f)), new Vector3(0.16f, 0.026f, 1.45f), brass).transform.SetParent(root.transform, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenBackOfficeDirectionGlow", new Vector3(2.92f, 0.36f, -3.1f), new Vector3(1.25f, 0.022f, 0.46f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenStormDrain", new Vector3(-3.72f, 0.47f, -6.1f), new Vector3(0.5f, 0.045f, 0.92f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenStormDrainSlotsA", new Vector3(-3.72f, 0.505f, -6.1f), new Vector3(0.56f, 0.025f, 0.08f), brass).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenStormDrainSlotsB", new Vector3(-3.72f, 0.506f, -5.86f), new Vector3(0.56f, 0.025f, 0.08f), brass).transform.SetParent(root.transform, true);

            CreateStaticVehicle("FirstScreenParkedPeriodSedan", new Vector3(2.8f, 0.78f, -12.1f), new Vector3(1.62f, 0.92f, 3.65f), new Color(0.045f, 0.049f, 0.052f), roof, glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenParkedSedanReflection", new Vector3(2.8f, 0.33f, -12.1f), new Vector3(1.95f, 0.018f, 4.25f), puddle).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenHeadlightSplashLeft", new Vector3(2.42f, 0.34f, -14.9f), new Vector3(0.22f, 0.018f, 2.75f), glow).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenHeadlightSplashRight", new Vector3(3.18f, 0.34f, -14.9f), new Vector3(0.22f, 0.018f, 2.75f), glow).transform.SetParent(root.transform, true);

            for (var crate = 0; crate < 5; crate += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenMarketCrate_" + crate, new Vector3(-5.72f + ((crate % 2) * 0.72f), 0.66f + ((crate / 2) * 0.34f), -14.95f + (crate * 0.45f)), new Vector3(0.62f, 0.5f, 0.54f), wood).transform.SetParent(root.transform, true);
            }

            CreateVisualPrimitive(PrimitiveType.Cylinder, "FirstScreenTrashCanA", new Vector3(-5.95f, 0.76f, -7.7f), new Vector3(0.28f, 0.58f, 0.28f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "FirstScreenTrashCanB", new Vector3(-6.32f, 0.68f, -7.35f), new Vector3(0.22f, 0.48f, 0.22f), metal).transform.SetParent(root.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, "FirstScreenNewspaperBundle", new Vector3(-5.35f, 0.57f, -7.9f), new Vector3(0.68f, 0.18f, 0.42f), sidewalk).transform.SetParent(root.transform, true);

            CreateNoirActor("DockPedestrianFishBuyer", new Vector3(-5.15f, 1f, -15.25f), sign, root.transform, false);
            CreateNoirActor("DockPedestrianLookout", new Vector3(5.45f, 1f, -5.35f), metal, root.transform, false);

            var steamLight = new GameObject("FirstScreenSteamAndWindowBounce");
            steamLight.transform.SetParent(root.transform, true);
            steamLight.transform.position = new Vector3(-4.2f, 2.2f, -12.0f);
            var light = steamLight.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 8.5f;
            light.intensity = 1.75f;
            light.color = new Color(1f, 0.64f, 0.31f);
        }

        private static void CreateDocksFacadeSlice(
            Transform root,
            string prefix,
            float x,
            float centerZ,
            float length,
            float height,
            Material facade,
            Material roof,
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material brass,
            bool facesEast)
        {
            var facingSign = facesEast ? 1f : -1f;
            var facadeX = x;
            var faceOffset = facingSign * 0.12f;
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "FinishedFacadeWall", new Vector3(facadeX, height * 0.5f, centerZ), new Vector3(0.52f, height, length), facade).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "RoofLip", new Vector3(facadeX + faceOffset, height + 0.16f, centerZ), new Vector3(0.72f, 0.28f, length + 0.34f), roof).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "StorefrontBase", new Vector3(facadeX + faceOffset, 1.18f, centerZ - (length * 0.18f)), new Vector3(0.28f, 2.05f, length * 0.5f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "DoorWarmRead", new Vector3(facadeX + (faceOffset * 1.45f), 1.17f, centerZ - (length * 0.36f)), new Vector3(0.08f, 1.65f, 0.72f), wood).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "DoorGlassGlow", new Vector3(facadeX + (faceOffset * 1.8f), 1.33f, centerZ - (length * 0.36f)), new Vector3(0.04f, 0.62f, 0.42f), glow).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "Awning", new Vector3(facadeX + (faceOffset * 2.5f), 2.25f, centerZ - (length * 0.12f)), new Vector3(1.08f, 0.24f, length * 0.56f), sign).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "AwningFrontTrim", new Vector3(facadeX + (faceOffset * 4.4f), 2.08f, centerZ - (length * 0.12f)), new Vector3(0.12f, 0.18f, length * 0.54f), brass).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WallSignBack", new Vector3(facadeX + (faceOffset * 1.7f), 3.25f, centerZ + (length * 0.18f)), new Vector3(0.11f, 0.72f, length * 0.46f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WallSignWarmLine", new Vector3(facadeX + (faceOffset * 2.08f), 3.28f, centerZ + (length * 0.18f)), new Vector3(0.045f, 0.12f, length * 0.38f), glow).transform.SetParent(root, true);

            for (var floor = 0; floor < 3; floor += 1)
            {
                var y = 2.85f + (floor * 1.05f);
                for (var bay = 0; bay < 3; bay += 1)
                {
                    var z = centerZ - (length * 0.32f) + (bay * length * 0.27f);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WindowFrame_" + floor + "_" + bay, new Vector3(facadeX + (faceOffset * 1.42f), y, z), new Vector3(0.075f, 0.68f, 0.68f), metal).transform.SetParent(root, true);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WindowGlow_" + floor + "_" + bay, new Vector3(facadeX + (faceOffset * 1.82f), y, z), new Vector3(0.035f, 0.42f, 0.42f), glow).transform.SetParent(root, true);
                    CreateVisualPrimitive(PrimitiveType.Cube, prefix + "WindowSill_" + floor + "_" + bay, new Vector3(facadeX + (faceOffset * 2.02f), y - 0.42f, z), new Vector3(0.09f, 0.075f, 0.84f), brass).transform.SetParent(root, true);
                }
            }

            for (var course = 0; course < 5; course += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, prefix + "BrickCourse_" + course, new Vector3(facadeX + (faceOffset * 1.1f), 1.6f + (course * 0.82f), centerZ), new Vector3(0.052f, 0.035f, length * 0.92f), roof).transform.SetParent(root, true);
            }
        }

        private static void CreateDocksOverheadWire(Transform root, string name, float z, Material metal)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, name + "Span", new Vector3(0f, 5.15f, z), new Vector3(13.5f, 0.035f, 0.035f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, name + "LeftInsulator", new Vector3(-6.72f, 5.05f, z), new Vector3(0.12f, 0.12f, 0.12f), metal).transform.SetParent(root, true);
            CreateVisualPrimitive(PrimitiveType.Cylinder, name + "RightInsulator", new Vector3(6.72f, 5.05f, z), new Vector3(0.12f, 0.12f, 0.12f), metal).transform.SetParent(root, true);
        }

        [System.Obsolete("Use physical sign geometry instead of camera-facing text in gameplay scenes.")]
        private static void CreateNoirWorldLabel(string name, string text, Vector3 position, float characterSize, Color color)
        {
            var labelObject = new GameObject(name);
            labelObject.transform.position = position;
            labelObject.transform.rotation = Quaternion.Euler(62f, 0f, 0f);
            var textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = 84;
            textMesh.characterSize = characterSize;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = color;
        }

        private static void CreateDocksIdentityPass(
            Material metal,
            Material glow,
            Material wood,
            Material sign,
            Material water,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, "HarborCraneMast", new Vector3(-14.8f, 3.3f, -5.8f), new Vector3(0.55f, 6.6f, 0.55f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "HarborCraneBoom", new Vector3(-12.3f, 6.2f, -5.8f), new Vector3(5.4f, 0.25f, 0.25f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "HarborCraneHook", new Vector3(-10.4f, 3.55f, -5.8f), new Vector3(0.35f, 0.28f, 0.35f), brass);
            CreateVisualPrimitive(PrimitiveType.Cube, "MooredTugHull", new Vector3(-17.8f, 0.45f, 6.2f), new Vector3(2.6f, 0.8f, 7.2f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "MooredTugCabin", new Vector3(-17.8f, 1.28f, 5.1f), new Vector3(1.35f, 1.1f, 1.9f), wood);
            CreateVisualPrimitive(PrimitiveType.Cube, "TugWindowGlow", new Vector3(-16.45f, 1.42f, 5.1f), new Vector3(0.06f, 0.42f, 1.1f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "WetHarborWake", new Vector3(-16.2f, 0.02f, 10.2f), new Vector3(2.8f, 0.01f, 1.2f), water);
            CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseFamilySign", new Vector3(8.05f, 4.35f, 15.2f), new Vector3(0.18f, 0.82f, 4.2f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "WarehouseFamilySignGlow", new Vector3(7.92f, 4.37f, 15.2f), new Vector3(0.08f, 0.5f, 3.5f), glow);

            for (var index = 0; index < 3; index += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cylinder, "DockRopeCoil_" + index, new Vector3(-13.2f, 0.34f, -0.8f + (index * 2.8f)), new Vector3(0.55f, 0.1f, 0.55f), brass);
                CreateVisualPrimitive(PrimitiveType.Cube, "DockCargoStack_" + index, new Vector3(-11.35f, 0.75f, -9.2f + (index * 1.1f)), new Vector3(1.1f, 1.2f, 0.9f), wood);
            }
        }

        private static void CreateBusinessCoreIdentityPass(
            Material metal,
            Material glow,
            Material stone,
            Material sign,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, "BankStepLower", new Vector3(-7.1f, 0.22f, 6.6f), new Vector3(1.4f, 0.22f, 6.2f), stone);
            CreateVisualPrimitive(PrimitiveType.Cube, "BankStepUpper", new Vector3(-7.55f, 0.46f, 6.6f), new Vector3(0.85f, 0.2f, 5.2f), stone);

            for (var index = 0; index < 4; index += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cylinder, "BankColumn_" + index, new Vector3(-7.7f, 2.8f, 2.4f + (index * 2.7f)), new Vector3(0.32f, 2.8f, 0.32f), stone);
                CreateVisualPrimitive(PrimitiveType.Cube, "BankColumnCap_" + index, new Vector3(-7.7f, 5.7f, 2.4f + (index * 2.7f)), new Vector3(0.85f, 0.25f, 0.85f), brass);
            }

            CreateVisualPrimitive(PrimitiveType.Cube, "TheatreMarquee", new Vector3(7.42f, 2.7f, -9.6f), new Vector3(0.45f, 0.75f, 4.8f), sign);
            CreateVisualPrimitive(PrimitiveType.Cube, "TheatreMarqueeGlow", new Vector3(7.18f, 2.7f, -9.6f), new Vector3(0.08f, 0.42f, 4.4f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "StreetcarShelterRoof", new Vector3(2.95f, 2f, 1.2f), new Vector3(2.4f, 0.18f, 1.3f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "StreetcarShelterBack", new Vector3(2.95f, 1.05f, 1.75f), new Vector3(2.2f, 1.5f, 0.08f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "UnionSquareExitMarker", new Vector3(-1.4f, 0.14f, 11.8f), new Vector3(1.6f, 0.035f, 0.55f), brass);
        }

        private static void CreateOldQuarterIdentityPass(
            Material metal,
            Material glow,
            Material churchStone,
            Material tenement,
            Material laundry,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, "ChapelStairLower", new Vector3(-6.1f, 0.22f, 8.4f), new Vector3(1.4f, 0.22f, 2.6f), churchStone);
            CreateVisualPrimitive(PrimitiveType.Cube, "ChapelStairUpper", new Vector3(-6.65f, 0.46f, 8.4f), new Vector3(0.8f, 0.2f, 2.1f), churchStone);
            CreateVisualPrimitive(PrimitiveType.Cube, "ChapelRoseWindow", new Vector3(-6.08f, 4.6f, 14.9f), new Vector3(0.08f, 1.4f, 1.4f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "QuarterArchLeft", new Vector3(-1.9f, 1.9f, -9.4f), new Vector3(0.45f, 3.6f, 0.6f), tenement);
            CreateVisualPrimitive(PrimitiveType.Cube, "QuarterArchRight", new Vector3(1.9f, 1.9f, -9.4f), new Vector3(0.45f, 3.6f, 0.6f), tenement);
            CreateVisualPrimitive(PrimitiveType.Cube, "QuarterArchTop", new Vector3(0f, 3.75f, -9.4f), new Vector3(4.3f, 0.45f, 0.6f), tenement);
            CreateVisualPrimitive(PrimitiveType.Cube, "StreetShrineBase", new Vector3(5.9f, 0.8f, 6.8f), new Vector3(0.8f, 1.2f, 0.55f), churchStone);
            CreateVisualPrimitive(PrimitiveType.Cube, "StreetShrineCandles", new Vector3(5.9f, 1.55f, 6.8f), new Vector3(0.62f, 0.1f, 0.36f), glow);

            for (var index = 0; index < 4; index += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "MarketStallRoof_" + index, new Vector3(5.9f, 1.9f, -6.8f + (index * 1.7f)), new Vector3(1.25f, 0.18f, 1.1f), laundry);
                CreateVisualPrimitive(PrimitiveType.Cube, "MarketStallCounter_" + index, new Vector3(5.9f, 0.82f, -6.8f + (index * 1.7f)), new Vector3(1.05f, 0.55f, 0.85f), brass);
            }
        }

        private static void CreateRailYardIdentityPass(
            Material metal,
            Material glow,
            Material rust,
            Material tank,
            Material wood,
            Material brass)
        {
            CreateVisualPrimitive(PrimitiveType.Cube, "RailGantryLeft", new Vector3(-4.8f, 3.2f, 5.6f), new Vector3(0.45f, 6.4f, 0.45f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailGantryRight", new Vector3(4.8f, 3.2f, 5.6f), new Vector3(0.45f, 6.4f, 0.45f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "RailGantryBeam", new Vector3(0f, 6.3f, 5.6f), new Vector3(10.4f, 0.35f, 0.35f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, "SignalTowerBase", new Vector3(7.8f, 2.7f, 10.4f), new Vector3(1.4f, 5.2f, 1.4f), rust);
            CreateVisualPrimitive(PrimitiveType.Cube, "SignalTowerCabin", new Vector3(7.8f, 5.7f, 10.4f), new Vector3(2.2f, 1.3f, 2f), tank);
            CreateVisualPrimitive(PrimitiveType.Cube, "SignalTowerGlow", new Vector3(7.8f, 5.75f, 9.35f), new Vector3(1.6f, 0.45f, 0.08f), glow);
            CreateVisualPrimitive(PrimitiveType.Cube, "LocomotiveNose", new Vector3(-1.8f, 1.65f, 14.8f), new Vector3(3.6f, 2.6f, 3.2f), tank);
            CreateVisualPrimitive(PrimitiveType.Cylinder, "LocomotiveLamp", new Vector3(-1.8f, 2.1f, 13.05f), new Vector3(0.32f, 0.12f, 0.32f), glow);

            for (var index = 0; index < 5; index += 1)
            {
                CreateVisualPrimitive(PrimitiveType.Cube, "RailPalletStack_" + index, new Vector3(-7.2f, 0.55f, 1f + (index * 1.35f)), new Vector3(1.15f, 0.8f, 0.95f), wood);
                CreateVisualPrimitive(PrimitiveType.Cube, "TrackSwitchLamp_" + index, new Vector3(3.4f, 0.72f, 0.8f + (index * 2.2f)), new Vector3(0.28f, 0.9f, 0.28f), glow);
            }
        }

        private static void CreateSteamVent(string name, Vector3 position, Material metal, Material glow)
        {
            var steam = GetOrCreateMaterial("Assets/Game/Materials/SteamHaze.mat", new Color(0.36f, 0.39f, 0.41f), 0.08f, 0f);
            CreateVisualPrimitive(PrimitiveType.Cylinder, name + "Grate", position, new Vector3(0.45f, 0.04f, 0.45f), metal);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "VaporA", position + new Vector3(0f, 0.48f, 0f), new Vector3(0.12f, 0.58f, 0.12f), steam);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "VaporB", position + new Vector3(0.22f, 0.86f, 0.12f), new Vector3(0.09f, 0.62f, 0.09f), steam);
        }

        private static void CreateLampPost(Vector3 basePosition, Material poleMaterial, Material glowMaterial)
        {
            CreateVisualPrimitive(
                PrimitiveType.Cylinder,
                "LampPost",
                basePosition + new Vector3(0f, 0.16f, 0f),
                new Vector3(0.16f, 0.045f, 0.16f),
                poleMaterial);

            CreateVisualPrimitive(
                PrimitiveType.Cube,
                "LampHead",
                basePosition + new Vector3(0f, 0.52f, 0.18f),
                new Vector3(0.24f, 0.12f, 0.24f),
                glowMaterial);

            var pointLightObject = new GameObject("LampLight");
            pointLightObject.transform.position = basePosition + new Vector3(0f, 0.5f, 0.18f);
            var pointLight = pointLightObject.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 2.8f;
            pointLight.intensity = 0.46f;
            pointLight.color = new Color(1f, 0.64f, 0.34f);
        }

        private static void ApplyExteriorAtmosphere(
            Color ambientLight,
            Color fogColor,
            float fogDensity,
            Color sunColor,
            float sunIntensity,
            Quaternion sunRotation)
        {
            var light = new GameObject("Sun");
            var lightComponent = light.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.color = sunColor;
            lightComponent.intensity = sunIntensity;
            lightComponent.shadows = LightShadows.Soft;
            lightComponent.shadowStrength = 0.22f;
            light.transform.rotation = sunRotation;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambientLight;
            RenderSettings.fog = true;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = fogDensity;
        }

        private static Material GetOrCreateMaterial(
            string assetPath,
            Color color,
            float smoothness = 0.24f,
            float metallic = 0f,
            Color? emissionColor = null)
        {
            var materialColor = ResolveMaterialColor(assetPath, color);
            var materialEmission = ResolveMaterialEmission(assetPath, emissionColor);
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material == null)
            {
                var shader = Shader.Find("Standard");
                if (shader == null)
                {
                    shader = Shader.Find("Universal Render Pipeline/Lit");
                }

                material = new Material(shader);
                AssetDatabase.CreateAsset(material, assetPath);
            }

            var desiredShader = Shader.Find("Standard");
            if (desiredShader == null)
            {
                desiredShader = Shader.Find("Universal Render Pipeline/Lit");
            }

            if (material.shader != desiredShader && desiredShader != null)
            {
                material.shader = desiredShader;
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", materialColor);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", materialColor);
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", smoothness);
            }

            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", smoothness);
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", metallic);
            }

            var surfaceTexture = GetOrCreateSurfaceTexture(assetPath, materialColor);
            if (surfaceTexture != null)
            {
                var textureScale = ResolveSurfaceTextureScale(assetPath);
                if (material.HasProperty("_BaseMap"))
                {
                    material.SetTexture("_BaseMap", surfaceTexture);
                    material.SetTextureScale("_BaseMap", textureScale);
                }

                if (material.HasProperty("_MainTex"))
                {
                    material.SetTexture("_MainTex", surfaceTexture);
                    material.SetTextureScale("_MainTex", textureScale);
                }
            }

            if (materialEmission.HasValue)
            {
                material.EnableKeyword("_EMISSION");
                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", materialEmission.Value);
                }
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }

            EditorUtility.SetDirty(material);

            return material;
        }

        private static Texture2D? GetOrCreateSurfaceTexture(string assetPath, Color baseColor)
        {
            var surfaceKind = ResolveSurfaceKind(assetPath);
            if (surfaceKind == null)
            {
                return null;
            }

            const int size = 256;
            var texturePath = "Assets/Game/Materials/GeneratedSurfaces/" +
                System.IO.Path.GetFileNameWithoutExtension(assetPath) +
                "_Surface.asset";

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture != null && texture.width == size && texture.height == size)
            {
                return texture;
            }

            var directory = System.IO.Path.GetDirectoryName(texturePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                EnsureFolder(directory.Replace('\\', '/'));
            }

            texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = System.IO.Path.GetFileNameWithoutExtension(texturePath),
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear
            };

            var pixels = new Color[size * size];
            for (var y = 0; y < size; y += 1)
            {
                for (var x = 0; x < size; x += 1)
                {
                    pixels[(y * size) + x] = ResolveSurfacePixel(surfaceKind, baseColor, x, y, size);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply(false, false);
            AssetDatabase.CreateAsset(texture, texturePath);
            return texture;
        }

        private static string? ResolveSurfaceKind(string assetPath)
        {
            if (assetPath.Contains("Asphalt", System.StringComparison.OrdinalIgnoreCase))
            {
                return "asphalt";
            }

            if (assetPath.Contains("Brick", System.StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains("Tenement", System.StringComparison.OrdinalIgnoreCase))
            {
                return "brick";
            }

            if (assetPath.Contains("Sidewalk", System.StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains("Stone", System.StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains("ChurchStone", System.StringComparison.OrdinalIgnoreCase))
            {
                return "stone";
            }

            if (assetPath.Contains("Roof", System.StringComparison.OrdinalIgnoreCase))
            {
                return "roof";
            }

            if (assetPath.Contains("CrateWood", System.StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains("WarmDoor", System.StringComparison.OrdinalIgnoreCase))
            {
                return "wood";
            }

            if (assetPath.Contains("Metal", System.StringComparison.OrdinalIgnoreCase))
            {
                return "metal";
            }

            return null;
        }

        private static Vector2 ResolveSurfaceTextureScale(string assetPath)
        {
            if (assetPath.Contains("Asphalt", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Vector2(4f, 7f);
            }

            if (assetPath.Contains("Brick", System.StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains("Tenement", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Vector2(3f, 6f);
            }

            if (assetPath.Contains("Sidewalk", System.StringComparison.OrdinalIgnoreCase) ||
                assetPath.Contains("Stone", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Vector2(2.5f, 4.5f);
            }

            return new Vector2(2f, 2f);
        }

        private static Color ResolveSurfacePixel(string surfaceKind, Color baseColor, int x, int y, int size)
        {
            var u = x / (float)size;
            var v = y / (float)size;
            var noise = (Mathf.PerlinNoise((u * 16f) + 13.7f, (v * 16f) + 2.9f) - 0.5f) * 0.12f;
            var fineNoise = (StableNoise(x, y) - 0.5f) * 0.055f;
            var color = ShiftColor(baseColor, noise + fineNoise);

            if (surfaceKind == "asphalt")
            {
                var crack = Mathf.Abs(Mathf.PerlinNoise(u * 5.5f, (v * 13f) + 9f) - 0.5f) < 0.018f;
                if (crack)
                {
                    return ShiftColor(baseColor, -0.11f);
                }

                return color;
            }

            if (surfaceKind == "brick")
            {
                var brickWidth = 42;
                var brickHeight = 18;
                var row = y / brickHeight;
                var staggeredX = x + ((row % 2) * (brickWidth / 2));
                var mortar = (staggeredX % brickWidth) < 3 || (y % brickHeight) < 3;
                if (mortar)
                {
                    return ShiftColor(baseColor, -0.16f);
                }

                return ShiftColor(color, ((row % 3) - 1) * 0.025f);
            }

            if (surfaceKind == "stone")
            {
                var slab = (x % 64) < 3 || (y % 64) < 3;
                return slab ? ShiftColor(baseColor, -0.13f) : color;
            }

            if (surfaceKind == "roof")
            {
                var seam = (y % 34) < 2;
                return seam ? ShiftColor(baseColor, -0.08f) : ShiftColor(color, -0.02f);
            }

            if (surfaceKind == "wood")
            {
                var grain = Mathf.Sin((u * 58f) + (Mathf.PerlinNoise(v * 3f, u * 5f) * 5f)) * 0.045f;
                return ShiftColor(baseColor, grain + fineNoise);
            }

            if (surfaceKind == "metal")
            {
                var streak = (x % 48) < 2 ? -0.07f : 0f;
                return ShiftColor(baseColor, streak + (fineNoise * 0.8f));
            }

            return color;
        }

        private static float StableNoise(int x, int y)
        {
            unchecked
            {
                var hash = (uint)((x * 73856093) ^ (y * 19349663) ^ 0x9E3779B9);
                hash ^= hash >> 16;
                hash *= 0x7FEB352D;
                hash ^= hash >> 15;
                hash *= 0x846CA68B;
                hash ^= hash >> 16;
                return (hash & 0xFFFFFF) / 16777215f;
            }
        }

        private static Color ShiftColor(Color color, float delta)
        {
            return new Color(
                Mathf.Clamp01(color.r + delta),
                Mathf.Clamp01(color.g + delta),
                Mathf.Clamp01(color.b + delta),
                color.a);
        }

        private static Color ResolveMaterialColor(string assetPath, Color color)
        {
            if (assetPath.Contains("WindowGlow", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Color(0.62f, 0.42f, 0.24f, color.a);
            }

            if (assetPath.Contains("LanePaint", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Color(0.68f, 0.54f, 0.27f, color.a);
            }

            return color;
        }

        private static Color? ResolveMaterialEmission(string assetPath, Color? emissionColor)
        {
            if (!emissionColor.HasValue)
            {
                return null;
            }

            if (assetPath.Contains("WindowGlow", System.StringComparison.OrdinalIgnoreCase))
            {
                return new Color(1f, 0.47f, 0.18f) * 0.58f;
            }

            return emissionColor.Value;
        }

        private static void CreateSpawnPoint(string name, string spawnPointId, Vector3 position)
        {
            var spawnPointObject = new GameObject(name);
            spawnPointObject.transform.position = position;
            var spawnPoint = spawnPointObject.AddComponent<SceneSpawnPoint>();
            SetStringValue(spawnPoint, "spawnPointId", spawnPointId);
        }

        private static CombatHealth CreateEnemyGuard(
            string name,
            Vector3 position,
            Material material,
            TopDownPlayerController targetPlayer,
            CombatHealth targetHealth,
            DialogueController? dialogueController = null,
            Transform? parent = null)
        {
            var guard = CreateNoirActor(name, position, material, parent);
            var health = guard.AddComponent<CombatHealth>();
            SetStringValue(health, "displayName", name);
            SetIntValue(health, "maximumHealth", 2);
            SetBoolValue(health, "destroyOnDeath", true);
            SetBoolValue(health, "reloadSceneOnDeath", false);
            var threat = guard.AddComponent<EnemyThreatController>();
            SetObjectReference(threat, "combatHealth", health);
            SetObjectReference(threat, "targetPlayer", targetPlayer);
            SetObjectReference(threat, "targetHealth", targetHealth);
            if (dialogueController != null)
            {
                SetObjectReference(threat, "dialogueController", dialogueController);
            }

            SetFloatValue(threat, "aggroRange", 4.5f);
            SetFloatValue(threat, "disengageRange", 9f);
            SetFloatValue(threat, "attackRange", 1.25f);
            return health;
        }

        private static GameObject CreatePoliceResponder(
            string name,
            Vector3 position,
            Material material,
            TopDownPlayerController targetPlayer,
            CombatHealth targetHealth)
        {
            var officer = CreateNoirActor(name, position, material);

            var health = officer.AddComponent<CombatHealth>();
            SetStringValue(health, "displayName", name);
            SetIntValue(health, "maximumHealth", 2);
            SetBoolValue(health, "destroyOnDeath", true);
            SetBoolValue(health, "reloadSceneOnDeath", false);

            var responder = officer.AddComponent<PoliceResponderController>();
            SetObjectReference(responder, "combatHealth", health);
            SetObjectReference(responder, "targetPlayer", targetPlayer);
            SetObjectReference(responder, "targetHealth", targetHealth);

            officer.SetActive(false);
            return officer;
        }

        private static void ConfigurePoliceResponse(
            PoliceResponseController controller,
            GameObject watchResponder,
            GameObject patrolResponderA,
            GameObject patrolResponderB,
            GameObject huntResponderA,
            GameObject huntResponderB,
            GameObject huntResponderC)
        {
            var serializedObject = new SerializedObject(controller);
            SetObjectArray(serializedObject.FindProperty("watchResponders"), new UnityEngine.Object[] { watchResponder });
            SetObjectArray(serializedObject.FindProperty("patrolResponders"), new UnityEngine.Object[] { patrolResponderA, patrolResponderB });
            SetObjectArray(serializedObject.FindProperty("huntResponders"), new UnityEngine.Object[] { huntResponderA, huntResponderB, huntResponderC });
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateStaticVehicle(
            string name,
            Vector3 position,
            Vector3 bodyScale,
            Color bodyColor,
            Material roofMaterial,
            Material glassMaterial)
        {
            var bodyMaterial = GetOrCreateMaterial(
                "Assets/Game/Materials/" + name + "Body.mat",
                bodyColor,
                0.76f,
                0.08f);
            var body = CreatePrimitive(PrimitiveType.Cube, name, position, bodyScale, bodyMaterial);
            HideRenderer(body);
            BuildNoirVehicleVisual(body, name, position, bodyScale, bodyMaterial, roofMaterial, roofMaterial, glassMaterial);
        }

        private static void BuildNoirVehicleVisual(
            GameObject body,
            string name,
            Vector3 position,
            Vector3 bodyScale,
            Material bodyMaterial,
            Material roofMaterial,
            Material trimMaterial,
            Material glowMaterial)
        {
            var halfWidth = bodyScale.x * 0.5f;
            var halfLength = bodyScale.z * 0.5f;
            var readyVehicleScale = ResolveReadyVehicleScale(name, bodyScale);
            var readyModel = EnvironmentArtCatalog.CreateKenneyFallback(
                "CarKit",
                ResolveReadyVehicleModel(name),
                name + "ReadyVehicleMesh",
                position + new Vector3(0f, -0.18f, 0f),
                Vector3.zero,
                readyVehicleScale,
                body.transform);
            if (readyModel != null)
            {
                ApplyNoirVehicleMaterials(readyModel, bodyMaterial, trimMaterial, glowMaterial);
                CreateVisualPrimitive(PrimitiveType.Cube, name + "ReadyHeadlampLeft", position + new Vector3(-halfWidth * 0.32f, 0.5f, -halfLength - 0.08f), new Vector3(0.26f, 0.14f, 0.08f), glowMaterial).transform.SetParent(body.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, name + "ReadyHeadlampRight", position + new Vector3(halfWidth * 0.32f, 0.5f, -halfLength - 0.08f), new Vector3(0.26f, 0.14f, 0.08f), glowMaterial).transform.SetParent(body.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, name + "ReadyGroundShadow", position + new Vector3(0f, -0.63f, 0f), new Vector3(bodyScale.x * 1.06f, 0.018f, bodyScale.z * 0.94f), trimMaterial).transform.SetParent(body.transform, true);
                return;
            }

            CreateVisualPrimitive(PrimitiveType.Cube, name + "LowerChassis", position + new Vector3(0f, 0.42f, 0f), new Vector3(bodyScale.x * 0.96f, 0.44f, bodyScale.z * 0.78f), bodyMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "LongHood", position + new Vector3(0f, 0.66f, -halfLength * 0.48f), new Vector3(bodyScale.x * 0.74f, 0.34f, bodyScale.z * 0.34f), bodyMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "RoundedTrunk", position + new Vector3(0f, 0.62f, halfLength * 0.42f), new Vector3(bodyScale.x * 0.78f, 0.3f, bodyScale.z * 0.26f), bodyMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "Cabin", position + new Vector3(0f, 1.0f, -0.04f), new Vector3(bodyScale.x * 0.58f, 0.62f, bodyScale.z * 0.34f), roofMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "Windshield", position + new Vector3(0f, 1.03f, -bodyScale.z * 0.23f), new Vector3(bodyScale.x * 0.5f, 0.26f, 0.08f), glowMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "RearWindow", position + new Vector3(0f, 1.03f, bodyScale.z * 0.16f), new Vector3(bodyScale.x * 0.46f, 0.24f, 0.08f), glowMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "LeftSideGlass", position + new Vector3(-halfWidth * 0.32f, 1.04f, -0.02f), new Vector3(0.06f, 0.22f, bodyScale.z * 0.28f), glowMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "RightSideGlass", position + new Vector3(halfWidth * 0.32f, 1.04f, -0.02f), new Vector3(0.06f, 0.22f, bodyScale.z * 0.28f), glowMaterial).transform.SetParent(body.transform, true);
            DecorateNoirVehicle(body, name, position, bodyScale, trimMaterial, glowMaterial);
        }

        private static string ResolveReadyVehicleModel(string name)
        {
            if (name.Contains("Truck", System.StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Delivery", System.StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Harbor", System.StringComparison.OrdinalIgnoreCase))
            {
                return "delivery";
            }

            if (name.Contains("Police", System.StringComparison.OrdinalIgnoreCase))
            {
                return "police";
            }

            if (name.Contains("Coupe", System.StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Sports", System.StringComparison.OrdinalIgnoreCase))
            {
                return "sedan-sports";
            }

            return "sedan";
        }

        private static Vector3 ResolveReadyVehicleScale(string name, Vector3 bodyScale)
        {
            // Vehicle meshes are parented under an invisible physics body whose local scale
            // defines the gameplay footprint, so the art mesh scale must stay relative.
            if (name.Contains("Truck", System.StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Delivery", System.StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Harbor", System.StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.one * 0.18f;
            }

            if (name.Contains("Coupe", System.StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Sports", System.StringComparison.OrdinalIgnoreCase))
            {
                return Vector3.one * 0.155f;
            }

            return Vector3.one * 0.165f;
        }

        private static void ApplyNoirVehicleMaterials(GameObject vehicleModel, Material bodyMaterial, Material trimMaterial, Material glowMaterial)
        {
            var renderers = vehicleModel.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                var sourceMaterials = renderer.sharedMaterials;
                if (sourceMaterials.Length == 0)
                {
                    renderer.sharedMaterial = bodyMaterial;
                    continue;
                }

                var replacements = new Material[sourceMaterials.Length];
                for (var index = 0; index < sourceMaterials.Length; index += 1)
                {
                    var sourceName = sourceMaterials[index] != null ? sourceMaterials[index].name : string.Empty;
                    if (sourceName.Contains("window", System.StringComparison.OrdinalIgnoreCase) ||
                        sourceName.Contains("glass", System.StringComparison.OrdinalIgnoreCase) ||
                        sourceName.Contains("light", System.StringComparison.OrdinalIgnoreCase))
                    {
                        replacements[index] = glowMaterial;
                    }
                    else if (sourceName.Contains("wheel", System.StringComparison.OrdinalIgnoreCase) ||
                        sourceName.Contains("tire", System.StringComparison.OrdinalIgnoreCase) ||
                        sourceName.Contains("bumper", System.StringComparison.OrdinalIgnoreCase))
                    {
                        replacements[index] = trimMaterial;
                    }
                    else
                    {
                        replacements[index] = bodyMaterial;
                    }
                }

                renderer.sharedMaterials = replacements;
            }
        }

        private static void DecorateNoirVehicle(
            GameObject body,
            string name,
            Vector3 position,
            Vector3 bodyScale,
            Material trimMaterial,
            Material glowMaterial)
        {
            var halfWidth = bodyScale.x * 0.5f;
            var halfLength = bodyScale.z * 0.5f;

            CreateVisualPrimitive(PrimitiveType.Cube, name + "LongHoodHighlight", position + new Vector3(0f, 0.56f, -halfLength * 0.58f), new Vector3(bodyScale.x * 0.56f, 0.035f, bodyScale.z * 0.34f), trimMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "TrunkHighlight", position + new Vector3(0f, 0.56f, halfLength * 0.58f), new Vector3(bodyScale.x * 0.58f, 0.035f, bodyScale.z * 0.26f), trimMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "LeftRunningBoard", position + new Vector3(-halfWidth - 0.08f, 0.43f, 0f), new Vector3(0.12f, 0.12f, bodyScale.z * 0.82f), trimMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "RightRunningBoard", position + new Vector3(halfWidth + 0.08f, 0.43f, 0f), new Vector3(0.12f, 0.12f, bodyScale.z * 0.82f), trimMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "FrontChrome", position + new Vector3(0f, 0.52f, -halfLength - 0.06f), new Vector3(bodyScale.x * 0.82f, 0.12f, 0.08f), trimMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "RearChrome", position + new Vector3(0f, 0.52f, halfLength + 0.06f), new Vector3(bodyScale.x * 0.78f, 0.12f, 0.08f), trimMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "LeftHeadlamp", position + new Vector3(-halfWidth * 0.42f, 0.63f, -halfLength - 0.12f), new Vector3(0.28f, 0.2f, 0.08f), glowMaterial).transform.SetParent(body.transform, true);
            CreateVisualPrimitive(PrimitiveType.Cube, name + "RightHeadlamp", position + new Vector3(halfWidth * 0.42f, 0.63f, -halfLength - 0.12f), new Vector3(0.28f, 0.2f, 0.08f), glowMaterial).transform.SetParent(body.transform, true);

            for (var side = -1; side <= 1; side += 2)
            {
                CreateVisualPrimitive(PrimitiveType.Cylinder, name + "FrontWheel_" + side, position + new Vector3(side * (halfWidth + 0.16f), 0.38f, -halfLength * 0.48f), new Vector3(0.25f, 0.12f, 0.25f), trimMaterial).transform.SetParent(body.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cylinder, name + "RearWheel_" + side, position + new Vector3(side * (halfWidth + 0.16f), 0.38f, halfLength * 0.48f), new Vector3(0.25f, 0.12f, 0.25f), trimMaterial).transform.SetParent(body.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, name + "FenderFront_" + side, position + new Vector3(side * (halfWidth + 0.1f), 0.7f, -halfLength * 0.48f), new Vector3(0.18f, 0.18f, 0.72f), trimMaterial).transform.SetParent(body.transform, true);
                CreateVisualPrimitive(PrimitiveType.Cube, name + "FenderRear_" + side, position + new Vector3(side * (halfWidth + 0.1f), 0.7f, halfLength * 0.48f), new Vector3(0.18f, 0.18f, 0.72f), trimMaterial).transform.SetParent(body.transform, true);
            }
        }

        private static GameObject CreateObjectiveActionMissionRoot(
            string rootName,
            string markerName,
            Vector3 markerPosition,
            Vector3 markerScale,
            Material markerMaterial,
            string promptText,
            SimpleObjectiveSystem objectiveSystem,
            string requiredObjectiveId,
            CampaignProgressionController campaignProgressionController,
            MissionDefinitionAsset missionAsset,
            string missionStageId)
        {
            var root = new GameObject(rootName);
            var marker = CreatePrimitive(PrimitiveType.Cube, markerName, markerPosition, markerScale, markerMaterial);
            marker.transform.SetParent(root.transform, true);
            var interactable = marker.AddComponent<ObjectiveActionInteractable>();
            SetStringValue(interactable, "promptText", promptText);
            SetStringValue(interactable, "requiredObjectiveId", requiredObjectiveId);
            SetObjectReference(interactable, "objectiveSystem", objectiveSystem);
            SetObjectReference(interactable, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(interactable, "missionAsset", missionAsset);
            SetStringValue(interactable, "missionStageIdToComplete", missionStageId);
            return root;
        }

        private static GameObject CreateMissionExitDoor(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            string promptText,
            string sourceScene,
            string targetScene,
            string spawnPointId,
            string requiredObjectiveId,
            SceneTransitionController sceneTransitionController,
            SimpleObjectiveSystem objectiveSystem,
            CampaignProgressionController campaignProgressionController,
            MissionDefinitionAsset missionAsset,
            string missionStageId)
        {
            var door = CreatePrimitive(PrimitiveType.Cube, name, position, scale, material);
            door.transform.SetParent(parent, true);
            var interactable = door.AddComponent<SceneDoorInteractable>();
            SetObjectReference(interactable, "sceneTransitionController", sceneTransitionController);
            SetStringValue(interactable, "promptText", promptText);
            SetStringValue(interactable, "sourceScene", sourceScene);
            SetStringValue(interactable, "targetScene", targetScene);
            SetStringValue(interactable, "spawnPointId", spawnPointId);
            SetObjectReference(interactable, "objectiveSystem", objectiveSystem);
            SetStringValue(interactable, "requiredObjectiveId", requiredObjectiveId);
            SetObjectReference(interactable, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(interactable, "missionAsset", missionAsset);
            SetStringValue(interactable, "missionStageIdToComplete", missionStageId);
            return door;
        }

        private static GameObject CreateMissionTravelGate(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            string promptText,
            string targetScene,
            string spawnPointId,
            string requiredObjectiveId,
            SceneTransitionController sceneTransitionController,
            SaveGameFileService saveGameFileService,
            SimpleObjectiveSystem objectiveSystem,
            CampaignProgressionController campaignProgressionController,
            MissionDefinitionAsset missionAsset,
            string missionStageId)
        {
            var gate = CreatePrimitive(PrimitiveType.Cube, name, position, scale, material);
            gate.transform.SetParent(parent, true);
            HideRenderer(gate);
            if (gate.TryGetComponent<Collider>(out var gateCollider))
            {
                gateCollider.isTrigger = true;
            }

            CreateTravelGateStreetMarkers(name, position, scale, material, parent);

            var travel = gate.AddComponent<DistrictTravelInteractable>();
            SetStringValue(travel, "promptText", promptText);
            SetStringValue(travel, "requiredDistrictId", string.Empty);
            SetStringValue(travel, "targetScene", targetScene);
            SetStringValue(travel, "spawnPointId", spawnPointId);
            SetObjectReference(travel, "sceneTransitionController", sceneTransitionController);
            SetObjectReference(travel, "saveGameFileService", saveGameFileService);
            SetObjectReference(travel, "objectiveSystem", objectiveSystem);
            SetStringValue(travel, "requiredObjectiveId", requiredObjectiveId);
            SetObjectReference(travel, "campaignProgressionController", campaignProgressionController);
            SetObjectReference(travel, "missionAsset", missionAsset);
            SetStringValue(travel, "missionStageIdToComplete", missionStageId);
            return gate;
        }

        private static GameObject CreateFreeRoamSceneDoor(
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            string promptText,
            string sourceScene,
            string targetScene,
            string spawnPointId,
            SceneTransitionController sceneTransitionController)
        {
            var door = CreatePrimitive(PrimitiveType.Cube, name, position, scale, material);
            var interactable = door.AddComponent<SceneDoorInteractable>();
            SetObjectReference(interactable, "sceneTransitionController", sceneTransitionController);
            SetStringValue(interactable, "promptText", promptText);
            SetStringValue(interactable, "sourceScene", sourceScene);
            SetStringValue(interactable, "targetScene", targetScene);
            SetStringValue(interactable, "spawnPointId", spawnPointId);
            return door;
        }

        private static GameObject CreateChapelAshMissionRoot(
            ExteriorRuntimeBundle runtime,
            SimpleObjectiveSystem objectiveSystem,
            MissionDefinitionAsset missionAsset,
            Material markerMaterial,
            Material gateMaterial)
        {
            var root = CreateObjectiveActionMissionRoot(
                "ChapelAshMissionRoot",
                "CourtyardAshMarker",
                new Vector3(-7.8f, 1.1f, 12.8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                markerMaterial,
                "Investigate the chapel ash",
                objectiveSystem,
                "investigate-courtyard",
                runtime.CampaignProgressionController,
                missionAsset,
                "investigate-courtyard");

            CreateMissionTravelGate(
                root.transform,
                "ChapelAshExitGate",
                new Vector3(-13.0f, 1.1f, 1.6f),
                new Vector3(0.4f, 2.2f, 3.2f),
                gateMaterial,
                "Leave the quarter",
                "District_RailYard_01",
                "RailYardStart",
                "leave-quarter",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService,
                objectiveSystem,
                runtime.CampaignProgressionController,
                missionAsset,
                "leave-quarter");

            return root;
        }

        private static GameObject CreateYardBetrayalMissionRoot(
            ExteriorRuntimeBundle runtime,
            SimpleObjectiveSystem objectiveSystem,
            MissionDefinitionAsset missionAsset,
            Material markerMaterial,
            Material gateMaterial)
        {
            var root = CreateObjectiveActionMissionRoot(
                "YardBetrayalMissionRoot",
                "BetrayalMarker",
                new Vector3(3.8f, 1.1f, 1.8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                markerMaterial,
                "Inspect the handoff point",
                objectiveSystem,
                "inspect-yard",
                runtime.CampaignProgressionController,
                missionAsset,
                "inspect-yard");

            CreateMissionTravelGate(
                root.transform,
                "YardBetrayalExitGate",
                new Vector3(-8.2f, 1.1f, -6.8f),
                new Vector3(1.6f, 2.2f, 0.4f),
                gateMaterial,
                "Get out of the yard",
                "Interior_BackOffice_01",
                "InteriorSpawn",
                "leave-yard",
                runtime.SceneTransitionController,
                runtime.SaveGameFileService,
                objectiveSystem,
                runtime.CampaignProgressionController,
                missionAsset,
                "leave-yard");

            return root;
        }

        private static void CreateTravelGate(
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            string promptText,
            string requiredDistrictId,
            string targetScene,
            string spawnPointId,
            SceneTransitionController sceneTransitionController,
            SaveGameFileService saveGameFileService,
            SimpleObjectiveSystem? objectiveSystem = null,
            string requiredObjectiveId = "",
            CampaignProgressionController? campaignProgressionController = null,
            MissionDefinitionAsset? missionAsset = null,
            string missionStageId = "")
        {
            var gate = CreatePrimitive(PrimitiveType.Cube, name, position, scale, material);
            HideRenderer(gate);
            if (gate.TryGetComponent<Collider>(out var gateCollider))
            {
                gateCollider.isTrigger = true;
            }

            CreateTravelGateStreetMarkers(name, position, scale, material);

            var travel = gate.AddComponent<DistrictTravelInteractable>();
            SetStringValue(travel, "promptText", promptText);
            SetStringValue(travel, "requiredDistrictId", requiredDistrictId);
            SetStringValue(travel, "targetScene", targetScene);
            SetStringValue(travel, "spawnPointId", spawnPointId);
            SetObjectReference(travel, "sceneTransitionController", sceneTransitionController);
            SetObjectReference(travel, "saveGameFileService", saveGameFileService);

            if (objectiveSystem != null)
            {
                SetObjectReference(travel, "objectiveSystem", objectiveSystem);
                SetStringValue(travel, "requiredObjectiveId", requiredObjectiveId);
            }

            if (campaignProgressionController != null && missionAsset != null)
            {
                SetObjectReference(travel, "campaignProgressionController", campaignProgressionController);
                SetObjectReference(travel, "missionAsset", missionAsset);
                SetStringValue(travel, "missionStageIdToComplete", missionStageId);
            }
        }

        private static void CreateTravelGateStreetMarkers(
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            Transform? parent = null)
        {
            var markerHalfOffset = scale.x >= scale.z
                ? new Vector3(Mathf.Max(1.2f, scale.x * 0.45f), 0f, 0f)
                : new Vector3(0f, 0f, Mathf.Max(1.2f, scale.z * 0.45f));

            for (var side = -1; side <= 1; side += 2)
            {
                var marker = CreateVisualPrimitive(
                    PrimitiveType.Cube,
                    name + "_CurbMarker_" + (side < 0 ? "A" : "B"),
                    position + (markerHalfOffset * side) + new Vector3(0f, -0.76f, 0f),
                    new Vector3(0.26f, 0.18f, 0.26f),
                    material);
                if (parent != null)
                {
                    marker.transform.SetParent(parent, true);
                }
            }
        }
    }
}
