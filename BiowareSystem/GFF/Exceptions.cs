using System;

namespace Bioware.GFF.Exception {
    public static class Error {
        public const string UnknownComponentType = "Type de composant inconnu.";
        public const string UnknownFieldType = "Type de champ inconnu.";
        public const string UnknownLargeFieldType = "Type de champ large inconnu.";
        public const string UnknownCompositeType = "Type composite inconnu.";
        public const string AddRootToSomething = "Ajout d'une structure racine à un champ composite.";
        public const string AddWrongStructureClassToList = "Ajout d'une structure non listée dans une liste.";
    }
    public class ComponentException : ApplicationException {
        public ComponentException(string error) : base(error) {}
    }
    public class CompositeException : ApplicationException {
        public CompositeException(string error) : base(error) {}
    }
    public class FieldException : ApplicationException {
        public FieldException(string error) : base(error) {}
    }
}
namespace Bioware.GFF.XML.Exception {
    public static class Error {
        public const string CanNotGetLabel = "Erreur lors de la récupération du label.";
        public const string CanNotGetStructType = "Erreur lors de la récupération du type de structure.";
        public const string CanNotGetFieldType = "Erreur lors de la récupération du type de champ.";
        public const string CanNotGetExtention = "Erreur lors de la récupération de l'extention.";
        public const string RootMissing = "Elément racine manquant.";
    }
    public class FileException : ApplicationException {
        public FileException(string error) : base(error) {}
    }
}