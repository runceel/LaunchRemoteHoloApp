using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaunchHoloAppSample.Models
{
    public class HoloLensManager : BindableBase
    {
        private readonly Subject<Unit> _appNameFilterUpdateTrigger = new Subject<Unit>();
        private readonly Subject<Unit> _runningProcessNameFilterUpdateTrigger = new Subject<Unit>();
        public static HoloLensManager Instance { get; } = new HoloLensManager();

        private ObservableCollection<HoloLens> ManagedHoloLensSource { get; set; } = new ObservableCollection<HoloLens>();

        public ReadOnlyObservableCollection<HoloLens> ManagedHoloLens { get; }

        private ObservableCollection<HoloLensApplication> ApplicationsSource { get; } = new ObservableCollection<HoloLensApplication>();

        private ObservableCollection<HoloLensApplication> FilteredApplicationsSource { get; } = new ObservableCollection<HoloLensApplication>();

        public ReadOnlyObservableCollection<HoloLensApplication> Applications { get; }

        private ObservableCollection<RunningProcessInfo> RunningProcessInfosSource { get; } = new ObservableCollection<RunningProcessInfo>();

        public ReadOnlyObservableCollection<RunningProcessInfo> RunningProcessInfos { get; }

        private string _appNameFilter;
        public string AppNameFilter
        {
            get { return _appNameFilter; }
            set
            {
                if (SetProperty(ref _appNameFilter, value))
                {
                    _appNameFilterUpdateTrigger.OnNext(Unit.Default);
                }
            }
        }

        private string _runningProcessNameFilter;

        public string RunningProcessNameFilter
        {
            get { return _runningProcessNameFilter; }
            set
            {
                if(SetProperty(ref this._runningProcessNameFilter, value))
                {
                    _runningProcessNameFilterUpdateTrigger.OnNext(Unit.Default);
                }
            }
        }

        public HoloLensManager()
        {
            ManagedHoloLens = new ReadOnlyObservableCollection<HoloLens>(ManagedHoloLensSource);
            Applications = new ReadOnlyObservableCollection<HoloLensApplication>(FilteredApplicationsSource);
            RunningProcessInfos = new ReadOnlyObservableCollection<RunningProcessInfo>(RunningProcessInfosSource);

            _appNameFilterUpdateTrigger.Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => FilterApps());

            _runningProcessNameFilterUpdateTrigger.Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(_ => FilterRunningProcesses());
        }

        public void AddManagedHoloLens(HoloLens holoLens) => ManagedHoloLensSource.Add(holoLens);

        public void RemoveManagedHoloLens(HoloLens holoLens) => ManagedHoloLensSource.Remove(holoLens);

        public void DeserializeFromJson(string json)
        {
            var items = JsonConvert.DeserializeObject<HoloLens[]>(json);
            ManagedHoloLensSource.Clear();
            foreach (var item in items)
            {
                AddManagedHoloLens(item);
            }
        }

        public async Task LaunchAppAsync(HoloLensApplication app)
        {
            foreach (var holoLens in ManagedHoloLens)
            {
                await holoLens.LaunchAppAsync(app);
            }
        }

        public Task ConnectAllAsync() => Task.WhenAll(ManagedHoloLens.Select(x => x.ConnectAsync()));

        public async Task UpdateAllHoloLensApplicationsAsync()
        {
            await Task.WhenAll(ManagedHoloLens.Select(x => x.UpdateApplicationsAsync()));
            var apps = ManagedHoloLens.SelectMany(x => x.Applications)
                .GroupBy(x => x.AppId)
                .Where(x => x.Count() == ManagedHoloLens.Count)
                .Select(x => x.First())
                .Select(x => new HoloLensApplication(x.AppId, x.Name, x.PackageName))
                .OrderBy(x => x.Name);
            ApplicationsSource.Clear();
            foreach (var app in apps)
            {
                ApplicationsSource.Add(app);
            }

            FilterApps();
        }

        public string SerializeToJson() => JsonConvert.SerializeObject(ManagedHoloLensSource);

        public async Task UpdateRunningProcessesInfoAsync()
        {
            var tasks = ManagedHoloLens.Select(x => x.UpdateRunningProcessesAsync()).ToArray();
            await Task.WhenAll(tasks);
            FilterRunningProcesses();
        }

        private void FilterApps()
        {
            FilteredApplicationsSource.Clear();
            var appNames = AppNameFilter?.Split(';')?.Select(x => x.Trim().ToLower()) ?? Array.Empty<string>();
            var isAll = !appNames.Any();
            foreach (var item in ApplicationsSource.Where(x => isAll ? true : appNames.Any(y => x.Name.ToLower().Contains(y))))
            {
                FilteredApplicationsSource.Add(item);
            }
        }

        private void FilterRunningProcesses()
        {
            var processNames = RunningProcessNameFilter?.Split(';')
                ?.Where(x => !string.IsNullOrEmpty(x))
                ?.Select(x => x.Trim().ToLower()) ?? Array.Empty<string>();
            var isAll = !processNames.Any();
            var processInfos = ManagedHoloLens.Select(x => new RunningProcessInfo
            {
                DeviceName = x.Name,
                RunningProcesses = x.RunningProcesses
                    .Where(y => isAll ? true : processNames.Any(z => y.Name.ToLower().Contains(z)))
                    .ToArray(),
            });

            RunningProcessInfosSource.Clear();
            foreach (var pi in processInfos)
            {
                RunningProcessInfosSource.Add(pi);
            }
        }
    }
}
