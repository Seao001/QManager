using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QManager.Service;

namespace QManager.View
{
    public partial class DashboardView : UserControl, INotifyPropertyChanged
    {
        private readonly TicketState _ticketState = TicketState.Instance;

        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;
        public new event PropertyChangedEventHandler? PropertyChanged;

        private string _selectedBank = "Toate";
        public List<string> Banks { get; } = new() { "Toate", "Maib", "VictoriaBank", "BNM", "MICB" };

        public string SelectedBank
        {
            get => _selectedBank;
            set
            {
                if (_selectedBank != value)
                {
                    _selectedBank = value;
                    OnPropertyChanged();
                    FilterTickets();
                }
            }
        }

        public ObservableCollection<TicketViewModel> OnHoldTickets { get; } = new();
        public ObservableCollection<TicketViewModel> ReadyTickets { get; } = new();

        public string WaitingNowText => _ticketState.WaitingNowText;

        public DashboardView()
        {
            AvaloniaXamlLoader.Load(this);
            
            _ticketState.OnHoldTickets.CollectionChanged += (s, e) => FilterTickets();
            _ticketState.ReadyTickets.CollectionChanged += (s, e) => FilterTickets();
            _ticketState.PropertyChanged += OnTicketStateChanged;
            
            FilterTickets();
            DataContext = this;
        }

        private void FilterTickets()
        {
            OnHoldTickets.Clear();
            ReadyTickets.Clear();

            var filter = SelectedBank == "Toate" ? string.Empty : SelectedBank;

            foreach (var ticket in _ticketState.OnHoldTickets)
            {
                if (string.IsNullOrEmpty(filter) || ticket.Bank == filter)
                    OnHoldTickets.Add(ticket);
            }

            foreach (var ticket in _ticketState.ReadyTickets)
            {
                if (string.IsNullOrEmpty(filter) || ticket.Bank == filter)
                    ReadyTickets.Add(ticket);
            }
            OnPropertyChanged(nameof(WaitingNowText));
        }

        private void OnTicketStateChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TicketState.WaitingNowText))
            {
                OnPropertyChanged(nameof(WaitingNowText));
            }
        }

        private void MoveToReady_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: TicketViewModel ticket })
            {
                return;
            }

            _ticketState.MoveToReady(ticket);
        }

        private void MoveToOnHold_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: TicketViewModel ticket })
            {
                return;
            }

            _ticketState.MoveToOnHold(ticket);
        }

        private void ResetQueue_Click(object? sender, RoutedEventArgs e)
        {
            _ticketState.ClearAll();
        }

        private void NavigateToDisplay_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string target })
            {
                NavigationRequested?.Invoke(this, new NavigationRequestEventArgs(target));
            }
        }

        private async void ExportData_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel is null)
            {
                return;
            }

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Exportă Raport PDF",
                SuggestedFileName = $"{LocalizationService.Instance["ReportFileName"]}-{DateTime.Now:yyyy-MM-dd-HHmm}.pdf",
                DefaultExtension = "pdf",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Document PDF")
                    {
                        Patterns = new[] { "*.pdf" },
                        MimeTypes = new[] { "application/pdf" }
                    }
                }
            });

            if (file is null)
            {
                return;
            }

            try
            {
                Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Header().Text(LocalizationService.Instance["ReportTitle"]).SemiBold().FontSize(22).FontColor(QuestPDF.Helpers.Colors.Blue.Medium); // Folosim cheia localizată

                        page.Content().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(); // Coloană nouă pentru descrierea serviciului
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text(LocalizationService.Instance["Status"]).SemiBold();
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text(LocalizationService.Instance["Bank"]).SemiBold();
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text(LocalizationService.Instance["Ticket"]).SemiBold();
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text(LocalizationService.Instance["ReceiptServiceLabel"]).SemiBold(); // Antet pentru serviciu
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text(LocalizationService.Instance["Room"]).SemiBold();
                            });

                            foreach (var ticket in OnHoldTickets)
                            {
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(LocalizationService.Instance["InWaiting"]);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Bank);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Talon);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.ServiceDescription); // Afișăm descrierea serviciului
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Room);
                            }

                            foreach (var ticket in ReadyTickets)
                            {
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(LocalizationService.Instance["Ready"]);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Bank);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Talon);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.ServiceDescription); // Afișăm descrierea serviciului
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Room);
                            }
                        });
                    });
                });

                await using var stream = await file.OpenWriteAsync();
                document.GeneratePdf(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la exportul PDF: {ex.Message}");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
