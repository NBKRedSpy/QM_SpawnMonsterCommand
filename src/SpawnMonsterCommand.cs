using MGSC;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace QM_SpawnMonsterCommand
{
    //[ConsoleCommand(new string[] { "exit-no-save" })]
    public class SpawnMonsterCommand
    {
        public static string CommandName { get; set; } = "spawn-monster-under-cursor";

        public static string Help(string command, bool verbose)
        {
            return $"Creates a monster under the cursor.  Usage: {CommandName} <monster_id>";
        }

        public string Execute(string[] tokens)
        {
            if (tokens.Length != 1 || string.IsNullOrWhiteSpace(tokens[0]))
            {
                return "Requires the monster id to be set.";
            }

            string creatureId = tokens[0].Trim(' ', '\t');

            Creatures creatures = DungeonGameMode.Instance.Creatures;
            DungeonGameMode dungeonGameMode = SingletonMonoBehaviour<DungeonGameMode>.Instance;

            //-- Get and validate cursor location.
            CellPosition cellUnderCursor = dungeonGameMode.Get<MapRenderer>().GetCellUnderCursor();
            MapCell cell = SingletonMonoBehaviour<DungeonGameMode>.Instance.Get<MapGrid>().GetCell(cellUnderCursor);

            if (!IsValidCell(creatures, cell))
            {
                return $"Cell {cell.X}, {cell.Y} is not a valid location";
            }

            //--Find creature to validate
            CreatureRecord record = Data.Creatures.GetRecord(creatureId);
            if (record == null) return $"creature id not found {creatureId}";

            //--Spawn
            TurnController turnController = dungeonGameMode._state.Get<TurnController>();
            if (!CreatureSystem.SpawnMonster(creatures, turnController, creatureId, cellUnderCursor))
            {
                return "Spawn Monster failed";
            }

            return "done!";
        }

        /// <summary>
        /// Full Copy from local function in the SpawnSystem.SpawnMonsters function.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static bool IsValidCell(Creatures creatures, MapCell cell)
        {
            if (cell.Type == MapCellType.Floor && !cell.isObjBlockPass && creatures.GetCreature(cell.X, cell.Y) == null)
            {

                //Original:
                //  return CellPosition.Distance(creatures.Player.pos, new CellPosition(cell.X, cell.Y)) > minRadius;
                CellPosition.Distance(creatures.Player.pos, new CellPosition(cell.X, cell.Y));
                return true;
            }
            return false;
        }


        public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
        {
            if (tokens.Length != 1 || string.IsNullOrWhiteSpace(tokens[0])) return new List<string>();

            string creatureId = tokens[0].Trim();
            return FindSimilarCreatures(command, creatureId);
        }

        /// <summary>
        /// Returns a list of creatures that partially match the creature id.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="partialCreatureId">set to blank to return all items</param>
        /// <returns></returns>
        public static List<string> FindSimilarCreatures(string command, string partialCreatureId)
        {

            List<string> creatures;

            if (partialCreatureId == "_")
            {
                creatures = Data.Creatures.Records
                    .Select(x => command + " " + x.Id)
                    .ToList();
            }
            else
            {
                creatures = Data.Creatures.Records
                   .Where(x => x.Id.Contains(partialCreatureId))
                   .Select(x => command + " " + x.Id)
                   .ToList();
            }

            return creatures.Count > 0 ? creatures : null;
        }

        public static bool IsAvailable()
        {
            return DungeonGameMode.Instance != null;
        }

        public static bool ShowInHelpAndAutocomplete()
        {
            return true;
        }
    }
}