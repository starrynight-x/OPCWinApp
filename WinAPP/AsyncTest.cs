using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using CefSharp;


namespace WinAPP
{
    public class AsyncTest
    {
        private Random Rd = new Random();
        public void DoSomething(IJavascriptCallback Success, IJavascriptCallback Error)
        {
            if (Rd.Next() % 2 == 0)
            {
                Thread.Sleep(2000);
                Success.ExecuteAsync();
            }
            else
            {
                Thread.Sleep(2000);
                Error.ExecuteAsync("1", "2", "3");
            }
        }
    }
}
