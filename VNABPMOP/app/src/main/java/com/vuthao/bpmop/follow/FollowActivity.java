package com.vuthao.bpmop.follow;

import android.annotation.SuppressLint;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Typeface;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import androidx.core.content.ContextCompat;
import androidx.core.content.res.ResourcesCompat;
import androidx.localbroadcastmanager.content.LocalBroadcastManager;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.vuthao.bpmop.R;
import com.vuthao.bpmop.base.AnimationController;
import com.vuthao.bpmop.base.BroadcastUtility;
import com.vuthao.bpmop.base.Functions;
import com.vuthao.bpmop.base.Utility;
import com.vuthao.bpmop.base.activity.BaseActivity;
import com.vuthao.bpmop.base.api.ApiBPM;
import com.vuthao.bpmop.base.custom.expression.Function;
import com.vuthao.bpmop.base.keyboard.KeyboardManager;
import com.vuthao.bpmop.base.model.app.AppBase;
import com.vuthao.bpmop.base.vars.VarsReceiver;
import com.vuthao.bpmop.detail.DetailWorkflowActivity;
import com.vuthao.bpmop.follow.adapter.FollowAdapter;
import com.vuthao.bpmop.follow.presenter.FollowPresenter;

import java.util.ArrayList;

import butterknife.BindView;
import butterknife.ButterKnife;
import butterknife.OnClick;

public class FollowActivity extends BaseActivity implements ApiBPM.ApiBPMRefreshListener, TextWatcher, FollowAdapter.FollowListener, SwipeRefreshLayout.OnRefreshListener {
    @BindView(R.id.edt_ListWorkflowView_Search)
    EditText edtSearch;
    @BindView(R.id.img_ListWorkflowView_Search_Delete)
    ImageView imgDeleteSearch;
    @BindView(R.id.img_ViewListWorkflow_ShowSearch)
    ImageView imgShowSearch;
    @BindView(R.id.swipe_ViewListWorkflow)
    SwipeRefreshLayout swipe;
    @BindView(R.id.ln_ViewListWorkflow_NoData)
    LinearLayout lnNoData;
    @BindView(R.id.ln_ViewListWorkflow_Content)
    LinearLayout lnContent;
    @BindView(R.id.tv_ViewListWorkflow_NoData)
    TextView tvNoData;
    @BindView(R.id.recy_ViewListWorkflow)
    RecyclerView recyData;
    @BindView(R.id.tv_ViewListWorkflow_Name)
    TextView tvTitle;
    @BindView(R.id.ln_ViewListWorkflow_Search)
    LinearLayout lnSearch;
    @BindView(R.id.img_ViewListWorkflow_Back)
    ImageView imgBack;

    private FollowPresenter presenter;
    private ArrayList<AppBase> follows;
    private FollowAdapter adapter;
    private ApiBPM apiBPM;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_follow);
        ButterKnife.bind(this);

        init();
        setTitle();
        setData();
        setCountFollow();

        swipe.setOnRefreshListener(this);
        edtSearch.addTextChangedListener(this);
    }

    private void init() {
        presenter = new FollowPresenter();
        follows = new ArrayList<>();
        apiBPM = new ApiBPM(this);

        recyData.setLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.VERTICAL, false));
        recyData.setDrawingCacheEnabled(true);
        recyData.setHasFixedSize(true);
        recyData.setItemViewCacheSize(20);

        Utility.share.setupSwipeRefreshLayout(swipe);

        BroadcastUtility.register(this, mReceiver, VarsReceiver.FOLLOW);
        BroadcastUtility.register(this, mReceiver, VarsReceiver.REFRESHAFTERSUBMITACTION);
    }

    @SuppressLint("SetTextI18n")
    private void setTitle() {
        tvTitle.setText(Functions.share.getTitle("TEXT_FOLLOW", "Follow") + " " + Functions.share.getCountOfNumText(tvTitle.getText().toString()));
        tvNoData.setText(Functions.share.getTitle("TEXT_NODATA", "Không có dữ liệu"));
        edtSearch.setHint(Functions.share.getTitle("TEXT_SEARCH", "Tìm kiếm"));

        Functions.share.setTVHighligtColor(tvTitle, tvTitle.getText().toString(), "(", ")", 0);
    }

    private void setData() {
        follows = presenter.getListFollow();
        if (!follows.isEmpty()) {
            lnNoData.setVisibility(View.GONE);
            recyData.setVisibility(View.VISIBLE);

            if (adapter == null) {
                adapter = new FollowAdapter(this, follows, this);
                recyData.setAdapter(adapter);
            } else {
                adapter.setListRefresh(follows);
                if (edtSearch.getText().length() > 0) {
                    edtSearch.setText(edtSearch.getText());
                }
            }
        } else {
            lnNoData.setVisibility(View.VISIBLE);
            recyData.setVisibility(View.GONE);
        }
    }

    private void setCountFollow() {
        Functions.share.setFormatItemCount(tvTitle, follows.size(), "follow", "");
        Functions.share.setTVHighligtColor(tvTitle, tvTitle.getText().toString(), "(", ")", 0);
    }

    @OnClick({R.id.img_ViewListWorkflow_Back, R.id.img_ViewListWorkflow_ShowSearch, R.id.img_ListWorkflowView_Search_Delete})
    public void OnViewClicked(View v) {
        switch (v.getId()) {
            case R.id.img_ViewListWorkflow_Back: {
                imgBack.startAnimation(AnimationController.share.fadeIn(this));
                finish();
                break;
            }
            case R.id.img_ViewListWorkflow_ShowSearch: {
                search();
                break;
            }
            case R.id.img_ListWorkflowView_Search_Delete: {
                edtSearch.setText("");
                break;
            }
        }
    }

    private void search() {
        imgShowSearch.startAnimation(AnimationController.share.fadeIn(this));
        if (lnSearch.getVisibility() == View.GONE) {
            lnSearch.setVisibility(View.VISIBLE);
            edtSearch.requestFocus();
            KeyboardManager.showKeyBoard(edtSearch, FollowActivity.this);
        } else {
            lnSearch.setVisibility(View.GONE);
            KeyboardManager.hideKeyboard(edtSearch, FollowActivity.this);
        }
    }

    @Override
    public void OnClick(int pos) {
        Intent intent = new Intent(this, DetailWorkflowActivity.class);
        intent.putExtra("WorkflowItemId", Functions.share.getWorkflowItemIDByUrl(follows.get(pos).getItemUrl()));
        startActivity(intent);
    }

    @Override
    public void onRefresh() {
        apiBPM.updateAllMasterData(false);
    }

    @Override
    public void beforeTextChanged(CharSequence s, int start, int count, int after) {

    }

    @Override
    public void onTextChanged(CharSequence s, int start, int before, int count) {

    }

    @Override
    public void afterTextChanged(Editable s) {
        if (s.toString().length() > 0) {
            imgDeleteSearch.setVisibility(View.VISIBLE);
            edtSearch.setTypeface(ResourcesCompat.getFont(this, R.font.fontarial), Typeface.NORMAL);
            imgShowSearch.setColorFilter(ContextCompat.getColor(this, R.color.clVer2BlueMain));
        } else {
            imgDeleteSearch.setVisibility(View.GONE);
            edtSearch.setTypeface(ResourcesCompat.getFont(this, R.font.fontarial), Typeface.ITALIC);
            imgShowSearch.setColorFilter(ContextCompat.getColor(this, R.color.clBottomDisable));
        }

        if (adapter != null) {
            adapter.getFilter().filter(s.toString());
        }
    }

    private final BroadcastReceiver mReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch (intent.getAction()) {
                case VarsReceiver.FOLLOW: {
                    String workflowId = intent.getStringExtra("WorkflowId");
                    boolean isFollow = intent.getBooleanExtra("isFollow", false);

                    // unfollow mới update
                    if (!isFollow) {
                        int index = -1;
                        for (int i = 0; i < follows.size(); i++) {
                            if (follows.get(i).getID() == Integer.parseInt(workflowId)) {
                                index = i;
                                break;
                            }
                        }
                        if (index != -1) {
                            follows.remove(index);
                        }

                        adapter.notifyDataSetChanged();
                    }

                    setCountFollow();
                    break;
                }
                case VarsReceiver.REFRESHAFTERSUBMITACTION: {
                    OnRefreshSuccess();
                    break;
                }
            }
        }
    };

    @Override
    protected void onDestroy() {
        super.onDestroy();
        BroadcastUtility.unregister(this, mReceiver);
    }

    @Override
    public void OnRefreshSuccess() {
        if (swipe.isRefreshing()) {
            swipe.setRefreshing(false);
        }

        setData();
    }

    @Override
    public void OnRefreshErr() {
        swipe.setRefreshing(false);
    }
}