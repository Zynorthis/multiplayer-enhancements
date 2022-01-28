using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using R2API.Networking;
using static Multiplayer_Enhancements.Main;

namespace Multiplayer_Enhancements.Artifact
{
    public abstract class GuardianArtifact: ArtifactBase<GuardianArtifact>
    {
        public static ConfigEntry<int> TimesToPrintMessageOnStart;
        public PlayersTracker players;
        public List<CharacterBody> PlayerDeathList; //TODO: Change this from type string to whatever type is used to represent a player.

        public override string ArtifactName => "Artifact of Guardians";

        public override string ArtifactLangTokenName => "ARTIFACT_OF_GUARDIANS";

        public override string ArtifactDescription => "When enabled, players gain a temporary buff to armour if they died on the previous stage.";

        public override Sprite ArtifactEnabledIcon => MainAssets.LoadAsset<Sprite>("ExampleArtifactEnabledIcon.png");

        public override Sprite ArtifactDisabledIcon => MainAssets.LoadAsset<Sprite>("ExampleArtifactDisabledIcon.png");
        public override void Init(ConfigFile config)
        {
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
            if (player == null) return;
            if (!player.isPlayerControlled) return;

            PlayerDeathList.Add(player);
        }

        public void GivePlayersBuffs()
        {
            foreach (var player in PlayerDeathList)
            {
                player.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, 30000);
            }
            PlayerDeathList.Clear();
        }
    }
}
