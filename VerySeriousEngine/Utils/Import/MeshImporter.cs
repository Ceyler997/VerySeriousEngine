using System;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Utils.Importers;

namespace VerySeriousEngine.Utils
{
    public class MeshImporter
    {
        private enum SupportedFormats
        {
            Unknown,
            Obj,
        }

        private static SupportedFormats GetFormatFromFile(string filePath)
        {
            if (filePath.EndsWith(".obj", StringComparison.InvariantCultureIgnoreCase))
                return SupportedFormats.Obj;

            return SupportedFormats.Unknown;
        }

        public static StaticMesh[] ImportModelFromFile(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            switch(GetFormatFromFile(filePath))
            {
                case SupportedFormats.Obj:
                    var objImporter = new ObjFileImporter(filePath);
                    return objImporter.GetResult();
                default:
                    throw new ArgumentException("Unsupported file format");
            }
        }

    }
}
