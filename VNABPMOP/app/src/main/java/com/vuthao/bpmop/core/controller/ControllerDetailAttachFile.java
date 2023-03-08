package com.vuthao.bpmop.core.controller;

import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.custom.GroupAttachFile;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.util.stream.Collectors;

public class ControllerDetailAttachFile extends ControllerBase {
    public ArrayList<GroupAttachFile> cloneListAttachFiles(ArrayList<AttachFile> files) {
        ArrayList<GroupAttachFile> result = new ArrayList<>();

        HashMap<String, String> hashMap = new HashMap<>();
        for (AttachFile file : files) {
            if (!hashMap.containsKey(file.getAttachTypeName())) {
                hashMap.put(file.getAttachTypeName(), file.getAttachTypeName());
            }
        }

        GroupAttachFile groupAttachFile = new GroupAttachFile(Functions.share.getTitle("TEXT_ADDNEW_ATTACHMENT", "Tạo mới") , new ArrayList<>());
        for(Map.Entry<String, String> entry : hashMap.entrySet()) {
            String s = entry.getKey();

            if (!Functions.isNullOrEmpty(s)) {
                ArrayList<AttachFile> _lstByCategory = new ArrayList<>();
                for (AttachFile file : files) {
                    if (!Functions.isNullOrEmpty(file.getAttachTypeName()) && file.getAttachTypeName().equals(s)) {
                        _lstByCategory.add(file);
                    }
                }

                result.add(new GroupAttachFile(s,_lstByCategory));
            } else {
                ArrayList<AttachFile> _lstByCategory = (ArrayList<AttachFile>) files.stream().filter(r -> Functions.isNullOrEmpty(r.getAttachTypeName())).collect(Collectors.toList());
                groupAttachFile.setAttachFiles(_lstByCategory);
            }
        }

        if (groupAttachFile.getAttachFiles() != null && groupAttachFile.getAttachFiles().size() > 0) {
            result.add(groupAttachFile);
        }

        return result;
    }
}
