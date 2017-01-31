using System;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApplication1.Model;

namespace WpfApplication1.View {
  public class ApplicantValidationRule : ValidationRule {
    static readonly string[] _formats = {
      "dd/MM/yyyy",
      "dd-MM-yyyy",
      "dd/MMM/yyyy",
      "dd-MMM-yyyy",
      "dd MMMM yyyy",
      "M/d/yyyy",
      "MM/dd/yyyy",
      "M/dd/yyyy"
    };

    public override ValidationResult Validate( object value, CultureInfo cultureInfo ) {
      var bindingGroup = value as BindingGroup;
      if (bindingGroup == null || bindingGroup.Items.Count == 0)
        return new ValidationResult( true, null );
      var applicant = bindingGroup.Items[ 0 ] as Applicant;
      if (applicant == null)
        return new ValidationResult( true, null );

      StringBuilder sb = null;
      if (string.IsNullOrWhiteSpace( applicant.Name )) {
        sb = sb == null ? new StringBuilder() : sb.AppendLine();
        sb.Append( "��� ������ ���� �������." );
      } else {
        var sl = applicant.Name.Split( ' ' );
        if (sl.Length < 2) {
          sb = sb == null ? new StringBuilder() : sb.AppendLine();
          sb.Append( "����� ������� �������, ��� � �������� ����� ������" );
        }
      }

      // ���� ��������
      if (string.IsNullOrWhiteSpace( applicant.BirthDay )) {
        sb = sb == null ? new StringBuilder() : sb.AppendLine();
        sb.Append( "���� �������� ������ ���� �������." );
      } else {
        DateTime date;
        if (DateTime.TryParse( applicant.BirthDay, out date ) == false)
          if (DateTime.TryParse( applicant.BirthDay, CultureInfo.InvariantCulture, DateTimeStyles.None, out date ) == false)
            if (DateTime.TryParseExact( applicant.BirthDay, _formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out date )) {
              sb = sb == null ? new StringBuilder() : sb.AppendLine();
              sb.Append( "��������� ������� ���������� ���� ��������" );
            }
      }

      // EMail
      if (string.IsNullOrWhiteSpace( applicant.EMail )) {
        sb = sb == null ? new StringBuilder() : sb.AppendLine();
        sb.Append( "����� ����������� ����� ������ ���� ������." );
      } else {
        try {
          var m = new MailAddress( applicant.EMail );
        } catch (FormatException) {
          sb = sb == null ? new StringBuilder() : sb.AppendLine();
          sb.Append( "��������� ���������� ����� ����������� �����." );
        }      
      }

      // Phone
      if (string.IsNullOrWhiteSpace( applicant.Phone )) {
        sb = sb == null ? new StringBuilder() : sb.AppendLine();
        sb.Append( "������� ������ ���� ������." );
      } else {
        // � ��������� �� �����, ����� �����  http://www.regexlib.com/Search.aspx?k=phone&AspxAutoDetectCookieSupport=1
        var r = new Regex(@"^\+?(\+[1-9][0-9]*(\([0-9]*\)|-[0-9]*-))?[0]?[1-9][0-9\- ]*$");
        if (r.IsMatch( applicant.Phone ) == false) {
          sb = sb == null ? new StringBuilder() : sb.AppendLine();
          sb.Append( "��������� ���������� ����� ��������. ������: +7 900 111 22 33 ��� 89001112233" );
        }
      }

      if (sb != null)
        return new ValidationResult( false, sb.ToString() );
      return ValidationResult.ValidResult;
    }
  }
}