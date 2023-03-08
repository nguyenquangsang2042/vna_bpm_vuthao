package com.vuthao.bpmop.workflow;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import android.app.Dialog;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.os.Bundle;
import android.view.Gravity;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.view.animation.AnimationUtils;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.google.gson.Gson;
import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.Constants;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.custom.expandable.AnimatedExpandableListView;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.CurrentUser;
import com.vuthao.bpmop.base.model.app.Workflow;
import com.vuthao.bpmop.base.model.app.WorkflowCategory;
import com.vuthao.bpmop.base.model.custom.BoardWorkflow;
import com.vuthao.bpmop.base.realm.BoardController;
import com.vuthao.bpmop.board.adapter.BoardChooseCategoryAdapter;
import com.vuthao.bpmop.board.adapter.ExpandBoardMainGroupAdapter;
import com.vuthao.bpmop.board.presenter.BoardPresenter;
import com.vuthao.bpmop.workflow.adapter.ExpandListWorkflowAdapter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;
import de.hdodenhof.circleimageview.CircleImageView;

public class ListWorkflowActivity extends BaseActivity implements ExpandListWorkflowAdapter.ExpandListWorkflowListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.swipe_ViewBoard)
    SwipeRefreshLayout swipe;
    @BindView(R.id.tv_ViewBoard_Name)
    TextView tvTitle;
    @BindView(R.id.img_ViewBoard_Back)
    ImageView imgBack;
    @BindView(R.id.expand_ViewBoard_Data)
    AnimatedExpandableListView expandData;
    @BindView(R.id.ln_ViewBoard_NoData)
    LinearLayout lnNoDataExpand;
    @BindView(R.id.tv_ViewBoard_NoData)
    TextView tvNoDataExpand;
    @BindView(R.id.card_ViewBoard_GroupWorkflow)
    CardView cardGroupWorkflow;
    @BindView(R.id.tv_ViewBoard_CurrentGroupWorkflow)
    TextView tvCurrentGroup;

    private BoardController controller;
    private WorkflowCategory currentExpandWFCategory = null;
    private Dialog dialog;
    private ExpandListWorkflowAdapter adapter;
    private ArrayList<WorkflowCategory> lstWFCategory; // List nhóm quy trình
    private ArrayList<BoardWorkflow> lstExpand; // List của Expandable ListView

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_list_workflow);
        ButterKnife.bind(this);

        init();
        setData();

        swipe.setOnRefreshListener(this);
    }

    private void init() {
        controller = new BoardController();
        lstWFCategory = new ArrayList<>();
        lstExpand = new ArrayList<>();

        tvTitle.setText(Functions.share.getTitle("TEXT_CREATENEW_WORKFLOW", "Create new"));

        expandData.setGroupIndicator(null);
        expandData.setChildIndicator(null);
        expandData.setDividerHeight(0);
    }

    private void setData() {
        ArrayList<WorkflowCategory> temps = new ArrayList<>();
        WorkflowCategory categoryAll = new WorkflowCategory();
        categoryAll.setID(0);
        categoryAll.setTitle(Functions.share.getTitle("TEXT_ALL", "Tất cả"));
        categoryAll.setSelected(true);

        temps.add(categoryAll);
        temps.addAll(controller.getWorkflowCategory());

        lstExpand.clear();
        lstWFCategory.clear();

        for (int i = 0; i < temps.size(); i++) {
            if (temps.get(i).getID() != 0) {
                ArrayList<Workflow> workflows = controller.getWorkflows(temps.get(i).getID());
                if (workflows.size() > 0) {
                    lstWFCategory.add(temps.get(i));
                    lstExpand.add(new BoardWorkflow(temps.get(i), workflows));
                }
            } else {
                lstWFCategory.add(temps.get(i));
            }
        }

        currentExpandWFCategory = categoryAll;
        tvCurrentGroup.setText(currentExpandWFCategory.getTitle());

        setListExpandBoard(lstExpand);
    }

    @OnClick({R.id.ln_ViewBoard_CurrentGroupWorkflow, R.id.img_ViewBoard_Back})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.ln_ViewBoard_CurrentGroupWorkflow: {
                filter();
                break;
            }
            case R.id.img_ViewBoard_Back: {
                finish();
                break;
            }
        }
    }

    private void filter() {
        KeyboardManager.hideKeyboard(this);
        View view = getLayoutInflater().inflate(R.layout.popup_control_single_choice, null);
        ImageView imgClose = view.findViewById(R.id.img_PopupControl_SingleChoice_Close);
        TextView tvTitle = view.findViewById(R.id.tv_PopupControl_SingleChoice_Title);
        RecyclerView recyData = view.findViewById(R.id.recy_PopupControl_SingleChoice_Data);
        ImageView imgDone = view.findViewById(R.id.img_PopupControl_SingleChoice_Done);

        imgDone.setVisibility(View.INVISIBLE);
        tvTitle.setText(CurrentUser.getInstance().getUser().getLanguage() == Integer.parseInt(Constants.mLangVN) ? "Nhóm quy trình" : "Group workflow");

        BoardChooseCategoryAdapter adapter = new BoardChooseCategoryAdapter(this, lstWFCategory, pos -> {
            for (WorkflowCategory category : lstWFCategory) {
                category.setSelected(false);
            }

            lstWFCategory.get(pos).setSelected(true);

            currentExpandWFCategory = lstWFCategory.get(pos);
            this.adapter.setWFCategory(lstWFCategory.get(pos));
            tvCurrentGroup.setText(lstWFCategory.get(pos).getTitle());
            this.adapter.filter();
            dialog.dismiss();
        });

        recyData.setAdapter(adapter);
        recyData.setLayoutManager(new LinearLayoutManager(this));

        imgClose.setOnClickListener(v -> dialog.dismiss());

        //region Dialog
        dialog = new Dialog(this, R.style.Theme_Custom_BPMOP_Dialog_FullScreen);
        Window window = dialog.getWindow();
        dialog.requestWindowFeature(1);
        dialog.setCanceledOnTouchOutside(false);
        dialog.setCancelable(true);
        window.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN);
        window.setGravity(Gravity.BOTTOM);

        dialog.setContentView(view);
        dialog.show();
        WindowManager.LayoutParams s = window.getAttributes();
        s.width = WindowManager.LayoutParams.MATCH_PARENT;
        s.height = WindowManager.LayoutParams.MATCH_PARENT;
        window.setAttributes(s);
        window.setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        //endregion
    }

    private void setListExpandBoard(ArrayList<BoardWorkflow> lstBoardWorkflow) {
        if (lstBoardWorkflow.size() > 0) {
            expandData.setVisibility(View.VISIBLE);
            lnNoDataExpand.setVisibility(View.GONE);

            expandData.startAnimation(AnimationUtils.loadAnimation(this, R.anim.anim_fade_in));
            adapter = new ExpandListWorkflowAdapter(this, lstBoardWorkflow, this);
            adapter.setWFCategory(currentExpandWFCategory);
            expandData.setAdapter(adapter);
            for (int i = 0; i < adapter.getGroupCount(); i++) {
                if (lstExpand.get(i).getWorkflows().size() > 0)
                    expandData.expandGroup(i);
            }
        } else {
            expandData.setVisibility(View.GONE);
            lnNoDataExpand.setVisibility(View.VISIBLE);
        }
    }

    @Override
    public void onRefresh() {
        swipe.setRefreshing(false);
    }

    @Override
    public void OnItemClick(Workflow workflow) {
        Intent intent = new Intent(this, CreateWorkflowActivity.class);
        intent.putExtra("workflowId", workflow.getWorkflowID());
        startActivity(intent);
    }
}