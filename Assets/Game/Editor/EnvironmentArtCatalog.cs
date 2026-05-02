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
                district,
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
                parent,
                InferDistrict(instanceName));
        }

        internal static void ApplyNoirMaterials(GameObject instance, string assetPath, DistrictArtStyle? district = null)
        {
            var resolvedDistrict = district ?? InferDistrict(instance.name + " " + assetPath);
            var renderers = instance.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                var sourceMaterials = renderer.sharedMaterials;
                if (sourceMaterials.Length == 0)
                {
                    continue;
                }

                var replacementMaterials = new Material[sourceMaterials.Length];
                for (var index = 0; index < sourceMaterials.Length; index += 1)
                {
                    var sourceName = sourceMaterials[index] != null ? sourceMaterials[index].name : string.Empty;
                    replacementMaterials[index] = ResolveNoirMaterial(assetPath, instance.name, sourceName, resolvedDistrict);
                }

                renderer.sharedMaterials = replacementMaterials;
            }
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
            DistrictArtStyle district,
            string instanceName,
            Vector3 position,
            Vector3 rotationEuler,
            Vector3 scale,
            Transform parent)
        {
            foreach (var assetPath in assetPaths)
            {
                var instance = CreateModel(assetPath, instanceName, position, rotationEuler, scale, parent, district);
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
            Transform parent,
            DistrictArtStyle? district = null)
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
            ApplyNoirMaterials(instance, assetPath, district);

            return instance;
        }

        private static DistrictArtStyle InferDistrict(string text)
        {
            if (text.Contains("Business", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Commercial", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Union", System.StringComparison.OrdinalIgnoreCase))
            {
                return DistrictArtStyle.BusinessCore;
            }

            if (text.Contains("Quarter", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Tenement", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Suburban", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Chapel", System.StringComparison.OrdinalIgnoreCase))
            {
                return DistrictArtStyle.OldQuarter;
            }

            if (text.Contains("Rail", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Industrial", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Garage", System.StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Ironline", System.StringComparison.OrdinalIgnoreCase))
            {
                return DistrictArtStyle.RailYard;
            }

            return DistrictArtStyle.Docks;
        }

        private static Material ResolveNoirMaterial(
            string assetPath,
            string instanceName,
            string sourceMaterialName,
            DistrictArtStyle district)
        {
            var key = (assetPath + " " + instanceName + " " + sourceMaterialName).ToLowerInvariant();
            var materialKey = sourceMaterialName.ToLowerInvariant();
            var assetNameStart = assetPath.LastIndexOf('/') + 1;
            var assetName = assetPath.Substring(assetNameStart).ToLowerInvariant();
            if (materialKey.Contains("window") || materialKey.Contains("glass") || materialKey.Contains("lamp") || materialKey.Contains("light") || materialKey.Contains("emissive"))
            {
                return GetOrCreateNoirMaterial("NoirReadyWindowGlow", new Color(1f, 0.72f, 0.34f), 0.72f, 0.02f, new Color(1f, 0.58f, 0.24f) * 1.8f);
            }

            if (assetName.StartsWith("road", System.StringComparison.OrdinalIgnoreCase) ||
                key.Contains("asphalt"))
            {
                return GetOrCreateNoirMaterial("NoirReadyWetAsphalt", new Color(0.055f, 0.064f, 0.072f), 0.84f, 0.02f);
            }

            if (key.Contains("sidewalk") || key.Contains("pavement") || assetName.StartsWith("tile", System.StringComparison.OrdinalIgnoreCase))
            {
                return GetOrCreateNoirMaterial("NoirReadySidewalk", new Color(0.22f, 0.215f, 0.195f), 0.32f, 0f);
            }

            if (key.Contains("roof"))
            {
                return GetOrCreateNoirMaterial("NoirReadyTarRoof", new Color(0.045f, 0.048f, 0.052f), 0.24f, 0f);
            }

            if (key.Contains("metal") ||
                key.Contains("rail") ||
                key.Contains("tank") ||
                key.Contains("chimney") ||
                key.Contains("barrier") ||
                assetName.StartsWith("light", System.StringComparison.OrdinalIgnoreCase) ||
                assetName.StartsWith("bridge", System.StringComparison.OrdinalIgnoreCase) ||
                assetName.StartsWith("construction", System.StringComparison.OrdinalIgnoreCase))
            {
                return GetOrCreateNoirMaterial("NoirReadyOiledMetal", new Color(0.18f, 0.19f, 0.19f), 0.68f, 0.72f);
            }

            if (key.Contains("sign") || key.Contains("awning") || key.Contains("parasol") || key.Contains("cone"))
            {
                return GetOrCreateNoirMaterial("NoirReadyAgedSignage", new Color(0.38f, 0.28f, 0.13f), 0.42f, 0.12f);
            }

            if (key.Contains("tree") || key.Contains("planter"))
            {
                return GetOrCreateNoirMaterial("NoirReadyWetVegetation", new Color(0.1f, 0.14f, 0.1f), 0.28f, 0f);
            }

            switch (district)
            {
                case DistrictArtStyle.BusinessCore:
                    return GetOrCreateNoirMaterial("NoirReadyBusinessStone", new Color(0.25f, 0.235f, 0.205f), 0.28f, 0f);
                case DistrictArtStyle.OldQuarter:
                    return GetOrCreateNoirMaterial("NoirReadyOldQuarterBrick", new Color(0.205f, 0.12f, 0.09f), 0.2f, 0f);
                case DistrictArtStyle.RailYard:
                    return GetOrCreateNoirMaterial("NoirReadyRailCorrugated", new Color(0.17f, 0.155f, 0.135f), 0.46f, 0.32f);
                default:
                    return GetOrCreateNoirMaterial("NoirReadyDockBrick", new Color(0.18f, 0.095f, 0.075f), 0.2f, 0f);
            }
        }

        private static Material GetOrCreateNoirMaterial(
            string materialName,
            Color color,
            float smoothness,
            float metallic,
            Color? emissionColor = null)
        {
            const string folder = "Assets/Game/Materials/ReadyAssetNoir";
            EnsureFolder(folder);
            var assetPath = folder + "/" + materialName + ".mat";
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
            else
            {
                material.DisableKeyword("_EMISSION");
                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", Color.black);
                }
            }

            EditorUtility.SetDirty(material);
            return material;
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
