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
using static MGSC.ConsoleDaemon;

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


            InjectCommand(typeof(SpawnMonsterCommand), SpawnMonsterCommand.CommandName);
            ExportCreatureList(ConfigDirectories.ModPersistenceFolder);
        }

        public static void InjectCommand(Type commandType, string name)
        {

            ConsoleDaemon consoleDaemon = ConsoleDaemon.Instance;

            if (consoleDaemon == null) return;


            MethodInfo helpMethod = commandType.GetMethod("Help", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[2]
            {
                typeof(string),
                typeof(bool)
            }, null);

            MethodInfo fetchAutocompleteMethod = commandType.GetMethod("FetchAutocompleteOptions", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[2]
{
                typeof(string),
                typeof(string[])
}, null);

            if (helpMethod == null && fetchAutocompleteMethod == null) return;

            MethodInfo isAvailableMethod = commandType.GetMethod("IsAvailable", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[0], null);
            MethodInfo showInHelpAndAutocompleteMethod = commandType.GetMethod("ShowInHelpAndAutocomplete", BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Standard, new Type[0], null);



            consoleDaemon.Commands[name.ToLower()] = new CommandInterface(commandType, consoleDaemon.Resolve, helpMethod, fetchAutocompleteMethod,
                isAvailableMethod, showInHelpAndAutocompleteMethod);

            //Seems to always set this, even without alts.
            //Oddly does not do a ToLower
            consoleDaemon.AlternateCommandNames[name] = name;
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
