using XmlConverter.Data;

namespace XmlConverter.Business;

public class ParserFactory
{
    public IObjectParser? GetParser(string xmlNodeName, DateOnly dateFormation, string quarterCadNumber)
    {
        return xmlNodeName switch
        {
            CadastralConstants.ObjectTypesTags.CadastralBlockTag => new CadastralBlockParser(dateFormation, quarterCadNumber),
            CadastralConstants.ObjectTypesTags.LandRecordTag => new LandRecordParser(dateFormation, quarterCadNumber),
            CadastralConstants.ObjectTypesTags.BuildRecordTag =>
            new BuiltRecordParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.BuildRecordTag),
            CadastralConstants.ObjectTypesTags.ConstructionRecordTag =>
            new BuiltRecordParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.ConstructionRecordTag),
            CadastralConstants.ObjectTypesTags.ObjectUnderConstructionRecordTag =>
            new BuiltRecordParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.ObjectUnderConstructionRecordTag),
            CadastralConstants.ObjectTypesTags.MunicipalBoundaryRecordTag =>
            new BoundaryParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.MunicipalBoundaryRecordTag),
            CadastralConstants.ObjectTypesTags.InhabitedLocalityBoundaryRecordTag =>
            new BoundaryParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.InhabitedLocalityBoundaryRecordTag),
            CadastralConstants.ObjectTypesTags.ZonesAndTerritoriesRecordTag =>
            new BoundaryParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.ZonesAndTerritoriesRecordTag),
            CadastralConstants.ObjectTypesTags.PublicEasementRecordTag =>
            new BoundaryParser(dateFormation, quarterCadNumber, CadastralConstants.ObjectTypesTags.PublicEasementRecordTag),
            _ => null
        };
    }
}