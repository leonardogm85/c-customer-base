namespace CustomerBase.ApplicationService
{
    // Serviços expostos pela View.
    public interface IWindowServices
    {
        void UpdateBindings();
        void PutFocusOnForm();
        bool ConfirmDelete();
        bool ConfirmSave();
        void CloseWindow();
    }
}
