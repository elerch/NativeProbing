Probing support for architecture-specific native DLLs
=====================================================

Architecture-specific .NET DLL Probing
--------------------------------------

The Fusion process does not support bin-deployed architecture specific
DLLs loaded at runtime. If you add a probing element it appears to work if
your architecture is listed first:

    <probing privatePath="amd64;x86"/>

Will work if you're on a 64 bit OS and will fail on a 32 bit OS. 
Similarly, switching the order will succeed on a 32 bit OS and fail on a 64 bit OS.

The only way to solve this problem appears to be to hook into .NET's assembly
resolution process using a technique similar to the following:

[http://stackoverflow.com/a/9951658/113225](http://stackoverflow.com/a/9951658/113225)

Architecture-specific native DLL Probing
----------------------------------------

Neither the probing tag nor the AssemblyResolve event will, however, alter the
native DLL discovery process documented on MSDN: [http://msdn.microsoft.com/en-us/library/windows/desktop/ms682586(v=vs.85).aspx](http://msdn.microsoft.com/en-us/library/windows/desktop/ms682586(v=vs.85).aspx)

To address this, you need to call SetDllDirectory, as this StackOverflow answer recommends: [http://stackoverflow.com/a/10881132/113225](http://stackoverflow.com/a/10881132/113225)

You can also alter the path environment variable, but that's a bit like using a sledgehammer rather than a tack hammer. SetDllDirectory is a Windows API in kernel32.dll callable through C# through P/Invoke: [http://www.pinvoke.net/default.aspx/kernel32.setdlldirectory](http://www.pinvoke.net/default.aspx/kernel32.setdlldirectory)

Putting it all together
-----------------------

The NativeDLLProbing.cs file in this repo puts both techniques together to enable full .NET and native DLL probing based on the current architecture used in an AnyCPU configuration. Compiled as an executable, it can be called from the command line as a wrapper:

	NativeDLLProbing.exe YourAssembly.exe [params]

If hosted elsewhere (e.g. ASP.NET), the resulting library can be referenced and called at startup (e.g. Global.asax), passing the base directory into the AddPlatformDependentProbing method.

Sample
------

The Oracle.DataAccess.dll provides an example of the use case solved by this technique. Once hacked (see OracleDLLHacking.md), there are identically versioned but architecture-specific Oracle.DataAccess.dll assemblies. These assemblies carry dependencies on their native counterparts, also hosted in amd64/x86 directories side by side with the .NET assemblies.

The Test.cs can be compiled referencing either Oracle.DataAccess.dll, and when called through: NativeDLLProbing.exe TestAnyCPU.exe the connectivity tests should pass on either a 32 bit or 64 bit OS.