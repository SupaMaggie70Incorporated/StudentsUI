using System.Threading;

namespace StudentsUI
{
    internal static class Program
    {
        public static Form1 form;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            form = new Form1();
            Application.Run(form);
        }
    }
}