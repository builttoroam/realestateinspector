namespace BuiltToRoam.Navigation
{
    public interface INativeNavigateService<TView> : INavigateService
        where TView : class,new()
    {
        void Register<TViewModel, TViewType>() where TViewType : TView;
    }
}