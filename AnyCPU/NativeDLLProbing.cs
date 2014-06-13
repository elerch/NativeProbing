/*
    Thanks to: 

    * (Adjust search path for native DLLs) http://stackoverflow.com/a/10881132/113225
    * (Custom assembly resolver for .NET) http://stackoverflow.com/a/9951658/113225
    * (Decoration pattern for .NET exes) http://lostechies.com/gabrielschenker/2009/10/21/force-net-application-to-run-in-32bit-process-on-64bit-os/

    Compiling:

    set path=%path%;c:\windows\Microsoft.Net\Framework64\v4.0.30319

    or 

    $env:Path=$env:Path + ";c:\windows\Microsoft.Net\Framework64\v4.0.30319"

    Then:

    csc.exe NativeDllProbing.cs
*/
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace NativeDLLProbing
{
    class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        static int Main(string[] args)
        {
            AddPlatformDependentProbing(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            var assemblyName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), args[0]);
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

        public static void AddPlatformDependentProbing(string directory)
        {
            string arch;
            // determine processor architecture for this process
            switch (IntPtr.Size) {
                case 4:
                    arch = "x86";
                    break;
                case 8:
                    arch = "amd64";
                    break;
                default:
                    throw new InvalidOperationException("Unkown architecture");
            }
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) => Resolver(s, a, directory, arch);
            SetDllDirectory(Path.Combine(directory, arch)); // Set native library loading
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args, string directory, string arch)
        {
            string assemblyName = args.Name.Split(new[] {','}, 2)[0] + ".dll";
            string archSpecificPath = Path.Combine(directory,
                                                   arch,
                                                   assemblyName);

            return File.Exists(archSpecificPath)
                       ? Assembly.LoadFile(archSpecificPath)
                       : null;
        }
    }
}