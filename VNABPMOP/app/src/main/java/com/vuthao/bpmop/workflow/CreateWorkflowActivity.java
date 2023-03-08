package com.vuthao.bpmop.workflow;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.RecyclerView;

import android.os.Bundle;
import android.view.View;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.realm.RealmController;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import io.realm.Realm;

public class CreateWorkflowActivity extends AppCompatActivity {
    @BindView(R.id.img_ViewDetailWorkflow_Back)
    ImageView imgBack;
    @BindView(R.id.tv_ViewDetailWorkflow_Name)
    TextView tvName;
    @BindView(R.id.recy_ViewDetailWorkflow_Data)
    RecyclerView rvDetail;
    @BindView(R.id.ln_ViewDetailWorkflow_ActionAll)
    LinearLayout lnActionAll;

    private Workflow workflow;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_create_workflow);
        ButterKnife.bind(this);

        init();
    }

    private void init() {
        Bundle bundle = getIntent().getExtras();
        int workflowId = bundle.getInt("workflowId");
        workflow = new RealmController().getRealm()
                .where(Workflow.class)
                .equalTo("WorkflowID", workflowId)
                .findFirst();

        if (CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN)) {
            tvName.setText(workflow.getTitle());
        } else {
            tvName.setText(workflow.getTitleEN());
        }
    }

    @OnClick({R.id.img_ViewDetailWorkflow_Back})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewDetailWorkflow_Back: {
                finish();
                break;
            }
        }
    }
}