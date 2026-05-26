using System.Xml.Serialization;

namespace MoFs.Prototype.Models;

public class MobilityOnlineRow
{
    [XmlElement("BEW_ID")] public string? ApplicationId { get; set; }
    [XmlElement("KZ_BEW_ART")] public string? ApplicationType { get; set; }
    [XmlElement("KZ_BEW_PERS")] public string? PersonType { get; set; }
    [XmlElement("AUST_PROG_ID")] public string? MobilityProgram { get; set; }
    [XmlElement("STUDJ_ID")] public string? StudyYear { get; set; }
    [XmlElement("SEM_ID")] public string? Semester { get; set; }
    [XmlElement("BEW_NACHNAME")] public string? LastName { get; set; }
    [XmlElement("BEW_NACHNAME_2")] public string? LastName2 { get; set; }
    [XmlElement("BEW_VORNAME")] public string? FirstName { get; set; }
    [XmlElement("BEW_GESCHLECHT")] public string? Gender { get; set; }
    [XmlElement("BEW_GEB_DATUM")] public string? BirthDate { get; set; }
    [XmlElement("P_SOZIAL_VER_NR")] public string? SocialSecurityNumber { get; set; }
    [XmlElement("BEW_EMAIL")] public string? Email { get; set; }
    [XmlElement("BEW_MATR_NR")] public string? StudentNumber { get; set; }
    [XmlElement("SPR_ID")] public string? Language { get; set; }
    [XmlElement("LCD_ID_NAT")] public string? Nationality { get; set; }
    [XmlElement("LCD_ID_HEIM")] public string? HomeCountry { get; set; }
    [XmlElement("INST_ID_HEIM")] public string? HomeInstitutionCode { get; set; }
    [XmlElement("INST_ID_GAST")] public string? HostInstitutionCode { get; set; }
    [XmlElement("INST_NAME_FULL")] public string? HostInstitutionName { get; set; }
    [XmlElement("BEW_DAT_VON")] public string? MobilityStartDate { get; set; }
    [XmlElement("BEW_DAT_BIS")] public string? MobilityEndDate { get; set; }
    [XmlElement("BEW_MONATE")] public string? MobilityMonths { get; set; }
    [XmlElement("STUDR_ID")] public string? StudyProgramName { get; set; }
    [XmlElement("BEW_STATUS_ID")] public string? ApplicationStatus { get; set; }
    [XmlElement("KZ_SPRACHKURS")] public string? LanguageCourse { get; set; }
    [XmlElement("IS_VERLAENGERUNG_VORH")] public string? HasExtension { get; set; }
    [XmlElement("ERFDAT")] public string? CreatedDate { get; set; }
    [XmlElement("MODDAT")] public string? ModifiedDate { get; set; }
}
