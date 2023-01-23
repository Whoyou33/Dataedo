namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new DataReader();

            List<ImportedObject> ImportObjectsColumn = new List<ImportedObject>();
            List<ImportedObject> ImportObjectsTable = new List<ImportedObject>();
            List<ImportedObject> ImportObjectsDataBase = new List<ImportedObject>();

            reader.ImportDataToLists("data.csv", ref ImportObjectsDataBase, ref ImportObjectsTable, ref ImportObjectsColumn);

            reader.ClearAndCorrectImportedData(ref ImportObjectsDataBase);
            reader.ClearAndCorrectImportedData(ref ImportObjectsTable);
            reader.ClearAndCorrectImportedData(ref ImportObjectsColumn);

            reader.AssignNumberOfChildren(ref ImportObjectsDataBase, ImportObjectsTable);
            reader.AssignNumberOfChildren(ref ImportObjectsTable, ImportObjectsColumn);

            reader.Print(ImportObjectsDataBase, ImportObjectsTable, ImportObjectsColumn);
        }
    }
}
