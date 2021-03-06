﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
//using Applicants.Core.Annotations;

namespace Applicants.Core {
  public class BaseEntity : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    //[NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
      var handler = PropertyChanged;
      if (handler != null)
        handler( this, new PropertyChangedEventArgs( propertyName ) );
    }
  }
}