using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace ELY_TRAVEL_DOC.Services
{
    public class ExcelExportService
    {
        public void ExportPersonalDataToExcel(List<PersonalDataDto> personalDataList, List<string> selectedFields, string filePath)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PersonalData");

                int columnIndex = 1;
                if (selectedFields.Contains("Name"))
                {
                    worksheet.Cells[1, columnIndex].Value = "Name";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].Name;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("Surname"))
                {
                    worksheet.Cells[1, columnIndex].Value = "Surname";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].Surname;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("BirthDate"))
                {
                    worksheet.Cells[1, columnIndex].Value = "BirthDate";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].BirthDate;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("Nationality"))
                {
                    worksheet.Cells[1, columnIndex].Value = "Nationality";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].Nationality;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("Sex"))
                {
                    worksheet.Cells[1, columnIndex].Value = "Sex";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].Sex;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("ExpiryDate"))
                {
                    worksheet.Cells[1, columnIndex].Value = "ExpiryDate";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].ExpiryDate;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("DocumentNumber"))
                {
                    worksheet.Cells[1, columnIndex].Value = "DocumentNumber";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].DocumentNumber;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("DocumentType"))
                {
                    worksheet.Cells[1, columnIndex].Value = "DocumentType";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].DocumentType;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("Issuer"))
                {
                    worksheet.Cells[1, columnIndex].Value = "Issuer";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].Issuer;
                    }
                    columnIndex++;
                }
                if (selectedFields.Contains("OptionalData"))
                {
                    worksheet.Cells[1, columnIndex].Value = "OptionalData";
                    for (int i = 0; i < personalDataList.Count; i++)
                    {
                        worksheet.Cells[i + 2, columnIndex].Value = personalDataList[i].OptionalData;
                    }
                    columnIndex++;
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}
