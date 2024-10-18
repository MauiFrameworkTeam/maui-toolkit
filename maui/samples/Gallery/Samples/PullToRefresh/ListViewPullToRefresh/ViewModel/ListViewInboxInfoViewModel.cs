﻿
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Syncfusion.Maui.ControlsGallery.PullToRefresh;

/// <summary>
/// ViewModel class of ListViewPullToRefresh.
/// </summary>
public class ListViewInboxInfoViewModel : INotifyPropertyChanged
{
    #region Fields

    ObservableCollection<InboxInfo>? inboxInfos;
    Random random;

    #endregion

    #region Interface Member

    /// <summary>
    /// Represents the method that will handle the <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"></see> event raised when a property is changed on a component
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Triggers when Items Collections Changed.
    /// </summary>
    /// <param name="name">string type parameter represent propertyName as name</param>
    public void OnPropertyChanged(string name)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initiates new intance of ListViewInboxInfoViewModel class.
    /// </summary>
    public ListViewInboxInfoViewModel()
    {
        this.random = new Random();
        GenerateSource();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the InboxInfo type of ObservableCollection and notifies user when collection value gets changed.
    /// </summary>
    public ObservableCollection<InboxInfo>? InboxInfos
    {
        get { return inboxInfos; }
        set { inboxInfos = value; OnPropertyChanged("InboxInfos"); }
    }

    #endregion

    #region Generate Source

    /// <summary>
    /// Initiates Commands, Repository and Collections. Also generates items for the collections.
    /// </summary>
    private void GenerateSource()
    {
        ListViewInboxInfoRepository inboxinfo = new ListViewInboxInfoRepository();
        inboxInfos = inboxinfo.GetInboxInfo();
    }

    /// <summary>
    /// This method helps to add messages while refreshing.
    /// </summary>
    /// <param name="count">Represent number of messages to be added.</param>
    public void AddItemsRefresh(int count)
    {
        ListViewInboxInfoRepository inboxinfo = new ListViewInboxInfoRepository();
        
        foreach (var i in inboxinfo.AddRefreshItems(count))
        {
            this.inboxInfos?.Insert(0, i);
        }
    }

    #endregion
}

