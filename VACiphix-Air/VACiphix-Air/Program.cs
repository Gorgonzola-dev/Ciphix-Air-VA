using System;
using System.Threading.Tasks;

namespace VACiphix_Air
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while(true)
            {
                await ListenToDialogFlow();
            }
        }
    }
}
