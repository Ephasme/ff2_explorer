using System.IO;
using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    internal class GffReader
    {
        private readonly GffFactory _fac;
        private readonly GffBase _gb;
        private LatinBinaryReader _br;
        private GffRootStruct _root;

        public GffReader(GffBase gbase)
        {
            _gb = gbase;
            _fac = new GffFactory(_gb);
        }

        public GffReader(string path)
        {
            Load(path);
        }

        public GffReader(Stream stream)
        {
            Load(stream);
        }

        public GffRootStruct RootStruct => _root ?? (_root = _fac.CreateRoot());

        public void Load(string path)
        {
            if (File.Exists(path))
            {
                Load(File.OpenRead(path));
            }
        }

        public void Load(Stream stream)
        {
            _br = new LatinBinaryReader(stream);
            _gb.Header.Load(_br);
            LoadStructures();
            LoadFields();
            LoadLabels();
            LoadFieldDatas();
            LoadFieldIndicesArray();
            LoadListIndicesArray();
            _br.Close();
        }

        private void LoadStructures()
        {
            var pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.StructOffset;
            for (var i = 0; i < _gb.Header.StructCount; i++)
            {
                var q = _br.GetUInt32Queue(GffBasicFrame.ValueCount);
                var sf = new GffStructFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                _gb.StructArray.Add(sf);
            }
            _br.Stream.Position = pos;
        }

        private void LoadFields()
        {
            var pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.FieldOffset;
            for (var i = 0; i < _gb.Header.FieldCount; i++)
            {
                var q = _br.GetUInt32Queue(GffBasicFrame.ValueCount);
                var ff = new GffFieldFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                _gb.FieldArray.Add(ff);
            }
            _br.Stream.Position = pos;
        }

        private void LoadLabels()
        {
            var pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.LabelOffset;
            for (var i = 0; i < _gb.Header.LabelCount; i++)
            {
                var lbl = new string(_br.ReadChars(GffConst.LabelLength));
                lbl = lbl.TrimEnd(GffConst.LabelPaddingCharacter);
                _gb.LabelArray.Add(lbl);
            }
            _br.Stream.Position = pos;
        }

        private void LoadFieldDatas()
        {
            var pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.FieldDataOffset;
            _gb.FieldDataBlock = new MemoryStream(_br.ReadBytes((int) _gb.Header.FieldDataCount));
            _br.Stream.Position = pos;
        }

        private void LoadFieldIndicesArray()
        {
            var pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.FieldIndicesOffset;
            _gb.FieldIndicesArray = new MemoryStream(_br.ReadBytes((int) _gb.Header.FieldIndicesCount));
            _br.Stream.Position = pos;
        }

        private void LoadListIndicesArray()
        {
            var pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.ListIndicesOffset;
            _gb.ListIndicesArray = new MemoryStream(_br.ReadBytes((int) _gb.Header.ListIndicesCount));
            _br.Stream.Position = pos;
        }
    }
}