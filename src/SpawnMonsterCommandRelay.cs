using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpawnMonsterCommand_Bootstrap
{
    /// <summary>
    /// Relays the SpawnMonsterCommand to the beta or stable version via reflection.
    /// Required since the game uses static functions via convention.
    /// </summary>
    [ConsoleCommand(new string[] { CommandName, "smuc" })]
    public class SpawnMonsterCommandRelay 
    {
        public const string CommandName = "spawn-monster-under-cursor";

        private static IGameCommand Command;

        public static void Init(Assembly assembly, string typeName)
        {
            Type type = assembly.GetType(typeName, true);

            Command = Activator.CreateInstance(type) as IGameCommand;

            if (Command == null)
            {
                throw new Exception($"Failed to create instance of type {typeName} as IGameCommand");
            }   
        }

        public static string Help(string command, bool verbose) => Command.Help(command, verbose);

        public string Execute(string[] tokens) => Command.Execute(tokens);

        public static List<string> FetchAutocompleteOptions(string command, string[] tokens) => 
            Command.FetchAutocompleteOptions(command, tokens);

        public static bool IsAvailable() => Command.IsAvailable();

        public static bool ShowInHelpAndAutocomplete() => Command.ShowInHelpAndAutocomplete();
    }
}
