
namespace Ichongli.Rosi.Tombstoning
{
    using Caliburn.Micro;
    using Ichongli.Rosi.ViewModels;
    public class ViewerPageModelStorage : StorageHandler<ViewerPageViewModel>
    {
        public override void Configure()
        {
            Id(x => x.DisplayName);

            Property(x => x.SelectItem)
                .InPhoneState()
                .RestoreAfterActivation();
        }
    }
}
