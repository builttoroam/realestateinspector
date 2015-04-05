using System.Linq;
using System.Threading.Tasks;
using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core.ViewModels
{
    public class SecondViewModel : RealEstateBaseViewModel
    {
        private RealEstateProperty currentProperty;

        public RealEstateProperty CurrentProperty
        {
            get { return currentProperty; }
            set
            {
                if (CurrentProperty == value) return;
                currentProperty = value;
                OnPropertyChanged();
            }
        }

        public SecondViewModel()
        {
            CurrentProperty = new RealEstateProperty {Address = "21 Vincent"};
        }

        public async Task Save()
        {
            await SyncService.DataService.MobileService.GetSyncTable<RealEstateProperty>().InsertAsync(CurrentProperty);

            var prop =
                (await SyncService.DataService.MobileService.GetSyncTable<RealEstateProperty>().ToListAsync())
                    .FirstOrDefault(x => x.Id == CurrentProperty.Id);
            CurrentProperty = prop;
        }
    }
}