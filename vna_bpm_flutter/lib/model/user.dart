import 'package:floor/floor.dart';
@entity
class User
{
    String id;
    String userId;
    String accountID;
    String accountName;
    String fullName;
    String name;
    int departmentID;
    String department;
    String departmentManager;
    String manager;
    bool gender;
    DateTime birthDay;
    String address;
    String image;
    String staffID;
    DateTime dateOfHire;
    String mobile;
    String ext;
    String notify;
    String reminder;
    String receiveMail;
    String email;
    String position;
    String experienceLevel;
    int afterCompletedDate;
    String phongBan;
    String publicSiteRedirect;
    String subSiteRedirect;
    String fullNameVn;
    String departmentTitle;
    String imagePath;
    int userStatus;
    int language;
    String positionTitle;
    int positionID;
    String siteName;
    int status;

    User(
      this.id,
      this.userId,
      this.accountID,
      this.accountName,
      this.fullName,
      this.name,
      this.departmentID,
      this.department,
      this.departmentManager,
      this.manager,
      this.gender,
      this.birthDay,
      this.address,
      this.image,
      this.staffID,
      this.dateOfHire,
      this.mobile,
      this.ext,
      this.notify,
      this.reminder,
      this.receiveMail,
      this.email,
      this.position,
      this.experienceLevel,
      this.afterCompletedDate,
      this.phongBan,
      this.publicSiteRedirect,
      this.subSiteRedirect,
      this.fullNameVn,
      this.departmentTitle,
      this.imagePath,
      this.userStatus,
      this.language,
      this.positionTitle,
      this.positionID,
      this.siteName,
      this.status);
}