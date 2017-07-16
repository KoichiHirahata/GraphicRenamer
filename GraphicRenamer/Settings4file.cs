using System;

namespace GraphicRenamer
{
    [Serializable()]
    public class Settings4file
    {
        public string imgDir { get; set; }
        public Boolean openFolderButtonVisible { get; set; } //Property for PtGraViewer
        public Boolean useDB { get; set; }
        public string DBSrvIP { get; set; } //IP address of DB server
        public string DBSrvPort { get; set; } //Port number of DB server
        public string DBconnectID { get; set; } //ID of DB user
        public string DBconnectPw { get; set; } //Pw of DB user
        public Boolean usePlugin { get; set; }
        public string ptInfoPlugin { get; set; }
    }
}