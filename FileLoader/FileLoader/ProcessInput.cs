using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace FileLoader
{
    class ProcessInput
    {
        private string _filePath;
        private string _serverName;
        private string _database;
        private Form1 _form;

        Dictionary<char, int> delimiters = new Dictionary<char, int> { { ',', 0 }, { '|', 0 }, { (char)9, 0 } };

        Metadata metadata = new Metadata();

        public ProcessInput(string filePath, string serverName, string database, Form1 form)
        {
            _filePath = filePath;
            _serverName = serverName;
            _database = database;
            _form = form;
        }

        public void LoadFile()
        {
            metadata.fileName = Path.GetFileNameWithoutExtension(_filePath);

            GetDelimiter();
            GetColumnNames();

            var file = new StreamReader(_filePath);
            if (_form._hasHeaders)
                file.ReadLine();

            var dt = new DataTable();
            foreach (Column col in metadata.columns)
            {
                var datacolumn = new DataColumn(col.name);
                datacolumn.AllowDBNull = true;
                dt.Columns.Add(datacolumn);
            }
            using (var csvReader = new TextFieldParser(file))
            {
                csvReader.SetDelimiters(metadata.delimiter.ToString());
                csvReader.HasFieldsEnclosedInQuotes = true;

                while (!csvReader.EndOfData)
                {
                    string[] filedData = csvReader.ReadFields();
                    for (int i = 0; i < filedData.Length; i++)
                    {
                        if (filedData[i] == "")
                        {
                            filedData[i] = null;
                        }
                    }
                    try
                    {
                        dt.Rows.Add(filedData);
                    }
                    catch (Exception e)
                    {
                        var result = string.Join(",", filedData);
                        _form.AppendText("[" + e.Message + "] [" + Path.GetFileName(_filePath) + "] [Line# " + (csvReader.LineNumber - 1).ToString() + "] [" + result + "]");
                    }
                }
            }

            //Create a list of column max lengths
            var maxColLength = Enumerable.Range(0, dt.Columns.Count)
                .Select(col => dt.AsEnumerable()
                .Select(row => row[col]).OfType<string>()
                .Max(val => val?.Length)).ToList();

            //Update the column typeLength metadata with the max lengths
            for (int i = 0; i < maxColLength.Count; i++)
            {
                metadata.columns[i].typeLength = maxColLength[i] ?? 100;
            }


            try
            {
                string connStr = "Data Source=" + _serverName + ";Initial Catalog=" + _database + ";Integrated Security=True;";
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    var createQuery = new StringBuilder();
                    createQuery.Append("create table ");
                    if (!string.IsNullOrEmpty(_form._schema))
                        createQuery.Append(_form._schema + ".");

                    createQuery.Append(metadata.fileName + "(");
                    for (int i = 0; i < metadata.columns.Count; i++)
                    {
                        Column c = metadata.columns[i];
                        createQuery.Append("[" + c.name + "] " + c.type);
                        if (c.type == "varchar")
                            createQuery.Append("(" + c.typeLength + ")");

                        if (i < metadata.columns.Count - 1)
                            createQuery.Append(",");
                    }
                    createQuery.Append(")");

                    var createCmd = new SqlCommand(createQuery.ToString(), conn);
                    createCmd.ExecuteNonQuery();

                    using (var bc = new SqlBulkCopy(conn))
                    {
                        if (!string.IsNullOrEmpty(_form._schema))
                            bc.DestinationTableName = _form._schema + "." + metadata.fileName;
                        else
                            bc.DestinationTableName = metadata.fileName;

                        bc.WriteToServer(dt);
                    }
                }

                _form.AppendText("Completed writing " + dt.Rows.Count.ToString() + " records to " + metadata.fileName);
                _form.AppendText("To update data types: alter table " + metadata.fileName + " alter column <column name> <new data type>");
            }
            catch (SqlException se)
            {
                _form.AppendText("Error: " + se.Message);
            }            
        }

        private void GetDelimiter()
        {
            if (!string.IsNullOrEmpty(_form._customDelimiter))
                metadata.delimiter = Convert.ToChar(_form._customDelimiter);
            else
            {
                var file = new StreamReader(_filePath);
                string firstLine = file.ReadLine();
                var count = firstLine.CharacterCount();

                var keys = new List<char>(delimiters.Keys);
                foreach (char key in keys)
                {
                    if (count.ContainsKey(key))
                        delimiters[key] = count[key];
                }
                
                metadata.delimiter = delimiters.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }
        }

        private void GetColumnNames()
        {
            var file = new StreamReader(_filePath);
            string firstLine = file.ReadLine();
            string[] columns = firstLine.Split(metadata.delimiter);
            metadata.columnCount = columns.Length;

            if (_form._hasHeaders)
            {
                metadata.columns = new List<Column>();
                foreach (string c in columns)
                {
                    metadata.columns.Add(new Column() { name = c, type = "varchar", typeLength = 200 });
                }
            }
            else
            {
                char c = 'A';
                metadata.columns = new List<Column>();
                for (int i = 0; i < columns.Length; i++)
                {
                    metadata.columns.Add(new Column() { name = c.ToString(), type = "varchar", typeLength = 200 });
                    c++;
                }
            }
        }
    }
}
