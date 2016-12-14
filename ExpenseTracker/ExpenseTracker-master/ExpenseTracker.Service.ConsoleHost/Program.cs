using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Service.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var expensesHost = new ServiceHost(typeof(ExpenseTracker.Services.ExpenseTrackerService));
            expensesHost.Open();
            Console.WriteLine("serviceStarted");
            Console.ReadKey();
            expensesHost.Close();
        }
    }
}
