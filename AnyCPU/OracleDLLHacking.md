Hacking the Oracle Libraries
============================

Disassemble the Oracle.DataAccess library
-----------------------------------------

Set Path to include ildasm.exe, PowerShell style (note other SDKs probably work just fine too):

$env:path = $env:path + ';' + 'C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\x64'

Disassemble each library: 

ildasm /out:Oracle.DataAccess.il /all /nobar Oracle.DataAccess.dll
	
Modify the IL
-------------

Search for ".publickey = (", ".hash algorithm 0x00008004", or maybe ".ver" (the last one has several false positives, though.

Change the version so a) 64 bit and 32 bit versions are the same, and b) it's obvious that the DLLs are no longer the same as before. I chose 100.0.0.0

Strip out the .publickey and hashalgorithm directives as well. This will remove the strong name, but we've invalidated the key anyway.

Lastly, search for "CheckVersionCompatibility". Two opcodes after this there should be a brfalse.s instruction. Switch this to brtrue.s. This prevents the exception from being thrown due to an "incompatible version". Make sure your versions really are compatible first, though!
	
Reassemble the IL
-----------------

Set Path to include ilasm.exe, PowerShell style:

$env:path = $env.path + ';' + 'C:\Windows\Microsoft.NET\Framework\v4.0.30319'

Reassemble (x86): /Flags=11 will set the 32 bit required flag
ilasm /dll /flags=11 Oracle.DataAccess.il

Reassemble (x64):
ilasm /dll /x64 Oracle.DataAccess.il

Remove intermediate files
-------------------------

rm *.il;rm *.res;rm *.bmp;rm *.resources; rm *.xml; rm *.ssdl; rm *.msl

Copy in the native libraries and OraOps11w.dll
-------------------------------------------

