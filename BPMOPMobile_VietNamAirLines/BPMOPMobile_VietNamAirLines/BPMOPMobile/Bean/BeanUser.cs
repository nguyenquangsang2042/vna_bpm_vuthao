using System;
using SQLite;

namespace BPMOPMobile.Bean
{
    public class BeanUser : BeanBase
    {
        [PrimaryKey, PrimaryKeyS]
        public string ID { get; set; }
        public string UserId { get; set; }
        public string AccountID { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public int? DepartmentID { get; set; }
        public string Department { get; set; }
        public string DepartmentManager { get; set; }
        public string Manager { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string StaffID { get; set; }
        public DateTime? DateOfHire { get; set; }
        public string Mobile { get; set; }
        public string Ext { get; set; }
        public string Notify { get; set; }
        public string Reminder { get; set; }
        public string ReceiveMail { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string ExperienceLevel { get; set; }
        public int? After_CompletedDate { get; set; }
        public string PhongBan { get; set; }
        public string PublicSiteRedirect { get; set; }
        public string SubSiteRedirect { get; set; }
        public string FullNameVn { get; set; }
        public string DepartmentTitle { get; set; }
        public string ImagePath { get; set; }
        public int? UserStatus { get; set; }
        public int Language { get; set; }
        public string PositionTitle { get; set; }
        public int? PositionID { get; set; }
        public string SiteName { get; set; }
        public int? Status { get; set; }
        [Ignore]
        public short? DeviceOS { get; set; }
        [Ignore]
        public string DeviceInfo { get; set; }
        [Ignore]
        public DateTime? Modified { get; set; }
        [Ignore]
        public string Editor { get; set; }
        [Ignore]
        public DateTime? Created { get; set; }
        [Ignore]
        public bool? IsSelected { get; set; }
        [Ignore]
        public bool? IsGroup { get; set; }

        public override string GetServerUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiMobilePublic.ashx?func=get&bname=" + this.GetType().Name;
        }

        public string GetCurrentUserUrl()
        {
            return "/_layouts/15/VuThao.BPMOP.API/ApiUser.ashx?func=login";
        }
    }
}
