using System.Xml;
using XmlConverter.Data;

namespace XmlConverter.Business
{
    public class BoundaryParser : BaseXmlParser, IObjectParser
    {
        private readonly DateOnly _dateFormation;
        private readonly string _quarterCadNumber;
        private readonly string _tag;

        public BoundaryParser(DateOnly dateFormation, string quarterCadNumber, string tag)
        {
            _dateFormation = dateFormation;
            _quarterCadNumber = quarterCadNumber;
            _tag = tag;
        }

        public async Task<ICadastralObject> ParseAsync(XmlReader reader)
        {
            //DateOnly registrationDate;
            string regNumbBorder = string.Empty;
            string typeBoundary = string.Empty;
            string typeZone = string.Empty;
            string description = string.Empty;
            string nameByDoc = string.Empty;
            List<Contour> contours = new List<Contour>();

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
                        case CadastralConstants.XmlContentTags.RegNumbBorderTag:
                            {
                                regNumbBorder = await reader.ReadElementContentAsStringAsync();
                            }
                            break;

                        case CadastralConstants.XmlContentTags.TypeBoundaryTag:
                            typeBoundary = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                            break;

                        case CadastralConstants.XmlContentTags.TypeZoneTag:
                            typeZone = await ReadChildrenTagAsync(reader, reader.Name, CadastralConstants.XmlContentTags.ValueTag);
                            break;

                        case CadastralConstants.XmlContentTags.DescriptionTag:
                                description = await reader.ReadElementContentAsStringAsync();
                            break;

                        case CadastralConstants.XmlContentTags.NameByDocTag:
                                nameByDoc = await reader.ReadElementContentAsStringAsync();
                            break;

                        case CadastralConstants.XmlContentTags.ContoursLocationTag:
                            contours = await ParseContoursListAsync(reader);
                            break;
                    }
                }
            }

            CadObjectBoundary cadObjectBoundary = new CadObjectBoundary(
                    _dateFormation,
                    _quarterCadNumber,
                    regNumbBorder,
                    typeBoundary,
                    typeZone,
                    description,
                    nameByDoc
                    );

            return cadObjectBoundary;
        }
    }
}
