using System.Drawing;
using System.Windows.Forms;
using GTAVStudio.Common;

namespace GTAVStudio.Theme
{
    public class ThemeColorTable : ProfessionalColorTable
    {
        private static Color _menuBorder = Color.Indigo;
        private static Color _menuItemSelected = Color.Indigo;
        private static Color _menuItemBorder = Color.Indigo;

        public override Color MenuItemSelected => _menuItemSelected;

        public override Color MenuBorder => _menuBorder;

        public override Color MenuItemBorder => _menuItemBorder;

        public static void Reload()
        {
            _menuItemBorder = StudioSettings.GetValue(Constants.Settings.Theme, "Menu_Item_Border", _menuItemSelected); 
            _menuItemSelected = StudioSettings.GetValue(Constants.Settings.Theme, "Menu_Item_Selected", _menuItemSelected);
            _menuBorder = StudioSettings.GetValue(Constants.Settings.Theme, "Menu_Border", _menuBorder);
        }
    }
}