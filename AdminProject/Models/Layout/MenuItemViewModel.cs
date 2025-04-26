namespace AdminProject.Models.Layout;

public class MenuItemViewModel
{
    public string Title { get; set; }  // Menü başlığı
    public string Url { get; set; }  // Link
    public string Icon { get; set; }  // İkon (Opsiyonel)
    public List<MenuItemViewModel> SubMenus { get; set; }  // Alt menüler
    public string RequiredRole { get; set; }  // Görünmesi için gereken rol

    public MenuItemViewModel()
    {
        SubMenus = new List<MenuItemViewModel>();
    }
}
