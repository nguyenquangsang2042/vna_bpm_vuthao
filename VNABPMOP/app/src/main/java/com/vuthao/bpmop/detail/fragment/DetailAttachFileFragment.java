package com.vuthao.bpmop.detail.fragment;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.localbroadcastmanager.content.LocalBroadcastManager;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.DownloadFile;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseFragment;
import com.vuthao.bpmop.base.model.custom.AttachFile;
import com.vuthao.bpmop.base.model.dynamic.ViewElement;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.core.Vars;
import com.vuthao.bpmop.core.component.ControlInputAttachmentVertical;
import com.vuthao.bpmop.detail.custom.DetailFunc;

import java.io.File;
import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class DetailAttachFileFragment extends BaseFragment {
    @BindView(R.id.ln_ViewDetailAttachFile_All)
    LinearLayout lnAll;
    @BindView(R.id.tv_ViewDetailAttachFile_Name)
    TextView tvName;
    @BindView(R.id.img_ViewDetailAttachFile_Back)
    ImageView imgBack;
    @BindView(R.id.tv_ViewDetailAttachFile_Title)
    TextView tvTitle;
    @BindView(R.id.ln_ViewDetailAttachFile_Dynamic)
    LinearLayout lnDynamic;

    private View rootView;
    private ViewElement elementAttachFile = new ViewElement();
    private AnimationController animationController;
    private ArrayList<AttachFile> files;

    public DetailAttachFileFragment() {
        // Required empty public constructor
    }

    public DetailAttachFileFragment(ViewElement elementAttachFile) {
        this.elementAttachFile = elementAttachFile;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        if (rootView == null) {
            rootView = inflater.inflate(R.layout.fragment_detail_attach_file, container, false);
            ButterKnife.bind(this, rootView);

            init();
            setTitle();
            setData();
        }

        return rootView;
    }

    private void init() {
        animationController = new AnimationController();
        files = new ArrayList<>();

        BroadcastUtility.register(requireActivity(), mReceiver, VarsReceiver.INNERACTIONCLICK);
    }

    private void setTitle() {
        tvTitle.setText(Functions.share.getTitle("TEXT_ATTACHMENT", "File đính kèm"));
    }

    private void setData() {
        lnDynamic.removeAllViews();
        ControlInputAttachmentVertical controlInputAttachmentVertical = new ControlInputAttachmentVertical(getActivity(), lnDynamic, elementAttachFile, -1, Vars.FlagViewControlAttachment.DetailAttachFile);
        controlInputAttachmentVertical.initializeFrameView(lnDynamic);
        controlInputAttachmentVertical.setTitle();
        controlInputAttachmentVertical.setValue();
        controlInputAttachmentVertical.setEnable();
        controlInputAttachmentVertical.setProprety();

        lnDynamic.startAnimation(animationController.fadeIn(requireActivity()));

    }

    @Override
    public void onDestroyView() {
        super.onDestroyView();
        BroadcastUtility.unregister(requireActivity(), mReceiver);
    }

    @OnClick({R.id.img_ViewDetailAttachFile_Back, R.id.ln_ViewDetailAttachFile_All})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewDetailAttachFile_Back: {
                sBaseActivity.backFragment("");
                break;
            }
            case R.id.ln_ViewDetailAttachFile_All: {
                break;
            }
        }
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String element = intent.getStringExtra("element");
            int position = intent.getIntExtra("positionToAction", 0);

            ViewElement clickedElement = new Gson().fromJson(element, ViewElement.class);
            files = new Gson().fromJson(clickedElement.getValue(), new TypeToken<ArrayList<AttachFile>>() {
            }.getType());

            // mở file từ server
            if (!files.get(position).getID().equals("")) {
                new DownloadFile().execute(Constants.BASE_URL + files.get(position).getPath() + ";#" + files.get(position).getTitle() + "." + files.get(position).getExtension());
            } else {
                if (new File(files.get(position).getPath()).exists()) {
                    DetailFunc.share.openFile(requireActivity(), (files.get(position).getPath()));
                }
            }
        }
    };
}