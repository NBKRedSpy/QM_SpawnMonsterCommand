using MGSC;
using QM_MissionExpirationHighlight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QM_SpawnMonsterCommand
{
    public static class Plugin
    {
        public static ConfigDirectories ConfigDirectories = new ConfigDirectories();

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            Directory.CreateDirectory(ConfigDirectories.ModPersistenceFolder);

            try
            {
                //Legacy clean up from when the mod was in the game's appdata folder.

                //Delete the old folder. It only contains the data, which will be recreated.
                string legacyFolder = Path.Combine(Application.persistentDataPath, ConfigDirectories.ModAssemblyName);
                if (Directory.Exists(legacyFolder))
                {
                    Directory.Delete(legacyFolder, true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error deleting the legacy file");
                Debug.LogException(ex);
            }

            ExportCreatureList(ConfigDirectories.ModPersistenceFolder);
        }

        public static void ExportCreatureList(string outputFolder)
        {

            StringBuilder sb = new StringBuilder();

            
            Data.MobClasses.Ids.ToList().ForEach(x => sb.AppendLine(x));

            string exportText = sb.ToString();

            string filePath = Path.Combine(outputFolder, "MobClasses.txt");

            if(File.Exists(filePath))
            {
                string existingText = File.ReadAllText(filePath);
                if (existingText == exportText) return;
            }

            File.WriteAllText(filePath, exportText);
        }


    }
}
