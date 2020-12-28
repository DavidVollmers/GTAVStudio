using System.Windows.Forms;

namespace GTAVStudio.Theme
{
    public class ThemeToolStripRenderer : ToolStripProfessionalRenderer
    {
        public ThemeToolStripRenderer() : base(new ThemeColorTable())
        {
        }
    }
}