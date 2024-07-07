using System;
using System.Windows.Forms;

namespace Crop_To_Search
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 mainForm = new Form1();
            Form2 backdropForm = new Form2();

            

            mainForm.Owner = backdropForm;
            backdropForm.Show();
            mainForm.Show();

            Application.Run(mainForm);
        }
    }
}
