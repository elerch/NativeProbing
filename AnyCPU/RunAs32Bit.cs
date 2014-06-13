/*
    Thanks to: http://lostechies.com/gabrielschenker/2009/10/21/force-net-application-to-run-in-32bit-process-on-64bit-os/

    Compiling:

    set path=%path%;c:\windows\Microsoft.Net\Framework64\v4.0.30319

    or 

    $env:Path=$env:Path + ";c:\windows\Microsoft.Net\Framework64\v4.0.30319"

    Then:

    csc.exe /out:RunAs32Bit.exe /platform:x86 RunAs32Bit.cs
*/
using System;
using System.Reflection;
using System.IO;
using System.Linq;
 
namespace RunAs32Bit
{
    class Program
    {
        static int Main(string[] args)
        {
            // load the assembly
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var assemblyName = Path.Combine(directory, args[0]);
            var assembly = Assembly.LoadFile(assemblyName);
         
            // find the entry point of the assembly
            var type = assembly.GetTypes().FirstOrDefault(t => t.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance) != null);
            var mi = type.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var parmsCount = mi.GetParameters().Count();
            // call the entry point of the wrapped assembly and forward the command line parameters
            var list = Enumerable.Range(0, parmsCount).Select(i => (object)null).ToArray();
            if (args.Length > 1) {
                var arguments = args[1].Split(' ');
                list = new object[] { arguments.Where(a => a.Trim() != "").ToArray() };
            }
            return (int)(mi.Invoke(type, parmsCount > 0 ? list : null) ?? 0);
        }
    }
}