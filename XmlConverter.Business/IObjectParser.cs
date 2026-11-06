using System.Xml;
using XmlConverter.Data;

namespace XmlConverter.Business;

public interface IObjectParser
{
    Task<ICadastralObject> ParseAsync(XmlReader reader);
}