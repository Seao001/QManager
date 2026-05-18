using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using QManager.Models;

namespace QManager.Service
{
    // Serviciu responsabil pentru scrierea și citirea stării de pe disc
    public class DashboardStateService
    {
        private static readonly string StateFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "QManager", "dashboard_state.json");

        // Serializează lista de tichete și o scrie în folderul AppData
        public void SaveCurrentState(IEnumerable<TicketStateDto> tickets)
        {
            try
            {
                var state = new DashboardState { Tickets = new List<TicketStateDto>(tickets) };
                string? directory = Path.GetDirectoryName(StateFilePath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) 
                    Directory.CreateDirectory(directory);

                string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StateFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la salvarea stării dashboard: {ex.Message}");
            }
        }

        // Citește fișierul JSON și deserializează datele în obiectul DashboardState
        public DashboardState? LoadLastState()
        {
            if (!File.Exists(StateFilePath)) return null;

            try
            {
                string json = File.ReadAllText(StateFilePath);
                return JsonSerializer.Deserialize<DashboardState>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la încărcarea stării dashboard: {ex.Message}");
                return null;
            }
        }
    }
}