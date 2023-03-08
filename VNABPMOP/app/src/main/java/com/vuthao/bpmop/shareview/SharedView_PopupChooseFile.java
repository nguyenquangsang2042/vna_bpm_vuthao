package com.vuthao.bpmop.shareview;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Dialog;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.provider.MediaStore;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.activity.result.ActivityResultLauncher;
import androidx.core.content.FileProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.custom.DownloadedFiles;
import com.vuthao.bpmop.base.realm.RealmController;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.detail.fragment.ReplyCommentFragment;
import com.vuthao.bpmop.shareview.adapter.DownloadedFilesAdapter;
import com.vuthao.bpmop.task.DetailCreateTaskActivity;
import com.vuthao.bpmop.task.child.CreateChildTaskActivity;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;

import io.realm.RealmResults;

public class SharedView_PopupChooseFile extends SharedViewBase implements View.OnClickListener, DownloadedFilesAdapter.DownloadedFilesListener {
    public int requestCodeFile;
    public int requestCodeCamera;
    public int flagview;
    private ActivityResultLauncher<Intent> mLauncherNormal;
    private ActivityResultLauncher<Intent> mLauncherCamera;
    private BaseFragment baseFragment;

    private Dialog dialog;
    private View view;
    private ImageView imgBack;
    private TextView tvTitle;
    private TextView tvInApp;
    private TextView tvOther;
    private LinearLayout lnOtherCloud;
    private LinearLayout lnOtherLibrary;
    private LinearLayout lnOtherCamera;
    private TextView tvOtherCloud;
    private TextView tvOtherLibrary;
    private TextView tvOtherCamera;
    private TextView tvNodata;
    private RecyclerView lvInApp;

    public SharedView_PopupChooseFile(LayoutInflater inflater, Activity mainAct, String fragmentTag, View rootView,
                                      int requestCodeFile, int requestCodeCamera, int flagview) {
        super(inflater, mainAct, fragmentTag, rootView);
        this.requestCodeFile = requestCodeFile;
        this.requestCodeCamera = requestCodeCamera;
        this.flagview = flagview;
    }

    // Dành cho ReplyComment
    public SharedView_PopupChooseFile(LayoutInflater inflater, Activity mainAct, BaseFragment baseFragment, ActivityResultLauncher<Intent> mLauncherNormal, ActivityResultLauncher<Intent> mLauncherCamera, String fragmentTag, View rootView,
                                      int requestCodeFile, int requestCodeCamera, int flagview) {
        super(inflater, mainAct, fragmentTag, rootView);
        this.requestCodeFile = requestCodeFile;
        this.requestCodeCamera = requestCodeCamera;
        this.flagview = flagview;
        this.mLauncherNormal = mLauncherNormal;
        this.mLauncherCamera = mLauncherCamera;
        this.baseFragment = baseFragment;
    }

    @SuppressLint("SetTextI18n")
    @Override
    public void initializeView() {
        view = inflater.inflate(R.layout.popup_control_attachment_choose_file, null);
        imgBack = view.findViewById(R.id.img_PopupControlAttachmentChooseFile_Back);
        tvTitle = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_title);
        tvInApp = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_InApp);
        tvOther = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_Other);
        lnOtherCloud = view.findViewById(R.id.ln_PopupControlAttachmentChooseFile_Other_Cloud);
        lnOtherLibrary = view.findViewById(R.id.ln_PopupControlAttachmentChooseFile_Other_Library);
        lnOtherCamera = view.findViewById(R.id.ln_PopupControlAttachmentChooseFile_Other_Camera);
        tvOtherCloud = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_Other_Cloud);
        tvOtherLibrary = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_Other_Library);
        tvOtherCamera = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_Other_Camera);
        tvNodata = view.findViewById(R.id.tv_PopupControlAttachmentChooseFile_NoData);
        lvInApp = view.findViewById(R.id.recy_PopupControlAttachmentChooseFile_InApp);

        tvTitle.setText(Functions.share.getTitle("TEXT_ATTACHMENT", "Tài liệu đính kèm"));
        tvNodata.setText(Functions.share.getTitle("TEXT_NODATA", "No data"));
        tvInApp.setText(Functions.share.getTitle("TEXT_FILE_INAPP", "Tập tin trong ứng dụng"));
        tvOther.setText(Functions.share.getTitle("TEXT_OTHER_RESOURCE", "Nguồn khác"));
        tvOtherCloud.setText(Functions.share.getTitle("TEXT_STORAGE_APPLICATION", "Ứng dụng lưu trữ"));
        tvOtherLibrary.setText(Functions.share.getTitle("TEXT_PHOTO_LIBRARY", "Thư viện ảnh"));
        tvOtherCamera.setText("Camera");

        loadDownloadedFiles();

        // region Dialog
        dialog = new Dialog(mainAct, R.style.Theme_Custom_BPMOP_Dialog_FullScreen_Animation);
        Window window = dialog.getWindow();
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.CENTER);
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        dialog.setContentView(view);
        dialog.show();

        WindowManager.LayoutParams s = window.getAttributes();
        s.width = WindowManager.LayoutParams.MATCH_PARENT;
        s.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(s);
        // endregion

        imgBack.setOnClickListener(this);
        lnOtherCloud.setOnClickListener(this);
        lnOtherLibrary.setOnClickListener(this);
        lnOtherCamera.setOnClickListener(this);
    }

    private void loadDownloadedFiles() {
        ArrayList<DownloadedFiles> files = getDownloadedFiles();
        if (files.size() > 0) {
            lvInApp.setLayoutManager(new LinearLayoutManager(mainAct));
            lvInApp.setHasFixedSize(true);
            lvInApp.setItemViewCacheSize(20);

            DownloadedFilesAdapter adapter = new DownloadedFilesAdapter(mainAct, files, this);
            lvInApp.setAdapter(adapter);

            tvNodata.setVisibility(View.GONE);
            lvInApp.setVisibility(View.VISIBLE);
        } else {
            tvNodata.setVisibility(View.VISIBLE);
            lvInApp.setVisibility(View.GONE);
        }
    }

    private File createImageFile() throws IOException {
        String imageFileName = "BPMON_" + System.currentTimeMillis() + ".png";
        File dirs = new File(mainAct.getFilesDir().toString(), "Camera");
        if (!dirs.exists()) {
            dirs.mkdirs();
        }
        return new File(dirs, imageFileName);
    }

    private void otherClould() {
        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
        intent.putExtra(Intent.EXTRA_MIME_TYPES, Constants.mimeTypes);
        intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.O) {
            intent.setType("*/*");
        } else {
            intent.setType("file/*");
        }

        if (mLauncherNormal != null) {
            mLauncherNormal.launch(intent);
        } else {
            mainAct.startActivityForResult(intent, requestCodeFile);
        }

        dialog.dismiss();
    }

    private void otherLibrary() {
        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
        intent.putExtra(Intent.EXTRA_MIME_TYPES, Constants.mimeTypesImage);

        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.O) {
            intent.setType("*/*");
        } else {
            intent.setType("file/*");
        }

        if (mLauncherNormal != null) {
            mLauncherNormal.launch(intent);
        } else {
            mainAct.startActivityForResult(intent, requestCodeFile);
        }

        dialog.dismiss();
    }

    private void otherCamera() {
        BaseActivity.sBaseActivity.checkUserPermission(new BaseActivity.PermissionListener() {
            @Override
            public void OnAcceptedAllPermission() {
                File file = null;
                Uri uri = null;
                try {
                    file = createImageFile();
                    file.createNewFile();

                    uri = FileProvider.getUriForFile(mainAct, mainAct.getPackageName() + ".provider", file);

                    switch (flagview) {
                        case Vars.FlagView.DetailWorkflow_Comment:
                        case Vars.FlagView.DetailWorkflow_ControlInputAttachmentVertical: {
                            ((DetailWorkflowActivity) mainAct).fileCamera = uri;
                            break;
                        }
                        case Vars.FlagView.ReplyComment: {
                            ((ReplyCommentFragment) baseFragment).fileImage = uri;
                            break;
                        }
                        case Vars.FlagView.DetailCreateTask_ControlInputAttachmentVertical:
                        case Vars.FlagView.DetailCreateTask_Comment: {
                            ((DetailCreateTaskActivity) mainAct).uri = uri;
                            break;
                        }
                        case Vars.FlagView.DetailCreateTask_Child_ControlInputAttachmentVertical: {
                            ((CreateChildTaskActivity) mainAct).uri = uri;
                            break;
                        }
                    }

                    Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
                    intent.putExtra(MediaStore.EXTRA_OUTPUT, uri);
                    if (mLauncherNormal != null) {
                        mLauncherCamera.launch(intent);
                    } else {
                        mainAct.startActivityForResult(intent, requestCodeCamera);
                    }
                    dialog.dismiss();
                } catch (Exception ex) {
                    dialog.dismiss();
                    Log.d("ERR Capture", ex.getMessage());
                }
            }

            @Override
            public void OnCancelPermission() {
            }

            @Override
            public void OnNeverRequestPermission() {

            }
        }, new String[]{
                Manifest.permission.CAMERA,
                Manifest.permission.WRITE_EXTERNAL_STORAGE,
                Manifest.permission.READ_EXTERNAL_STORAGE});
    }

    private ArrayList<DownloadedFiles> getDownloadedFiles() {
        ArrayList<DownloadedFiles> items = new ArrayList<>();
        File directory = new File(mainAct.getFilesDir().toString(), CurrentUser.getInstance().getUser().getID());
        File[] files = directory.listFiles();
        if (files != null) {
            for (int i = 0; i < files.length; i++) {
                DownloadedFiles file = new DownloadedFiles();
                file.setID("");
                file.setTitle(files[i].getName());
                file.setSize(files[i].length());
                file.setCreated(files[i].lastModified());
                file.setModified(files[i].lastModified());
                file.setPath(files[i].getPath());
                items.add(file);
            }
        }

        return items;
    }

    @Override
    public void onClick(View view) {
        switch (view.getId()) {
            case R.id.img_PopupControlAttachmentChooseFile_Back: {
                dialog.dismiss();
                break;
            }
            case R.id.ln_PopupControlAttachmentChooseFile_Other_Cloud: {
                otherClould();
                break;
            }
            case R.id.ln_PopupControlAttachmentChooseFile_Other_Library: {
                otherLibrary();
                break;
            }
            case R.id.ln_PopupControlAttachmentChooseFile_Other_Camera: {
                otherCamera();
                break;
            }
        }
    }

    @Override
    public void OnItemClick(DownloadedFiles file) {
        Intent intent = new Intent();
        intent.setAction(VarsReceiver.SELECT_FILE_INAPP);
        String type = "";

        if (requestCodeFile == Constants.mDetailAttachment || requestCodeFile == Constants.mTaskAttachment) {
            type = "inputattachmenthorizon";
        } else if (requestCodeFile == Constants.mDetailComment || requestCodeFile == Constants.mTaskComment) {
            type = "comment";
        }

        intent.putExtra("type",type);
        intent.putExtra("file", new Gson().toJson(file));
        BroadcastUtility.send(mainAct, intent);
        dialog.dismiss();
    }
}
