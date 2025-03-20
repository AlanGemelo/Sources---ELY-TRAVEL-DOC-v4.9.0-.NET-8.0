using System.Collections.Generic;
using System.Linq;

namespace ELY_TRAVEL_DOC.Services
{
    public class FilterService
    {
        public List<PersonalDataDto> FilterPersonalData(List<PersonalDataDto> personalDataList, List<string> selectedFields)
        {
            return personalDataList.Select(data => new PersonalDataDto
            {
                Name = selectedFields.Contains("Name") ? data.Name : null,
                Surname = selectedFields.Contains("Surname") ? data.Surname : null,
                BirthDate = selectedFields.Contains("BirthDate") ? data.BirthDate : null,
                Nationality = selectedFields.Contains("Nationality") ? data.Nationality : null,
                Sex = selectedFields.Contains("Sex") ? data.Sex : null,
                ExpiryDate = selectedFields.Contains("ExpiryDate") ? data.ExpiryDate : null,
                DocumentNumber = selectedFields.Contains("DocumentNumber") ? data.DocumentNumber : null,
                DocumentType = selectedFields.Contains("DocumentType") ? data.DocumentType : null,
                Issuer = selectedFields.Contains("Issuer") ? data.Issuer : null,
                OptionalData = selectedFields.Contains("OptionalData") ? data.OptionalData : null
            }).ToList();
        }
    }
}
