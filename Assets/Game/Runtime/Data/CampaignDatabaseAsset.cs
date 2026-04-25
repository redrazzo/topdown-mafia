using UnityEngine;

namespace MafiaTopDown.Gameplay.Runtime.Data
{
    [CreateAssetMenu(menuName = "MafiaTopDown/Data/Campaign Database", fileName = "CampaignDatabase")]
    public sealed class CampaignDatabaseAsset : ScriptableObject
    {
        [SerializeField] private MissionDefinitionAsset[] missions = new MissionDefinitionAsset[0];
        [SerializeField] private CampaignChapterAsset[] chapters = new CampaignChapterAsset[0];
        [SerializeField] private DistrictDefinitionAsset[] districts = new DistrictDefinitionAsset[0];
        [SerializeField] private ActivityDefinitionAsset[] activities = new ActivityDefinitionAsset[0];
        [SerializeField] private LandmarkDefinitionAsset[] landmarks = new LandmarkDefinitionAsset[0];
        [SerializeField] private VehicleArchetypeAsset[] vehicles = new VehicleArchetypeAsset[0];
        [SerializeField] private InteriorPortalAsset[] interiorPortals = new InteriorPortalAsset[0];
        [SerializeField] private DialogueSequenceAsset[] dialogues = new DialogueSequenceAsset[0];

        public MissionDefinitionAsset[] Missions => missions;

        public CampaignChapterAsset[] Chapters => chapters;

        public DistrictDefinitionAsset[] Districts => districts;

        public ActivityDefinitionAsset[] Activities => activities;

        public LandmarkDefinitionAsset[] Landmarks => landmarks;

        public VehicleArchetypeAsset[] Vehicles => vehicles;

        public InteriorPortalAsset[] InteriorPortals => interiorPortals;

        public DialogueSequenceAsset[] Dialogues => dialogues;
    }
}
