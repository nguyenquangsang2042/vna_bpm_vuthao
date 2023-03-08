package com.vuthao.bpmop.base.model.app;

import io.realm.RealmObject;
import io.realm.annotations.Ignore;
import io.realm.annotations.PrimaryKey;

public class User extends RealmObject {
    @PrimaryKey
    private String ID;
    private String UserId;
    private String AccountID;
    private String AccountName;
    private String FullName;
    private String Name;
    private int DepartmentID;
    private String Department;
    private String DepartmentManager;
    private String Manager;
    private boolean Gender;
    private String BirthDay;
    private String Address;
    private String Image;
    private String StaffID;
    private String DateOfHire;
    private String Mobile;
    private String Ext;
    private String Notify;
    private String Reminder;
    private String ReceiveMail;
    private String Email;
    private String Position;
    private String ExperienceLevel;
    private int After_CompletedDate;
    private String PhongBan;
    private String privateSiteRedirect;
    private String SubSiteRedirect;
    private String FullNameVn;
    private String DepartmentTitle;
    private String ImagePath;
    private int UserStatus;
    private int Language;
    private String PositionTitle;
    private int PositionID;
    private String SiteName;
    @Ignore
    private int DeviceOS;
    @Ignore
    private String DeviceInfo;
    @Ignore
    private String Modified;
    @Ignore
    private String Editor;
    @Ignore
    private String Created;
    @Ignore
    private boolean IsSelected;
    @Ignore
    private boolean IsGroup;
    @Ignore
    private User beanUser;
    @Ignore
    private int Type;

    public User() {
    }

    public User getBeanUser() {
        return beanUser;
    }

    public void setBeanUser(User beanUser) {
        this.beanUser = beanUser;
    }

    public String getID() {
        return ID;
    }

    public void setID(String ID) {
        this.ID = ID;
    }

    public String getUserId() {
        return UserId;
    }

    public void setUserId(String userId) {
        UserId = userId;
    }

    public String getAccountID() {
        return AccountID;
    }

    public void setAccountID(String accountID) {
        AccountID = accountID;
    }

    public String getAccountName() {
        return AccountName;
    }

    public void setAccountName(String accountName) {
        AccountName = accountName;
    }

    public String getFullName() {
        return FullName;
    }

    public void setFullName(String fullName) {
        FullName = fullName;
    }

    public String getName() {
        return Name;
    }

    public void setName(String name) {
        Name = name;
    }

    public int getDepartmentID() {
        return DepartmentID;
    }

    public void setDepartmentID(int departmentID) {
        DepartmentID = departmentID;
    }

    public String getDepartment() {
        return Department;
    }

    public void setDepartment(String department) {
        Department = department;
    }

    public String getDepartmentManager() {
        return DepartmentManager;
    }

    public void setDepartmentManager(String departmentManager) {
        DepartmentManager = departmentManager;
    }

    public String getManager() {
        return Manager;
    }

    public void setManager(String manager) {
        Manager = manager;
    }

    public boolean isGender() {
        return Gender;
    }

    public void setGender(boolean gender) {
        Gender = gender;
    }

    public String getBirthDay() {
        return BirthDay;
    }

    public void setBirthDay(String birthDay) {
        BirthDay = birthDay;
    }

    public String getAddress() {
        return Address;
    }

    public void setAddress(String address) {
        Address = address;
    }

    public String getImage() {
        return Image;
    }

    public void setImage(String image) {
        Image = image;
    }

    public String getStaffID() {
        return StaffID;
    }

    public void setStaffID(String staffID) {
        StaffID = staffID;
    }

    public String getDateOfHire() {
        return DateOfHire;
    }

    public void setDateOfHire(String dateOfHire) {
        DateOfHire = dateOfHire;
    }

    public String getMobile() {
        return Mobile;
    }

    public void setMobile(String mobile) {
        Mobile = mobile;
    }

    public String getExt() {
        return Ext;
    }

    public void setExt(String ext) {
        Ext = ext;
    }

    public String getNotify() {
        return Notify;
    }

    public void setNotify(String notify) {
        Notify = notify;
    }

    public String getReminder() {
        return Reminder;
    }

    public void setReminder(String reminder) {
        Reminder = reminder;
    }

    public String getReceiveMail() {
        return ReceiveMail;
    }

    public void setReceiveMail(String receiveMail) {
        ReceiveMail = receiveMail;
    }

    public String getEmail() {
        return Email;
    }

    public void setEmail(String email) {
        Email = email;
    }

    public String getPosition() {
        return Position;
    }

    public void setPosition(String position) {
        Position = position;
    }

    public String getExperienceLevel() {
        return ExperienceLevel;
    }

    public void setExperienceLevel(String experienceLevel) {
        ExperienceLevel = experienceLevel;
    }

    public int getAfter_CompletedDate() {
        return After_CompletedDate;
    }

    public void setAfter_CompletedDate(int after_CompletedDate) {
        After_CompletedDate = after_CompletedDate;
    }

    public String getPhongBan() {
        return PhongBan;
    }

    public void setPhongBan(String phongBan) {
        PhongBan = phongBan;
    }

    public String getPrivateSiteRedirect() {
        return privateSiteRedirect;
    }

    public void setPrivateSiteRedirect(String privateSiteRedirect) {
        this.privateSiteRedirect = privateSiteRedirect;
    }

    public String getSubSiteRedirect() {
        return SubSiteRedirect;
    }

    public void setSubSiteRedirect(String subSiteRedirect) {
        SubSiteRedirect = subSiteRedirect;
    }

    public String getFullNameVn() {
        return FullNameVn;
    }

    public void setFullNameVn(String fullNameVn) {
        FullNameVn = fullNameVn;
    }

    public String getDepartmentTitle() {
        return DepartmentTitle;
    }

    public void setDepartmentTitle(String departmentTitle) {
        DepartmentTitle = departmentTitle;
    }

    public String getImagePath() {
        return ImagePath;
    }

    public void setImagePath(String imagePath) {
        ImagePath = imagePath;
    }

    public int getUserStatus() {
        return UserStatus;
    }

    public void setUserStatus(int userStatus) {
        UserStatus = userStatus;
    }

    public int getLanguage() {
        return Language;
    }

    public void setLanguage(int language) {
        Language = language;
    }

    public String getPositionTitle() {
        return PositionTitle;
    }

    public void setPositionTitle(String positionTitle) {
        PositionTitle = positionTitle;
    }

    public int getPositionID() {
        return PositionID;
    }

    public void setPositionID(int positionID) {
        PositionID = positionID;
    }

    public String getSiteName() {
        return SiteName;
    }

    public void setSiteName(String siteName) {
        SiteName = siteName;
    }

    public int getDeviceOS() {
        return DeviceOS;
    }

    public void setDeviceOS(int deviceOS) {
        DeviceOS = deviceOS;
    }

    public String getDeviceInfo() {
        return DeviceInfo;
    }

    public void setDeviceInfo(String deviceInfo) {
        DeviceInfo = deviceInfo;
    }

    public String getModified() {
        return Modified;
    }

    public void setModified(String modified) {
        Modified = modified;
    }

    public String getEditor() {
        return Editor;
    }

    public void setEditor(String editor) {
        Editor = editor;
    }

    public String getCreated() {
        return Created;
    }

    public void setCreated(String created) {
        Created = created;
    }

    public boolean isSelected() {
        return IsSelected;
    }

    public void setSelected(boolean selected) {
        IsSelected = selected;
    }

    public boolean isGroup() {
        return IsGroup;
    }

    public void setGroup(boolean group) {
        IsGroup = group;
    }

    public int getType() {
        return Type;
    }

    public void setType(int type) {
        Type = type;
    }
}
