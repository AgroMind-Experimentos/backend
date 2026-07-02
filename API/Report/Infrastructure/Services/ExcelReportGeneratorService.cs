namespace EcotrackPlatform.API.Report.Infrastructure.Services;

using ClosedXML.Excel;
using System.Text.Json;
using System.IO;

public class ExcelReportGeneratorService
{
    public byte[] GenerateTasksReportExcel(object reportDataObj)
    {
        var jsonString = JsonSerializer.Serialize(reportDataObj);
        using var document = JsonDocument.Parse(jsonString);
        var root = document.RootElement;

        var summary = root.GetProperty("Summary");
        var totalTasks = summary.GetProperty("TotalTasks").GetInt32();
        var completedTasks = summary.GetProperty("CompletedTasks").GetInt32();
        var inProgressTasks = summary.GetProperty("InProgressTasks").GetInt32();
        var pendingTasks = summary.GetProperty("PendingTasks").GetInt32();
        var completionRate = summary.GetProperty("CompletionRate").GetDouble();

        var tasksArray = root.GetProperty("Tasks").EnumerateArray().ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Tareas");

        // --- HEADER ---
        var headerRange = worksheet.Range("A1:E2");
        headerRange.Merge().Style
            .Font.SetBold()
            .Font.SetFontSize(16)
            .Font.SetFontColor(XLColor.FromHtml("#0D47A1")) 
            .Fill.SetBackgroundColor(XLColor.FromHtml("#90CAF9"));
        worksheet.Cell("A1").Value = "AGROMIND ECOTRACK - Reporte de Estado de Tareas";
        worksheet.Cell("A1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

        worksheet.Cell("A3").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
        worksheet.Cell("A3").Style.Font.SetItalic().Font.SetFontSize(9).Font.SetFontColor(XLColor.Gray);

        worksheet.Cell("A5").Value = "RESUMEN EJECUTIVO";
        worksheet.Cell("A5").Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.FromHtml("#0D47A1"));

        string[] titles = { "Total", "Completadas", "En Progreso", "Pendientes", "Tasa" };
        string[] values = { totalTasks.ToString(), completedTasks.ToString(), inProgressTasks.ToString(), pendingTasks.ToString(), $"{completionRate}%" };
        string[] bgColors = { "#E3F2FD", "#E8F5E9", "#FFF3E0", "#F5F5F5", "#E0F2F1" };
        string[] textColors = { "#0D47A1", "#2E7D32", "#EF6C00", "#424242", "#00695C" };

        for (int i = 0; i < 5; i++)
        {
            var colLetter = ((char)('A' + i)).ToString();
            
            var labelCell = worksheet.Cell($"{colLetter}6");
            labelCell.Value = titles[i];
            labelCell.Style.Font.SetFontSize(9).Font.SetFontColor(XLColor.DarkGray).Fill.SetBackgroundColor(XLColor.FromHtml(bgColors[i]));
            labelCell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.LightGray);

            var valueCell = worksheet.Cell($"{colLetter}7");
            valueCell.Value = values[i];
            valueCell.Style.Font.SetBold().Font.SetFontSize(16).Font.SetFontColor(XLColor.FromHtml(textColors[i])).Fill.SetBackgroundColor(XLColor.FromHtml(bgColors[i]));
            valueCell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin).Border.SetOutsideBorderColor(XLColor.LightGray);
            valueCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        worksheet.Cell("A9").Value = "DETALLE DE TAREAS";
        worksheet.Cell("A9").Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.FromHtml("#0D47A1"));

        string[] headers = { "ID", "Título", "Resp.", "Estado", "Creada" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(10, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.SetBold().Font.SetFontColor(XLColor.White).Fill.SetBackgroundColor(XLColor.FromHtml("#0D47A1"));
            cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        int currentRow = 11;
        foreach (var taskElement in tasksArray)
        {
            var rowBgColor = (currentRow % 2 == 0) ? XLColor.FromHtml("#F5F5F5") : XLColor.White;

            var taskId = GetIntValue(taskElement, "Id");
            var taskTitle = taskElement.GetProperty("Title").GetString() ?? "N/A";
            var taskResponsibleId = GetIntValue(taskElement, "ResponsibleId");
            var taskStatus = taskElement.GetProperty("Status").GetString() ?? "Pending";
            var taskCreatedAt = GetDateTimeValue(taskElement, "CreatedAt");

            worksheet.Cell(currentRow, 1).SetValue(taskId);
            worksheet.Cell(currentRow, 2).SetValue(taskTitle);
            worksheet.Cell(currentRow, 3).SetValue(taskResponsibleId);
            worksheet.Cell(currentRow, 4).SetValue(GetStatusSpanish(taskStatus));
            worksheet.Cell(currentRow, 5).SetValue(taskCreatedAt.ToString("dd/MM/yyyy"));

            for (int col = 1; col <= 5; col++)
            {
                var cell = worksheet.Cell(currentRow, col);
                cell.Style.Fill.SetBackgroundColor(rowBgColor);
                cell.Style.Font.SetFontSize(10);
                
                if (col == 4) 
                {
                    cell.Style.Font.SetBold();
                    cell.Style.Font.SetFontColor(taskStatus switch
                    {
                        "Completed" => XLColor.FromHtml("#2E7D32"),
                        "InProgress" => XLColor.FromHtml("#EF6C00"),
                        _ => XLColor.FromHtml("#424242")
                    });
                }
            }
            currentRow++;
        }

        worksheet.Columns(1, 5).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private string GetStatusSpanish(string status) => status switch
    {
        "Completed" => "Completada",
        "InProgress" => "En Progreso",
        "Pending" => "Pendiente",
        _ => status
    };

    private int GetIntValue(JsonElement element, string propertyName)
    {
        var property = element.GetProperty(propertyName);
        if (property.ValueKind == JsonValueKind.Number) return property.GetInt32();
        if (property.ValueKind == JsonValueKind.String && int.TryParse(property.GetString(), out var result)) return result;
        return 0;
    }

    private DateTime GetDateTimeValue(JsonElement element, string propertyName)
    {
        var property = element.GetProperty(propertyName);
        if (property.TryGetDateTime(out var dateTime)) return dateTime;
        if (property.ValueKind == JsonValueKind.String && DateTime.TryParse(property.GetString(), out var result)) return result;
        return DateTime.MinValue;
    }
}