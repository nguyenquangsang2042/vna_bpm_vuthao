import 'dart:async';
import 'package:sqflite/sqflite.dart' as sqflite;
import 'package:floor/floor.dart';
import 'package:vna_bpm_flutter/model/appbase/appbase.dart';
import 'package:vna_bpm_flutter/model/appbase/appbase_dao.dart';
part 'appdatabase.g.dart'; // the generated code will be there

@Database(version: 1, entities: [AppBase])
abstract class AppDatabase extends FloorDatabase {
  AppBaseDao get appBaseDao;
}


