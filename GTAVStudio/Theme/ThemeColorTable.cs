using System.Drawing;
using System.Windows.Forms;
using GTAVStudio.Common;

namespace GTAVStudio.Theme
{
    public class ThemeColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected =>
            StudioSettings.GetValue(Constants.Settings.Theme, "Menu_Item_Selected", Color.Indigo);
    }
}