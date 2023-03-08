package com.vuthao.bpmop.base.realm;

import androidx.annotation.Nullable;

import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.model.app.AppLanguage;
import com.vuthao.bpmop.base.model.app.AppStatus;
import com.vuthao.bpmop.base.model.app.DBVariable;
import com.vuthao.bpmop.base.model.app.Group;
import com.vuthao.bpmop.base.model.app.Notify;
import com.vuthao.bpmop.base.model.app.Position;
import com.vuthao.bpmop.base.model.app.ResourceView;
import com.vuthao.bpmop.base.model.app.Settings;
import com.vuthao.bpmop.base.model.app.TimeLanguage;
import com.vuthao.bpmop.base.model.app.User;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.app.WorkflowFollow;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.base.model.app.WorkflowStatus;
import com.vuthao.bpmop.base.model.app.WorkflowStepDefine;
import com.vuthao.bpmop.base.model.custom.Comment;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.base.vars.VarsTable;

import java.lang.reflect.Field;

import io.realm.DynamicRealm;
import io.realm.RealmMigration;
import io.realm.RealmObjectSchema;
import io.realm.RealmSchema;

public class Migration implements RealmMigration {
    @Override
    public void migrate(DynamicRealm realm, long oldVersion, long newVersion) {
        RealmSchema schema = realm.getSchema();
        if (oldVersion == 0 || oldVersion == 1 || oldVersion == 2 || oldVersion == 3 || oldVersion == 8 || oldVersion == 5) {
            checkRealm(schema);
            oldVersion++;
        }
    }

    private void checkRealm(RealmSchema schema) {

        // region DBVariable
        final RealmObjectSchema dbVariable;
        if (schema.contains(VarsTable.DBVARIABLE)) {
            dbVariable = schema.get(VarsTable.DBVARIABLE);
            if (dbVariable != null) {
                for (Field f : DBVariable.class.getDeclaredFields()) {
                    if (!dbVariable.hasField(f.getName()))
                        dbVariable.addField(f.getName(), f.getType());
                }
            }
        } else {
            dbVariable = schema.create(VarsTable.DBVARIABLE);
            for (Field f : DBVariable.class.getDeclaredFields()) {
                dbVariable.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region Settings
        final RealmObjectSchema setting;
        if (schema.contains(VarsTable.SETTINGS)) {
            setting = schema.get(VarsTable.SETTINGS);
            if (setting != null) {
                for (Field f : Settings.class.getDeclaredFields()) {
                    if (!setting.hasField(f.getName()))
                        setting.addField(f.getName(), f.getType());
                }
            }
        } else {
            setting = schema.create(VarsTable.SETTINGS);
            for (Field f : Settings.class.getDeclaredFields()) {
                setting.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region User
        final RealmObjectSchema user;
        if (schema.contains(VarsTable.USERS)) {
            user = schema.get(VarsTable.USERS);
            if (user != null) {
                for (Field f : User.class.getDeclaredFields()) {
                    if (!user.hasField(f.getName()))
                        user.addField(f.getName(), f.getType());
                }
            }
        } else {
            user = schema.create(VarsTable.USERS);
            for (Field f : User.class.getDeclaredFields()) {
                user.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region AppLanguage
        final RealmObjectSchema language;
        if (schema.contains(VarsTable.APPLANGUAGE)) {
            language = schema.get(VarsTable.APPLANGUAGE);
            if (language != null) {
                for (Field f : AppLanguage.class.getDeclaredFields()) {
                    if (!language.hasField(f.getName()))
                        language.addField(f.getName(), f.getType());
                }
            }
        } else {
            language = schema.create(VarsTable.APPLANGUAGE);
            for (Field f : AppLanguage.class.getDeclaredFields()) {
                language.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region Notify
        final RealmObjectSchema notify;
        if (schema.contains(VarsTable.NOTIFY)) {
            notify = schema.get(VarsTable.NOTIFY);
            if (notify != null) {
                for (Field f : Notify.class.getDeclaredFields()) {
                    if (!notify.hasField(f.getName()))
                        notify.addField(f.getName(), f.getType());
                }
            }
        } else {
            notify = schema.create(VarsTable.NOTIFY);
            for (Field f : Notify.class.getDeclaredFields()) {
                notify.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region AppBase
        final RealmObjectSchema appbase;
        if (schema.contains(VarsTable.APPBASE)) {
            appbase = schema.get(VarsTable.APPBASE);
            if (appbase != null) {
                for (Field f : AppBase.class.getDeclaredFields()) {
                    if (!appbase.hasField(f.getName()))
                        appbase.addField(f.getName(), f.getType());
                }
            }
        } else {
            appbase = schema.create(VarsTable.APPBASE);
            for (Field f : AppBase.class.getDeclaredFields()) {
                appbase.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region Time Language
        final RealmObjectSchema timeLanguage;
        if (schema.contains(VarsTable.TIMELANGUAGE)) {
            timeLanguage = schema.get(VarsTable.TIMELANGUAGE);
            if (timeLanguage != null) {
                for (Field f : TimeLanguage.class.getDeclaredFields()) {
                    if (!timeLanguage.hasField(f.getName()))
                        timeLanguage.addField(f.getName(), f.getType());
                }
            }
        } else {
            timeLanguage = schema.create(VarsTable.TIMELANGUAGE);
            for (Field f : TimeLanguage.class.getDeclaredFields()) {
                timeLanguage.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region Workflow
        final RealmObjectSchema workflow;
        if (schema.contains(VarsTable.WORKFLOWS)) {
            workflow = schema.get(VarsTable.WORKFLOWS);
            if (workflow != null) {
                for (Field f : Workflow.class.getDeclaredFields()) {
                    if (!workflow.hasField(f.getName()))
                        workflow.addField(f.getName(), f.getType());
                }
            }
        } else {
            workflow = schema.create(VarsTable.WORKFLOWS);
            for (Field f : Workflow.class.getDeclaredFields()) {
                workflow.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region AppStatus
        final RealmObjectSchema appstatus;
        if (schema.contains(VarsTable.APPSTATUS)) {
            appstatus = schema.get(VarsTable.APPSTATUS);
            if (appstatus != null) {
                for (Field f : AppStatus.class.getDeclaredFields()) {
                    if (!appstatus.hasField(f.getName()))
                        appstatus.addField(f.getName(), f.getType());
                }
            }
        } else {
            appstatus = schema.create(VarsTable.APPSTATUS);
            for (Field f : AppStatus.class.getDeclaredFields()) {
                appstatus.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region Group
        final RealmObjectSchema group;
        if (schema.contains(VarsTable.GROUPS)) {
            group = schema.get(VarsTable.GROUPS);
            if (group != null) {
                for (Field f : Group.class.getDeclaredFields()) {
                    if (!group.hasField(f.getName()))
                        group.addField(f.getName(), f.getType());
                }
            }
        } else {
            group = schema.create(VarsTable.GROUPS);
            for (Field f : Group.class.getDeclaredFields()) {
                group.addField(f.getName(), f.getType());
            }
        }
        // endregion

        // region Follow
        final RealmObjectSchema follow;
        if (schema.contains(VarsTable.WORKFLOWFOLLOW)) {
            follow = schema.get(VarsTable.WORKFLOWFOLLOW);
            if (follow != null) {
                for (Field f : WorkflowFollow.class.getDeclaredFields()) {
                    if (!follow.hasField(f.getName()))
                        follow.addField(f.getName(), f.getType());
                }
            }
        } else {
            follow = schema.create(VarsTable.WORKFLOWFOLLOW);
            for (Field f : WorkflowFollow.class.getDeclaredFields()) {
                follow.addField(f.getName(), f.getType());
            }
        }
        // endregion

        //region WorkflowCategory
        final RealmObjectSchema workflowCategory;
        if (schema.contains(VarsTable.WORKFLOWCATEGORY)) {
            workflowCategory = schema.get(VarsTable.WORKFLOWCATEGORY);
            if (workflowCategory != null) {
                for (Field f : WorkflowCategory.class.getDeclaredFields()) {
                    if (!workflowCategory.hasField(f.getName()))
                        workflowCategory.addField(f.getName(), f.getType());
                }
            }
        } else {
            workflowCategory = schema.create(VarsTable.WORKFLOWCATEGORY);
            for (Field f : WorkflowCategory.class.getDeclaredFields()) {
                workflowCategory.addField(f.getName(), f.getType());
            }
        }
        //endregion

        //region WorkflowItem
        final RealmObjectSchema workflowItem;
        if (schema.contains(VarsTable.WORKFLOWITEM)) {
            workflowItem = schema.get(VarsTable.WORKFLOWITEM);
            if (workflowItem != null) {
                for (Field f : WorkflowItem.class.getDeclaredFields()) {
                    if (!workflowItem.hasField(f.getName()))
                        workflowItem.addField(f.getName(), f.getType());
                }
            }
        } else {
            workflowItem = schema.create(VarsTable.WORKFLOWITEM);
            for (Field f : WorkflowItem.class.getDeclaredFields()) {
                workflowItem.addField(f.getName(), f.getType());
            }
        }
        //endregion

        //region Position
        final RealmObjectSchema position;
        if (schema.contains(VarsTable.POSITION)) {
            position = schema.get(VarsTable.POSITION);
            if (position != null) {
                for (Field f : Position.class.getDeclaredFields()) {
                    if (!position.hasField(f.getName()))
                        position.addField(f.getName(), f.getType());
                }
            }
        } else {
            position = schema.create(VarsTable.POSITION);
            for (Field f : Position.class.getDeclaredFields()) {
                position.addField(f.getName(), f.getType());
            }
        }
        //endregion

        //region Comment
        final RealmObjectSchema comment;
        if (schema.contains("Comment")) {
            comment = schema.get("Comment");
            if (comment != null) {
                for (Field f : Comment.class.getDeclaredFields()) {
                    if (!comment.hasField(f.getName()))
                        comment.addField(f.getName(), f.getType());
                }
            }
        } else {
            comment = schema.create("Comment");
            for (Field f : Comment.class.getDeclaredFields()) {
                comment.addField(f.getName(), f.getType());
            }
        }
        //endregion

        //region WorkflowStepDefine
        final RealmObjectSchema workflowStepDefine;
        if (schema.contains(VarsTable.WORKFLOWSTEPDEFINE)) {
            workflowStepDefine = schema.get(VarsTable.WORKFLOWSTEPDEFINE);
            if (workflowStepDefine != null) {
                for (Field f : WorkflowStepDefine.class.getDeclaredFields()) {
                    if (!workflowStepDefine.hasField(f.getName()))
                        workflowStepDefine.addField(f.getName(), f.getType());
                }
            }
        } else {
            workflowStepDefine = schema.create(VarsTable.WORKFLOWSTEPDEFINE);
            for (Field f : WorkflowStepDefine.class.getDeclaredFields()) {
                workflowStepDefine.addField(f.getName(), f.getType());
            }
        }
        //endregion

        //region ResourceView
        final RealmObjectSchema resourceView;
        if (schema.contains(VarsTable.RESOURCEVIEW)) {
            resourceView = schema.get(VarsTable.RESOURCEVIEW);
            if (resourceView != null) {
                for (Field f : ResourceView.class.getDeclaredFields()) {
                    if (!resourceView.hasField(f.getName()))
                        resourceView.addField(f.getName(), f.getType());
                }
            }
        } else {
            resourceView = schema.create(VarsTable.RESOURCEVIEW);
            for (Field f : ResourceView.class.getDeclaredFields()) {
                resourceView.addField(f.getName(), f.getType());
            }
        }
        //endregion

        //region WorkflowStatus
        final RealmObjectSchema workflowStatus;
        if (schema.contains(VarsTable.WORKFLOWSTATUS)) {
            workflowStatus = schema.get(VarsTable.WORKFLOWSTATUS);
            if (workflowStatus != null) {
                for (Field f : WorkflowStatus.class.getDeclaredFields()) {
                    if (!workflowStatus.hasField(f.getName()))
                        workflowStatus.addField(f.getName(), f.getType());
                }
            }
        } else {
            workflowStatus = schema.create(VarsTable.WORKFLOWSTATUS);
            for (Field f : WorkflowStatus.class.getDeclaredFields()) {
                workflowStatus.addField(f.getName(), f.getType());
            }
        }
        //endregion
    }

    @Override
    public int hashCode() {
        return Migration.class.hashCode();
    }

    @Override
    public boolean equals(@Nullable Object obj) {
        return (obj instanceof Migration);
    }
}
