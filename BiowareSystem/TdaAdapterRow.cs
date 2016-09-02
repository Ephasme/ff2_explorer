using System;

namespace Bioware
{
    public class TdaAdapterRow
    {
        private readonly TdaAdapter _ad;
        private readonly string[] _values;

        public TdaAdapterRow(string[] values, TdaAdapter ad)
        {
            _values = values;
            _ad = ad;
        }

        public string this[string columnName]
        {
            get
            {
                var index = Array.IndexOf(_ad.Columns, columnName) + 1;
                return _values[index];
            }
        }
    }
}