using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Applicants.Core;

namespace WpfApplication1.Model {
  public static class CsvHelper {
    public static List<Applicant> ImportCsvFile( string fileName ) {
      List<Applicant> list;
      using (var file = File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.Read )) {
        var reader = new StreamReader( file, Encoding.Default );
        list = new List<Applicant>();

        while (reader.EndOfStream == false) {
          var line = reader.ReadLine();
          if (string.IsNullOrWhiteSpace( line ))
            continue;
          var sl = line.Split( '\t' );
          if (sl.Length != 4) {
            continue;
          }
          var a = new Applicant() {
            ApplicantId = Guid.NewGuid(),
            Name = sl[ 0 ],
            BirthDay = sl[ 1 ],
            EMail = sl[ 2 ],
            Phone = sl[ 3 ]
          };
          list.Add( a );
        }
      }
      return list;
    }

    public static void ExportToCsvFile( string fileName, IList<Applicant> applicants, Encoding encoding ) {
      if (applicants == null || applicants.Count == 0)
        return;
      using (var f = File.Open( fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read ))
      using (var stream = new StreamWriter( f, encoding ))
        foreach (var applicant in applicants)
          stream.Write( "{0}\t{1}\t{2}\t{3}\n", applicant.Name, applicant.BirthDay, applicant.EMail, applicant.Phone );
    }
  }
}