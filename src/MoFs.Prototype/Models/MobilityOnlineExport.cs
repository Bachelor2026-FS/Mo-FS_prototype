using System.Xml.Serialization;

namespace MoFs.Prototype.Models;

[XmlRoot("AUSWERTUNG")]
public class MobilityOnlineExport
{
    [XmlElement("row")]
    public List<MobilityOnlineRow> Rows { get; set; } = new();
}
