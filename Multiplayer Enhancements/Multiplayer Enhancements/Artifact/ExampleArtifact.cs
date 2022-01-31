using BepInEx.Configuration;
using BepInEx.Logging;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static Multiplayer_Enhancements.Main;

namespace Multiplayer_Enhancements.Artifact
{
    class ExampleArtifact : ArtifactBase<ExampleArtifact>
    {
        public static ConfigEntry<int> TimesToPrintMessageOnStart;
        private ManualLogSource Logger;

        public override string ArtifactName => "Artifact of Example";

        public override string ArtifactLangTokenName => "ARTIFACT_OF_EXAMPLE";

        public override string ArtifactDescription => "When enabled, print a message to the chat at the start of the run.";

        public override Sprite ArtifactEnabledIcon => RoR2Content.Artifacts.Swarms.smallIconSelectedSprite;

        public override Sprite ArtifactDisabledIcon => RoR2Content.Artifacts.Swarms.smallIconDeselectedSprite;

        public override void Init(ConfigFile config, ManualLogSource logger)
        {
            Logger = logger;
            CreateConfig(config);
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        private void CreateConfig(ConfigFile config)
        {
            Logger.LogInfo($"{nameof(ExampleArtifact)} - Running CreateConfig.");
            TimesToPrintMessageOnStart = config.Bind<int>("Artifact: " + ArtifactName, "Times to Print Message in Chat", 5, "How many times should a message be printed to the chat on run start?");
        }

        public override void Hooks()
        {
            Run.onRunStartGlobal += PrintMessageToChat;
        }

        private void PrintMessageToChat(Run run)
        {
            Logger.LogInfo($"{nameof(ExampleArtifact)} - Printing Chat Messages...");
            if (NetworkServer.active && ArtifactEnabled)
            {
                for (int i = 0; i < TimesToPrintMessageOnStart.Value; i++)
                {
                    Chat.AddMessage("Example Artifact has been Enabled.");
                }
            }
        }
    }
}
