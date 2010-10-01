using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFFSystem.Exception {
    public static class GError {
        public const string UNKNOWN_COMPONENT_TYPE = "Type de composant inconnu.";
        public const string UNKNOWN_FIELD_TYPE = "Type de champ inconnu.";
        public const string UNKNOWN_LARGE_FIELD_TYPE = "Type de champ large inconnu.";
        public const string UNKNOWN_COMPOSITE_TYPE = "Type composite inconnu.";
        public const string ADD_ROOT_TO_SOMETHING = "Ajout d'une structure racine à un champ composite.";
        public const string ADD_WRONG_STRUCTURE_CLASS_TO_LIST = "Ajout d'une structure non listée dans une liste.";
    }
    public static class XError {
        public const string CAN_NOT_GET_LABEL = "Erreur lors de la récupération du label.";
        public const string CAN_NOT_GET_STRUCT_TYPE = "Erreur lors de la récupération du type de structure.";
        public const string CAN_NOT_GET_FIELD_TYPE = "Erreur lors de la récupération du type de champ.";
        public const string ROOT_MISSING = "Elément racine manquant.";
    }
    public class ComponentException : ApplicationException {
        public ComponentException(string error) : base(error) { }
    }
    public class CompositeException : ApplicationException {
        public CompositeException(string error) : base(error) { }
    }
    public class FieldException : ApplicationException {
        public FieldException(string error) : base(error) { }
    }
    public class XFileException : ApplicationException {
        public XFileException(string error) : base(error) { }
    }
}
