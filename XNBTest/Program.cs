using System;
using System.IO;
using XNB;

namespace XNBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            XnbFile file = XnbFile.Load(new FileStream("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Stardew Valley\\Content\\Maps\\spring_outdoorsTileSheet.xnb", FileMode.Open));
            Console.WriteLine("DATA:"+file.Data);
        }
    }
}
