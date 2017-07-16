using System;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace GraphicRenamer
{
    public class Settings
    {
        public static string imgDir { get; set; }
        public static Boolean openFolderButtonVisible { get; set; }
        public static Boolean useFeDB { get; set; }
        public static string DBSrvIP { get; set; } //IP address of DB server
        public static string DBSrvPort { get; set; } //Port number of DB server
        public static string DBconnectID { get; set; } //ID of DB user
        public static string DBconnectPw { get; set; } //Pw of DB user
        public static string settingFile_location { get; set; } //Config file path
        public static string lang { get; set; } //language
        public static string DBname { get; set; }
        public static string sslSetting { get; set; } //SSL setting string
        public static Boolean usePlugin { get; set; }
        public static string ptInfoPlugin { get; set; } //File location of the plug-in to get patient information

        public static void initiateSettings()
        {
            settingFile_location = Application.StartupPath + "\\settings.config";
            readSettings();
            lang = Application.CurrentCulture.TwoLetterISOLanguageName;
            DBname = "endoDB";
            //Settings.sslSetting = ""; //Use this when you want to connect without using SSL
            sslSetting = "SSL Mode=Require;Trust Server Certificate=true"; //Use this when you want to connect using SSL
            ptInfoPlugin = checkPtInfoPlugin();

            if (useFeDB)
            {
                if (!db.testConnect(DBSrvIP, DBSrvPort, DBconnectID, DBconnectPw))
                {
                    MessageBox.Show(Properties.Resources.CouldntOpenConn, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    useFeDB = false;
                }
            }
        }

        public static void saveSettings()
        {
            Settings4file st = new Settings4file();
            st.imgDir = imgDir;
            st.openFolderButtonVisible = openFolderButtonVisible;
            st.useDB = useFeDB;
            if (useFeDB)
            {
                st.DBSrvIP = DBSrvIP;
                st.DBSrvPort = DBSrvPort;
                st.DBconnectID = DBconnectID;
                st.DBconnectPw = (String.IsNullOrWhiteSpace(DBconnectPw)) ? "" : PasswordEncoder.Encrypt(DBconnectPw);
            }
            else
            {
                st.DBSrvIP = "";
                st.DBSrvPort = "";
                st.DBconnectID = "";
                st.DBconnectPw = "";
            }
            st.usePlugin = usePlugin;
            st.ptInfoPlugin = ptInfoPlugin;

            XmlSerializer xserializer = new XmlSerializer(typeof(Settings4file));
            //Open file
            FileStream fs1 = new FileStream(settingFile_location, FileMode.Create);
            xserializer.Serialize(fs1, st);
            fs1.Close();

            #region Save to plugins.ini
            if (usePlugin && !String.IsNullOrWhiteSpace(ptInfoPlugin))
            {
                string text = "Patient information=" + ptInfoPlugin + "\r\n";
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\plugins.ini", false);
                sw.Write(text);
                sw.Close();
            }
            #endregion
        }

        //Read from file
        public static void readSettings()
        {
            if (File.Exists(settingFile_location))
            {
                Settings4file st = new Settings4file();

                XmlSerializer xserializer = new XmlSerializer(typeof(Settings4file));
                FileStream fs2 = new FileStream(settingFile_location, FileMode.Open);
                try
                {
                    st = (Settings4file)xserializer.Deserialize(fs2);
                    fs2.Close();
                }
                catch (InvalidOperationException)
                {
                    DialogResult ret;
                    ret = MessageBox.Show(Properties.Resources.SettingFileBroken, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    fs2.Close();
                    if (ret == DialogResult.Yes)
                    { file_control.delFile(Settings.settingFile_location); }
                }

                imgDir = st.imgDir;
                openFolderButtonVisible = st.openFolderButtonVisible;
                useFeDB = st.useDB;
                usePlugin = st.usePlugin;
                DBSrvIP = st.DBSrvIP;
                DBSrvPort = st.DBSrvPort;
                DBconnectID = st.DBconnectID;
                DBconnectPw = PasswordEncoder.Decrypt(st.DBconnectPw);
            }
        }

        public static string checkPtInfoPlugin()
        {
            if (File.Exists(Application.StartupPath + "\\plugins.ini"))
            {
                string text = file_control.readFromFile(Application.StartupPath + "\\plugins.ini");
                string plugin_location = file_control.readItemSettingFromText(text, "Patient information=");
                if (File.Exists(plugin_location))
                { return plugin_location; }
                else
                { return ""; }
            }
            else
            { return ""; }
        }
    }
}