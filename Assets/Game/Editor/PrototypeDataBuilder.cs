using MafiaTopDown.Gameplay.Runtime.Data;
using UnityEditor;
using UnityEngine;

namespace MafiaTopDown.Editor
{
    public static class PrototypeDataBuilder
    {
        [MenuItem("MafiaTopDown/Setup/Create Prototype Data Assets")]
        public static void CreatePrototypeDataAssets()
        {
            EnsureFolder("Assets/Game/Data");
            EnsureFolder("Assets/Game/Data/Chapters");
            EnsureFolder("Assets/Game/Data/Missions");
            EnsureFolder("Assets/Game/Data/Districts");
            EnsureFolder("Assets/Game/Data/Activities");
            EnsureFolder("Assets/Game/Data/Dialogues");
            EnsureFolder("Assets/Game/Data/Landmarks");
            EnsureFolder("Assets/Game/Data/Vehicles");
            EnsureFolder("Assets/Game/Data/Portals");

            var mission = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/AQuietFavor.asset");
            var unionDue = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/UnionDue.asset");
            var chapelDebt = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/ChapelDebt.asset");
            var yardHeat = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/YardHeat.asset");
            var belloriBooks = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/BelloriBooks.asset");
            var saintVeraSilence = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/SaintVeraSilence.asset");
            var pierNightWatch = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/PierNightWatch.asset");
            var ironlineLedger = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/IronlineLedger.asset");
            var bloodLedger = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/BloodLedger.asset");
            var unionCrackdown = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/UnionCrackdown.asset");
            var chapelAsh = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/ChapelAsh.asset");
            var yardBetrayal = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/YardBetrayal.asset");
            var lastRun = EnsureAsset<MissionDefinitionAsset>("Assets/Game/Data/Missions/LastRun.asset");
            var actOne = EnsureAsset<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActOne.asset");
            var actTwo = EnsureAsset<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActTwo.asset");
            var actThree = EnsureAsset<CampaignChapterAsset>("Assets/Game/Data/Chapters/ActThree.asset");
            var docks = EnsureAsset<DistrictDefinitionAsset>("Assets/Game/Data/Districts/Docks.asset");
            var business = EnsureAsset<DistrictDefinitionAsset>("Assets/Game/Data/Districts/BusinessCore.asset");
            var oldQuarter = EnsureAsset<DistrictDefinitionAsset>("Assets/Game/Data/Districts/OldQuarter.asset");
            var railYard = EnsureAsset<DistrictDefinitionAsset>("Assets/Game/Data/Districts/RailYard.asset");
            var dockCourier = EnsureAsset<ActivityDefinitionAsset>("Assets/Game/Data/Activities/DockCourier.asset");
            var ledgerRun = EnsureAsset<ActivityDefinitionAsset>("Assets/Game/Data/Activities/LedgerRun.asset");
            var lookoutRun = EnsureAsset<ActivityDefinitionAsset>("Assets/Game/Data/Activities/LookoutRun.asset");
            var chopDelivery = EnsureAsset<ActivityDefinitionAsset>("Assets/Game/Data/Activities/ChopDelivery.asset");
            var lucaBriefing = EnsureAsset<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/LucaBriefing.asset");
            var vincentHandoff = EnsureAsset<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/VincentHandoff.asset");
            var bookkeeperWarning = EnsureAsset<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/BookkeeperWarning.asset");
            var debtorThreat = EnsureAsset<DialogueSequenceAsset>("Assets/Game/Data/Dialogues/DebtorThreat.asset");
            var harborOffice = EnsureAsset<LandmarkDefinitionAsset>("Assets/Game/Data/Landmarks/HarborOffice.asset");
            var unionSquare = EnsureAsset<LandmarkDefinitionAsset>("Assets/Game/Data/Landmarks/UnionSquare.asset");
            var saintVera = EnsureAsset<LandmarkDefinitionAsset>("Assets/Game/Data/Landmarks/SaintVera.asset");
            var ironlineGarage = EnsureAsset<LandmarkDefinitionAsset>("Assets/Game/Data/Landmarks/IronlineGarage.asset");
            var fleetlineSedan = EnsureAsset<VehicleArchetypeAsset>("Assets/Game/Data/Vehicles/FleetlineSedan.asset");
            var harborTruck = EnsureAsset<VehicleArchetypeAsset>("Assets/Game/Data/Vehicles/HarborTruck.asset");
            var backOfficePortal = EnsureAsset<InteriorPortalAsset>("Assets/Game/Data/Portals/BackOfficePortal.asset");
            var belloriBooksPortal = EnsureAsset<InteriorPortalAsset>("Assets/Game/Data/Portals/BelloriBooksPortal.asset");
            var saintVeraPortal = EnsureAsset<InteriorPortalAsset>("Assets/Game/Data/Portals/SaintVeraPortal.asset");
            var ironlineOfficePortal = EnsureAsset<InteriorPortalAsset>("Assets/Game/Data/Portals/IronlineOfficePortal.asset");
            var campaign = EnsureAsset<CampaignDatabaseAsset>("Assets/Game/Data/CampaignDatabase.asset");

            PopulateMission(
                mission,
                "mission-a-quiet-favor",
                "A Quiet Favor",
                "Take the assigned sedan, reach the harbor office, deliver the ledger, and leave without drawing heat.",
                "pickup",
                200,
                2,
                new[] { "business-core" },
                new[] { "activity-ledger-run" },
                new[]
                {
                    ("pickup", "Take the sedan", "Enter the assigned sedan.", "drive", true),
                    ("drive", "Reach the office", "Drive to the family office.", "handoff", true),
                    ("handoff", "Hand over the ledger", "Deliver the ledger inside the back office.", "leave", true),
                    ("leave", "Leave the office", "Walk back outside clean.", string.Empty, false)
                },
                new[]
                {
                    ("pickup-checkpoint", "District_01", "PickupSpawn", "pickup"),
                    ("drive-checkpoint", "District_01", "DriveSpawn", "drive"),
                    ("handoff-checkpoint", "Interior_BackOffice_01", "InteriorSpawn", "handoff"),
                    ("leave-checkpoint", "Interior_BackOffice_01", "LedgerDeskSpawn", "leave")
                },
                "assigned-car-lost",
                1,
                "Do not lose the assigned sedan.");
            PopulateMission(
                unionDue,
                "mission-union-due",
                "Union Due",
                "Carry the family’s warning to a union bookkeeper in the business core and get back out before the patrols notice.",
                "meet-bookkeeper",
                250,
                1,
                new[] { "old-quarter" },
                new[] { "activity-lookout-run" },
                new[]
                {
                    ("meet-bookkeeper", "Meet the bookkeeper", "Reach the business core contact.", "leave-square", true),
                    ("leave-square", "Leave the square", "Get clear before patrols arrive.", string.Empty, true)
                },
                new[]
                {
                    ("bookkeeper-checkpoint", "District_BusinessCore_01", "BusinessCoreStart", "meet-bookkeeper"),
                    ("square-exit-checkpoint", "District_BusinessCore_01", "BusinessSquareExitSpawn", "leave-square")
                },
                "caught-in-square",
                4,
                "Do not linger once the warning is delivered.");
            PopulateMission(
                chapelDebt,
                "mission-chapel-debt",
                "The Chapel Debt",
                "Find the debtor hiding in the old quarter and make sure he understands who now owns his Sunday mornings.",
                "find-debtor",
                300,
                1,
                new[] { "rail-yard" },
                new[] { "activity-chop-delivery" },
                new[]
                {
                    ("find-debtor", "Find the debtor", "Track the debtor through the old quarter.", "push-message", true),
                    ("push-message", "Push the message", "Make the family’s warning clear.", string.Empty, true)
                },
                new[]
                {
                    ("quarter-start-checkpoint", "District_OldQuarter_01", "OldQuarterStart", "find-debtor"),
                    ("quarter-message-checkpoint", "District_OldQuarter_01", "OldQuarterMessageSpawn", "push-message")
                },
                "debtor-escaped",
                3,
                "Do not let the debtor slip away.");
            PopulateMission(
                yardHeat,
                "mission-yard-heat",
                "Yard Heat",
                "Check the Ironline garage, deal with the heat building in the yard, and make it clear the family is taking over transport routes.",
                "check-garage",
                350,
                2,
                new string[0],
                new string[0],
                new[]
                {
                    ("check-garage", "Check the garage", "Reach the Ironline garage in the rail yard.", "clear-watchmen", true),
                    ("clear-watchmen", "Clear the watchmen", "Push through the yard pressure and secure the garage.", string.Empty, true)
                },
                new[]
                {
                    ("yard-start-checkpoint", "District_RailYard_01", "RailYardStart", "check-garage"),
                    ("yard-fight-checkpoint", "District_RailYard_01", "RailYardWatchmenSpawn", "clear-watchmen")
                },
                "yard-overrun",
                2,
                "Do not let the rail yard crew pin you down.");
            PopulateMission(
                belloriBooks,
                "mission-bellori-books",
                "Bellori's Books",
                "Search Nico Bellori's private ledgers and get back out before the books can disappear into the night.",
                "inspect-ledgers",
                420,
                2,
                new string[0],
                new string[0],
                new[]
                {
                    ("inspect-ledgers", "Inspect the ledgers", "Search Bellori's hidden books for the missing names.", "leave-bookstore", true),
                    ("leave-bookstore", "Leave the bookstore", "Get back onto the avenue with the pages intact.", string.Empty, true)
                },
                new[]
                {
                    ("bookshelf-checkpoint", "Interior_BusinessBookkeeper_01", "BusinessInteriorSpawn", "inspect-ledgers"),
                    ("bookstore-exit-checkpoint", "Interior_BusinessBookkeeper_01", "BusinessBookstoreExitSpawn", "leave-bookstore")
                },
                "books-lost",
                3,
                "Do not let the ledger pages get burned or taken.");
            PopulateMission(
                saintVeraSilence,
                "mission-saint-vera-silence",
                "Saint Vera Silence",
                "Search the vestry for the priest's side-book and leave the chapel before the quarter starts whispering.",
                "search-vestry",
                460,
                2,
                new string[0],
                new string[0],
                new[]
                {
                    ("search-vestry", "Search the vestry", "Turn over the vestry and find the side-book.", "leave-chapel", true),
                    ("leave-chapel", "Leave the chapel", "Slip back into the quarter before the bells start a rumor.", string.Empty, true)
                },
                new[]
                {
                    ("vestry-checkpoint", "Interior_OldQuarter_Chapel_01", "ChapelInteriorSpawn", "search-vestry"),
                    ("chapel-exit-checkpoint", "Interior_OldQuarter_Chapel_01", "ChapelExitSpawn", "leave-chapel")
                },
                "vestry-raised-alarm",
                2,
                "Do not make enough noise to bring the block in on you.");
            PopulateMission(
                pierNightWatch,
                "mission-pier-night-watch",
                "Pier Night Watch",
                "Check the late pier crew, confirm who is skimming the crates, and leave the harbor before the watch changes.",
                "check-pier",
                500,
                2,
                new string[0],
                new string[0],
                new[]
                {
                    ("check-pier", "Check the pier", "Search the late pier line for the marked crate.", "leave-harbor", true),
                    ("leave-harbor", "Leave the harbor", "Get clear before the next watch rolls in.", string.Empty, true)
                },
                new[]
                {
                    ("pier-checkpoint", "District_01", "ExteriorReturn", "check-pier"),
                    ("harbor-exit-checkpoint", "District_01", "PierExitSpawn", "leave-harbor")
                },
                "caught-by-watch",
                4,
                "Do not let the night watch close the exits.");
            PopulateMission(
                ironlineLedger,
                "mission-ironline-ledger",
                "Ironline Ledger",
                "Inspect the garage office books and leave the yard before the morning crews realize the numbers moved.",
                "inspect-garage-ledger",
                540,
                2,
                new string[0],
                new string[0],
                new[]
                {
                    ("inspect-garage-ledger", "Inspect the garage ledger", "Find the routing ledger in the Ironline office.", "leave-garage", true),
                    ("leave-garage", "Leave the garage", "Head back into the yard before the foreman returns.", string.Empty, true)
                },
                new[]
                {
                    ("garage-ledger-checkpoint", "Interior_RailYard_Garage_01", "GarageInteriorSpawn", "inspect-garage-ledger"),
                    ("garage-exit-checkpoint", "Interior_RailYard_Garage_01", "GarageExitSpawn", "leave-garage")
                },
                "ledger-burned",
                3,
                "Do not lose the routing ledger.");
            PopulateMission(
                bloodLedger,
                "mission-blood-ledger",
                "Blood Ledger",
                "Read the family ledger in Vincent's office and walk out knowing who is meant to disappear next.",
                "read-ledger",
                620,
                3,
                new string[0],
                new string[0],
                new[]
                {
                    ("read-ledger", "Read the ledger", "Review Vincent's marked ledger before anyone else sees it.", "leave-office", true),
                    ("leave-office", "Leave the office", "Get out before the names in the book become yours too.", string.Empty, true)
                },
                new[]
                {
                    ("blood-ledger-checkpoint", "Interior_BackOffice_01", "LedgerDeskSpawn", "read-ledger"),
                    ("blood-ledger-exit-checkpoint", "Interior_BackOffice_01", "BackOfficeExitSpawn", "leave-office")
                },
                "ledger-read-wrong",
                1,
                "Do not let anyone know you opened the ledger.");
            PopulateMission(
                unionCrackdown,
                "mission-union-crackdown",
                "Union Crackdown",
                "Survey Union Square after dark, mark the weak storefront, and leave before the crackdown turns public.",
                "survey-square",
                680,
                3,
                new string[0],
                new string[0],
                new[]
                {
                    ("survey-square", "Survey the square", "Mark the storefront that folds when the pressure starts.", "leave-downtown", true),
                    ("leave-downtown", "Leave downtown", "Get clear before the square hardens against you.", string.Empty, true)
                },
                new[]
                {
                    ("survey-square-checkpoint", "District_BusinessCore_01", "BusinessCoreStart", "survey-square"),
                    ("downtown-exit-checkpoint", "District_BusinessCore_01", "BusinessDowntownExitSpawn", "leave-downtown")
                },
                "square-locked-down",
                4,
                "Do not get trapped when the square locks down.");
            PopulateMission(
                chapelAsh,
                "mission-chapel-ash",
                "Ashes At Saint Vera",
                "Check the chapel courtyard after the fire and leave with proof before the old quarter shuts its doors.",
                "investigate-courtyard",
                720,
                3,
                new string[0],
                new string[0],
                new[]
                {
                    ("investigate-courtyard", "Investigate the courtyard", "Search the Saint Vera yard for what survived the fire.", "leave-quarter", true),
                    ("leave-quarter", "Leave the quarter", "Disappear before the mourners turn into witnesses.", string.Empty, true)
                },
                new[]
                {
                    ("courtyard-checkpoint", "District_OldQuarter_01", "FromChapelInterior", "investigate-courtyard"),
                    ("quarter-exit-checkpoint", "District_OldQuarter_01", "OldQuarterExitSpawn", "leave-quarter")
                },
                "crowd-turned",
                2,
                "Do not let the courtyard crowd pin you in.");
            PopulateMission(
                yardBetrayal,
                "mission-yard-betrayal",
                "Yard Betrayal",
                "Inspect the midnight yard handoff and leave alive once the betrayal finally shows its face.",
                "inspect-yard",
                780,
                4,
                new string[0],
                new string[0],
                new[]
                {
                    ("inspect-yard", "Inspect the yard handoff", "Reach the yard marker and confirm who sold the route.", "leave-yard", true),
                    ("leave-yard", "Leave the yard", "Get out before the trains and traitors close around you.", string.Empty, true)
                },
                new[]
                {
                    ("betrayal-checkpoint", "District_RailYard_01", "RailYardBetrayalSpawn", "inspect-yard"),
                    ("yard-exit-checkpoint", "District_RailYard_01", "RailYardExitSpawn", "leave-yard")
                },
                "yard-ambush",
                4,
                "Do not let the betrayal turn into a clean execution.");
            PopulateMission(
                lastRun,
                "mission-last-run",
                "Last Run",
                "Take Vincent's final order, step back into the harbor night, and accept that there is no clean road left.",
                "hear-orders",
                900,
                5,
                new string[0],
                new string[0],
                new[]
                {
                    ("hear-orders", "Hear Vincent's orders", "Take the final order in the back office.", "leave-for-harbor", true),
                    ("leave-for-harbor", "Leave for the harbor", "Step out knowing there is no turning back.", string.Empty, true)
                },
                new[]
                {
                    ("final-orders-checkpoint", "Interior_BackOffice_01", "InteriorSpawn", "hear-orders"),
                    ("final-leave-checkpoint", "Interior_BackOffice_01", "BackOfficeFinalExitSpawn", "leave-for-harbor")
                },
                "final-order-missed",
                1,
                "Do not walk away before Vincent finishes.");
            PopulateChapter(
                actOne,
                "chapter-act-1",
                "Act I - First Debts",
                new[] { mission, unionDue, chapelDebt, yardHeat },
                actTwo);
            PopulateChapter(
                actTwo,
                "chapter-act-2",
                "Act II - Open Accounts",
                new[] { belloriBooks, saintVeraSilence, pierNightWatch, ironlineLedger, bloodLedger },
                actThree);
            PopulateChapter(
                actThree,
                "chapter-act-3",
                "Act III - Closed Doors",
                new[] { unionCrackdown, chapelAsh, yardBetrayal, lastRun },
                null);
            PopulateDistrict(docks, "docks", "Harbor Docks", "District_01", 0, true, string.Empty, new[] { "landmark-harbor-office" }, new[] { "activity-dock-courier" });
            PopulateDistrict(business, "business-core", "Business Core", "District_BusinessCore_01", 1, false, "mission-a-quiet-favor", new[] { "landmark-union-square" }, new[] { "activity-ledger-run" });
            PopulateDistrict(oldQuarter, "old-quarter", "Old Quarter", "District_OldQuarter_01", 2, false, "mission-union-due", new[] { "landmark-saint-vera" }, new[] { "activity-lookout-run" });
            PopulateDistrict(railYard, "rail-yard", "Ironline Rail Yard", "District_RailYard_01", 3, false, "mission-chapel-debt", new[] { "landmark-ironline-garage" }, new[] { "activity-chop-delivery" });

            PopulateActivity(dockCourier, "activity-dock-courier", "Dock Courier", 0, "docks", 35, true, string.Empty);
            PopulateActivity(ledgerRun, "activity-ledger-run", "Ledger Run", 0, "business-core", 90, true, "mission-a-quiet-favor");
            PopulateActivity(lookoutRun, "activity-lookout-run", "Church Lookout", 4, "old-quarter", 110, true, "mission-union-due");
            PopulateActivity(chopDelivery, "activity-chop-delivery", "Rail Yard Chop Delivery", 3, "rail-yard", 140, true, "mission-chapel-debt");
            PopulateDialogue(
                lucaBriefing,
                "dialogue-luca-briefing",
                "Luca Briefing",
                "luca-intro",
                new[]
                {
                    ("luca-intro", "Luca Moretti", "Take the Fleetline and keep your head down. Vincent wants the ledger tonight.", "luca-warning"),
                    ("luca-warning", "Luca Moretti", "Nobody opens the book. Not you, not the dockmen, nobody.", "tommy-answer"),
                    ("tommy-answer", "Tommy Bell", "I make the drop and come straight back.", string.Empty)
                });
            PopulateDialogue(
                vincentHandoff,
                "dialogue-vincent-handoff",
                "Vincent Handoff",
                "vincent-intro",
                new[]
                {
                    ("vincent-intro", "Vincent D'Agostino", "Luca says you can drive and keep your mouth shut. Both are rare.", "tommy-response"),
                    ("tommy-response", "Tommy Bell", "I brought the ledger. That's all I was told to do.", "vincent-closing"),
                    ("vincent-closing", "Vincent D'Agostino", "For tonight, that's enough. Next time, you won't be carrying paper.", string.Empty)
                });
            PopulateDialogue(
                bookkeeperWarning,
                "dialogue-bookkeeper-warning",
                "Bookkeeper Warning",
                "bookkeeper-open",
                new[]
                {
                    ("bookkeeper-open", "Nico Bellori", "The union books were supposed to stay between the men who bled for them.", "tommy-warning"),
                    ("tommy-warning", "Tommy Bell", "Those books belong to the family now. Consider this your quiet notice.", "bookkeeper-close"),
                    ("bookkeeper-close", "Nico Bellori", "Then I suppose I just became an expensive friend to have.", string.Empty)
                });
            PopulateDialogue(
                debtorThreat,
                "dialogue-debtor-threat",
                "Debtor Threat",
                "debtor-open",
                new[]
                {
                    ("debtor-open", "Pietro Sava", "I told your men I need one more Sunday. The church can vouch for me.", "tommy-threat"),
                    ("tommy-threat", "Tommy Bell", "The family owns your Sundays now. Next week, you pay in cash or blood.", "debtor-close"),
                    ("debtor-close", "Pietro Sava", "Then I’ll be waiting outside the chapel with an envelope and a prayer.", string.Empty)
                });

            PopulateLandmark(harborOffice, "landmark-harbor-office", "docks", "Harbor Back Office", 0);
            PopulateLandmark(unionSquare, "landmark-union-square", "business-core", "Union Square", 2);
            PopulateLandmark(saintVera, "landmark-saint-vera", "old-quarter", "Saint Vera Chapel", 4);
            PopulateLandmark(ironlineGarage, "landmark-ironline-garage", "rail-yard", "Ironline Garage", 3);

            PopulateVehicle(fleetlineSedan, "fleetline-sedan", "Fleetline Sedan", 18f, 10f, 12f, 88f);
            PopulateVehicle(harborTruck, "harbor-truck", "Harbor Truck", 14f, 8f, 14f, 62f);

            PopulatePortal(backOfficePortal, "portal-back-office", "District_01", "ExteriorReturn", "Interior_BackOffice_01", "InteriorSpawn");
            PopulatePortal(belloriBooksPortal, "portal-bellori-books", "District_BusinessCore_01", "FromBookkeeperInterior", "Interior_BusinessBookkeeper_01", "BusinessInteriorSpawn");
            PopulatePortal(saintVeraPortal, "portal-saint-vera", "District_OldQuarter_01", "FromChapelInterior", "Interior_OldQuarter_Chapel_01", "ChapelInteriorSpawn");
            PopulatePortal(ironlineOfficePortal, "portal-ironline-office", "District_RailYard_01", "FromGarageInterior", "Interior_RailYard_Garage_01", "GarageInteriorSpawn");
            PopulateCampaignDatabase(
                campaign,
                new UnityEngine.Object[] { mission, unionDue, chapelDebt, yardHeat, belloriBooks, saintVeraSilence, pierNightWatch, ironlineLedger, bloodLedger, unionCrackdown, chapelAsh, yardBetrayal, lastRun },
                new UnityEngine.Object[] { actOne, actTwo, actThree },
                new UnityEngine.Object[] { docks, business, oldQuarter, railYard },
                new UnityEngine.Object[] { dockCourier, ledgerRun, lookoutRun, chopDelivery },
                new UnityEngine.Object[] { harborOffice, unionSquare, saintVera, ironlineGarage },
                new UnityEngine.Object[] { fleetlineSedan, harborTruck },
                new UnityEngine.Object[] { backOfficePortal, belloriBooksPortal, saintVeraPortal, ironlineOfficePortal },
                new UnityEngine.Object[] { lucaBriefing, vincentHandoff, bookkeeperWarning, debtorThreat });

            EditorUtility.SetDirty(mission);
            EditorUtility.SetDirty(unionDue);
            EditorUtility.SetDirty(chapelDebt);
            EditorUtility.SetDirty(yardHeat);
            EditorUtility.SetDirty(belloriBooks);
            EditorUtility.SetDirty(saintVeraSilence);
            EditorUtility.SetDirty(pierNightWatch);
            EditorUtility.SetDirty(ironlineLedger);
            EditorUtility.SetDirty(bloodLedger);
            EditorUtility.SetDirty(unionCrackdown);
            EditorUtility.SetDirty(chapelAsh);
            EditorUtility.SetDirty(yardBetrayal);
            EditorUtility.SetDirty(lastRun);
            EditorUtility.SetDirty(actOne);
            EditorUtility.SetDirty(actTwo);
            EditorUtility.SetDirty(actThree);
            EditorUtility.SetDirty(docks);
            EditorUtility.SetDirty(business);
            EditorUtility.SetDirty(oldQuarter);
            EditorUtility.SetDirty(railYard);
            EditorUtility.SetDirty(dockCourier);
            EditorUtility.SetDirty(ledgerRun);
            EditorUtility.SetDirty(lookoutRun);
            EditorUtility.SetDirty(chopDelivery);
            EditorUtility.SetDirty(lucaBriefing);
            EditorUtility.SetDirty(vincentHandoff);
            EditorUtility.SetDirty(bookkeeperWarning);
            EditorUtility.SetDirty(debtorThreat);
            EditorUtility.SetDirty(harborOffice);
            EditorUtility.SetDirty(unionSquare);
            EditorUtility.SetDirty(saintVera);
            EditorUtility.SetDirty(ironlineGarage);
            EditorUtility.SetDirty(fleetlineSedan);
            EditorUtility.SetDirty(harborTruck);
            EditorUtility.SetDirty(backOfficePortal);
            EditorUtility.SetDirty(belloriBooksPortal);
            EditorUtility.SetDirty(saintVeraPortal);
            EditorUtility.SetDirty(ironlineOfficePortal);
            EditorUtility.SetDirty(campaign);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Prototype data assets populated under Assets/Game/Data.");
        }

        private static T EnsureAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void PopulateMission(
            MissionDefinitionAsset mission,
            string missionId,
            string title,
            string summary,
            string startingStageId,
            int cashReward,
            int reputationDelta,
            string[] unlockDistrictIds,
            string[] unlockActivityIds,
            (string Id, string Title, string ObjectiveText, string NextStageId, bool CreatesCheckpoint)[] stagesData,
            (string Id, string SceneName, string SpawnPointId, string MissionStageId)[] checkpointData,
            string failureId,
            int failureType,
            string failureDescription)
        {
            var serializedObject = new SerializedObject(mission);
            serializedObject.FindProperty("missionId").stringValue = missionId;
            serializedObject.FindProperty("title").stringValue = title;
            serializedObject.FindProperty("summary").stringValue = summary;
            serializedObject.FindProperty("startingStageId").stringValue = startingStageId;
            serializedObject.FindProperty("cashReward").intValue = cashReward;
            serializedObject.FindProperty("reputationDelta").intValue = reputationDelta;
            SetStringArray(serializedObject.FindProperty("unlockDistrictIds"), unlockDistrictIds);
            SetStringArray(serializedObject.FindProperty("unlockActivityIds"), unlockActivityIds);

            var stages = serializedObject.FindProperty("stages");
            stages.arraySize = stagesData.Length;
            for (var index = 0; index < stagesData.Length; index += 1)
            {
                SetMissionStage(
                    stages.GetArrayElementAtIndex(index),
                    stagesData[index].Id,
                    stagesData[index].Title,
                    stagesData[index].ObjectiveText,
                    stagesData[index].NextStageId,
                    stagesData[index].CreatesCheckpoint);
            }

            var checkpoints = serializedObject.FindProperty("checkpoints");
            checkpoints.arraySize = checkpointData.Length;
            for (var index = 0; index < checkpointData.Length; index += 1)
            {
                SetMissionCheckpoint(
                    checkpoints.GetArrayElementAtIndex(index),
                    checkpointData[index].Id,
                    checkpointData[index].SceneName,
                    checkpointData[index].SpawnPointId,
                    checkpointData[index].MissionStageId);
            }

            var failureConditions = serializedObject.FindProperty("failureConditions");
            failureConditions.arraySize = 1;
            var failure = failureConditions.GetArrayElementAtIndex(0);
            failure.FindPropertyRelative("Id").stringValue = failureId;
            failure.FindPropertyRelative("Type").enumValueIndex = failureType;
            failure.FindPropertyRelative("Description").stringValue = failureDescription;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateChapter(
            CampaignChapterAsset chapter,
            string chapterId,
            string displayName,
            MissionDefinitionAsset[] missions,
            CampaignChapterAsset nextChapter)
        {
            var serializedObject = new SerializedObject(chapter);
            serializedObject.FindProperty("chapterId").stringValue = chapterId;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            SetObjectArray(serializedObject.FindProperty("missions"), missions);
            serializedObject.FindProperty("nextChapter").objectReferenceValue = nextChapter;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateDistrict(
            DistrictDefinitionAsset district,
            string districtId,
            string displayName,
            string sceneName,
            int theme,
            bool unlockedByDefault,
            string unlockMissionId,
            string[] landmarkIds,
            string[] activityIds)
        {
            var serializedObject = new SerializedObject(district);
            serializedObject.FindProperty("districtId").stringValue = districtId;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("sceneName").stringValue = sceneName;
            serializedObject.FindProperty("theme").enumValueIndex = theme;
            serializedObject.FindProperty("unlockedByDefault").boolValue = unlockedByDefault;
            serializedObject.FindProperty("unlockMissionId").stringValue = unlockMissionId;
            SetStringArray(serializedObject.FindProperty("landmarkIds"), landmarkIds);
            SetStringArray(serializedObject.FindProperty("activityIds"), activityIds);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateActivity(
            ActivityDefinitionAsset activity,
            string activityId,
            string displayName,
            int activityType,
            string districtId,
            int rewardCash,
            bool repeatable,
            string unlockMissionId)
        {
            var serializedObject = new SerializedObject(activity);
            serializedObject.FindProperty("activityId").stringValue = activityId;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("activityType").enumValueIndex = activityType;
            serializedObject.FindProperty("districtId").stringValue = districtId;
            serializedObject.FindProperty("rewardCash").intValue = rewardCash;
            serializedObject.FindProperty("repeatable").boolValue = repeatable;
            serializedObject.FindProperty("unlockMissionId").stringValue = unlockMissionId;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateLandmark(
            LandmarkDefinitionAsset landmark,
            string landmarkId,
            string districtId,
            string displayName,
            int landmarkType)
        {
            var serializedObject = new SerializedObject(landmark);
            serializedObject.FindProperty("landmarkId").stringValue = landmarkId;
            serializedObject.FindProperty("districtId").stringValue = districtId;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("landmarkType").enumValueIndex = landmarkType;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateVehicle(
            VehicleArchetypeAsset vehicle,
            string archetypeId,
            string displayName,
            float topSpeed,
            float acceleration,
            float braking,
            float handling)
        {
            var serializedObject = new SerializedObject(vehicle);
            serializedObject.FindProperty("archetypeId").stringValue = archetypeId;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("topSpeed").floatValue = topSpeed;
            serializedObject.FindProperty("acceleration").floatValue = acceleration;
            serializedObject.FindProperty("braking").floatValue = braking;
            serializedObject.FindProperty("handling").floatValue = handling;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulatePortal(
            InteriorPortalAsset portal,
            string portalId,
            string exteriorSceneName,
            string exteriorSpawnPointId,
            string interiorSceneName,
            string interiorSpawnPointId)
        {
            var serializedObject = new SerializedObject(portal);
            serializedObject.FindProperty("portalId").stringValue = portalId;
            serializedObject.FindProperty("exteriorSceneName").stringValue = exteriorSceneName;
            serializedObject.FindProperty("exteriorSpawnPointId").stringValue = exteriorSpawnPointId;
            serializedObject.FindProperty("interiorSceneName").stringValue = interiorSceneName;
            serializedObject.FindProperty("interiorSpawnPointId").stringValue = interiorSpawnPointId;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateCampaignDatabase(
            CampaignDatabaseAsset campaign,
            UnityEngine.Object[] missions,
            UnityEngine.Object[] chapters,
            UnityEngine.Object[] districts,
            UnityEngine.Object[] activities,
            UnityEngine.Object[] landmarks,
            UnityEngine.Object[] vehicles,
            UnityEngine.Object[] interiorPortals,
            UnityEngine.Object[] dialogues)
        {
            var serializedObject = new SerializedObject(campaign);
            SetObjectArray(serializedObject.FindProperty("missions"), missions);
            SetObjectArray(serializedObject.FindProperty("chapters"), chapters);
            SetObjectArray(serializedObject.FindProperty("districts"), districts);
            SetObjectArray(serializedObject.FindProperty("activities"), activities);
            SetObjectArray(serializedObject.FindProperty("landmarks"), landmarks);
            SetObjectArray(serializedObject.FindProperty("vehicles"), vehicles);
            SetObjectArray(serializedObject.FindProperty("interiorPortals"), interiorPortals);
            SetObjectArray(serializedObject.FindProperty("dialogues"), dialogues);
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void PopulateDialogue(
            DialogueSequenceAsset dialogue,
            string dialogueId,
            string displayName,
            string startingNodeId,
            (string Id, string SpeakerName, string Text, string NextNodeId)[] nodes)
        {
            var serializedObject = new SerializedObject(dialogue);
            serializedObject.FindProperty("dialogueId").stringValue = dialogueId;
            serializedObject.FindProperty("displayName").stringValue = displayName;
            serializedObject.FindProperty("startingNodeId").stringValue = startingNodeId;

            var nodeArray = serializedObject.FindProperty("nodes");
            nodeArray.arraySize = nodes.Length;
            for (var index = 0; index < nodes.Length; index += 1)
            {
                var node = nodeArray.GetArrayElementAtIndex(index);
                node.FindPropertyRelative("Id").stringValue = nodes[index].Id;
                node.FindPropertyRelative("SpeakerName").stringValue = nodes[index].SpeakerName;
                node.FindPropertyRelative("Text").stringValue = nodes[index].Text;
                node.FindPropertyRelative("NextNodeId").stringValue = nodes[index].NextNodeId;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetMissionStage(
            SerializedProperty element,
            string id,
            string title,
            string objectiveText,
            string nextStageId,
            bool createsCheckpoint)
        {
            element.FindPropertyRelative("Id").stringValue = id;
            element.FindPropertyRelative("Title").stringValue = title;
            element.FindPropertyRelative("ObjectiveText").stringValue = objectiveText;
            element.FindPropertyRelative("NextStageId").stringValue = nextStageId;
            element.FindPropertyRelative("CreatesCheckpoint").boolValue = createsCheckpoint;
        }

        private static void SetMissionCheckpoint(
            SerializedProperty element,
            string id,
            string sceneName,
            string spawnPointId,
            string missionStageId)
        {
            element.FindPropertyRelative("Id").stringValue = id;
            element.FindPropertyRelative("SceneName").stringValue = sceneName;
            element.FindPropertyRelative("SpawnPointId").stringValue = spawnPointId;
            element.FindPropertyRelative("MissionStageId").stringValue = missionStageId;
        }

        private static void SetStringArray(SerializedProperty property, string[] values)
        {
            property.arraySize = values.Length;
            for (var index = 0; index < values.Length; index += 1)
            {
                property.GetArrayElementAtIndex(index).stringValue = values[index];
            }
        }

        private static void SetObjectArray(SerializedProperty property, UnityEngine.Object[] values)
        {
            property.arraySize = values.Length;
            for (var index = 0; index < values.Length; index += 1)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = values[index];
            }
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
    }
}
