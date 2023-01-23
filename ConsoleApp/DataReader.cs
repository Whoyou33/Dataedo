namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class DataReader
    {
        /// <summary>
        /// importing data to the lists
        /// </summary>
        /// <param name="ImportObjectsDataBase">DataBase List</param>
        /// <param name="ImportObjectsTable">Table List</param>
        /// <param name="ImportObjectsColumn">Column List</param>
        internal void ImportDataToLists(string fileToImport, ref List<ImportedObject> ImportObjectsDataBase, ref List<ImportedObject> ImportObjectsTable, ref List<ImportedObject> ImportObjectsColumn)
        {
            List<string> importedLines = new List<string>();

            using (StreamReader streamReader = new StreamReader(fileToImport))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    importedLines.Add(line);
                }
            }

            foreach(var importedLine in importedLines)
            {
                try
                {
                    var values = importedLine.Split(';');

                    switch (values[0].ToUpper())
                    {
                        case "DATABASE":
                            ImportObjectsDataBase.Add(DataSeparation(values));
                            break;
                        case "TABLE":
                            ImportObjectsTable.Add(DataSeparation(values));
                            break;
                        case "COLUMN":
                            ImportObjectsColumn.Add(DataSeparation(values));
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// data separation
        /// </summary>
        internal ImportedObject DataSeparation(string[] values)
        {
            var importedObject = new ImportedObject
            {
                Type = values[0],
                Name = values[1],
                Schema = values[2],
                ParentName = values[3],
                ParentType = values[4],
                DataType = values[5],
                IsNullable = values[6]
            };

            return importedObject;
        }

        /// <summary>
        /// Clear and correct imported data
        /// </summary>
        /// <param name="ImportedObjects"></param>
        internal void ClearAndCorrectImportedData(ref List<ImportedObject> ImportedObjects)
        {
            foreach (var importedObject in ImportedObjects)
            {
                try
                {
                    importedObject.Type = importedObject.Type.Trim().Replace(Environment.NewLine, "").ToUpper();
                    importedObject.Name = importedObject.Name.Trim().Replace(Environment.NewLine, "");
                    importedObject.Schema = importedObject.Schema.Trim().Replace(Environment.NewLine, "");
                    importedObject.ParentName = importedObject.ParentName.Trim().Replace(Environment.NewLine, "").ToUpper();
                    importedObject.ParentType = importedObject.ParentType.Trim().Replace(Environment.NewLine, "").ToUpper();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Assign mumber of children
        /// </summary>
        /// <param name="ImportedObjects">Parent</param>
        /// <param name="Objects">Children</param>
        internal void AssignNumberOfChildren(ref List<ImportedObject> ImportedObjects, List<ImportedObject> Objects)
        {
            foreach (ImportedObject item in ImportedObjects)
            {
                try
                {
                    foreach (var impObj in Objects)
                    {
                            if (impObj.ParentName.ToUpper() == item.Name.ToUpper())
                            {
                                item.NumberOfChildren++;
                            }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// printing the data
        /// </summary>
        /// <param name="ImportObjectsDataBase">DataBase</param>
        /// <param name="ImportObjectsTable">Table</param>
        /// <param name="ImportObjectsColum">Column</param>
        internal void Print(List<ImportedObject> ImportObjectsDataBase,List<ImportedObject> ImportObjectsTable,List<ImportedObject> ImportObjectsColum)
        {
            foreach(var database in ImportObjectsDataBase)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportObjectsTable)
                    {
                        if (table.ParentType == database.Type)
                        {
                            if (table.ParentName == database.Name.ToUpper())
                            {
                                Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                // print all table's columns
                                foreach (var column in ImportObjectsColum)
                                {
                                    if (column.ParentType == table.Type)
                                    {
                                        if (column.ParentName == table.Name.ToUpper())
                                        {
                                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.ReadLine();
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }
        public string ParentName { get; set; }
        public string ParentType { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public double NumberOfChildren { get; set; }
    }

    abstract class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
