/*
    Compiling:

    set path=%path%;c:\windows\Microsoft.Net\Framework64\v4.0.30319

    or 

    $env:Path=$env:Path + ";c:\windows\Microsoft.Net\Framework64\v4.0.30319"

    Then:

    csc.exe /r:amd64\Oracle.DataAccess.dll /out:TestAnyCPU.exe Test.cs

    To run on 32 bit on a 64 bit machine, you need a 32 bit process. 
    So, you need to use the RunAs32Bit.exe. At the command line:

        "RunAs32Bit TestAnyCPU.exe" from the x86 directory
    
    OR
    
        ".\NativeDLLProbing.exe RunAs32Bit.exe TestAnyCPU.exe" from the base directory

*/
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