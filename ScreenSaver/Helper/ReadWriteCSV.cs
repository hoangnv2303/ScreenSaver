using CsvHelper;
using ScreenSaver.Controllers;
using ScreenSaver.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ScreenSaver.Helper
{
    public class ReadWriteCSV : BaseController
    {
        public static List<ImageModel> ReadCSV(string pathCSV)
        {
            List<ImageModel> imagesList = new List<ImageModel>();
            try
            {
                using (var reader = new StreamReader(pathCSV))
                using (var csv = new CsvReader(reader))
                {
                    var records = csv.GetRecords<ImageModel>();
                    imagesList = records.Cast<ImageModel>().ToList();
                }
                return imagesList;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public static void WriteCSV(List<ImageModel> listImages)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(pathCSV, false, System.Text.Encoding.UTF8))
                {
                    var csv = new CsvWriter(writer);
                    //csv.Configuration.Delimiter = ";";
                    //csv.Configuration.HasHeaderRecord = true;
                    //csv.Configuration.AutoMap<ImageModel>();
                    //csv.WriteHeader<ImageModel>();
                    //writer.Flush();
                    csv.WriteRecords(listImages); // where values implements IEnumerable
                }
            }
            catch
            {
                throw;
            }
        }
    }
}