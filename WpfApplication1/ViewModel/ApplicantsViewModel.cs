using System.ComponentModel;
using System.Windows.Input;
using WpfApplication1.Model;

namespace WpfApplication1.ViewModel {
  public class ApplicantsViewModel : BaseViewModel {
    BindingList<Applicant> _applicants;
    Applicant _current;

    public ICommand ImportCommand { get; set; }
    public ICommand AddCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand ClearCommand { get; set; }
    public ICommand ExportCommand { get; set; }

    public BindingList<Applicant> Applicants {
      get { return _applicants; }
      set {
        if (_applicants == value)
          return;
        _applicants = value;
        OnPropertyChanged();
      }
    }

    public Applicant Current {
      get { return _current; }
      set {
        if (_current == value)
          return;
        _current = value;
        OnPropertyChanged();
      }
    }
  }
}