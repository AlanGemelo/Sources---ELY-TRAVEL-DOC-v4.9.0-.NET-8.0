using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ELY_TRAVEL_DOC.Services
{
    public class JsonExportService
    {
        public void ExportPersonalDataToJson(List<PersonalDataDto> data, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }
    }
}
