namespace XmlConverter.Database;

public static class DatabaseConstants
{
    public static class Tables
    {
        public const string Registry = "cad_registry";
        public const string Quarters = "cad_quarters";
        public const string Lands = "cad_lands";
        public const string Builts = "cad_builts";
        public const string Boundaries = "cad_boundaries";
        public const string BoundaryQuarters = "cad_boundary_quarters";
        public const string Blocks = "cad_blocks";
        public const string Contours = "contours";
        public const string Coordinates = "coordinates";
        public const string LandAscendants = "cad_land_ascendants";
        public const string LandDescendants = "cad_land_descendants";
        public const string LandIncluded = "cad_land_included";
        public const string LandParts = "cad_land_parts";
        public const string PropertyRights = "property_rights";
    }

    public static class Columns
    {
        public const string ObjectId = "object_id";
        public const string ObjectType = "object_type";
        public const string QuarterId = "quarter_id";
        public const string ParentObjectId = "parent_object_id";
        public const string ContourId = "contour_id";
        public const string QuarterNumber = "quarter_number";
        public const string CadastralNumber = "cadastral_number";
        public const string ExtractDate = "extract_date"; 
        public const string SubType = "sub_type";
        public const string Category = "category";
        public const string PermittedUse = "permitted_use";
        public const string Inaccuracy = "inaccuracy";
        public const string SpecialNotes = "special_notes";
        public const string Area = "area";
        public const string Address = "address";
        public const string Cost = "cost";
        public const string RelatedCadastralNumber = "related_cadastral_number";
        public const string PartNumber = "part_number";
        public const string RecordNumber = "record_number";
        public const string RightHolder = "right_holder";
        public const string RightType = "right_type";
        public const string Purpose = "purpose";
        public const string Extension = "extension";
        public const string BuiltUpArea = "built_up_area";
        public const string Depth = "depth";
        public const string OccurenceDepth = "occurence_depth";
        public const string Volume = "volume";
        public const string Height = "height";
        public const string RegNumbBorder = "reg_numb_border";
        public const string TypeBoundary = "type_boundary";
        public const string TypeZone = "type_zone";
        public const string Description = "description";
        public const string NameByDoc = "name_by_doc";
        public const string OrganRegistrRight = "organ_registr_right";
        public const string ContourNumber = "contour_number";
        public const string OrdNmb = "ord_nmb";
        public const string X = "x";
        public const string Y = "y";

    }

    public static class Discriminators
    {
        public const string Land = "land";
        public const string Built = "built";
        public const string Boundary = "boundary";
        public const string Block = "block";
        public const string LandPart = "land_part";
        public const string UnknownType = "Unknown type of cadastre object";
    }
}