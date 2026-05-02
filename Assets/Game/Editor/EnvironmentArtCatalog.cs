#nullable enable annotations

using UnityEditor;
using UnityEngine;

namespace MafiaTopDown.Editor
{
    internal static class EnvironmentArtCatalog
    {
        private const string QuaterniusUltimateBuildingsRoot = "Assets/ThirdParty/Quaternius/UltimateBuildingsPack/Models/FBX/";
        private const string KenneyRoot = "Assets/ThirdParty/Kenney/";

        internal static GameObject? CreateDocksBuilding(
            DocksBuildingRole role,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform parent)
        {
            return CreateDistrictBuilding(
                DistrictArtStyle.Docks,
                MapDocksRole(role),
                instanceName,
                position,
                rotationEuler,
                scale,
                parent);
        }

        internal static GameObject? CreateDistrictBuilding(
            DistrictArtStyle district,
            DistrictBuildingRole role,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform parent)
        {
            return CreateFirstAvailable(
                GetDistrictBuildingChoices(district, role),
                instanceName,
                position,
                rotationEuler,
                scale,
                parent);
        }

        internal static GameObject? CreateKenneyFallback(
            string packName,
            string modelName,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform parent)
        {
            return CreateModel(
                KenneyRoot + packName + "/Models/FBX format/" + modelName + ".fbx",
                instanceName,
                position,
                rotationEuler,
                scale,
                parent);
        }

        private static DistrictBuildingRole MapDocksRole(DocksBuildingRole role)
        {
            switch (role)
            {
                case DocksBuildingRole.Warehouse:
                    return DistrictBuildingRole.Warehouse;
                case DocksBuildingRole.CornerShop:
                    return DistrictBuildingRole.CornerShop;
                case DocksBuildingRole.NarrowTenement:
                    return DistrictBuildingRole.NarrowTenement;
                case DocksBuildingRole.RowHouse:
                    return DistrictBuildingRole.RowHouse;
                case DocksBuildingRole.OfficeBlock:
                    return DistrictBuildingRole.OfficeBlock;
                default:
                    return DistrictBuildingRole.RowHouse;
            }
        }

        private static string[] GetDistrictBuildingChoices(DistrictArtStyle district, DistrictBuildingRole role)
        {
            switch (district)
            {
                case DistrictArtStyle.BusinessCore:
                    return GetBusinessCoreChoices(role);
                case DistrictArtStyle.OldQuarter:
                    return GetOldQuarterChoices(role);
                case DistrictArtStyle.RailYard:
                    return GetRailYardChoices(role);
                default:
                    return GetDocksChoices(role);
            }
        }

        private static string[] GetDocksChoices(DistrictBuildingRole role)
        {
            switch (role)
            {
                case DistrictBuildingRole.Warehouse:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "4Story_Wide_2Doors_Roof_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "4Story_Wide_2Doors_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "4Story_Mat.fbx",
                        KenneyRoot + "CityKitIndustrial/Models/FBX format/building-f.fbx"
                    };

                case DistrictBuildingRole.CornerShop:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Sign_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Wide_2Doors_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Wide_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-k.fbx"
                    };

                case DistrictBuildingRole.NarrowTenement:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "3Story_Slim_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "3Story_Small_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Slim_Mat.fbx",
                        KenneyRoot + "CityKitSuburban/Models/FBX format/building-type-g.fbx"
                    };

                case DistrictBuildingRole.RowHouse:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Balcony_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Center_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Mat.fbx",
                        KenneyRoot + "CityKitSuburban/Models/FBX format/building-type-r.fbx"
                    };

                case DistrictBuildingRole.OfficeBlock:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "4Story_Center_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "3Story_Balcony_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Columns_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-h.fbx"
                    };

                default:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-d.fbx"
                    };
            }
        }

        private static string[] GetBusinessCoreChoices(DistrictBuildingRole role)
        {
            switch (role)
            {
                case DistrictBuildingRole.HeroLandmark:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "4Story_Center_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "4Story_Wide_2Doors_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-h.fbx"
                    };

                case DistrictBuildingRole.OfficeBlock:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "4Story_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "4Story_Wide_2Doors_Roof_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-skyscraper-c.fbx"
                    };

                case DistrictBuildingRole.CornerShop:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Columns_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Sign_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-k.fbx"
                    };

                default:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "3Story_Balcony_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Wide_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-d.fbx"
                    };
            }
        }

        private static string[] GetOldQuarterChoices(DistrictBuildingRole role)
        {
            switch (role)
            {
                case DistrictBuildingRole.HeroLandmark:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Columns_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_GableRoof_Mat.fbx",
                        KenneyRoot + "CityKitSuburban/Models/FBX format/building-type-q.fbx"
                    };

                case DistrictBuildingRole.NarrowTenement:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "3Story_Slim_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "3Story_Small_Mat.fbx",
                        KenneyRoot + "CityKitSuburban/Models/FBX format/building-type-g.fbx"
                    };

                case DistrictBuildingRole.RowHouse:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Balcony_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Sidehouse_Mat.fbx",
                        KenneyRoot + "CityKitSuburban/Models/FBX format/building-type-r.fbx"
                    };

                default:
                    return new[]
                    {
                        QuaterniusUltimateBuildingsRoot + "2Story_Mat.fbx",
                        QuaterniusUltimateBuildingsRoot + "1Story_GableRoof_Mat.fbx",
                        KenneyRoot + "CityKitSuburban/Models/FBX format/building-type-c.fbx"
                    };
            }
        }

        private static string[] GetRailYardChoices(DistrictBuildingRole role)
        {
            switch (role)
            {
                case DistrictBuildingRole.HeroLandmark:
                case DistrictBuildingRole.Warehouse:
                    return new[]
                    {
                        KenneyRoot + "CityKitIndustrial/Models/FBX format/building-f.fbx",
                        KenneyRoot + "CityKitIndustrial/Models/FBX format/building-r.fbx",
                        QuaterniusUltimateBuildingsRoot + "4Story_Wide_2Doors_Roof_Mat.fbx"
                    };

                case DistrictBuildingRole.CornerShop:
                    return new[]
                    {
                        KenneyRoot + "CityKitIndustrial/Models/FBX format/building-k.fbx",
                        QuaterniusUltimateBuildingsRoot + "2Story_Wide_2Doors_Mat.fbx",
                        KenneyRoot + "CityKitCommercial/Models/FBX format/building-n.fbx"
                    };

                default:
                    return new[]
                    {
                        KenneyRoot + "CityKitIndustrial/Models/FBX format/building-d.fbx",
                        KenneyRoot + "CityKitIndustrial/Models/FBX format/building-h.fbx",
                        QuaterniusUltimateBuildingsRoot + "3Story_Small_Mat.fbx"
                    };
            }
        }

        private static GameObject? CreateFirstAvailable(
            string[] assetPaths,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform parent)
        {
            foreach (var assetPath in assetPaths)
            {
                var instance = CreateModel(assetPath, instanceName, position, rotationEuler, scale, parent);
                if (instance != null)
                {
                    return instance;
                }
            }

            return null;
        }

        private static GameObject? CreateModel(
            string assetPath,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform parent)
        {
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
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(rotationEuler);
            instance.transform.localScale = scale;
            RemoveCollidersRecursive(instance);

            return instance;
        }

        private static void RemoveCollidersRecursive(GameObject gameObject)
        {
            var colliders = gameObject.GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                UnityEngine.Object.DestroyImmediate(collider);
            }
        }
    }

    internal enum DocksBuildingRole
    {
        Warehouse,
        CornerShop,
        NarrowTenement,
        RowHouse,
        OfficeBlock
    }

    internal enum DistrictArtStyle
    {
        Docks,
        BusinessCore,
        OldQuarter,
        RailYard
    }

    internal enum DistrictBuildingRole
    {
        HeroLandmark,
        Warehouse,
        OfficeBlock,
        CornerShop,
        NarrowTenement,
        RowHouse
    }
}
