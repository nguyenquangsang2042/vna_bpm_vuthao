
import 'package:floor/floor.dart';

@entity
class AppBase {
  @primaryKey
  int id;
  String title;
  String titleEN;
  String content;
  String assignedTo;
  int? status;
  int? statusGroup;
  int? resourceCategoryId;
  int? resourceSubCategoryId;
  DateTime? commentChanged;
  String otherResourceId;
  String createdBy;
  String modifiedBy;
  String permission;
  int? workflowId;
  String notifiedUsers;
  int? fileCount;
  int? commentCount;
  int? numComment;
  String itemUrl;
  int? step;
  int? appFlg;
  String assignedBy;
  DateTime? created;
  DateTime? dueDate;
  DateTime? modified;
  @ignore
  bool isFollow;
  int? approvalStatus;
  AppBase({
    required this.id,
    required this.title,
    required this.titleEN,
    required this.content,
    required this.assignedTo,
    this.status,
    this.statusGroup,
    this.resourceCategoryId,
    this.resourceSubCategoryId,
    this.commentChanged,
    required this.otherResourceId,
    required this.createdBy,
    required this.modifiedBy,
    required this.permission,
    this.workflowId,
    required this.notifiedUsers,
    this.fileCount,
    this.commentCount,
    this.numComment,
    required this.itemUrl,
    this.step,
    this.appFlg,
    required this.assignedBy,
    this.created,
    this.dueDate,
    this.modified,
    required this.isFollow,
    this.approvalStatus,
  });
  factory AppBase.fromJson(Map<String, dynamic> json) {
    return AppBase(
      id: json['id'],
      title: json['title'],
      titleEN: json['titleEN'],
      content: json['content'],
      assignedTo: json['assignedTo'],
      status: json['status'],
      statusGroup: json['statusGroup'],
      resourceCategoryId: json['resourceCategoryId'],
      resourceSubCategoryId: json['resourceSubCategoryId'],
      commentChanged: json['commentChanged'],
      otherResourceId: json['otherResourceId'],
      createdBy: json['createdBy'],
      modifiedBy: json['modifiedBy'],
      permission: json['permission'],
      workflowId: json['workflowId'],
      notifiedUsers: json['notifiedUsers'],
      fileCount: json['fileCount'],
      commentCount: json['commentCount'],
      numComment: json['numComment'],
      itemUrl: json['itemUrl'],
      step: json['step'],
      appFlg: json['appFlg'],
      assignedBy: json['assignedBy'],
      created: json['created'],
      dueDate: json['dueDate'],
      modified: json['modified'],
      isFollow: json['isFollow'],
      approvalStatus: json['approvalStatus'],
    );
  }
}
