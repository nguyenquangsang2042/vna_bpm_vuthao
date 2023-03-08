import 'package:floor/floor.dart';
import 'package:vna_bpm_flutter/model/appbase/appbase.dart';

@dao
abstract class AppBaseDao
{
  @Query('Select * From AppBase')
  Future<List<AppBase>> findAllAppBase();
  @Insert(onConflict: OnConflictStrategy.replace)
  Future<void> insertAll(List<AppBase> list);
}