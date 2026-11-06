using System.Xml;
using XmlConverter.Data;

namespace XmlConverter.Business;

public class BuiltRecordParser : BaseXmlParser, IObjectParser
{
    private readonly DateOnly _dateFormation;
    private readonly string _tag;
    private string _quarterCadNumber;


    public BuiltRecordParser(DateOnly dateFormation, string quarterCadNumber, string tag)
    {
        _dateFormation = dateFormation;
        _quarterCadNumber = quarterCadNumber;
        _tag = tag;
    }

    public async Task<ICadastralObject> ParseAsync(XmlReader reader)
    {
        string cadNumber = string.Empty;
        string objectType = string.Empty;
        string purpose = string.Empty;
        double area = 0;
        string address = string.Empty;
        decimal cost = 0;
        string spesialNotes = string.Empty;
        double extension = 0;
        double builtUpArea = 0;
        double depth = 0;
        double occurenceDepth = 0;
        double volume = 0;
        double height = 0;

        List<Contour> contours = new List<Contour>();
        List<RightRecord> rightRecords = new List<RightRecord>();

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == _tag)
            {
                break;
            }

            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case CadastralConstants.XmlContentTags.CadNumberTag:
                        cadNumber = await reader.ReadElementContentAsStringAsync();
                        break;

                    case CadastralConstants.XmlContentTags.QuarterCadastralNumberTag:
                        {
                            if (_quarterCadNumber == null)
                                _quarterCadNumber = await reader.ReadElementContentAsStringAsync();
                        }
                        break;

                    case CadastralConstants.XmlContentTags.ObjectTypeTag:
                        objectType = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                        break;

                    case CadastralConstants.XmlContentTags.PurposeTag:
                        purpose = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                        break;

                    case CadastralConstants.XmlContentTags.AreaTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out area);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.ExtensionTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out extension);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.BuiltUpAreaTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out builtUpArea);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.DepthTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out depth);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.OccurenceDepthTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out occurenceDepth);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.VolumeTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out volume);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.HeightTag:
                        {
                            string areaString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out height);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.AddressTag:
                        address = await reader.ReadElementContentAsStringAsync();
                        break;

                    case CadastralConstants.XmlContentTags.CostTag:
                        {
                            string costString = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                            decimal.TryParse(costString, System.Globalization.CultureInfo.InvariantCulture, out cost);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.SpesialNotes:
                        spesialNotes = await reader.ReadElementContentAsStringAsync();
                        break;

                    case CadastralConstants.XmlContentTags.ContoursLocationTag:
                        contours = await ParseContoursListAsync(reader);
                        break;

                    case CadastralConstants.XmlContentTags.RightRecordTag:
                        await ParseRightRecordsListAsync(reader);
                        break;
                }
            }

        }

        CadObjectBuilt cadObjectLand = new CadObjectBuilt(
             _dateFormation,
             cadNumber,
             _quarterCadNumber,
             objectType,
             purpose,
             area,
             address,
             cost,
             spesialNotes,
             extension,
             builtUpArea,
             depth,
             occurenceDepth,
             volume,
             height
        );

        return cadObjectLand;
    }
}
