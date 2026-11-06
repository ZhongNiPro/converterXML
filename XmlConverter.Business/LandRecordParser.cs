using System.Xml;
using XmlConverter.Data;

namespace XmlConverter.Business;

public class LandRecordParser : BaseXmlParser, IObjectParser
{
    private readonly DateOnly _dateFormation;
    private string _cadNumber;
    private string _quarterCadNumber;

    public LandRecordParser(DateOnly dateFormation, string quarterCadNumber)
    {
        _dateFormation = dateFormation;
        _quarterCadNumber = quarterCadNumber;
        _cadNumber = string.Empty;
    }

    public async Task<ICadastralObject> ParseAsync(XmlReader reader)
    {
        string objectType = string.Empty;
        string subType = string.Empty;
        string category = string.Empty;
        string permittedUse = string.Empty;
        double area = 0;
        double inaccuracy = 0;
        string address = string.Empty;
        decimal cost = 0;
        string spesialNotes = string.Empty;
        List<string> ascendantCadNumbers = new List<string>();
        List<string> descendantCadNumbers = new List<string>();
        List<string> includedCadNumbers = new List<string>();
        List<ObjectPart> objectParts = new List<ObjectPart>();
        List<Contour> contours = new List<Contour>();
        List<Contour> partContours = new List<Contour>();
        List<RightRecord> rightRecords = new List<RightRecord>();

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CadastralConstants.ObjectTypesTags.LandRecordTag)
            {
                break;
            }

            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case CadastralConstants.XmlContentTags.CadNumberTag:
                        _cadNumber = await reader.ReadElementContentAsStringAsync();
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

                    case CadastralConstants.XmlContentTags.SubtypeTag:
                        subType = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                        break;

                    case CadastralConstants.XmlContentTags.CategoryTag:
                        category = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                        break;

                    case CadastralConstants.XmlContentTags.PermittedUseTag:
                        permittedUse = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ByDocumentTag);
                        break;

                    case CadastralConstants.XmlContentTags.AreaTag:
                        {
                            string areaString = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                            double.TryParse(areaString, System.Globalization.CultureInfo.InvariantCulture, out area);
                        }
                        break;

                    case CadastralConstants.XmlContentTags.InaccuracyTag:
                        {
                            string inaccuracyString = await reader.ReadElementContentAsStringAsync();
                            double.TryParse(inaccuracyString, System.Globalization.CultureInfo.InvariantCulture, out inaccuracy);
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

                    case CadastralConstants.XmlContentTags.AscendantCadNumberTag:
                        await ParseCadNumberListAsync(reader, ascendantCadNumbers, CadastralConstants.XmlContentTags.AscendantCadNumberTag);
                        break;

                    case CadastralConstants.XmlContentTags.DescendantCadNumberTag:
                        await ParseCadNumberListAsync(reader, descendantCadNumbers, CadastralConstants.XmlContentTags.DescendantCadNumberTag);
                        break;

                    case CadastralConstants.XmlContentTags.IncludedCadNumberTag:
                        await ParseCadNumberListAsync(reader, includedCadNumbers, CadastralConstants.XmlContentTags.IncludedCadNumberTag);
                        break;

                    case CadastralConstants.XmlContentTags.ObjectPartTag:
                        var part = await ParseSinglePartAsync(reader);
                        objectParts.Add(part);
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

        CadObjectLand cadObjectLand = new CadObjectLand(
             _dateFormation,
             _cadNumber,
             _quarterCadNumber,
             objectType,
             subType,
             category,
             permittedUse,
             area,
             inaccuracy,
             address,
             cost,
             spesialNotes
        );

        return cadObjectLand;
    }

    private async Task<ObjectPart> ParseSinglePartAsync(XmlReader reader)
    {
        int partNumber = 0;
        List<Contour> partContours = new List<Contour>();

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CadastralConstants.XmlContentTags.ObjectPartTag)
            {
                break;
            }

            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == CadastralConstants.XmlContentTags.PartNumberTag)
                {
                    int.TryParse(await reader.ReadElementContentAsStringAsync(), out partNumber);
                }
                else if (reader.Name == CadastralConstants.XmlContentTags.ContoursLocationTag)
                {
                    partContours = await ParseContoursListAsync(reader);
                }
            }
        }

        return new ObjectPart(partNumber, partContours);
    }

    private async Task ParseCadNumberListAsync(XmlReader reader, List<string> CadNumber, string tag)
    {
        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == tag)
            {
                break;
            }

            if (reader.NodeType == XmlNodeType.Element && reader.Name == tag)
            {
                string cadNumberValue = string.Empty;

                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == tag)
                    {
                        break;
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == CadastralConstants.XmlContentTags.CadNumberTag)
                    {
                        cadNumberValue = await reader.ReadElementContentAsStringAsync();
                    }
                }

                CadNumber.Add(cadNumberValue);
            }
        }
    }
}
