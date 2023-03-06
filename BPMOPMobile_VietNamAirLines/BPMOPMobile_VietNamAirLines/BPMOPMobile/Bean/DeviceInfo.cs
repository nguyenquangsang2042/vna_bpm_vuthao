namespace BPMOPMobile.Bean
{
    public class DeviceInfo
    {
        public string DeviceId { get; set; }
        public string DevicePushToken { get; set; }
        public string DeviceName { get; set; } // Tên thiết bị
        public short DeviceOS { get; set; } // 1: Android   2: IOS  4: WindowPhone
        public string DeviceModel { get; set; } // loai thiet bi , IPhong X v.v.v
        public string AppVersion { get; set; }//version app
        public string DeviceOSVersion { get; set; } //version system
    }
}
