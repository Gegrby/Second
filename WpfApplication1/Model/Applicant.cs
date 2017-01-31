using System;
using System.Diagnostics;
using Applicants.Core;
using LinqToDB;
using LinqToDB.Mapping;


namespace WpfApplication1.Model {
  [DebuggerDisplay("{Name} {BirthDay} {EMail} {Phone} {ApplicantId}")]
  [Table(Name = "Applicants")]
  public class Applicant : BaseEntity {
    Guid _applicantId;
    string _name;
    string _birthDay;
    string _eMail;
    string _phone;

    [Column(IsPrimaryKey = true, DataType = DataType.Guid, DbType = "uniqueidentifier")]
    public Guid ApplicantId {
      get { return _applicantId; }
      set {
        if (value.Equals( _applicantId ))
          return;
        _applicantId = value;
        OnPropertyChanged();
      }
    }

    [Column(Length = 256)]
    public string Name {
      get { return _name; }
      set {
        if (value == _name)
          return;
        _name = value;
        OnPropertyChanged();
      }
    }

    [Column(Length = 100)]
    public string BirthDay {
      get { return _birthDay; }
      set {
        if (value == _birthDay)
          return;
        _birthDay = value;
        OnPropertyChanged();
      }
    }

    [Column(Length = 256)]
    public string EMail {
      get { return _eMail; }
      set {
        if (value == _eMail)
          return;
        _eMail = value;
        OnPropertyChanged();
      }
    }

    [Column(Length = 100)]
    public string Phone {
      get { return _phone; }
      set {
        if (value == _phone)
          return;
        _phone = value;
        OnPropertyChanged();
      }
    }
  }
}