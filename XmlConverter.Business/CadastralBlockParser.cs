using System.Xml;
using XmlConverter.Data;

namespace XmlConverter.Business
{
    public class CadastralBlockParser : BaseXmlParser, IObjectParser
    {
        private readonly DateOnly _dateFormation;
        private readonly string _quarterCadNumber;

        public CadastralBlockParser(DateOnly dateFormation, string quarterCadNumber)
        {
            _dateFormation = dateFormation;
            _quarterCadNumber = quarterCadNumber;
        }

        public async Task<ICadastralObject> ParseAsync(XmlReader reader)
        {
            string organRegistrRight = string.Empty;
            List<Contour> contours = new List<Contour>();

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == CadastralConstants.ObjectTypesTags.CadastralBlockTag)
                {
                    break;
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case CadastralConstants.XmlContentTags.OrganRegistrRightsTag:
                            organRegistrRight = await reader.ReadElementContentAsStringAsync();
                            break;

                        case CadastralConstants.XmlContentTags.ContoursLocationTag:
                            contours = await ParseContoursListAsync(reader);
                            break;
                    }
                }
            }

            CadObjectBlock cadObjectBlock = new CadObjectBlock(
             organRegistrRight,
             _dateFormation,
             _quarterCadNumber);

            return cadObjectBlock;
        }
    }
}
