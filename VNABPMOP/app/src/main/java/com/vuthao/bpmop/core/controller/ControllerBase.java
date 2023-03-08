package com.vuthao.bpmop.core.controller;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Functions;

public class ControllerBase {
    public int getResourceIDAttachment(String path) {
        int _result = R.drawable.icon_attachfile_other;

        if (!Functions.isNullOrEmpty(path)) {
            String extension = getExt(path);

            switch (extension) {
                case "doc":
                case "docx":
                    _result = R.drawable.icon_word;
                    break;
                case "txt":
                    _result = R.drawable.icon_attachfile_txt;
                    break;
                case "png":
                case "jpeg":
                case "jpg":
                    _result = R.drawable.icon_attachfile_photo;
                    break;
                case "xls":
                case "xlsx":
                    _result = R.drawable.icon_attachfile_excel;
                    break;
                case "pdf":
                    _result = R.drawable.icon_attachfile_pdf;
                    break;
                case "ppt":
                    _result = R.drawable.icon_attachfile_ppt;
                    break;
                default:
                    _result = R.drawable.icon_attachfile_other;
                    break;
            }
        }

        return _result;
    }

    public String getExt(String filePath){
        int strLength = filePath.lastIndexOf(".");
        if(strLength > 0)
            return filePath.substring(strLength + 1).toLowerCase();
        return null;
    }
}
