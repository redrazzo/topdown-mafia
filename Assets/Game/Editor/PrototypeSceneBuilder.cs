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
            PrototypeDataBuilder.CreatePrototypeDataAssets();
            BuildBootScene();
            BuildDistrictScene();
            BuildBusinessCoreScene();
            BuildOldQuarterScene();
            BuildRailYardScene();
            BuildBusinessInteriorScene();
            BuildOldQuarterInteriorScene();
            BuildRailYardInteriorScene();
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

            var bootRoot = new GameObject("BootFlow");
            var saveGameFileService = bootRoot.AddComponent<SaveGameFileService>();
            SetStringValue(saveGameFileService, "defaultSceneName", "District_01");
            SetStringValue(saveGameFileService, "defaultSpawnPointId", "PickupSpawn");
            var bootFlowController = bootRoot.AddComponent<BootFlowController>();
            SetObjectReference(bootFlowController, "saveGameFileService", saveGameFileService);

            EditorSceneManager.SaveScene(scene);
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

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.18f, 0.18f, 0.2f), 0.72f, 0f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/Sidewalk.mat", new Color(0.36f, 0.34f, 0.32f), 0.24f, 0f);
            var lanePaint = GetOrCreateMaterial("Assets/Game/Materials/LanePaint.mat", new Color(0.9f, 0.76f, 0.34f), 0.65f, 0f);
            var brick = GetOrCreateMaterial("Assets/Game/Materials/Brick.mat", new Color(0.34f, 0.18f, 0.16f), 0.12f, 0f);
            var warmDoor = GetOrCreateMaterial("Assets/Game/Materials/WarmDoor.mat", new Color(0.62f, 0.49f, 0.28f), 0.28f, 0f);
            var sedanPaint = GetOrCreateMaterial("Assets/Game/Materials/SedanPaint.mat", new Color(0.37f, 0.07f, 0.08f), 0.8f, 0.1f);
            var awning = GetOrCreateMaterial("Assets/Game/Materials/Awning.mat", new Color(0.1f, 0.12f, 0.14f), 0.18f, 0f);
            var playerCoat = GetOrCreateMaterial("Assets/Game/Materials/PlayerCoat.mat", new Color(0.68f, 0.67f, 0.61f), 0.15f, 0f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.45f, 0.32f, 0.21f), 0.22f, 0f);
            var roof = GetOrCreateMaterial("Assets/Game/Materials/Roof.mat", new Color(0.16f, 0.12f, 0.11f), 0.14f, 0f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(0.92f, 0.74f, 0.42f), 0.78f, 0.1f, new Color(0.92f, 0.74f, 0.42f) * 0.9f);
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
            lightComponent.intensity = 0.82f;
            lightComponent.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(36f, -38f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.17f, 0.18f, 0.22f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.17f, 0.19f, 0.23f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.018f;

            var streetPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            streetPlane.name = "StreetPlane";
            streetPlane.transform.localScale = new Vector3(4f, 1f, 4f);
            AssignMaterial(streetPlane, asphalt);

            CreatePrimitive(
                PrimitiveType.Cube,
                "DockWater",
                new Vector3(-16.2f, -0.18f, 3f),
                new Vector3(6.2f, 0.08f, 38f),
                water);

            CreatePrimitive(
                PrimitiveType.Cube,
                "DockPier",
                new Vector3(-12.6f, 0.12f, 3f),
                new Vector3(1.7f, 0.24f, 37f),
                stone);

            CreatePrimitive(
                PrimitiveType.Cube,
                "DockEdgeRail",
                new Vector3(-14.1f, 0.8f, 3f),
                new Vector3(0.15f, 1.1f, 37f),
                metal);

            var road = CreatePrimitive(
                PrimitiveType.Cube,
                "MainRoad",
                new Vector3(0f, 0.05f, 2f),
                new Vector3(9f, 0.1f, 36f),
                asphalt);

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeftSidewalk",
                new Vector3(-6.2f, 0.1f, 2f),
                new Vector3(3f, 0.2f, 36f),
                sidewalk);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RightSidewalk",
                new Vector3(6.2f, 0.1f, 2f),
                new Vector3(3f, 0.2f, 36f),
                sidewalk);

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeftBuildingRow",
                new Vector3(-10.6f, 3.1f, 2f),
                new Vector3(5.5f, 6.2f, 34f),
                brick);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RightBuildingRow",
                new Vector3(10.6f, 3.1f, 2f),
                new Vector3(5.5f, 6.2f, 34f),
                brick);

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeftRoofLine",
                new Vector3(-10.6f, 6.6f, 2f),
                new Vector3(6.1f, 0.5f, 34.5f),
                roof);

            CreatePrimitive(
                PrimitiveType.Cube,
                "RightRoofLine",
                new Vector3(10.6f, 6.6f, 2f),
                new Vector3(6.1f, 0.5f, 34.5f),
                roof);

            CreatePrimitive(
                PrimitiveType.Cube,
                "CanopyFacade",
                new Vector3(10.55f, 1.75f, 17.2f),
                new Vector3(6.2f, 3.4f, 7.6f),
                stone);

            CreatePrimitive(
                PrimitiveType.Cube,
                "WarehouseInset",
                new Vector3(-10.55f, 2.2f, -5.4f),
                new Vector3(4.8f, 4.4f, 8.2f),
                stone);

            for (var index = 0; index < 7; index += 1)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "LaneMarker_" + index,
                    new Vector3(0f, 0.11f, -9f + (index * 5f)),
                    new Vector3(0.35f, 0.02f, 2f),
                    lanePaint);
            }

            for (var index = 0; index < 4; index += 1)
            {
                var z = -5f + (index * 8f);
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "LeftWindow_" + index,
                    new Vector3(-8.1f, 3f, z),
                    new Vector3(1.4f, 1.6f, 0.15f),
                    windowGlow);

                CreatePrimitive(
                    PrimitiveType.Cube,
                    "RightWindow_" + index,
                    new Vector3(8.1f, 3f, z + 2f),
                    new Vector3(1.4f, 1.6f, 0.15f),
                    windowGlow);
            }

            for (var index = 0; index < 6; index += 1)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "DockBollard_" + index,
                    new Vector3(-13.55f, 0.55f, -10f + (index * 5.6f)),
                    new Vector3(0.28f, 0.9f, 0.28f),
                    metal);
            }

            CreatePrimitive(
                PrimitiveType.Cube,
                "CrosswalkA",
                new Vector3(0f, 0.11f, 12.4f),
                new Vector3(8.6f, 0.02f, 0.55f),
                lanePaint);

            CreatePrimitive(
                PrimitiveType.Cube,
                "CrosswalkB",
                new Vector3(0f, 0.11f, 13.7f),
                new Vector3(8.6f, 0.02f, 0.55f),
                lanePaint);

            CreatePrimitive(
                PrimitiveType.Cube,
                "PuddleNearLamp",
                new Vector3(3.6f, 0.065f, 5.6f),
                new Vector3(1.6f, 0.01f, 1.2f),
                puddle);

            CreatePrimitive(
                PrimitiveType.Cube,
                "PuddleNearOffice",
                new Vector3(-1.8f, 0.065f, 14.2f),
                new Vector3(1.4f, 0.01f, 1f),
                puddle);

            CreateLampPost(new Vector3(-4.8f, 0f, -4f), metal, windowGlow);
            CreateLampPost(new Vector3(4.8f, 0f, 4f), metal, windowGlow);
            CreateLampPost(new Vector3(-4.8f, 0f, 12f), metal, windowGlow);
            CreateLampPost(new Vector3(4.8f, 0f, 18f), metal, windowGlow);
            CreateNoirStreetDressing("Dock", metal, windowGlow, crateWood, officeSign, sidewalk, puddle, brass);

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, -12f);
            AssignMaterial(player, playerCoat);
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

            CreateSpawnPoint("DefaultSpawnPoint", "DefaultSpawn", new Vector3(0f, 1f, -12f));
            CreateSpawnPoint("PickupSpawnPoint", "PickupSpawn", new Vector3(0f, 1f, -12f));
            CreateSpawnPoint("DriveSpawnPoint", "DriveSpawn", new Vector3(0f, 1f, -4.2f));
            CreateSpawnPoint("ExteriorReturnSpawnPoint", "ExteriorReturn", new Vector3(0f, 1f, 12.5f));
            CreateSpawnPoint("FromBusinessCoreGatePoint", "FromBusinessCoreGate", new Vector3(0f, 1f, -12.8f));
            CreateSpawnPoint("FromRailYardGatePoint", "FromRailYardGate", new Vector3(6.8f, 1f, -1.6f));

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
            SetStringValue(sceneSpawnController, "defaultSpawnPointId", "PickupSpawn");
            var dialogueController = campaignSystems.AddComponent<DialogueController>();
            SetObjectReference(dialogueController, "playerController", playerController);

            var cameraRoot = new GameObject("CameraRig");
            var camera = cameraRoot.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.28f, 0.29f, 0.33f);
            camera.orthographic = false;
            camera.fieldOfView = 46f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            cameraRoot.transform.position = new Vector3(0f, 19f, -16f);
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            SetVector3Value(topDownCamera, "offset", new Vector3(0f, 18f, -13f));
            SetFloatValue(topDownCamera, "minimumHeight", 16f);
            SetFloatValue(topDownCamera, "maximumHeight", 24f);

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
            var luca = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            luca.name = "LucaContact";
            luca.transform.SetParent(docksMissionRoot.transform, false);
            luca.transform.position = new Vector3(-3.2f, 1f, -10.4f);
            AssignMaterial(luca, contactCoat);
            var lucaInteractable = luca.AddComponent<DialogueInteractable>();
            SetObjectReference(lucaInteractable, "dialogueController", dialogueController);
            SetObjectReference(lucaInteractable, "dialogueSequence", lucaBriefing);
            SetStringValue(lucaInteractable, "promptText", "Talk to Luca");

            var car = GameObject.CreatePrimitive(PrimitiveType.Cube);
            car.name = "Sedan";
            car.transform.SetParent(docksMissionRoot.transform, false);
            car.transform.position = new Vector3(0f, 0.75f, -3f);
            car.transform.localScale = new Vector3(2.2f, 1f, 4.6f);
            AssignMaterial(car, sedanPaint);
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
            CreatePrimitive(
                PrimitiveType.Cube,
                "SedanRoof",
                new Vector3(0f, 1.3f, -3f),
                new Vector3(1.4f, 0.4f, 2.2f),
                roof).transform.SetParent(car.transform, true);

            CreatePrimitive(
                PrimitiveType.Cube,
                "SedanWindshield",
                new Vector3(0f, 1.05f, -4.15f),
                new Vector3(1.3f, 0.35f, 0.12f),
                windowGlow).transform.SetParent(car.transform, true);

            var building = CreatePrimitive(
                PrimitiveType.Cube,
                "BackOfficeBuilding",
                new Vector3(0f, 2.8f, 20f),
                new Vector3(12f, 5.6f, 8f),
                brick);

            CreatePrimitive(
                PrimitiveType.Cube,
                "DoorAwning",
                new Vector3(0f, 2.35f, 15.6f),
                new Vector3(4f, 0.3f, 1.4f),
                awning);

            CreatePrimitive(
                PrimitiveType.Cube,
                "OfficeSign",
                new Vector3(0f, 3.8f, 15.8f),
                new Vector3(3.8f, 0.65f, 0.2f),
                officeSign);

            CreatePrimitive(
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

            CreatePrimitive(
                PrimitiveType.Cube,
                "LeftCrateStack",
                new Vector3(-3.1f, 0.6f, 15.8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                crateWood);

            CreatePrimitive(
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
                new Vector3(12.8f, 1.05f, -1.8f),
                new Vector3(0.4f, 2.1f, 5.2f),
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
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.45f, 0.32f, 0.21f), 0.22f, 0f);
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

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "InteriorPlayer";
            player.transform.position = new Vector3(0f, 1f, -5.5f);
            AssignMaterial(player, officeTrim);
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
            camera.fieldOfView = 52f;
            cameraRoot.transform.position = new Vector3(0f, 15f, -10f);
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            SetVector3Value(topDownCamera, "offset", new Vector3(0f, 14f, -9f));
            SetFloatValue(topDownCamera, "minimumHeight", 11f);
            SetFloatValue(topDownCamera, "maximumHeight", 18f);

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

            var vincent = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            vincent.name = "VincentContact";
            vincent.transform.SetParent(backOfficeMissionRoot.transform, false);
            vincent.transform.position = new Vector3(0f, 1f, 3.7f);
            AssignMaterial(vincent, contactCoat);
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
            var contactCoat = GetOrCreateMaterial("Assets/Game/Materials/ContactCoat.mat", new Color(0.45f, 0.32f, 0.21f), 0.22f, 0f);

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

            var bookkeeper = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bookkeeper.name = "NicoBelloriInterior";
            bookkeeper.transform.position = new Vector3(0f, 1f, 3.9f);
            AssignMaterial(bookkeeper, contactCoat);
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
            var unionCrackdownMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/UnionCrackdown.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.18f, 0.18f, 0.2f), 0.72f, 0f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/Sidewalk.mat", new Color(0.36f, 0.34f, 0.32f), 0.24f, 0f);
            var stone = GetOrCreateMaterial("Assets/Game/Materials/Stone.mat", new Color(0.28f, 0.27f, 0.24f), 0.22f, 0f);
            var brick = GetOrCreateMaterial("Assets/Game/Materials/Brick.mat", new Color(0.31f, 0.19f, 0.18f), 0.12f, 0f);
            var lanePaint = GetOrCreateMaterial("Assets/Game/Materials/LanePaint.mat", new Color(0.9f, 0.76f, 0.34f), 0.65f, 0f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(0.92f, 0.74f, 0.42f), 0.78f, 0.1f, new Color(0.92f, 0.74f, 0.42f) * 0.9f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var policeCoat = GetOrCreateMaterial("Assets/Game/Materials/PoliceCoat.mat", new Color(0.08f, 0.13f, 0.19f), 0.18f, 0f);
            var officeSign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);

            ApplyExteriorAtmosphere(new Color(0.16f, 0.17f, 0.2f), new Color(0.16f, 0.18f, 0.21f), 0.014f, new Color(1f, 0.9f, 0.78f), 0.88f, Quaternion.Euler(38f, -24f, 0f));

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "BusinessGround";
            ground.transform.localScale = new Vector3(4f, 1f, 4f);
            AssignMaterial(ground, asphalt);

            CreatePrimitive(PrimitiveType.Cube, "BusinessRoad", new Vector3(0f, 0.05f, 0f), new Vector3(10f, 0.1f, 36f), asphalt);
            CreatePrimitive(PrimitiveType.Cube, "BusinessLeftWalk", new Vector3(-6.2f, 0.1f, 0f), new Vector3(3f, 0.2f, 36f), sidewalk);
            CreatePrimitive(PrimitiveType.Cube, "BusinessRightWalk", new Vector3(6.2f, 0.1f, 0f), new Vector3(3f, 0.2f, 36f), sidewalk);
            CreatePrimitive(PrimitiveType.Cube, "BankFacade", new Vector3(-10.4f, 3.4f, 6.6f), new Vector3(6f, 6.8f, 13f), stone);
            CreatePrimitive(PrimitiveType.Cube, "StoneRow", new Vector3(10.5f, 3.2f, 0f), new Vector3(5.7f, 6.4f, 33f), stone);
            CreatePrimitive(PrimitiveType.Cube, "BrickCorner", new Vector3(-10.7f, 2.7f, -11f), new Vector3(5.3f, 5.4f, 8.5f), brick);
            CreatePrimitive(PrimitiveType.Cube, "SquarePlaza", new Vector3(-2.7f, 0.08f, 13.5f), new Vector3(5.5f, 0.04f, 6f), stone);
            CreatePrimitive(PrimitiveType.Cube, "StatueBase", new Vector3(-2.7f, 0.8f, 13.5f), new Vector3(1.2f, 1.2f, 1.2f), stone);
            CreatePrimitive(PrimitiveType.Cylinder, "StatueColumn", new Vector3(-2.7f, 2.3f, 13.5f), new Vector3(0.25f, 1.2f, 0.25f), brass);
            CreatePrimitive(PrimitiveType.Cube, "StreetcarRailA", new Vector3(-1.7f, 0.075f, 0f), new Vector3(0.08f, 0.01f, 36f), metal);
            CreatePrimitive(PrimitiveType.Cube, "StreetcarRailB", new Vector3(1.7f, 0.075f, 0f), new Vector3(0.08f, 0.01f, 36f), metal);

            for (var index = 0; index < 6; index += 1)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "BusinessLaneMarker_" + index,
                    new Vector3(0f, 0.11f, -12f + (index * 5f)),
                    new Vector3(0.35f, 0.02f, 1.8f),
                    lanePaint);
            }

            for (var index = 0; index < 5; index += 1)
            {
                var z = -12f + (index * 6f);
                CreatePrimitive(PrimitiveType.Cube, "BusinessWindowL_" + index, new Vector3(-8.2f, 3.2f, z), new Vector3(1.5f, 1.8f, 0.12f), windowGlow);
                CreatePrimitive(PrimitiveType.Cube, "BusinessWindowR_" + index, new Vector3(8.3f, 3.2f, z + 1.4f), new Vector3(1.5f, 1.8f, 0.12f), windowGlow);
            }

            CreateLampPost(new Vector3(-4.8f, 0f, -6f), metal, windowGlow);
            CreateLampPost(new Vector3(4.8f, 0f, 0f), metal, windowGlow);
            CreateLampPost(new Vector3(-4.8f, 0f, 9f), metal, windowGlow);
            CreateLampPost(new Vector3(4.8f, 0f, 16f), metal, windowGlow);
            CreateNoirStreetDressing("Business", metal, windowGlow, crateWood, officeSign, sidewalk, puddle, brass);

            var runtime = CreateFreeRoamDistrictRuntime(
                "District_BusinessCore_01",
                "BusinessCoreStart",
                new Vector3(0f, 1f, -15f),
                new Color(0.16f, 0.17f, 0.21f),
                new Vector3(0f, 18f, -13f),
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
            CreateSpawnPoint("FromDocksGatePoint", "FromDocksGate", new Vector3(0f, 1f, -14f));
            CreateSpawnPoint("FromOldQuarterGatePoint", "FromOldQuarterGate", new Vector3(12f, 1f, 0f));
            CreateSpawnPoint("FromBookkeeperInteriorPoint", "FromBookkeeperInterior", new Vector3(5.4f, 1f, -3.2f));

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
                new Vector3(13.8f, 1.1f, 0f),
                new Vector3(0.4f, 2.2f, 6.4f),
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

            CreateStaticVehicle("BusinessSedan", new Vector3(3.8f, 0.72f, 6.5f), new Vector3(1.8f, 0.95f, 4f), new Color(0.12f, 0.12f, 0.14f), metal, windowGlow);
            CreateEnemyGuard("BusinessWatchman", new Vector3(4.2f, 1f, 11.5f), enemyCoat, runtime.PlayerController, runtime.PlayerHealth);

            var unionMissionRoot = new GameObject("UnionDueMissionRoot");
            var bookkeeper = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bookkeeper.name = "BookkeeperContact";
            bookkeeper.transform.SetParent(unionMissionRoot.transform, false);
            bookkeeper.transform.position = new Vector3(-1.6f, 1f, 13.8f);
            AssignMaterial(bookkeeper, brass);
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
            var chapelAshMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/ChapelAsh.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.17f, 0.17f, 0.19f), 0.72f, 0f);
            var sidewalk = GetOrCreateMaterial("Assets/Game/Materials/Sidewalk.mat", new Color(0.33f, 0.31f, 0.29f), 0.2f, 0f);
            var tenement = GetOrCreateMaterial("Assets/Game/Materials/TenementBrick.mat", new Color(0.3f, 0.17f, 0.15f), 0.1f, 0f);
            var churchStone = GetOrCreateMaterial("Assets/Game/Materials/ChurchStone.mat", new Color(0.36f, 0.34f, 0.29f), 0.18f, 0f);
            var laundry = GetOrCreateMaterial("Assets/Game/Materials/LaundryCloth.mat", new Color(0.67f, 0.63f, 0.58f), 0.12f, 0f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(0.92f, 0.74f, 0.42f), 0.78f, 0.1f, new Color(0.92f, 0.74f, 0.42f) * 0.9f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var policeCoat = GetOrCreateMaterial("Assets/Game/Materials/PoliceCoat.mat", new Color(0.08f, 0.13f, 0.19f), 0.18f, 0f);
            var sign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);

            ApplyExteriorAtmosphere(new Color(0.14f, 0.15f, 0.18f), new Color(0.15f, 0.16f, 0.18f), 0.016f, new Color(0.96f, 0.87f, 0.78f), 0.8f, Quaternion.Euler(32f, -34f, 0f));

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "QuarterGround";
            ground.transform.localScale = new Vector3(3.6f, 1f, 3.6f);
            AssignMaterial(ground, asphalt);

            CreatePrimitive(PrimitiveType.Cube, "QuarterRoad", new Vector3(0f, 0.05f, 0f), new Vector3(8.4f, 0.1f, 32f), asphalt);
            CreatePrimitive(PrimitiveType.Cube, "QuarterLeftWalk", new Vector3(-5.5f, 0.1f, 0f), new Vector3(2.6f, 0.2f, 32f), sidewalk);
            CreatePrimitive(PrimitiveType.Cube, "QuarterRightWalk", new Vector3(5.5f, 0.1f, 0f), new Vector3(2.6f, 0.2f, 32f), sidewalk);
            CreatePrimitive(PrimitiveType.Cube, "TenementRowLeft", new Vector3(-9.2f, 3f, 0f), new Vector3(4.8f, 6f, 30f), tenement);
            CreatePrimitive(PrimitiveType.Cube, "TenementRowRight", new Vector3(9.2f, 3f, 0f), new Vector3(4.8f, 6f, 30f), tenement);
            CreatePrimitive(PrimitiveType.Cube, "ChurchBody", new Vector3(-9.2f, 4.2f, 11.5f), new Vector3(6f, 8.4f, 9.4f), churchStone);
            CreatePrimitive(PrimitiveType.Cube, "ChurchTower", new Vector3(-9.2f, 7.3f, 16.2f), new Vector3(2.2f, 14f, 2.2f), churchStone);
            CreatePrimitive(PrimitiveType.Cube, "NarrowAlley", new Vector3(0f, 0.06f, -9.4f), new Vector3(3.4f, 0.04f, 7f), sidewalk);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -10f + (index * 5.8f);
                CreatePrimitive(PrimitiveType.Cube, "QuarterWindowL_" + index, new Vector3(-7.2f, 3.1f, z), new Vector3(1.2f, 1.6f, 0.1f), windowGlow);
                CreatePrimitive(PrimitiveType.Cube, "QuarterWindowR_" + index, new Vector3(7.2f, 3.1f, z + 1.1f), new Vector3(1.2f, 1.6f, 0.1f), windowGlow);
                CreatePrimitive(PrimitiveType.Cube, "LaundryLine_" + index, new Vector3(0f, 5.6f, z + 2f), new Vector3(9.6f, 0.05f, 0.05f), metal);
                CreatePrimitive(PrimitiveType.Cube, "LaundryCloth_" + index, new Vector3(-1.8f + (index * 0.7f), 5.25f, z + 2f), new Vector3(0.42f, 0.6f, 0.02f), laundry);
            }

            CreateLampPost(new Vector3(-4.2f, 0f, -4f), metal, windowGlow);
            CreateLampPost(new Vector3(4.2f, 0f, 4f), metal, windowGlow);
            CreateLampPost(new Vector3(-4.2f, 0f, 12f), metal, windowGlow);
            CreateNoirStreetDressing("Quarter", metal, windowGlow, crateWood, sign, sidewalk, puddle, brass);

            var runtime = CreateFreeRoamDistrictRuntime(
                "District_OldQuarter_01",
                "OldQuarterStart",
                new Vector3(0f, 1f, -12.8f),
                new Color(0.14f, 0.15f, 0.18f),
                new Vector3(0f, 17f, -12f),
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

            CreateSpawnPoint("OldQuarterStartPoint", "OldQuarterStart", new Vector3(0f, 1f, -12.8f));
            CreateSpawnPoint("FromBusinessCoreGatePoint", "FromBusinessCoreGate", new Vector3(12.2f, 1f, 0f));
            CreateSpawnPoint("FromChapelInteriorPoint", "FromChapelInterior", new Vector3(-5.2f, 1f, 8.4f));

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
                new Vector3(-13.1f, 1.1f, 6.5f),
                new Vector3(0.4f, 2.2f, 6.2f),
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

            CreateStaticVehicle("QuarterCoupe", new Vector3(-2.9f, 0.72f, 7.4f), new Vector3(1.7f, 0.92f, 3.8f), new Color(0.19f, 0.07f, 0.05f), metal, windowGlow);
            CreateEnemyGuard("QuarterCollector", new Vector3(2.6f, 1f, 9.2f), enemyCoat, runtime.PlayerController, runtime.PlayerHealth);

            var chapelMissionRoot = new GameObject("ChapelDebtMissionRoot");
            var debtor = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            debtor.name = "DebtorContact";
            debtor.transform.SetParent(chapelMissionRoot.transform, false);
            debtor.transform.position = new Vector3(-5.8f, 1f, 10.2f);
            AssignMaterial(debtor, sign);
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
            var yardBetrayalMission = AssetDatabase.LoadAssetAtPath<MissionDefinitionAsset>("Assets/Game/Data/Missions/YardBetrayal.asset");

            EnsureFolder("Assets/Game/Materials");
            var asphalt = GetOrCreateMaterial("Assets/Game/Materials/Asphalt.mat", new Color(0.16f, 0.17f, 0.18f), 0.72f, 0f);
            var gravel = GetOrCreateMaterial("Assets/Game/Materials/Gravel.mat", new Color(0.24f, 0.23f, 0.22f), 0.18f, 0f);
            var rustMetal = GetOrCreateMaterial("Assets/Game/Materials/RustMetal.mat", new Color(0.34f, 0.22f, 0.17f), 0.58f, 0.45f);
            var tankMetal = GetOrCreateMaterial("Assets/Game/Materials/TankMetal.mat", new Color(0.22f, 0.26f, 0.28f), 0.72f, 0.62f);
            var windowGlow = GetOrCreateMaterial("Assets/Game/Materials/WindowGlow.mat", new Color(0.92f, 0.74f, 0.42f), 0.78f, 0.1f, new Color(0.92f, 0.74f, 0.42f) * 0.9f);
            var metal = GetOrCreateMaterial("Assets/Game/Materials/Metal.mat", new Color(0.27f, 0.28f, 0.3f), 0.78f, 0.7f);
            var enemyCoat = GetOrCreateMaterial("Assets/Game/Materials/EnemyCoat.mat", new Color(0.14f, 0.16f, 0.18f), 0.15f, 0f);
            var policeCoat = GetOrCreateMaterial("Assets/Game/Materials/PoliceCoat.mat", new Color(0.08f, 0.13f, 0.19f), 0.18f, 0f);
            var sign = GetOrCreateMaterial("Assets/Game/Materials/OfficeSign.mat", new Color(0.13f, 0.12f, 0.12f), 0.32f, 0f);
            var crateWood = GetOrCreateMaterial("Assets/Game/Materials/CrateWood.mat", new Color(0.44f, 0.29f, 0.18f), 0.18f, 0f);
            var puddle = GetOrCreateMaterial("Assets/Game/Materials/Puddle.mat", new Color(0.12f, 0.14f, 0.15f), 0.96f, 0.02f);
            var brass = GetOrCreateMaterial("Assets/Game/Materials/Brass.mat", new Color(0.63f, 0.47f, 0.21f), 0.8f, 0.88f);

            ApplyExteriorAtmosphere(new Color(0.13f, 0.14f, 0.17f), new Color(0.14f, 0.16f, 0.18f), 0.02f, new Color(0.95f, 0.84f, 0.71f), 0.76f, Quaternion.Euler(28f, -40f, 0f));

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "RailGround";
            ground.transform.localScale = new Vector3(4f, 1f, 4f);
            AssignMaterial(ground, gravel);

            CreatePrimitive(PrimitiveType.Cube, "RailRoad", new Vector3(0f, 0.05f, -8.5f), new Vector3(8.8f, 0.1f, 16f), asphalt);
            CreatePrimitive(PrimitiveType.Cube, "TrackA", new Vector3(-2.2f, 0.07f, 8f), new Vector3(0.08f, 0.02f, 26f), metal);
            CreatePrimitive(PrimitiveType.Cube, "TrackB", new Vector3(2.2f, 0.07f, 8f), new Vector3(0.08f, 0.02f, 26f), metal);
            CreatePrimitive(PrimitiveType.Cube, "Sleepers", new Vector3(0f, 0.05f, 8f), new Vector3(5.4f, 0.02f, 26f), rustMetal);
            CreatePrimitive(PrimitiveType.Cube, "GarageHall", new Vector3(-9.8f, 2.9f, -2f), new Vector3(6f, 5.8f, 16f), tankMetal);
            CreatePrimitive(PrimitiveType.Cube, "TrainCar", new Vector3(2.2f, 1.55f, 10.5f), new Vector3(2.8f, 3f, 8.2f), rustMetal);
            CreatePrimitive(PrimitiveType.Cylinder, "FuelTankA", new Vector3(9.6f, 2.2f, -2.5f), new Vector3(1.3f, 2.2f, 1.3f), tankMetal);
            CreatePrimitive(PrimitiveType.Cylinder, "FuelTankB", new Vector3(9.6f, 2.2f, 4.2f), new Vector3(1.1f, 2.2f, 1.1f), tankMetal);

            for (var index = 0; index < 4; index += 1)
            {
                CreatePrimitive(PrimitiveType.Cube, "GarageWindow_" + index, new Vector3(-7.5f, 2.8f, -9f + (index * 5f)), new Vector3(1.6f, 1.2f, 0.1f), windowGlow);
            }

            CreateLampPost(new Vector3(-4.6f, 0f, -11f), metal, windowGlow);
            CreateLampPost(new Vector3(4.6f, 0f, -4f), metal, windowGlow);
            CreateLampPost(new Vector3(4.6f, 0f, 11f), metal, windowGlow);
            CreateNoirStreetDressing("Rail", metal, windowGlow, crateWood, sign, gravel, puddle, brass);

            var runtime = CreateFreeRoamDistrictRuntime(
                "District_RailYard_01",
                "RailYardStart",
                new Vector3(0f, 1f, -13.5f),
                new Color(0.13f, 0.14f, 0.17f),
                new Vector3(0f, 17f, -12f),
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

            CreateSpawnPoint("RailYardStartPoint", "RailYardStart", new Vector3(0f, 1f, -13.5f));
            CreateSpawnPoint("FromDocksGatePoint", "FromDocksGate", new Vector3(0f, 1f, -12.2f));
            CreateSpawnPoint("FromOldQuarterGatePoint", "FromOldQuarterGate", new Vector3(-2.6f, 1f, -12.2f));
            CreateSpawnPoint("FromGarageInteriorPoint", "FromGarageInterior", new Vector3(-5.2f, 1f, -6.8f));

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
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_OldQuarter_Chapel_01.unity", true),
                new EditorBuildSettingsScene("Assets/Game/Scenes/Interior_RailYard_Garage_01.unity", true),
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
            SaveGameFileService saveGameFileService = null,
            CampaignDatabaseAsset campaignDatabase = null)
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
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "InteriorPlayer";
            player.transform.position = playerPosition;
            AssignMaterial(player, GetOrCreateMaterial("Assets/Game/Materials/PlayerCoat.mat", new Color(0.68f, 0.67f, 0.61f), 0.15f, 0f));
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
            camera.fieldOfView = 52f;
            cameraRoot.transform.position = playerPosition + cameraOffset;
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            SetVector3Value(topDownCamera, "offset", cameraOffset);
            SetFloatValue(topDownCamera, "minimumHeight", 11f);
            SetFloatValue(topDownCamera, "maximumHeight", 18f);

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
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = playerPosition;
            AssignMaterial(player, GetOrCreateMaterial("Assets/Game/Materials/PlayerCoat.mat", new Color(0.68f, 0.67f, 0.61f), 0.15f, 0f));
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
            camera.fieldOfView = 46f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            cameraRoot.transform.position = playerPosition + cameraOffset;
            var topDownCamera = cameraRoot.AddComponent<TopDownCameraController>();
            SetObjectReference(topDownCamera, "followTarget", player.transform);
            SetVector3Value(topDownCamera, "offset", cameraOffset);
            SetFloatValue(topDownCamera, "minimumHeight", 15f);
            SetFloatValue(topDownCamera, "maximumHeight", 24f);

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
            CreatePrimitive(PrimitiveType.Cube, prefix + "LeftCurbShadow", new Vector3(-4.55f, 0.13f, 0f), new Vector3(0.18f, 0.04f, 31f), sign);
            CreatePrimitive(PrimitiveType.Cube, prefix + "RightCurbShadow", new Vector3(4.55f, 0.13f, 0f), new Vector3(0.18f, 0.04f, 31f), sign);

            for (var index = 0; index < 5; index += 1)
            {
                var z = -12f + (index * 6f);
                var leftX = -6.55f - (index % 2 * 0.45f);
                var rightX = 6.65f + (index % 2 * 0.4f);

                CreatePrimitive(PrimitiveType.Cube, prefix + "PosterBoardL_" + index, new Vector3(leftX, 1.45f, z), new Vector3(0.12f, 1.05f, 1.1f), sign);
                CreatePrimitive(PrimitiveType.Cube, prefix + "PosterTrimL_" + index, new Vector3(leftX + 0.02f, 1.98f, z), new Vector3(0.14f, 0.08f, 1.16f), brass);
                CreatePrimitive(PrimitiveType.Cube, prefix + "NeonSlashR_" + index, new Vector3(rightX, 2.2f, z + 2.6f), new Vector3(0.12f, 0.18f, 1.45f), glow);
                CreatePrimitive(PrimitiveType.Cube, prefix + "Awning_" + index, new Vector3(rightX, 2.05f, z + 0.7f), new Vector3(1.25f, 0.18f, 1.5f), sign);
            }

            for (var index = 0; index < 6; index += 1)
            {
                var z = -13f + (index * 5f);
                var x = index % 2 == 0 ? -3.25f : 3.15f;
                CreatePrimitive(PrimitiveType.Cube, prefix + "WetStreetGlint_" + index, new Vector3(x, 0.125f, z), new Vector3(1.25f, 0.015f, 0.55f), puddle);
            }

            CreatePrimitive(PrimitiveType.Cube, prefix + "NewsStandBase", new Vector3(-5.75f, 0.75f, -7.6f), new Vector3(1.25f, 1.1f, 1.1f), wood);
            CreatePrimitive(PrimitiveType.Cube, prefix + "NewsStandRoof", new Vector3(-5.75f, 1.45f, -7.6f), new Vector3(1.55f, 0.18f, 1.35f), sign);
            CreatePrimitive(PrimitiveType.Cube, prefix + "Payphone", new Vector3(5.65f, 1.25f, -6.2f), new Vector3(0.55f, 1.8f, 0.45f), metal);
            CreatePrimitive(PrimitiveType.Cube, prefix + "PayphoneGlow", new Vector3(5.65f, 1.85f, -6.47f), new Vector3(0.42f, 0.42f, 0.05f), glow);

            for (var index = 0; index < 4; index += 1)
            {
                CreatePrimitive(PrimitiveType.Cylinder, prefix + "Barrel_" + index, new Vector3(-6.75f + (index * 0.55f), 0.65f, 6.8f + (index % 2 * 0.55f)), new Vector3(0.28f, 0.6f, 0.28f), metal);
                CreatePrimitive(PrimitiveType.Cube, prefix + "Crate_" + index, new Vector3(6.2f + (index % 2 * 0.55f), 0.55f, 8.8f + (index * 0.55f)), new Vector3(0.8f, 0.8f, 0.8f), wood);
            }

            CreatePrimitive(PrimitiveType.Cube, prefix + "OverheadCableA", new Vector3(0f, 5.95f, -3f), new Vector3(13.2f, 0.035f, 0.035f), metal);
            CreatePrimitive(PrimitiveType.Cube, prefix + "OverheadCableB", new Vector3(0f, 5.55f, 8f), new Vector3(13.2f, 0.035f, 0.035f), metal);
            CreatePrimitive(PrimitiveType.Cube, prefix + "CigaretteKiosk", new Vector3(5.9f, 0.72f, 13.8f), new Vector3(1.15f, 1.1f, 0.9f), groundAccent);
            CreatePrimitive(PrimitiveType.Cube, prefix + "KioskSign", new Vector3(5.9f, 1.55f, 13.8f), new Vector3(1.28f, 0.22f, 1f), brass);
            CreateSteamVent(prefix + "SteamVentA", new Vector3(-3.8f, 0.12f, 11.2f), metal, glow);
            CreateSteamVent(prefix + "SteamVentB", new Vector3(3.5f, 0.12f, -10.6f), metal, glow);
        }

        private static void CreateSteamVent(string name, Vector3 position, Material metal, Material glow)
        {
            CreatePrimitive(PrimitiveType.Cylinder, name + "Grate", position, new Vector3(0.45f, 0.04f, 0.45f), metal);
            CreatePrimitive(PrimitiveType.Cube, name + "VaporA", position + new Vector3(0f, 0.55f, 0f), new Vector3(0.28f, 0.75f, 0.28f), glow);
            CreatePrimitive(PrimitiveType.Cube, name + "VaporB", position + new Vector3(0.24f, 1f, 0.12f), new Vector3(0.2f, 0.8f, 0.2f), glow);
        }

        private static void CreateLampPost(Vector3 basePosition, Material poleMaterial, Material glowMaterial)
        {
            var pole = CreatePrimitive(
                PrimitiveType.Cylinder,
                "LampPost",
                basePosition + new Vector3(0f, 2f, 0f),
                new Vector3(0.15f, 2f, 0.15f),
                poleMaterial);

            var lampHead = CreatePrimitive(
                PrimitiveType.Cube,
                "LampHead",
                basePosition + new Vector3(0f, 4.25f, 0.2f),
                new Vector3(0.45f, 0.3f, 0.45f),
                glowMaterial);

            var pointLightObject = new GameObject("LampLight");
            pointLightObject.transform.position = basePosition + new Vector3(0f, 3.8f, 0.2f);
            var pointLight = pointLightObject.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 9f;
            pointLight.intensity = 2.4f;
            pointLight.color = new Color(1f, 0.8f, 0.55f);
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
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
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

            if (emissionColor.HasValue)
            {
                material.EnableKeyword("_EMISSION");
                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", emissionColor.Value);
                }
            }

            return material;
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
            var guard = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            guard.name = name;
            guard.transform.position = position;
            if (parent != null)
            {
                guard.transform.SetParent(parent, true);
            }

            AssignMaterial(guard, material);
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
            var officer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            officer.name = name;
            officer.transform.position = position;
            AssignMaterial(officer, material);

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

            CreatePrimitive(
                PrimitiveType.Cube,
                name + "Roof",
                position + new Vector3(0f, 0.58f, -0.08f),
                new Vector3(bodyScale.x * 0.64f, 0.34f, bodyScale.z * 0.48f),
                roofMaterial).transform.SetParent(body.transform, true);

            CreatePrimitive(
                PrimitiveType.Cube,
                name + "Glass",
                position + new Vector3(0f, 0.45f, -bodyScale.z * 0.25f),
                new Vector3(bodyScale.x * 0.58f, 0.22f, 0.1f),
                glassMaterial).transform.SetParent(body.transform, true);
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
            SimpleObjectiveSystem objectiveSystem = null,
            string requiredObjectiveId = "",
            CampaignProgressionController campaignProgressionController = null,
            MissionDefinitionAsset missionAsset = null,
            string missionStageId = "")
        {
            var gate = CreatePrimitive(PrimitiveType.Cube, name, position, scale, material);
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
    }
}
