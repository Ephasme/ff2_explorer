using System;
using System.Collections.Generic;
using System.IO;

namespace Bioware
{
    public class TdaAdapter
    {
        public const string Blank = "****";

        public static readonly char[] Separators = {' '};

        private readonly List<TdaAdapterRow> _rows;

        private readonly StreamReader _srd;

        public TdaAdapter(ContentObject co)
        {
            _rows = new List<TdaAdapterRow>();

            if (co.ResType != ResType.Dda)
            {
                throw new ApplicationException("L'objet n'est pas un 2da valide.");
            }

            _srd = new StreamReader(co.DataStream);
            _srd.ReadLine(); // Skip useless line.
            _srd.ReadLine(); // Skip useless line.

            while (!_srd.EndOfStream)
            {
                var line = _srd.ReadLine();
                if (line == null)
                {
                    break;
                }
                Columns = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                _rows.Add(new TdaAdapterRow(Columns, this));
            }
        }

        public string[] Columns { get; }

        public TdaAdapterRow this[int rowIndex] => _rows[rowIndex];
    }
}