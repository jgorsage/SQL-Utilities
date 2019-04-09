using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileLoader
{
    class Metadata
    {
        private string _fileName;
        public string fileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                //Ensure that we are using a file name that is alpha numeric or underscore.
                //This is so that SQL Server doesn't throw an error when creating the table.
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
                {
                    _fileName = Regex.Replace(value, "[^a-zA-Z0-9_]", "");
                }
                else
                {
                    _fileName = value;
                }
            }
        }
        public char delimiter { get; set; }
        public int columnCount { get; set; }
        public List<Column> columns { get; set; }
    }

    public class Column
    {
        public string name { get; set; }
        public string type { get; set; }
        public int typeLength { get; set; }
    }
}
