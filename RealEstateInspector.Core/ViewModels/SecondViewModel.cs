using RealEstateInspector.Shared.Entities;

namespace RealEstateInspector.Core.ViewModels
{
    public class SecondViewModel : BaseViewModel
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
                
    }
}