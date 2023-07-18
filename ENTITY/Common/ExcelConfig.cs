using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;

namespace Entity.Common
{
    public static class ExcelConfig
    {
        public static List<double> GetOLEDBVersion()
        {
            OleDbEnumerator enumerator = new OleDbEnumerator();
            DataTable table = enumerator.GetElements();
            var OLEDBVersion = string.Empty;
            var OLEDBs = table.Select().ToList().Select(
                                                        x => x["SOURCES_NAME"].ToString()
                                                        ).ToList().Where(x =>
                                                        x.Contains("Microsoft.ACE.OLEDB")
                                                        ).ToList();
            List<double> version = new List<double>();
            for (var i = 0; i < OLEDBs.Count; i++)
            {
                OLEDBVersion = OLEDBs[i].ToString();
                Double.TryParse(Regex.Match(OLEDBVersion, @"\d+\.*\d*").Value, out double ver);
                version.Add(ver);
            }



            return version;
        }
    }
}
