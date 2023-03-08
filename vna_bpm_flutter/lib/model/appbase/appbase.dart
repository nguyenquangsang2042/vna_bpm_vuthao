
import 'package:floor/floor.dart';

@entity
class AppBase {
  @primaryKey
  int id;
  String title;
  String titleEN;
  String content;
  String assignedTo;
  AppBase({
    required this.id,
    required this.title,
    required this.titleEN,
    required this.content,
    required this.assignedTo,

  });
  factory AppBase.fromJson(Map<String, dynamic> json) {
    return AppBase(
      id: json['id'],
      title: json['title'],
      titleEN: json['titleEN'],
      content: json['content'],
      assignedTo: json['assignedTo']
    );
  }
}
