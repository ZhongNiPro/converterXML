using System.Xml;
using XmlConverter.Data;

namespace XmlConverter.Business;

public abstract class BaseXmlParser
{
    protected async Task<string> ReadChildrenTagAsync(XmlReader reader, string parentTag, string childrenTag)
    {
        string result = string.Empty;

        using (XmlReader subReader = reader.ReadSubtree())
        {
            while (await subReader.ReadAsync())
            {
                if (subReader.NodeType == XmlNodeType.Element && subReader.Name == childrenTag)
                {
                    result = await subReader.ReadElementContentAsStringAsync();
                    break;
                }
            }
        }

        return result;
    }

    protected async Task<List<Contour>> ParseContoursListAsync(XmlReader reader)
    {
        List<Contour> contours = new List<Contour>();

        int counterContour = 1;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CadastralConstants.XmlContentTags.ContoursTag)
            {
                break;
            }

            if (reader.NodeType == XmlNodeType.Element && reader.Name == CadastralConstants.XmlContentTags.ContourTag)
            {
                Contour contour = await ParseSingleContourAsync(reader, counterContour);
                contours.Add(contour);
                counterContour++;
            }
        }

        return contours;
    }

    private async Task<Contour> ParseSingleContourAsync(XmlReader reader, int contourId)
    {
        string contourCadNumber = string.Empty;
        string skId = string.Empty;

        List<Coordinate> coordinates = new List<Coordinate>();

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CadastralConstants.XmlContentTags.ContourTag)
            {
                break;
            }

            if (reader.NodeType == XmlNodeType.Element && reader.Name == CadastralConstants.XmlContentTags.CadNumberTag)
            {
                contourCadNumber = await reader.ReadElementContentAsStringAsync();
            }

            if (reader.NodeType == XmlNodeType.Element && reader.Name == CadastralConstants.XmlContentTags.SkIdTag)
            {
                skId = await reader.ReadElementContentAsStringAsync();
            }

            if (reader.NodeType == XmlNodeType.Element && reader.Name == CadastralConstants.XmlContentTags.OrdinateTag)
            {
                int ordNmb = 0;
                double x = 0;
                double y = 0;

                string? attrNum = reader.GetAttribute(CadastralConstants.XmlContentTags.AttributeOrdNumber) ?? reader.GetAttribute(CadastralConstants.XmlContentTags.OrdNmbTag);
                string? attrX = reader.GetAttribute(CadastralConstants.XmlContentTags.AttributeX);
                string? attrY = reader.GetAttribute(CadastralConstants.XmlContentTags.AttributeY);

                if (attrX != null && attrY != null)
                {
                    double.TryParse(attrX, System.Globalization.CultureInfo.InvariantCulture, out x);
                    double.TryParse(attrY, System.Globalization.CultureInfo.InvariantCulture, out y);
                    int.TryParse(attrNum, out ordNmb);
                }
                else
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CadastralConstants.XmlContentTags.OrdinateTag)
                        {
                            break;
                        }

                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case CadastralConstants.XmlContentTags.OrdNmbTag:
                                    string rawNum = await reader.ReadElementContentAsStringAsync();
                                    int.TryParse(rawNum, out ordNmb);
                                    break;

                                case CadastralConstants.XmlContentTags.XTag:
                                    string rawX = await reader.ReadElementContentAsStringAsync();
                                    double.TryParse(rawX, System.Globalization.CultureInfo.InvariantCulture, out x);
                                    break;

                                case CadastralConstants.XmlContentTags.YTag:
                                    string rawY = await reader.ReadElementContentAsStringAsync();
                                    double.TryParse(rawY, System.Globalization.CultureInfo.InvariantCulture, out y);
                                    break;
                            }
                        }
                    }
                }

                Coordinate coord = new Coordinate(ordNmb, x, y);
                coordinates.Add(coord);
            }
        }

        return new Contour(contourId, contourCadNumber, skId, coordinates);
    }

    protected async Task<List<RightRecord>> ParseRightRecordsListAsync(XmlReader reader)
    {
        List<RightRecord> rightRecords = new List<RightRecord>();
        int counterRightRecords = 1;

        using (XmlReader subReader = reader.ReadSubtree())
        {
            string rightType = string.Empty;
            string name = string.Empty;
            string surname = string.Empty;
            string patronymic = string.Empty;

            while (await subReader.ReadAsync())
            {
                if (subReader.NodeType == XmlNodeType.EndElement && subReader.Name == CadastralConstants.XmlContentTags.RightRecordTag)
                {
                    string rightHolder = string.IsNullOrEmpty(surname) == false && string.IsNullOrEmpty(patronymic) == false
                        ? $"{surname} {name} {patronymic}".Trim()
                        : name;

                    if (string.IsNullOrEmpty(rightType) == false || string.IsNullOrEmpty(rightHolder) == false)
                    {
                        rightRecords.Add(new RightRecord(counterRightRecords++, rightType, rightHolder));
                    }

                    rightType = name = surname = patronymic = string.Empty;
                }

                if (subReader.NodeType == XmlNodeType.Element)
                {
                    switch (subReader.Name)
                    {
                        case CadastralConstants.XmlContentTags.ValueTag:
                            rightType = await subReader.ReadElementContentAsStringAsync();
                            break;

                        case CadastralConstants.XmlContentTags.NameTag:
                            name = await subReader.ReadElementContentAsStringAsync();
                            break;

                        case CadastralConstants.XmlContentTags.SurnameTag:
                            surname = await subReader.ReadElementContentAsStringAsync();
                            break;

                        case CadastralConstants.XmlContentTags.PatronymicTag:
                            patronymic = await subReader.ReadElementContentAsStringAsync();
                            break;
                    }
                }
            }
        }

        return rightRecords;
    }
}