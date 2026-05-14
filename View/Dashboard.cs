using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
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

        public new event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<NavigationRequestEventArgs>? NavigationRequested;

        public ObservableCollection<TicketViewModel> OnHoldTickets => _ticketState.OnHoldTickets;
        public ObservableCollection<TicketViewModel> ReadyTickets => _ticketState.ReadyTickets;

        public string WaitingNowText => _ticketState.WaitingNowText;



        public DashboardView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            _ticketState.PropertyChanged += OnTicketStateChanged;
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
            OnHoldTickets.Clear();
            ReadyTickets.Clear();
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
                SuggestedFileName = $"raport-cozi-{DateTime.Now:yyyy-MM-dd-HHmm}.pdf",
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
                        page.Header().Text("Raport Gestiune Cozi").SemiBold().FontSize(22).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                        page.Content().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text("Status").SemiBold();
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text("Talon").SemiBold();
                                header.Cell().BorderBottom(1).PaddingVertical(5).Text("Birou").SemiBold();
                            });

                            foreach (var ticket in OnHoldTickets)
                            {
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text("În așteptare");
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Talon);
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Room);
                            }

                            foreach (var ticket in ReadyTickets)
                            {
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text("Pregătit");
                                table.Cell().BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5).Text(ticket.Talon);
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
