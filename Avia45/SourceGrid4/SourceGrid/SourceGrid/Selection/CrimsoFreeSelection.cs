using System.Windows.Forms;

namespace SourceGrid.Selection
{
    public class CrimsoFreeSelection : FreeSelection
    {
        public override bool CanReceiveFocus(Position position)
        {
            if (Control.ModifierKeys == Keys.Control) return false;

            return base.CanReceiveFocus(position);
        }

    }
}
