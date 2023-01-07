using Microsoft.Win32;

namespace SACG.sysSacg.Helpers
{
    public static class RegistroWindowsHelper
    {

        private static string path = "SOFTWARE\\Wow6432Node\\SACG";


        public static RegistroWindows GetDatosRegistro()
        {
            RegistroWindows registroWindows = new RegistroWindows();
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@path);
            if(key != null)
            {
                registroWindows.Provider = key.GetValue("Provider").ToString();
                registroWindows.SQLUser = key.GetValue("UserID").ToString();
                registroWindows.SQLPassword = key.GetValue("Password").ToString();
                registroWindows.PersistSecurityInfo = bool.Parse(key.GetValue("PersistSecurityInfo").ToString());
                registroWindows.InitialCatalog = key.GetValue("InitialCatalog").ToString();
                registroWindows.DataSource = key.GetValue("DataSource").ToString();
            }
            return registroWindows;

        }


    }

    public class RegistroWindows
    {

        public string Provider{ get; set; }
        public string SQLUser{ get; set; }
        public string SQLPassword{ get; set; }
        public bool PersistSecurityInfo { get; set; }
        public string InitialCatalog { get; set; }
        public string DataSource { get; set; }

        public RegistroWindows() { }

        //"Provider"="SQLOLEDB.1"
        //"UserID"="sa"
        //"Password"="5B0704070256"
        //"PersistSecurityInfo"="True"
        //"InitialCatalog"="SACGSYS"
        //"DataSource"="."


    }
}
