using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ELY_TRAVEL_DOC.Services
{
    public class FilterService
    {
        public List<PersonalDataDto> FilterPersonalData(List<PersonalDataDto> personalDataList, List<string> selectedFields)
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

            var normalizedFields = selectedFields
            .Select(field => fieldMappings.ContainsKey(field) ? fieldMappings[field] : null)
            .Where(field => field != null)
            .ToList();

            return personalDataList.Select(data => new PersonalDataDto
            {
            Name = normalizedFields.Contains("Name") ? data.Name ?? "N/A" : null,
            Surname = normalizedFields.Contains("Surname") ? data.Surname ?? "N/A" : null,
            BirthDate = normalizedFields.Contains("BirthDate") ? data.BirthDate ?? "N/A" : null,
            Nationality = normalizedFields.Contains("Nationality") ? data.Nationality ?? "N/A" : null,
            Sex = normalizedFields.Contains("Sex") ? data.Sex ?? "N/A" : null,
            ExpiryDate = normalizedFields.Contains("ExpiryDate") ? data.ExpiryDate ?? "N/A" : null,
            DocumentNumber = normalizedFields.Contains("DocumentNumber") ? data.DocumentNumber ?? "N/A" : null,
            DocumentType = normalizedFields.Contains("DocumentType") ? data.DocumentType ?? "N/A" : null,
            Issuer = normalizedFields.Contains("Issuer") ? data.Issuer ?? "N/A" : null,
            OptionalData = normalizedFields.Contains("OptionalData") ? data.OptionalData ?? "N/A" : null
            }).ToList();
        }
    }
}
