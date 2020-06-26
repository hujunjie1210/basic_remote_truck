using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RCComm;

namespace basic_remote_truck
{
    
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new mainform());
            }

            catch(System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
         
        }
    }
}
