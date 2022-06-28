using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CsvHelper;
using Spire.Xls;

namespace Task1Module1

{
    class Program
    {
        static void Main(string[] args)

        {
            if (args.Count() < 1)
            {
                //Принтиране за съобщение за въвеждане на пътя към файла.
                Console.WriteLine("Please enter a file path:");
                args = new string[1];
                args[0] = Console.ReadLine();
            }
            var path = args[0];
            //Ако пътят е грешен или има грешка в изписването на файла, излиза съобщение "Must provide a valid CSV"
            //Програмата не приема за грешка ако пътят бъде изписан с глани или малки букви.
            if (!File.Exists(path) || !path.ToUpper().Contains("CSV"))
            {
                Console.WriteLine("Must provide a valid CSV");
                System.Threading.Thread.Sleep(500);
                return;
            }

            List<dynamic> issues;
            using (var reader = new StreamReader(path))
            {
                //четене на файла
                using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    issues = csv.GetRecords<dynamic>().ToList();
                }
            }
            //превръщане от .csv в .xlsx
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                DataTable table = ToDataTable(issues);
                wb.AddWorksheet(table, "Sheet1");
                foreach (var ws in wb.Worksheets)
                {
                    ws.Columns().AdjustToContents();
                }
                //файлът се запаметява в същата папка на нашия csv файл със същоъо име, обаче със съкращение .xlsx
                var output = path.Substring(0, path.Length - 3) + "xlsx";
                wb.SaveAs(output);
                Console.WriteLine("wrote to : " + output);
                System.Threading.Thread.Sleep(500);

            }
            // разделяне на данните във файлът в отделни колони и премахване на символа "|"
            Spire.Xls.Workbook book = new Spire.Xls.Workbook();

            book.LoadFromFile(@"C:\Users\Maryo\source\repos\split\split\bin\Debug\netcoreapp3.1\CustommerData.xlsx");

            Spire.Xls.Worksheet sheet = book.Worksheets[0];

            string[] splitText = null;

            string text = null;

            for (int i = 0; i < sheet.LastRow; i++)

            {

                text = sheet.Range[i + 1, 1].Text;

                splitText = text.Split("|");

                for (int j = 0; j < splitText.Length; j++)

                {

                    sheet.Range[i + 1, 1 + j + 1].Text = splitText[j];

                }
            }
            //След като файлът бъде разделен, се създава нов такъв с име "result" и се намира в папката netcoreapp3.1
            book.SaveToFile("result.xlsx", ExcelVersion.Version2007);

        }

        public static DataTable ToDataTable(IEnumerable<dynamic> items)
        {
            var data = items.ToArray();
            if (data.Count() == 0) return null;

            var dt = new DataTable();
            foreach (var key in ((IDictionary<string, object>)data[0]).Keys)
            {
                dt.Columns.Add(key);
            }
            foreach (var d in data)
            {
                dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }

            return dt;

        }

    }

}

