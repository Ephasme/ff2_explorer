using System;
using System.IO;
using System.Xml;
using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    internal class GffXmlReader : GffXmlBase
    {
        public const int SclassInField = 0;
        public const int SclassInList = 1;
        public const int SclassRoot = 2;
        public GffRootStruct RootStruct { get; private set; }

        public void Load(string path)
        {
            Initialize();
            if (File.Exists(path) && Path.GetExtension(path) == Ext)
            {
                Xdoc.Load(path);
            }
            Xroot = Xdoc.SelectSingleNode("/" + EStruct);
            RootStruct = (GffRootStruct) CreateComponent(Xroot);
        }

        private static string GetLabel(XmlNode node)
        {
            if (node.Attributes?[ALabel] != null)
            {
                return node.Attributes[ALabel].Value;
            }
            throw new FileException(ErrorLabels.CanNotGetLabel);
        }

        private static uint GetStructType(XmlNode node)
        {
            if (node.Attributes?[AType] != null)
            {
                return uint.Parse(node.Attributes[AType].Value);
            }
            throw new FileException(ErrorLabels.CanNotGetStructType);
        }

        private int GetStructClass(XmlNode node)
        {
            var parent = node.ParentNode;
            if (parent == Xdoc) return SclassRoot;
            if (parent != null && parent.Name == EList)
            {
                return SclassInList;
            }
            return SclassInField;
        }

        private static GffType GetFieldType(XmlNode node)
        {
            if (node.Attributes?[AType] != null)
            {
                return (GffType) Enum.Parse(typeof (GffType), node.Attributes[AType].Value);
            }
            throw new FileException(ErrorLabels.CanNotGetFieldType);
        }

        private GffComponent CreateComponent(XmlNode node)
        {
            GffComponent cpnt = null;
            switch (node.Name)
            {
                case EStruct:
                    switch (GetStructClass(node))
                    {
                        case SclassInField:
                            cpnt = new GffInFieldStruct(GetLabel(node), GetStructType(node));
                            break;
                        case SclassInList:
                            cpnt = new GffInListStruct(GetStructType(node));
                            break;
                        case SclassRoot:
                            cpnt = new GffRootStruct(GetExtention(node));
                            break;
                    }
                    break;
                case EList:
                    cpnt = new GffList(GetLabel(node));
                    break;
                case EField:
                    cpnt = new GffField(GetLabel(node), GetFieldType(node), GetFieldValue(node));
                    break;
            }
            var composite = cpnt as GffComposite;
            if (composite == null) return cpnt;
            var cpsit = composite;
            foreach (XmlNode child in node.ChildNodes)
            {
                cpsit.Add(CreateComponent(child));
            }
            return cpnt;
        }

        private static string GetFieldValue(XmlNode node)
        {
            return node.InnerText;
        }

        private static string GetExtention(XmlNode node)
        {
            if (node.Attributes?[AExtention] != null)
            {
                return node.Attributes[AExtention].Value;
            }
            throw new FileException(ErrorLabels.CanNotGetExtention);
        }
    }
}