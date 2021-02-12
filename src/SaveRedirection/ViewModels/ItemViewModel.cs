using SaveRedirection.ViewModels.Commands;

namespace SaveRedirection.ViewModels
{
    public class ItemViewModel
    {
        public ItemCommand RemoveItemCommand { get; private set; }
        public ItemCommand EditItemCommand { get; private set; }

        public ItemViewModel()
        {
            RemoveItemCommand = new ItemCommand(RemoveItem);
            EditItemCommand = new ItemCommand(EditItem);
        }

        public static void RemoveItem(Redirection redirectionToRemove)
        {
            Redirector.Straighten(redirectionToRemove);
            SettingsLoader.Instance.Settings.redirections.Remove(redirectionToRemove);
        }

        public static void EditItem(Redirection redirectionToEdit)
        {
            EditRedirection Window = new EditRedirection(redirectionToEdit);
            Window.Show();
        }
    }
}
