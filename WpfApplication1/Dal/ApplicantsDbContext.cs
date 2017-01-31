using System.IO;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlCe;
using LinqToDB.SqlQuery;
using WpfApplication1.Model;

namespace WpfApplication1.Dal {
  public class ApplicantsDbContext : DataConnection {
    const string _dbFileName = "Applicants.sdf";

    public ApplicantsDbContext() : base( SqlCeTools.GetDataProvider(), string.Format("Data Source={0}", _dbFileName) ) {
      if (File.Exists( _dbFileName ) == false) {
        SqlCeTools.CreateDatabase(_dbFileName);
        this.CreateTable<Applicant>();
      }
    }

    public ITable<Applicant> Applicants { get { return GetTable<Applicant>(); } }
  }
}