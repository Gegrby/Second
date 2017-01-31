using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Win32;
using WpfApplication1.Dal;
using WpfApplication1.Model;
using WpfApplication1.ViewModel;

namespace WpfApplication1.View {
  public partial class ApplicantsView : Window {
    readonly ApplicantsViewModel _viewModel = new ApplicantsViewModel();
    bool _importing;
    const string _errorUpdate = "При обновлении записи возникла ошибка: {0}";
    const string _errorClear = "При удалении всех записей возникла ошибка: {0}";
    const string _erroradding = "При добавлении новой записи возникла ошибка: {0}";
    const string _errorDelete = "При удалении записи возникла ошибка: {0}";
    const string _messageClear = "Удалить все записи?";
    const string _errorExport = "При экспорте дынных возникла ошибка: {0}";
    const string _errorLoading = "При загрузке DB возникла ошибка: {0}";
    const string _erorImport = "При импорте данных возникла ошибка: {0}";
    const string _errorImportFile = "При импорте файла\n{0}\nвозникла ошибка: {1}";
    const string _fileFilter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

    public ApplicantsView() {
      InitializeComponent();
      Loaded += MainWindow_Loaded;
      CommandBindings.AddRange( new[] {
        new CommandBinding( _viewModel.ImportCommand = new RoutedCommand(), ImportCommandExecuted, ImportCommandCanExecute ),
        new CommandBinding( _viewModel.AddCommand = new RoutedCommand(), AddCommandExecuted, AddCommandCanExecute ),
        new CommandBinding( _viewModel.DeleteCommand = new RoutedCommand(), DeleteCommandExecuted, DeleteCommandCanExecute ),
        new CommandBinding( _viewModel.ClearCommand = new RoutedCommand(), ClearCommandExecuted, ClearCommandCanExecute ),
        new CommandBinding( _viewModel.ExportCommand = new RoutedCommand(), ExportCommandExecuted ),
      } );
    }

    void MainWindow_Loaded( object sender, RoutedEventArgs e ) {
      try {
        DataConnection.TraceSwitch.Level = TraceLevel.Verbose;
        ReloadData();
        DataContext = _viewModel;
      } catch (Exception ex) {
        MessageBox.Show( string.Format( _errorLoading, ex ), Title );
      }
    }

    void ImportCommandExecuted( object sender, ExecutedRoutedEventArgs e ) {
      var dlg = new OpenFileDialog() { Filter = _fileFilter, Multiselect = true, CheckFileExists = true };
      if (dlg.ShowDialog() != true)
        return;

      Progress.Visibility = Visibility.Visible;
      _importing = true;

      // Разбивать загрузку файлов на различные таски нет смысла, так-как это очень небольшие задачи и они не сопоставимы
      // по времени выполнения со вставкой данных в БД средствами EF. Поэтому создаю только один поток.
      var task = Task.Run( () => DoImport( dlg.FileNames ) );
      
      // По его окончании 
      task.ContinueWith( t => {
        Dispatcher.Invoke( () => {
          // в основном потоке обновить данные и скрыть прогрессбар
          ReloadData();
          Progress.Visibility = Visibility.Hidden;
          _importing = false;
        } );
      } );
    }

    void DoImport( string[] fileNames ) {
      var list = new List<IEnumerable<Applicant>>();
      foreach (var fileName in fileNames)
        try {
          list.Add( CsvHelper.ImportCsvFile( fileName ) );
        } catch (Exception ex) {
          Dispatcher.Invoke( () => MessageBox.Show( string.Format( _errorImportFile, fileName, ex ), Title ) );
        }

      var db = new ApplicantsDbContext();
      try {
        foreach (var l in list)
          foreach (var i in l)
            db.Insert( i );
      } catch(Exception ex) {
        Dispatcher.Invoke( () => MessageBox.Show( string.Format( _erorImport, ex ), Title ) );
      }
    }

    void ReloadData() {
      var db = new ApplicantsDbContext();
      if (_viewModel.Applicants != null)
        _viewModel.Applicants.ListChanged -= ApplicantsOnCollectionChanged;
      _viewModel.Applicants = new BindingList<Applicant>( db.Applicants.Select( c => c ).ToList() );
      _viewModel.Applicants.ListChanged += ApplicantsOnCollectionChanged;
    }

    void ApplicantsOnCollectionChanged( object sender, ListChangedEventArgs e ) {
      if (e.ListChangedType == ListChangedType.ItemChanged) {
        var newa = _viewModel.Applicants[ e.NewIndex ];
        var db = new ApplicantsDbContext();
        try {
          db.Update( newa );
        } catch(Exception ex) {
          MessageBox.Show( string.Format( _errorUpdate, ex ), Title );
        }
      }
    }

    void ClearCommandExecuted( object sender, ExecutedRoutedEventArgs e ) {
      var res = MessageBox.Show( _messageClear, Title, MessageBoxButton.YesNo, MessageBoxImage.Question );
      if (res != MessageBoxResult.Yes)
        return;

      var db = new ApplicantsDbContext();
      try {
        db.Applicants.Delete();
      } catch(Exception ex) {
        MessageBox.Show( string.Format( _errorClear, ex ), Title );
      } finally {
        ReloadData();
      }
    }

    void AddCommandExecuted( object sender, ExecutedRoutedEventArgs e ) {
      var db = new ApplicantsDbContext();
      var n = new Applicant() { ApplicantId = Guid.NewGuid() };
      try {
        db.Insert( n );
      } catch(Exception ex) {
        MessageBox.Show( string.Format( _erroradding, ex ), Title );
      } finally {
        ReloadData();
        _viewModel.Current = _viewModel.Applicants.FirstOrDefault( a => a.ApplicantId == n.ApplicantId );
      }
    }

    void DeleteCommandExecuted( object sender, ExecutedRoutedEventArgs e ) {
      var a = _viewModel.Current;
      if (a == null)
        return;
      var db = new ApplicantsDbContext();
      try {
        db.Delete( a );
      } catch(Exception ex) {
        MessageBox.Show( string.Format( _errorDelete, ex ), Title );
      } finally {
        ReloadData();
      }
    }

    void ExportCommandExecuted( object sender, ExecutedRoutedEventArgs e ) {
      var dlg = new SaveFileDialog() { Filter = _fileFilter };
      if (dlg.ShowDialog() != true)
        return;
      try {
        CsvHelper.ExportToCsvFile( dlg.FileName, _viewModel.Applicants, Encoding.Default );
      } catch(Exception ex) {
        MessageBox.Show( string.Format( _errorExport, ex ), Title );
      }
    }

    void ImportCommandCanExecute( object sender, CanExecuteRoutedEventArgs e ) {
      e.CanExecute = _importing == false;
    }

    void AddCommandCanExecute( object sender, CanExecuteRoutedEventArgs e ) {
      e.CanExecute = _importing == false;
    }

    void ClearCommandCanExecute( object sender, CanExecuteRoutedEventArgs e ) {
      e.CanExecute = _importing == false;
    }

    void DeleteCommandCanExecute( object sender, CanExecuteRoutedEventArgs e ) {
      e.CanExecute = _importing == false && _viewModel.Current != null;
    }
  }
}
