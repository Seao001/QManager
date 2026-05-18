using System;
using System.IO;
using System.Text;
using QManager.Models;

namespace QManager.Service
{
    // Serviciu pentru exportarea detaliilor tichetului într-un fișier text (bon)
    public class TicketExportService
    {
        private const int ReceiptWidth = 40; // Lățimea bonului pentru centrare vizuală

        // Metodă pentru a exporta detaliile unui tichet într-un fișier .txt
        public void ExportTicketToTxt(Ticket ticket)
        {
            try
            {
                // 1. Definirea căii de salvare: Desktop/Bonuri_Emise
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Bonuri_Emise");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // 2. Generarea unui nume unic pentru fișier: Bon_M11_20260518_0952.txt
                string fileName = $"Bon_{ticket.TicketLabel}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(folderPath, fileName);

                // 3. Construirea conținutului bonului
                StringBuilder sb = new StringBuilder();

                // Antet
                sb.AppendLine(CenterText("========================================"));
                sb.AppendLine(CenterText("QMANAGER"));
                sb.AppendLine(CenterText("========================================"));
                sb.AppendLine();

                // Detalii bancă și serviciu (localizate)
                sb.AppendLine($"{LocalizationService.Instance["ReceiptBankLabel"]} {ticket.Bank}");
                sb.AppendLine($"{LocalizationService.Instance["ReceiptServiceLabel"]} {ticket.ServiceDescription}");
                sb.AppendLine(CenterText("----------------------------------------"));
                sb.AppendLine();

                // Număr talon (localizat și centrat)
                sb.AppendLine(CenterText($"{LocalizationService.Instance["ReceiptTalonLabel"]} {ticket.TicketLabel}"));
                sb.AppendLine();
                sb.AppendLine(CenterText("----------------------------------------"));
                sb.AppendLine();

                // Data și ora curente (localizate)
                sb.AppendLine($"{LocalizationService.Instance["ReceiptDateLabel"]} {DateTime.Now:dd.MM.yyyy}");
                sb.AppendLine($"{LocalizationService.Instance["ReceiptTimeLabel"]}  {DateTime.Now:HH:mm}");
                sb.AppendLine();

                // Mesaj subsol (localizat)
                sb.AppendLine(CenterText(LocalizationService.Instance["ReceiptFooterLine1"]));
                sb.AppendLine(CenterText(LocalizationService.Instance["ReceiptFooterLine2"]));
                sb.AppendLine(CenterText("========================================"));

                // 4. Salvarea fișierului
                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Console.WriteLine($"Bon generat cu succes: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la generarea bonului TXT: {ex.Message}");
            }
        }

        // Metodă helper pentru a centra textul într-o lățime fixă
        private string CenterText(string text)
        {
            if (string.IsNullOrEmpty(text)) return new string(' ', ReceiptWidth);
            if (text.Length >= ReceiptWidth) return text;

            int padding = (ReceiptWidth - text.Length) / 2;
            return new string(' ', padding) + text + new string(' ', ReceiptWidth - text.Length - padding);
        }
    }
}