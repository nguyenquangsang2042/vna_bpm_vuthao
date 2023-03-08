package com.vuthao.bpmop.qrcode;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.view.View;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ImageView;
import android.widget.TextView;

import androidx.annotation.Nullable;
import androidx.core.content.FileProvider;

import com.google.zxing.Result;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.WorkflowItem;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.qrcode.presenter.QRCodePresenter;

import butterknife.BindView;
import butterknife.ButterKnife;
import me.dm7.barcodescanner.zxing.ZXingScannerView;

public class QRCodeActivity extends BaseActivity implements QRCodePresenter.QRCodeListener, View.OnClickListener, ZXingScannerView.ResultHandler {
    @BindView(R.id.iewQRCode_Camera)
    ZXingScannerView mScannerView;
    @BindView(R.id.img_ViewQRCode_Back)
    ImageView imgBack;
    @BindView(R.id.tv_QRCode_Title)
    TextView tv_QRCode_Title;

    private QRCodePresenter presenter;

    @SuppressLint("SetTextI18n")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_qrcode);
        ButterKnife.bind(this);

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tv_QRCode_Title.setText("Quét mã QR");
        } else {
            tv_QRCode_Title.setText("Scan QR");
        }

        presenter = new QRCodePresenter(this);
        imgBack.setOnClickListener(this);
    }

    @Override
    public void onClick(View v) {
        finish();
    }

    @Override
    public void onResume() {
        super.onResume();
        // Register ourselves as a handler for scan results.
        mScannerView.setResultHandler(this);
        // Start camera on resume
        mScannerView.startCamera();
    }

    @Override
    public void onPause() {
        super.onPause();
        // Stop camera on pause
        mScannerView.stopCamera();
    }

    @Override
    public void handleResult(Result result) {
        if (result.getText().contains(Constants.BASE_URL)) {
            showProgressDialog();
            Uri uri = Uri.parse(result.getText());
            String listId = uri.getQueryParameter("lid");
            String itemId = uri.getQueryParameter("itemid");

            WorkflowItem workflowItem = presenter.getWorkflowItemByQRCode(listId, itemId);
            if (workflowItem != null) {
                hideProgressDialog();
                Intent intent = new Intent(this, DetailWorkflowActivity.class);
                intent.putExtra("WorkflowItemId", workflowItem.getID());
                startActivity(intent);
            } else {
                presenter.getWorkflowItemById(itemId);
            }
        } else {
            Utility.share.showAlertWithOnlyOK("Không tìm thấy thông tin phiếu", Functions.share.getTitle("TEXT_CLOSE", "Close"), this, new Utility.OkListener() {
                @Override
                public void onOkListener() {
                    mScannerView.startCamera();
                }
            });
        }
    }

    @Override
    public void GetWorkflowSuccess(WorkflowItem workflowItem) {
        hideProgressDialog();
        Intent intent = new Intent(this, DetailWorkflowActivity.class);
        intent.putExtra("WorkflowItemId", workflowItem.getID());
        startActivity(intent);
    }

    @Override
    public void GetWorkflowErr(String err) {
        hideProgressDialog();
        Utility.share.showAlertWithOnlyOK(err, Functions.share.getTitle("TEXT_CLOSE", "Close"), this, new Utility.OkListener() {
            @Override
            public void onOkListener() {
                mScannerView.startCamera();
            }
        });
    }
}