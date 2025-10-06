using System.Collections.Generic;

namespace SpawnMonsterCommand_Bootstrap
{
    /// <summary>
    /// The contract use to relay a game command to a version
    /// </summary>
    public interface IGameCommand
    {
        string Execute(string[] tokens);
        List<string> FetchAutocompleteOptions(string command, string[] tokens);
        string Help(string command, bool verbose);
        bool IsAvailable();
        bool ShowInHelpAndAutocomplete();
    }
}