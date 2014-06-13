/*
    Compiling:

    set path=%path%;c:\windows\Microsoft.Net\Framework64\v4.0.30319

    or 

    $env:Path=$env:Path + ";c:\windows\Microsoft.Net\Framework64\v4.0.30319"

    Then:

    csc.exe /r:64bit\Oracle.DataAccess.dll /out:64bit\Test.exe /platform:x64 Test.cs
    csc.exe /r:32bit\Oracle.DataAccess.dll /out:32bit\Test.exe /platform:x86 Test.cs /nowarn:1607

    The warning disable in the 32 bit version is due to compiling 32 bit with the 64 bit compiler.
    The warnings all involve CLR assemblies which all have both 32 and 64 bit versions, so it's safe to ignore
*/
using Oracle.DataAccess.Client;
using System;
 
namespace NoOraClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Traditional TNS Definition
            ORA11G =
  (DESCRIPTION =
    (ADDRESS = (PROTOCOL = TCP)(HOST = ora11g-win.windsor.com)(PORT = 1521))
    (CONNECT_DATA =
      (SERVER = DEDICATED)
      (SID = ora11g)
    )
  )

  .Net Connection String information:
  "user id=NV_NSITE_DEV;password=memorial;data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=ora11g-win.windsor.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ora11g)))"

  Easy Connect Naming version (standard):
  "User Id=NV_NSITE_DEV;password=memorial;Data Source=host[port - default 1521][/servicename]"

  Easy Connect Naming version (dedicated server):
  "User Id=NV_NSITE_DEV;password=memorial;Data Source=ora11g-win.windsor.com/ora11g:dedicated"

            */
            string connectionString = "user id=NV_NSITE_DEV;password=memorial;data source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=ora11g-win.windsor.com)(PORT=1521))(CONNECT_DATA=(SERVICER=DEDICATED)(SID=ora11g)))";            
            //string connectionString = "Data Source=NV_NSITE_DEV/memorial@ora11g-win.windsor.com:1521/ora11g";

            Console.WriteLine("Connecting with System.Data.OracleClient");
#pragma warning disable 618
            using (var connection = new System.Data.OracleClient.OracleConnection(connectionString))
#pragma warning restore 618            
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection Successful using System.Data.OracleClient (deprecated)");
                    connection.Close();
                }
                catch (OracleException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }  

            Console.WriteLine("Connecting with Oracle.DataAccess.Client.OracleConnection");
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection Successful using Oracle.DataAccess");
                    connection.Close();
                }
                catch (OracleException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }            
        }
    }
}