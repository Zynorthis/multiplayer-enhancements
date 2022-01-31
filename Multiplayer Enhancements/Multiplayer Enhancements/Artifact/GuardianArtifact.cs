using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking;
using static Multiplayer_Enhancements.Main;
using BepInEx.Logging;

namespace Multiplayer_Enhancements.Artifact
{
    public class GuardianArtifact: ArtifactBase<GuardianArtifact>
    {
        private static ConfigEntry<int> TimesToPrintMessageOnStart;
        private List<CharacterBody> PlayerDeathList; //TODO: Change this from type string to whatever type is used to represent a player.
        private ManualLogSource Logger;

        public override string ArtifactName => "Artifact of Guardians";

        public override string ArtifactLangTokenName => "ARTIFACT_OF_GUARDIANS";

        public override string ArtifactDescription => "When enabled, players gain a temporary buff to armour if they died on the previous stage.";

        public override Sprite ArtifactEnabledIcon => RoR2Content.Artifacts.Swarms.smallIconSelectedSprite;

        public override Sprite ArtifactDisabledIcon => RoR2Content.Artifacts.Swarms.smallIconDeselectedSprite;
        public override void Init(ConfigFile config, ManualLogSource logger)
        {
            Logger = logger;
            PlayerDeathList = new List<CharacterBody>();
            CreateConfig(config);
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        private void CreateConfig(ConfigFile config)
        {
            TimesToPrintMessageOnStart = config.Bind<int>("Artifact: " + ArtifactName, "Times to Print Message in Chat", 1, "How many times should a message be printed to the chat on run start?");
        }

        public override void Hooks()
        {
            Logger.LogInfo($"Running Hooks Method");
            Run.onRunStartGlobal += PrintMessageToChat;

            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += (orig, self, damageReport, victimNetworkUser) =>
            {
                AddPlayerToDeathList(damageReport.victimBody);
                orig(self, damageReport, victimNetworkUser);
            };

            On.RoR2.Run.AdvanceStage += (orig, self, nextScene) =>
            {
                GivePlayersBuffs();
                orig(self, nextScene);
            };
        }

        private void PrintMessageToChat(Run run)
        {
            if (NetworkServer.active && ArtifactEnabled)
            {
                Chat.AddMessage("Artifact of Guardians has been Enabled.");
            }
        }

        public void AddPlayerToDeathList(CharacterBody player)
        {
            Logger.LogInfo($"Checking Player...\n---------\n{player}\n{player.isPlayerControlled}\n{player.name}\n---------");
            if (player == null) return;
            if (!player.isPlayerControlled) return;

            PlayerDeathList.Add(player);
        }

        public void GivePlayersBuffs()
        {
            Logger.LogInfo($"Giving Buffs To Players. Current Player Count is: {PlayerDeathList.Count}");
            foreach (var player in PlayerDeathList)
            {
                player.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, 30000);
            }
            PlayerDeathList.Clear();
            Logger.LogInfo("Buffs Given, Player Death List Cleared.");
        }
    }
}
