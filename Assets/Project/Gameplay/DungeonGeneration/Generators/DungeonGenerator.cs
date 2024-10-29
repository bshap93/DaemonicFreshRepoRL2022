using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Gameplay.DungeonGeneration.Generators;
using UnityEngine;
using RoomTemplate = Project.Gameplay.DungeonGeneration.Rooms.Models.RoomTemplate;

public class NewDungeonGenerator : MonoBehaviour
{
    [SerializeField] private List<RoomTemplate> roomTemplates;
    [SerializeField] private int dungeonSize;
    
    public async Task<DungeonData> GenerateDungeon(int seed)
    {
        Random.InitState(seed);
        
        var dungeonData = new DungeonData {
            Seed = seed,
            Rooms = new List<RoomData>()
        };
        
        // Generate layout
        // Place rooms
        // Add corridors
        // Populate enemies/items
        
        return dungeonData;
    }
}