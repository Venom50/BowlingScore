using System.Windows.Forms;

namespace BowlingScore
{
    public static class MessageBoxHelper
    {
        internal static void ErrorMessageBox(string message)
        {
            var caption = "Error";
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal static void InfoMessageBox(string message)
        {
            var caption = "Information";
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
