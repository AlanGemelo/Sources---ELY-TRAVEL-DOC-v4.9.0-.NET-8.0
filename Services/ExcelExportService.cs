using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;

namespace ELY_TRAVEL_DOC.Services
{
    public class ExcelExportService
    {
        public void ExportPersonalDataToExcel(List<PersonalDataDto> personalDataList, List<string> selectedFields, string filePath)
        {
            var fieldMappings = new Dictionary<string, string>
            {
            { "Nombre", "Name" },
            { "Apellido", "Surname" },
            { "Fecha de Nacimiento", "BirthDate" },
            { "Nacionalidad", "Nationality" },
            { "Sexo", "Sex" },
            { "Fecha de Expiración", "ExpiryDate" },
            { "Número de Documento", "DocumentNumber" },
            { "Tipo de Documento", "DocumentType" },
            { "Emisor", "Issuer" },
            { "Datos Opcionales", "OptionalData" },
            { "Name", "Name" },
            { "Surname", "Surname" },
            { "Birth Date", "BirthDate" },
            { "Nationality", "Nationality" },
            { "Sex", "Sex" },
            { "Expiry Date", "ExpiryDate" },
            { "Document Number", "DocumentNumber" },
            { "Document Type", "DocumentType" },
            { "Issuer", "Issuer" },
            { "Optional Data", "OptionalData" },
            { "Nom", "Name" },
            { "Prénom", "Surname" },
            { "Date de naissance", "BirthDate" },
            { "Nationalité", "Nationality" },
            { "Sexe", "Sex" },
            { "Date d'expiration", "ExpiryDate" },
            { "Numéro de document", "DocumentNumber" },
            { "Type de document", "DocumentType" },
            { "Émetteur", "Issuer" },
            { "Données optionnelles", "OptionalData" }
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
            var worksheet = package.Workbook.Worksheets.Add("PersonalData");

            int columnIndex = 1;
            foreach (var field in selectedFields)
            {
                var normalizedField = fieldMappings.ContainsKey(field) ? fieldMappings[field] : null;
                if (normalizedField == null) continue;

                worksheet.Cells[1, columnIndex].Value = field; // Use original field name for header
                for (int i = 0; i < personalDataList.Count; i++)
                {
                var value = typeof(PersonalDataDto).GetProperty(normalizedField)?.GetValue(personalDataList[i], null);
                worksheet.Cells[i + 2, columnIndex].Value = value ?? null; // Ensure no empty values
                }
                columnIndex++;
            }

            package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}
