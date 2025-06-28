using System;
using System.IO;
using System.Windows.Forms;

namespace Crop_To_Search
{
    static class Program
    {
        static bool HasAcceptedTerms()
        {
            return File.Exists("accepted");
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!HasAcceptedTerms())
            {
                Application.Run(new Terms_And_Conditions());
            }

            Form1 mainForm = new Form1();
            Form2 backdropForm = new Form2();
            SearchBarForm searchBarForm = new SearchBarForm();
            mainForm.Owner = backdropForm;
            backdropForm.Show();
            mainForm.Show();
            searchBarForm.Show();
            searchBarForm.Activate();


            Application.Run(searchBarForm);
        }
    }
}