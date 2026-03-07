using MGSC;
using SpawnMonsterCommand_Bootstrap;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SpawnMonsterCommand
{
    public class SpawnMonsterCommand : IGameCommand
    {
        public const string CommandName = "spawn-monster-under-cursor";

        public string Help(string command, bool verbose)
        {
            return $"Creates a monster under the cursor.  Usage: {CommandName} <monster_id>";
        }

        public string Execute(string[] tokens)
        {
            if (tokens.Length != 1 || string.IsNullOrWhiteSpace(tokens[0]))
            {
                return "Requires the mob class id to be set.";
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
            MobClassRecord record = Data.MobClasses.GetRecord(creatureId);
            if (record == null) return $"mobclass id not found {creatureId}";


            //--Spawn
            State state = dungeonGameMode._state;

            Difficulty difficulty = state.Get<Difficulty>();

            TurnController turnController = state.Get<TurnController>();

            if (!CreatureSystem.SpawnMonsterFromMobClass(state.Get<Mercenaries>(), state.Get<PerkFactory>(), state.Get<Difficulty>(),
                creatures, state.Get<RaidMetadata>(), turnController, creatureId, new CellPosition(cell.X, cell.Y)))
            {
                return "Spawn Monster failed";
            }

            return "done!";
        }

        /// <summary>
        /// Full Copy from the local function inside the SpawnSystem.SpawnMonsters function.
        /// There are a couple of local functions with the same name.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool IsValidCell(Creatures creatures, MapCell cell)
        {

            return (cell.ReachableCellFlag && cell.Type == MapCellType.Floor && !cell.IsObjBlockPass && cell.specialFlag == MapCellSpecialFlag.None &&
                creatures.GetCreature(cell.X, cell.Y) == null);

        }


        public List<string> FetchAutocompleteOptions(string command, string[] tokens)
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
        public List<string> FindSimilarCreatures(string command, string partialCreatureId)
        {

            List<string> creatures;

            if (partialCreatureId == "_")
            {
                creatures = Data.MobClasses.Records
                    .Select(x => command + " " + x.Id)
                    .ToList();
            }
            else
            {
                creatures = Data.MobClasses.Records
                   .Where(x => x.Id.Contains(partialCreatureId))
                   .Select(x => command + " " + x.Id)
                   .ToList();
            }

            return creatures.Count > 0 ? creatures : null;
        }

        public bool IsAvailable()
        {
            return DungeonGameMode.Instance != null;
        }

        public bool ShowInHelpAndAutocomplete()
        {
            return true;
        }
    }
}